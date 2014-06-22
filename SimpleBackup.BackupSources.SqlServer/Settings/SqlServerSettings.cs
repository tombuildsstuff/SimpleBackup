namespace SimpleBackup.BackupSources.SqlServer.Settings
{
    using System;
    using System.Configuration;

    public class SqlServerSettings : ISqlServerSettings
    {
        public SqlServerAuthType Authentication
        {
            get
            {
                SqlServerAuthType value;

                if (Enum.TryParse(ConfigurationManager.AppSettings["SqlServer.Authentication"], true, out value))
                    return value;

                return SqlServerAuthType.Unknown;
            }
        }

        public bool BackupEnabled
        {
            get
            {
                bool value;

                if (bool.TryParse(ConfigurationManager.AppSettings["SqlServer.BackupsEnabled"], out value))
                    return value;

                return false;
            }
        }

        public string ConnectionString
        {
            get
            {
                switch (Authentication)
                {
                    case SqlServerAuthType.Credentials:
                        return string.Format("Server={0};User ID={1};Password={2}", Instance, Username, Password);

                    case SqlServerAuthType.SSPI:
                        return string.Format("Server={0};Integrated Security=SSPI", Instance);
            
                    default:
                        throw new NotSupportedException(string.Format("Cannot configure the connection string for the {0} authentication type.", Authentication));
                }
            }
        }

        public string Instance
        {
            get
            {
                return ConfigurationManager.AppSettings["SqlServer.Instance"];
            }
        }

        public string Password
        {
            get
            {
                return ConfigurationManager.AppSettings["SqlServer.Password"];
            }
        }

        public int Timeout
        {
            get
            {
                int value;

                if (int.TryParse(ConfigurationManager.AppSettings["SqlServer.Timeout"], out value))
                    return value;

                return 30000;
            }
        }

        public string Username
        {
            get
            {
                return ConfigurationManager.AppSettings["SqlServer.Username"];
            }
        }
    }
}