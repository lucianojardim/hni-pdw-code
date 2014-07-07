using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PDWModels.Articles
{
	public class ArticleInformation
	{
		public class ArticleTypes
		{
			public const int Scoop = 1;
			public const int NewsAndUpdates = 2;
			public const int Internal = 3;

			public static Dictionary<int, string> ArticleTypeList
			{
				get
				{
					return new Dictionary<int, string>() {
					{ Scoop, "Scoop" },				
					{ NewsAndUpdates, "News and Updates" },				
					{ Internal, "Internal Only" },				
                };
				}
			}
		}

		public int ArticleID { get; set; }
		public string BigHeadline { get; set; }
		public string BigText { get; set; }
		public string BigImageURL { get; set; }
		public string SmallHeadline { get; set; }
		public string SmallText { get; set; }
		public string SmallImageURL { get; set; }
		public string ContentBlock { get; set; }
		public string Title { get; set; }
		[DisplayFormat( ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}" )]
		public DateTime? PubDate { get; set; }
		[Required]
		[DisplayName( "Author Name" )]
		public int AuthorID { get; set; }
		public bool ShowBigImage { get; set; }
		public int ArticleType { get; set; }
	}
}
