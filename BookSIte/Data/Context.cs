using BookSIte.Models;
using Microsoft.EntityFrameworkCore;

namespace BookSIte.Data
{
    public class Context : DbContext
    {

        public Context(DbContextOptions<Context> options) : base(options)
        {
                
        }

        public DbSet<Category> Category { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1 , Name = "Action", OrderNumber = 1 },
                new Category { Id = 2, Name = "History", OrderNumber = 2 }
                );
        }
    }
}
