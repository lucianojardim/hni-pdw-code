using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace PDWModels.Collateral
{
	public class CollateralGroupInformation : CollateralInformation
	{
		public class GroupInfoDetail
		{
			public int ItemID { get; set; }
			[Range( 1, int.MaxValue, ErrorMessage = "The field Quantity must be greater than 1." )]
			public int Quantity { get; set; }
		}

		public CollateralGroupInformation()
		{
			GroupItems = new List<GroupInfoDetail>();
		}

		public List<GroupInfoDetail> GroupItems { get; set; }
	}
}
