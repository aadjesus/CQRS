using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Praxio.Folga.Domain.NotificationHandlers;
using Praxio.Folga.Domain.Notifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Praxio.Folga.Api.Controllers
{

    /// <summary>
    /// 
    /// </summary>
    /// 
    //#if DEBUG
    //    [AllowAnonymous]
    //#endif
    [AllowAnonymous]

    public class BaseController : ControllerBase
    {
        private readonly DomainNotificationHandler _notifications;
        private readonly DomainDataNotificationHandler _dataNotification;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="notifications"/>
        protected BaseController(
            INotificationHandler<DomainNotification> notifications)
        {
            _notifications = (DomainNotificationHandler)notifications;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="notifications"/>
        /// /// <param name="dataNotifications"/>
        protected BaseController(
            INotificationHandler<DomainNotification> notifications,
            INotificationHandler<DomainDataNotification> dataNotifications)
        {
            _notifications = (DomainNotificationHandler)notifications;
            _dataNotification = (DomainDataNotificationHandler)dataNotifications;
        }


        /// <summary>
        /// 
        /// </summary>
        protected IEnumerable<DomainNotification> Notifications => _notifications.GetNotifications();


        /// <summary/>
        protected bool IsValidOperation()
        {
            return (!_notifications.HasNotifications());
        }


        /// <summary/>
        protected new IActionResult Response(params object[] result)
        {
            if (!result?.Any() ?? true)
                result = _dataNotification?.GetNotifications()
                    .Select(n => n.Data)
                    .ToArray();

            if (IsValidOperation())
            {
                return Ok(new
                {
                    success = true,
                    data = result?.First()
                });
            }

            return Ok(new
            {
                success = false,
                errors = Notifications.Select(n => n.Value)
            });
        }

        /// <summary/>
        protected void NotifyError(string code, string message)
        {
            _notifications.Handle(new DomainNotification(code, message), default(CancellationToken));
        }
    }
}