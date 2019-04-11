using BgmRodotec.Framework.Domain.Core.Commands;
using NHibernate;
using Praxio.Folga.Domain.Interfaces;
using Praxio.Folga.Infra.Data.Filters;
using System;

namespace Praxio.Folga.Infra.Data.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private ITransaction _transaction;

        public UnitOfWork(ISessionFactory sessionFactory)
        {
            //TODO: Automatico
            TenantId = 1;
            Session = sessionFactory.OpenSession();

            //Session.EnableFilter(typeof(TenantIdFilter).Name).SetParameter("tenantId", TenantId);

            Session.EnableFilter(nameof(DataHoraExclusaoFilter));

            _transaction = Session.BeginTransaction();
        }

        public int TenantId { get; }

        public ISession Session { get; }

        public CommandResponse Commit()
        {
            try
            {
                if (_transaction.IsActive)
                    _transaction.Commit();
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex);
                if (_transaction.IsActive)
                    _transaction.Rollback();
            }

            var comitted = _transaction.WasCommitted;

            _transaction = Session.BeginTransaction();

            return new CommandResponse(comitted);
        }

        public void Rollback()
        {
            if (_transaction.IsActive)
                _transaction.Rollback();
        }

        public void Dispose()
        {
            Session.Dispose();
        }
    }
}
