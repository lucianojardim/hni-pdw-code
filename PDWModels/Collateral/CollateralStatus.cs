using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Collateral
{
	public static class CollateralStatus
	{
		public const int Available = 1;
		public const int Backordered = 2;
		public const int Discontinued = 3;

		public static Dictionary<int, string> DisplayStrings
		{
			get
			{
				return new Dictionary<int, string>()
				{
					{ Available, "Available" },
					{ Backordered, "Backordered" },
					{ Discontinued, "Discontinued" },
				};
			}
		}
	}
}
