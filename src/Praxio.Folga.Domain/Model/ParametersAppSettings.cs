using Praxio.Folga.Domain.Interfaces;
using System;

namespace Praxio.Folga.Domain.Model
{
    public class ParametersAppSettings : IParametersAppSettings
    {
        public int QtdePaginacao { get; set; }
        public object Outros { get; set; }
        public bool Producao { get; set; }
        public bool GPS { get; set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
