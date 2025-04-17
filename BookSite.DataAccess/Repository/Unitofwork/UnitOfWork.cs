using BookSite.DataAccess.Repository.Category;
using BookSite.DataAccess.Repository.ProductRepository;
using BookSIte.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.Unitofwork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _Context;
        public ICategoryRepo CategoryRepo { get; private set; }
        public IProductRepo ProductRepo { get; private set; }

        public UnitOfWork(Context Context)
        {

        
            _Context = Context;
            CategoryRepo = new CategoryRepo(Context);
            ProductRepo = new ProductRepo(Context);

        }


        public void savechanges()
        {
            _Context.SaveChanges();
        }
    }
}
