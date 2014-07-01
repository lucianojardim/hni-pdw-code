using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.Articles
{
	public class ArticleSummary
	{
		public int ArticleID { get; set; }
		public string Title { get; set; }
		public string PublishDate { get; set; }
		public string Author { get; set; }
		public string ImgURL { get; set; }
		public string Description { get; set; }
		public int Rank { get; set; }
		public string EditButtons { get; set; }
		public string RankButtons { get; set; }
	}
}
