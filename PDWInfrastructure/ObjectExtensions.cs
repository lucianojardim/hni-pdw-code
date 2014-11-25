using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure
{
	public static class ObjectExtensions
	{
		public static string DefaultString( this string s, string defaultValue )
		{
			if( ( s ?? "" ).Any() )
				return s;

			return defaultValue;
		}
	}
}
