using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWModels.Images;

namespace PDWModels.Typicals
{
	public class TypicalInformation
	{
		public int TypicalID { get; set; }
		public string Name { get; set; }
		public string Series { get; set; }
		public string Category { get; set; }
		public ImageComboItem FeaturedImageFileData { get; set; }
		public IEnumerable<ImageComboItem> Images { get; set; }
		public Dictionary<string, IEnumerable<string>> Options { get; set; }
		public Dictionary<string, IEnumerable<string>> Details { get; set; }
	}
}
