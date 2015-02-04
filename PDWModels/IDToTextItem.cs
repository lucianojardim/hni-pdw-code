using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels
{
	public class IDToTextItem
	{
		public int ID { get; set; }
		public string Text { get; set; }
	}

	public class IDToTextItemExtra : IDToTextItem
	{
		public string Extra { get; set; }
	}
}
