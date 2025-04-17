using BookSite.DataAccess.Repository.IRepository;
using BookSIte.Data;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.ProductRepository
{
    public class ProductRepo : Repository<Product> , IProductRepo
    {
        private readonly Context _Context;

        public ProductRepo(Context Context) : base(Context)
        {

            _Context = Context;

        }

    }
}
