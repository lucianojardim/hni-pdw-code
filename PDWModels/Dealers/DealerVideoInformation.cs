using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PDWModels.Dealers
{
	public class DealerVideoInformation
	{
		public int VideoID { get; set; }
		[DisplayName("Display Name")]
		[Required]
		public string Name { get; set; }
		[DisplayName( "YouTube Video ID" )]
		[Required]
		public string YouTubeID { get; set; }
	}
}
