﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Images
{
	public class ImageItemDetails
	{
		public class ImageSeries
		{
			public int SeriesID { get; set; }
			public string Name { get; set; }
			public bool IsFeatured { get; set; }
			public IEnumerable<string> TypicalList { get; set; }
		}

		public int ImageID { get; set; }
		public string FileName { get; set; }
		public string Caption { get; set; }
		public string Name { get; set; }
		public string SecondaryName { get; set; }
		public IEnumerable<ImageSeries> SeriesList { get; set; }
		public string HiResFileName { get; set; }
		public string FeaturedSeries { get; set; }
	}
}
