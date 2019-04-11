using System.ComponentModel.DataAnnotations;

namespace Praxio.Folga.Application.ViewModels
{
    /// <summary/>
    public class ExemploViewModel : BaseIdViewModel
    {
        /// <summary/>        
        [StringLength(5, MinimumLength = 1), Required]
        public string Codigo { get; set; }
        /// <summary/>
        [StringLength(100, MinimumLength = 1), Required]
        public string Descricao { get; set; }
    }
}
