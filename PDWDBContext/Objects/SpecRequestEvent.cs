using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWDBContext
{
	public partial class SpecRequestEvent
	{
		public static SpecRequestEvent CreatedEvent( int userId )
		{
			return new SpecRequestEvent() { EventDate = DateTime.UtcNow, UserID = userId, EventType = SpecRequest.SpecRequestEventType.Created };
		}

		public static SpecRequestEvent CanceledEvent( int userId )
		{
			return new SpecRequestEvent() { EventDate = DateTime.UtcNow, UserID = userId, EventType = SpecRequest.SpecRequestEventType.Canceled };
		}

		public static SpecRequestEvent CompletedEvent( int userId )
		{
			return new SpecRequestEvent() { EventDate = DateTime.UtcNow, UserID = userId, EventType = SpecRequest.SpecRequestEventType.Completed };
		}

		public static SpecRequestEvent ReOpenedEvent( int userId )
		{
			return new SpecRequestEvent() { EventDate = DateTime.UtcNow, UserID = userId, EventType = SpecRequest.SpecRequestEventType.ReOpened };
		}
	}
}
