using AutoMapper;
using BgmRodotec.Framework.Domain.Core.Bus;
using Praxio.Folga.Application.Interfaces;
using Praxio.Folga.Application.ViewModels;
using System.Collections.Generic;

namespace Praxio.Folga.Application.Services
{
    /// <summary/>
    public abstract class ExemploAppService : AppService, IExemploAppService
    {
        /// <summary/>
        protected ExemploAppService(IMapper mapper, IMediatorHandler mediator) : base(mapper, mediator)
        {
        }

        public IEnumerable<ExemploViewModel> Obter()
        {
            throw new System.NotImplementedException();
        }

        public ExemploViewModel Obter(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
