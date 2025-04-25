using BookSite.DataAccess.Repository.IRepository;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.OrderDetailRepository
{
    public interface IOrderDetailRepo : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
    }
}
