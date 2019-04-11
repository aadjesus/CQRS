using System.ComponentModel.DataAnnotations;

namespace Praxio.Folga.Application.ViewModels
{
    /// <summary/>
    public class BaseIdViewModel : BaseViewModel
    {
        /// <summary/>
        [Required]
        public virtual int? Id { get; set; }
    }
}
