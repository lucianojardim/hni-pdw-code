using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Companies
{
	public class CompanySummary
	{
		public int CompanyID { get; set; }
		public string Name { get; set; }
		public string CompanyType { get; set; }
		public string MasterID { get; set; }
		public string BaseNumber { get; set; }
		public int UserCount { get; set; }
		public string EditButtons { get; set; }
		public bool CanDelete { get; set; }
	}
}
