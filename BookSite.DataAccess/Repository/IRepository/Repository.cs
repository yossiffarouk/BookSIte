using BookSIte.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookSite.DataAccess.Repository.IRepository
{
    public class Repository<T> : IRepository<T> where T : class
    {


        private readonly Context _Context;
        internal DbSet<T> dbset { get; set; }
        public Repository(Context Context)
        {
            _Context = Context;
            dbset = _Context.Set<T>();

        }
        public void Add(T entity)
        {
           dbset.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> Filter)
        {
            IQueryable<T> query = dbset;
            
            return query.FirstOrDefault(Filter);

        }

        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbset;
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbset.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbset.RemoveRange(entity);
        }
        public void Update(T entity)
        {
            dbset.Update(entity);
        }

    }

    
}
