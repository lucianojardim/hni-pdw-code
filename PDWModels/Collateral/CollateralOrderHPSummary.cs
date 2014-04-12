using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Collateral
{
	public class CollateralOrderHPSummary : CollateralOrderSummary
	{
		public string RPName { get; set; }
		public string RPCompany { get; set; }
		public string SPName { get; set; }
		public string SPCompany { get; set; }
	}
}
