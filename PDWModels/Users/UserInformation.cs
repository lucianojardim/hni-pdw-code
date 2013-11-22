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
		[DisplayName( "User Type" )]
		[Required]
		public string UserType { get; set; }
		[DisplayName( "User Role" )]
		[Required]
		public string Role { get; set; }
		[DisplayName( "Enabled" )]
		public bool Enabled { get; set; }
	}
}
