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
		[DisplayName( "Work Mobile" )]
		[PhoneNumber]
		public string CellPhone { get; set; }
		[DisplayName( "Address 1" )]
		[StringLength( 100 )]
		public string HomeAddress1 { get; set; }
		[DisplayName( "Address 2" )]
		[StringLength( 100 )]
		public string HomeAddress2 { get; set; }
		[DisplayName( "City" )]
		[StringLength( 50 )]
		public string HomeCity { get; set; }
		[DisplayName( "State" )]
		public string HomeState { get; set; }
		[DisplayName( "Zip Code" )]
		[PostalCode]
		public string HomeZip { get; set; }
		[DisplayName( "Home Phone" )]
		[PhoneNumber]
		public string HomePhone { get; set; }
		[DisplayName( "Personal Mobile" )]
		[PhoneNumber]
		public string PersonalCellPhone { get; set; }
		[DisplayName( "Fax" )]
		[PhoneNumber]
		public string FaxNumber { get; set; }
		[DisplayName( "Extension" )]
		[StringLength( 10 )]
		public string Extension { get; set; }
		[DisplayName( "Title" )]
		[StringLength( 50 )]
		public string Title { get; set; }
		[DisplayName( "Image" )]
		public string UserImageFileName { get; set; }
		[DisplayName( "Default Shipping Address" )]
		public int DefaultShippingAddress { get; set; }
		[DisplayName( "User Tier" )]
		public string TierGroup { get; set; }

		[DisplayName( "Company" )]
		public string CompanyName { get; set; }
		[DisplayName( "Company Tier" )]
		public string CompanyTierGroup { get; set; }
	}
}
