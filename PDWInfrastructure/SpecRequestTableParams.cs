using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure
{
	public class SpecRequestTableParams : DataTableParams
	{
		public bool pendingOnly { get; set; }
		public bool recommendedOnly { get; set; }
		public bool notYetAssigned { get; set; }
		public bool auditSpecOnly { get; set; }
		public int? companyId { get; set; }
		public bool showGSAOnly { get; set; }
		public int? userId { get; set; }
	}
}
