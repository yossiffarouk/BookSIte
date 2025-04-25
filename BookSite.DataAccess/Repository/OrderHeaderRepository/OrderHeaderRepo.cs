using BookSite.DataAccess.Repository.IRepository;
using BookSite.DataAccess.Repository.OrderHeaderRepository;
using BookSIte.Data;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.OrderHeaderRepository
{
    public class OrderHeaderRepo : Repository<OrderHeader> , IOrderHeaderRepo
    {
        private readonly Context _Context;

        public OrderHeaderRepo(Context Context) : base(Context)
        {

            _Context = Context;

        }

        public void Update(OrderHeader obj)
        {
            _Context.Update(obj);

            
        }
    }
}
