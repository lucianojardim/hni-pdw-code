using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PDWInfrastructure.Attributes
{
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = true )]	// [Match("Email", "EmailConfirmation", ErrorMessage = "Emails must match")]
	public class MatchAttribute : ValidationAttribute
	{
		public string SourceProperty { get; set; }
		public string MatchProperty { get; set; }
		public bool ShouldMatch { get; set; }

		public MatchAttribute( string source, string match, bool bShouldMatch = true )
		{
			SourceProperty = source;
			MatchProperty = match;
			ShouldMatch = bShouldMatch;
		}

		public override Boolean IsValid( Object value )
		{
			Type objectType = value.GetType();

			PropertyInfo[] properties = objectType.GetProperties();

			object sourceValue = new object();
			object matchValue = new object();

			Type sourceType = null;
			Type matchType = null;

			int counter = 0;

			foreach( PropertyInfo propertyInfo in properties )
			{
				if( propertyInfo.Name == SourceProperty || propertyInfo.Name == MatchProperty )
				{
					if( counter == 0 )
					{
						sourceValue = propertyInfo.GetValue( value, null );
						if( sourceValue != null )
							sourceType = sourceValue.GetType();
					}
					if( counter == 1 )
					{
						matchValue = propertyInfo.GetValue( value, null );
						if( matchValue != null )
							matchType = matchValue.GetType();
					}
					counter++;
					if( counter == 2 )
					{
						break;
					}
				}
			}

			if( sourceValue == null && matchValue == null )
			{
				return true;
			}
			if( sourceType != null && matchType != null )
			{
				return ( sourceValue.ToString() == matchValue.ToString() ) == ShouldMatch;
			}
			return !ShouldMatch;
		}
	}
}
