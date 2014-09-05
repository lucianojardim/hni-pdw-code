using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web;

namespace PDWModels.SpecRequests
{
	public class ReOpenRequestInformation
	{
		public int RequestID { get; set; }
		public string Name { get; set; }

		[DisplayName( "Related Drawings or Notes" )]
		public List<HttpPostedFileBase> addlFiles { get; set; }

		[DisplayName( "Notes and Directions for Spec Team" )]
		public string Notes { get; set; }

	}
}
