using BgmRodotec.Framework.Domain.Core.Notifications;

namespace Praxio.Folga.Domain.Notifications
{
    public class DomainNotification : Notification
    {
        public DomainNotification(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public string Value { get; }
    }
}
