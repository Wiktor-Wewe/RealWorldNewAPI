using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealWorldNew.DAL.Entities;

namespace RealWorldNewAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Article { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Articles
            modelBuilder.Entity<Article>()
                .Property(a => a.Title)
                .IsRequired();

            modelBuilder.Entity<Article>()
                .Property(a => a.Topic)
                .IsRequired();

            modelBuilder.Entity<Article>()
                .HasMany(t => t.Tags)
                .WithMany(a => a.Articles);


            //Comments
            modelBuilder.Entity<Comment>()
                .Property(c => c.Text)
                .IsRequired();

            //Tags
            modelBuilder.Entity<Tag>()
                .Property(t => t.Name)
                .IsRequired();

            modelBuilder.Entity<Tag>()
                .HasMany(a => a.Articles)
                .WithMany(t => t.Tags);

            //Users
            modelBuilder.Entity<User>()
                .Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(25);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Articles)
                .WithOne(u => u.Author);

            modelBuilder.Entity<User>()
                .HasMany(u => u.FollowedUsers);

            modelBuilder.Entity<User>()
                .HasMany(u => u.LikedArticles)
                .WithOne();

            modelBuilder.Entity<User>()
                .HasMany(u => u.FollowedArticles)
                .WithOne();

            base.OnModelCreating(modelBuilder);

        }
    }
}