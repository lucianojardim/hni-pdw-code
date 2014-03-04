using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PDWInfrastructure.Attributes
{
	public class PaoliPasswordAttribute : RegularExpressionAttribute
	{
		public PaoliPasswordAttribute() :
			base( "^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z]).*|$" )
		{ }
	}
}
