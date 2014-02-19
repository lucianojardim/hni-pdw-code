using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PDWInfrastructure;

namespace PDWModels.Companies
{
	public class CompanyInformation
	{
		public int CompanyID { get; set; }
		[DisplayName("Name")]
		[Required]
		[StringLength( 50 )]
		public string Name { get; set; }
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
		[DisplayName( "Phone Number" )]
		[PhoneNumber]
		public string Phone { get; set; }
		[DisplayName( "FAX Number" )]
		[PhoneNumber]
		public string FAX { get; set; }
		[DisplayName( "Contact Email" )]
		[Email]
		public string ContactEmail { get; set; }
		[DisplayName( "Web Site URL" )]
		public string WebSite { get; set; }

		[DisplayName( "Display Name As" )]
		public string PublicDisplayName { get; set; }
		[DisplayName( "Address 1" )]
		[StringLength( 100 )]
		public string PublicAddress1 { get; set; }
		[DisplayName( "Address 2" )]
		[StringLength( 100 )]
		public string PublicAddress2 { get; set; }
		[DisplayName( "City" )]
		[StringLength( 50 )]
		public string PublicCity { get; set; }
		[DisplayName( "State" )]
		public string PublicState { get; set; }
		[DisplayName( "Zip Code" )]
		[PostalCode]
		public string PublicZip { get; set; }
		[DisplayName( "Phone Number" )]
		[PhoneNumber]
		public string PublicPhone { get; set; }
		[DisplayName( "FAX Number" )]
		[PhoneNumber]
		public string PublicFAX { get; set; }
		[DisplayName( "Contact Email" )]
		[Email]
		public string PublicContactEmail { get; set; }
		[DisplayName( "Web Site URL" )]
		public string PublicWebSite { get; set; }
	
		[DisplayName( "Master Consolidated Number" )]
		[StringLength( 20 )]
		public string MasterID { get; set; }
		[DisplayName( "Customer Base Number" )]
		[StringLength( 200 )]
		public string SubCompanyIDs { get; set; }
		[DisplayName( "Trip Incentive" )]
		public bool TripIncentive { get; set; }
		[DisplayName( "Company Type" )]
		[Required]
		public int CompanyType { get; set; }
		[DisplayName( "Territory ID" )]
		public int? TerritoryID { get; set; }
		[DisplayName( "Business Unit Name" )]
		[StringLength( 200 )]
		public string BusinessUnitName { get; set; }

		public bool LockCompanyType { get; set; }

		[DisplayName( "Has a Show Room" )]
		public bool HasShowroom { get; set; }
		[DisplayName( "Display Name As" )]
		public string ShowroomDisplayName { get; set; }
		[DisplayName( "Address 1" )]
		[StringLength( 100 )]
		public string ShowroomAddress1 { get; set; }
		[DisplayName( "Address 2" )]
		[StringLength( 100 )]
		public string ShowroomAddress2 { get; set; }
		[DisplayName( "City" )]
		[StringLength( 50 )]
		public string ShowroomCity { get; set; }
		[DisplayName( "State" )]
		public string ShowroomState { get; set; }
		[DisplayName( "Zip Code" )]
		[PostalCode]
		public string ShowroomZip { get; set; }
		[DisplayName( "Phone Number" )]
		[PhoneNumber]
		public string ShowroomPhone { get; set; }
		[DisplayName( "FAX Number" )]
		[PhoneNumber]
		public string ShowroomFAX { get; set; }
		[DisplayName( "Web Site URL" )]
		public string ShowroomWebsite { get; set; }
		[DisplayName( "Description" )]
		public string ShowroomDescription { get; set; }
		[DisplayName( "Hours" )]
		public string ShowroomHours { get; set; }
		[DisplayName( "Images" )]
		public string ShowroomImages { get; set; }
	}
}
