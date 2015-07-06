using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure
{
	public class CompanyTableParams : DataTableParams
	{
		public int companyType { get; set; }
		public int? territoryId { get; set; }
		public bool tripOnly { get; set; }
		public bool activeOnly { get; set; }
	}
}
