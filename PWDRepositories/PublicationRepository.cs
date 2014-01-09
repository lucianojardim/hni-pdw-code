using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWInfrastructure;
using PDWModels.Publications;

namespace PWDRepositories
{
	public class PublicationRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		public PublicationRepository()
		{
		}

		private PublicationSummary ToPublicationSummary( Publication pub )
		{
			return new PublicationSummary()
			{
				PublicationID = pub.PublicationID,
				Name = pub.Name,
				PubDate = pub.PubDate.ToShortDateString(),
				ImageCt = pub.PublicationImages.Count
			};
		}

		private PublicationInformation ToPublicationInformation( Publication pub )
		{
			return new PublicationInformation()
			{
				PublicationID = pub.PublicationID,
				Name = pub.Name,
				PublicationDate = pub.PubDate,
				FilterVisible = pub.FilterVisible
			};
		}

		public IEnumerable<PublicationSummary> GetFullPublicationList( DataTableParams param,
			out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var pubList = database.Publications.AsQueryable();

			totalRecords = pubList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
				pubList = pubList.Where( i => i.Name.Contains( param.sSearch ) );
			}
			displayedRecords = pubList.Count();

			string sortCol = param.sColumns.Split( ',' )[param.iSortCol_0];

			IQueryable<Publication> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
				case "pubdate":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = pubList.OrderBy( v => v.PubDate );
					}
					else
					{
						filteredAndSorted = pubList.OrderByDescending( v => v.PubDate );
					}
					break;
				case "imagect":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = pubList.OrderBy( v => v.PublicationImages.Count );
					}
					else
					{
						filteredAndSorted = pubList.OrderByDescending( v => v.PublicationImages.Count );
					}
					break;
				case "name":
				default:
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = pubList.OrderBy( v => v.Name );
					}
					else
					{
						filteredAndSorted = pubList.OrderByDescending( v => v.Name );
					}
					break;
			}

			if( (displayedRecords > param.iDisplayLength) && (param.iDisplayLength > 0) )
			{
				filteredAndSorted = filteredAndSorted.Skip( param.iDisplayStart ).Take( param.iDisplayLength );
			}

			return filteredAndSorted.ToList().Select( v => ToPublicationSummary( v ) );
		}

		public void AddPublication( PublicationInformation pubInfo )
		{
			if( database.Publications.Any( p => p.Name == pubInfo.Name ) )
			{
				throw new Exception( "Print Material with this name already exists." );
			}

			Publication pubData = new Publication();
			pubData.Name = pubInfo.Name;
			pubData.PubDate = pubInfo.PublicationDate;
			pubData.FilterVisible = pubInfo.FilterVisible;

			database.Publications.AddObject( pubData );

			database.SaveChanges();
		}

		public PublicationInformation GetPublicationInformation( int id )
		{
			var pubInfo = database.Publications.FirstOrDefault( p => p.PublicationID == id );

			if( pubInfo != null )
			{
				return ToPublicationInformation( pubInfo );
			}

			throw new Exception( "Print Material Does Not Exist" );
		}

		public void UpdatePublication( PublicationInformation pubInfo )
		{
			var pubData = database.Publications.FirstOrDefault( p => p.PublicationID == pubInfo.PublicationID );

			if( pubData != null )
			{
				if( database.Publications.Any( p => (p.Name == pubInfo.Name) && (p.PublicationID != pubInfo.PublicationID) ) )
				{
					throw new Exception( "Print Material with this name already exists." );
				}

				pubData.Name = pubInfo.Name;
				pubData.PubDate = pubInfo.PublicationDate;
				pubData.FilterVisible = pubInfo.FilterVisible;

				database.SaveChanges();

				return;
			}

			throw new Exception( "Print Material Does Not Exist" );
		}

		public void AddPubImage( int pubId, int imageId, int? pageNumber )
		{
			var pub = database.Publications.FirstOrDefault( p => p.PublicationID == pubId );
			var img = database.ImageFiles.FirstOrDefault( i => i.ImageID == imageId );

			if( pub == null )
				throw new Exception( "Print Material does not exist" );
			if( img == null )
				throw new Exception( "Image does not exist" );
			if( pub.PublicationImages.Any( pi => pi.ImageID == imageId ) )
				throw new Exception( "Image is already associated with Print Material" );

			PublicationImage pubImg = new PublicationImage();
			pubImg.ImageFile = img;
			pubImg.PageNumber = pageNumber;

			pub.PublicationImages.Add( pubImg );

			database.SaveChanges();
		}

		public void RemovePubImage( int pubId, int imageId )
		{
			var pub = database.Publications.FirstOrDefault( p => p.PublicationID == pubId );
			var img = database.ImageFiles.FirstOrDefault( i => i.ImageID == imageId );

			if( pub == null )
				throw new Exception( "Publication does not exist" );
			if( img == null )
				throw new Exception( "Image does not exist" );
			var pubImg = pub.PublicationImages.FirstOrDefault( pi => pi.ImageID == imageId );

			if( pubImg == null )
				throw new Exception( "Image is not associated with Print Material" );

			pub.PublicationImages.Remove( pubImg );

			database.SaveChanges();
		}

		public IEnumerable<PublicationComboItem> GetInUseList()
		{
			return database.Publications
				.Where( p => p.FilterVisible )
				.Where( p => p.PublicationImages.Any() )
				.OrderByDescending( p => p.PubDate )
				.Select( p => new PublicationComboItem() { PublicationID = p.PublicationID, Name = p.Name } );
		}
	}
}
