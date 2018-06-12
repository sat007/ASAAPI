using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDataBaseFactory databasefactory;
        private ASAEntitiesContext dataContext;

        protected ASAEntitiesContext DataContext {get { return dataContext ?? (dataContext = databasefactory.Get()); }}

        public UnitOfWork(IDataBaseFactory dataBaseFactory)
        {
            this.databasefactory = dataBaseFactory;
        }

        public void Commit()
        {
            DataContext.SaveChanges();
        }
    }
}
