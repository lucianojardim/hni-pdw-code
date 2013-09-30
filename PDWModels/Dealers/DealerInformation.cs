using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PDWModels.Dealers
{
	public class DealerInformation
	{
		public DealerInformation()
		{
			VideoList = new List<int>();
			ProductList = new List<string>();
			PageList = new List<int>();
		}

		public int DealerID { get; set; }
		[DisplayName("Dealer Name")]
		[Required]
		public string Name { get; set; }
		[DisplayName("URL")]
		[Required]
		public string URL { get; set; }
		[DisplayName("Featured Video")]
		public int FeaturedVideo { get; set; }
		[DisplayName("Main Content Block")]
		public string MainContent { get; set; }
		[DisplayName( "Products We Discussed" )]
		public List<string> ProductList { get; set; }
		[DisplayName( "Pages We Discussed" )]
		public List<int> PageList { get; set; }
		[DisplayName( "Videos You Missed" )]
		public List<int> VideoList { get; set; }
	}
}
