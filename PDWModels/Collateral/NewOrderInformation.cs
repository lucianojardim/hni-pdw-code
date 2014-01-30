using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PDWInfrastructure;

namespace PDWModels.Collateral
{
	public class NewOrderInformation
	{
		public const int RPPaoliMember = 0;
		public const int RPPaoliRepresentative = 1;
		public const int RPDealer = 2;
		public const int RPEndUser = 3;

		public static Dictionary<int, string> RequestingParties
		{
			get
			{
				return new Dictionary<int, string>() {
					{ RPPaoliMember, "Paoli Member" },				
					{ RPPaoliRepresentative, "Paoli Representative" },				
					{ RPDealer, "Dealer" },				
					{ RPEndUser, "End User / Customer" },				
                };
			}
		}

		public const int STGround = 0;
		public const int ST2DayFedex = 1;
		public const int STOvernightFedex = 2;

		public static Dictionary<int, string> ShippingTypes
		{
			get
			{
				return new Dictionary<int, string>()
				{
					{ STGround, "Ground (5-7 Business Days) - Free" },
					{ ST2DayFedex, "2-Day (Fedex Acct. Required)" },
					{ STOvernightFedex, "Priority Overnight (Fedex Acct. Required)" }
				};
			}
		}

		public class OrderDetail
		{
			public int CollateralID { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public string Status { get; set; }
			public int Quantity { get; set; }
			public int CollateralTypeID { get; set; }
		}

		public int OrderID { get; set; }
		[DisplayName( "Who is making this request?" )]
		public int RequestingParty { get; set; }
		[DisplayName( "Paoli Member" )]
		public int? PaoliMemberID { get; set; }
		[DisplayName( "Paoli Rep Group" )]
		public int? PaoliRepGroupID { get; set; }
		[DisplayName( "Paoli Rep" )]
		public int? PaoliRepGroupMemberID { get; set; }
		[DisplayName( "Dealership Name" )]
		public int? DealerID { get; set; }
		[DisplayName( "Dealership Point of Contact" )]
		public int? DealerMemberID { get; set; }
		[DisplayName( "First Name" )]
		public string EndUserFirstName { get; set; }
		[DisplayName( "Last Name" )]
		public string EndUserLastName { get; set; }
		[DisplayName( "End User POC Direct Phone Number" )]
		public string EndUserPhoneNumber { get; set; }
		[DisplayName( "End User POC Email Address" )]
		public string EndUserEMailAddress { get; set; }

		public List<OrderDetail> OrderDetails { get; set; }

		[DisplayName( "Shipping Priority" )]
		public int ShippingType { get; set; }
		[DisplayName( "Which Address do you want to ship to?" )]
		public int ShippingAddressType { get; set; }
		[DisplayName( "Fedex Account Number" )]
		public string ShippingFedexAccount { get; set; }
		[DisplayName( "First Name" )]
		public string ShippingFirstName { get; set; }
		[DisplayName( "Last Name" )]
		public string ShippingLastName { get; set; }
		[DisplayName( "Company" )]
		public string ShippingCompanyName { get; set; }
		[DisplayName( "Address 1" )]
		[StringLength( 100 )]
		public string ShippingAddress1 { get; set; }
		[DisplayName( "Address 2" )]
		[StringLength( 100 )]
		public string ShippingAddress2 { get; set; }
		[DisplayName( "City" )]
		[StringLength( 50 )]
		public string ShippingCity { get; set; }
		[DisplayName( "State" )]
		public string ShippingState { get; set; }
		[DisplayName( "Zip Code" )]
		[PostalCode]
		public string ShippingZip { get; set; }
		[DisplayName( "Phone Number" )]
		[PhoneNumber]
		public string ShippingPhoneNumber { get; set; }
		[DisplayName( "Email" )]
		[Email]
		public string ShippingEmailAddress { get; set; }

		[DisplayName( "Order Date" )]
		public DateTime? OrderDate { get; set; }
	}
}
