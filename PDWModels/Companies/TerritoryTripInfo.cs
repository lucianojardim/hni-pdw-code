using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Companies
{
	public class TerritoryTripInfo
	{
		public int TerritoryID { get; set; }
		public int NewProductPoints { get; set; }
		public int FocusDealerPoints { get; set; }
		public int CurrentTerritoryVol { get; set; }
		public int TerritoryPoints { get; set; }
		public int NumDealerTripsAwarded { get; set; }
		public int DealerTripPoints { get; set; }
		public int EducationPoints { get; set; }
		public int TotalTripPoints { get; set; }
		public DateTime ImportDate { get; set; }
	}
}
