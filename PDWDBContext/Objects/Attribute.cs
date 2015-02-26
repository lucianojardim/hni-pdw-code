using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWDBContext
{
	public partial class Attribute
	{
		public Attribute( bool bDetail ) : this()
		{
			DetailItem = bDetail;
		}
	}
}
