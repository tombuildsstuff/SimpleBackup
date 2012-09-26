namespace SimpleBackup.Domain.Interfaces
{
    public abstract class ISettingsProvider
    {
        public abstract string Get(SettingsType type);

        public enum SettingsType
        {
            /// <summary>
            /// The directory which is used for backups locally
            /// </summary>
            LocalBackupDirectory,
            
            /// <summary>
            /// The number of local backups which should be kept
            /// </summary>
            LocalBackupsToKeep,

            /// <summary>
            /// Password given to encrypt the archive
            /// </summary>
            Password,

            /// <summary>
            /// The number of remote backups which should be kept
            /// </summary>
            RemoteBackupsToKeep,
            
            /// <summary>
            /// The prefix used when uploading archives to S3
            /// </summary>
            S3Prefix,
            
            /// <summary>
            /// The Secret Access Key provided from S3
            /// </summary>
            S3SecretAccessKey,

            /// <summary>
            /// The Access Key provided from S3
            /// </summary>
            S3AccessKey,

            /// <summary>
            /// The bucket used in S3 to store data
            /// </summary>
            S3Bucket,

            /// <summary>
            /// The type of authentication which should be used for SQL Server - SSPI or Credentials
            /// </summary>
            SQLServerAuth,
            
            /// <summary>
            /// The location of the SQL Server (e.g. localhost\sqlexpress)
            /// </summary>
            SQLServer,

            /// <summary>
            /// The username which should be used when connecting to SQL Server (only used when connecting via Credentials - see SQLServerAuth)
            /// </summary>
            SQLServerUser,

            /// <summary>
            /// The password which should be used when connecting to SQL Server (only used when connecting via Credentials - see SQLServerAuth)
            /// </summary>
			SQLServerPass,

			/// <summary>
			/// The Timeout which should be used by Sql Server
			/// </summary>
			SQLServerTimeout,
            
            /// <summary>
            /// Temporary Directory used for Working Purposes
            /// </summary>
            TempDirectory
        }
    }
}