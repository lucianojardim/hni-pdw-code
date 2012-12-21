using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWModels.Images;

namespace PDWModels.Series
{
	public class SeriesInformation
	{
		public class RelatedSeriesInfo
		{
			public int SeriesID { get; set; }
			public string Name { get; set; }
			public ImageComboItem ImageFileData { get; set; }
		}

		public class TypicalInfoForSeries
		{
			public int TypicalID { get; set; }
			public string Name { get; set; }
			public ImageComboItem ImageFileData { get; set; }
		}

		public int SeriesID { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public ImageComboItem FeaturedImageFile { get; set; }
		public IEnumerable<ImageComboItem> Images { get; set; }
		public Dictionary<string, IEnumerable<string>> Options { get; set; }
		public Dictionary<string, IEnumerable<string>> Details { get; set; }
		public IEnumerable<RelatedSeriesInfo> RelatedSeries { get; set; }
		public IEnumerable<TypicalInfoForSeries> Typicals { get; set; }
	}
}
