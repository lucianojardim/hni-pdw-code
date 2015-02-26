using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Users
{
	public class UserActivityExport
	{
		public string EmailAddress { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Activity { get; set; }
		public DateTime UTCDateTime { get; set; }
	}
}
