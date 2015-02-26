﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure
{
	public class CollateralOrderTableParams : DataTableParams
	{
		public bool hideFulfilled { get; set; }
		public int? companyId { get; set; }
		public int? userId { get; set; }
	}
}
