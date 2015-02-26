using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PDWModels.Collateral
{
	public class ShipmentInformation
	{
		public int ShipmentID { get; set; }
		[DisplayName( "Vendor" )]
		public string Vendor { get; set; }
		[DisplayName( "Tracking Numbers" )]
		public string TrackingNumbers { get; set; }
		[DisplayName( "GL Code" )]
		public string GLCode { get; set; }
		[DisplayName( "Shipping Priority" )]
		public string ShippingType { get; set; }
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
		[DisplayName( "Shipment Date" )]
		public DateTime ShippingDate { get; set; }
		public int ShippingTypeID { get; set; }

		[DisplayName( "Order Date" )]
		public DateTime? OrderDate { get; set; }

		public string RPUserName { get; set; }
		public string RPCompany { get; set; }
		public string RPEmailAddress { get; set; }
		public string RPPhoneNumber { get; set; }
		public string SPUserName { get; set; }
		public string SPCompany { get; set; }
		public string SPEmailAddress { get; set; }
		public string SPPhoneNumber { get; set; }

		public List<PendingOrderInformation.ShipmentDetailSummary> Details { get; set; }
	}
}
