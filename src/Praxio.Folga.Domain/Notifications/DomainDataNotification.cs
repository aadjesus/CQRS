using BgmRodotec.Framework.Domain.Core.Notifications;

namespace Praxio.Folga.Domain.Notifications
{
    public class DomainDataNotification : Notification
    {
        public object Data { get; set; }
    }
}
