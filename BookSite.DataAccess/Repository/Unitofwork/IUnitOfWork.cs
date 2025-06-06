﻿using BookSite.DataAccess.Repository.ApplicationRepository;
using BookSite.DataAccess.Repository.CategoryRepository;
using BookSite.DataAccess.Repository.CompanyRepository;
using BookSite.DataAccess.Repository.OrderDetailRepository;
using BookSite.DataAccess.Repository.OrderHeaderRepository;
using BookSite.DataAccess.Repository.ProductImageRepository;
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
        IOrderHeaderRepo OrderHeaderRepo { get;  }
        IOrderDetailRepo OrderDetailRepo { get;  }
        IApplicationRepo ApplicationRepo { get;  }
        IProductImageRepo ProductImageRepo { get;  }

        void savechanges();
    }
}
