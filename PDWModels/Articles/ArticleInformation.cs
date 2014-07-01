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
	}
}
