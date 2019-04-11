using MediatR;
using Praxio.Folga.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Praxio.Folga.Domain.NotificationHandlers
{
    public sealed class DomainNotificationHandler : INotificationHandler<DomainNotification>, IDisposable
    {
        private IList<DomainNotification> _notifications;

        public DomainNotificationHandler()
        {
            _notifications = new List<DomainNotification>();
        }

        public void Dispose()
        {
            _notifications = new List<DomainNotification>();
        }

        public Task Handle(DomainNotification notification, CancellationToken cancellationToken)
        {
            _notifications.Add(notification);
            return Task.FromResult(true);
        }

        public IEnumerable<DomainNotification> GetNotifications()
        {
            return _notifications;
        }

        public bool HasNotifications()
        {
            return GetNotifications().Any();
        }
    }
}
