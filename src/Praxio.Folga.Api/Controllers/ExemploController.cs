using MediatR;
using Microsoft.AspNetCore.Mvc;
using Praxio.Folga.Domain.Notifications;

namespace Praxio.Folga.Api.Controllers
{
    [Route("[controller]")]
    public class ExemploController : BaseController
    {
        /// <summary/>
        public ExemploController(
            INotificationHandler<DomainNotification> notifications) : base(notifications)
        {
        }

        // <summary/>
        /// <remarks>
        /// Obter 
        /// </remarks>         
        /// <returns></returns>
        [HttpGet]
        [Route(nameof(Obter))]
        //[ProducesResponseType(201, Type = typeof(Teste))]
        public IActionResult Obter() =>
            Response(new { Codigo = 1, Descricao = "Teste" });
    }
}
