using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PDWModels.Images
{
	public class ImageUsage
	{
		public ImageUsage()
		{
			Serieses = new List<string>();
			Typicals = new List<string>();
			Showrooms = new List<string>();
			ECollateral = new List<string>();
		}

		[DisplayName( "Serieses" )]
		public IEnumerable<string> Serieses { get; set; }
		[DisplayName( "Typicals" )]
		public IEnumerable<string> Typicals { get; set; }
		[DisplayName( "Showrooms" )]
		public IEnumerable<string> Showrooms { get; set; }
		[DisplayName( "eCollateral Pages" )]
		public IEnumerable<string> ECollateral { get; set; }
	}
}
