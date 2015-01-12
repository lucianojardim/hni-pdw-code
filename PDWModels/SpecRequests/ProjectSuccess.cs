using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.SpecRequests
{
	public class ProjectSuccess
	{
		public const int Pending = 1;
		public const int Won = 2;
		public const int Lost = 3;

		public static string GetDisplayString( int success )
		{
			switch( success )
			{
				case Pending:
					return "Pending";
				case Won:
					return "Won";
				case Lost:
					return "Lost";
			}

			return "";
		}

		public static IEnumerable<IDToTextItem> List
		{
			get
			{
				return new List<IDToTextItem>()
				{
					new IDToTextItem() { ID = Pending, Text = GetDisplayString( Pending ) },
					new IDToTextItem() { ID = Won, Text = GetDisplayString( Won ) },
					new IDToTextItem() { ID = Lost, Text = GetDisplayString( Lost ) }
				};
			}
		}
	}
}
