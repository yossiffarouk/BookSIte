using BookSite.DataAccess.Repository.IRepository;
using BookSite.DataAccess.Repository.ProductImageRepository;
using BookSIte.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.ProductImageRepository
{
    public class ProductImageRepo : Repository<ProductImage> , IProductImageRepo
    {
        private readonly Context _Context;
        
        public ProductImageRepo(Context Context) : base(Context) 
        {
            _Context = Context;
            
        }

        public void Update(ProductImage obj)
        {
            _Context.Update(obj);
        }
    }
}
