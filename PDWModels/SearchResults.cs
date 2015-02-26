using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels
{
	public class SearchResults
	{
		public DateTime SearchDateTime { get; set; }
		public string SearchText { get; set; }
		public int SeriesCount { get; set; }
		public int ImageCount { get; set; }
		public int TypicalCount { get; set; }
		public int PageCount { get; set; }
	}
}
