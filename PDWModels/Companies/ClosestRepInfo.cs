using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWModels.Images;

namespace PDWModels.Companies
{
	public class ClosestRepInfo
	{
		public class RepInfo
		{
			public string Name { get; set; }
			public string Phone { get; set; }
			public string Email { get; set; }
			public string ImageFile { get; set; }
			public string City { get; set; }
			public string State { get; set; }
		}

		public string Name { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string Phone { get; set; }
		public string FAX { get; set; }
		public string ContactEmail { get; set; }
		public string WebSite { get; set; }

		public string ShowroomDisplayName { get; set; }
		public string ShowroomAddress1 { get; set; }
		public string ShowroomAddress2 { get; set; }
		public string ShowroomCity { get; set; }
		public string ShowroomState { get; set; }
		public string ShowroomZip { get; set; }
		public string ShowroomPhone { get; set; }
		public string ShowroomFAX { get; set; }
		public string ShowroomWebsite { get; set; }
		public string ShowroomDescription { get; set; }
		public string ShowroomHours { get; set; }

		public IEnumerable<RepInfo> Contacts { get; set; }
		public IEnumerable<ImageSummary> ShowroomImages { get; set; }
	}
}
