using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.SpecRequests
{
	public class NewProjectInformation
	{
		public string projectName { get; set; }
		public int? customer { get; set; }
		public int dealer { get; set; }
		public bool isGSA { get; set; }
		public int? contractId { get; set; }
	}
}
