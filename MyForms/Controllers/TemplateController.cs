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

        [Authorize(Roles = AppConsts.AdminRole)]
        public async Task<IActionResult> Index()
        {
            var templates = await templateService.GetAllTemplatesAsync(includeTopic: true, includeAuthor: true);
            return View(templates.Select(x=>new TemplateVM()
            {
                Template=x
            }).ToList());
        }

        [TypeFilter(typeof(TemplateAccessFilter))]
        public async Task<IActionResult> Statistics(int templateId)
        {
            var templateStatisticsVM = await templateService.GetTemplateStatisticsAsync(templateId);
            return View(templateStatisticsVM);
        }

        [TypeFilter(typeof(TemplateAccessFilter))]
        public async Task<IActionResult> Forms(int templateId)
        {
            var template= await templateService.GetTemplateAsync(x => x.Id == templateId,includeFormsAndUser:true);
           
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
            await templateService.RemoveTemplatesAsync(templatesToRemove);
            return RedirectToAction("Index", "Home");

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
            string userId= User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await templateService.CreateAsync(templateCreateVM, userId);
            return RedirectToAction("Index", "Home");

        }

        [AllowAnonymous]
        [TypeFilter(typeof(TemplateAccessFilter))]
        public async Task<IActionResult> Details(int templateId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var templateVM = await templateService.GetTemplateDetailsAsync(templateId, userId);
            if (templateVM is null)
            {
                return NotFound();
            }
            return View(templateVM);
        }

        [TypeFilter(typeof(OwnerOrAdminFilter))]
        [TypeFilter(typeof(TemplateAccessFilter))]
        public async Task<IActionResult> Update(int templateId)
        {
            var template = await templateService.GetTemplateAsync(x => x.Id == templateId,
                includeTemplateTagsAndTag: true, includeTemplateUsersAndUser: true);                
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
        [TypeFilter(typeof(OwnerOrAdminFilter))]
        [TypeFilter(typeof(TemplateAccessFilter))]
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
        [TypeFilter(typeof(TemplateAccessFilter))]
        public async Task<IActionResult> QuestionsUpdate(int templateId)
        {
            var questions = await templateService.GetQuestionsAsync(x => x.TemplateId == templateId,includeAnswerOptions:true);
            TemplateQuestionUpdateVM questionUpdateVM = new()
            {
                TemplateId = templateId,
                Questions = questions.OrderBy(x=>x.OrderIndex).ToList()
            };
            return View(questionUpdateVM);
        }

        [HttpPost]
        [TypeFilter(typeof(OwnerOrAdminFilter))]
        [TypeFilter(typeof(TemplateAccessFilter))]
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
