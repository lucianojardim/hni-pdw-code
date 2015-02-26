using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PDWModels.Collateral
{
	public class CollateralShipmentDetail
	{
		public int CollateralID { get; set; }
		[Range( 1, int.MaxValue, ErrorMessage = "The field Quantity must be greater than 1." )]
		public int Quantity { get; set; }
	}
}
