using BgmRodotec.Framework.Domain.Core.Commands;
using NHibernate;
using System;

namespace Praxio.Folga.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ISession Session { get; }
        CommandResponse Commit();
        void Rollback();
    }
}
