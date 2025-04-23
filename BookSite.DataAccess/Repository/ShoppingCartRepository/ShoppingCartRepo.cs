using BookSite.DataAccess.Repository.IRepository;
using BookSIte.Data;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.ShoppinCartRepository
{
    public class ShoppinCartRepo : Repository<ShoppigCart> , IShoppinCartRepo
    {
        private readonly Context _Context;

        public ShoppinCartRepo(Context Context) : base(Context)
        {

            _Context = Context;

        }

     
    }
}
