﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure
{
	public class UserTableParams : DataTableParams
	{
		public int accountType { get; set; }
		public int? territoryId { get; set; }
	}
}
