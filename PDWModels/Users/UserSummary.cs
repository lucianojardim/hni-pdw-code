using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Users
{
	public class UserSummary
	{
		public int UserID { get; set; }
		public string FullName { get; set; }
		public string EmailAddress { get; set; }
		public string CompanyName { get; set; }
		public string EditButtons { get; set; }
	}
}
