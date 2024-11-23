using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MovieXReview.Models;
using System.Reflection;

namespace MovieXReview.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Tag> Tags { get; set; }

        public DbSet<MovieImage> Images { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<Viewer> Viewers { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // It is a method call that ensures default config from IdentityDbContext are applied to the model
            // I had an error where it said primary key IdentityUserLogin is not defined 
            // Therefore this method ensuring that identity-related entities are applied
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Many to Many relationship here,setting the foreign keys
            modelBuilder.Entity<Movie>()
            .HasMany(p => p.Tags)
            .WithMany(t => t.Movies);
        }
    }
}
