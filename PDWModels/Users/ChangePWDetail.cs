using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using PDWInfrastructure;
using System.ComponentModel.DataAnnotations;

namespace PDWModels.Users
{
	[Match( "NewPassword", "ConfirmPassword", ErrorMessage = "Passwords must match" )]
	[Match( "OldPassword", "NewPassword", false, ErrorMessage = "Current and New Passwords cannot match" )]
	public class ChangePWDetail
	{
		[DisplayName( "Current Password" )]
		[Required]
		public string OldPassword { get; set; }
		[DisplayName( "New Password" )]
		[PaoliPassword( ErrorMessage = "Password does not meet complexity requirements." )]
		[Required]
		public string NewPassword { get; set; }
		[DisplayName( "Re-type Password" )]
		public string ConfirmPassword { get; set; }

		public static bool IsGoodPassword( string pwCheck )
		{
			return ( new PaoliPasswordAttribute().IsMatch( pwCheck ) );
		}
	}
}
