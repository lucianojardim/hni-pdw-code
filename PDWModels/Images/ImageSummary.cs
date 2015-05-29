using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Images
{
	public class ImageSummary
	{
		public int ImageID { get; set; }
		public string FileName { get; set; }
		public string Name { get; set; }
		public bool CanLightbox { get; set; }
		public string HiResFileName { get; set; }
		public string TypicalName { get; set; }
	}
}
