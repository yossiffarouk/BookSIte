
using BookSite.DataAccess.Repository.IRepository;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.ProductImageRepository
{
    public interface IProductImageRepo : IRepository<ProductImage>
    {
        void Update(ProductImage obj);
    }
}
