namespace SimpleBackup
{
    using System.Configuration;

    using SimpleBackup.Domain.Interfaces;

    public class ConfigurationBasedSettingsProvider : ISettingsProvider
    {
        public override string Get(SettingsType type)
        {
            return ConfigurationManager.AppSettings[type.ToString()];
        }
    }
}