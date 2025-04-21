using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(string? includeproperty = null);
        T Get(Expression<Func<T ,bool>> Filterv , string? includeproperty = null);
        void Add(T entity);
        void Remove(T entity);
        
        void RemoveRange(IEnumerable<T> entity);
       

    }
}
