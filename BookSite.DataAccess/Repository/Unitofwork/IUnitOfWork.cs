using BookSite.DataAccess.Repository.Category;
using BookSite.DataAccess.Repository.CompanyRepository;
using BookSite.DataAccess.Repository.ProductRepository;
using BookSite.DataAccess.Repository.ShoppinCartRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.Unitofwork
{
    public interface IUnitOfWork
    {
        ICategoryRepo CategoryRepo { get;  }
        IProductRepo ProductRepo { get;  }
        ICompanyRepo CompanyRepo { get;  }
        IShoppinCartRepo ShoppinCartRepo { get;  }

        void savechanges();
    }
}
