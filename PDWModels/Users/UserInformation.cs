using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PDWModels.Users
{
	public class UserInformation : MyAccountInfo
	{
		[DisplayName( "Account Type" )]
		[Required]
		public int AccountType { get; set; }
		[DisplayName( "Enabled" )]
		public bool Enabled { get; set; }
	}
}
