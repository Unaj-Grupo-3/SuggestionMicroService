
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Suggestion> Suggestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Suggestion>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();
                entity.Property(c => c.MainUser).IsRequired();
                entity.Property(c => c.SuggestedUser).IsRequired();
                entity.Property(c => c.DateView).IsRequired(false);
                entity.Property(c => c.View).IsRequired().HasDefaultValue(false);
            });
        }
    }
}
