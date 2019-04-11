using NHibernate;
using NHibernate.SqlCommand;
using Serilog;
using System.Diagnostics;

namespace Praxio.Folga.Domain.Extensions
{
    public class NHSQLInterceptor : EmptyInterceptor, IInterceptor
    {
        private readonly ILogger _logger;

        public NHSQLInterceptor() { }

        public NHSQLInterceptor(ILogger logger)
        {
            _logger = logger;
        }

        public override SqlString OnPrepareStatement(SqlString sql)
        {
            var textoSql = sql.ToString();

            Debug.WriteLine("SQL: " + textoSql);

            //var parametros = sql.GetParameters().Select((s, index) => ":p" + index).ToArray();
            //int i = 0;
            //var retorno = FormatStyle.Basic.Formatter.Format(Regex.Replace(
            //    textoSql,
            //    @"\?",
            //    r => parametros[i++]));

            _logger?.Information(textoSql);

            return base.OnPrepareStatement(sql);
        }


    }
}
