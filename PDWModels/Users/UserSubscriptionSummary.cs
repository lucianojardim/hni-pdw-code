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
			NewCollateralOrderShipment = true;
			NewCollateralOrderShipmentTerritory = true;
			NewSpecRequest = true;
			NewSpecRequestTerritory = true;
			CompleteSpecRequest = true;
			CompleteSpecRequestTerritory = true;
			UpdateSpecRequest = true;
			UpdateSpecRequestTerritory = true;
			ReOpenSpecRequest = true;
			ReOpenSpecRequestTerritory = true;
		}

		public int UserID { get; set; }

		[DisplayName( "New Collateral Order" )]
		public bool NewCollateralOrder { get; set; }
		[DisplayName( "New Collateral Order in My Territory" )]
		public bool NewCollateralOrderTerritory { get; set; }
		[DisplayName( "New Collateral Order Shipment" )]
		public bool NewCollateralOrderShipment { get; set; }
		[DisplayName( "New Collateral Order Shipment in My Territory" )]
		public bool NewCollateralOrderShipmentTerritory { get; set; }

		[DisplayName( "New Specification Request" )]
		public bool NewSpecRequest { get; set; }
		[DisplayName( "New Specification Request in My Territory" )]
		public bool NewSpecRequestTerritory { get; set; }

		[DisplayName( "Completed Specification Request" )]
		public bool CompleteSpecRequest { get; set; }
		[DisplayName( "Completed Specification Request in My Territory" )]
		public bool CompleteSpecRequestTerritory { get; set; }

		[DisplayName( "Updated Specification Request" )]
		public bool UpdateSpecRequest { get; set; }
		[DisplayName( "Updated Specification Request in My Territory" )]
		public bool UpdateSpecRequestTerritory { get; set; }

		[DisplayName( "Re-Opened Specification Request" )]
		public bool ReOpenSpecRequest { get; set; }
		[DisplayName( "Re-Opened Specification Request in My Territory" )]
		public bool ReOpenSpecRequestTerritory { get; set; }
	}
}
