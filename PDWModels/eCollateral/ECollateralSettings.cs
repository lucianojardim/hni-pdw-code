﻿using System;
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
		[DisplayName( "Page Name" )]
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
		[DisplayName( "Customer" )]
		public int? CustomerID { get; set; }
		public string RealCustomerName { get; set; }
		[DisplayName( "Project Name" )]
		public int? ProjectID { get; set; }
		public string RealProjectName { get; set; }

		public string PaoliSalesRepGroupName { get; set; }
		public string DealershipName { get; set; }
		public string DealershipPOCMember { get; set; }
		public string Status { get; set; }
		public int UpdateStatus { get; set; }
		public string CreatedByUserName { get; set; }
	}
}
