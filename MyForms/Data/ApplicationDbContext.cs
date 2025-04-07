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
            builder.Entity<TemplateTag>().HasIndex(x => new { x.TemplateId, x.TagId }).IsUnique(true);
            builder.Entity<TemplateTag>().HasOne(x => x.Template).WithMany(x => x.TemplateTags).HasForeignKey(x => x.TemplateId);
            builder.Entity<TemplateTag>().HasOne(x => x.Tag).WithMany(x => x.TemplateTags).HasForeignKey(x => x.TagId);

            builder.Entity<TemplateUserAccess>().HasIndex(x => new { x.TemplateId, x.UserId }).IsUnique(true);
            builder.Entity<TemplateUserAccess>().HasOne(x => x.Template).WithMany(x => x.TemplateUsers).HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<TemplateUserAccess>().HasOne(x => x.ApplicationUser).WithMany(x => x.TemplateUsers).HasForeignKey(x => x.UserId);
            
            builder.Entity<Like>().HasIndex(x => new { x.TemplateId, x.UserId }).IsUnique(true);
            builder.Entity<Like>().HasOne(x => x.Template).WithMany(x => x.Likes).HasForeignKey(x => x.TemplateId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Like>().HasOne(x => x.ApplicationUser).WithMany(x => x.Likes).HasForeignKey(x => x.UserId);

            builder.Entity<Models.Comment>().HasOne(x => x.Template).WithMany(x => x.Comments).HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);
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

    }

}
