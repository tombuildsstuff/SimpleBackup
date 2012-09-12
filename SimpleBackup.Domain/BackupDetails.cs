namespace SimpleBackup.Domain
{
	using System;

	public class BackupDetails
	{
		public const string Extension = "backup";

		public string Name { get; set; }

		public DateTime BackupDate { get; set; }

		public string GenerateFileName()
		{
			return string.Format("{0}-{1}_{2}_{3}-{4}_{5}_{6}.{7}", Name, BackupDate.Year, BackupDate.Month, BackupDate.Day,
			                     BackupDate.Hour, BackupDate.Minute, BackupDate.Second, Extension);
		}

		public static BackupDetails ParseFromBackupFile(string fileName)
		{
			var name = fileName.Split('-');
			if (name.Length != 3)
				return null;

			var stringDate = name[1].Split('_');
			var stringTime = name[2].Replace(string.Format(".{0}", Extension), string.Empty).Split('_');
			var date = new DateTime(int.Parse(stringDate[0]), int.Parse(stringDate[1]), int.Parse(stringDate[2]),
			                        int.Parse(stringTime[0]), int.Parse(stringTime[1]), int.Parse(stringTime[2]));

			return new BackupDetails
			{
				Name = name[0],
				BackupDate = date
			};
		}
	}
}