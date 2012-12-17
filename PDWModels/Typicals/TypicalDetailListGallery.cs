using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Typicals
{
	public class TypicalDetailListGallery
	{
		public int TotalListCount { get; set; }
		public int FilteredListCount { get; set; }
		public IEnumerable<TypicalDetailListData> Typicals { get; set; }
	}
}
