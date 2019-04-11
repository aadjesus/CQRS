using System;

namespace Praxio.Folga.Domain.Interfaces
{
    public interface IParametersAppSettings : IDisposable
    {
        /// <summary/>
        int QtdePaginacao { get; set; }
        /// <summary/>
        object Outros { get; set; }
        /// <summary/>
        bool Producao { get; set; }
        /// <summary/>
        bool GPS { get; set; }
    }
}
