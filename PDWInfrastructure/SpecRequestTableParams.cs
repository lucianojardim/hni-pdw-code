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
	}
}
