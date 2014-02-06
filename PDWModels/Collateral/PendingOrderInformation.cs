using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PDWModels.Collateral
{
	public class PendingOrderInformation
	{
		public class ShipmentDetailSummary
		{
			public int OrderDetailID { get; set; }
			public string Name { get; set; }
			public int Quantity { get; set; }
		}

		public class ShipmentSummary
		{
			public int ShipmentID { get; set; }
			public string Vendor { get; set; }
			public string TrackingNumber { get; set; }
			public string GLCode { get; set; }
			public string ShippingType { get; set; }
			public string ShippingFedexAccount { get; set; }
			public string ShippingAttn { get; set; }
			public string ShippingCompanyName { get; set; }
			public string ShippingAddress1 { get; set; }
			public string ShippingAddress2 { get; set; }
			public string ShippingCity { get; set; }
			public string ShippingState { get; set; }
			public string ShippingZip { get; set; }
			public string ShippingPhoneNumber { get; set; }
			public string ShippingEmailAddress { get; set; }

			public List<ShipmentDetailSummary> Details { get; set; }
		}

		public class PendingOrderDetail
		{
			public int DetailID { get; set; }
			public int CollateralID { get; set; }
			public string Name { get; set; }
			public int Quantity { get; set; }
			public int RemainingQuantity { get; set; }
			public int? GroupID { get; set; }
			public string GroupName { get; set; }
			public int CollateralTypeID { get; set; }
			public string CollateralType { get; set; }
		}

		public int OrderID { get; set; }
		[DisplayName( "Who is making this request?" )]
		public string RequestingParty { get; set; }
		public int RequestingPartyID { get; set; }
		[DisplayName( "Paoli Member" )]
		public string PaoliMember { get; set; }
		[DisplayName( "Paoli Rep Group" )]
		public string PaoliRepGroup { get; set; }
		[DisplayName( "Paoli Rep" )]
		public string PaoliRepGroupMember { get; set; }
		[DisplayName( "Dealership Name" )]
		public string Dealer { get; set; }
		[DisplayName( "Dealership Point of Contact" )]
		public string DealerMember { get; set; }
		[DisplayName( "First Name" )]
		public string EndUserFirstName { get; set; }
		[DisplayName( "Last Name" )]
		public string EndUserLastName { get; set; }
		[DisplayName( "End User POC Direct Phone Number" )]
		public string EndUserPhoneNumber { get; set; }
		[DisplayName( "End User POC Email Address" )]
		public string EndUserEMailAddress { get; set; }

		public List<PendingOrderDetail> OrderDetails { get; set; }

		[DisplayName( "Shipping Priority" )]
		public string ShippingType { get; set; }
		public int ShippingTypeID { get; set; }
		[DisplayName( "Fedex Account Number" )]
		public string ShippingFedexAccount { get; set; }
		[DisplayName( "Attn To" )]
		public string ShippingAttn { get; set; }
		[DisplayName( "Company" )]
		public string ShippingCompanyName { get; set; }
		[DisplayName( "Address 1" )]
		public string ShippingAddress1 { get; set; }
		[DisplayName( "Address 2" )]
		public string ShippingAddress2 { get; set; }
		[DisplayName( "City" )]
		public string ShippingCity { get; set; }
		[DisplayName( "State" )]
		public string ShippingState { get; set; }
		[DisplayName( "Zip Code" )]
		public string ShippingZip { get; set; }
		[DisplayName( "Phone Number" )]
		public string ShippingPhoneNumber { get; set; }
		[DisplayName( "Email" )]
		public string ShippingEmailAddress { get; set; }

		[DisplayName( "Order Date" )]
		public DateTime? OrderDate { get; set; }
		[DisplayName( "Order Status" )]
		public string Status { get; set; }

		public string CreatedByUserName { get; set; }
		public string CreatedByCompany { get; set; }
		public string CreatedByEmailAddress { get; set; }
		public string CreatedByPhoneNumber { get; set; }

		public string CanceledByUserName { get; set; }
		public DateTime? CanceledOnDateTime { get; set; }

		public List<ShipmentSummary> Shipments { get; set; }
	}
}
