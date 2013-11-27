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
				RepGroup = sRequest.PaoliSalesRepGroupID.HasValue ? sRequest.PaoliSalesRepGroup.Name : "",
				SpecTeamMember = sRequest.PaoliSpecTeamMemberID.HasValue ? sRequest.SpecTeamMember.FullName : "",
				IsRecommended = sRequest.IsGoodForWeb ?? false
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
					i.PaoliSalesRepGroup.Name.Contains( paramDetails.sSearch ) ||
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
				case "repgroup":
					if( paramDetails.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = requestList.OrderBy( v => v.PaoliSalesRepGroup.Name );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.PaoliSalesRepGroup.Name );
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
				PaoliSpecTeamMember = sInfo.PaoliSpecTeamMemberID,
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
				imgFileList = GetFileListing( sInfo.Name, sInfo.SpecRequestFiles, "img" )
					
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
			newSpec.PaoliSpecTeamMemberID = sInfo.PaoliSpecTeamMember;
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

				newSpec.Name = string.Format( "S{0}", newSpec.RequestID.ToString( "D4" ) );

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
			specInfo.PaoliSpecTeamMemberID = sInfo.PaoliSpecTeamMember;
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
	}
}
