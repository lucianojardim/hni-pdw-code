using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels
{
	public static class SearchText
	{
		public static IEnumerable<string> GetSearchList( string searchText )
		{
			List<string> searchList = new List<string>();
			var wordList = searchText.Split( ' ' );

			foreach( var w in wordList )
			{
				if( w.Substring( w.Length - 1 ).ToLower() == "s" )
				{
					searchList.Add( w.Substring( 0, w.Length - 1 ) );
				}
				else
				{
					searchList.Add( w );
				}
			}

			return searchList;
		}
	}
}
