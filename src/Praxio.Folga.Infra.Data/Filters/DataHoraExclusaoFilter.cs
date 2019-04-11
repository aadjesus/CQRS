using FluentNHibernate.Mapping;

namespace Praxio.Folga.Infra.Data.Filters
{
    public class DataHoraExclusaoFilter : FilterDefinition
    {
        public DataHoraExclusaoFilter()
        {

            WithName(nameof(DataHoraExclusaoFilter)).WithCondition("DataHoraExclusao is null");
        }
    }
}
