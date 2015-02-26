using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace PDWInfrastructure.Attributes
{
	public class PostalCodeAttribute : RegularExpressionAttribute
	{
		public PostalCodeAttribute() :
			base( @"^\d{5}(-\d{4})?$" )
		{ }

		public override string FormatErrorMessage( string name )
		{
			return String.Format( "{0} must be in the format ##### or #####-####.", name );
		}
	}
}
