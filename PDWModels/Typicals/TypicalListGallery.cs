using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Typicals
{
	public class TypicalListGallery
	{
		public int TotalListCount { get; set; }
		public int FilteredListCount { get; set; }
		public IEnumerable<TypicalListData> Typicals { get; set; }
	}
}
