using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyForms.Data;
using MyForms.Models.VM;
using MyForms.Models;
using MyForms.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Imagekit.Sdk;

namespace MyForms.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ApplicationDbContext db;
        private readonly ImagekitClient imagekit;
        public TemplateService(ApplicationDbContext db, ImagekitClient imagekit)
        {
            this.db = db;
            this.imagekit = imagekit;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {

            // 2. Чтение файла в массив байтов
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            // 3. Загрузка через ImageKit
            var uploadResponse = await imagekit.UploadAsync(new FileCreateRequest
            {
                file = Convert.ToBase64String(fileBytes),
                fileName = Guid.NewGuid() + Path.GetExtension(file.FileName),
                folder = "/templates" // Опционально
            });

            var imageUrl = uploadResponse.url;
            return imageUrl;
        }

        public async Task<List<Tag>> AddTags(IEnumerable<string> tags)
        {
            List<Tag> existingTags = await db.Tags.Where(x => tags.Contains(x.Name)).ToListAsync();
            var newTagNames = tags.Except(existingTags.Select(x => x.Name)).ToList();
            var newTags = newTagNames.Select(x => new Tag() { Name = x }).ToList();
            if (newTags.Any())
            {
                await db.Tags.AddRangeAsync(newTags);
                await db.SaveChangesAsync();
            }
            existingTags = existingTags.Concat(newTags).ToList();
            return existingTags;
        }
        public async Task AssignTagsAsync(IEnumerable<string> tagsName, int templateId)
        {
            var tags = await AddTags(tagsName);
            List<TemplateTag> templateTags = tags.Select(x => new TemplateTag()
            {
                TemplateId = templateId,
                TagId = x.Id
            }).ToList();
            await db.TemplateTags.AddRangeAsync(templateTags);
            await db.SaveChangesAsync();
        }

        public async Task AssignUserAccessAsync(IEnumerable<string> emails, int templateId)
        {
            var userIds = await db.ApplicationUsers.Where(x => emails.Contains(x.Email)).Select(x => x.Id).ToListAsync();
            List<TemplateUserAccess> userAccesses = userIds.Select(x => new TemplateUserAccess()
            {
                UserId = x,
                TemplateId = templateId
            }).ToList();
            await db.TemplateUserAccesses.AddRangeAsync(userAccesses);
            await db.SaveChangesAsync();
        }

        public async Task<List<SelectListItem>> GetTopicSelectListAsync()
        {
            return await db.Topics.Select(x => new SelectListItem(x.Name, x.Id.ToString(), x.Id == 1)).ToListAsync();
        }
        public async Task UpdateTagsAsync(IEnumerable<string> tagNames, int templateId)
        {
            List<string> templateTagsNames = await db.TemplateTags.Include(x => x.Tag)
                 .Where(x => x.TemplateId == templateId).Select(x => x.Tag.Name).ToListAsync();
            var removedTagNames = templateTagsNames.Except(tagNames);
            var tagsToRemove = await db.TemplateTags.Where(x => removedTagNames.Contains(x.Tag.Name) && x.TemplateId == templateId).ToListAsync();
            if (tagsToRemove.Any())
            {
                db.TemplateTags.RemoveRange(tagsToRemove);
                await db.SaveChangesAsync();
            }
            var tagstoAdd = tagNames.Except(templateTagsNames);
            if (tagstoAdd.Any())
            {
                await AssignTagsAsync(tagstoAdd, templateId);
            }
        }
        public async Task UpdateUserAccessAsync(IEnumerable<string> emails, int templateId)
        {
            List<string?> templateEmails = await db.TemplateUserAccesses.Include(x => x.ApplicationUser)
                .Where(x => x.TemplateId == templateId).Select(x => x.ApplicationUser.Email).ToListAsync();
            var removedEmails = templateEmails.Except(emails);
            var usersToRemove = await db.TemplateUserAccesses.Include(x => x.ApplicationUser)
                .Where(x => removedEmails.Contains(x.ApplicationUser.Email) && x.TemplateId == templateId).ToListAsync();
            if (usersToRemove.Any())
            {
                db.TemplateUserAccesses.RemoveRange(usersToRemove);
                await db.SaveChangesAsync();
            }
            var emailsToAdd = emails.Except(templateEmails);
            if (emailsToAdd.Any())
            {
                await AssignUserAccessAsync(emailsToAdd, templateId);
            }
        }
        public async Task UpdateTemplateAsync(Template template)
        {
            var templateFromDb = await db.Templates.FirstOrDefaultAsync(x => x.Id == template.Id);
            if (templateFromDb is null)
            {
                throw new ArgumentException();
            }
            templateFromDb.Title = template.Title;
            templateFromDb.Description = template.Description;
            templateFromDb.TopicId = template.TopicId;
            templateFromDb.IsPrivate = template.IsPrivate;
            await db.SaveChangesAsync();
        }
        public async Task AddQuestions(IEnumerable<Question> questions, int templateId)
        {
            questions = questions.Select(x =>
            {
                x.TemplateId = templateId;
                return x;
            });
            db.Questions.AddRange(questions);
            await db.SaveChangesAsync();
        }
        public async Task UpdateQuestionsAsync(IEnumerable<Question> questions, int templateId)
        {
            var questionsFromDb = await db.Questions.AsNoTracking().Where(x => x.TemplateId == templateId).ToListAsync();
            var comparer = new QuestionComparer();
            var questionsToRemove = questionsFromDb.Except(questions, comparer);
            if (questionsToRemove.Any())
            {
                db.Questions.RemoveRange(questionsToRemove);
            }
            var newQuestions = questions.Where(x => x.Id == 0);
            if (newQuestions.Any())
            {
                await AddQuestions(newQuestions, templateId);
            }

            var questionsToUpdate = questions.Where(x => x.Id != 0 && !questionsToRemove.Contains(x, comparer));
            db.Questions.UpdateRange(questionsToUpdate);
            await db.SaveChangesAsync();
            await UpdateAnswerOptions(questionsToUpdate);
        }

        public async Task UpdateAnswerOptions(IEnumerable<Question> questions)
        {
            foreach (var question in questions)
            {
                if (question.Type == QuestionType.Checkbox)
                {
                    var options = question.AnswerOptions.ToList();
                    var optionsFromDb = await db.AnswerOptions.Where(x => x.QuestionId == question.Id).ToListAsync();
                    var comparer = new AnswerOptionComparer();

                    var optionsToRemove = optionsFromDb.Except(options, comparer);
                    db.AnswerOptions.RemoveRange(optionsToRemove);

                    var optionsToAdd = options.Except(optionsFromDb, comparer);
                    db.AnswerOptions.AddRange(optionsToAdd);

                    var optionsToUpdate = options.Where(x => x.Id != 0 && !optionsToRemove.Any(o => o.Id == x.Id));
                    db.AnswerOptions.UpdateRange(optionsToUpdate);
                }
            }
                await db.SaveChangesAsync();
        }
    }
    class AnswerOptionComparer : IEqualityComparer<AnswerOption>
    {
      
        public bool Equals(AnswerOption? x, AnswerOption? y)
        {
            if (x is null || y is null)
            {
                return false;
            }
            return x.Id == y.Id;
        }
        public int GetHashCode([DisallowNull] AnswerOption obj)
        {
            return obj.Id.GetHashCode();
        }
    }
    class QuestionComparer : IEqualityComparer<Question>
    {
        public bool Equals(Question? x, Question? y)
        {
            if (x is null || y is null )
            {
                return false;
            }
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] Question obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
