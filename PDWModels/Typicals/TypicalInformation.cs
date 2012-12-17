using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Typicals
{
	public class TypicalInformation
	{
		public int TypicalID { get; set; }
		public string Name { get; set; }
		public string Series { get; set; }
		public string Category { get; set; }
		public string FeaturedImageFileName { get; set; }
		public IEnumerable<string> Images { get; set; }
		public Dictionary<string, IEnumerable<string>> Options { get; set; }
		public Dictionary<string, IEnumerable<string>> Details { get; set; }
	}
}
