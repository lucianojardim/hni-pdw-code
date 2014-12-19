using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWModels.Images;

namespace PDWModels.Attributes
{
	public class FinishInformation
	{
		public string Name { get; set; }
		public int FinishType { get; set; }
		public int LaminatePattern { get; set; }
		public bool LaminateIsTFL { get; set; }
		public bool LaminateIsHPL { get; set; }
		public int VeneerGrade { get; set; }
		public int VeneerSpecies { get; set; }
		public int VeneerColorTone { get; set; }
		public int SeatingGrade { get; set; }
		public ImageComboItem FeaturedImage { get; set; }
		public IEnumerable<string> Serieses { get; set; }
		public string Caption { get; set; }
		public int Popularity { get; set; }
	}
}
