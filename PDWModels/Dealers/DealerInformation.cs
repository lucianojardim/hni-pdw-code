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
		[DisplayName( "Product Listing" )]
		public List<string> ProductList { get; set; }
		[DisplayName( "Page Listing" )]
		public List<int> PageList { get; set; }
		[DisplayName( "Video Listing" )]
		public List<int> VideoList { get; set; }
		[DisplayName( "Product List Headline" )]
		public string ProductHeadline { get; set; }
		[DisplayName( "Page List Headline" )]
		public string PageHeadline { get; set; }
		[DisplayName( "Video List Headline" )]
		public string VideoHeadline { get; set; }
	}
}
