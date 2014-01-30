﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWModels.Companies;
using PDWInfrastructure;
using PDWModels;

namespace PWDRepositories
{
	public class CompanyRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		public CompanyRepository()
		{
		}

		private CompanySummary ToCompanySummary( Company c )
		{
			return new CompanySummary()
			{
				CompanyID = c.CompanyID,
				Name = c.Name,
				CompanyType = PaoliWebUser.PaoliCompanyType.CompanyTypeList[c.CompanyType],
				MasterID = c.MasterID,
				CanDelete = !c.Users.Any() && !c.SpecRequests.Any() && !c.SpecRequests1.Any() && !c.CollateralOrders.Any() && !c.CollateralOrders1.Any()
			};
		}

		public bool AddCompany( CompanyInformation cInfo )
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
			newCompany.TripIncentive = cInfo.TripIncentive;
			newCompany.CompanyType = cInfo.CompanyType;
			newCompany.TerritoryID = PaoliWebUser.PaoliCompanyType.HasTerritory.Contains( cInfo.CompanyType ) ? cInfo.TerritoryID : null;
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

			database.Companies.AddObject( newCompany );

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
				TripIncentive = eCompany.TripIncentive,
				CompanyType = eCompany.CompanyType,
				TerritoryID = eCompany.TerritoryID,
				LockCompanyType = eCompany.Users.Any() || eCompany.SpecRequests.Any() || eCompany.SpecRequests1.Any() || eCompany.CollateralOrders.Any() || eCompany.CollateralOrders1.Any(),
				ContactEmail = eCompany.ContactEmail,
				WebSite = eCompany.WebSite,
				PublicContactEmail = eCompany.PublicContactEmail,
				PublicWebSite = eCompany.PublicWebSite,
				PublicDisplayName = eCompany.PublicDisplayName,

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

		public bool UpdateCompany( CompanyInformation cInfo )
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
			eCompany.TripIncentive = cInfo.TripIncentive;
			eCompany.CompanyType = cInfo.CompanyType;
			eCompany.TerritoryID = PaoliWebUser.PaoliCompanyType.HasTerritory.Contains( cInfo.CompanyType ) ? cInfo.TerritoryID : null;
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
				database.DeleteObject( eCompany.Showroom );
			}

			return database.SaveChanges() > 0;
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
				database.DeleteObject( eCompany.Showroom );
			}
			database.DeleteObject( eCompany );

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
					i.MasterID.Contains( param.sSearch ) );
			}
			if( param.companyType != 0 )
			{
				companyList = companyList.Where( c => c.CompanyType == param.companyType );
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
			}

			if( ( displayedRecords > param.iDisplayLength ) && ( param.iDisplayLength > 0 ) )
			{
				filteredAndSorted = filteredAndSorted.Skip( param.iDisplayStart ).Take( param.iDisplayLength );
			}

			return filteredAndSorted.ToList().Select( v => ToCompanySummary( v ) );
		}

		public IEnumerable<IDToTextItem> GetFullCompanyList( int? companyType )
		{
			return database.Companies
				.Where( c => !companyType.HasValue || ( companyType.HasValue && companyType.Value == c.CompanyType ) )
				.OrderBy( c => c.Name )
				.ToList()
				.Select( c => new IDToTextItem() { ID = c.CompanyID, Text = c.Name } );
		}

		public IEnumerable<TerritorySummary> GetTerritoryList()
		{
			return database.Territories
				.ToList()
				.Select( t => new TerritorySummary() { TerritoryID = t.TerritoryID, Name = t.Name } );
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

				Contacts = closestCompany.Users.Where( u => u.AccountType == PaoliWebUser.PaoliWebRole.PaoliSalesRep )
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

		public IEnumerable<IDToTextItem> GetDealerList( int salesRepCompanyId )
		{
			var salesRep = database.Companies.FirstOrDefault( c => c.CompanyID == salesRepCompanyId );
			if( salesRep != null )
			{
				return database.Companies
					.Where( c => c.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer && c.TerritoryID == salesRep.TerritoryID )
					.OrderBy( c => c.Name )
					.ToList()
					.Select( c => new IDToTextItem() { ID = c.CompanyID, Text = c.Name } );
			}

			return new List<IDToTextItem>();
		}

		public ShippingAddress GetCompanyAddress( int companyId, int? userId )
		{
			var eCompany = database.Companies.FirstOrDefault( u => u.CompanyID == companyId );
			if( eCompany == null )
			{
				throw new Exception( "Unable to find company." );
			}

			User eUser = eCompany.Users.FirstOrDefault( u => u.UserID == userId );

			return new ShippingAddress()
			{
				CompanyID = eCompany.CompanyID,
				ContactFirstName = eUser != null ? eUser.FirstName : "",
				ContactLastName = eUser != null ? eUser.LastName : "",
				Name = eCompany.Name,
				Address1 = eCompany.Address1,
				Address2 = eCompany.Address2,
				City = eCompany.City,
				State = eCompany.State,
				Zip = eCompany.Zip,
				Phone = eCompany.Phone,
				ContactEmail = eCompany.ContactEmail
			};
		}
	}
}
