using BgmRodotec.Framework.Domain.Core.Model;
using FluentNHibernate.Mapping;
using Praxio.Folga.Domain.Interfaces;
using Praxio.Folga.Infra.Data.Filters;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Praxio.Folga.Infra.Data.Mappings
{
    /// <summary>
    /// Classe base para o mapeamento do banco de dados pelo fluent hibernate. Geralmente representa o mapeamento de uma tabela no banco de dados.
    /// </summary>
    /// <typeparam name="T">Tipo de dados</typeparam>
    public class MapBase<T> : ClassMap<T> where T : IEntity
    {
        /// <summary>
        ///     Construtor
        /// </summary>
        /// <param name="lazyLoad">
        ///     Especifica o comportamento de carregamento padrão de sub propriedades (relacionamentos).
        ///     O valor padrão é o não carregamento de sub propriedades (relacionamentos)
        /// </param>
        protected MapBase(bool lazyLoad = true)
        {
            ConstrutorPadrao(lazyLoad);
        }

        protected MapBase(string tableName)
        {
            ConstrutorPadrao(true, tableName);
        }

        private void ConstrutorPadrao(bool lazyLoad, string tableName = null)
        {
            DynamicUpdate();
            if (lazyLoad)
                LazyLoad();

            if (string.IsNullOrEmpty(tableName))
                tableName = typeof(T).Name.Replace("Model", string.Empty);

            Table(tableName);

            Id(i => i.Id).GeneratedBy
                .Native(builder => builder.AddParam("sequence", "SEQ_" + tableName));

            var propertyInfo = typeof(T).GetProperties()
                .FirstOrDefault(a => a.Name == nameof(IDataHoraExclusao.DataHoraExclusao));

            if (propertyInfo != null)
            {
                Map(m => ((IDataHoraExclusao)m).DataHoraExclusao, propertyInfo.Name);
                ApplyFilter<DataHoraExclusaoFilter>();
            }
        }

        //public new PropertyPart Map(Expression<Func<T, object>> memberExpression)
        //{
        //    var member = ((memberExpression.Body as UnaryExpression).Operand as MemberExpression).Member;

        //    if (member.Name == nameof(IDataHoraExclusao.DataHoraExclusao))
        //        throw new Exception("Não mapear a propriedade '" + nameof(IDataHoraExclusao.DataHoraExclusao) + "', a mesma é mapeada automaticamente, classe '" + this.GetType().Name + "'.");

        //    //this.HibernateMapping.DefaultAccess.
        //    return Map(memberExpression, member.Name);
        //}


        public new ManyToOnePart<TOther> References<TOther>(Expression<Func<T, TOther>> memberExpression)
        {
            var propertyInfo = (memberExpression.Body as MemberExpression).Member as PropertyInfo;
            return References(memberExpression, "Id" + propertyInfo.Name);
        }

    }
}
