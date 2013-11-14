using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PDWInfrastructure;

namespace PDWModels.Users
{
	public class UserInformation
	{
		public int UserID { get; set; }
		[DisplayName( "Email Address" )]
		[Required]
		[Email( ErrorMessage = "Email Address doesn't look like a valid email address." )]
		public string EmailAddress { get; set; }
		[DisplayName( "First Name" )]
		[Required]
		public string FirstName { get; set; }
		[DisplayName( "Last Name" )]
		[Required]
		public string LastName { get; set; }
		[DisplayName( "Company Name" )]
		[Required]
		public string CompanyName { get; set; }
		[DisplayName( "Address 1" )]
		public string Address1 { get; set; }
		[DisplayName( "Address 2" )]
		public string Address2 { get; set; }
		[DisplayName( "City" )]
		public string City { get; set; }
		[DisplayName( "State" )]
		public string State { get; set; }
		[DisplayName( "Zip Code" )]
		public string Zip { get; set; }
		[DisplayName( "Business Phone" )]
		public string BusinessPhone { get; set; }
		[DisplayName( "Cell Phone" )]
		public string CellPhone { get; set; }
		[DisplayName( "Title" )]
		public string Title { get; set; }
		[DisplayName( "User Type" )]
		[Required]
		public string UserType { get; set; }
		[DisplayName( "User Role" )]
		[Required]
		public string Role { get; set; }
	}
}
