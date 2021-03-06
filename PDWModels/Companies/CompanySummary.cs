﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Companies
{
	public class CompanySummary
	{
		public int CompanyID { get; set; }
		public string Name { get; set; }
		public string CompanyType { get; set; }
		public string MasterID { get; set; }
		public string BaseNumber { get; set; }
		public string BusinessUnitName { get; set; }
		public int UserCount { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public bool SignedUpForTrip { get; set; }
		public string TripGroupInfo { get; set; }
		public string PSRContact { get; set; }
		public int? PSRContactID { get; set; }
		public string TierGroup { get; set; }
		public string EditButtons { get; set; }
		public bool CanDelete { get; set; }
		public int? TerritoryID { get; set; }
		public int DealerCount { get; set; }
		public bool IsDisabled { get; set; }
		public string Location
		{
			get
			{
				if( !string.IsNullOrEmpty( City ) )
				{
					if( !string.IsNullOrEmpty( State ) )
					{
						return City + ", " + State;
					}
					else
					{
						return City;
					}
				}
				else
				{
					return State;
				}
			}
		}
	}
}
