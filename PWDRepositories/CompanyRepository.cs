using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWModels.Companies;
using PDWInfrastructure;

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
				CanDelete = !c.Users.Any() && !c.SpecRequests.Any() && !c.SpecRequests1.Any()
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
				MasterID = eCompany.MasterID,
				SubCompanyIDs = eCompany.SubCompanyIDs,
				TripIncentive = eCompany.TripIncentive,
				CompanyType = eCompany.CompanyType,
				TerritoryID = eCompany.TerritoryID,
				LockCompanyType = eCompany.Users.Any() || eCompany.SpecRequests.Any() || eCompany.SpecRequests1.Any()
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

			return database.SaveChanges() > 0;
		}

		public bool DeleteCompany( int id )
		{
			var eCompany = database.Companies.FirstOrDefault( u => u.CompanyID == id );
			if( eCompany == null )
			{
				throw new Exception( "Unable to find company." );
			}

			database.Companies.DeleteObject( eCompany );

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

		public IEnumerable<CompanySummary> GetFullCompanyList( int? companyType )
		{
			return database.Companies
				.Where( c => !companyType.HasValue || (companyType.HasValue && companyType.Value == c.CompanyType) )
				.OrderBy( c => c.Name )
				.ToList()
				.Select( c => ToCompanySummary( c ) );
		}

		public IEnumerable<TerritorySummary> GetTerritoryList()
		{
			return database.Territories
				.ToList()
				.Select( t => new TerritorySummary() { TerritoryID = t.TerritoryID, Name = t.Name } );
		}
	}
}
