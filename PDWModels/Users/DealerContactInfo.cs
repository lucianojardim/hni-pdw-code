using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Users
{
	public class DealerContactInfo
	{
		public int UserID { get; set; }
		public int UserCompanyID { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Title { get; set; }

		public string BusinessAddress1 { get; set; }
		public string BusinessAddress2 { get; set; }
		public string BusinessCity { get; set; }
		public string BusinessState { get; set; }
		public string BusinessZip { get; set; }
		public string BusinessPhone { get; set; }
		public string CellPhone { get; set; }
		public string Extension { get; set; }
		public string FaxNumber { get; set; }

		public string HomeAddress1 { get; set; }
		public string HomeAddress2 { get; set; }
		public string HomeCity { get; set; }
		public string HomeState { get; set; }
		public string HomeZip { get; set; }
		public string HomePhone { get; set; }
		public string PersonalCellPhone { get; set; }

	}
}
