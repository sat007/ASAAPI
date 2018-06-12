namespace ASA.Core.Infrastructure
{
    public class DataBaseFactory : Disposable, IDataBaseFactory
    {
        private ASAEntitiesContext _dataContext;

        public ASAEntitiesContext Get()
        {
            var d = _dataContext ?? (_dataContext = new ASAEntitiesContext());
            return d;
        }

        protected override void DisposeCore()
        {
            if (_dataContext != null)
            {
                _dataContext.Dispose();
            }
        }
    }
}
