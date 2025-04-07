using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using MyForms.Models;
using MyForms.Models.VM;
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

        public async Task<IActionResult> Index()
        {
            return View(await db.Templates.Include(x=>x.Topic).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            var templateVm = new TemplateCreateVM()
            {
                Topics = await templateService.GetTopicSelectListAsync()
            };
            return View(templateVm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TemplateCreateVM templateCreateVM)
        {
            if (!ModelState.IsValid)
            {
                templateCreateVM = new TemplateCreateVM()
                {
                    Topics = await templateService.GetTopicSelectListAsync()
                };
                return View(templateCreateVM);
            }
            Template template = templateCreateVM.Template;
            string userId= User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            template.CreatedById = userId;
            await db.Templates.AddAsync(template);
            await db.SaveChangesAsync();
            if (template.IsPrivate && templateCreateVM.Emails.Any())
            {               
                await templateService.AssignUserAccessAsync(templateCreateVM.Emails, template.Id);
            }
            if (templateCreateVM.Tags.Any())
            {
                await templateService.AssignTagsAsync(templateCreateVM.Tags, template.Id);
            }
            return RedirectToAction(nameof(Index));
        }

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
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if(await db.Likes.FirstOrDefaultAsync(x => x.UserId == userId && x.TemplateId==templateId) is not null)
            {
                templateVM.LikeVM.IsLiked= true;
            }
            ViewBag.templateId = templateId;
            return View(templateVM);
        }

        public async Task<IActionResult> Update(int templateId)
        {
            ViewBag.templateId = templateId;
            return View();
        }

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
        public async Task<IActionResult> CheckEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Ok();
            }
            bool isExist = await db.ApplicationUsers.AnyAsync(x=>x.Email==email);
            return Json(new { isExist });
        }        

        [HttpPost("addComment")]
        public  async Task<IActionResult> AddComment([FromBody] CommentRequest comment)
        {
            var user = await db.Users.FirstOrDefaultAsync(x=>x.Id== User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Comment templateComment = new()
            {
                TemplateId = comment.TemplateId,
                Text = comment.Text,
                UserId = user!.Id
            };
            await db.Comments.AddAsync(templateComment);
            await db.SaveChangesAsync();
            int commentsCount= await db.Comments.CountAsync(x => x.TemplateId == comment.TemplateId);
            return Ok(new {text=comment.Text, author=user.FullName, commentsCount });
        }
        [HttpPost("addLike")]
        public async Task<IActionResult> AddLike([FromBody] LikeRequest likeModel)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            Like? templateLike = await db.Likes.FirstOrDefaultAsync(x => x.UserId == userId && x.TemplateId == likeModel.TemplateId);            
            bool isLiked=templateLike ==null;
            if (templateLike is null)
            {
                templateLike = new()
                {
                    UserId = userId,
                    TemplateId = likeModel.TemplateId,                    
                };
                await db.Likes.AddAsync(templateLike);
            }
            else
            {              
                db.Likes.Remove(templateLike);
            }
            await db.SaveChangesAsync();
            int likesCount = await db.Likes.CountAsync(x => x.TemplateId == likeModel.TemplateId);
            return Ok(new { isLiked , likesCount });
        }
        #endregion
    }
}
public class LikeRequest
{
    public int TemplateId { get; set; }
}
public class CommentRequest
{
    public int TemplateId { get; set; }
    public string Text { get; set; }
}