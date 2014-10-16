using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWDBContext
{
	public partial class Territory
	{
		private static class PaoliCompanyType
		{
			public const int PaoliRepGroup = 2;	// defined in PDWInfrastructure.PaoliWebUser
		}

		public string SalesRepCompanyName
		{
			get
			{
				var salesRep = Companies.FirstOrDefault( c => c.CompanyType == PaoliCompanyType.PaoliRepGroup );
				if( salesRep != null )
				{
					return salesRep.FullName;
				}

				return "";
			}
		}

		public int? SalesRepCompanyID
		{
			get
			{
				var salesRep = Companies.FirstOrDefault( c => c.CompanyType == PaoliCompanyType.PaoliRepGroup );
				if( salesRep != null )
				{
					return salesRep.CompanyID;
				}

				return null;
			}
		}
	}
}
