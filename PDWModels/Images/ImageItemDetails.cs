﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Images
{
	public class ImageItemDetails
	{
		public string FileName { get; set; }
		public string Caption { get; set; }
		public string Description { get; set; }
		public List<string> SeriesList { get; set; }
		public string HiResFileName { get; set; }
	}
}