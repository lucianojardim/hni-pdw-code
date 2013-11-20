using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace PDWInfrastructure
{
	public class PhoneNumberAttribute : RegularExpressionAttribute
	{
		public PhoneNumberAttribute() :
			base( @"^[2-9]\d{2}-\d{3}-\d{4}$" )
		{ }

		public override string FormatErrorMessage( string name )
		{
			return String.Format( "{0} must be in the format ###-####.", name );
		}
	}
}
