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
using PDWInfrastructure.EmailSenders;
using System.Data.Entity;

namespace PWDRepositories
{
	public class SpecRequestRepository : BaseRepository
	{
		public SpecRequestRepository()
		{
		}

		private SpecRequestSummary ToSpecRequestSummary( SpecRequest sRequest )
		{
			return new SpecRequestSummary()
			{
				RequestID = sRequest.RequestID,
				Name = sRequest.Name,
				Dealer = sRequest.PrimaryCompanyID.HasValue ? sRequest.PrimaryCompany.FullName : "",
				DealerMember = sRequest.DealerSalesRepID.HasValue ? sRequest.DealerSalesRep.FullName : sRequest.DealerPOCText,
				ProjectName = sRequest.ProjectID.HasValue ? sRequest.Project.ProjectName : "",
				SpecTeamMember = sRequest.PaoliSpecTeamMemberID.HasValue ? sRequest.SpecTeamMember.LastName : "",
				SalesRepGroup = sRequest.PaoliSalesRepGroupID.HasValue ? sRequest.PaoliSalesRepGroup.FullName : "",
				IsRecommended = sRequest.IsGoodForWeb ?? false,
				IsPublished = sRequest.Typicals.Any( t => t.IsPublished ),
				HasTypical = sRequest.Typicals.Any(),
				IsCompleted = sRequest.IsCompleted,
				IsOnHold = sRequest.IsOnHold,
				IsCanceled = sRequest.IsCanceled,
				CreatedDate = sRequest.CreatedOnDate,
				IsAuditSpecOnly = sRequest.NeedAuditSpecs && !sRequest.NeedFloorplanSpecs &&
					!sRequest.NeedPhotoRendering && !sRequest.Need2DDrawing && !sRequest.NeedValueEng
			};
		}

		public IEnumerable<SpecRequestSummary> GetHomePageRequestList( int itemCount )
		{
			var requestList = database.SpecRequests
				.Include( s => s.PrimaryCompany )
				.Include( s => s.DealerSalesRep )
				.Include( s => s.SpecTeamMember )
				.Include( s => s.PaoliSalesRepGroup )
				.Include( s => s.Typicals )
				.Include( s => s.SpecRequestEvents )
				.Include( s => s.Project )
				.AsQueryable();

			if( PaoliWebUser.CurrentUser.IsDealerUser )
			{
				requestList = requestList.Where( s => s.PrimaryCompanyID == database.Users.FirstOrDefault( u => u.UserID == PaoliWebUser.CurrentUser.UserId ).CompanyID );
			}
			else if( PaoliWebUser.CurrentUser.IsInRole( PaoliWebUser.PaoliWebRole.PaoliSalesRep ) )
			{
				requestList = requestList.Where( s => s.PaoliSalesRepGroupID == database.Users.FirstOrDefault( u => u.UserID == PaoliWebUser.CurrentUser.UserId ).CompanyID );
			}

			requestList = requestList
				.OrderByDescending( s => s.LastModifiedDate )
				.Take( itemCount );

			return requestList.ToList().Select( v => ToSpecRequestSummary( v ) );
		}

		public IEnumerable<SpecRequestExport> GetExportList()
		{
			return database.SpecRequests
				.OrderBy( r => r.Name )
				.ToList()
				.Select( r => ToSpecRequestExport( r ) );
		}

		private SpecRequestExport ToSpecRequestExport( SpecRequest sInfo )
		{
			return new SpecRequestExport()
			{
				Name = sInfo.Name,
				ProjectName = sInfo.ProjectID.HasValue ? sInfo.Project.ProjectName : "",
				DealerPointOfContact = sInfo.DealerPOCText,
				IsGSA = sInfo.ProjectID.HasValue ? (sInfo.Project.IsGSA ?? false) : false,
				ContractName = sInfo.ProjectID.HasValue ? (sInfo.Project.ContractID.HasValue ? sInfo.Project.GSAContract.Name : "") : "",
				ListPrice = sInfo.ListPrice,
				Received = sInfo.Received ?? false,
				SPLQuote = sInfo.SPLQuote,
				IsGoodForWeb = ( sInfo.IsGoodForWeb ?? false ) && sInfo.IsCompleted,
				AvailableForIn2 = sInfo.AvailableForIn2 ?? false,
				IsCompleted = sInfo.IsCompleted,
				IsOnHold = sInfo.IsOnHold,
				IsCanceled = sInfo.IsCanceled,
				Footprint = sInfo.Footprint,
				Notes = sInfo.Notes,
				NeedDWGFiles = sInfo.NeedDWGFiles,
				NeedSP4Files = sInfo.NeedSP4Files,
				NeedSIFFiles = sInfo.NeedSIFFiles,
				NeedXLSFiles = sInfo.NeedXLSFiles,
				NeedPDFFiles = sInfo.NeedPDFFiles,
				CreatedDate = sInfo.CreatedOnDate,
				DealerName = sInfo.PrimaryCompanyID.HasValue ? sInfo.PrimaryCompany.FullName : "None",
				SalesRepGroupName = sInfo.PaoliSalesRepGroupID.HasValue ? sInfo.PaoliSalesRepGroup.FullName : "None",
				SalesRepMemberName = sInfo.PaoliSalesRepMemberID.HasValue ? sInfo.PaoliSalesRepMember.FullName : "None",
				DealerSalesRepName = sInfo.DealerSalesRepID.HasValue ? sInfo.DealerSalesRep.FullName : "None",
				DealerSalesRepContact = sInfo.DealerSalesRepID.HasValue ? sInfo.DealerSalesRep.ContactInfo : "",
				SpecTeamMemberName = sInfo.PaoliSpecTeamMemberID.HasValue ? sInfo.SpecTeamMember.FullName : "Not Assigned",
				EndCustomer = sInfo.ProjectID.HasValue ? (sInfo.Project.EndCustomerID.HasValue ? sInfo.Project.EndCustomerCompany.Name : "") : "",
				ProjectSize = sInfo.ProjectSize,
				NeedFloorplanSpecs = sInfo.NeedFloorplanSpecs,
				NeedValueEng = sInfo.NeedValueEng,
				NeedPhotoRendering = sInfo.NeedPhotoRendering,
				Need2DDrawing = sInfo.Need2DDrawing,
				NeedAuditSpecs = sInfo.NeedAuditSpecs,
				Casegoods = sInfo.Casegoods,
				Conferencing = sInfo.Conferencing,
				Seating = sInfo.Seating,
				Finishes = sInfo.Finishes,
				LaminatePreference = sInfo.LaminatePreference,
				OtherFinishDetails = sInfo.OtherFinishDetails,
				Grommets = sInfo.Grommets,
				GrommetDetails = sInfo.Grommets ? sInfo.GrommetDetails : null,
				GrommetPosition = sInfo.Grommets ? sInfo.GrommetPosition : null,
				DrawerOption = sInfo.DrawerOption,
				FabricGrade = sInfo.FabricGrade,
				Fabric = sInfo.FabricDetails,
				SpecialRequests = sInfo.SpecialRequests,
				CreatedByUser = sInfo.CreatedByUserName,
				CreatedByUserCompany = sInfo.CreatedByCompany,
				CanceledDate = sInfo.CanceledOnDate,
				CanceledByUser = sInfo.CanceledByUserName,
				SpecTeamNotes = sInfo.SpecTeamNotes,
				InternalNotes = sInfo.InternalNotes,
				CompletedDate = sInfo.CompletedOnDate,
				ReOpenedCount = sInfo.ReOpenedCount

			};
		}

		public IEnumerable<SpecRequestSummary> GetUserRequestList( UserSpecRequestTableParams paramDetails, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var requestList = database.SpecRequests
				.Include( s => s.PrimaryCompany )
				.Include( s => s.DealerSalesRep )
				.Include( s => s.SpecTeamMember )
				.Include( s => s.PaoliSalesRepGroup )
				.Include( s => s.Typicals )
				.Include( s => s.SpecRequestEvents )
				.Include( s => s.Project )
				.AsQueryable();

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
					i.PrimaryCompany.BusinessUnitName.Contains( paramDetails.sSearch ) ||
					i.PaoliSalesRepGroup.Name.Contains( paramDetails.sSearch ) ||
					i.PaoliSalesRepGroup.BusinessUnitName.Contains( paramDetails.sSearch ) ||
					i.Project.ProjectName.Contains( paramDetails.sSearch ) ||
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
						filteredAndSorted = requestList.OrderBy( v => v.Project.ProjectName );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.Project.ProjectName );
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

			var requestList = database.SpecRequests
				.Include( s => s.PrimaryCompany )
				.Include( s => s.DealerSalesRep )
				.Include( s => s.SpecTeamMember )
				.Include( s => s.PaoliSalesRepGroup )
				.Include( s => s.Typicals )
				.Include( s => s.SpecRequestEvents )
				.Include( s => s.Project )
				.AsQueryable();

			totalRecords = requestList.Count();

			if( !string.IsNullOrEmpty( paramDetails.sSearch ) )
			{
				requestList = requestList.Where( i =>
					i.Name.Contains( paramDetails.sSearch ) ||
					i.PrimaryCompany.Name.Contains( paramDetails.sSearch ) ||
					i.PrimaryCompany.BusinessUnitName.Contains( paramDetails.sSearch ) ||
					i.PaoliSalesRepGroup.Name.Contains( paramDetails.sSearch ) ||
					i.PaoliSalesRepGroup.BusinessUnitName.Contains( paramDetails.sSearch ) ||
					i.Project.ProjectName.Contains( paramDetails.sSearch ) ||
					i.SpecTeamMember.FirstName.Contains( paramDetails.sSearch ) ||
					i.SpecTeamMember.LastName.Contains( paramDetails.sSearch ) );
			}

			if( paramDetails.pendingOnly )
			{
				requestList = requestList.Where( i => !i.IsCompleted );
			}
			if( paramDetails.recommendedOnly )
			{
				requestList = requestList.Where( i => i.IsCompleted && (i.IsGoodForWeb ?? false) && !i.Typicals.Any( t => t.IsPublished ) );
			}
			if( paramDetails.notYetAssigned )
			{
				requestList = requestList.Where( i => !i.IsCompleted && !i.PaoliSpecTeamMemberID.HasValue );
			}
			if( paramDetails.auditSpecOnly )
			{
				requestList = requestList.Where( i => i.NeedAuditSpecs && !i.NeedFloorplanSpecs &&
					!i.NeedPhotoRendering && !i.Need2DDrawing && !i.NeedValueEng );
			}
			if( paramDetails.showGSAOnly )
			{
				requestList = requestList.Where( i => i.Project.IsGSA ?? false );
			}
			if( paramDetails.companyId.HasValue )
			{
				requestList = requestList.Where( s => s.PrimaryCompanyID == paramDetails.companyId.Value || s.PaoliSalesRepGroupID == paramDetails.companyId.Value );
				totalRecords = requestList.Count();
			}
			if( paramDetails.userId.HasValue )
			{
				requestList = requestList.Where( s => s.DealerSalesRepID == paramDetails.userId.Value || s.PaoliSalesRepMemberID == paramDetails.userId.Value );
				totalRecords = requestList.Count();
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
						filteredAndSorted = requestList.OrderBy( v => v.Project.ProjectName );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.Project.ProjectName );
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
						filteredAndSorted = requestList.OrderBy( v => v.RequestID );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.RequestID );
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
							FileID = i.FileID,
							FileName = i.Name,
							Extension = i.Extension,
							UploadDate = i.UploadDate,
							VersionNumber = i.VersionNumber
						} )
					.ToList()
			};
		}

		public SpecRequestInformation NewSpecRequest( bool activeOnly )
		{
			return new SpecRequestInformation()
			{
				Casegoods = database.Serieses.Where( s => s.Category.Name == "Casegood" && (s.IsActive || !activeOnly) ).Select( s => s.Name ).ToList(),
				Seating = database.Serieses.Where( s => s.Category.Name == "Seating" && ( s.IsActive || !activeOnly ) ).Select( s => s.Name ).ToList(),
				Conferencing = database.Serieses.Where( s => s.Category.Name == "Tables" && ( s.IsActive || !activeOnly ) ).Select( s => s.Name ).ToList(),
				Finishes = new List<string>() { "Laminate", "Veneer", "Mixed Materials" }
			};
		}

		public ReOpenRequestInformation GetReOpenRequest( int requestId )
		{
			var sInfo = database.SpecRequests.FirstOrDefault( s => s.RequestID == requestId );
			if( sInfo == null )
			{
				throw new Exception( "Unable to find Spec Request" );
			}

			return new ReOpenRequestInformation() {
				RequestID = sInfo.RequestID,
				Name = sInfo.Name
			};
		}

		public bool ReOpenSpecRequest( ReOpenRequestInformation sInfo )
		{
			var dbInfo = database.SpecRequests
				.Include( s => s.SpecRequestEvents )
				.Include( s => s.SpecRequestEvents.Select( e => e.User ) )
				.Include( s => s.SpecRequestEvents.Select( e => e.User.Company ) )
				.Include( s => s.SpecRequestEvents.Select( e => e.User.Company.PaoliMember ) )
				.Include( s => s.PaoliSalesRepMember )
				.Include( s => s.PaoliSalesRepMember.Company )
				.Include( s => s.PaoliSalesRepMember.Company.PaoliMember )
				.Include( s => s.PaoliSalesRepGroup )
				.Include( s => s.PaoliSalesRepGroup.Users )
				.Include( s => s.PaoliSalesRepGroup.Users.Select( u => u.Company ) )
				.Include( s => s.PaoliSalesRepGroup.Users.Select( u => u.Company.PaoliMember ) )
				.Include( s => s.DealerSalesRep )
				.Include( s => s.DealerSalesRep.Company )
				.Include( s => s.PrimaryCompany )
				.Include( s => s.DealerSalesRep.Company.Territory )
				.Include( s => s.PrimaryCompany.Territory )
				.Include( s => s.PaoliSalesRepMember.Company.Territory )
				.Include( s => s.PaoliSalesRepGroup.Territory )
				.Include( s => s.SpecRequestFiles )
				.Include( s => s.Project )
				.FirstOrDefault( s => s.RequestID == sInfo.RequestID );
			if( dbInfo == null )
			{
				throw new Exception( "Unable to find Spec Request" );
			}

			dbInfo.IsCompleted = false;

			dbInfo.Notes += (((dbInfo.Notes ?? "").Any() ? "\n" : "") + "Re-Opened: " + sInfo.Notes);

			dbInfo.SpecRequestEvents.Add( SpecRequestEvent.ReOpenedEvent( PaoliWebUser.CurrentUser.UserId ) );

			var rootLocation = Path.Combine( ConfigurationManager.AppSettings["SpecRequestDocumentLocation"], dbInfo.Name );

			var files = new List<EmailSender.FileDetail>();
			if( sInfo.addlFiles != null )
			{
				foreach( var fileStream in sInfo.addlFiles )
				{
					if( fileStream != null )
					{
						files.Add( SaveNewFileVersion( dbInfo, rootLocation, Path.GetExtension( fileStream.FileName ).Trim( '.' ), fileStream.InputStream, fileStream.FileName, false ) );
					}
				}
			}

			if( database.SaveChanges() > 0 )
			{
				EmailForReopenedRequest( dbInfo, files, sInfo.Notes );

				return true;
			}

			return false;
		}

		private void EmailForReopenedRequest( SpecRequest dbInfo, IEnumerable<EmailSender.FileDetail> files, string notes )
		{
			var summary = new RequestChangeSpecRequestEmailSender.RequestChangeSpecRequestSummary();
			var emailTargets = new List<EmailSender.EmailTarget>();

			summary.requestId = dbInfo.RequestID;
			summary.requestName = dbInfo.Name;
			summary.reOpenedByName = PaoliWebUser.CurrentUser.FullName;
			summary.projectName = dbInfo.ProjectID.HasValue ? dbInfo.Project.ProjectName : "";
			summary.newNotes = notes;
			summary.newFiles.AddRange( files );

			if( dbInfo.PaoliSpecTeamMemberID.HasValue )
			{
				summary.specTeamMember = dbInfo.SpecTeamMember.FullName;
				summary.recipients.Add( dbInfo.SpecTeamMember.FullName );
				if( (dbInfo.SpecTeamMember.Enabled || EmailSender.EmailDisabledUsers) && 
					dbInfo.SpecTeamMember.UserNotification.ReOpenSpecRequest )
				{
					// spec team member
					emailTargets.Add( new EmailSender.EmailTarget()
					{
						EmailAddress = dbInfo.SpecTeamMember.Email,
						FirstName = dbInfo.SpecTeamMember.FirstName,
						TemplateName = "ChangeSpecRequestSpecTeam",
						FromDetails = GetPaoliMemberFromDetails( dbInfo.SpecTeamMember.Company )
					} );
				}
			}

			if( dbInfo.PrimaryCompanyID.HasValue )
			{
				summary.dealership = dbInfo.PrimaryCompany.FullName;
				if( dbInfo.PrimaryCompany.PaoliMemberID.HasValue )
				{
					summary.recipients.Add( dbInfo.PrimaryCompany.PaoliMember.FullName );
					if( (dbInfo.PrimaryCompany.PaoliMember.Enabled || EmailSender.EmailDisabledUsers) &&
						dbInfo.PrimaryCompany.PaoliMember.UserNotification.ReOpenSpecRequest )
					{
						// customer service for dealership
						emailTargets.Add( new EmailSender.EmailTarget()
						{
							EmailAddress = dbInfo.PrimaryCompany.PaoliMember.Email,
							FirstName = dbInfo.PrimaryCompany.PaoliMember.FirstName,
							TemplateName = "ChangeSpecRequestService",
							FromDetails = GetPaoliMemberFromDetails( dbInfo.PrimaryCompany.PaoliMember.Company )
						} );
					}
				}
			}

			if( dbInfo.PaoliSalesRepMemberID.HasValue )
			{
				summary.recipients.Add( dbInfo.PaoliSalesRepMember.FullName );
				if( (dbInfo.PaoliSalesRepMember.Enabled || EmailSender.EmailDisabledUsers) &&
					dbInfo.PaoliSalesRepMember.UserNotification.ReOpenSpecRequest )
				{
					// sales rep member
					emailTargets.Add( new EmailSender.EmailTarget()
					{
						EmailAddress = dbInfo.PaoliSalesRepMember.Email,
						FirstName = dbInfo.PaoliSalesRepMember.FirstName,
						TemplateName = "ChangeSpecRequestService",
						FromDetails = GetPaoliMemberFromDetails( dbInfo.PaoliSalesRepMember.Company )
					} );
				}
			}

			if( dbInfo.DealerSalesRepID.HasValue )
			{
				summary.dealerPOCName = dbInfo.DealerSalesRep.FullName;
				summary.recipients.Add( summary.dealerPOCName );
				if( ( dbInfo.DealerSalesRep.Enabled || EmailSender.EmailDisabledUsers ) &&
					dbInfo.DealerSalesRep.UserNotification.ReOpenSpecRequest )
				{
					// dealer poc
					emailTargets.Add( new EmailSender.EmailTarget()
					{
						EmailAddress = dbInfo.DealerSalesRep.Email,
						FirstName = dbInfo.DealerSalesRep.FirstName,
						TemplateName = "ChangeSpecRequestDealer",
						FromDetails = GetPaoliMemberFromDetails( dbInfo.DealerSalesRep.Company )
					} );
				}
			}
			else if( ( dbInfo.DealerPOCEmail ?? "" ).Any() )
			{
				summary.dealerPOCName = dbInfo.DealerPOCText;
				summary.recipients.Add( summary.dealerPOCName );
				// dealer poc
				emailTargets.Add( new EmailSender.EmailTarget()
				{
					EmailAddress = dbInfo.DealerPOCEmail,
					FirstName = dbInfo.DealerPOCText,
					TemplateName = "ChangeSpecRequestDealer",
					FromDetails = GetPaoliMemberFromDetails( dbInfo.PrimaryCompany )
				} );
			}

			foreach( var target in emailTargets )
			{
				summary.firstName = target.FirstName;

				( new RequestChangeSpecRequestEmailSender( target.TemplateName ) ).SubmitNewRequestEmail( target.EmailAddress, summary, target.FromDetails );
			}
		}

		public SpecRequestInformation GetSpecRequest( int requestId )
		{
			var sInfo = database.SpecRequests.FirstOrDefault( s => s.RequestID == requestId );
			if( sInfo == null )
			{
				throw new Exception( "Unable to find Spec Request" );
			}

			return ToSpecRequestInformation( sInfo );
		}

		private SpecRequestInformation ToSpecRequestInformation( SpecRequest sInfo )
		{
			return new SpecRequestInformation()
			{
				RequestID = sInfo.RequestID,
				Name = sInfo.Name,
				ProjectID = sInfo.ProjectID,
				RealProjectName = sInfo.ProjectID.HasValue ? sInfo.Project.ProjectName : "",
				RealEndCustomer = sInfo.ProjectID.HasValue ? (sInfo.Project.EndCustomerID.HasValue ? sInfo.Project.EndCustomerCompany.Name : "") : "",
				RealIsGSA = sInfo.ProjectID.HasValue ? (sInfo.Project.IsGSA ?? false) : false,
				RealContractName = sInfo.ProjectID.HasValue ? (sInfo.Project.ContractID.HasValue ? sInfo.Project.GSAContract.Name : null) : null,
				PaoliSalesRepGroupID = sInfo.PaoliSalesRepGroupID,
				PaoliSalesRepMemberID = sInfo.PaoliSalesRepMemberID,
				DealerID = sInfo.PrimaryCompanyID,
				DealerMemberID = sInfo.DealerSalesRepID,
				DealerPointOfContact = sInfo.DealerPOCText,
				DealerPointOfContactEmail = sInfo.DealerPOCEmail,
				DealerPointOfContactPhone = sInfo.DealerPOCPhone,
				DealerPointOfContactAcctType = sInfo.DealerPOCAcctType,
				DealerPointOfContactAcctTypeName = sInfo.DealerPOCAcctType.HasValue ? PaoliWebUser.PaoliWebRole.RoleList[sInfo.DealerPOCAcctType.Value] : "",
				SavedLocation = sInfo.SavedLocation,
				ListPrice = sInfo.ListPrice,
				Received = sInfo.Received ?? false,
				SPLQuote = sInfo.SPLQuote,
				PaoliSpecTeamMember = sInfo.PaoliSpecTeamMemberID ?? 0,
				IsGoodForWeb = ( sInfo.IsGoodForWeb ?? false ) && sInfo.IsCompleted,
				SendCompleteEmail = true,
				AvailableForIn2 = sInfo.AvailableForIn2 ?? false,
				IsCompleted = sInfo.IsCompleted,
				IsOnHold = sInfo.IsOnHold,
				IsCanceled = sInfo.IsCanceled,
				Footprint = sInfo.Footprint,
				Notes = sInfo.Notes,
				NeedDWGFiles = sInfo.NeedDWGFiles,
				NeedSP4Files = sInfo.NeedSP4Files,
				NeedSIFFiles = sInfo.NeedSIFFiles,
				NeedXLSFiles = sInfo.NeedXLSFiles,
				NeedPDFFiles = sInfo.NeedPDFFiles,
				addlFileList = GetFileListing( sInfo.Name, sInfo.SpecRequestFiles.Where( f => !f.IsSpecTeam ) ),
				specTeamFileList = GetFileListing( sInfo.Name, sInfo.SpecRequestFiles.Where( f => f.IsSpecTeam ) ),
				CreatedDate = sInfo.CreatedOnDate,
				DealerName = sInfo.PrimaryCompanyID.HasValue ? sInfo.PrimaryCompany.FullName : "None",
				SalesRepGroupName = sInfo.PaoliSalesRepGroupID.HasValue ? sInfo.PaoliSalesRepGroup.FullName : "None",
				SalesRepMemberName = sInfo.PaoliSalesRepMemberID.HasValue ? sInfo.PaoliSalesRepMember.FullName : "None",
				SalesRepMemberContact = sInfo.PaoliSalesRepMemberID.HasValue ? sInfo.PaoliSalesRepMember.ContactInfo : "",
				DealerSalesRepName = sInfo.DealerSalesRepID.HasValue ? sInfo.DealerSalesRep.FullName : "None",
				DealerSalesRepContact = sInfo.DealerSalesRepID.HasValue ? sInfo.DealerSalesRep.ContactInfo : "",
				SpecTeamMemberName = sInfo.PaoliSpecTeamMemberID.HasValue ? sInfo.SpecTeamMember.FullName : "Not Assigned",
				EndCustomerID = sInfo.ProjectID.HasValue ? sInfo.Project.EndCustomerID : null,
				ProjectSize = sInfo.ProjectSize,
				NeedFloorplanSpecs = sInfo.NeedFloorplanSpecs,
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
				GrommetPosition = sInfo.Grommets ? sInfo.GrommetPosition : null,
				DrawerOption = sInfo.DrawerOption,
				FabricGrade = sInfo.FabricGrade,
				Fabric = sInfo.FabricDetails,
				SpecialRequests = sInfo.SpecialRequests,
				SpecTeamNotes = sInfo.SpecTeamNotes,
				PreviousSpecTeamNotes = sInfo.SpecTeamNotes,
				InternalNotes = sInfo.InternalNotes,

				CreatedByUser = sInfo.CreatedByUserName,
				CreatedByUserCompany = sInfo.CreatedByCompany,
				CreatedByUserPhone = sInfo.CreatedByUserPhone,
				CreatedByUserEmail = sInfo.CreatedByUserEmail,
				CanceledDate = sInfo.CanceledOnDate,
				CanceledByUser = sInfo.CanceledByUserName,
				ReOpenedDate = sInfo.ReOpenedOnDate,
				ReOpenedByUser = sInfo.ReOpenedByUserName,
				ReOpenedByUserCompany = sInfo.ReOpenedByCompany,
				ReOpenedByUserPhone = sInfo.ReOpenedByUserPhone,
				ReOpenedByUserEmail = sInfo.ReOpenedByUserEmail,
	
			};
		}

		public bool AddSpecRequest( SpecRequestInformation sInfo )
		{
			SpecRequest newSpec = new SpecRequest();

			newSpec.ProjectID = sInfo.ProjectID;
			newSpec.PaoliSalesRepGroupID = ( sInfo.PaoliSalesRepGroupID ?? 0 ) > 0 ? sInfo.PaoliSalesRepGroupID : null;
			newSpec.PaoliSalesRepMemberID = ( sInfo.PaoliSalesRepMemberID ?? 0 ) > 0 ? sInfo.PaoliSalesRepMemberID : null;
			newSpec.PrimaryCompanyID = ( sInfo.DealerID ?? 0 ) > 0 ? sInfo.DealerID : null;
			newSpec.DealerSalesRepID = ( sInfo.DealerMemberID ?? 0 ) > 0 ? sInfo.DealerMemberID : null;
			newSpec.DealerPOCText = !newSpec.DealerSalesRepID.HasValue ? sInfo.DealerPointOfContact : null;
			newSpec.DealerPOCEmail = !newSpec.DealerSalesRepID.HasValue ? sInfo.DealerPointOfContactEmail : null;
			newSpec.DealerPOCPhone = !newSpec.DealerSalesRepID.HasValue ? sInfo.DealerPointOfContactPhone : null;
			newSpec.DealerPOCAcctType = !newSpec.DealerSalesRepID.HasValue ? sInfo.DealerPointOfContactAcctType : null;
			newSpec.AvailableForIn2 = sInfo.AvailableForIn2;
			newSpec.PaoliSpecTeamMemberID = ( sInfo.PaoliSpecTeamMember ?? 0 ) > 0 ? sInfo.PaoliSpecTeamMember : null;
			newSpec.LastModifiedDate = DateTime.UtcNow;
			newSpec.Notes = sInfo.Notes;
			newSpec.NeedDWGFiles = sInfo.NeedDWGFiles;
			newSpec.NeedSP4Files = sInfo.NeedSP4Files;
			newSpec.NeedSIFFiles = sInfo.NeedSIFFiles;
			newSpec.NeedXLSFiles = sInfo.NeedXLSFiles;
			newSpec.NeedPDFFiles = sInfo.NeedPDFFiles;
			newSpec.ProjectSize = sInfo.ProjectSize;
			newSpec.NeedFloorplanSpecs = sInfo.NeedFloorplanSpecs;
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
			newSpec.GrommetPosition = sInfo.Grommets ? sInfo.GrommetPosition : null;
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
			newSpec.IsCanceled = false;
			newSpec.Footprint = null;
			newSpec.SpecTeamNotes = null;

			newSpec.SpecRequestEvents.Add( SpecRequestEvent.CreatedEvent( PaoliWebUser.CurrentUser.UserId ) );

			database.SpecRequests.Add( newSpec );

			if( database.SaveChanges() > 0 )
			{
				database.Entry( newSpec ).Reload();

				newSpec.Name = string.Format( "T{0}", newSpec.RequestID.ToString( "D4" ) );

				var rootLocation = Path.Combine( ConfigurationManager.AppSettings["SpecRequestDocumentLocation"], newSpec.Name );

				if( sInfo.addlFiles != null )
				{
					foreach( var fileStream in sInfo.addlFiles )
					{
						if( fileStream != null )
						{
							SaveNewFileVersion( newSpec, rootLocation, Path.GetExtension( fileStream.FileName ).Trim( '.' ), fileStream.InputStream, fileStream.FileName, false );
						}
					}
				}

				if( database.SaveChanges() > 0 )
				{
					var reloadRequest = database.SpecRequests
						.Include( s => s.SpecRequestEvents )
						.Include( s => s.SpecRequestEvents.Select( e => e.User ) )
						.Include( s => s.SpecRequestEvents.Select( e => e.User.Company ) )
						.Include( s => s.SpecRequestEvents.Select( e => e.User.Company.PaoliMember ) )
						.Include( s => s.PaoliSalesRepMember )
						.Include( s => s.PaoliSalesRepMember.Company )
						.Include( s => s.PaoliSalesRepMember.UserNotification )
						.Include( s => s.PaoliSalesRepMember.Company.PaoliMember )
						.Include( s => s.PaoliSalesRepGroup )
						.Include( s => s.PaoliSalesRepGroup.Users )
						.Include( s => s.PaoliSalesRepGroup.Users.Select( u => u.Company ) )
						.Include( s => s.PaoliSalesRepGroup.Users.Select( u => u.UserNotification ) )
						.Include( s => s.PaoliSalesRepGroup.Users.Select( u => u.Company.PaoliMember ) )
						.Include( s => s.DealerSalesRep )
						.Include( s => s.DealerSalesRep.Company )
						.Include( s => s.PrimaryCompany )
						.Include( s => s.DealerSalesRep.Company.Territory )
						.Include( s => s.PrimaryCompany.Territory )
						.Include( s => s.PaoliSalesRepMember.Company.Territory )
						.Include( s => s.PaoliSalesRepGroup.Territory )
						.Include( s => s.Project )
						.Include( s => s.Project.EndCustomerCompany )
						.FirstOrDefault( s => s.RequestID == newSpec.RequestID );

					//( new NewSpecRequestEmailSender( "NewSpecRequestSpecTeam" ) ).SubmitNewRequestEmail( "PAOProjectSpecTeam@paoli.com", ToEmailSpecRequestSummary( newSpec, new EmailSender.EmailTarget() ) );

					var cUserNotifications = database.UserNotifications.FirstOrDefault( n => n.UserID == reloadRequest.CreatedByUserID );
					if( cUserNotifications != null )
					{
						if( cUserNotifications.NewSpecRequest )
						{
							( new NewSpecRequestEmailSender( "NewSpecRequest" ) ).SubmitNewRequestEmail( reloadRequest.CreatedByUserEmail,
								ToEmailSpecRequestSummary( reloadRequest, new EmailSender.EmailTarget()
								{
									EmailAddress = reloadRequest.CreatedByUserEmail,
									FirstName = reloadRequest.CreatedByUserFirstName
								} ),
								GetPaoliMemberFromDetails( reloadRequest.CreatedByCompanyDetails ) );
						}
					}

					if( reloadRequest.PaoliSalesRepMemberID.HasValue )
					{
						if( reloadRequest.PaoliSalesRepMemberID.Value != reloadRequest.CreatedByUserID.Value && 
							( reloadRequest.PaoliSalesRepMember.Enabled || EmailSender.EmailDisabledUsers ) &&
							reloadRequest.PaoliSalesRepMember.UserNotification.NewSpecRequest )
						{
							( new NewSpecRequestEmailSender( "NewSpecRequestSalesRep" ) ).SubmitNewRequestEmail( reloadRequest.PaoliSalesRepMember.Email,
								ToEmailSpecRequestSummary( reloadRequest, new EmailSender.EmailTarget() { EmailAddress = reloadRequest.PaoliSalesRepMember.Email, FirstName = reloadRequest.PaoliSalesRepMember.FirstName } ),
								GetPaoliMemberFromDetails( reloadRequest.PaoliSalesRepMember.Company ) );
						}
					}
					else if( reloadRequest.PaoliSalesRepGroupID.HasValue )
					{
						foreach( var salesRepUser in reloadRequest.PaoliSalesRepGroup.Users )
						{
							if( salesRepUser.UserID != reloadRequest.CreatedByUserID.Value && 
								( salesRepUser.Enabled || EmailSender.EmailDisabledUsers ) &&
								( salesRepUser.UserNotification.NewSpecRequestTerritory ||
									( salesRepUser.UserNotification.NewSpecRequestMyDealers && reloadRequest.PrimaryCompany != null && salesRepUser.UserID == reloadRequest.PrimaryCompany.PaoliSalesRepMemberID ) ) )
							{
								( new NewSpecRequestEmailSender( "NewSpecRequestSalesRep" ) ).SubmitNewRequestEmail( salesRepUser.Email,
									ToEmailSpecRequestSummary( reloadRequest, new EmailSender.EmailTarget() { EmailAddress = salesRepUser.Email, FirstName = salesRepUser.FirstName } ),
									GetPaoliMemberFromDetails( salesRepUser.Company ) );
							}
						}
					}

					if( reloadRequest.ProjectID.HasValue )
					{
						if( reloadRequest.Project.IsGSA ?? false )
						{
							( new NewSpecRequestEmailSender( "NewSpecRequestGSA" ) ).SubmitNewRequestEmail( "gsa@paoli.com",
								ToEmailSpecRequestSummary( reloadRequest, new EmailSender.EmailTarget() { EmailAddress = "gsa@paoli.com", FirstName = "" } ),
								GetPaoliMemberFromDetails( null ) );
						}
					}

					return true;
				}
			}

			throw new Exception( "Unable to save Spec Request" );
		}

		private NewSpecRequestEmailSender.EmailSpecRequestSummary ToEmailSpecRequestSummary( SpecRequest request, EmailSender.EmailTarget target )
		{
			var summary = new NewSpecRequestEmailSender.EmailSpecRequestSummary()
			{
				requestId = request.RequestID,
				requestName = request.Name,
				firstName = target.FirstName,
				companyName = request.ProjectID.HasValue ? (request.Project.EndCustomerID.HasValue ? request.Project.EndCustomerCompany.Name : "") : "",
				projectName = request.ProjectID.HasValue ? request.Project.ProjectName : "",
				scopeDescription = request.ProjectSize,
				createdBy = request.CreatedByUserName
			};

			Territory dsrTerritory = null;
			if( request.DealerSalesRepID.HasValue )
			{
				summary.placingNameAndCompany = string.Format( "{0} at {1}", request.DealerSalesRep.FullName, request.DealerSalesRep.Company.Name );
				dsrTerritory = request.DealerSalesRep.Company.Territory;
			}
			else if( (request.DealerPOCText ?? "").Any() )
			{
				summary.placingNameAndCompany = string.Format( "{0} at {1}", request.DealerPOCText, request.PrimaryCompany.Name );
				dsrTerritory = request.PrimaryCompany.Territory;
			}
			else if( request.PrimaryCompanyID.HasValue )
			{
				summary.placingNameAndCompany = string.Format( "{0}", request.PrimaryCompany.Name );
				dsrTerritory = request.PrimaryCompany.Territory;
			}
			else if( request.PaoliSalesRepMemberID.HasValue )
			{
				summary.placingNameAndCompany = string.Format( "{0} at {1}", request.PaoliSalesRepMember.FullName, request.PaoliSalesRepMember.Company.Name );
				dsrTerritory = request.PaoliSalesRepMember.Company.Territory;
			}
			else if( request.PaoliSalesRepGroupID.HasValue )
			{
				summary.placingNameAndCompany = string.Format( "{0}", request.PaoliSalesRepGroup.Name );
				dsrTerritory = request.PaoliSalesRepGroup.Territory;
			}

			if( request.DealerSalesRepID.HasValue )
			{
				summary.dealerPOCName = request.DealerSalesRep.FullName;
			}
			else if( ( request.DealerPOCText ?? "" ).Any() )
			{
				summary.dealerPOCName = request.DealerPOCText;
			}

			if( request.PaoliSalesRepMemberID.HasValue )
			{
				summary.salesRepName = request.PaoliSalesRepMember.FullName;
			}

			if( dsrTerritory != null )
			{
				summary.territoryName = dsrTerritory.TerritoryID.ToString() + " - " + dsrTerritory.SalesRepCompanyName;
			}

			var fullSeriesList = string.Join( ",", request.Casegoods, request.Conferencing, request.Seating );
			summary.seriesNames = new List<string>();
			if( (fullSeriesList ?? "").Any() )
			{
				summary.seriesNames = fullSeriesList.Split( ',' ).Where( s => (s ?? "").Any() ).ToList();
				summary.seriesNames.Sort();
			}

			summary.servicesReqd = new List<string>();
			if( request.NeedAuditSpecs )
			{
				summary.servicesReqd.Add( "Spec Check Audit" );
			}			
			if( request.NeedFloorplanSpecs )
			{
				summary.servicesReqd.Add( "Floorplan Specifications" );
			}
			if( request.NeedPhotoRendering )
			{
				summary.servicesReqd.Add( "Photo Rendering" );
			}
			if( request.Need2DDrawing )
			{
				summary.servicesReqd.Add( "2D Drawing" );
			}
			if( request.NeedValueEng )
			{
				summary.servicesReqd.Add( "Value Engineering" );
			}

			return summary;
		}

		public bool UpdateSpecRequest( SpecRequestInformation sInfo )
		{
			var specInfo = database.SpecRequests
				.Include( s => s.SpecRequestFiles )
				.FirstOrDefault( s => s.RequestID == sInfo.RequestID );
			if( specInfo == null )
			{
				throw new Exception( "Unable to find Spec Request" );
			}

			specInfo.ProjectID = sInfo.ProjectID;
			specInfo.PaoliSalesRepGroupID = ( sInfo.PaoliSalesRepGroupID ?? 0 ) > 0 ? sInfo.PaoliSalesRepGroupID : null;
			specInfo.PaoliSalesRepMemberID = ( sInfo.PaoliSalesRepMemberID ?? 0 ) > 0 ? sInfo.PaoliSalesRepMemberID : null;
			specInfo.PrimaryCompanyID = ( sInfo.DealerID ?? 0 ) > 0 ? sInfo.DealerID : null;
			specInfo.DealerSalesRepID = ( sInfo.DealerMemberID ?? 0 ) > 0 ? sInfo.DealerMemberID : null;
			specInfo.DealerPOCText = !specInfo.DealerSalesRepID.HasValue ? sInfo.DealerPointOfContact : null;
			specInfo.DealerPOCEmail = !specInfo.DealerSalesRepID.HasValue ? sInfo.DealerPointOfContactEmail : null;
			specInfo.DealerPOCPhone = !specInfo.DealerSalesRepID.HasValue ? sInfo.DealerPointOfContactPhone : null;
			specInfo.DealerPOCAcctType = !specInfo.DealerSalesRepID.HasValue ? sInfo.DealerPointOfContactAcctType : null;
			specInfo.AvailableForIn2 = sInfo.AvailableForIn2;
			specInfo.PaoliSpecTeamMemberID = ( sInfo.PaoliSpecTeamMember ?? 0 ) > 0 ? sInfo.PaoliSpecTeamMember : null;
			specInfo.LastModifiedDate = DateTime.UtcNow;
			specInfo.Notes = sInfo.Notes;
			specInfo.NeedDWGFiles = sInfo.NeedDWGFiles;
			specInfo.NeedSP4Files = sInfo.NeedSP4Files;
			specInfo.NeedSIFFiles = sInfo.NeedSIFFiles;
			specInfo.NeedXLSFiles = sInfo.NeedXLSFiles;
			specInfo.NeedPDFFiles = sInfo.NeedPDFFiles;
			specInfo.ProjectSize = sInfo.ProjectSize;
			specInfo.NeedFloorplanSpecs = sInfo.NeedFloorplanSpecs;
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
			specInfo.GrommetPosition = sInfo.Grommets ? sInfo.GrommetPosition : null;
			specInfo.DrawerOption = sInfo.Casegoods != null && sInfo.Casegoods.Any() ? sInfo.DrawerOption : null;
			specInfo.FabricGrade = sInfo.Seating != null && sInfo.Seating.Any() ? sInfo.FabricGrade : null;
			specInfo.FabricDetails = sInfo.Seating != null && sInfo.Seating.Any() ? sInfo.Fabric : null;
			specInfo.SpecialRequests = sInfo.SpecialRequests;
			specInfo.SavedLocation = sInfo.SavedLocation;
			specInfo.ListPrice = sInfo.ListPrice;
			specInfo.Received = sInfo.Received;
			specInfo.SPLQuote = sInfo.SPLQuote;
			specInfo.IsGoodForWeb = sInfo.IsCompleted && sInfo.IsGoodForWeb;
			bool bDoCompleteEmail = sInfo.IsCompleted && !specInfo.IsCompleted && sInfo.SendCompleteEmail;
			bool bDoInProgressEmail = !sInfo.IsCompleted && !specInfo.IsCompleted && sInfo.SendInProgressEmail;
			if( sInfo.IsCompleted && !specInfo.IsCompleted )
			{
				specInfo.SpecRequestEvents.Add( SpecRequestEvent.CompletedEvent( PaoliWebUser.CurrentUser.UserId ) );
			}
			specInfo.IsCompleted = sInfo.IsCompleted;
			specInfo.IsOnHold = sInfo.IsOnHold && !sInfo.IsCompleted;
			specInfo.IsCanceled = false;
			specInfo.Footprint = sInfo.Footprint;
			if( ( sInfo.SpecTeamNotes ?? "" ).Any() )
			{
				specInfo.SpecTeamNotes += ( ( ( specInfo.SpecTeamNotes ?? "" ).Any() ? "\n" : "" ) + string.Format( "Notes added by {0}: {1}", PaoliWebUser.CurrentUser.FullName, sInfo.SpecTeamNotes ) );
			}
			specInfo.InternalNotes = sInfo.InternalNotes;

			var rootLocation = Path.Combine( ConfigurationManager.AppSettings["SpecRequestDocumentLocation"], specInfo.Name );
			var nowDate = DateTime.UtcNow;

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

			if( ( sInfo.deletespecTeamFileList ?? "" ).Any() )
			{
				foreach( var sFileId in sInfo.deletespecTeamFileList.Split( ',' ).Where( s => s.Any() ) )
				{
					var fileId = 0;
					if( int.TryParse( sFileId, out fileId ) )
					{
						var deleteFile = specInfo.SpecRequestFiles.FirstOrDefault( f => f.FileID == fileId );
						if( deleteFile != null )
						{
							//DeleteRequestFile( deleteFile, rootLocation );

							database.SpecRequestFiles.Remove( deleteFile );
						}
					}
				}
			}

			if( database.SaveChanges() > 0 )
			{
				if( bDoCompleteEmail || bDoInProgressEmail )
				{
					var reloadRequest = database.SpecRequests
						.Include( s => s.SpecRequestEvents )
						.Include( s => s.SpecRequestEvents.Select( e => e.User ) )
						.Include( s => s.SpecRequestEvents.Select( e => e.User.Company ) )
						.Include( s => s.SpecRequestEvents.Select( e => e.User.Company.PaoliMember ) )
						.Include( s => s.PaoliSalesRepMember )
						.Include( s => s.PaoliSalesRepMember.Company )
						.Include( s => s.PaoliSalesRepMember.UserNotification )
						.Include( s => s.PaoliSalesRepMember.Company.PaoliMember )
						.Include( s => s.PaoliSalesRepGroup )
						.Include( s => s.PaoliSalesRepGroup.Users )
						.Include( s => s.PaoliSalesRepGroup.Users.Select( u => u.Company ) )
						.Include( s => s.PaoliSalesRepGroup.Users.Select( u => u.UserNotification ) )
						.Include( s => s.PaoliSalesRepGroup.Users.Select( u => u.Company.PaoliMember ) )
						.Include( s => s.DealerSalesRep )
						.Include( s => s.DealerSalesRep.Company )
						.Include( s => s.DealerSalesRep.UserNotification )
						.Include( s => s.PrimaryCompany )
						.Include( s => s.DealerSalesRep.Company.Territory )
						.Include( s => s.PrimaryCompany.Territory )
						.Include( s => s.PaoliSalesRepMember.Company.Territory )
						.Include( s => s.PaoliSalesRepGroup.Territory )
						.Include( s => s.Project )
						.FirstOrDefault( s => s.RequestID == specInfo.RequestID );

					List<EmailSender.EmailTarget> emailAddresses = new List<EmailSender.EmailTarget>();
					if( reloadRequest.CreatedByUserEmail != null )
					{
						var cUserNotifications = database.UserNotifications.FirstOrDefault( n => n.UserID == reloadRequest.CreatedByUserID );
						if( cUserNotifications != null )
						{
							if( cUserNotifications.PermissionByName( bDoCompleteEmail ? "CompleteSpecRequest" : "UpdateSpecRequest" ) )
							{
								emailAddresses.Add( new EmailSender.EmailTarget()
								{
									EmailAddress = reloadRequest.CreatedByUserEmail,
									FirstName = reloadRequest.CreatedByUserFirstName,
									FromDetails = GetPaoliMemberFromDetails( reloadRequest.CreatedByCompanyDetails )
								} );
							}
						}
					}
					if( reloadRequest.DealerSalesRep != null )
					{
						if( reloadRequest.DealerSalesRepID != reloadRequest.CreatedByUserID && 
							( reloadRequest.DealerSalesRep.Enabled || EmailSender.EmailDisabledUsers ) &&
							reloadRequest.DealerSalesRep.UserNotification.PermissionByName( bDoCompleteEmail ? "CompleteSpecRequest" : "UpdateSpecRequest" ) )
						{
							emailAddresses.Add( new EmailSender.EmailTarget()
							{
								EmailAddress = reloadRequest.DealerSalesRep.Email,
								FirstName = reloadRequest.DealerSalesRep.FirstName,
								FromDetails = GetPaoliMemberFromDetails( reloadRequest.DealerSalesRep.Company )
							} );
						}
					}
					if( reloadRequest.PaoliSalesRepMember != null )
					{
						if( reloadRequest.PaoliSalesRepMemberID != reloadRequest.CreatedByUserID &&
							( reloadRequest.PaoliSalesRepMember.Enabled || EmailSender.EmailDisabledUsers ) &&
							reloadRequest.PaoliSalesRepMember.UserNotification.PermissionByName( bDoCompleteEmail ? "CompleteSpecRequest" : "UpdateSpecRequest" ) )
						{
							emailAddresses.Add( new EmailSender.EmailTarget()
							{
								EmailAddress = reloadRequest.PaoliSalesRepMember.Email,
								FirstName = reloadRequest.PaoliSalesRepMember.FirstName,
								FromDetails = GetPaoliMemberFromDetails( reloadRequest.PaoliSalesRepMember.Company )
							} );
						}
					}
					else if( reloadRequest.PaoliSalesRepGroup != null )
					{
						emailAddresses.AddRange( reloadRequest.PaoliSalesRepGroup.Users
							.Where( u => u.UserID != reloadRequest.CreatedByUserID &&
								( u.Enabled || EmailSender.EmailDisabledUsers ) &&
								( u.UserNotification.PermissionByName( bDoCompleteEmail ? "CompleteSpecRequestTerritory" : "UpdateSpecRequestTerritory" ) ||
									( u.UserNotification.PermissionByName( bDoCompleteEmail ? "CompleteSpecRequestMyDealers" : "UpdateSpecRequestMyDealers" ) && reloadRequest.PrimaryCompany != null && reloadRequest.PrimaryCompany.PaoliSalesRepMemberID == u.UserID ) ) )
							.Select( u => new EmailSender.EmailTarget()
							{
								EmailAddress = u.Email,
								FirstName = u.FirstName,
								FromDetails = GetPaoliMemberFromDetails( reloadRequest.PaoliSalesRepGroup )
							} ) );
					}

					if( bDoCompleteEmail )
					{
						foreach( var emailTarget in emailAddresses )
						{
							( new CompletedSpecRequestEmailSender() ).SubmitCompletedRequestEmail( emailTarget.EmailAddress,
								ToEmailCompleteSpecRequestSummary( reloadRequest, emailTarget, sInfo.SpecTeamNotes ),
								emailTarget.FromDetails );
						}
					}
					else if( bDoInProgressEmail )
					{
						foreach( var emailTarget in emailAddresses )
						{
							( new CompletedSpecRequestEmailSender() ).SubmitInProgressRequestEmail( emailTarget.EmailAddress,
								ToEmailCompleteSpecRequestSummary( reloadRequest, emailTarget, sInfo.SpecTeamNotes, nowDate ),
								emailTarget.FromDetails );
						}
					}
				}

				return true;
			}
			return false;
		}

		private CompletedSpecRequestEmailSender.EmailCompleteSpecRequestSummary ToEmailCompleteSpecRequestSummary( 
			SpecRequest request, EmailSender.EmailTarget target, string finalNotes, DateTime? updateDate = null )
		{
			var summary = new CompletedSpecRequestEmailSender.EmailCompleteSpecRequestSummary()
			{
				requestId = request.RequestID,
				requestName = request.Name,
				firstName = target.FirstName,
				projectName = request.ProjectID.HasValue ? request.Project.ProjectName : "",
				specTeamMember =
					( request.CompletedByUserName != null ) ? request.CompletedByUserName :
						((request.SpecTeamMember != null) ? request.SpecTeamMember.FullName : "a member of our team"),
				specTeamNotes = finalNotes
			};

			DateTime? recentFileDate = null;
			if( !updateDate.HasValue )
			{
				recentFileDate = request.ReOpenedOnDate;
			}
			else
			{
				recentFileDate = updateDate.Value;
			}

			summary.fullFileList = request.SpecRequestFiles
				.Where( f => f.IsSpecTeam )
				.Where( f => f.UploadDate >= recentFileDate || recentFileDate == null )
				.Select( srf => new EmailSender.FileDetail()
				{
					fileName = srf.Name,
					filePath = request.Name + "/" + ( srf.Extension.Any() ? ( srf.Extension + "/" ) : "" ) + srf.VersionNumber.ToString()
				} )
				.ToList();

			summary.oldFileList = request.SpecRequestFiles
				.Where( f => f.IsSpecTeam )
				.Where( f => f.UploadDate < recentFileDate && recentFileDate != null )
				.Select( srf => new EmailSender.FileDetail()
				{
					fileName = srf.Name,
					filePath = request.Name + "/" + ( srf.Extension.Any() ? ( srf.Extension + "/" ) : "" ) + srf.VersionNumber.ToString()
				} )
				.ToList();

			return summary;
		}

		private bool DeleteRequestFile( SpecRequestFile sFile, string rootLocation )
		{
			var fileLocation = Path.Combine( rootLocation, sFile.Extension, sFile.VersionNumber.ToString(), sFile.Name );

			if( File.Exists( fileLocation ) )
			{
				File.Delete( fileLocation );
			}

			return true;
		}

		private EmailSender.FileDetail SaveNewFileVersion( SpecRequest sRequest, string rootLocation, string fileType, Stream inputStream, string fileName, bool isSpecTeam )
		{
			var existingCount = sRequest.SpecRequestFiles.Count( f => f.Extension == fileType );
			existingCount++;

			var fileLocation = Path.Combine( rootLocation, fileType, existingCount.ToString() );

			if( !Directory.Exists( fileLocation ) )
			{
				Directory.CreateDirectory( fileLocation );
			}

			fileName = fileName.Replace( "&", "and" );
			fileName = Path.GetFileName( fileName );

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

			return new EmailSender.FileDetail()
			{
				fileName = newFile.Name,
				filePath = sRequest.Name + "/" + ( newFile.Extension.Any() ? ( newFile.Extension + "/" ) : "" ) + newFile.VersionNumber.ToString()
			};
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
				IsPublished = false
			};
		}

		public bool AddTypical( TypicalMgmtInfo tInfo, bool publish )
		{
			var specRequest = database.SpecRequests.FirstOrDefault( r => r.RequestID == tInfo.RequestID );
			if( specRequest == null )
			{
				throw new Exception( "Unable to find Spec Request" );
			}

			Typical tData = new Typical();
			tData.CreatedDate = DateTime.Now;

			tData.Name = specRequest.Name;
			tData.IsPublished = publish;

			List<string> arrKeywordList = new List<string>();
			var rSeries = database.Serieses.FirstOrDefault( s => s.Name == tInfo.FeaturedSeries );
			if( rSeries != null )
			{
				SeriesTypical stData = new SeriesTypical();
				stData.IsPrimary = true;
				stData.Series = rSeries;
				stData.Typical = tData;
				database.SeriesTypicals.Add( stData );
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
						database.SeriesTypicals.Add( stData );
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
				database.TypicalImageFiles.Add( sif );
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
						database.TypicalImageFiles.Add( sif );
					}
				}
			}

			{
				var attData = database.TAttributes.FirstOrDefault( a => a.Name == "pricing" );
				if( attData == null )
				{
					attData = new PDWDBContext.TAttribute();
					attData.Name = "pricing";
					database.TAttributes.Add( attData );
				}

				var attForTypical = new TypicalIntAttribute();
				attForTypical.TAttribute = attData;
				attForTypical.Value = tInfo.ListPrice;
				attForTypical.Typical = tData;
				database.TypicalIntAttributes.Add( attForTypical );
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
			FillTypicalAttribute( tData, tInfo.Shape, "Shape" );

			arrKeywordList.Add( tInfo.FeaturedSeries );
			arrKeywordList.Add( tInfo.Name );
			arrKeywordList.Add( tInfo.SeriesList );
			//				arrKeywordList.Add( tInfo.Keywords );

			tData.DBKeywords = SearchText.GetKeywordList( arrKeywordList );

			tData.SpecRequest = specRequest;
			tData.Notes = tInfo.Notes;
			tData.AvailableForIn2 = tInfo.AvailableForIn2;

			database.Typicals.Add( tData );

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
				database.TAttributes.Add( attData );
			}

			if( tData.TypicalTextAttributes.Any( tta => tta.AttributeID == attData.AttributeID ) )
			{
				database.TypicalTextAttributes.Remove( tData.TypicalTextAttributes.First( tta => tta.AttributeID == attData.AttributeID ) );
			}

			var attForTypical = new TypicalTextAttribute();
			attForTypical.TAttribute = attData;
			attForTypical.Value = @"Typicals/" + tData.Name + @"/" + fileName;
			attForTypical.Typical = tData;
			database.TypicalTextAttributes.Add( attForTypical );
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
				Shape = string.Join( ",", tInfo.AttributeSet( "Shape" ) ),

				RenderingImage = string.Join( ",", tInfo.TypicalImageFiles.Where( tif => tif.IsFeatured ).Select( tif => tif.ImageFile.Name ) ),
				AdditionalImages = string.Join( ",", tInfo.TypicalImageFiles.Where( tif => !tif.IsFeatured ).Select( tif => tif.ImageFile.Name ) ),

				xlsFileName = "",
				sifFileName = "",
				sp4FileName = "",
				pdfFileName = "",
				dwgFileName = "",

				IsPublished = tInfo.IsPublished
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

		public bool UpdateTypical( TypicalMgmtInfo tInfo, bool togglePublish )
		{
			var tData = database.Typicals.FirstOrDefault( t => t.TypicalID == tInfo.TypicalID );
			if( tData == null )
			{
				throw new Exception( "Unable to find Typical for Spec Request" );
			}

			if( togglePublish )
			{
				tData.IsPublished = !tData.IsPublished;
			}

			tData.SeriesTypicals.ToList().ForEach( st => database.SeriesTypicals.Remove( st ) );

			List<string> arrKeywordList = new List<string>();
			var rSeries = database.Serieses.FirstOrDefault( s => s.Name == tInfo.FeaturedSeries );
			if( rSeries != null )
			{
				SeriesTypical stData = new SeriesTypical();
				stData.IsPrimary = true;
				stData.Series = rSeries;
				stData.Typical = tData;
				database.SeriesTypicals.Add( stData );
				tData.FeaturedSeries = rSeries.Name;
			}
			else
			{
				tData.FeaturedSeries = null;
			}

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
						database.SeriesTypicals.Add( stData );
					}
				}
				tData.SeriesList = string.Join( ", ", tData.SeriesTypicals.Where( st => !st.IsPrimary ).Select( st => st.Series.Name ) );
			}
			else
			{
				tData.SeriesList = null;
			}

			tData.TypicalImageFiles.ToList().ForEach( tif => database.TypicalImageFiles.Remove( tif ) );

			var img = database.ImageFiles.FirstOrDefault( i => i.Name == tInfo.RenderingImage );
			if( img != null )
			{
				TypicalImageFile sif = new TypicalImageFile();
				sif.IsFeatured = true;
				sif.ImageFile = img;
				sif.Typical = tData;
				database.TypicalImageFiles.Add( sif );
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
						database.TypicalImageFiles.Add( sif );
					}
				}
			}

			{
				var attData = database.TAttributes.FirstOrDefault( a => a.Name == "Pricing" );
				if( attData == null )
				{
					attData = new PDWDBContext.TAttribute();
					attData.Name = "Pricing";
					database.TAttributes.Add( attData );
				}

				if( tData.TypicalIntAttributes.Any( tia => tia.AttributeID == attData.AttributeID ) )
				{
					database.TypicalIntAttributes.Remove( tData.TypicalIntAttributes.First( tia => tia.AttributeID == attData.AttributeID ) );
				}

				var attForTypical = new TypicalIntAttribute();
				attForTypical.TAttribute = attData;
				attForTypical.Value = tInfo.ListPrice;
				attForTypical.Typical = tData;
				database.TypicalIntAttributes.Add( attForTypical );
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
			FillTypicalAttribute( tData, tInfo.Shape, "Shape" );

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
					database.TAttributes.Add( attData );
				}

				if( tData.TypicalOptionAttributes.Any( toa => toa.AttributeID == attData.AttributeID ) )
				{
					database.TypicalOptionAttributes.Remove( tData.TypicalOptionAttributes.First( toa => toa.AttributeID == attData.AttributeID ) );
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
						database.TAttributeOptions.Add( optVal );
						attData.TAttributeOptions.Add( optVal );
					}

					var attForTypical = new TypicalOptionAttribute();
					attForTypical.TAttribute = attData;
					attForTypical.TAttributeOption = optVal;
					attForTypical.Typical = tData;
					database.TypicalOptionAttributes.Add( attForTypical );
				}
			}
		}

		public IEnumerable<string> GetFootprintList( string query )
		{
			return database.SpecRequests
				.Where( ao => ao.Footprint.Contains( query ) )
				.Select( ao => ao.Footprint )
				.Distinct()
				.OrderBy( s => s )
				.ToList();
		}

		public IEnumerable<IDToTextItem> GetGSAContractList()
		{
			return database.GSAContracts
				.Select( g => new IDToTextItem() { ID = g.ContractID, Text = g.Name } );
		}

		public bool CancelRequest( int requestId )
		{
			var specInfo = database.SpecRequests.FirstOrDefault( s => s.RequestID == requestId );
			if( specInfo == null )
			{
				throw new Exception( "Unable to find Spec Request" );
			}

			specInfo.IsCanceled = true;
			specInfo.IsCompleted = true;
			specInfo.SpecRequestEvents.Add( SpecRequestEvent.CanceledEvent( PaoliWebUser.CurrentUser.UserId ) );

			return database.SaveChanges() > 0;
		}

		public bool ReOpenRequest( int requestId )
		{
			var specInfo = database.SpecRequests
				.Include( s => s.SpecRequestEvents )
				.Include( s => s.SpecRequestEvents.Select( e => e.User ) )
				.Include( s => s.SpecRequestEvents.Select( e => e.User.Company ) )
				.Include( s => s.SpecRequestEvents.Select( e => e.User.Company.PaoliMember ) )
				.Include( s => s.PaoliSalesRepMember )
				.Include( s => s.PaoliSalesRepMember.Company )
				.Include( s => s.PaoliSalesRepMember.Company.PaoliMember )
				.Include( s => s.PaoliSalesRepGroup )
				.Include( s => s.PaoliSalesRepGroup.Users )
				.Include( s => s.PaoliSalesRepGroup.Users.Select( u => u.Company ) )
				.Include( s => s.PaoliSalesRepGroup.Users.Select( u => u.Company.PaoliMember ) )
				.Include( s => s.DealerSalesRep )
				.Include( s => s.DealerSalesRep.Company )
				.Include( s => s.PrimaryCompany )
				.Include( s => s.DealerSalesRep.Company.Territory )
				.Include( s => s.PrimaryCompany.Territory )
				.Include( s => s.PaoliSalesRepMember.Company.Territory )
				.Include( s => s.PaoliSalesRepGroup.Territory )
				.Include( s => s.Project )
				.FirstOrDefault( s => s.RequestID == requestId );
			if( specInfo == null )
			{
				throw new Exception( "Unable to find Spec Request" );
			}

			specInfo.IsCompleted = false;

			specInfo.SpecRequestEvents.Add( SpecRequestEvent.ReOpenedEvent( PaoliWebUser.CurrentUser.UserId ) );

			if( database.SaveChanges() > 0 )
			{
				EmailForReopenedRequest( specInfo, new List<EmailSender.FileDetail>(), null );

				return true;
			}

			return false;
		}


		public IEnumerable<ProjectSummary> GetUserProjectList( ProjectTableParams paramDetails, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var requestList = database.Projects
				.Include( p => p.Company )
				.Include( p => p.EndCustomerCompany )
				.AsQueryable();

			var dbUser = database.Users.FirstOrDefault( u => u.UserID == PaoliWebUser.CurrentUser.UserId );

			if( PaoliWebUser.CurrentUser.IsDealerUser )
			{
				requestList = requestList.Where( s => s.DealerID == dbUser.CompanyID );
			}
			else if( PaoliWebUser.CurrentUser.IsInRole( PaoliWebUser.PaoliWebRole.PaoliSalesRep ) )
			{
				requestList = requestList.Where( s => s.TerritoryID == dbUser.Company.TerritoryID );
			}

			totalRecords = requestList.Count();

			if( !string.IsNullOrEmpty( paramDetails.sSearch ) )
			{
				requestList = requestList.Where( i =>
					i.ProjectName.Contains( paramDetails.sSearch ) ||
					i.EndCustomerCompany.Name.Contains( paramDetails.sSearch ) ||
					i.Company.Name.Contains( paramDetails.sSearch ) );
			}

			string sortCol = paramDetails.sColumns.Split( ',' )[paramDetails.iSortCol_0];

			IQueryable<Project> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
				case "projectname":
				default:
					if( paramDetails.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = requestList.OrderBy( v => v.ProjectName );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.ProjectName );
					}
					break;
				case "customername":
					if( paramDetails.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = requestList.OrderBy( v => v.EndCustomerCompany.Name );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.EndCustomerCompany.Name );
					}
					break;
				case "dealershipname":
					if( paramDetails.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = requestList.OrderBy( v => v.Company.Name );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.Company.Name );
					}
					break;
				case "projectstatus":
					if( paramDetails.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = requestList.OrderBy( v => v.ProjectSuccess );
					}
					else
					{
						filteredAndSorted = requestList.OrderByDescending( v => v.ProjectSuccess );
					}
					break;
			}

			displayedRecords = requestList.Count();

			if( ( displayedRecords > paramDetails.iDisplayLength ) && ( paramDetails.iDisplayLength > 0 ) )
			{
				filteredAndSorted = filteredAndSorted.Skip( paramDetails.iDisplayStart ).Take( paramDetails.iDisplayLength );
			}

			return filteredAndSorted.ToList().Select( v => ToProjectSummary( v ) );
		}

		private ProjectSummary ToProjectSummary( Project dbProject )
		{			
			return new ProjectSummary()
			{
				ProjectID = dbProject.ProjectID,
				ProjectName = dbProject.ProjectName,
				CustomerName = dbProject.EndCustomerID.HasValue ? dbProject.EndCustomerCompany.Name : null,
				DealershipName = dbProject.DealerID.HasValue ? dbProject.Company.Name : null,
				ProjectStatus = ProjectSuccess.GetDisplayString( dbProject.ProjectSuccess )
			};
		}

		public ProjectSeriesInformation GetProjectSeriesInformation()
		{
			return new ProjectSeriesInformation()
			{
				VeneerCasegoods = database.Serieses.Where( s => s.Category.Name == "Casegood" )
					.Where( s => database.ImageFiles.Any( sif => sif.FinishType == (int)( PDWModels.Images.ImageInformation.FinishTypes.Veneer ) && 
						s.SeriesOptionAttributes.Where( soa => soa.Attribute.Name == "Finish" ).Any( soa => soa.AttributeOption.Name == sif.FeaturedFinish ) ) )
					.Select( s => s.Name ).ToList(),
				LaminateCasegoods = database.Serieses.Where( s => s.Category.Name == "Casegood" )
					.Where( s => database.ImageFiles.Any( sif => sif.FinishType == (int)( PDWModels.Images.ImageInformation.FinishTypes.Laminate ) &&
						s.SeriesOptionAttributes.Where( soa => soa.Attribute.Name == "Finish" ).Any( soa => soa.AttributeOption.Name == sif.FeaturedFinish ) ) )
					.Select( s => s.Name ).ToList(),
				Seating = database.Serieses.Where( s => s.Category.Name == "Seating" ).Select( s => s.Name ).ToList(),
				Conferencing = database.Serieses.Where( s => s.Category.Name == "Tables" ).Select( s => s.Name ).ToList()
			};
		}

		public void Add( ProjectInformation pInfo )
		{
			var newProject = new Project()
			{
				ProjectName = pInfo.ProjectName,
				DealerID = pInfo.DealerID.HasValue ? (pInfo.DealerID.Value > 0 ? pInfo.DealerID : null) : null,
				EndCustomerID = pInfo.EndCustomerID.HasValue ? (pInfo.EndCustomerID.Value > 0 ? pInfo.EndCustomerID : null) : null,
				TerritoryID = pInfo.TerritoryID,

				IsGSA = pInfo.IsGSA,
				ContractID = pInfo.IsGSA ? pInfo.ContractID : (int?)null,

				HasADFirm = pInfo.HasADFirm,
				ADFirm = pInfo.HasADFirm ? pInfo.ADFirm : null,

				AnticipatedOrderDate = pInfo.AnticipatedOrderDate,
				AnticipatedShipDate = pInfo.AnticipatedShipDate,

				VeneerCasegoods = pInfo.VeneerCasegoods != null ? string.Join( ",", pInfo.VeneerCasegoods ) : "",
				NetVeneerCasegoods = pInfo.NetVeneerCasegoods,
				LaminateCasegoods = pInfo.LaminateCasegoods != null ? string.Join( ",", pInfo.LaminateCasegoods ) : "",
				NetLaminateCasegoods = pInfo.NetLaminateCasegoods,
				Conferencing = pInfo.Conferencing != null ? string.Join( ",", pInfo.Conferencing ) : "",
				NetConferencing = pInfo.NetConferencing,
				Seating = pInfo.Seating != null ? string.Join( ",", pInfo.Seating ) : "",
				NetSeating = pInfo.NetSeating,

				SPADone = pInfo.SPADone,
				Probability = pInfo.Probability,

				PrimaryCompetitor = pInfo.PrimaryCompetitor,
				Comments = pInfo.Comments,

				PipelineStatus = pInfo.PipelineStatus,
				ProjectSuccess = pInfo.ProjectSuccess
			};

			database.Projects.Add( newProject );

			database.SaveChanges();
		}

		public int Add( NewProjectInformation pInfo )
		{
			var newProject = new Project()
			{
				ProjectName = pInfo.projectName,
				DealerID = pInfo.dealer.HasValue ? ( pInfo.dealer.Value > 0 ? pInfo.dealer : null ) : null,
				EndCustomerID = pInfo.customer,
				TerritoryID = pInfo.territory,

				IsGSA = pInfo.isGSA,
				ContractID = pInfo.isGSA ? pInfo.contractId : null,
				HasADFirm = false,
				SPADone = false,
				PipelineStatus = ProjectStatus.Step1,
				ProjectSuccess = ProjectSuccess.Pending
			};

			database.Projects.Add( newProject );

			database.SaveChanges();

			database.Entry( newProject ).Reload();

			return newProject.ProjectID;
		}

		public ProjectInformation GetProject( int projectId )
		{
			var dbProject = database.Projects.FirstOrDefault( p => p.ProjectID == projectId );
			if( dbProject == null )
			{
				throw new Exception( "Unable to find project" );
			}

			return new ProjectInformation()
			{
				ProjectID = dbProject.ProjectID,
				ProjectName = dbProject.ProjectName,
				DealerID = dbProject.DealerID,
				EndCustomerID = dbProject.EndCustomerID,
				TerritoryID = dbProject.TerritoryID,

				IsGSA = dbProject.IsGSA ?? false,
				ContractID = dbProject.ContractID,
				ContractName  = (dbProject.IsGSA ?? false) ? (dbProject.ContractID.HasValue ? dbProject.GSAContract.Name : null) : null,
				HasADFirm = dbProject.HasADFirm,
				ADFirm = dbProject.ADFirm,

				AnticipatedOrderDate = dbProject.AnticipatedOrderDate,
				AnticipatedShipDate = dbProject.AnticipatedShipDate,

				VeneerCasegoods = ( dbProject.VeneerCasegoods ?? "" ).Split( ',' ).ToList(),
				NetVeneerCasegoods = dbProject.NetVeneerCasegoods,
				LaminateCasegoods = ( dbProject.LaminateCasegoods ?? "" ).Split( ',' ).ToList(),
				NetLaminateCasegoods = dbProject.NetLaminateCasegoods,
				Conferencing = ( dbProject.Conferencing ?? "" ).Split( ',' ).ToList(),
				NetConferencing = dbProject.NetConferencing,
				Seating = ( dbProject.Seating ?? "" ).Split( ',' ).ToList(),
				NetSeating = dbProject.NetSeating,

				SPADone = dbProject.SPADone,
				Probability = dbProject.Probability,

				PrimaryCompetitor = dbProject.PrimaryCompetitor,
				Comments = dbProject.Comments,

				PipelineStatus = dbProject.PipelineStatus,
				ProjectSuccess = dbProject.ProjectSuccess
			};
		}

		public void Update( ProjectInformation pInfo )
		{
			var dbProject = database.Projects.FirstOrDefault( p => p.ProjectID == pInfo.ProjectID );
			if( dbProject == null )
			{
				throw new Exception( "Unable to find project" );
			}

			dbProject.ProjectName = pInfo.ProjectName;
			dbProject.DealerID = pInfo.DealerID.HasValue ? ( pInfo.DealerID.Value > 0 ? pInfo.DealerID : null ) : null;
			dbProject.EndCustomerID = pInfo.EndCustomerID.HasValue ? ( pInfo.EndCustomerID.Value > 0 ? pInfo.EndCustomerID : null ) : null;
			dbProject.TerritoryID = pInfo.TerritoryID;

			dbProject.IsGSA = pInfo.IsGSA;
			dbProject.ContractID = pInfo.IsGSA ? pInfo.ContractID : (int?)null;

			dbProject.HasADFirm = pInfo.HasADFirm;
			dbProject.ADFirm = pInfo.HasADFirm ? pInfo.ADFirm : null;

			dbProject.AnticipatedOrderDate = pInfo.AnticipatedOrderDate;
			dbProject.AnticipatedShipDate = pInfo.AnticipatedShipDate;

			dbProject.VeneerCasegoods = pInfo.VeneerCasegoods != null ? string.Join( ",", pInfo.VeneerCasegoods ) : "";
			dbProject.NetVeneerCasegoods = pInfo.NetVeneerCasegoods;
			dbProject.LaminateCasegoods = pInfo.LaminateCasegoods != null ? string.Join( ",", pInfo.LaminateCasegoods ) : "";
			dbProject.NetLaminateCasegoods = pInfo.NetLaminateCasegoods;
			dbProject.Conferencing = pInfo.Conferencing != null ? string.Join( ",", pInfo.Conferencing ) : "";
			dbProject.NetConferencing = pInfo.NetConferencing;
			dbProject.Seating = pInfo.Seating != null ? string.Join( ",", pInfo.Seating ) : "";
			dbProject.NetSeating = pInfo.NetSeating;

			dbProject.SPADone = pInfo.SPADone;
			dbProject.Probability = pInfo.Probability;

			dbProject.PrimaryCompetitor = pInfo.PrimaryCompetitor;
			dbProject.Comments = pInfo.Comments;

			dbProject.PipelineStatus = pInfo.PipelineStatus;
			dbProject.ProjectSuccess = pInfo.ProjectSuccess;

			database.SaveChanges();
		}

		public IEnumerable<IDToTextItem> GetEndCustomerList()
		{
			var theList = database.Companies
				.Where( c => c.CompanyType == PaoliWebUser.PaoliCompanyType.EndUser )
				.Select( p => new IDToTextItem() { ID = p.CompanyID, Text = p.Name } )
				.ToList();

			theList.Insert( 0, new IDToTextItem() { ID = 0, Text = "" } );

			return theList.OrderBy( i => i.Text );
		}

		public IEnumerable<IDToTextItemExtra> GetProjectForCustomer( int customerId, int? dealerId, int territoryId )
		{
			return database.Projects
				.Where( p => p.EndCustomerID == customerId )
				.Where( p => p.TerritoryID == territoryId )
				.Where( p => p.DealerID == dealerId || !dealerId.HasValue || dealerId == 0 )
				.ToList()
				.Select( p => new IDToTextItemExtra() { ID = p.ProjectID, Text = p.ProjectName, Extra = p.DealerID.HasValue ? p.DealerID.ToString() : "" } )
				.ToList();
		}
	}
}
