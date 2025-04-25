using BookSite.DataAccess.Repository.IRepository;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.OrderHeaderRepository
{
    public interface IOrderHeaderRepo : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
    }
}
