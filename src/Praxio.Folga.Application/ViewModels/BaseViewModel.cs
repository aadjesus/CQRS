using System;

namespace Praxio.Folga.Application.ViewModels
{
    /// <summary/>
    public class BaseViewModel : IViewModel
    {
        /// <summary/>
        public void Dispose() => GC.SuppressFinalize(this);
    }
}
