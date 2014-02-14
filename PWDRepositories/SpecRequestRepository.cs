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
				SpecTeamMember = sRequest.PaoliSpecTeamMemberID.HasValue ? sRequest.SpecTeamMember.LastName : "",
				SalesRepGroup = sRequest.PaoliSalesRepGroupID.HasValue ? sRequest.PaoliSalesRepGroup.Name : "",
				IsRecommended = sRequest.IsGoodForWeb ?? false,
				IsPublished = sRequest.Typicals.Any(),
				IsCompleted = sRequest.IsCompleted,
				CreatedDate = sRequest.RequestDate
			};
		}

		public IEnumerable<SpecRequestSummary> GetUserRequestList( UserSpecRequestTableParams paramDetails, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var requestList = database.SpecRequests.AsQueryable();

			if( PaoliWebUser.CurrentUser.IsDealerUser )
			{
				displayedRecords = 10;
				requestList = requestList.Where( s => s.PrimaryCompanyID == database.Users.FirstOrDefault( u => u.UserID == PaoliWebUser.CurrentUser.UserId ).CompanyID );
				requestList = requestList.OrderByDescending( s => s.RequestID );
			}
			else if( PaoliWebUser.CurrentUser.IsInRole( PaoliWebUser.PaoliWebRole.PaoliSalesRep ) )
			{
				displayedRecords = 10;
				requestList = requestList.Where( s => s.PaoliSalesRepGroupID == database.Users.FirstOrDefault( u => u.UserID == PaoliWebUser.CurrentUser.UserId ).CompanyID );
				requestList = requestList.OrderByDescending( s => s.RequestID );
			}
			else
			{
				displayedRecords = Math.Max( database.SpecRequests.Count( s => !s.IsCompleted ), 10 );
				requestList = requestList.OrderBy( s => s.IsCompleted ).ThenByDescending( s => s.RequestID );
			}

			totalRecords = requestList.Count();

			if( !string.IsNullOrEmpty( paramDetails.sSearch ) )
			{
				requestList = requestList.Where( i =>
					i.Name.Contains( paramDetails.sSearch ) ||
					i.PrimaryCompany.Name.Contains( paramDetails.sSearch ) ||
					i.PaoliSalesRepGroup.Name.Contains( paramDetails.sSearch ) ||
					i.ProjectName.Contains( paramDetails.sSearch ) ||
					i.SpecTeamMember.FirstName.Contains( paramDetails.sSearch ) ||
					i.SpecTeamMember.LastName.Contains( paramDetails.sSearch ) );
			}

			if( paramDetails.bMinView )
			{
				requestList = requestList.Take( displayedRecords );

				return requestList.ToList().Select( v => ToSpecRequestSummary( v ) );
			}

			string sortCol = paramDetails.sColumns.Split( ',' )[paramDetails.iSortCol_0];

			IQueryable<SpecRequest> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
				case "name":
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
				case "salesrepgroup":
					if( paramDetails.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = requestList.OrderBy( v => v.PaoliSalesRepGroup.Name );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.PaoliSalesRepGroup.Name );
					}
					break;
				case "status":
				default:
					if( paramDetails.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = requestList.OrderByDescending( s => s.IsCompleted ).ThenBy( s => s.RequestID );
					}
					else
					{
						filteredAndSorted = requestList.OrderBy( s => s.IsCompleted ).ThenByDescending( s => s.RequestID );
					}
					break;
			}

			displayedRecords = requestList.Count();

			if( ( displayedRecords > paramDetails.iDisplayLength ) && ( paramDetails.iDisplayLength > 0 ) )
			{
				filteredAndSorted = filteredAndSorted.Skip( paramDetails.iDisplayStart ).Take( paramDetails.iDisplayLength );
			}

			return filteredAndSorted.ToList().Select( v => ToSpecRequestSummary( v ) );
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
					i.ProjectName.Contains( paramDetails.sSearch ) ||
					i.SpecTeamMember.FirstName.Contains( paramDetails.sSearch ) ||
					i.SpecTeamMember.LastName.Contains( paramDetails.sSearch ) );
			}

			if( paramDetails.pendingOnly )
			{
				requestList = requestList.Where( i => !i.IsCompleted );
			}
			if( paramDetails.recommendedOnly )
			{
				requestList = requestList.Where( i => i.IsCompleted && (i.IsGoodForWeb ?? false) && !i.Typicals.Any() );
			}

			displayedRecords = requestList.Count();

			string sortCol = paramDetails.sColumns.Split( ',' )[paramDetails.iSortCol_0];

			IQueryable<SpecRequest> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
				case "name":
				case "editbuttons":
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
				case "salesrepgroup":
					if( paramDetails.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = requestList.OrderBy( v => v.PaoliSalesRepGroup.Name );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.PaoliSalesRepGroup.Name );
					}
					break;
				case "createddate":
					if( paramDetails.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = requestList.OrderBy( v => v.RequestDate );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.RequestDate );
					}
					break;
			}

			if( ( displayedRecords > paramDetails.iDisplayLength ) && ( paramDetails.iDisplayLength > 0 ) )
			{
				filteredAndSorted = filteredAndSorted.Skip( paramDetails.iDisplayStart ).Take( paramDetails.iDisplayLength );
			}

			return filteredAndSorted.ToList().Select( v => ToSpecRequestSummary( v ) );
		}

		private SpecRequestInformation.FileListing GetFileListing( string name, IEnumerable<SpecRequestFile> fileList )
		{
			return new SpecRequestInformation.FileListing()
			{
				SpecName = name,
				FileList =
					fileList
					.OrderByDescending( f => f.UploadDate )
					.Select( i =>
						new SpecRequestInformation.FileInformation()
						{
							FileName = i.Name,
							Extension = i.Extension,
							UploadDate = i.UploadDate.ToString( "MM/dd/yyyy HH:mm" ),
							VersionNumber = i.VersionNumber
						} )
					.ToList()
			};
		}

		public SpecRequestInformation NewSpecRequest()
		{
			return new SpecRequestInformation()
			{
				Casegoods = database.Serieses.Where( s => s.Category.Name == "Casegood" ).Select( s => s.Name ).ToList(),
				Seating = database.Serieses.Where( s => s.Category.Name == "Seating" ).Select( s => s.Name ).ToList(),
				Conferencing = database.Serieses.Where( s => s.Category.Name == "Tables" ).Select( s => s.Name ).ToList(),
				Finishes = new List<string>() { "Laminate", "Veneer" }
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
				PaoliSalesRepMemberID = sInfo.PaoliSalesRepMemberID,
				DealerID = sInfo.PrimaryCompanyID,
				DealerMemberID = sInfo.DealerSalesRepID,
				IsGSA = sInfo.IsGSA ?? false,
				SavedLocation = sInfo.SavedLocation,
				ListPrice = sInfo.ListPrice,
				Received = sInfo.Received ?? false,
				SPLQuote = sInfo.SPLQuote,
				PaoliSpecTeamMember = sInfo.PaoliSpecTeamMemberID ?? 0,
				IsGoodForWeb = ( sInfo.IsGoodForWeb ?? false ) && sInfo.IsCompleted,
				AvailableForIn2 = sInfo.AvailableForIn2 ?? false,
				IsCompleted = sInfo.IsCompleted,
				Footprint = sInfo.Footprint,
				Notes = sInfo.Notes,
				addlFileList = GetFileListing( sInfo.Name, sInfo.SpecRequestFiles.Where( f => !f.IsSpecTeam ) ),
				specTeamFileList = GetFileListing( sInfo.Name, sInfo.SpecRequestFiles.Where( f => f.IsSpecTeam ) ),
				CreatedDate = sInfo.RequestDate.Value,
				DealerName = sInfo.PrimaryCompanyID.HasValue ? sInfo.PrimaryCompany.Name : "None",
				SalesRepGroupName = sInfo.PaoliSalesRepGroupID.HasValue ? sInfo.PaoliSalesRepGroup.Name : "None",
				SalesRepMemberName = sInfo.PaoliSalesRepMemberID.HasValue ? sInfo.PaoliSalesRepMember.FullName : "None",
				DealerSalesRepName = sInfo.DealerSalesRepID.HasValue ? sInfo.DealerSalesRep.FullName : "None",
				SpecTeamMemberName = sInfo.PaoliSpecTeamMemberID.HasValue ? sInfo.SpecTeamMember.FullName : "None",
				EndCustomer = sInfo.EndCustomer,
				ProjectSize = sInfo.ProjectSize,
				NeedFloorplanSpecs = sInfo.NeedFloorplanSpecs,
				Need3DDrawing = sInfo.Need3DDrawing,
				NeedValueEng = sInfo.NeedValueEng,
				NeedPhotoRendering = sInfo.NeedPhotoRendering,
				Need2DDrawing = sInfo.Need2DDrawing,
				NeedAuditSpecs = sInfo.NeedAuditSpecs,
				Casegoods = (sInfo.Casegoods ?? "").Split( ',' ).ToList(),
				Conferencing = (sInfo.Conferencing ?? "").Split( ',' ).ToList(),
				Seating = (sInfo.Seating ?? "").Split( ',' ).ToList(),
				Finishes = ( sInfo.Finishes ?? "" ).Split( ',' ).ToList(),
				LaminatePreference = sInfo.LaminatePreference,
				OtherFinishDetails = sInfo.OtherFinishDetails,
				Grommets = sInfo.Grommets,
				GrommetDetails = sInfo.Grommets ? sInfo.GrommetDetails : null,
				DrawerOption = sInfo.DrawerOption,
				FabricGrade = sInfo.FabricGrade,
				Fabric = sInfo.FabricDetails,
				SpecialRequests = sInfo.SpecialRequests,
				CreatedByUser = sInfo.CreatedByUserId.HasValue ? sInfo.CreatedByUser.FullName : null,
				CreatedByUserCompany = sInfo.CreatedByUserId.HasValue ? sInfo.CreatedByUser.Company.Name : null,
				CreatedByUserPhone = sInfo.CreatedByUserId.HasValue ? sInfo.CreatedByUser.BusinessPhone : null,
				CreatedByUserEmail = sInfo.CreatedByUserId.HasValue ? sInfo.CreatedByUser.Email : null,
					
			};
		}

		public bool AddSpecRequest( SpecRequestInformation sInfo )
		{
			SpecRequest newSpec = new SpecRequest();

			newSpec.ProjectName = sInfo.ProjectName;
			newSpec.PaoliSalesRepGroupID = ( sInfo.PaoliSalesRepGroupID ?? 0 ) > 0 ? sInfo.PaoliSalesRepGroupID : null;
			newSpec.PaoliSalesRepMemberID = ( sInfo.PaoliSalesRepMemberID ?? 0 ) > 0 ? sInfo.PaoliSalesRepMemberID : null;
			newSpec.PrimaryCompanyID = ( sInfo.DealerID ?? 0 ) > 0 ? sInfo.DealerID : null;
			newSpec.DealerSalesRepID = ( sInfo.DealerMemberID ?? 0 ) > 0 ? sInfo.DealerMemberID : null;
			newSpec.RequestDate = DateTime.UtcNow;
			newSpec.IsGSA = sInfo.IsGSA;
			newSpec.AvailableForIn2 = sInfo.AvailableForIn2;
			newSpec.PaoliSpecTeamMemberID = ( sInfo.PaoliSpecTeamMember ?? 0 ) > 0 ? sInfo.PaoliSpecTeamMember : null;
			newSpec.LastModifiedDate = DateTime.UtcNow;
			newSpec.Notes = sInfo.Notes;
			newSpec.EndCustomer = sInfo.EndCustomer;
			newSpec.ProjectSize = sInfo.ProjectSize;
			newSpec.NeedFloorplanSpecs = sInfo.NeedFloorplanSpecs;
			newSpec.Need3DDrawing = sInfo.Need3DDrawing;
			newSpec.NeedValueEng = sInfo.NeedValueEng;
			newSpec.NeedPhotoRendering = sInfo.NeedPhotoRendering;
			newSpec.Need2DDrawing = sInfo.Need2DDrawing;
			newSpec.NeedAuditSpecs = sInfo.NeedAuditSpecs;
			newSpec.Casegoods = sInfo.Casegoods != null ? string.Join( ",", sInfo.Casegoods ) : "";
			newSpec.Conferencing = sInfo.Conferencing != null ? string.Join( ",", sInfo.Conferencing ) : "";
			newSpec.Seating = sInfo.Seating != null ? string.Join( ",", sInfo.Seating ) : "";
			newSpec.Finishes = sInfo.Finishes != null ? string.Join( ",", sInfo.Finishes ) : "";
			newSpec.LaminatePreference = ( sInfo.Finishes != null ? sInfo.Finishes.Contains( "Laminate" ) : false ) ? sInfo.LaminatePreference : null;
			newSpec.OtherFinishDetails = sInfo.OtherFinishDetails;
			newSpec.Grommets = sInfo.Grommets;
			newSpec.GrommetDetails = sInfo.Grommets ? sInfo.GrommetDetails : null;
			newSpec.DrawerOption = sInfo.Casegoods != null && sInfo.Casegoods.Any() ? sInfo.DrawerOption : null;
			newSpec.FabricGrade = sInfo.Seating != null && sInfo.Seating.Any() ? sInfo.FabricGrade : null;
			newSpec.FabricDetails = sInfo.Seating != null && sInfo.Seating.Any() ? sInfo.Fabric : null;
			newSpec.SpecialRequests = sInfo.SpecialRequests;
			newSpec.SavedLocation = null;
			newSpec.ListPrice = null;
			newSpec.Received = false;
			newSpec.SPLQuote = null;
			newSpec.IsGoodForWeb = false;
			newSpec.IsCompleted = false;
			newSpec.Footprint = null;
			newSpec.CreatedByUserId = PaoliWebUser.CurrentUser.UserId;

			database.SpecRequests.AddObject( newSpec );

			if( database.SaveChanges() > 0 )
			{
				database.Refresh( System.Data.Objects.RefreshMode.StoreWins, newSpec );

				newSpec.Name = string.Format( "T{0}", newSpec.RequestID.ToString( "D4" ) );

				var rootLocation = Path.Combine( ConfigurationManager.AppSettings["SpecRequestDocumentLocation"], newSpec.Name );

				foreach( var fileStream in sInfo.addlFiles )
				{
					if( fileStream != null )
					{
						SaveNewFileVersion( newSpec, rootLocation, Path.GetExtension( fileStream.FileName ).Trim( '.' ), fileStream.InputStream, fileStream.FileName, false );
					}
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
			specInfo.PaoliSalesRepGroupID = ( sInfo.PaoliSalesRepGroupID ?? 0 ) > 0 ? sInfo.PaoliSalesRepGroupID : null;
			specInfo.PaoliSalesRepMemberID = ( sInfo.PaoliSalesRepMemberID ?? 0 ) > 0 ? sInfo.PaoliSalesRepMemberID : null;
			specInfo.PrimaryCompanyID = ( sInfo.DealerID ?? 0 ) > 0 ? sInfo.DealerID : null;
			specInfo.DealerSalesRepID = ( sInfo.DealerMemberID ?? 0 ) > 0 ? sInfo.DealerMemberID : null;
			specInfo.RequestDate = DateTime.UtcNow;
			specInfo.IsGSA = sInfo.IsGSA;
			specInfo.AvailableForIn2 = sInfo.AvailableForIn2;
			specInfo.PaoliSpecTeamMemberID = ( sInfo.PaoliSpecTeamMember ?? 0 ) > 0 ? sInfo.PaoliSpecTeamMember : null;
			specInfo.LastModifiedDate = DateTime.UtcNow;
			specInfo.Notes = sInfo.Notes;
			specInfo.EndCustomer = sInfo.EndCustomer;
			specInfo.ProjectSize = sInfo.ProjectSize;
			specInfo.NeedFloorplanSpecs = sInfo.NeedFloorplanSpecs;
			specInfo.Need3DDrawing = sInfo.Need3DDrawing;
			specInfo.NeedValueEng = sInfo.NeedValueEng;
			specInfo.NeedPhotoRendering = sInfo.NeedPhotoRendering;
			specInfo.Need2DDrawing = sInfo.Need2DDrawing;
			specInfo.NeedAuditSpecs = sInfo.NeedAuditSpecs;
			specInfo.Casegoods = sInfo.Casegoods != null ? string.Join( ",", sInfo.Casegoods ) : "";
			specInfo.Conferencing = sInfo.Conferencing != null ? string.Join( ",", sInfo.Conferencing ) : "";
			specInfo.Seating = sInfo.Seating != null ? string.Join( ",", sInfo.Seating ) : "";
			specInfo.Finishes = sInfo.Finishes != null ? string.Join( ",", sInfo.Finishes ) : "";
			specInfo.LaminatePreference = ( sInfo.Finishes != null ? sInfo.Finishes.Contains( "Laminate" ) : false ) ? sInfo.LaminatePreference : null;
			specInfo.OtherFinishDetails = sInfo.OtherFinishDetails;
			specInfo.Grommets = sInfo.Grommets;
			specInfo.GrommetDetails = sInfo.Grommets ? sInfo.GrommetDetails : null;
			specInfo.DrawerOption = sInfo.Casegoods != null && sInfo.Casegoods.Any() ? sInfo.DrawerOption : null;
			specInfo.FabricGrade = sInfo.Seating != null && sInfo.Seating.Any() ? sInfo.FabricGrade : null;
			specInfo.FabricDetails = sInfo.Seating != null && sInfo.Seating.Any() ? sInfo.Fabric : null;
			specInfo.SpecialRequests = sInfo.SpecialRequests;
			specInfo.SavedLocation = sInfo.SavedLocation;
			specInfo.ListPrice = sInfo.ListPrice;
			specInfo.Received = sInfo.Received;
			specInfo.SPLQuote = sInfo.SPLQuote;
			specInfo.IsGoodForWeb = sInfo.IsCompleted && sInfo.IsGoodForWeb;
			specInfo.IsCompleted = sInfo.IsCompleted;
			specInfo.Footprint = sInfo.Footprint;

			var rootLocation = Path.Combine( ConfigurationManager.AppSettings["SpecRequestDocumentLocation"], specInfo.Name );

			foreach( var fileStream in sInfo.addlFiles )
			{
				if( fileStream != null )
				{
					SaveNewFileVersion( specInfo, rootLocation, Path.GetExtension( fileStream.FileName ).Trim( '.' ), fileStream.InputStream, fileStream.FileName, false );
				}
			}

			foreach( var fileStream in sInfo.specTeamFiles )
			{
				if( fileStream != null )
				{
					SaveNewFileVersion( specInfo, rootLocation, Path.GetExtension( fileStream.FileName ).Trim( '.' ), fileStream.InputStream, fileStream.FileName, true );
				}
			}

			return database.SaveChanges() > 0;
		}

		private bool SaveNewFileVersion( SpecRequest sRequest, string rootLocation, string fileType, Stream inputStream, string fileName, bool isSpecTeam )
		{
			var existingCount = sRequest.SpecRequestFiles.Count( f => f.Extension == fileType );
			existingCount++;

			var fileLocation = Path.Combine( rootLocation, fileType, existingCount.ToString() );

			if( !Directory.Exists( fileLocation ) )
			{
				Directory.CreateDirectory( fileLocation );
			}

			fileName = fileName.Replace( "&", "and" );

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
			newFile.IsSpecTeam = isSpecTeam;

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

			if( ( tInfo.AdditionalImages ?? "" ).Any() )
			{
				foreach( var indVal in tInfo.AdditionalImages.Split( ',' ).Select( s => s.Trim() ) )
				{
					var oImg = database.ImageFiles.FirstOrDefault( s => s.Name == indVal );
					if( oImg != null )
					{
						TypicalImageFile sif = new TypicalImageFile();
						sif.IsFeatured = false;
						sif.ImageFile = oImg;
						sif.Typical = tData;
						database.TypicalImageFiles.AddObject( sif );
					}
				}
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

			fileName = fileName.Replace( "&", "and" );

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
				AdditionalImages = string.Join( ",", tInfo.TypicalImageFiles.Where( tif => !tif.IsFeatured ).Select( tif => tif.ImageFile.Name ) ),

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

			if( ( tInfo.AdditionalImages ?? "" ).Any() )
			{
				foreach( var indVal in tInfo.AdditionalImages.Split( ',' ).Select( s => s.Trim() ) )
				{
					var oImg = database.ImageFiles.FirstOrDefault( s => s.Name == indVal );
					if( oImg != null )
					{
						TypicalImageFile sif = new TypicalImageFile();
						sif.IsFeatured = false;
						sif.ImageFile = oImg;
						sif.Typical = tData;
						database.TypicalImageFiles.AddObject( sif );
					}
				}
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

		public IEnumerable<string> GetFootprintList( string query )
		{
			return database.SpecRequests
				.Where( ao => ao.Footprint.Contains( query ) )
				.Select( ao => ao.Footprint )
				.Distinct()
				.OrderBy( s => s );
		}
	}
}
