using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Companies
{
	public class DealerTripSummary
	{
		public int CompanyID { get; set; }
		public string Name { get; set; }
		public string GroupName { get; set; }
		public int NumTrips { get; set; }
		public string TotalSales { get; set; }
	}
}
