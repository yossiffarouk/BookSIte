
using BookSite.DataAccess.Repository.IRepository;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.Category
{
    public interface ICategoryRepo : IRepository<BookStore.Models.Category>
    {
        void Update(BookStore.Models.Category category); // =
                                                         // > look here BookStore.Models
    }
}
