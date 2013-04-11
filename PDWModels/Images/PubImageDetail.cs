using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Images
{
	public class PubImageDetail
	{
		public string Name { get; set; }
		public int ImageID { get; set; }
		public string Thumbnail { get; set; }
		public int? PageNumber { get; set; }
	}
}
