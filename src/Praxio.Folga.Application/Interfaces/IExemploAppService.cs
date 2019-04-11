using Praxio.Folga.Application.ViewModels;
using System.Collections.Generic;

namespace Praxio.Folga.Application.Interfaces
{
    /// <summary/>
    public interface IExemploAppService : IAppService
    {
        /// <summary/>
        IEnumerable<ExemploViewModel> Obter();

        ExemploViewModel Obter(int id);
    }
}
