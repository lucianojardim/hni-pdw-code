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
		public int UserID { get; set; }
		[DisplayName( "Product Introductions & Announcements (A few times each year)" )]
		public bool ProductIntroductions { get; set; }
		[DisplayName( "Behind The Scenes (2-3 per month)" )]
		public bool BehindTheScenes { get; set; }
		[DisplayName( "Meet Our Members (1 per month)" )]
		public bool MeetOurMembers { get; set; }
		[DisplayName( "Program Changes" )]
		public bool ProgramChanges { get; set; }
		[DisplayName( "Pricelist Updates" )]
		public bool PricelistUpdates { get; set; }
		[DisplayName( "Quote Requests & Rendering Status Notifications" )]
		public bool QuoteRequests { get; set; }
		[DisplayName( "Yes, please contact me at:" )]
		public bool SMSAlerts { get; set; }
		[PhoneNumber]
		[DisplayName( "SMS Phone Number" )]
		public string SMSPhoneNumber { get; set; }
	}
}
