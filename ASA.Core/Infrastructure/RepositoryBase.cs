using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core.Infrastructure
{
    public class RepositoryBase<T> : IRepository<T> where T : class
    {
        private ASAEntitiesContext _dataContext;
        private readonly IDbSet<T> _dbset;

        protected IDataBaseFactory DatabaseFactory { get; private set; }
        protected ASAEntitiesContext DataContext {get { return _dataContext ?? (_dataContext = DatabaseFactory.Get());}}

        public RepositoryBase(IDataBaseFactory databasefactory)
        {
            DatabaseFactory = databasefactory;
            _dbset = DataContext.Set<T>();
           
        }
        public virtual async Task<T> AddAsyn(T entity)
        {
            _dbset.Add(entity);
            await _dataContext.SaveChangesAsync();
            return entity;
        }
        public virtual T Add(T entity)
        {
            _dbset.Add(entity);
            return entity;
        }
        public virtual void Update(T entity)
        {
            _dbset.Attach(entity);
            _dataContext.Entry(entity).State = EntityState.Modified;
            
        }
       
        public virtual void Delete(T entity)
        {
            _dbset.Remove(entity);
               
        }
        public void Commit()
        {
            DataContext.SaveChanges();
        }
       
        public virtual T GetById(int id)
        {
            return _dbset.Find(id);
        }
        //public virtual IEnumerable<T> GetAll()
        //{
        //    return _dbset.ToList();
        //}

        public virtual IQueryable<T> Query()
        {
            return _dbset.AsQueryable<T>();
            
            //IQueryable<T> query = _entities.Set<T>();
            //return query;
        }
        public virtual IQueryable<T> GetAll()
        {
            return _dbset.AsQueryable<T>();
        }

        public IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> queryable = GetAll();
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include<T, object>(includeProperty);
            }
            return queryable;
        }


    }
}
