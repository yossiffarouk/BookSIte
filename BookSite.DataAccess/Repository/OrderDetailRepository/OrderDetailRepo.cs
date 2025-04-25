using BookSite.DataAccess.Repository.IRepository;
using BookSIte.Data;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.OrderDetailRepository
{
    public class OrderDetailRepo : Repository<OrderDetail> , IOrderDetailRepo
    {
        private readonly Context _Context;

        public OrderDetailRepo(Context Context) : base(Context)
        {

            _Context = Context;

        }

        public void Update(OrderDetail obj)
        {
            _Context.Update(obj);

            
        }
    }
}
