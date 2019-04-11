using AutoMapper;
using BgmRodotec.Framework.Domain.Core.Bus;
using Praxio.Folga.Application.Interfaces;
using System;

namespace Praxio.Folga.Application.Services
{
    /// <summary/>
    public abstract class AppService : IAppService
    {
        protected readonly IMediatorHandler _mediator;
        protected readonly IMapper _mapper;

        /// <summary/>
        protected AppService(IMapper mapper, IMediatorHandler mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        /// <summary/>
        public virtual void Dispose() => GC.SuppressFinalize(this);
    }
}
