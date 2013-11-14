using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWModels.Images;

namespace PDWModels.Attributes
{
	public class CMInformation
	{
		public string Name { get; set; }
		public ImageComboItem FeaturedImage { get; set; }
		public IEnumerable<string> Serieses { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
	}
}
