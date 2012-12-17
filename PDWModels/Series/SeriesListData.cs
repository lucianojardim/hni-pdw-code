using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Series
{
	public class SeriesListData
	{
		public int SeriesID { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public string ImageFileName { get; set; }
		public DateTime DateCreated { get; set; }
		public int Ranking { get; set; }
		public IEnumerable<string> Style { get; set; }
	}
}
