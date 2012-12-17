using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;

namespace PWDRepositories
{
	public class AttributeRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		public AttributeRepository()
		{
		}

		public IEnumerable<string> GetOptionList( string attr )
		{
			var attrData = database.Attributes.FirstOrDefault( a => a.Name == attr );

			if( attrData != null )
			{
				return attrData.AttributeOptions.Select( ao => ao.Name );
			}

			return new List<string>();
		}

		public IEnumerable<string> GetTypicalOptionList( string attr )
		{
			var attrData = database.TAttributes.FirstOrDefault( a => a.Name == attr );

			if( attrData != null )
			{
				return attrData.TAttributeOptions.Select( ao => ao.Name );
			}

			return new List<string>();
		}
	}
}
