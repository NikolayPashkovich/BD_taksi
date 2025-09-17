namespace BD_taksi.Services
{
    public interface INotificationService
    {
        event Action<string> DataChanged;
        void NotifyDataChanged(string entityType);
    }
}
