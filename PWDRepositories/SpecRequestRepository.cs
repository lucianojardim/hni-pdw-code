using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWInfrastructure;
using PDWModels.SpecRequests;
using System.IO;
using System.Configuration;
using System.Data.Objects.DataClasses;
using PDWModels;

namespace PWDRepositories
{
	public class SpecRequestRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		public SpecRequestRepository()
		{
		}

		private SpecRequestSummary ToSpecRequestSummary( SpecRequest sRequest )
		{
			return new SpecRequestSummary()
			{
				RequestID = sRequest.RequestID,
				Name = sRequest.Name,
				Dealer = sRequest.PrimaryCompanyID.HasValue ? sRequest.PrimaryCompany.Name : "",
				ProjectName = sRequest.ProjectName,
				SpecTeamMember = sRequest.PaoliSpecTeamMemberID.HasValue ? sRequest.SpecTeamMember.FullName : "",
				IsRecommended = sRequest.IsGoodForWeb ?? false,
				IsPublished = sRequest.Typicals.Any()
			};
		}

		public IEnumerable<SpecRequestSummary> GetFullRequestList( SpecRequestTableParams paramDetails, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var requestList = database.SpecRequests.AsQueryable();

			totalRecords = requestList.Count();

			if( !string.IsNullOrEmpty( paramDetails.sSearch ) )
			{
				requestList = requestList.Where( i =>
					i.Name.Contains( paramDetails.sSearch ) ||
					i.PrimaryCompany.Name.Contains( paramDetails.sSearch ) ||
					i.ProjectName.Contains( paramDetails.sSearch ) ||
					i.SpecTeamMember.FirstName.Contains( paramDetails.sSearch ) ||
					i.SpecTeamMember.LastName.Contains( paramDetails.sSearch ) );
			}

			displayedRecords = requestList.Count();

			string sortCol = paramDetails.sColumns.Split( ',' )[paramDetails.iSortCol_0];

			IQueryable<SpecRequest> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
				case "name":
				default:
					if( paramDetails.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = requestList.OrderBy( v => v.Name );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.Name );
					}
					break;
				case "dealer":
					if( paramDetails.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = requestList.OrderBy( v => v.PrimaryCompany.Name );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.PrimaryCompany.Name );
					}
					break;
				case "projectname":
					if( paramDetails.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = requestList.OrderBy( v => v.ProjectName );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.ProjectName );
					}
					break;
				case "specteammember":
					if( paramDetails.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = requestList.OrderBy( v => v.SpecTeamMember.LastName );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.SpecTeamMember.LastName );
					}
					break;
			}

			if( ( displayedRecords > paramDetails.iDisplayLength ) && ( paramDetails.iDisplayLength > 0 ) )
			{
				filteredAndSorted = filteredAndSorted.Skip( paramDetails.iDisplayStart ).Take( paramDetails.iDisplayLength );
			}

			return filteredAndSorted.ToList().Select( v => ToSpecRequestSummary( v ) );
		}

		private SpecRequestInformation.FileListing GetFileListing( string name, IEnumerable<SpecRequestFile> fileList, string fileType )
		{
			return new SpecRequestInformation.FileListing()
			{
				SpecName = name,
				Extension = fileType,
				FileList =
					fileList
					.Where( f => f.Extension == fileType )
					.OrderByDescending( f => f.UploadDate )
					.Select( i =>
						new SpecRequestInformation.FileInformation()
						{
							FileName = i.Name,
							UploadDate = i.UploadDate.ToString( "MM/dd/yyyy HH:mm" ),
							VersionNumber = i.VersionNumber
						} )
					.ToList()
			};
		}

		public SpecRequestInformation GetSpecRequest( int requestId )
		{
			var sInfo = database.SpecRequests.FirstOrDefault( s => s.RequestID == requestId );
			if( sInfo == null )
			{
				throw new Exception( "Unable to find Spec Request" );
			}

			return new SpecRequestInformation()
			{
				RequestID = sInfo.RequestID,
				Name = sInfo.Name,
				ProjectName = sInfo.ProjectName,
				PaoliSalesRepGroupID = sInfo.PaoliSalesRepGroupID,
				CompanyID = sInfo.PrimaryCompanyID,
				DealerSalesRep = sInfo.DealerSalesRepID,
				IsGSA = sInfo.IsGSA ?? false,
				SavedLocation = sInfo.SavedLocation,
				ListPrice = sInfo.ListPrice,
				SeriesList = sInfo.SeriesList,
				Received = sInfo.Received ?? false,
				SPLQuote = sInfo.SPLQuote,
				PaoliSpecTeamMember = sInfo.PaoliSpecTeamMemberID ?? 0,
				IsGoodForWeb = sInfo.IsGoodForWeb ?? false,
				AvailableForIn2 = sInfo.AvailableForIn2 ?? false,
				Footprint = sInfo.Footprint,
				FeaturedSeries = sInfo.FeaturedSeries,
				Material = sInfo.Material,
				Finish = sInfo.Finish,
				Notes = sInfo.Notes,
				xlsFileList = GetFileListing( sInfo.Name, sInfo.SpecRequestFiles, "xls" ),
				sifFileList = GetFileListing( sInfo.Name, sInfo.SpecRequestFiles, "sif" ),
				sp4FileList = GetFileListing( sInfo.Name, sInfo.SpecRequestFiles, "sp4" ),
				pdfFileList = GetFileListing( sInfo.Name, sInfo.SpecRequestFiles, "pdf" ),
				dwgFileList = GetFileListing( sInfo.Name, sInfo.SpecRequestFiles, "dwg" ),
				imgFileList = GetFileListing( sInfo.Name, sInfo.SpecRequestFiles, "img" ),
				CreatedDate = sInfo.RequestDate.HasValue ? sInfo.RequestDate.Value.ToShortDateString() : ""
					
			};
		}

		public bool AddSpecRequest( SpecRequestInformation sInfo )
		{
			SpecRequest newSpec = new SpecRequest();

			newSpec.ProjectName = sInfo.ProjectName;
			newSpec.PaoliSalesRepGroupID = sInfo.PaoliSalesRepGroupID;
			newSpec.PrimaryCompanyID = sInfo.CompanyID;
			newSpec.DealerSalesRepID = sInfo.DealerSalesRep;
			newSpec.RequestDate = DateTime.UtcNow;
			newSpec.IsGSA = sInfo.IsGSA;
			newSpec.SavedLocation = sInfo.SavedLocation;
			newSpec.ListPrice = sInfo.ListPrice;
			newSpec.SeriesList = sInfo.SeriesList;
			newSpec.Received = sInfo.Received;
			newSpec.SPLQuote = sInfo.SPLQuote;
			newSpec.PaoliSpecTeamMemberID = (sInfo.PaoliSpecTeamMember ?? 0) > 0 ? sInfo.PaoliSpecTeamMember : null;
			newSpec.LastModifiedDate = DateTime.UtcNow;
			newSpec.IsGoodForWeb = sInfo.IsGoodForWeb;
			newSpec.AvailableForIn2 = sInfo.AvailableForIn2;
			newSpec.Footprint = sInfo.Footprint;
			newSpec.FeaturedSeries = sInfo.FeaturedSeries;
			newSpec.Material = sInfo.Material;
			newSpec.Finish = sInfo.Finish;
			newSpec.Notes = sInfo.Notes;

			database.SpecRequests.AddObject( newSpec );

			if( database.SaveChanges() > 0 )
			{
				database.Refresh( System.Data.Objects.RefreshMode.StoreWins, newSpec );

				newSpec.Name = string.Format( "T{0}", newSpec.RequestID.ToString( "D4" ) );

				var rootLocation = Path.Combine( ConfigurationManager.AppSettings["SpecRequestDocumentLocation"], newSpec.Name );

				bool bAddFile = false;
				if( sInfo.xlsFile != null )
				{
					bAddFile |= SaveNewFileVersion( newSpec, rootLocation, "xls", sInfo.xlsFile.InputStream, sInfo.xlsFile.FileName );
				}

				if( sInfo.sifFile != null )
				{
					bAddFile |= SaveNewFileVersion( newSpec, rootLocation, "sif", sInfo.sifFile.InputStream, sInfo.sifFile.FileName );
				}

				if( sInfo.sp4File != null )
				{
					bAddFile |= SaveNewFileVersion( newSpec, rootLocation, "sp4", sInfo.sp4File.InputStream, sInfo.sp4File.FileName );
				}

				if( sInfo.pdfFile != null )
				{
					bAddFile |= SaveNewFileVersion( newSpec, rootLocation, "pdf", sInfo.pdfFile.InputStream, sInfo.pdfFile.FileName );
				}

				if( sInfo.dwgFile != null )
				{
					bAddFile |= SaveNewFileVersion( newSpec, rootLocation, "dwg", sInfo.dwgFile.InputStream, sInfo.dwgFile.FileName );
				}

				if( sInfo.imgFile != null )
				{
					bAddFile |= SaveNewFileVersion( newSpec, rootLocation, "img", sInfo.imgFile.InputStream, sInfo.imgFile.FileName );
				}

				return database.SaveChanges() > 0;
			}

			throw new Exception( "Unable to save Spec Request" );
		}

		public bool UpdateSpecRequest( SpecRequestInformation sInfo )
		{
			var specInfo = database.SpecRequests.FirstOrDefault( s => s.RequestID == sInfo.RequestID );
			if( specInfo == null )
			{
				throw new Exception( "Unable to find Spec Request" );
			}

			specInfo.ProjectName = sInfo.ProjectName;
			specInfo.PaoliSalesRepGroupID = sInfo.PaoliSalesRepGroupID;
			specInfo.PrimaryCompanyID = sInfo.CompanyID;
			specInfo.DealerSalesRepID = sInfo.DealerSalesRep;
			specInfo.IsGSA = sInfo.IsGSA;
			specInfo.SavedLocation = sInfo.SavedLocation;
			specInfo.ListPrice = sInfo.ListPrice;
			specInfo.SeriesList = sInfo.SeriesList;
			specInfo.Received = sInfo.Received;
			specInfo.SPLQuote = sInfo.SPLQuote;
			specInfo.PaoliSpecTeamMemberID = ( sInfo.PaoliSpecTeamMember ?? 0 ) > 0 ? sInfo.PaoliSpecTeamMember : null;
			specInfo.LastModifiedDate = DateTime.UtcNow;
			specInfo.IsGoodForWeb = sInfo.IsGoodForWeb;
			specInfo.AvailableForIn2 = sInfo.AvailableForIn2;
			specInfo.Footprint = sInfo.Footprint;
			specInfo.FeaturedSeries = sInfo.FeaturedSeries;
			specInfo.Material = sInfo.Material;
			specInfo.Finish = sInfo.Finish;
			specInfo.Notes = sInfo.Notes;

			var rootLocation = Path.Combine( ConfigurationManager.AppSettings["SpecRequestDocumentLocation"], specInfo.Name );

			bool bAddFile = false;
			if( sInfo.xlsFile != null )
			{
				bAddFile |= SaveNewFileVersion( specInfo, rootLocation, "xls", sInfo.xlsFile.InputStream, sInfo.xlsFile.FileName );
			}

			if( sInfo.sifFile != null )
			{
				bAddFile |= SaveNewFileVersion( specInfo, rootLocation, "sif", sInfo.sifFile.InputStream, sInfo.sifFile.FileName );
			}

			if( sInfo.sp4File != null )
			{
				bAddFile |= SaveNewFileVersion( specInfo, rootLocation, "sp4", sInfo.sp4File.InputStream, sInfo.sp4File.FileName );
			}

			if( sInfo.pdfFile != null )
			{
				bAddFile |= SaveNewFileVersion( specInfo, rootLocation, "pdf", sInfo.pdfFile.InputStream, sInfo.pdfFile.FileName );
			}

			if( sInfo.dwgFile != null )
			{
				bAddFile |= SaveNewFileVersion( specInfo, rootLocation, "dwg", sInfo.dwgFile.InputStream, sInfo.dwgFile.FileName );
			}

			if( sInfo.imgFile != null )
			{
				bAddFile |= SaveNewFileVersion( specInfo, rootLocation, "img", sInfo.imgFile.InputStream, sInfo.imgFile.FileName );
			}

			return database.SaveChanges() > 0;
		}

		private bool SaveNewFileVersion( SpecRequest sRequest, string rootLocation, string fileType, Stream inputStream, string fileName )
		{
			var existingCount = sRequest.SpecRequestFiles.Count( f => f.Extension == fileType );
			existingCount++;

			var fileLocation = Path.Combine( rootLocation, fileType, existingCount.ToString() );

			if( !Directory.Exists( fileLocation ) )
			{
				Directory.CreateDirectory( fileLocation );
			}

			fileLocation = Path.Combine( fileLocation, fileName );

			using( var fileStream = File.Create( fileLocation ) )
			{
				inputStream.CopyTo( fileStream );
			}

			var newFile = new SpecRequestFile();

			newFile.Extension = fileType;
			newFile.Name = fileName;
			newFile.VersionNumber = existingCount;
			newFile.UploadDate = DateTime.UtcNow;

			sRequest.SpecRequestFiles.Add( newFile );

			return true;
		}

		public TypicalMgmtInfo GetNewTypical( int requestId )
		{
			var sInfo = database.SpecRequests.FirstOrDefault( s => s.RequestID == requestId );
			if( sInfo == null )
			{
				throw new Exception( "Unable to find Spec Request" );
			}

			return new TypicalMgmtInfo()
			{
				RequestID = sInfo.RequestID,
				Name = sInfo.Name,
				ListPrice = sInfo.ListPrice ?? 0,
				SeriesList = sInfo.SeriesList,
				AvailableForIn2 = sInfo.AvailableForIn2 ?? false,
				Footprint = sInfo.Footprint,
				FeaturedSeries = sInfo.FeaturedSeries,
				Material = sInfo.Material,
				Finish = sInfo.Finish,
				Notes = sInfo.Notes,
			};
		}

		public bool AddTypical( TypicalMgmtInfo tInfo )
		{
			var specRequest = database.SpecRequests.FirstOrDefault( r => r.RequestID == tInfo.RequestID );
			if( specRequest == null )
			{
				throw new Exception( "Unable to find Spec Request" );
			}

			Typical tData = new Typical();
			tData.CreatedDate = DateTime.Now;

			tData.Name = specRequest.Name;

			List<string> arrKeywordList = new List<string>();
			var rSeries = database.Serieses.FirstOrDefault( s => s.Name == tInfo.FeaturedSeries );
			if( rSeries != null )
			{
				SeriesTypical stData = new SeriesTypical();
				stData.IsPrimary = true;
				stData.Series = rSeries;
				stData.Typical = tData;
				database.SeriesTypicals.AddObject( stData );
			}
			else
			{
				throw new Exception( "Unable to find Featured Series" );
			}
			tData.FeaturedSeries = rSeries.Name;

			if( ( tInfo.SeriesList ?? "" ).Any() )
			{
				foreach( var indVal in tInfo.SeriesList.Split( ',' ).Select( s => s.Trim() ) )
				{
					var oSeries = database.Serieses.FirstOrDefault( s => s.Name == indVal );
					if( oSeries != null )
					{
						SeriesTypical stData = new SeriesTypical();
						stData.IsPrimary = false;
						stData.Series = oSeries;
						stData.Typical = tData;
						database.SeriesTypicals.AddObject( stData );
					}
				}
				tData.SeriesList = string.Join( ", ", tData.SeriesTypicals.Where( st => !st.IsPrimary ).Select( st => st.Series.Name ) );
			}

			var img = database.ImageFiles.FirstOrDefault( i => i.Name == tInfo.RenderingImage );
			if( img != null )
			{
				TypicalImageFile sif = new TypicalImageFile();
				sif.IsFeatured = true;
				sif.ImageFile = img;
				sif.Typical = tData;
				database.TypicalImageFiles.AddObject( sif );
			}

			{
				var attData = database.TAttributes.FirstOrDefault( a => a.Name == "pricing" );
				if( attData == null )
				{
					attData = new PDWDBContext.TAttribute();
					attData.Name = "pricing";
					database.TAttributes.AddObject( attData );
				}

				var attForTypical = new TypicalIntAttribute();
				attForTypical.TAttribute = attData;
				attForTypical.Value = tInfo.ListPrice;
				attForTypical.Typical = tData;
				database.TypicalIntAttributes.AddObject( attForTypical );
			}

			var rootLocation = Path.Combine( ConfigurationManager.AppSettings["SpecRequestDocumentLocation"], tInfo.Name );

			if( tInfo.dwgFile != null )
			{
				FillTypicalFileData( tData, rootLocation, tInfo.dwgFile.FileName, tInfo.dwgFile.InputStream, "drawing dwg" );
			}

			if( tInfo.xlsFile != null )
			{
				FillTypicalFileData( tData, rootLocation, tInfo.xlsFile.FileName, tInfo.xlsFile.InputStream, "spec & price xls" );
			}

			if( tInfo.sifFile != null )
			{
				FillTypicalFileData( tData, rootLocation, tInfo.sifFile.FileName, tInfo.sifFile.InputStream, "spec & price sif" );
			}

			if( tInfo.sp4File != null )
			{
				FillTypicalFileData( tData, rootLocation, tInfo.sp4File.FileName, tInfo.sp4File.InputStream, "spec & price sp4" );
			}

			if( tInfo.pdfFile != null )
			{
				FillTypicalFileData( tData, rootLocation, tInfo.pdfFile.FileName, tInfo.pdfFile.InputStream, "spec & price pdf" );
			}

			FillTypicalAttribute( tData, tInfo.Footprint, "Footprint" );
			FillTypicalAttribute( tData, tInfo.Material, "Material" );
			FillTypicalAttribute( tData, tInfo.Finish, "Finish" );

			arrKeywordList.Add( tInfo.FeaturedSeries );
			arrKeywordList.Add( tInfo.Name );
			arrKeywordList.Add( tInfo.SeriesList );
			//				arrKeywordList.Add( tInfo.Keywords );

			tData.DBKeywords = SearchText.GetKeywordList( arrKeywordList );

			tData.SpecRequest = specRequest;
			tData.Notes = tInfo.Notes;
			tData.AvailableForIn2 = tInfo.AvailableForIn2;

			database.Typicals.AddObject( tData );

			return database.SaveChanges() > 0;
		}

		private void FillTypicalFileData( Typical tData, string rootLocation, string fileName, Stream inputStream, string header )
		{
			if( !Directory.Exists( rootLocation ) )
			{
				Directory.CreateDirectory( rootLocation );
			}

			var fileLocation = Path.Combine( rootLocation, fileName );

			using( var fileStream = File.Create( fileLocation ) )
			{
				inputStream.CopyTo( fileStream );
			}

			var attData = database.TAttributes.FirstOrDefault( a => a.Name == header );
			if( attData == null )
			{
				attData = new PDWDBContext.TAttribute();
				attData.Name = header;
				database.TAttributes.AddObject( attData );
			}

			if( tData.TypicalTextAttributes.Any( tta => tta.AttributeID == attData.AttributeID ) )
			{
				database.DeleteObject( tData.TypicalTextAttributes.First( tta => tta.AttributeID == attData.AttributeID ) );
			}

			var attForTypical = new TypicalTextAttribute();
			attForTypical.TAttribute = attData;
			attForTypical.Value = @"Typicals/" + tData.Name + @"/" + fileName;
			attForTypical.Typical = tData;
			database.TypicalTextAttributes.AddObject( attForTypical );
		}

		public TypicalMgmtInfo GetTypical( int requestId )
		{
			var sInfo = database.SpecRequests.FirstOrDefault( s => s.RequestID == requestId );
			if( sInfo == null )
			{
				throw new Exception( "Unable to find Spec Request" );
			}

			var tInfo = sInfo.Typicals.FirstOrDefault();
			if( tInfo == null )
			{
				throw new Exception( "Unable to find Typical for Spec Request" );
			}

			var typData = new TypicalMgmtInfo()
			{
				TypicalID = tInfo.TypicalID,
				RequestID = tInfo.SpecRequestID.Value,
				Name = tInfo.Name,
				Notes = tInfo.Notes,
				AvailableForIn2 = tInfo.AvailableForIn2 ?? false,
				SeriesList = string.Join( ",", tInfo.SeriesTypicals.Where( st => !st.IsPrimary ).Select( st => st.Series.Name ) ),
				FeaturedSeries = string.Join( ",", tInfo.SeriesTypicals.Where( st => st.IsPrimary ).Select( st => st.Series.Name ) ),
				
				ListPrice = tInfo.IntAttribute( "Pricing" ),

				Footprint = string.Join( ",", tInfo.AttributeSet( "Footprint" ) ),
				Material = string.Join( ",", tInfo.AttributeSet( "Material" ) ),
				Finish = string.Join( ",", tInfo.AttributeSet( "Finish" ) ),

				RenderingImage = string.Join( ",", tInfo.TypicalImageFiles.Where( tif => tif.IsFeatured ).Select( tif => tif.ImageFile.Name ) ),

				xlsFileName = "",
				sifFileName = "",
				sp4FileName = "",
				pdfFileName = "",
				dwgFileName = ""
			};

			var xlsFiles = tInfo.TextAttribute( "Spec & Price XLS" );
			if( xlsFiles.Any() )
			{
				typData.xlsFileName = xlsFiles.Split( '/' ).Last();
			}

			var sifFiles = tInfo.TextAttribute( "Spec & Price SIF" );
			if( sifFiles.Any() )
			{
				typData.sifFileName = sifFiles.Split( '/' ).Last();
			}

			var sp4Files = tInfo.TextAttribute( "Spec & Price SP4" );
			if( sp4Files.Any() )
			{
				typData.sp4FileName = sp4Files.Split( '/' ).Last();
			}

			var pdfFiles = tInfo.TextAttribute( "Spec & Price PDF" );
			if( pdfFiles.Any() )
			{
				typData.pdfFileName = pdfFiles.Split( '/' ).Last();
			}

			var dwgFiles = tInfo.TextAttribute( "Drawing DWG" );
			if( dwgFiles.Any() )
			{
				typData.dwgFileName = dwgFiles.Split( '/' ).Last();
			}

			return typData;
		}

		public bool UpdateTypical( TypicalMgmtInfo tInfo )
		{
			var tData = database.Typicals.FirstOrDefault( t => t.TypicalID == tInfo.TypicalID );
			if( tData == null )
			{
				throw new Exception( "Unable to find Typical for Spec Request" );
			}

			tData.SeriesTypicals.ToList().ForEach( st => database.DeleteObject( st ) );

			List<string> arrKeywordList = new List<string>();
			var rSeries = database.Serieses.FirstOrDefault( s => s.Name == tInfo.FeaturedSeries );
			if( rSeries != null )
			{
				SeriesTypical stData = new SeriesTypical();
				stData.IsPrimary = true;
				stData.Series = rSeries;
				stData.Typical = tData;
				database.SeriesTypicals.AddObject( stData );
			}
			else
			{
				throw new Exception( "Unable to find Featured Series" );
			}
			tData.FeaturedSeries = rSeries.Name;

			if( ( tInfo.SeriesList ?? "" ).Any() )
			{
				foreach( var indVal in tInfo.SeriesList.Split( ',' ).Select( s => s.Trim() ) )
				{
					var oSeries = database.Serieses.FirstOrDefault( s => s.Name == indVal );
					if( oSeries != null )
					{
						SeriesTypical stData = new SeriesTypical();
						stData.IsPrimary = false;
						stData.Series = oSeries;
						stData.Typical = tData;
						database.SeriesTypicals.AddObject( stData );
					}
				}
				tData.SeriesList = string.Join( ", ", tData.SeriesTypicals.Where( st => !st.IsPrimary ).Select( st => st.Series.Name ) );
			}
			else
			{
				tData.SeriesList = null;
			}

			tData.TypicalImageFiles.ToList().ForEach( tif => database.DeleteObject( tif ) );

			var img = database.ImageFiles.FirstOrDefault( i => i.Name == tInfo.RenderingImage );
			if( img != null )
			{
				TypicalImageFile sif = new TypicalImageFile();
				sif.IsFeatured = true;
				sif.ImageFile = img;
				sif.Typical = tData;
				database.TypicalImageFiles.AddObject( sif );
			}

			{
				var attData = database.TAttributes.FirstOrDefault( a => a.Name == "Pricing" );
				if( attData == null )
				{
					attData = new PDWDBContext.TAttribute();
					attData.Name = "Pricing";
					database.TAttributes.AddObject( attData );
				}

				if( tData.TypicalIntAttributes.Any( tia => tia.AttributeID == attData.AttributeID ) )
				{
					database.DeleteObject( tData.TypicalIntAttributes.First( tia => tia.AttributeID == attData.AttributeID ) );
				}

				var attForTypical = new TypicalIntAttribute();
				attForTypical.TAttribute = attData;
				attForTypical.Value = tInfo.ListPrice;
				attForTypical.Typical = tData;
				database.TypicalIntAttributes.AddObject( attForTypical );
			}

			var rootLocation = Path.Combine( ConfigurationManager.AppSettings["SpecRequestDocumentLocation"], tInfo.Name );

			if( tInfo.dwgFile != null )
			{
				FillTypicalFileData( tData, rootLocation, tInfo.dwgFile.FileName, tInfo.dwgFile.InputStream, "drawing dwg" );
			}

			if( tInfo.xlsFile != null )
			{
				FillTypicalFileData( tData, rootLocation, tInfo.xlsFile.FileName, tInfo.xlsFile.InputStream, "spec & price xls" );
			}

			if( tInfo.sifFile != null )
			{
				FillTypicalFileData( tData, rootLocation, tInfo.sifFile.FileName, tInfo.sifFile.InputStream, "spec & price sif" );
			}

			if( tInfo.sp4File != null )
			{
				FillTypicalFileData( tData, rootLocation, tInfo.sp4File.FileName, tInfo.sp4File.InputStream, "spec & price sp4" );
			}

			if( tInfo.pdfFile != null )
			{
				FillTypicalFileData( tData, rootLocation, tInfo.pdfFile.FileName, tInfo.pdfFile.InputStream, "spec & price pdf" );
			}

			FillTypicalAttribute( tData, tInfo.Footprint, "Footprint" );
			FillTypicalAttribute( tData, tInfo.Material, "Material" );
			FillTypicalAttribute( tData, tInfo.Finish, "Finish" );

			arrKeywordList.Add( tInfo.FeaturedSeries );
			arrKeywordList.Add( tInfo.Name );
			arrKeywordList.Add( tInfo.SeriesList );
			//				arrKeywordList.Add( tInfo.Keywords );

			tData.DBKeywords = SearchText.GetKeywordList( arrKeywordList );

			tData.Notes = tInfo.Notes;
			tData.AvailableForIn2 = tInfo.AvailableForIn2;

			return database.SaveChanges() > 0;
		}

		private void FillTypicalAttribute( Typical tData, string val, string header )
		{
			if( ( val ?? "" ).Any() )
			{
				var attData = database.TAttributes.FirstOrDefault( a => a.Name == header );
				if( attData == null )
				{
					attData = new PDWDBContext.TAttribute();
					switch( header.ToLower() )
					{
						case "footprint":
							break;
						default:
							attData.DetailItem = true;
							break;
					}
					attData.Name = header;
					database.TAttributes.AddObject( attData );
				}

				if( tData.TypicalOptionAttributes.Any( toa => toa.AttributeID == attData.AttributeID ) )
				{
					database.DeleteObject( tData.TypicalOptionAttributes.First( toa => toa.AttributeID == attData.AttributeID ) );
				}

				var values = val.Split( ',' );
				foreach( var indVal in values.Select( s => s.Trim() ) )
				{
					var optVal = attData.TAttributeOptions.FirstOrDefault( ao => ao.Name == indVal );
					if( optVal == null )
					{
						if( indVal.Length > 500 )
						{
							throw new Exception( string.Format( "Cannot add option value {0} for {1}", indVal, header ) );
						}
						optVal = new TAttributeOption();
						optVal.Name = indVal;
						database.TAttributeOptions.AddObject( optVal );
						attData.TAttributeOptions.Add( optVal );
					}

					var attForTypical = new TypicalOptionAttribute();
					attForTypical.TAttribute = attData;
					attForTypical.TAttributeOption = optVal;
					attForTypical.Typical = tData;
					database.TypicalOptionAttributes.AddObject( attForTypical );
				}
			}
		}
	}
}
