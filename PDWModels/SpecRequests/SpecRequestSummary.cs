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
		public string RepGroup { get; set; }
		public string SpecTeamMember { get; set; }
		public string EditButtons { get; set; }
		public bool IsRecommended { get; set; }
		public bool IsPublished { get; set; }
		public string RecommendedStar { get; set; }
	}
}
