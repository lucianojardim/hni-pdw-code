using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWDBContext
{
	public partial class UserNotification
	{
		public bool PermissionByName( string name )
		{
			var checkProperty = this.GetType().GetProperty( name );
			if( checkProperty == null )
				return false;

			if( checkProperty.PropertyType == typeof( bool ) )
			{
				return (bool)( checkProperty.GetValue( this, new object[0] ) );
			}

			return false;
		}
	}
}
