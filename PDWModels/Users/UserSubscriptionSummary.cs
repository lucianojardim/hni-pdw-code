using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWInfrastructure;
using System.ComponentModel;
using PDWInfrastructure.Attributes;

namespace PDWModels.Users
{
	public class UserSubscriptionSummary
	{
		public UserSubscriptionSummary()
		{
			NewCollateralOrder = true;
			NewCollateralOrderTerritory = true;
			NewCollateralOrderMyDealers = true;
			NewCollateralOrderShipment = true;
			NewCollateralOrderShipmentTerritory = true;
			NewCollateralOrderShipmentMyDealers = true;
			NewSpecRequest = true;
			NewSpecRequestTerritory = true;
			NewSpecRequestMyDealers = true;
			CompleteSpecRequest = true;
			CompleteSpecRequestTerritory = true;
			CompleteSpecRequestMyDealers = true;
			UpdateSpecRequest = true;
			UpdateSpecRequestTerritory = true;
			UpdateSpecRequestMyDealers = true;
			ReOpenSpecRequest = true;
			ReOpenSpecRequestTerritory = true;
			ReOpenSpecRequestMyDealers = true;
		}

		public int UserID { get; set; }

		[DisplayName( "My orders" )]
		public bool NewCollateralOrder { get; set; }
		[DisplayName( "All orders for my territory" )]
		public bool NewCollateralOrderTerritory { get; set; }
		[DisplayName( "All orders for dealerships that I am the POC" )]
		public bool NewCollateralOrderMyDealers { get; set; }
		[DisplayName( "My shipments" )]
		public bool NewCollateralOrderShipment { get; set; }
		[DisplayName( "All shipments for my territory" )]
		public bool NewCollateralOrderShipmentTerritory { get; set; }
		[DisplayName( "All shipments for dealership that I am the POC" )]
		public bool NewCollateralOrderShipmentMyDealers { get; set; }

		[DisplayName( "New Spec Requests" )]
		public bool NewSpecRequest { get; set; }
		[DisplayName( "New Specification Request in My Territory" )]
		public bool NewSpecRequestTerritory { get; set; }
		[DisplayName( "New Specification Request for My Dealerships" )]
		public bool NewSpecRequestMyDealers { get; set; }

		[DisplayName( "Completed Spec Requests" )]
		public bool CompleteSpecRequest { get; set; }
		[DisplayName( "Completed Specification Request in My Territory" )]
		public bool CompleteSpecRequestTerritory { get; set; }
		[DisplayName( "Completed Specification Request for My Dealerships" )]
		public bool CompleteSpecRequestMyDealers { get; set; }

		[DisplayName( "Updates On Pending Spec Requests" )]
		public bool UpdateSpecRequest { get; set; }
		[DisplayName( "Updated Specification Request in My Territory" )]
		public bool UpdateSpecRequestTerritory { get; set; }
		[DisplayName( "Updated Specification Request for My Dealerships" )]
		public bool UpdateSpecRequestMyDealers { get; set; }

		[DisplayName( "Re-Opened Spec Requests" )]
		public bool ReOpenSpecRequest { get; set; }
		[DisplayName( "Re-Opened Specification Request in My Territory" )]
		public bool ReOpenSpecRequestTerritory { get; set; }
		[DisplayName( "Re-Opened Specification Request for My Dealerships" )]
		public bool ReOpenSpecRequestMyDealers { get; set; }
	}
}
