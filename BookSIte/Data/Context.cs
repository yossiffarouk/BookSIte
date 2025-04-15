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
    }
}
