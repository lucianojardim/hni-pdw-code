using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWModels.Images;

namespace PDWModels.Series
{
	public class SeriesSearchResult
	{
		public int SeriesID { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public ImageComboItem ImageData { get; set; }
		public string Headline { get; set; }
	}
}
