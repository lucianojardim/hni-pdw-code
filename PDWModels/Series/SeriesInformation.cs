using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Series
{
	public class SeriesInformation
	{
		public class RelatedSeriesInfo
		{
			public int SeriesID { get; set; }
			public string Name { get; set; }
			public string ImageFileName { get; set; }
		}

		public int SeriesID { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public string FeaturedImageFileName { get; set; }
		public IEnumerable<string> Images { get; set; }
		public Dictionary<string, IEnumerable<string>> Options { get; set; }
		public Dictionary<string, IEnumerable<string>> Details { get; set; }
		public IEnumerable<RelatedSeriesInfo> RelatedSeries { get; set; }
	}
}
