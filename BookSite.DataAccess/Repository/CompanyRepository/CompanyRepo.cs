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
    public class CompanyRepo : Repository<Company> , ICompanyRepo
    {
        private readonly Context _Context;

        public CompanyRepo(Context Context) : base(Context)
        {

            _Context = Context;

        }

        public void Update(Company obj)
        {
            _Context.Update(obj);

            
        }
    }
}
