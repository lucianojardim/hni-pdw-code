using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PDWModels.eCollateral
{
	public class ECollateralSettings
	{
		public int ItemID { get; set; }
		public bool IsTemplate { get; set; }
		[DisplayName( "File Name" )]
		[Required]
		public string FileName { get; set; }
		[DisplayName( "Custom URL" )]
		[Required]
		public string CustomURL { get; set; }

		[DisplayName( "Sales Rep Territory" )]
		public int? PaoliSalesRepGroupID { get; set; }
		[DisplayName( "Dealership" )]
		public int? DealershipID { get; set; }
		[DisplayName( "Dealership Point of Contact" )]
		public int? DealershipPOCID { get; set; }
		[DisplayName( "Name" )]
		public string DealershipPOCName { get; set; }
		public string DealershipPOCEmail { get; set; }
		public string DealershipPOCPhone { get; set; }
		public int? DealershipPOCAcctType { get; set; }
		[DisplayName( "Customer Name" )]
		public string CustomerName { get; set; }
		[DisplayName( "Project Name" )]
		public string ProjectName { get; set; }

		public string PaoliSalesRepGroupName { get; set; }
		public string DealershipName { get; set; }
		public string DealershipPOCMember { get; set; }
		public string DealershipPOCAcctTypeName { get; set; }

	}
}
