using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pathed.Services
{
    [Export(typeof(ISettingsService)), PartCreationPolicy(CreationPolicy.Shared)]
    public class SettingsService : ISettingsService, IPartImportsSatisfiedNotification
    {
        [Import(typeof(IApplicationService))]
        private IApplicationService applicationService;

        private Dictionary<string, object> properties;
        private string filename;
        private BinaryFormatter serializer;

        [ImportingConstructor]
        public SettingsService()
        {
        }

        public void Set<T>(string propertyName, T value)
        {
            lock (this.properties)
            {
                this.properties[propertyName] = value;
            }
        }

        public T Get<T>(string propertyName, T defaultValue = default(T))
        {
            lock (this.properties)
            {
                return this.properties.ContainsKey(propertyName) ? (T)this.properties[propertyName] : defaultValue;
            }
        }

        public void Load()
        {
            if (File.Exists(this.filename))
            {
                using (var stream = File.OpenRead(this.filename))
                {
                    lock (this.properties)
                    {
                        try
                        {
                            this.properties = this.serializer.Deserialize<Dictionary<string, object>>(stream);
                        }
                        catch
                        {
                            if (this.properties == null)
                                this.properties = new Dictionary<string, object>();
                        }
                    }
                }
            }
        }

        public void Save()
        {
            var directory = Path.GetDirectoryName(this.filename);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var stream = File.Create(this.filename))
            {
                lock (this.properties)
                {
                    this.serializer.Serialize(stream, this.properties);
                }
            }
        }

        public void OnImportsSatisfied()
        {
            this.properties = new Dictionary<string, object>();
            var title = this.applicationService.Title;
            this.filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create),
                title, title + ".settings");
            this.serializer = new BinaryFormatter();

            Load();
        }
    }
}
