using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.eCollateral
{
	public class ECollateralSummary
	{
		public int ItemID { get; set; }
		public string FileName { get; set; }
		public DateTime LastModifiedDate { get; set; }
		public double LastModifiedDateMilliseconds { get { return ( LastModifiedDate - ( new DateTime( 1970, 1, 1 ) ) ).TotalMilliseconds; } }
		public string AuthorName { get; set; }
		public string AuthorImage { get; set; }
		public string Dealership { get; set; }
		public string CustomerName { get; set; }
		public string ProjectName { get; set; }
		public string ContentType { get; set; }
		public string LayoutImage { get; set; }
		public string LayoutName { get; set; }
		public string Status { get; set; }
		public bool HasLayout { get; set; }
		public bool HasSections { get; set; }
		public bool IsComplete { get; set; }
	}
}
