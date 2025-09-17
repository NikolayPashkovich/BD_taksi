namespace BD_taksi.Services
{
    public class NotificationService : INotificationService
    {
        public event Action<string>? DataChanged;

        public void NotifyDataChanged(string entityType)
        {
            DataChanged?.Invoke(entityType);
        }
    }
}
