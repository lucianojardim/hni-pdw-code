using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWModels.Images;

namespace PDWModels.Series
{
	public class SeriesDocListData
	{
		public int SeriesID { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public Dictionary<string, string> Documents { get; set; }
		public ImageComboItem FeaturedImageFile { get; set; }
		public Dictionary<string, string> Details { get; set; }
	}
}
