using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure
{
	public class CollateralTableParams : DataTableParams
	{
		public int collateralType { get; set; }
		public bool showInactive { get; set; }
	}
}
