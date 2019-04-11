using MediatR;
using Praxio.Folga.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Praxio.Folga.Domain.NotificationHandlers
{
    public sealed class DomainDataNotificationHandler : INotificationHandler<DomainDataNotification>, IDisposable
    {
        private IList<DomainDataNotification> _notifications;

        public DomainDataNotificationHandler()
        {
            _notifications = new List<DomainDataNotification>();
        }

        public void Dispose()
        {
            _notifications = new List<DomainDataNotification>();
        }

        public Task Handle(DomainDataNotification notification, CancellationToken cancellationToken)
        {
            _notifications.Add(notification);
            return Task.FromResult(true);
        }

        public IEnumerable<DomainDataNotification> GetNotifications()
        {
            return _notifications;
        }

        public bool HasNotifications()
        {
            return GetNotifications().Any();
        }
    }
}
