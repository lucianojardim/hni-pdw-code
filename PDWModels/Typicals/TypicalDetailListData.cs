using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Typicals
{
	public class TypicalDetailListData
	{
		public int TypicalID { get; set; }
		public string Name { get; set; }
		public string ImageFileName { get; set; }
		public int Price { get; set; }
		public IEnumerable<string> Footprints { get; set; }
	}
}
