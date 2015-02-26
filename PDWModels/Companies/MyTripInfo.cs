using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Companies
{
	public class MyTripInfo
	{
		public bool CanSeePerfData { get; set; }
		public string YTDSales { get; set; }
		public int TripEarned { get; set; }
		public string SalesToNextTrip { get; set; }

		public string TripGroup { get; set; }
		public int TripGroupCount { get; set; }

		public string NetGrowth { get; set; }
		public string PercNetGrowth { get; set; }
		public int NetGrowthRank { get; set; }
		public int PercNetGrowthRank { get; set; }

		public DateTime ImportDate { get; set; }
	}
}
