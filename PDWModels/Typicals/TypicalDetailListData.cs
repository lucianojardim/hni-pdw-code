using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWModels.Images;

namespace PDWModels.Typicals
{
	public class TypicalDetailListData
	{
		public int TypicalID { get; set; }
		public string Name { get; set; }
		public ImageComboItem ImageFileData { get; set; }
		public int Price { get; set; }
		public IEnumerable<string> Footprints { get; set; }
		public IEnumerable<string> SeriesList { get; set; }
	}
}
