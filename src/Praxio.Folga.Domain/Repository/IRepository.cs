using BgmRodotec.Framework.Domain.Core.Model;
using NHibernate;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Praxio.Folga.Domain.Repository
{
    public interface IRepository<TEntity> : IDisposable where TEntity : IEntity
    {
        ISession Session { get; }

        IQueryable<TResult> Obter<TResult>();

        IQueryable<TResult> Obter<TResult>(
                    Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> Obter();

        IQueryable<TEntity> Obter(
            Expression<Func<TEntity, bool>> predicate);

        IQueryable<TResult> Obter<TResult>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TResult>> selector);

        int ObterId(Expression<Func<TEntity, bool>> predicate);

        TEntity Obter(int id);

        TResult Obter<TResult>(
            int id,
            Expression<Func<TEntity, TResult>> selector);

        bool Exists(Expression<Func<TEntity, bool>> predicate);

        int Count(Expression<Func<TEntity, bool>> predicate);

        void Inserir(params TEntity[] entity);

        void Alterar(params TEntity[] entity);

        void Excluir(params int[] id);

        void Excluir(params TEntity[] entity);

        void InserirOuAlterar(params TEntity[] entity);

        [Obsolete("EM DESENVOLVIMENTO")]
        void Alterar(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> selector);

        [Obsolete("EM DESENVOLVIMENTO")]
        void Alterar(
            Expression<Func<TEntity, bool>> predicate,
            params (Expression<Func<TEntity, object>> propriedade, object valor)[] propriedades);

        [Obsolete("EM DESENVOLVIMENTO")]
        void Alterar(
            Expression<Func<TEntity, bool>> predicate,
            Action<IAlterarPropriedade<TEntity>> selector)
            ;

        [Obsolete("EM DESENVOLVIMENTO")]
        void Alterar(
            Expression<Func<TEntity, bool>> predicate,
            Action<dynamic> propriedade)
            ;
    }

    public interface IAlterarPropriedade<TEntity> : IDisposable where TEntity : IEntity
    {
        (string Nome, dynamic value)[] Propriedades { get; }
        void Propriedade<TKey>(Expression<Func<TEntity, TKey>> propriedade, TKey valor);
    }
}
