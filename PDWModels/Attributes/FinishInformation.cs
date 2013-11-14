using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWModels.Images;

namespace PDWModels.Attributes
{
	public class FinishInformation
	{
		public string Name { get; set; }
		public int FinishType { get; set; }
		public int FinishSubType { get; set; }
		public ImageComboItem FeaturedImage { get; set; }
		public IEnumerable<string> Serieses { get; set; }
		public string Caption { get; set; }
	}
}
