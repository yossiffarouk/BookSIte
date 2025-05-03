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

        public void Update(Product obj)
        {
            var product = _Context.Products.FirstOrDefault(a=>a.Id == obj.Id);


            if (product != null)
            {
                product.Title = obj.Title;
                product.Description = obj.Description;
                product.ISBN = obj.ISBN;
                product.CategoryId = obj.CategoryId;
                product.Author = obj.Author;
                product.ListPrice = obj.ListPrice;
                product.Price = obj.Price;
                product.Price50 = obj.Price50;
                product.Price100 = obj.Price100;
                product.ProductImages = obj.ProductImages;

            }
        }
    }
}
