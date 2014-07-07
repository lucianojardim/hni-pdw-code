﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWModels.Articles;
using PDWInfrastructure;
using System.Web;

namespace PWDRepositories
{
	public class ArticleRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		private const int ArticleCount = 9;

		public ArticleRepository()
		{
		}

		private ArticleInformation ToArticleInformation( ScoopArticle article )
		{
			return new ArticleInformation()
			{
				ArticleID = article.ArticleID,
				BigHeadline = article.BigHeadline,
				BigText = article.BigText,
				BigImageURL = article.BigImageURL,
				SmallHeadline = article.SmallHeadline,
				SmallText = article.SmallText,
				SmallImageURL = article.SmallImageURL,
				ContentBlock = HttpUtility.HtmlDecode( article.ContentBlock ),
				Title = article.Title,
				PubDate = article.PubDate,
				AuthorID = article.AuthorID,
				ShowBigImage = article.ShowBigImage,
				ArticleType = article.ArticleType
			};
		}

		private ArticleSummary ToArticleSummary( ScoopArticle article, bool bWantHeadline = false )
		{
			return new ArticleSummary()
			{
				ArticleID = article.ArticleID,
				Title = bWantHeadline ? article.BigHeadline : article.Title,
				PublishDate = article.PubDate.HasValue ? article.PubDate.Value.ToString( "MMMM dd, yyyy" ) : "",
				Rank = article.Rank,
				Author = article.User.FullName,
				ImgURL = article.BigImageURL,
				Description = article.BigText
			};
		}

		private ArticleDisplayInfo ToArticleDisplayInfo( ScoopArticle article )
		{
			return new ArticleDisplayInfo()
			{
				ArticleID = article.ArticleID,
				Headline = article.Rank == 1 ? article.BigHeadline : article.SmallHeadline,
				Subheadline = article.Rank == 1 ? article.BigText : article.SmallText,
				ImageURL = article.Rank == 1 ? article.BigImageURL: article.SmallImageURL
			};
		}

		public IEnumerable<ArticleDisplayInfo> GetMainArticleList( int articleType )
		{
			return database.ScoopArticles
				.Where( a => a.PubDate < DateTime.Now && a.ArticleType == articleType )
				.OrderBy( a => a.Rank )
				.Take( ArticleCount )
				.ToList()
				.Select( s => ToArticleDisplayInfo( s ) );
		}

		public IEnumerable<ArticleSummary> GetFullArticleList( ArticleTableParams param, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var articleList = database.ScoopArticles.AsQueryable();

			totalRecords = articleList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
				articleList = articleList.Where( i =>
					i.Title.Contains( param.sSearch ) ||
					i.User.FirstName.Contains( param.sSearch ) || i.User.LastName.Contains( param.sSearch ) );
			}

			displayedRecords = articleList.Count();

			string sortCol = param.sColumns.Split( ',' )[param.iSortCol_0];

			articleList = articleList.OrderBy( a => a.Rank );

			if( ( displayedRecords > param.iDisplayLength ) && ( param.iDisplayLength > 0 ) )
			{
				articleList = articleList.Skip( param.iDisplayStart ).Take( param.iDisplayLength );
			}

			return articleList.ToList().Select( v => ToArticleSummary( v ) );
		}

		public IEnumerable<ArticleSummary> GetArticleViewList( int articleType )
		{
			var articleList = database.ScoopArticles.AsQueryable();

			articleList = articleList
				.Where( a => a.PubDate < DateTime.Now && a.ArticleType == articleType )
				.OrderByDescending( a => a.PubDate );

			return articleList.ToList().Select( v => ToArticleSummary( v, true ) );
		}

		public bool AddArticle( ArticleInformation aInfo )
		{
			ScoopArticle newArticle = new ScoopArticle();

			newArticle.BigHeadline = aInfo.BigHeadline;
			newArticle.BigText = aInfo.BigText;
			newArticle.BigImageURL = aInfo.BigImageURL;
			newArticle.SmallHeadline = aInfo.SmallHeadline;
			newArticle.SmallText = aInfo.SmallText;
			newArticle.SmallImageURL = aInfo.SmallImageURL;
			newArticle.ContentBlock = aInfo.ContentBlock;
			newArticle.Title = aInfo.Title;
			newArticle.PubDate = aInfo.PubDate;
			newArticle.AuthorID = aInfo.AuthorID;
			newArticle.Rank = database.ScoopArticles.Count() + 1;
			newArticle.ShowBigImage = aInfo.ShowBigImage;
			newArticle.ArticleType = aInfo.ArticleType;

			database.ScoopArticles.AddObject( newArticle );

			return database.SaveChanges() > 0;
		}

		public ArticleInformation GetArticle( int ArticleID )
		{
			var dbArticle = database.ScoopArticles.FirstOrDefault( a => a.ArticleID == ArticleID );

			if( dbArticle == null )
			{
				throw new Exception( "Unable to find article." );
			}

			return ToArticleInformation( dbArticle );
		}

		public bool EditArticle( ArticleInformation aInfo )
		{
			var dbArticle = database.ScoopArticles.FirstOrDefault( a => a.ArticleID == aInfo.ArticleID );

			if( dbArticle == null )
			{
				throw new Exception( "Unable to find article." );
			}

			dbArticle.BigHeadline = aInfo.BigHeadline;
			dbArticle.BigText = aInfo.BigText;
			dbArticle.BigImageURL = aInfo.BigImageURL;
			dbArticle.SmallHeadline = aInfo.SmallHeadline;
			dbArticle.SmallText = aInfo.SmallText;
			dbArticle.SmallImageURL = aInfo.SmallImageURL;
			dbArticle.ContentBlock = aInfo.ContentBlock;
			dbArticle.Title = aInfo.Title;
			dbArticle.PubDate = aInfo.PubDate;
			dbArticle.AuthorID = aInfo.AuthorID;
			dbArticle.ShowBigImage = aInfo.ShowBigImage;
			dbArticle.ArticleType = aInfo.ArticleType;

			return database.SaveChanges() > 0;
		}

		public bool DeleteArticle( int id )
		{
			var dbArticle = database.ScoopArticles.FirstOrDefault( a => a.ArticleID == id );

			if( dbArticle == null )
			{
				throw new Exception( "Unable to find article." );
			}

			foreach( var laterArticle in database.ScoopArticles.Where( a => a.Rank > dbArticle.Rank ) )
			{
				laterArticle.Rank--;
			}

			database.DeleteObject( dbArticle );

			return database.SaveChanges() > 0;
		}

		public bool MoveArticle( int id, int direction )
		{
			var dbArticle = database.ScoopArticles.FirstOrDefault( a => a.ArticleID == id );

			if( dbArticle == null )
			{
				throw new Exception( "Unable to find article." );
			}

			switch( direction )
			{
				case -2:	// to front of line
					foreach( var laterArticle in database.ScoopArticles.Where( a => a.Rank < dbArticle.Rank ) )
					{
						laterArticle.Rank++;
					}
					dbArticle.Rank = 1;
					break;
				case -1:	// up one
					{
						var other = database.ScoopArticles.FirstOrDefault( a => a.Rank == dbArticle.Rank - 1 );
						if( other != null )
						{
							other.Rank++;
						}
						dbArticle.Rank--;
					}
					break;
				case 1:		// down one
					{
						var other = database.ScoopArticles.FirstOrDefault( a => a.Rank == dbArticle.Rank + 1 );
						if( other != null )
						{
							other.Rank--;
						}
						dbArticle.Rank++;
					}
					break;
				case 2:		// end of line
					foreach( var laterArticle in database.ScoopArticles.Where( a => a.Rank > dbArticle.Rank ) )
					{
						laterArticle.Rank--;
					}
					dbArticle.Rank = database.ScoopArticles.Count();
					break;
			}

			return database.SaveChanges() > 0;
		}

		public ArticleViewDetails GetArticlePreview( ArticleInformation aInfo )
		{
			var dbUser = database.Users.FirstOrDefault( u => u.UserID == aInfo.AuthorID );

			return new ArticleViewDetails()
			{
				Headline = aInfo.BigHeadline,
				Content = aInfo.ContentBlock,
				ImageURL = aInfo.ShowBigImage ? aInfo.BigImageURL : null,
				PublishDate = aInfo.PubDate.HasValue ? aInfo.PubDate.Value.ToString( "MMMM dd, yyyy" ) : "",
				AuthorName = dbUser != null ? dbUser.FullName : "",
				AuthorImage = dbUser != null ? dbUser.ImageFileName : null,
				AuthorCredit = dbUser != null ? dbUser.AuthorCredit : null,
				ArticleType = aInfo.ArticleType,
				RecentArticles = database.ScoopArticles
					.Where( a => a.PubDate < DateTime.Now && a.ArticleType == aInfo.ArticleType )
					.OrderByDescending( a => a.PubDate )
					.Take( ArticleCount )
					.ToList()
					.Select( s => new ArticleViewDetails.RecentArticleDetails()
					{
						ArticleID = s.ArticleID,
						Headline = s.BigHeadline
					} )
			};
		}

		public ArticleViewDetails GetArticleViewer( int id )
		{
			var dbArticle = database.ScoopArticles.FirstOrDefault( a => a.ArticleID == id );

			if( dbArticle == null )
			{
				throw new Exception( "Unable to find article." );
			}

			return new ArticleViewDetails()
			{
				Headline = dbArticle.BigHeadline,
				Content = dbArticle.ContentBlock,
				ImageURL = dbArticle.ShowBigImage ? dbArticle.BigImageURL : null,
				PublishDate = dbArticle.PubDate.HasValue ? dbArticle.PubDate.Value.ToString( "MMMM dd, yyyy" ) : "",
				AuthorName = dbArticle.User.FullName,
				AuthorImage = dbArticle.User.ImageFileName,
				AuthorCredit = dbArticle.User.AuthorCredit,
				ArticleType = dbArticle.ArticleType,
				RecentArticles = database.ScoopArticles
					.Where( a => a.PubDate < DateTime.Now && a.ArticleType == dbArticle.ArticleType )
					.OrderByDescending( a => a.PubDate )
					.Take( ArticleCount )
					.ToList()
					.Select( s => new ArticleViewDetails.RecentArticleDetails()
					{
						ArticleID = s.ArticleID,
						Headline = s.BigHeadline
					} )
			};
		}
	}
}
