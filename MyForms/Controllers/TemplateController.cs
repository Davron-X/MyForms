using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using MyForms.Filters;
using MyForms.Models;
using MyForms.Models.DTOs;
using MyForms.Models.VM.TemplateVMs;
using MyForms.Services.Interfaces;
using System.Security.Claims;

namespace MyForms.Controllers
{
    [Authorize]
    public class TemplateController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly ITemplateService templateService;
        public TemplateController(ApplicationDbContext db, ITemplateService templateService)
        {
            this.db = db;
            this.templateService = templateService;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await db.Templates.Include(x=>x.Topic).Select(x=>new TemplateVM() { Template = x }).ToListAsync());
        }

        public async Task<IActionResult> Statistics(int templateId)
        {
            var template = await  db.Templates.Include(x => x.Questions).ThenInclude(x => x.FormAnswers)
                .Include(x => x.Forms)
                .FirstOrDefaultAsync(x=>x.Id==templateId);       


            if (template is null)
            {
                return NotFound();
            }
            List<QuestionStatisticsVM> questionStatistics = new();
            foreach (var question in template.Questions)
            {
                if (question.Type==QuestionType.Numeric)
                {
                    var result= question.FormAnswers.Select(x => double.TryParse(x.AnswerText, out double answer) ? answer : (double?)null)
                        .Where(x=>x.HasValue).ToList();
                    questionStatistics.Add(new QuestionStatisticsVM()
                    {
                        QuestionId = question.Id,
                        QuestionText = question.Text,
                        Type = question.Type,
                        AverageValue = result.Any() ? result.Average() : null
                    });
                }
                else
                {
                    var mostFrequent= question.FormAnswers.GroupBy(x => x.AnswerText).OrderByDescending(g => g.Count())
                        .FirstOrDefault();
                    questionStatistics.Add(new QuestionStatisticsVM()
                    {
                        QuestionText = question.Text,
                        QuestionId = question.Id,
                        Type = question.Type,
                        MostFrequentAnswer=mostFrequent?.Key,
                        Frequency = mostFrequent?.Count() ?? 0,
                        OptionFrequencies = question.FormAnswers
                            .GroupBy(a => a.AnswerText).Take(5)
                            .ToDictionary(g => g.Key, g => g.Count())
                    });
                }
            }

            return View(questionStatistics);
        }

        public async Task<IActionResult> Forms(int templateId)
        {
            var template =await db.Templates.Include(x=>x.Forms).ThenInclude(x=>x.ApplicationUser)
                .FirstOrDefaultAsync(x => x.Id == templateId);
            if (template is null)
            {
                return NotFound();
            }
            return View(template);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(IEnumerable<TemplateVM> templateVMs)
        {
            var templatesToRemove = templateVMs.Where(x => x.IsChecked).Select(x => x.Template.Id).ToList();          
            await db.Templates.Where(x => templatesToRemove.Contains(x.Id)).ExecuteDeleteAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Create()
        {
            var templateVm = new TemplateCreateVM()
            {
                TemplateSettingVM = new()
                {
                    Topics = await templateService.GetTopicSelectListAsync()
                }
            };
            return View(templateVm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TemplateCreateVM templateCreateVM)
        {
            ModelState.Remove("Template.Id");
            if (!ModelState.IsValid)
            {               
                templateCreateVM = new TemplateCreateVM()
                {
                    TemplateSettingVM = new()
                    {
                        Topics = await templateService.GetTopicSelectListAsync()
                    }
                };
                return View(templateCreateVM);
            }
            Template template = templateCreateVM.Template;
            string userId= User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            template.CreatedById = userId;
            await db.Templates.AddAsync(template);
            await db.SaveChangesAsync();
            if (templateCreateVM.Questions.Any())
            {
                await templateService.AddQuestions(templateCreateVM.Questions, template.Id);
            }
            if (template.IsPrivate && templateCreateVM.Emails.Any())
            {               
                await templateService.AssignUserAccessAsync(templateCreateVM.Emails, template.Id);
            }
            if (templateCreateVM.TagNames.Any())
            {
                await templateService.AssignTagsAsync(templateCreateVM.TagNames, template.Id);
            }
            if (templateCreateVM.Template.Image is not null)
            {
               template.ImageUrl= await templateService.UploadImageAsync(templateCreateVM.Template.Image);
                await db.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int templateId)
        {
            var template = await db.Templates.Include(x => x.Topic)
                .Include(x => x.ApplicationUser)
                .Include(x => x.Questions).ThenInclude(x => x.AnswerOptions)
                .Include(x => x.TemplateTags).ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync(x=>x.Id==templateId);
            if (template is null)
            {
                return NotFound();
            }
            TemplateDetailsVM templateVM = new()
            {
                Template = template,
                LikeVM = new()
                {
                    TemplateId = template.Id,
                    LikesCount = await db.Likes.Where(x => x.TemplateId == templateId).CountAsync()
                },
                CommentsVM = new()
                {
                    TemplateId = template.Id,
                    Comments = await db.Comments.Where(x => x.TemplateId == templateId).ToListAsync()
                }
            };
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null &&  await db.Likes.FirstOrDefaultAsync(x => x.UserId == userId && x.TemplateId==templateId) is not null)
            {
                templateVM.LikeVM.IsLiked= true;
            }            
            if (userId != null && userId==template.CreatedById)
            {
                templateVM.IsOwner = true;
            }
            return View(templateVM);
        }
        [TypeFilter(typeof(OwnerOrAdminFilter))]
        public async Task<IActionResult> Update(int templateId)
        {
            var template = await db.Templates.Include(x => x.TemplateUsers).ThenInclude(x => x.ApplicationUser)
                .Include(x=>x.TemplateTags).ThenInclude(x=>x.Tag)
                .FirstOrDefaultAsync(x => x.Id == templateId);                
            if (template is null)
            {
                return NotFound();
            }         
            TemplateUpdateVM templateUpdateVM = new()
            {
                TemplateSettingVM = new()
                {
                    Template= template,
                    Topics =await templateService.GetTopicSelectListAsync()
                }
            };
            return View(templateUpdateVM);       
        }

        [HttpPost]
        public async Task<IActionResult> Update(TemplateUpdateVM updateVM)
        {
            if (!ModelState.IsValid)
            {
                return View(updateVM);
            }
            await templateService.UpdateTemplateAsync(updateVM.Template);
            await templateService.UpdateTagsAsync(updateVM.TagNames, updateVM.Template.Id);
            if (updateVM.Template.IsPrivate)
            {
                await templateService.UpdateUserAccessAsync(updateVM.Emails, updateVM.Template.Id);
            }
            return RedirectToAction(nameof(Details),new {templateId=updateVM.Template.Id});
        }
        [TypeFilter(typeof(OwnerOrAdminFilter))]
        public async Task<IActionResult> QuestionsUpdate(int templateId)
        {
            var questions = await db.Questions.Include(x => x.AnswerOptions).Where(x => x.TemplateId == templateId).ToListAsync();
            TemplateQuestionUpdateVM questionUpdateVM = new()
            {
                TemplateId = templateId,
                Questions = questions
            };
            return View(questionUpdateVM);
        }

        [HttpPost]
        public async Task<IActionResult> QuestionsUpdate(TemplateQuestionUpdateVM questionUpdateVM)
        {
            if (!ModelState.IsValid)
            {
                return View(questionUpdateVM);
            }
            await templateService.UpdateQuestionsAsync(questionUpdateVM.Questions,questionUpdateVM.TemplateId);
            return RedirectToAction(nameof(Details),new {templateId= questionUpdateVM.TemplateId});

        }

        #region Like's & Comment's Api's

        [HttpPost("addComment")]
        public async Task<IActionResult> AddComment([FromBody] CreateCommentDto comment)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Forbid();
            }
            var user = await db.ApplicationUsers.FirstOrDefaultAsync(x => x.Id ==userId);
            Comment templateComment = new()
            {
                TemplateId = comment.TemplateId,
                Text = comment.Text,
                UserId = user!.Id
            };
            await db.Comments.AddAsync(templateComment);
            await db.SaveChangesAsync();
            int commentsCount = await db.Comments.CountAsync(x => x.TemplateId == comment.TemplateId);
            return Ok(new { text = comment.Text, author = user.FullName, commentsCount });
        }

        [HttpPost("addLike")]
        public async Task<IActionResult> AddLike([FromBody] CreateLikeDto likeDto)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return Forbid();
            }
            Like? templateLike = await db.Likes.FirstOrDefaultAsync(x => x.UserId == userId && x.TemplateId == likeDto.TemplateId);
            bool isLiked = templateLike == null;
            if (templateLike is null)
            {
                templateLike = new()
                {
                    UserId = userId,
                    TemplateId = likeDto.TemplateId,
                };
                await db.Likes.AddAsync(templateLike);
            }
            else
            {
                db.Likes.Remove(templateLike);
            }
            await db.SaveChangesAsync();
            int likesCount = await db.Likes.CountAsync(x => x.TemplateId == likeDto.TemplateId);
            return Ok(new { isLiked, likesCount });
        }
        #endregion

        #region  API's

        [HttpGet("tags")]
        public async Task<IActionResult> SearchTags(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Ok();
            }
            List<string> tags= await db.Tags.Where(x => x.Name.StartsWith(query)).Select(x=>x.Name).Take(10).ToListAsync();
            return Json(tags);
        }
        [HttpGet("emails")]
        public async Task<IActionResult> SearchEmails(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Ok();
            }
            List<string?> emails = await db.ApplicationUsers.Where(x => x.Email!.StartsWith(query)).Select(x => x.Email).Take(10).ToListAsync();
            return Json(emails);
        }

        [HttpGet("checkEmail")]
        public async Task<IActionResult> CheckEmail(string email,int templateId=0)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Ok();
            }
            email = email.ToUpper();
            
            bool isExist = await db.ApplicationUsers.AnyAsync(x=>x.NormalizedEmail==email);
            bool isAllowed=false;
            if (isExist && templateId != 0)
            {
                isAllowed= await db.TemplateUserAccesses.Include(x => x.ApplicationUser)
                    .AnyAsync(x => x.ApplicationUser.NormalizedEmail == email && x.TemplateId==templateId);
                return Json(new { isExist,isAllowed ,message="Пользователь с таким email уже добавлен"});
            }
            return Json(new { isExist, isAllowed, message = "Пользователь с таким email не существует" });
        }        

        
        #endregion
    }
}
