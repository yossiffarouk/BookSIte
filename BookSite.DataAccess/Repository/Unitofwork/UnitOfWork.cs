using BookSite.DataAccess.Repository.ApplicationRepository;
using BookSite.DataAccess.Repository.CategoryRepository;
using BookSite.DataAccess.Repository.CompanyRepository;
using BookSite.DataAccess.Repository.OrderDetailRepository;
using BookSite.DataAccess.Repository.OrderHeaderRepository;
using BookSite.DataAccess.Repository.ProductRepository;
using BookSite.DataAccess.Repository.ShoppinCartRepository;
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
        public ICompanyRepo CompanyRepo { get; private set; }
        public IProductRepo ProductRepo { get; private set; }
        public IShoppinCartRepo ShoppinCartRepo { get; private set; }
        public IOrderHeaderRepo OrderHeaderRepo { get; private set; }
        public IOrderDetailRepo OrderDetailRepo { get; private set; }
        public IApplicationRepo ApplicationRepo { get; private set; }

        public UnitOfWork(Context Context)
        {

        
            _Context = Context;
            CategoryRepo = new CategoryRepo(Context);
            ProductRepo = new ProductRepo(Context);
            CompanyRepo = new CompanyRepo(Context);
            ShoppinCartRepo = new ShoppinCartRepo(Context);
            OrderHeaderRepo = new OrderHeaderRepo(Context);
            OrderDetailRepo = new OrderDetailRepo(Context);
            ApplicationRepo = new ApplicationRepo(Context);

        }


        public void savechanges()
        {
            _Context.SaveChanges();
        }
    }
}
