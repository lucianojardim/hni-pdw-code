using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PDWModels.Dealers
{
	public class DealerPageInformation
	{
		public int PageID { get; set; }
		[DisplayName( "Display Name" )]
		[Required]
		public string Name { get; set; }
		[DisplayName( "URL Location" )]
		[Required]
		public string URLLocation { get; set; }
	}
}
