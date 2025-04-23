using BookSite.DataAccess.Repository.IRepository;
using BookSIte.Data;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.ApplicationRepository
{
    public class ApplicationRepo : Repository<ApplicationsUser> , IApplicationRepo
    {
        private readonly Context _Context;

        public ApplicationRepo(Context Context) : base(Context)
        {

            _Context = Context;

        }

        public void Update(ApplicationsUser obj)
        {
            _Context.Update(obj);

            
        }
    }
}
