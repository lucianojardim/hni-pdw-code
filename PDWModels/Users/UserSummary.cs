using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Users
{
	public class UserSummary
	{
		public int UserID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string EmailAddress { get; set; }
		public string PhoneNumber { get; set; }
		public string CompanyName { get; set; }
		public string EditButtons { get; set; }
		public bool Enabled { get; set; }
		public bool SentWelcomeEmail { get; set; }
		public DateTime? LastLogin { get; set; }
		public double LastLoginMilliseconds { get { return LastLogin.HasValue ? ( LastLogin.Value - ( new DateTime( 1970, 1, 1 ) ) ).TotalMilliseconds : 0; } }

		public string FullName { get { return FirstName + " " + LastName; } }
	}
}
