using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace PDWInfrastructure.Attributes
{
	public class EmailAttribute : RegularExpressionAttribute
	{
		public EmailAttribute() :
			base( "^[A-Za-z0-9](([_\\.\\-\\+]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\\.\\-]?[a-zA-Z0-9]+)*)\\.([A-Za-z]{2,})$" )
		{ }
	}
}
