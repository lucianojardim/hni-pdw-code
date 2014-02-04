using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWDBContext
{
	public partial class User
	{
		public string FullName
		{
			get
			{
				return FirstName + " " + LastName;
			}
		}

		public string FullNameWithCompany
		{
			get
			{
				return FullName + " (" + Company.Name + ")";
			}
		}
	}
}
