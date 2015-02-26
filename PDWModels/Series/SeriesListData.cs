using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWModels.Images;

namespace PDWModels.Series
{
	public class SeriesListData
	{
		public int SeriesID { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }
		public ImageComboItem ImageData { get; set; }
		public DateTime DateCreated { get; set; }
		public int Ranking { get; set; }
		public int Price { get; set; }
		public IEnumerable<string> Style { get; set; }
		public IEnumerable<string> Applications { get; set; }
		public bool IsInTwo { get; set; }
	}
}
