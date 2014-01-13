using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Collateral
{
	public class CollateralSummary
	{
		public int CollateralID { get; set; }
		public string Name { get; set; }
		public string CollateralType { get; set; }
		public string Status { get; set; }
		public int Quantity { get; set; }
		public bool IsGroup { get; set; }
		public string EditButtons { get; set; }
	}
}
