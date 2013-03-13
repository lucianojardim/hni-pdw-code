using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PDWModels
{
	public static class SearchText
	{
		public static List<string> StopList { get { return new List<string>() { "the", "is", "at", "which", "and", "or", "of", "for", "that", "on", "under", "behind", "beside", "in", "as", "what" }; } }

		private static bool InStopList( string s )
		{
			return StopList.Contains( s.ToLower() );
		}

		public static IEnumerable<string> GetSearchList( string searchText )
		{
			List<string> searchList = new List<string>();
			searchText = Regex.Replace( searchText, @"[^A-Za-z0-9\- ]", "" );

			var wordList = searchText.ToLower().Split( ' ' ).ToList();

			wordList.RemoveAll( InStopList );

			foreach( var w in wordList )
			{
				if( w.Substring( w.Length - 1 ).ToLower() == "s" )
				{
					searchList.Add( w );
					searchList.Add( w.Substring( 0, w.Length - 1 ) );
				}
				else
				{
					searchList.Add( w );
				}
			}

			searchList = searchList.Select( s => " " + s + " " ).ToList();

			return searchList;
		}

		public static string GetKeywordList( List<string> arrKeywordList )
		{
			List<string> fullKeywordList = new List<string>();
			
			arrKeywordList.ForEach( s =>
				fullKeywordList.AddRange( Regex.Replace( s, @"[^A-Za-z0-9\- ]", "" ).ToLower().Split( ' ' ).ToList() ) );
			fullKeywordList = fullKeywordList.Where( s => s.Any() ).Distinct().ToList();
			fullKeywordList.RemoveAll( InStopList );

			return " " + string.Join( " ", fullKeywordList ) + " ";
		}
	}
}
