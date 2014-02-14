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

		public string ContactInfo
		{
			get
			{
				return (BusinessPhone ?? "") + ((BusinessPhone != null || Email != null) ? " | " : "") + (Email ?? "");
			}
		}

		public string FullNameWithCompany
		{
			get
			{
				return Company.Name + " - " + FullName;
			}
		}
	}
}
