using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Publications
{
	public class PublicationSummary
	{
		public int PublicationID { get; set; }
		public string Name { get; set; }
		public string PubDate { get; set; }
		public int ImageCt { get; set; }
	}
}
