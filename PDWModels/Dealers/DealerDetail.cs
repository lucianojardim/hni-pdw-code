using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Dealers
{
	public class DealerDetail
	{
		public class VideoDetail
		{
			public string Display { get; set; }
			public string VideoLink { get; set; }
		}

		public class PageDetail
		{
			public string Display { get; set; }
			public string URL { get; set; }
		}

		public class SeriesDetail
		{
			public string SeriesName { get; set; }
			public string FeaturedImageName { get; set; }
			public string LinkName { get { return SeriesName.Replace( ' ', '-' ); } }
		}

		public int DealerID { get; set; }
		public string Name { get; set; }
		public string URL { get; set; }
		public string FeaturedVideoLink { get; set; }
		public string MainContent { get; set; }
		public IEnumerable<VideoDetail> VideoLinks { get; set; }
		public IEnumerable<PageDetail> PageLinks { get; set; }
		public IEnumerable<SeriesDetail> ProductList { get; set; }
		public string ProductHeadline { get; set; }
		public string PageHeadline { get; set; }
		public string VideoHeadline { get; set; }
	}
}
