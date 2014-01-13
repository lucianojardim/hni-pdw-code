using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PDWModels.Collateral
{
	public class CollateralInformation
	{
		public int CollateralID { get; set; }
		[DisplayName( "Title" )]
		[Required]
		[StringLength( 100 )]
		public string Name { get; set; }
		[DisplayName( "Type" )]
		[Required]
		public int CollateralTypeID { get; set; }
		[DisplayName( "Description" )]
		public string Description { get; set; }
		[DisplayName( "Lead Time" )]
		[StringLength( 20 )]
		public string LeadTime { get; set; }
		[DisplayName( "Weight" )]
		[Range( 0, float.MaxValue, ErrorMessage = "The field Weight must be greater than or equal to 0." )]
		public double? Weight { get; set; }
		[DisplayName( "Status" )]
		[Required]
		public int Status { get; set; }
		[DisplayName( "Backordered Until" )]
		public DateTime? StatusDate { get; set; }
		[DisplayName( "Quantity" )]
		[Required]
		public int Quantity { get; set; }
		[DisplayName( "Image" )]
		public string ImageFileName { get; set; }
		[DisplayName( "Price per Item" )]
		public double? Price { get; set; }
		[DisplayName( "Shipping per Item" )]
		public double? Shipping { get; set; }
	}
}
