using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Collateral
{
	public class CollateralOrderSummary
	{
		public int OrderID { get; set; }
		public DateTime OrderDate { get; set; }
		public double OrderDateMilliseconds { get { return ( OrderDate - ( new DateTime( 1970, 1, 1 ) ) ).TotalMilliseconds; } }
		public string RequestingParty { get; set; }
		public string Status { get; set; }
		public string EditButtons { get; set; }
		public bool CanEdit { get; set; }
	}
}
