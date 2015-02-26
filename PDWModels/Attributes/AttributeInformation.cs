using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWModels.Images;

namespace PDWModels.Attributes
{
	public class AttributeOptionInformation
	{
		public string Name { get; set; }
		public ImageComboItem FeaturedImage { get; set; }
		public IEnumerable<string> Serieses { get; set; }
	}
}
