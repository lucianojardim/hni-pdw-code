using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Xml.Linq;

namespace PDWInfrastructure.UmbracoAPI
{
	public static class UmbracoInterface
	{
		public class NewsItem
		{
			public string Headline { get; set; }
			public string Author { get; set; }
			public DateTime PubDate { get; set; }
			public string Link { get; set; }
			public string Image { get; set; }
		}

		public static List<NewsItem> GetNewsItems( string baseUrl )
		{
			List<NewsItem> theList = new List<NewsItem>();

			string url = baseUrl + "newsrss";

			XmlReader reader = XmlReader.Create( url );
			SyndicationFeed feed = SyndicationFeed.Load( reader );
			reader.Close();
			foreach( SyndicationItem item in feed.Items )
			{
				var news = new NewsItem();
				news.Headline = item.Title.Text;
				news.PubDate = item.PublishDate.Date;
				news.Link = item.Links[0].Uri.ToString();

				var extPicture = item.ElementExtensions;
				var pictureItem = extPicture.Where( e => e.OuterName == "pictureLink" ).FirstOrDefault();
				XElement xPicture = pictureItem.GetObject<XElement>();
				news.Image = baseUrl + xPicture.Value;

				var extAuthor = item.ElementExtensions;
				var authorItem = extAuthor.Where( e => e.OuterName == "authorName" ).FirstOrDefault();
				XElement xAuthor = authorItem.GetObject<XElement>();
				news.Author = xAuthor.Value;

				theList.Add( news );

				if( theList.Count >= 3 )
				{
					break;
				}
			}

			return theList;
		}

		public class ScoopItem
		{
			public string Headline { get; set; }
			public string Author { get; set; }
			public DateTime PubDate { get; set; }
			public string Link { get; set; }
			public string Image { get; set; }
		}

		public static List<ScoopItem> GetScoopItems( string baseUrl )
		{
			List<ScoopItem> theList = new List<ScoopItem>();

			string url = baseUrl + "scooprss";

			XmlReader reader = XmlReader.Create( url );
			SyndicationFeed feed = SyndicationFeed.Load( reader );
			reader.Close();
			foreach( SyndicationItem item in feed.Items )
			{
				var news = new ScoopItem();
				news.Headline = item.Title.Text;
				news.PubDate = item.PublishDate.Date;
				news.Link = item.Links[0].Uri.ToString();

				var extPicture = item.ElementExtensions;
				var pictureItem = extPicture.Where( e => e.OuterName == "pictureLink" ).FirstOrDefault();
				XElement xPicture = pictureItem.GetObject<XElement>();
				news.Image = baseUrl + xPicture.Value;

				var extAuthor = item.ElementExtensions;
				var authorItem = extAuthor.Where( e => e.OuterName == "authorName" ).FirstOrDefault();
				XElement xAuthor = authorItem.GetObject<XElement>();
				news.Author = xAuthor.Value;

				theList.Add( news );

				if( theList.Count >= 3 )
				{
					break;
				}
			}

			return theList;
		}

	}
}
