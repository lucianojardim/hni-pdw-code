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

		public bool LockCompanyType { get; set; }
	}
}
