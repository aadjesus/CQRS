using AutoMapper.QueryableExtensions;
using BgmRodotec.Framework.Domain.Core.Model;
using NHibernate;
using NHibernate.Persister.Entity;
using Praxio.Folga.Domain.Interfaces;
using Praxio.Folga.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Praxio.Folga.Infra.Data.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : IEntity
    {
        private readonly IUnitOfWork _unitOfWork;

        public ISession Session => _unitOfWork.Session;

        protected Repository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<TResult> Obter<TResult>()
        {
            var query = Session
                .Query<TEntity>()
                .ProjectTo<TResult>();

            return query;
        }

        public IQueryable<TResult> Obter<TResult>(
            Expression<Func<TEntity, bool>> predicate)
        {
            var query = Session
                .Query<TEntity>()
                .Where(predicate)
                .ProjectTo<TResult>();
            return query;
        }

        public IQueryable<TEntity> Obter()
        {
            var query = Session
                .Query<TEntity>();

            return query;
        }

        public IQueryable<TEntity> Obter(
            Expression<Func<TEntity, bool>> predicate)
        {
            var query = Session
                .Query<TEntity>()
                .Where(predicate);

            return query;
        }

        public TEntity Obter(int id)
        {
            var query = Obter(o => o.Id == id)
                .ToList();

            return query
                .FirstOrDefault();
        }

        public TResult Obter<TResult>(
            int id,
            Expression<Func<TEntity, TResult>> selector)
        {
            var query = Session
                .Query<TEntity>()
                .Where(w => w.Id == id)
                .Select(selector)
                .ToList();

            return query
                .FirstOrDefault();
        }

        public IQueryable<TResult> Obter<TResult>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TResult>> selector)
        {
            var query = Session
                .Query<TEntity>()
                .Where(predicate)
                .Select(selector);

            return query;
        }

        public int ObterId(
           Expression<Func<TEntity, bool>> predicate)
        {
            var query = Session
                .Query<TEntity>()
                .Where(predicate)
                .Select(s => s.Id);

            var retrono = query.ToList();
            if (retrono.Any())
                return retrono.FirstOrDefault();

            return 0;
        }

        public bool Exists(
            Expression<Func<TEntity, bool>> predicate)
        {
            return Count(predicate) > 0;
        }

        public int Count(
            Expression<Func<TEntity, bool>> predicate)
        {
            return Session
                .Query<TEntity>()
                .Count(predicate);
        }

        public void Inserir(params TEntity[] entity)
        {
            if (entity == null)
                return;

            foreach (var item in entity)
                Session.Save(item);
        }

        public void Alterar(params TEntity[] entity)
        {
            if (entity == null)
                return;

            foreach (var item in entity)
                Session.Update(item, item.Id);
        }

        public void Excluir(params int[] id)
        {
            if (!id.Any())
                return;

            var query = Session.CreateQuery(string.Format("DELETE {0} WHERE Id IN (:listaId)", typeof(TEntity).Name))
                .SetParameterList("listaId", id);

            query.ExecuteUpdate();
        }

        public void Excluir(params TEntity[] entity)
        {
            if (entity == null)
                return;

            foreach (var item in entity)
                Session.Delete(item);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void InserirOuAlterar(params TEntity[] entity)
        {
            if (entity == null)
                return;

            foreach (var item in entity)
            {
                if (item.Id == 0)
                    Session.Save(item);
                else
                    Session.Update(item, item.Id);
            }
        }


        public void Alterar(
            Expression<Func<TEntity, bool>> predicate,
            Action<IAlterarPropriedade<TEntity>> selector)
        {
            var alterarPropriedade = new AlterarPropriedade<TEntity>();
            selector(alterarPropriedade);

            if (!alterarPropriedade.Propriedades.Any())
                return;
        }


        public void Alterar(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, object>> selector)
        {
            var listaMemberInfo = new List<MemberInfo>();

            if (selector.Body is NewExpression newExpression)
                listaMemberInfo.AddRange(
                        newExpression.Arguments
                            .Select(s => (s as MemberExpression).Member));
            else
                listaMemberInfo.Add(
                    ((selector.Body as UnaryExpression).Operand as MemberExpression).Member);

            var propriedades = string.Join(
                ',',
                listaMemberInfo
                    .Select(s => string.Concat(s.Name, "= :", s.Name)));

            var textoQuery = string.Format(
                "UPDATE {0} " +
                "   SET {1} " +
                " WHERE Id IN (:listaId)",
                typeof(TEntity).Name,
                propriedades);

            //var query = Session.CreateQuery(textoQuery)
            //    .SetParameterList("listaId", 0);

            //foreach (var item in listaMemberInfo)
            //{
            //    query.SetParameter(item.Name, 0);
            //}

            //query.ExecuteUpdate();
        }


        public void Alterar(
            Expression<Func<TEntity, bool>> predicate,
            params (Expression<Func<TEntity, object>> propriedade, object valor)[] propriedades)
        {
            if (propriedades == null)
            {

            }
        }


        public void Alterar(
            Expression<Func<TEntity, bool>> predicate,
            Action<dynamic> propriedade)
        {
            var expandoObject = new ExpandoObject();
            propriedade(expandoObject);

            if (expandoObject == null)
            {

            }
        }


        internal string TableName =>
            (Session.SessionFactory.GetClassMetadata(typeof(TEntity)) as AbstractEntityPersister).RootTableName;
    }

    public class AlterarPropriedade<TEntity> : IAlterarPropriedade<TEntity> where TEntity : IEntity
    {
        List<(string Nome, dynamic value)> _propriedades;

        public (string Nome, dynamic value)[] Propriedades => _propriedades.ToArray();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Propriedade<TKey>(Expression<Func<TEntity, TKey>> propriedade, TKey valor)
        {
            var propertyInfo = (propriedade.Body as MemberExpression).Member as PropertyInfo;
            if (_propriedades == null)
                _propriedades = new List<(string Nome, dynamic value)>();

            _propriedades.Add((propertyInfo.Name, valor));
        }
    }

}
