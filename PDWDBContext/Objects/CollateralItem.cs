using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWDBContext
{
	public partial class CollateralItem
	{
		public string StatusValue
		{
			get
			{
				if( ComputeQuantity <= 30 )
				{
					return "Backordered";
				}
				if( ComputeQuantity <= 150 )
				{
					return "Limited Quantity";
				}
				return "Available";
			}
		}

		public int ComputeQuantity
		{
			get
			{
				return IsGroup ? CollateralGroupItems.Min( cgi => cgi.ChildCollateralItem.Quantity / cgi.Quantity ) : Quantity;
			}
		}

		public bool NeedStatusDate
		{
			get
			{
				return ComputeQuantity <= 30;
			}
		}
	}
}
