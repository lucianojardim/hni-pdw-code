using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Collateral
{
	public class CollateralOrderExport
	{
		public int OrderID { get; set; }
		public string RequestingParty { get; set; }
		public string EndUserFirstName { get; set; }
		public string EndUserLastName { get; set; }
		public string EndUserPhoneNumber { get; set; }
		public string EndUserEMailAddress { get; set; }

		public string ShippingType { get; set; }
		public string ShippingAttn { get; set; }
		public string ShippingCompanyName { get; set; }
		public string ShippingAddress1 { get; set; }
		public string ShippingAddress2 { get; set; }
		public string ShippingCity { get; set; }
		public string ShippingState { get; set; }
		public string ShippingZip { get; set; }
		public string ShippingPhoneNumber { get; set; }
		public string ShippingEmailAddress { get; set; }

		public DateTime? OrderDate { get; set; }
		public string Status { get; set; }

		public string CreatedByUserName { get; set; }
		public string CreatedByCompany { get; set; }

		public string RPUserName { get; set; }
		public string RPCompany { get; set; }
		public string SPUserName { get; set; }
		public string SPCompany { get; set; }

		public string CanceledByUserName { get; set; }
		public DateTime? CanceledOnDateTime { get; set; }

		public List<string> ItemNames { get; set; }
	}
}
