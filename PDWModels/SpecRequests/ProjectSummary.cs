using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.SpecRequests
{
	public class ProjectSummary
	{
		public int ProjectID { get; set; }
		public string ProjectName { get; set; }
		public string CustomerName { get; set; }
		public string DealershipName { get; set; }
		public string PipelineStatus { get; set; }
	}
}
