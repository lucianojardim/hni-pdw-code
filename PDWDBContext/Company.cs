using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWDBContext
{
	public partial class Company
	{
		public string FullName
		{
			get
			{
				return Name + ((BusinessUnitName ?? "").Any() ? (" - " + BusinessUnitName) : "" );
			}
		}

		public string FullNameWithTerritory
		{
			get
			{
				return ( TerritoryID.HasValue ? (TerritoryID.ToString() + " - ") : "" ) + FullName;
			}
		}
	}
}
