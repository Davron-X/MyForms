using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyForms.Models;

namespace MyForms.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Topic>().HasData(new Topic()
            {
                Id=1,
                Name="Education"
            },
            new Topic()
            {
                Id=2,
                Name= "Quiz"
            },
            new Topic()
            {
                Id=3,
                Name="Other",
            });
            builder.Entity<ApplicationUser>().HasMany(x => x.Templates).WithOne(x => x.ApplicationUser).HasForeignKey(x => x.CreatedById)
         .OnDelete(DeleteBehavior.SetNull);
            builder.Entity<ApplicationUser>().HasMany(x => x.TemplateUsers).WithOne(x => x.ApplicationUser).HasForeignKey(x => x.UserId)
         .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationUser>().HasMany(x => x.Likes).WithOne(x => x.ApplicationUser).HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationUser>().HasMany(x => x.Comments).WithOne(x => x.ApplicationUser).HasForeignKey(x => x.UserId)
       .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationUser>().HasMany(x => x.Forms).WithOne(x => x.ApplicationUser).HasForeignKey(x => x.FilledBy)
       .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Template>().HasMany(x => x.TemplateUsers).WithOne(x => x.Template).HasForeignKey(x => x.TemplateId)
         .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Template>().HasMany(x => x.Likes).WithOne(x => x.Template).HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Template>().HasMany(x => x.Comments).WithOne(x => x.Template).HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Template>().HasMany(x => x.Forms).WithOne(x => x.Template).HasForeignKey(x => x.TemplateId)
        .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TemplateTag>().HasIndex(x => new { x.TemplateId, x.TagId }).IsUnique(true);
            builder.Entity<TemplateTag>().HasOne(x => x.Template).WithMany(x => x.TemplateTags).HasForeignKey(x => x.TemplateId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<TemplateTag>().HasOne(x => x.Tag).WithMany(x => x.TemplateTags).HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TemplateUserAccess>().HasIndex(x => new { x.TemplateId, x.UserId }).IsUnique(true);
                      
            builder.Entity<Like>().HasIndex(x => new { x.TemplateId, x.UserId }).IsUnique(true);       

            builder.Entity<FormAnswer>().HasOne(x => x.Form).WithMany(x => x.FormAnswers).HasForeignKey(x => x.FormId)
              .OnDelete(DeleteBehavior.Cascade);
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<TemplateTag> TemplateTags { get; set; }
        public DbSet<TemplateUserAccess> TemplateUserAccesses { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormAnswer> FormAnswers { get; set; }


    }

}
