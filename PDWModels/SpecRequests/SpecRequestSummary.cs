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
		public string DealerMember { get; set; }
		public string ProjectName { get; set; }
		public string SpecTeamMember { get; set; }
		public string SalesRepGroup { get; set; }
		public bool IsRecommended { get; set; }
		public bool IsPublished { get; set; }
		public bool IsCompleted { get; set; }
		public bool IsCanceled { get; set; }
		public bool IsAuditSpecOnly { get; set; }
		public bool HasTypical { get; set; }
		public DateTime? CreatedDate { get; set; }
		public double CreatedDateMilliseconds { get { return CreatedDate.HasValue ? ( CreatedDate.Value - ( new DateTime( 1970, 1, 1 ) ) ).TotalMilliseconds : 0; } }

		public string Status { get; set; }
		public string TypicalFinder { get; set; }
		public string EditButtons { get; set; }
	}
}
