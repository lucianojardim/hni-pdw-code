using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PDWModels.Publications
{
	public class PublicationInformation
	{
		[Required]
		public int PublicationID { get; set; }
		[Required]
		[DisplayName("Name")]
		public string Name { get; set; }
		[Required]
		[DisplayName( "Publication Date" )]
		[DataType( DataType.Date )]
		public DateTime PublicationDate { get; set; }
		[DisplayName( "Visible in Image Filter?" )]
		public bool FilterVisible { get; set; }
	}
}
