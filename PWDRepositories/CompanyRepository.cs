using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWModels.Companies;
using PDWInfrastructure;
using PDWModels;
using System.Data.Entity;
using System.IO;
using System.Configuration;
using System.Drawing;
using PDWInfrastructure.EmailSenders;
using LumenWorks.Framework.IO.Csv;

namespace PWDRepositories
{
	public class CompanyRepository : BaseRepository
	{
		public CompanyRepository()
		{
		}

		private CompanySummary ToCompanySummary( Company c )
		{
			return new CompanySummary()
			{
				CompanyID = c.CompanyID,
				Name = c.FullName,
				CompanyType = PaoliWebUser.PaoliCompanyType.CompanyTypeList[c.CompanyType],
				MasterID = c.MasterID,
				BaseNumber = c.SubCompanyIDs,
				City = c.City,
				State = c.State,
				SignedUpForTrip = c.SignedUpForTrip && ( c.CompanyTripData != null ),
				TripGroupInfo = c.SignedUpForTrip && ( c.CompanyTripData != null ) ? ( "Yes - " + c.TripGroup ) : "No",
				PSRContactID = c.PaoliSalesRepMemberID,
				PSRContact = c.PaoliSalesRepMemberID.HasValue ? c.PaoliSalesRepMember.FullName : "",
				TierGroup = c.TierGroup,
				UserCount = c.Users.Where( u => u.IsActive && u.Enabled ).Count(),
				CanDelete = c.CanDelete > 0
			};
		}

		public bool AddCompany( CompanyInformation cInfo, Stream imgStream, string fileName )
		{
			if( PaoliWebUser.PaoliCompanyType.HasTerritory.Contains( cInfo.CompanyType ) && 
				!cInfo.TerritoryID.HasValue )
			{
				throw new Exception( "Territory ID is a required field" );
			}

			Company newCompany = new Company();

			newCompany.Name = cInfo.Name;
			newCompany.Address1 = cInfo.Address1;
			newCompany.Address2 = cInfo.Address2;
			newCompany.City = cInfo.City;
			newCompany.State = cInfo.State;
			newCompany.Zip = cInfo.Zip;
			newCompany.Phone = cInfo.Phone;
			newCompany.FAX = cInfo.FAX;
			newCompany.MasterID = cInfo.MasterID;
			newCompany.SubCompanyIDs = cInfo.SubCompanyIDs;
			newCompany.CompanyType = cInfo.CompanyType;
			newCompany.TerritoryID = PaoliWebUser.PaoliCompanyType.HasTerritory.Contains( cInfo.CompanyType ) ? cInfo.TerritoryID : null;
			newCompany.PaoliMemberID = PaoliWebUser.PaoliCompanyType.HasTerritory.Contains( cInfo.CompanyType ) ? cInfo.PaoliMemberID : null;
			newCompany.PaoliSalesRepMemberID = cInfo.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer ? cInfo.PaoliSalesRepMemberID : null;
			newCompany.BusinessUnitName = cInfo.BusinessUnitName;
			newCompany.PublicAddress1 = cInfo.PublicAddress1;
			newCompany.PublicAddress2 = cInfo.PublicAddress2;
			newCompany.PublicCity = cInfo.PublicCity;
			newCompany.PublicState = cInfo.PublicState;
			newCompany.PublicZip = cInfo.PublicZip;
			newCompany.PublicPhone = cInfo.PublicPhone;
			newCompany.PublicFAX = cInfo.PublicFAX;
			newCompany.ContactEmail = cInfo.ContactEmail;
			newCompany.WebSite = cInfo.WebSite;
			newCompany.PublicContactEmail = cInfo.PublicContactEmail;
			newCompany.PublicWebSite = cInfo.PublicWebSite;
			newCompany.PublicDisplayName = cInfo.PublicDisplayName;
			newCompany.SignedUpForTrip = cInfo.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer ? cInfo.SignedUpForTrip : false;
			newCompany.TripGroup = cInfo.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer ? (newCompany.SignedUpForTrip ? cInfo.TripGroup : null) : null;
			newCompany.TierGroup = cInfo.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer ? cInfo.TierGroup : null;

			if( cInfo.HasShowroom )
			{
				Showroom newShowroom = new Showroom();

				newShowroom.DisplayName = cInfo.ShowroomDisplayName;
				newShowroom.Address1 = cInfo.ShowroomAddress1;
				newShowroom.Address2 = cInfo.ShowroomAddress2;
				newShowroom.City = cInfo.ShowroomCity;
				newShowroom.State = cInfo.ShowroomState;
				newShowroom.Zip = cInfo.ShowroomZip;
				newShowroom.Phone = cInfo.ShowroomPhone;
				newShowroom.FAX = cInfo.ShowroomFAX;
				newShowroom.WebSite = cInfo.ShowroomWebsite;
				newShowroom.Description = cInfo.ShowroomDescription;
				newShowroom.Hours = cInfo.ShowroomHours;

				newCompany.Showroom = newShowroom;

				if( ( cInfo.ShowroomImages ?? "" ).Any() )
				{
					foreach( var indVal in cInfo.ShowroomImages.Split( ',' ).Select( s => s.Trim() ) )
					{
						var oImg = database.ImageFiles.FirstOrDefault( s => s.Name == indVal );
						if( oImg != null )
						{
							newCompany.ShowroomImages.Add( oImg );
						}
					}
				}
			}

			if( imgStream != null )
			{
				newCompany.ImageFileName = Guid.NewGuid().ToString() + Path.GetExtension( fileName );

				Image fullSizeImg = Image.FromStream( imgStream );
				fullSizeImg.Save( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"],
					newCompany.ImageFileName ) );
			}

			database.Companies.Add( newCompany );

			return database.SaveChanges() > 0;
		}

		public CompanyInformation GetCompany( int id )
		{
			var eCompany = database.Companies.FirstOrDefault( u => u.CompanyID == id );
			if( eCompany == null )
			{
				throw new Exception( "Unable to find company." );
			}

			return new CompanyInformation()
			{
				CompanyID = eCompany.CompanyID,
				Name = eCompany.Name,
				Address1 = eCompany.Address1,
				Address2 = eCompany.Address2,
				City = eCompany.City,
				State = eCompany.State,
				Zip = eCompany.Zip,
				Phone = eCompany.Phone,
				FAX = eCompany.FAX,
				PublicAddress1 = eCompany.PublicAddress1,
				PublicAddress2 = eCompany.PublicAddress2,
				PublicCity = eCompany.PublicCity,
				PublicState = eCompany.PublicState,
				PublicZip = eCompany.PublicZip,
				PublicPhone = eCompany.PublicPhone,
				PublicFAX = eCompany.PublicFAX,
				MasterID = eCompany.MasterID,
				SubCompanyIDs = eCompany.SubCompanyIDs,
				CompanyType = eCompany.CompanyType,
				TerritoryID = eCompany.TerritoryID,
				PaoliMemberID = eCompany.PaoliMemberID,
				PaoliSalesRepMemberID = eCompany.PaoliSalesRepMemberID,
				BusinessUnitName = eCompany.BusinessUnitName,
				LockCompanyType = eCompany.Users.Any() || eCompany.SpecRequests.Any() || eCompany.SpecRequests1.Any() || eCompany.CollateralOrders.Any() || eCompany.CollateralOrders1.Any() ||
					eCompany.CollateralOrders2.Any() || eCompany.CollateralOrders3.Any() || eCompany.Projects.Any() || eCompany.Projects1.Any() || eCompany.eCollateralItems.Any(),
				ContactEmail = eCompany.ContactEmail,
				WebSite = eCompany.WebSite,
				PublicContactEmail = eCompany.PublicContactEmail,
				PublicWebSite = eCompany.PublicWebSite,
				PublicDisplayName = eCompany.PublicDisplayName,
				SignedUpForTrip = eCompany.SignedUpForTrip,
				TripGroup = eCompany.TripGroup,
				TierGroup = eCompany.TierGroup,
				ImageFileName = eCompany.ImageFileName,

				HasShowroom = eCompany.Showroom != null,
				ShowroomDisplayName = eCompany.Showroom != null ? eCompany.Showroom.DisplayName : null,
				ShowroomAddress1 = eCompany.Showroom != null ? eCompany.Showroom.Address1 : null,
				ShowroomAddress2 = eCompany.Showroom != null ? eCompany.Showroom.Address2 : null,
				ShowroomCity = eCompany.Showroom != null ? eCompany.Showroom.City : null,
				ShowroomState = eCompany.Showroom != null ? eCompany.Showroom.State : null,
				ShowroomZip = eCompany.Showroom != null ? eCompany.Showroom.Zip : null,
				ShowroomPhone = eCompany.Showroom != null ? eCompany.Showroom.Phone : null,
				ShowroomFAX = eCompany.Showroom != null ? eCompany.Showroom.FAX : null,
				ShowroomWebsite = eCompany.Showroom != null ? eCompany.Showroom.WebSite : null,
				ShowroomDescription = eCompany.Showroom != null ? eCompany.Showroom.Description : null,
				ShowroomHours = eCompany.Showroom != null ? eCompany.Showroom.Hours : null,
				ShowroomImages = eCompany.Showroom != null ? string.Join( ",", eCompany.ShowroomImages.Select( sri => sri.Name ) ) : null

			};
		}

		public bool UpdateCompany( CompanyInformation cInfo, Stream imgStream, string fileName )
		{
			if( PaoliWebUser.PaoliCompanyType.HasTerritory.Contains( cInfo.CompanyType ) &&
				!cInfo.TerritoryID.HasValue )
			{
				throw new Exception( "Territory ID is a required field" );
			}

			var eCompany = database.Companies.FirstOrDefault( u => u.CompanyID == cInfo.CompanyID );
			if( eCompany == null )
			{
				throw new Exception( "Unable to find company." );
			}

			eCompany.Name = cInfo.Name;
			eCompany.Address1 = cInfo.Address1;
			eCompany.Address2 = cInfo.Address2;
			eCompany.City = cInfo.City;
			eCompany.State = cInfo.State;
			eCompany.Zip = cInfo.Zip;
			eCompany.Phone = cInfo.Phone;
			eCompany.FAX = cInfo.FAX;
			eCompany.MasterID = cInfo.MasterID;
			eCompany.SubCompanyIDs = cInfo.SubCompanyIDs;
			eCompany.CompanyType = cInfo.CompanyType;
			eCompany.TerritoryID = PaoliWebUser.PaoliCompanyType.HasTerritory.Contains( cInfo.CompanyType ) ? cInfo.TerritoryID : null;
			eCompany.PaoliMemberID = PaoliWebUser.PaoliCompanyType.HasTerritory.Contains( cInfo.CompanyType ) ? cInfo.PaoliMemberID : null;
			eCompany.PaoliSalesRepMemberID = cInfo.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer ? cInfo.PaoliSalesRepMemberID : null;
			eCompany.BusinessUnitName = cInfo.BusinessUnitName;
			eCompany.PublicAddress1 = cInfo.PublicAddress1;
			eCompany.PublicAddress2 = cInfo.PublicAddress2;
			eCompany.PublicCity = cInfo.PublicCity;
			eCompany.PublicState = cInfo.PublicState;
			eCompany.PublicZip = cInfo.PublicZip;
			eCompany.PublicPhone = cInfo.PublicPhone;
			eCompany.PublicFAX = cInfo.PublicFAX;
			eCompany.ContactEmail = cInfo.ContactEmail;
			eCompany.WebSite = cInfo.WebSite;
			eCompany.PublicContactEmail = cInfo.PublicContactEmail;
			eCompany.PublicWebSite = cInfo.PublicWebSite;
			eCompany.PublicDisplayName = cInfo.PublicDisplayName;
			eCompany.SignedUpForTrip = cInfo.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer ? cInfo.SignedUpForTrip : false;
			eCompany.TripGroup = cInfo.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer ? ( eCompany.SignedUpForTrip ? cInfo.TripGroup : null ) : null;
			eCompany.TierGroup = cInfo.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer ? cInfo.TierGroup : null;

			eCompany.ShowroomImages.Clear();

			if( cInfo.HasShowroom )
			{
				Showroom eShowroom = eCompany.Showroom;
				if( eShowroom == null )
				{
					eShowroom = new Showroom();
					eCompany.Showroom = eShowroom;
				}

				eShowroom.DisplayName = cInfo.ShowroomDisplayName;
				eShowroom.Address1 = cInfo.ShowroomAddress1;
				eShowroom.Address2 = cInfo.ShowroomAddress2;
				eShowroom.City = cInfo.ShowroomCity;
				eShowroom.State = cInfo.ShowroomState;
				eShowroom.Zip = cInfo.ShowroomZip;
				eShowroom.Phone = cInfo.ShowroomPhone;
				eShowroom.FAX = cInfo.ShowroomFAX;
				eShowroom.WebSite = cInfo.ShowroomWebsite;
				eShowroom.Description = cInfo.ShowroomDescription;
				eShowroom.Hours = cInfo.ShowroomHours;
				
				if( ( cInfo.ShowroomImages ?? "" ).Any() )
				{
					foreach( var indVal in cInfo.ShowroomImages.Split( ',' ).Select( s => s.Trim() ) )
					{
						var oImg = database.ImageFiles.FirstOrDefault( s => s.Name == indVal );
						if( oImg != null )
						{
							eCompany.ShowroomImages.Add( oImg );
						}
					}
				}

			}
			else if( eCompany.Showroom != null )
			{
				database.Showrooms.Remove( eCompany.Showroom );
			}

			if( imgStream != null )
			{
				eCompany.ImageFileName = Guid.NewGuid().ToString() + Path.GetExtension( fileName );

				Image fullSizeImg = Image.FromStream( imgStream );
				fullSizeImg.Save( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"],
					eCompany.ImageFileName ) );
			}

			return database.SaveChanges() > 0;
		}

		public int UpdateCompany( int reqUserId, MyCompanyInfo cInfo )
		{
			var dbReqUser = database.Users.FirstOrDefault( u => u.UserID == reqUserId );
			if( cInfo.TheCompanyID != 0 )
			{
				var eCompany = database.Companies.FirstOrDefault( u => u.CompanyID == cInfo.TheCompanyID );
				if( eCompany == null )
				{
					throw new Exception( "Unable to find company." );
				}

				eCompany.Name = cInfo.CompanyName;
				eCompany.Address1 = cInfo.Address1;
				eCompany.Address2 = cInfo.Address2;
				eCompany.City = cInfo.City;
				eCompany.State = cInfo.State;
				eCompany.Zip = cInfo.Zip;
				eCompany.Phone = cInfo.PhoneNumber;
				eCompany.FAX = cInfo.FaxNumber;
				eCompany.ContactEmail = cInfo.ContactEmail;
				eCompany.WebSite = cInfo.WebSite;

				cInfo.CompanyType = eCompany.CompanyType;

				database.SaveChanges();

				new ChangeDealerInfoEmailSender( "ChangeDealerInfo" ).SubmitRequestEmail( dbReqUser.FullName, dbReqUser.Company.Name, cInfo.TheCompanyID, cInfo.CompanyName, cInfo.Address1, cInfo.Address2,
					cInfo.City, cInfo.State, cInfo.Zip, cInfo.PhoneNumber, cInfo.FaxNumber, cInfo.ContactEmail, cInfo.WebSite,
					PaoliWebUser.PaoliCompanyType.CompanyTypeList[cInfo.CompanyType], eCompany.MasterID, eCompany.SubCompanyIDs );

				return eCompany.CompanyID;
			}
			else
			{
				if( PaoliWebUser.PaoliCompanyType.HasTerritory.Contains( cInfo.CompanyType ) &&
					!cInfo.TerritoryID.HasValue )
				{
					throw new Exception( "Territory ID is a required field" );
				}

				Company newCompany = new Company();

				newCompany.Name = cInfo.CompanyName;
				newCompany.Address1 = cInfo.Address1;
				newCompany.Address2 = cInfo.Address2;
				newCompany.City = cInfo.City;
				newCompany.State = cInfo.State;
				newCompany.Zip = cInfo.Zip;
				newCompany.Phone = cInfo.PhoneNumber;
				newCompany.FAX = cInfo.FaxNumber;
				newCompany.CompanyType = cInfo.CompanyType;
				newCompany.TerritoryID = PaoliWebUser.PaoliCompanyType.HasTerritory.Contains( cInfo.CompanyType ) ? cInfo.TerritoryID : null;
				newCompany.PaoliMemberID = null;
				newCompany.PaoliSalesRepMemberID = null;
				newCompany.ContactEmail = cInfo.ContactEmail;
				newCompany.WebSite = cInfo.WebSite;
				newCompany.SignedUpForTrip = false;
				newCompany.TripGroup = null;
				newCompany.TierGroup = cInfo.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer ? PaoliWebUser.PaoliTierGroup.General : null;

				database.Companies.Add( newCompany );

				database.SaveChanges();

				new ChangeDealerInfoEmailSender( "AddDealerInfo" ).SubmitRequestEmail( dbReqUser.FullName, dbReqUser.Company.Name, cInfo.TheCompanyID, cInfo.CompanyName, cInfo.Address1, cInfo.Address2,
					cInfo.City, cInfo.State, cInfo.Zip, cInfo.PhoneNumber, cInfo.FaxNumber, cInfo.ContactEmail, cInfo.WebSite,
					PaoliWebUser.PaoliCompanyType.CompanyTypeList[cInfo.CompanyType], "", "" );

				database.Entry( newCompany ).Reload();

				return newCompany.CompanyID;
			}
		}

		public bool DeleteCompany( int id )
		{
			var eCompany = database.Companies.FirstOrDefault( u => u.CompanyID == id );
			if( eCompany == null )
			{
				throw new Exception( "Unable to find company." );
			}

			eCompany.ShowroomImages.Clear();
			if( eCompany.Showroom != null )
			{
				database.Showrooms.Remove( eCompany.Showroom );
			}
			if( eCompany.CompanyTripData != null )
			{
				database.CompanyTripDatas.Remove( eCompany.CompanyTripData );
			}
			if( eCompany.TerritoryTripData != null )
			{
				database.TerritoryTripDatas.Remove( eCompany.TerritoryTripData );
			}
			database.Companies.Remove( eCompany );

			return database.SaveChanges() > 0;
		}

		public IEnumerable<CompanySummary> GetFullCompanyList( CompanyTableParams param, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var companyList = database.Companies.AsQueryable();

			totalRecords = companyList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
				companyList = companyList.Where( i =>
					i.Name.Contains( param.sSearch ) ||
					i.BusinessUnitName.Contains( param.sSearch ) ||
					i.MasterID.Contains( param.sSearch ) ||
					i.SubCompanyIDs.Contains( param.sSearch ) );
			}
			if( param.companyType != 0 )
			{
				companyList = companyList.Where( c => c.CompanyType == param.companyType );
			}
			if( param.territoryId.HasValue )
			{
				companyList = companyList.Where( c => c.TerritoryID == param.territoryId.Value );
			}
			if( param.tripOnly )
			{
				companyList = companyList.Where( c => c.SignedUpForTrip );
			}

			displayedRecords = companyList.Count();

			string sortCol = param.sColumns.Split( ',' )[param.iSortCol_0];

			IQueryable<Company> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
				case "name":
				default:
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = companyList.OrderBy( v => v.Name );
					}
					else
					{
						filteredAndSorted = companyList.OrderByDescending( v => v.Name );
					}
					break;
				case "masterid":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = companyList.OrderBy( v => v.MasterID );
					}
					else
					{
						filteredAndSorted = companyList.OrderByDescending( v => v.MasterID );
					}
					break;
				case "city":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = companyList.OrderBy( v => v.City );
					}
					else
					{
						filteredAndSorted = companyList.OrderByDescending( v => v.City );
					}
					break;
				case "state":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = companyList.OrderBy( v => v.State );
					}
					else
					{
						filteredAndSorted = companyList.OrderByDescending( v => v.State );
					}
					break;
				case "usercount":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = companyList.OrderBy( v => v.Users.Where( u => u.IsActive && u.Enabled ).Count() );
					}
					else
					{
						filteredAndSorted = companyList.OrderByDescending( v => v.Users.Where( u => u.IsActive && u.Enabled ).Count() );
					}
					break;
				case "tripgroupinfo":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = companyList.OrderBy( v => v.SignedUpForTrip ).ThenBy( v => v.TripGroup );
					}
					else
					{
						filteredAndSorted = companyList.OrderByDescending( v => v.SignedUpForTrip ).ThenByDescending( v => v.TripGroup );
					}
					break;
				case "psrcontact":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = companyList.OrderBy( v => v.PaoliSalesRepMember.FirstName ).ThenBy( v => v.PaoliSalesRepMember.LastName );
					}
					else
					{
						filteredAndSorted = companyList.OrderByDescending( v => v.PaoliSalesRepMember.FirstName ).ThenByDescending( v => v.PaoliSalesRepMember.LastName );
					}
					break;
				case "tiergroup":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = companyList.OrderBy( v => v.TierGroup );
					}
					else
					{
						filteredAndSorted = companyList.OrderByDescending( v => v.TierGroup );
					}
					break;
			}

			if( ( displayedRecords > param.iDisplayLength ) && ( param.iDisplayLength > 0 ) )
			{
				filteredAndSorted = filteredAndSorted.Skip( param.iDisplayStart ).Take( param.iDisplayLength );
			}

			return filteredAndSorted
				.Include( c => c.PaoliSalesRepMember )
				.Include( c => c.Users )
				.Include( c => c.CompanyTripData )
				.ToList()
				.Select( v => ToCompanySummary( v ) );
		}

		public IEnumerable<IDToTextItem> GetFullCompanyList( int? companyType, bool includeTerritory = false )
		{
			return database.Companies
				.Where( c => !companyType.HasValue || ( companyType.HasValue && companyType.Value == c.CompanyType ) )
				.OrderBy( c => c.Name )
				.ToList()
				.Select( c => new IDToTextItem() { ID = c.CompanyID, Text = includeTerritory ? c.FullNameWithTerritory : c.FullName } );
		}

		public IEnumerable<TerritorySummary> GetTerritoryList()
		{
			return database.Territories
				.Include( t => t.Companies )
				.ToList()
				.Select( t => new TerritorySummary() { TerritoryID = t.TerritoryID, Name = t.Name, SalesRepCompany = t.SalesRepCompanyName } );
		}

		public ClosestRepInfo GetSalesRep( string zipCode )
		{
			var chosenZip = database.ZipCodeInfoes.FirstOrDefault( z => z.ZipCode == zipCode );
			if( chosenZip == null )
			{
				return null;
			}

			var closestCompany = database.Companies
				.FirstOrDefault( c => c.TerritoryID == chosenZip.TerritoryID && 
					c.CompanyType == PaoliWebUser.PaoliCompanyType.PaoliRepGroup );
			if( closestCompany == null )
			{
				return null;
			}

			var closestShowroom = closestCompany.Showroom;
			if( closestShowroom == null )
			{
				closestShowroom = database.ClosestShowroom( chosenZip.Latitude, chosenZip.Longitude )
					.FirstOrDefault();
			}

			return new ClosestRepInfo()
			{
				Name = closestCompany.PublicDisplayName,
				Address1 = closestCompany.PublicAddress1,
				Address2 = closestCompany.PublicAddress2,
				City = closestCompany.PublicCity,
				State = closestCompany.PublicState,
				Zip = closestCompany.PublicZip,
				Phone = closestCompany.PublicPhone,
				FAX = closestCompany.PublicFAX,
				ContactEmail = closestCompany.PublicContactEmail,
				WebSite = closestCompany.PublicWebSite,

				ShowroomDisplayName = closestShowroom != null ? closestShowroom.DisplayName : null,
				ShowroomAddress1 = closestShowroom != null ? closestShowroom.Address1 : null,
				ShowroomAddress2 = closestShowroom != null ? closestShowroom.Address2 : null,
				ShowroomCity = closestShowroom != null ? closestShowroom.City : null,
				ShowroomState = closestShowroom != null ? closestShowroom.State : null,
				ShowroomZip = closestShowroom != null ? closestShowroom.Zip : null,
				ShowroomPhone = closestShowroom != null ? closestShowroom.Phone : null,
				ShowroomFAX = closestShowroom != null ? closestShowroom.FAX : null,
				ShowroomWebsite = closestShowroom != null ? closestShowroom.WebSite : null,
				ShowroomDescription = closestShowroom != null ? closestShowroom.Description : null,
				ShowroomHours = closestShowroom != null ? closestShowroom.Hours : null,

				Contacts = closestCompany.Users
					.Where( u => u.AccountType == PaoliWebUser.PaoliWebRole.PaoliSalesRep && u.IsActive )
					.ToList()
					.Select( u => new ClosestRepInfo.RepInfo()
					{
						Name = u.FullName,
						Email = u.Email,
						Phone = u.BusinessPhone ?? closestCompany.PublicPhone,
						ImageFile = u.ImageFileName,
						City = u.City ?? closestCompany.PublicCity,
						State = u.State ?? closestCompany.PublicState
					} ),

				ShowroomImages = closestShowroom != null ? closestShowroom.Company.ShowroomImages.Select( sri => new PDWModels.Images.ImageSummary()
				{
					Name = sri.Name,
					FileName = sri.OriginalImage,
					ImageID = sri.ImageID,
					CanLightbox = ImageFile.ImageCanLightbox( sri.ImageType )
				} ) : new List<PDWModels.Images.ImageSummary>()
			};
		}

		public IDToTextItem GetMySalesRepInfo( bool includeTerritory = false )
		{
			var user = database.Users.FirstOrDefault( u => u.UserID == PaoliWebUser.CurrentUser.UserId );
			if( user != null )
			{
				var salesRep = database.Companies.FirstOrDefault( c => c.TerritoryID == user.Company.TerritoryID && 
					c.CompanyType == PaoliWebUser.PaoliCompanyType.PaoliRepGroup );
				if( salesRep != null )
				{
					return new IDToTextItem() { ID = salesRep.CompanyID, Text = includeTerritory ? salesRep.FullNameWithTerritory : salesRep.FullName };
				}

				throw new Exception( "Unable to find sales rep" );
			}

			throw new Exception( "Unable to find current user" );
		}

		public IEnumerable<IDToTextItem> GetMyDealerList()
		{
			var user = database.Users.FirstOrDefault( u => u.UserID == PaoliWebUser.CurrentUser.UserId );
			if( user != null )
			{
				return GetDealerList( user.CompanyID );
			}

			throw new Exception( "Unable to find current user" );
		}

		public IEnumerable<IDToTextItem> GetDealerList( int salesRepCompanyId, bool includeTerritory = false )
		{
			var salesRep = database.Companies.FirstOrDefault( c => c.CompanyID == salesRepCompanyId );
			if( salesRep != null )
			{
				return GetDealerListForTerritory( salesRep.TerritoryID.Value, includeTerritory );
			}

			return new List<IDToTextItem>();
		}

		public IEnumerable<IDToTextItem> GetDealerListForTerritory( int territoryId, bool includeTerritory = false )
		{
			return database.Companies
				.Where( c => c.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer && c.TerritoryID == territoryId )
				.OrderBy( c => c.Name )
				.ToList()
				.Select( c => new IDToTextItem() { ID = c.CompanyID, Text = includeTerritory ? c.FullNameWithTerritory : c.FullName } );
		}

		public ShippingAddress GetCompanyAddress( int companyId )
		{
			var eCompany = database.Companies.FirstOrDefault( u => u.CompanyID == companyId );
			if( eCompany == null )
			{
				return new ShippingAddress();
			}

			return new ShippingAddress()
			{
				CompanyID = eCompany.CompanyID,
				ContactAttn = null,
				Name = eCompany.FullName,
				Address1 = eCompany.Address1,
				Address2 = eCompany.Address2,
				City = eCompany.City,
				State = eCompany.State,
				Zip = eCompany.Zip,
				Phone = eCompany.Phone,
				ContactEmail = eCompany.ContactEmail
			};
		}

		public IEnumerable<IDToTextItem> GetDealerListForUser( int userId )
		{
			var user = database.Users.FirstOrDefault( u => u.UserID == userId );
			if( user != null )
			{
				switch( user.Company.CompanyType )
				{
					case PaoliWebUser.PaoliCompanyType.Paoli:
						return database.Companies
							.Where( c => c.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer )
							.OrderBy( c => c.Name )
							.ToList()
							.Select( c => new IDToTextItem() { ID = c.CompanyID, Text = c.FullName } );
					case PaoliWebUser.PaoliCompanyType.PaoliRepGroup:
						return database.Companies
							.Where( c => c.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer && c.TerritoryID == user.Company.TerritoryID )
							.OrderBy( c => c.Name )
							.ToList()
							.Select( c => new IDToTextItem() { ID = c.CompanyID, Text = c.FullName } );
					case PaoliWebUser.PaoliCompanyType.Dealer:
						return database.Companies
							.Where( c => c.CompanyID == user.CompanyID )
							.OrderBy( c => c.Name )
							.ToList()
							.Select( c => new IDToTextItem() { ID = c.CompanyID, Text = c.FullName } );
				}
			}

			return new List<IDToTextItem>();
		}

		public MyCompanyInfo GetMyCompanyInfo( int? companyId = null, int? userId = null )
		{
			if( !companyId.HasValue )
			{
				var thisUser = database.Users.FirstOrDefault( u => u.UserID == userId );
				if( thisUser == null )
				{
					throw new Exception( "Unable to find user." );
				}

				companyId = thisUser.CompanyID;
			}

			var thisCompany = database.Companies.FirstOrDefault( c => c.CompanyID == companyId.Value );

			return new MyCompanyInfo()
			{
				TheCompanyID = thisCompany.CompanyID,
				CompanyName = thisCompany.Name,
				Address1 = thisCompany.Address1,
				Address2 = thisCompany.Address2,
				City = thisCompany.City,
				State = thisCompany.State,
				Zip = thisCompany.Zip,
				PhoneNumber = thisCompany.Phone,
				FaxNumber = thisCompany.FAX,
				WebSite = thisCompany.WebSite,
				IsTripIncentive = thisCompany.SignedUpForTrip,
				ImageFileName = thisCompany.ImageFileName,
				ContactEmail = thisCompany.ContactEmail,
				TerritoryID = thisCompany.TerritoryID,
				CompanyType = thisCompany.CompanyType
			};
		}

		public MyTerritoryInfo GetMyTerritoryInfo( int userId )
		{
			var thisUser = database.Users.FirstOrDefault( u => u.UserID == userId );
			if( thisUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			return new MyTerritoryInfo()
			{
				CompanyID = thisUser.CompanyID,
				CompanyName = thisUser.Company.Name,
				TerritoryID = thisUser.Company.TerritoryID.Value,
				Address1 = thisUser.Company.Address1,
				Address2 = thisUser.Company.Address2,
				City = thisUser.Company.City,
				State = thisUser.Company.State,
				Zip = thisUser.Company.Zip,
				PhoneNumber = thisUser.Company.Phone,
				FaxNumber = thisUser.Company.FAX,
				WebSite = thisUser.Company.WebSite
			};
		}

		public bool UpdateSalesRepForDealer( int companyId, int psrContact )
		{
			var company = database.Companies.FirstOrDefault( c => c.CompanyID == companyId );

			if( company != null )
			{
				var user = database.Users.First( u => u.UserID == psrContact );

				if( user != null )
				{
					if( user.CompanyID == company.Territory.SalesRepCompanyID )
					{
						company.PaoliSalesRepMember = user;

						database.SaveChanges();

						return true;
					}
				}
			}

			return false;
		}

		public void UpdateImage( int companyID, Stream imgStream, string fileName )
		{
			var dbCompany = database.Companies.FirstOrDefault( c => c.CompanyID == companyID );

			if( dbCompany != null )
			{
				if( imgStream != null )
				{
					dbCompany.ImageFileName = Guid.NewGuid().ToString() + Path.GetExtension( fileName );

					Image fullSizeImg = Image.FromStream( imgStream );
					fullSizeImg.Save( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"],
						dbCompany.ImageFileName ) );

					database.SaveChanges();
				}
			}
		}

		public bool RequestCompanyDeactiviation( int reqUserId, int companyId, string reason )
		{
			var dbCompany = database.Companies.FirstOrDefault( c => c.CompanyID == companyId );
			var dbReqUser = database.Users.FirstOrDefault( u => u.UserID == reqUserId );

			if( dbCompany != null )
			{
				( new RequestDeactivationEmailSender() ).SubmitCompanyRequestEmail( dbReqUser.FullName, dbReqUser.Company.Name, dbReqUser.Email,
					companyId, dbCompany.Name, reason, dbCompany.MasterID, dbCompany.SubCompanyIDs );
			}

			return false;
		}

		public bool ImportTripTerritoryData( Stream fStream, List<string> errors )
		{
			var csvReader = new CsvReader( new StreamReader( fStream ), true );
			bool bRet = true;
			var companyIds = new List<int>();
			var columns = new List<string>() { "New Product Points", "Focus Dealer Points", "Current Territory Vol", "Territory Points", "# of Dealer Trips Awarded", "Dealer Trip Points", "Education Points", "Total Trip Points" };
			while( csvReader.ReadNextRecord() )
			{
				//Rep Territory #	Rep Territory Description	New Product Points	Focus Dealer Points	Current Territory Vol	Territory Points	# of Dealer Trips Awarded	Dealer Trip Points 	Education Points 	Total Trip Points 

				var territoryName = csvReader["Rep Territory #"];
				var companyName = csvReader["Rep Territory Description"];
				int territoryId = 0;
				int.TryParse( territoryName, out territoryId );

				var dbCompany = database.Companies.FirstOrDefault( c => c.TerritoryID == territoryId && c.Name == companyName && c.CompanyType == PaoliWebUser.PaoliCompanyType.PaoliRepGroup );

				if( dbCompany == null )
				{
					bRet = false;
					errors.Add( string.Format( "Unable to find company - Territory: {0} Name: {1}", territoryName, companyName ) );
					continue;
				}

				companyIds.Add( dbCompany.CompanyID );

				if( dbCompany.TerritoryTripData == null )
				{
					dbCompany.TerritoryTripData = new TerritoryTripData();
				}

				dbCompany.TerritoryTripData.ImportDate = DateTime.Now;

				foreach( var col in columns )
				{
					int? d = null;
					if( !GetIntValue( csvReader, col, out d ) )
					{
						bRet = false;
						errors.Add( string.Format( "Field does not contain valid data - Territory: {0} Name: {1} - {2}", territoryName, companyName, col ) );
					}
				}

				if( bRet )
				{
					dbCompany.TerritoryTripData.NewProductPoints = GetIntValue( csvReader, "New Product Points" ) ?? 0;
					dbCompany.TerritoryTripData.FocusDealerPoints = GetIntValue( csvReader, "Focus Dealer Points" ) ?? 0;
					dbCompany.TerritoryTripData.CurrentTerritoryVol = GetIntValue( csvReader, "Current Territory Vol" ) ?? 0;
					dbCompany.TerritoryTripData.TerritoryPoints = GetIntValue( csvReader, "Territory Points" ) ?? 0;
					dbCompany.TerritoryTripData.NumDealerTripsAwarded = GetIntValue( csvReader, "# of Dealer Trips Awarded" ) ?? 0;
					dbCompany.TerritoryTripData.DealerTripPoints = GetIntValue( csvReader, "Dealer Trip Points" ) ?? 0;
					dbCompany.TerritoryTripData.EducationPoints = GetIntValue( csvReader, "Education Points" ) ?? 0;
					dbCompany.TerritoryTripData.TotalTripPoints = GetIntValue( csvReader, "Total Trip Points" ) ?? 0;
				}
			}

			if( bRet )
			{
				foreach( var oldCompany in database.TerritoryTripDatas.Where( c => !companyIds.Contains( c.CompanyID ) ).ToList() )
				{
					database.TerritoryTripDatas.Remove( oldCompany );
				}

				database.SaveChanges();
			}

			return bRet;
		}

		public bool ImportTripData( Stream fStream, List<string> errors )
		{
			var csvReader = new CsvReader( new StreamReader( fStream ), true );
			bool bRet = true;
			var companyIds = new List<int>();

			var moneyValues = new List<string>() { "Total Sales YTD", "Sales To Next Trip", "Shaka - Threshold", "Shaka - Percent Growth YTD", "Shaka - Dollar Growth YTD", "Aloha - Threshold", "Aloha - Percent Growth YTD", "Aloha - Dollar Growth YTD", "Mahalo - Threshold", "Mahalo - Percent Growth YTD", "Mahalo - Dollar Growth YTD" };
			var rankValues = new List<string>() { "Total Trips YTD", "Shaka - Percent Growth Rank", "Shaka - Dollar Growth Rank", "Aloha - Percent Growth Rank", "Aloha - Dollar Growth Rank", "Mahalo - Percent Growth Rank", "Mahalo - Dollar Growth Rank" };

			while( csvReader.ReadNextRecord() )
			{
				// Dealership Name,Territory ID,Master Consolidated Number,Base Number,Trip Group,Total Sales YTD,Total Trips YTD,Sales To Next Trip,Shaka - Threshold,Shaka - Percent Growth YTD,Shaka - Percent Growth Rank,Shaka - Dollar Growth YTD,Shaka - Dollar Growth Rank,Aloha - Threshold,Aloha - Percent Growth YTD,Aloha - Percent Growth Rank,Aloha - Dollar Growth YTD,Aloha - Dollar Growth Rank,Mahalo - Threshold,Mahalo - Percent Growth YTD,Mahalo - Percent Growth Rank,Mahalo - Dollar Growth YTD,Mahalo - Dollar Growth Rank

				var masterNumber = csvReader["Master Consolidated Number"];
				var baseNumber = csvReader["Base Number"];

				var dbCompany = database.Companies.FirstOrDefault( c => c.MasterID == masterNumber && c.SubCompanyIDs == baseNumber );

				if( dbCompany == null )
				{
					bRet = false;
					errors.Add( string.Format( "Unable to find company - Master#: {0} Base#: {1}", masterNumber, baseNumber ) );
					continue;
				}

				companyIds.Add( dbCompany.CompanyID );

				if( !dbCompany.SignedUpForTrip )
				{
					bRet = false;
					errors.Add( string.Format( "Company is not signed up for Trip Group - Master#: {0} Base#: {1}", masterNumber, baseNumber ) );
					continue;
				}

				var tripGroup = csvReader["Trip Group"];
				if( dbCompany.TripGroup.ToLower() != tripGroup.ToLower() )
				{
					bRet = false;
					errors.Add( string.Format( "Trip Group does not match for company - Master#: {0} Base#: {1} - {2} <> {3}", masterNumber, baseNumber, dbCompany.TripGroup, tripGroup ) );
					continue;
				}

				if( dbCompany.CompanyTripData == null )
				{
					dbCompany.CompanyTripData = new CompanyTripData();
				}

				dbCompany.CompanyTripData.ImportDate = DateTime.Now;

				foreach( var money in moneyValues )
				{
					decimal? d = null;
					if( !GetDecimalValue( csvReader, money, out d ) )
					{
						bRet = false;
						errors.Add( string.Format( "Field does not contain valid data - Master#: {0} Base#: {1} - {2}", masterNumber, baseNumber, money ) );
					}
				}

				foreach( var rank in rankValues )
				{
					int? d = null;
					if( !GetIntValue( csvReader, rank, out d ) )
					{
						bRet = false;
						errors.Add( string.Format( "Field does not contain valid data - Master#: {0} Base#: {1} - {2}", masterNumber, baseNumber, rank ) );
					}
				}

				if( bRet )
				{
					dbCompany.CompanyTripData.TotalSalesYTD = GetDecimalValue( csvReader, "Total Sales YTD" );
					dbCompany.CompanyTripData.TotalTripsYTD = GetIntValue( csvReader, "Total Trips YTD" );
					dbCompany.CompanyTripData.SalesToNextTrip = GetDecimalValue( csvReader, "Sales To Next Trip" );

					switch( dbCompany.TripGroup.ToLower() )
					{
						case "ohana":
							break;
						case "shaka":
							dbCompany.CompanyTripData.ShakaThreshold = GetDecimalValue( csvReader, "Shaka - Threshold" );
							dbCompany.CompanyTripData.ShakaPercentGrowthYTD = GetDecimalValue( csvReader, "Shaka - Percent Growth YTD" );
							dbCompany.CompanyTripData.ShakaPercentGrowthRank = GetIntValue( csvReader, "Shaka - Percent Growth Rank" );
							dbCompany.CompanyTripData.ShakaDollarGrowthYTD = GetDecimalValue( csvReader, "Shaka - Dollar Growth YTD" );
							dbCompany.CompanyTripData.ShakaDollarGrowthRank = GetIntValue( csvReader, "Shaka - Dollar Growth Rank" );
							break;
						case "aloha":
							dbCompany.CompanyTripData.AlohaThreshold = GetDecimalValue( csvReader, "Aloha - Threshold" );
							dbCompany.CompanyTripData.AlohaPercentGrowthYTD = GetDecimalValue( csvReader, "Aloha - Percent Growth YTD" );
							dbCompany.CompanyTripData.AlohaPercentGrowthRank = GetIntValue( csvReader, "Aloha - Percent Growth Rank" );
							dbCompany.CompanyTripData.AlohaDollarGrowthYTD = GetDecimalValue( csvReader, "Aloha - Dollar Growth YTD" );
							dbCompany.CompanyTripData.AlohaDollarGrowthRank = GetIntValue( csvReader, "Aloha - Dollar Growth Rank" );
							break;
						case "mahalo":
							dbCompany.CompanyTripData.MahaloThreshold = GetDecimalValue( csvReader, "Mahalo - Threshold" );
							dbCompany.CompanyTripData.MahaloPercentGrowthYTD = GetDecimalValue( csvReader, "Mahalo - Percent Growth YTD" );
							dbCompany.CompanyTripData.MahaloPercentGrowthRank = GetIntValue( csvReader, "Mahalo - Percent Growth Rank" );
							dbCompany.CompanyTripData.MahaloDollarGrowthYTD = GetDecimalValue( csvReader, "Mahalo - Dollar Growth YTD" );
							dbCompany.CompanyTripData.MahaloDollarGrowthRank = GetIntValue( csvReader, "Mahalo - Dollar Growth Rank" );
							break;
					}
				}
			}

			if( bRet )
			{
				foreach( var oldCompany in database.CompanyTripDatas.Where( c => !companyIds.Contains( c.CompanyID ) ).ToList() )
				{
					database.CompanyTripDatas.Remove( oldCompany );
				}

				database.SaveChanges();
			}

			return bRet;
		}

		private decimal? GetDecimalValue( CsvReader csvReader, string column )
		{
			decimal? d = null;

			GetDecimalValue( csvReader, column, out d );

			return d;
		}

		private bool GetDecimalValue( CsvReader csvReader, string column, out decimal? retVal )
		{
			retVal = null;

			if( string.IsNullOrEmpty( csvReader[column] ) )
			{
				return true;
			}

			decimal d = 0.0M;

			if( decimal.TryParse( csvReader[column], out d ) )
			{
				retVal = d;
				return true;
			}

			return false;
		}

		private int? GetIntValue( CsvReader csvReader, string column )
		{
			int? d = null;

			GetIntValue( csvReader, column, out d );

			return d;
		}

		private bool GetIntValue( CsvReader csvReader, string column, out int? retVal )
		{
			retVal = null;

			if( string.IsNullOrEmpty( csvReader[column] ) )
			{
				return true;
			}

			int d = 0;

			if( int.TryParse( csvReader[column], out d ) )
			{
				retVal = d;
				return true;
			}

			return false;
		}

		private MyTripInfo GetCompanyTripInfo( Company company, bool canSeePerfData )
		{
			if( company.CompanyTripData == null )
			{
				return new MyTripInfo() { CanSeePerfData = false };
			}

			var tripInfo = new MyTripInfo()
			{
				CanSeePerfData = canSeePerfData,
				YTDSales = ( company.CompanyTripData.TotalSalesYTD ?? 0.0M ).ToString( "C0" ),
				TripEarned = company.CompanyTripData.TotalTripsYTD ?? 0,
				SalesToNextTrip = ( company.CompanyTripData.SalesToNextTrip ?? 0.0M ).ToString( "C0" ),

				TripGroup = company.TripGroup,
				TripGroupCount = database.Companies.Count( c => c.TripGroup == company.TripGroup ),

				ImportDate = company.CompanyTripData.ImportDate

			};

			switch( tripInfo.TripGroup )
			{
				case "Aloha":
					tripInfo.NetGrowth = ( company.CompanyTripData.AlohaDollarGrowthYTD ?? 0.0M ).ToString( "C0" );
					tripInfo.PercNetGrowth = ( ( company.CompanyTripData.AlohaPercentGrowthYTD ?? 0.0M ) / 100 ).ToString( "P" );
					tripInfo.NetGrowthRank = company.CompanyTripData.AlohaDollarGrowthRank ?? 0;
					tripInfo.PercNetGrowthRank = company.CompanyTripData.AlohaPercentGrowthRank ?? 0;
					break;
				case "Shaka":
					tripInfo.NetGrowth = ( company.CompanyTripData.ShakaDollarGrowthYTD ?? 0.0M ).ToString( "C0" );
					tripInfo.PercNetGrowth = ( ( company.CompanyTripData.ShakaPercentGrowthYTD ?? 0.0M ) / 100 ).ToString( "P" );
					tripInfo.NetGrowthRank = company.CompanyTripData.ShakaDollarGrowthRank ?? 0;
					tripInfo.PercNetGrowthRank = company.CompanyTripData.ShakaPercentGrowthRank ?? 0;
					break;
				case "Mahalo":
					tripInfo.NetGrowth = ( company.CompanyTripData.MahaloDollarGrowthYTD ?? 0.0M ).ToString( "C0" );
					tripInfo.PercNetGrowth = ( ( company.CompanyTripData.MahaloPercentGrowthYTD ?? 0.0M ) / 100 ).ToString( "P" );
					tripInfo.NetGrowthRank = company.CompanyTripData.MahaloDollarGrowthRank ?? 0;
					tripInfo.PercNetGrowthRank = company.CompanyTripData.MahaloPercentGrowthRank ?? 0;
					break;
			}

			return tripInfo;
		}

		public MyTripInfo GetCompanyTripInfo( int companyId )
		{
			var dbCompany = database.Companies.FirstOrDefault( u => u.CompanyID == companyId );

			if( dbCompany != null )
			{
				return GetCompanyTripInfo( dbCompany, true );
			}

			throw new NotImplementedException();
		}

		public MyTripInfo GetMyCompanyTripInfo( int userId )
		{
			var dbUser = database.Users.FirstOrDefault( u => u.UserID == userId );

			if( dbUser != null )
			{
				return GetCompanyTripInfo( dbUser.Company, dbUser.ViewPerfData );
			}

			throw new NotImplementedException();
		}

		public TerritoryTripInfo GetTerritoryTripInfo( int userId )
		{
			var dbUser = database.Users.FirstOrDefault( u => u.UserID == userId );

			if( dbUser != null )
			{
				if( dbUser.Company.TerritoryTripData != null )
				{
					return new TerritoryTripInfo()
					{
						TerritoryID = dbUser.Company.TerritoryID ?? 0,
						NewProductPoints = dbUser.Company.TerritoryTripData.NewProductPoints,
						FocusDealerPoints = dbUser.Company.TerritoryTripData.FocusDealerPoints,
						CurrentTerritoryVol = dbUser.Company.TerritoryTripData.CurrentTerritoryVol,
						TerritoryPoints = dbUser.Company.TerritoryTripData.TerritoryPoints,
						NumDealerTripsAwarded = dbUser.Company.TerritoryTripData.NumDealerTripsAwarded,
						DealerTripPoints = dbUser.Company.TerritoryTripData.DealerTripPoints,
						EducationPoints = dbUser.Company.TerritoryTripData.EducationPoints,
						TotalTripPoints = dbUser.Company.TerritoryTripData.TotalTripPoints,
						ImportDate = dbUser.Company.TerritoryTripData.ImportDate
					};
				}
			}

			throw new NotImplementedException();
		}

		public IEnumerable<DealerTripSummary> GetCompanyTripList( CompanyTableParams param, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var companyList = database.Companies
				.Where( c => c.TerritoryID == param.territoryId )
				.Where( c => c.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer )
				.Where( c => c.SignedUpForTrip && c.CompanyTripData != null )
				.AsQueryable();

			totalRecords = companyList.Count();
			displayedRecords = totalRecords;

			string sortCol = param.sColumns.Split( ',' )[param.iSortCol_0];

			IQueryable<Company> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
				case "name":
				default:
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = companyList.OrderBy( v => v.Name );
					}
					else
					{
						filteredAndSorted = companyList.OrderByDescending( v => v.Name );
					}
					break;
				case "groupname":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = companyList.OrderBy( v => v.TripGroup );
					}
					else
					{
						filteredAndSorted = companyList.OrderByDescending( v => v.TripGroup );
					}
					break;
				case "numtrips":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = companyList.OrderBy( v => v.CompanyTripData.TotalTripsYTD );
					}
					else
					{
						filteredAndSorted = companyList.OrderByDescending( v => v.CompanyTripData.TotalTripsYTD );
					}
					break;
				case "totalsales":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = companyList.OrderBy( v => v.CompanyTripData.TotalSalesYTD );
					}
					else
					{
						filteredAndSorted = companyList.OrderByDescending( v => v.CompanyTripData.TotalSalesYTD );
					}
					break;
			}

			return filteredAndSorted
				.Include( c => c.CompanyTripData )
				.ToList()
				.Select( v => new DealerTripSummary()
				{
					CompanyID = v.CompanyID,
					Name = v.Name,
					GroupName = v.TripGroup,
					NumTrips = v.CompanyTripData.TotalTripsYTD ?? 0,
					TotalSales = (v.CompanyTripData.TotalSalesYTD ?? 0.0M).ToString( "C" )
				} );
		}
	}
}
