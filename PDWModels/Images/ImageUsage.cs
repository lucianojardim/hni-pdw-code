using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PDWModels.Images
{
	public class ImageUsage
	{
		public class ECollateralUsage
		{
			public string Name { get; set; }
			public bool IsComplete { get; set; }
			public bool HasSections { get; set; }
			public bool HasLayout { get; set; }
			public int ItemID { get; set; }
			public string URL
			{
				get
				{
					if( IsComplete ) {
						return "/ePublisher/ViewLayout/" + ItemID;
					} else if( HasSections ) {
						return "/ePublisher/VerifyLayout/" + ItemID;
					} else if( HasLayout ) {
						return "/ePublisher/EditLayout/" + ItemID;
					}
					return "/ePublisher/SetLayout/" + ItemID;

				}
			}
		}

		public ImageUsage()
		{
			Serieses = new List<string>();
			Typicals = new List<string>();
			Showrooms = new List<string>();
			ECollateral = new List<ECollateralUsage>();
		}

		[DisplayName( "Serieses" )]
		public IEnumerable<string> Serieses { get; set; }
		[DisplayName( "Typicals" )]
		public IEnumerable<string> Typicals { get; set; }
		[DisplayName( "Showrooms" )]
		public IEnumerable<string> Showrooms { get; set; }
		[DisplayName( "eCollateral Pages" )]
		public IEnumerable<ECollateralUsage> ECollateral { get; set; }
	}
}
