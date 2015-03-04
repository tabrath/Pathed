namespace Pathed.Services
{
    public interface ISettingsService
    {
        void Set<T>(string propertyName, T value);
        T Get<T>(string propertyName, T defaultValue = default(T));
        
        void Load();
        void Save();
    }
}
