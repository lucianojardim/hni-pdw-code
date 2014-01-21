using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWModels.Images;

namespace PDWModels.Typicals
{
	public class TypicalListData
	{
		public int TypicalID { get; set; }
		public string Name { get; set; }
		public int SeriesID { get; set; }
		public ImageComboItem ImageFileData { get; set; }
	}
}
