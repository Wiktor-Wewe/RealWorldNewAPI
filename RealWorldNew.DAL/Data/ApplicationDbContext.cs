using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealWorldNew.DAL.Entities;

namespace RealWorldNewAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext
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
            
            modelBuilder.Entity<Article>()
                .Property(a => a.Title)
                .IsRequired();

            modelBuilder.Entity<Article>()
                .Property(a => a.Topic)
                .IsRequired();

            modelBuilder.Entity<Article>()
                .HasMany<Tag>(t => t.Tags)
                .WithMany(a => a.Articles);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Author);


            modelBuilder.Entity<Comment>()
                .Property(c => c.Text)
                .IsRequired();


            modelBuilder.Entity<Tag>()
                .Property(t => t.Name)
                .IsRequired();

            modelBuilder.Entity<Tag>()
                .HasMany<Article>(a => a.Articles)
                .WithMany(t => t.Tags);


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

            base.OnModelCreating(modelBuilder);

        }
    }
}