using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core.Infrastructure
{
    public interface IRepository<T> where T: class
    {
        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        T GetById(int id);
        //IEnumerable<T> GetAll();
        IQueryable<T> Query();

        //ading aynch methods 
        IQueryable<T> GetAll();
        Task<T>AddAsyn(T entity);
        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);

        //T Get(Expression<Func<T, bool>> where);
        //IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
    }
}
