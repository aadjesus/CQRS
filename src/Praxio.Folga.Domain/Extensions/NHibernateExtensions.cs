using BgmRodotec.Framework.Domain.Core.Model;
using NHibernate.AdoNet.Util;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Praxio.Folga.Domain.Model
{
    public static class NHibernateExtensions
    {
        public static IQueryable<TOriginating> AutoFetch<TOriginating>(
           this IQueryable<TOriginating> query)
        {
            var fields = typeof(TOriginating).GetProperties()
                .Where(f => f.PropertyType.IsSubclassOf(typeof(Entity)));

            var method = typeof(EagerFetchingExtensionMethods)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(me => me.Name == "Fetch");

            if (method == null)
                return query;

            foreach (var field in fields)
            {
                var methodField = method.MakeGenericMethod(typeof(TOriginating), field.PropertyType);
                var param = Expression.Parameter(typeof(TOriginating));
                var body = Expression.PropertyOrField(param, field.Name);
                var delegateType = typeof(Func<,>).MakeGenericType(typeof(TOriginating), field.PropertyType);
                var lambda = Expression.Lambda(delegateType, body, param);
                query = (IQueryable<TOriginating>)methodField.Invoke(query, new object[] { query, lambda });
            }

            return query;
        }

        // <summary>
        /// Retorna o SQL da query informada
        /// </summary>
        /// <param name="session"/>
        /// <param name="queryable"/>
        /// <returns><see cref="string"/></returns>
        public static string GetSql(this NHibernate.ISession session, IQueryable queryable)
        {
            var sessionImpl = session as NHibernate.Impl.SessionImpl;

            var nhLinqExpression = new NhLinqExpression(
                queryable.Expression,
                sessionImpl.Factory);

            var queryTranslatorImpl = new ASTQueryTranslatorFactory()
                .CreateQueryTranslators(
                    nhLinqExpression,
                    null,
                    false,
                    sessionImpl.EnabledFilters,
                    sessionImpl.Factory)
                .First() as QueryTranslatorImpl;

            var retorno = PopularParametros(
                nhLinqExpression,
                FormatStyle.Basic.Formatter.Format(queryTranslatorImpl.SQLString));

            return retorno;
        }

        private static string PopularParametros(NhLinqExpression nhLinqExpression, string sql)
        {
            try
            {
                var parametros = nhLinqExpression.ParameterValuesByName
                    .Aggregate(new List<string>(), (lista, item) =>
                    {
                        var valor = item.Value.Item1;

                        var listaValor = (valor as IList);
                        if (listaValor == null)
                            listaValor = new[] { valor };

                        valor = string.Join(
                            ',',
                            listaValor
                                .OfType<dynamic>()
                                .Select(s => FormartarValor(s))
                                .ToArray());

                        lista.Add(valor.ToString());

                        return lista;
                    })
                    .ToArray();

                int i = 0;
                var retorno = Regex.Replace(
                    sql,
                    @"\?",
                    r => parametros[i++]);

                return retorno;
            }
            catch
            {
                return sql;
            }
        }

        private static dynamic FormartarValor(dynamic valor)
        {
            if (valor.GetType() == typeof(DateTime))
            {
                var dataHora = (DateTime)valor;

                (string mascCodigo, string mascBanco) =
                    dataHora.TimeOfDay == TimeSpan.Zero
                        ? (string.Empty, string.Empty)
                        : (" HH:mm", " HH24:MI");

                valor = string.Concat(
                    "TO_DATE('",
                    dataHora.ToString("dd/MM/yyyy" + mascCodigo),
                    "', 'DD/MM/YYYY",
                    mascBanco,
                    "')");
            }
            else if (valor.GetType() == typeof(string))
                valor = string.Concat("'", valor, "'");

            return valor;
        }
    }
}
