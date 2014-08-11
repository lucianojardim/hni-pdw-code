using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.eCollateral
{
	public class ECollateralDetails
	{
		public ECollateralDetails()
		{
			Details = new List<DetailItem>();
		}

		public class DetailItem
		{
			public DetailItem()
			{
			}

			public int? ImageID { get; set; }
			public string Content { get; set; }
			public string ImageFileName { get; set; }
		}

		public int ItemID { get; set; }
		public string Name { get; set; }
		public string LayoutName { get; set; }
		public string EditViewName { get; set; }
		public string DisplayViewName { get; set; }
		public List<DetailItem> Details { get; set; }
	}
}
