using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Users
{
	public class UserSubscriptionSummary
	{
		public class SubscriptionInfo
		{
			public int SubscriptionID { get; set; }
			public string Name { get; set; }
			public bool Checked { get; set; }
		}

		public int UserID { get; set; }
		public List<SubscriptionInfo> subInfo { get; set; }

		public UserSubscriptionSummary()
		{
			subInfo = new List<SubscriptionInfo>();
		}
	}
}
