using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PDWInfrastructure;
using PDWInfrastructure.Attributes;

namespace PDWModels.Users
{
	public class MyAccountInfo
	{
		public int UserID { get; set; }
		[DisplayName( "Email Address" )]
		[Required]
		[Email( ErrorMessage = "Email Address doesn't look like a valid email address." )]
		[StringLength(50)]
		public string EmailAddress { get; set; }
		[DisplayName( "First Name" )]
		[Required]
		[StringLength( 50 )]
		public string FirstName { get; set; }
		[DisplayName( "Last Name" )]
		[Required]
		[StringLength( 50 )]
		public string LastName { get; set; }
		[DisplayName( "Address 1" )]
		[StringLength( 100 )]
		public string Address1 { get; set; }
		[DisplayName( "Address 2" )]
		[StringLength( 100 )]
		public string Address2 { get; set; }
		[DisplayName( "City" )]
		[StringLength( 50 )]
		public string City { get; set; }
		[DisplayName( "State" )]
		public string State { get; set; }
		[DisplayName( "Zip Code" )]
		[PostalCode]
		public string Zip { get; set; }
		[DisplayName( "Business Phone" )]
		[PhoneNumber]
		public string BusinessPhone { get; set; }
		[DisplayName( "Mobile Phone" )]
		[PhoneNumber]
		public string CellPhone { get; set; }
		[DisplayName( "Title" )]
		[StringLength( 50 )]
		public string Title { get; set; }

		[DisplayName( "Company" )]
		public string CompanyName { get; set; }
	}
}
