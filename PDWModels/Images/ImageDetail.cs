using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Images
{
	public class ImageDetail
	{
		public string Name { get; set; }
		public string Caption { get; set; }
		public string Keywords { get; set; }
		public int ImageID { get; set; }
		public bool HasReferences { get; set; }
		public string Thumbnail { get; set; }
	}
}
