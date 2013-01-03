namespace SimpleBackup.Domain.Email
{
	using System.Collections.Generic;

	public class EmailConfiguration
	{
		private readonly string _from;
		private readonly string _fromAlias;

        public EmailConfiguration(string from, string fromAlias, string successfulSubject, string failureSubject, params string[] addresses)
        {
            _from = @from;
            _fromAlias = fromAlias;
            SuccessfulSubject = successfulSubject;
            FailureSubject = failureSubject;
            SendToAddresses = addresses;
        }

		public IEnumerable<string> SendToAddresses { get; set; }

        public string SuccessfulSubject { get; set; }

        public string FailureSubject { get; set; }

		public string To
		{
			get
			{
				return string.Join(";", SendToAddresses);
			}
		}

		public string From
		{
			get
			{
				return string.IsNullOrWhiteSpace(_fromAlias) ? _from : string.Format("{0} <{1}>", _fromAlias, _from);
			}
		}
	}
}