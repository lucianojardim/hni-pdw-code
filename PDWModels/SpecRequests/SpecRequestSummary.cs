using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.SpecRequests
{
	public class SpecRequestSummary
	{
		public int RequestID { get; set; }
		public string Name { get; set; }
		public string Dealer { get; set; }
		public string ProjectName { get; set; }
		public string SpecTeamMember { get; set; }
		public string SalesRepGroup { get; set; }
		public bool IsRecommended { get; set; }
		public bool IsPublished { get; set; }
		public bool IsCompleted { get; set; }

		public string Status { get; set; }
		public string TypicalFinder { get; set; }
		public string EditButtons { get; set; }
	}
}
