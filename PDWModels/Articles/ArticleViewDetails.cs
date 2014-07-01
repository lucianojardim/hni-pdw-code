using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Articles
{
	public class ArticleViewDetails
	{
		public class RecentArticleDetails
		{
			public int ArticleID { get; set; }
			public string Headline { get; set; }
		}

		public string Headline { get; set; }
		public string Content { get; set; }
		public string ImageURL { get; set; }
		public string PublishDate { get; set; }
		public string AuthorName { get; set; }
		public string AuthorImage { get; set; }
		public string AuthorCredit { get; set; }
		public IEnumerable<RecentArticleDetails> RecentArticles { get; set; }
	}
}
