using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWDBContext
{
	public partial class SpecRequest
	{
		public static class SpecRequestEventType
		{
			public const int Created = 1;
			public const int Canceled = 2;
			public const int Completed = 3;
			public const int ReOpened = 4;
		}

		public DateTime? CreatedOnDate
		{
			get
			{
				var created = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.Created );
				if( created != null )
				{
					return created.EventDate;
				}

				return null;
			}
		}

		public DateTime? CanceledOnDate
		{
			get
			{
				if( IsCanceled )
				{
					var canceled = SpecRequestEvents
						.OrderByDescending( e => e.EventDate )
						.FirstOrDefault( e => e.EventType == SpecRequestEventType.Canceled );
					if( canceled != null )
					{
						return canceled.EventDate;
					}
				}

				return null;
			}
		}

		public DateTime? CompletedOnDate
		{
			get
			{
				if( IsCompleted )
				{
					var completed = SpecRequestEvents
						.OrderByDescending( e => e.EventDate )
						.FirstOrDefault( e => e.EventType == SpecRequestEventType.Completed );
					if( completed != null )
					{
						return completed.EventDate;
					}
				}

				return null;
			}
		}

		public string CreatedByUserName
		{
			get
			{
				var created = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.Created );
				if( created != null )
				{
					return created.User.FullName;
				}

				return null;
			}
		}

		public string CreatedByUserPhone
		{
			get
			{
				var created = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.Created );
				if( created != null )
				{
					return created.User.BusinessPhone;
				}

				return null;
			}
		}

		public string CreatedByUserEmail
		{
			get
			{
				var created = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.Created );
				if( created != null )
				{
					return created.User.Email;
				}

				return null;
			}
		}

		public string CreatedByUserFirstName
		{
			get
			{
				var created = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.Created );
				if( created != null )
				{
					return created.User.FirstName;
				}

				return null;
			}
		}

		public int? CreatedByUserID
		{
			get
			{
				var created = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.Created );
				if( created != null )
				{
					return created.UserID;
				}

				return null;
			}
		}

		public string CreatedByCompany
		{
			get
			{
				var created = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.Created );
				if( created != null )
				{
					return created.User.Company.FullName;
				}

				return null;
			}
		}

		public Company CreatedByCompanyDetails
		{
			get
			{
				var created = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.Created );
				if( created != null )
				{
					return created.User.Company;
				}

				return null;
			}
		}

		public string CanceledByUserName
		{
			get
			{
				var canceled = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.Canceled );
				if( canceled != null )
				{
					return canceled.User.FullName;
				}

				return null;
			}
		}

		public string CompletedByUserName
		{
			get
			{
				var completed = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.Completed );
				if( completed != null )
				{
					return completed.User.FullName;
				}

				return null;
			}
		}

		public int ReOpenedCount
		{
			get
			{
				return SpecRequestEvents
					.Count( e => e.EventType == SpecRequestEventType.ReOpened );
			}
		}

		public DateTime? ReOpenedOnDate
		{
			get
			{
				var reopened = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.ReOpened );
				if( reopened != null )
				{
					return reopened.EventDate;
				}

				return null;
			}
		}

		public string ReOpenedByUserName
		{
			get
			{
				var reopened = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.ReOpened );
				if( reopened != null )
				{
					return reopened.User.FullName;
				}

				return null;
			}
		}

		public string ReOpenedByCompany
		{
			get
			{
				var reopened = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.ReOpened );
				if( reopened != null )
				{
					return reopened.User.Company.Name;
				}

				return null;
			}
		}

		public string ReOpenedByUserPhone
		{
			get
			{
				var reopened = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.ReOpened );
				if( reopened != null )
				{
					return reopened.User.BusinessPhone;
				}

				return null;
			}
		}

		public string ReOpenedByUserEmail
		{
			get
			{
				var reopened = SpecRequestEvents
					.OrderByDescending( e => e.EventDate )
					.FirstOrDefault( e => e.EventType == SpecRequestEventType.ReOpened );
				if( reopened != null )
				{
					return reopened.User.Email;
				}

				return null;
			}
		}
	}
}
