﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Companies
{
	public class MyTerritoryInfo
	{
		public int CompanyID { get; set; }
		public string CompanyName { get; set; }
		public int TerritoryID { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string PhoneNumber { get; set; }
		public string FaxNumber { get; set; }
		public string WebSite { get; set; }
	}
}
