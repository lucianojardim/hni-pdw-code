using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWInfrastructure;
using PDWModels.Users;
using System.IO;
using System.Drawing;
using System.Configuration;
using PDWModels;
using PDWInfrastructure.EmailSenders;

namespace PWDRepositories
{
    public class UserRepository
    {
		private PaoliPDWEntities database = new PaoliPDWEntities();

        public UserRepository()
		{
		}

		private UserSummary ToUserSummary( User u )
		{
			return new UserSummary()
			{
				UserID = u.UserID,
				FirstName = u.FirstName,
				LastName = u.LastName,
				CompanyName = u.Company.FullName,
				EmailAddress = u.Email,
				PhoneNumber = u.BusinessPhone,
				Enabled = u.Enabled,
				SentWelcomeEmail = u.RecWelcomeEmail
			};
		}

        public PaoliWebUser CreatePaoliWebUser(string email, string authType)
        {
			User user = database.Users.FirstOrDefault( u => u.Email == email );
			if( user != null )
			{
				return new PaoliWebUser( user.Email, authType, user.UserID, user.FullName, user.AccountType, user.IsTempPassword );
			}

			return null;
		}

		public bool ValidateUserAccount( string userName, string password, out string homePage )
		{
			homePage = "";

			var user = database.Users
				.FirstOrDefault( u => u.Email == userName && u.Enabled );

			PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
			if( user == null )
			{
				if( !database.Users.Any() )
				{
					user = new User();
					user.Email = "matt.james@wddsoftware.com";
					user.FirstName = "Matt";
					user.LastName = "James";
					user.Password = enc.Encrypt( "Password!" );
					user.AccountType = PaoliWebUser.PaoliWebRole.SuperAdmin;
					user.CompanyID = database.Companies.First( c => c.CompanyType == PaoliWebUser.PaoliCompanyType.Paoli ).CompanyID;
					database.AddToUsers( user );

					if( database.SaveChanges() == 0 )
					{
						return false;
					}
					else if( userName != user.Email )
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}

			bool bValid = ( string.Compare( enc.Decrypt( user.Password ), password, false ) == 0 );

			homePage = PaoliWebUser.PaoliWebRole.RoleHomePage( user.AccountType );

			return bValid;
		}

		private string GenerateNewPassword()
		{
			string characterList = "bcdfghjkmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ23456789!@#$%^&*()";

			string retVal = string.Empty;
			do
			{
				retVal = string.Empty;
				Random rdm = new Random();
				while( retVal.Length < 10 )
				{
					retVal += characterList[rdm.Next( characterList.Length - 1 )];
				}
			} while( !ChangePWDetail.IsGoodPassword( retVal ) );

			return retVal;
		}

		public bool AddUser( UserInformation uInfo, Stream imgStream, string fileName )
		{
			if( database.Users.Any( u => u.Email == uInfo.EmailAddress ) )
			{
				throw new Exception( "Unable to add user - Email address already exists." );
			}

			User newUser = new User();

			newUser.Email = uInfo.EmailAddress;
			newUser.FirstName = uInfo.FirstName;
			newUser.LastName = uInfo.LastName;
			newUser.CompanyID = uInfo.CompanyID;
			newUser.Address1 = uInfo.Address1;
			newUser.Address2 = uInfo.Address2;
			newUser.City = uInfo.City;
			newUser.State = uInfo.State;
			newUser.Zip = uInfo.Zip;
			newUser.BusinessPhone = uInfo.BusinessPhone;
			newUser.CellPhone = uInfo.CellPhone;
			newUser.Title = uInfo.Title;
			newUser.AccountType = uInfo.AccountType;
			newUser.Enabled = uInfo.Enabled && uInfo.SendWelcomeEmail;
			newUser.RecWelcomeEmail = uInfo.SendWelcomeEmail;

			string password = GenerateNewPassword();
			PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
			newUser.Password = enc.Encrypt( password );
			newUser.IsTempPassword = true;

			if( imgStream != null )
			{
				newUser.ImageFileName = Guid.NewGuid().ToString() + Path.GetExtension( fileName );

				Image fullSizeImg = Image.FromStream( imgStream );
				fullSizeImg.Save( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"],
					newUser.ImageFileName ) );
			}

			database.Users.AddObject( newUser );

			if( database.SaveChanges() > 0 )
			{
				if( uInfo.SendWelcomeEmail )
				{
					( new NewAccountEmailSender() ).SubmitNewAccountEmail( newUser.Email, password );
				}

				return true;
			}

			return false;
		}

		public UserInformation GetUser( int id )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == id );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			return new UserInformation()
			{
				UserID = eUser.UserID,
				EmailAddress = eUser.Email,
				FirstName = eUser.FirstName,
				LastName = eUser.LastName,
				CompanyID = eUser.CompanyID,
				CompanyName = eUser.Company.FullName,
				Address1 = eUser.Address1,
				Address2 = eUser.Address2,
				City = eUser.City,
				State = eUser.State,
				Zip = eUser.Zip,
				BusinessPhone = eUser.BusinessPhone,
				CellPhone = eUser.CellPhone,
				Title = eUser.Title,
				AccountType = eUser.AccountType,
				Enabled = eUser.Enabled,
				SendWelcomeEmail = eUser.RecWelcomeEmail,
				LockAccountType = eUser.SpecRequests.Any() || eUser.SpecRequests1.Any(),
				UserImageFileName = eUser.ImageFileName
			};
		}

		public bool UpdateUser( MyAccountInfo uInfo, Stream imgStream = null, string fileName = null )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == uInfo.UserID );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			if( database.Users.Any( u => u.Email == uInfo.EmailAddress && u.UserID != uInfo.UserID ) )
			{
				throw new Exception( "Unable to update user - Email address already exists." );
			}

			bool bChangeEmail = false, bWelcomeEmail = false;
			if( eUser.Email == PaoliWebUser.CurrentUser.EmailAddress )
			{
				bChangeEmail = (eUser.Email != uInfo.EmailAddress);
			}

			eUser.Email = uInfo.EmailAddress;
			eUser.FirstName = uInfo.FirstName;
			eUser.LastName = uInfo.LastName;
			eUser.Address1 = uInfo.Address1;
			eUser.Address2 = uInfo.Address2;
			eUser.City = uInfo.City;
			eUser.State = uInfo.State;
			eUser.Zip = uInfo.Zip;
			eUser.BusinessPhone = uInfo.BusinessPhone;
			eUser.CellPhone = uInfo.CellPhone;
			eUser.Title = uInfo.Title;

			string password = GenerateNewPassword();
			if( uInfo is UserInformation )
			{
				eUser.AccountType = ( uInfo as UserInformation ).AccountType;
				eUser.Enabled = ( uInfo as UserInformation ).Enabled;
				eUser.CompanyID = ( uInfo as UserInformation ).CompanyID;
				if( !eUser.RecWelcomeEmail && ( uInfo as UserInformation ).SendWelcomeEmail )
				{
					bWelcomeEmail = true;
					PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
					eUser.Password = enc.Encrypt( password );
					eUser.IsTempPassword = true;
				}

				if( imgStream != null )
				{
					eUser.ImageFileName = Guid.NewGuid().ToString() + Path.GetExtension( fileName );

					Image fullSizeImg = Image.FromStream( imgStream );
					fullSizeImg.Save( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"],
						eUser.ImageFileName ) );
				}
			}

			if( database.SaveChanges() > 0 )
			{
				if( bChangeEmail )
				{
					System.Web.Security.FormsAuthentication.SetAuthCookie( uInfo.EmailAddress, false );
				}
				if( bWelcomeEmail )
				{
					( new NewAccountEmailSender() ).SubmitNewAccountEmail( eUser.Email, password );
				}

				return true;
			}

			return false;
		}

		public bool SendWelcomeEmail( int id )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == id );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			string password = GenerateNewPassword();
			PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
			eUser.Password = enc.Encrypt( password );
			eUser.IsTempPassword = true;
			eUser.RecWelcomeEmail = true;
			eUser.Enabled = true;

			if( database.SaveChanges() > 0 )
			{
				( new NewAccountEmailSender() ).SubmitNewAccountEmail( eUser.Email, password );

				return true;
			}

			return false;
		}

		public bool ResetUserPassword( string email )
		{
			var eUser = database.Users.FirstOrDefault( u => u.Email == email && u.Enabled );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			return ResetUserPassword( eUser );
		}

		public bool ResetUserPassword( int id )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == id );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			return ResetUserPassword( eUser );
		}

		private bool ResetUserPassword( User eUser )
		{
			string password = GenerateNewPassword();
			PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
			eUser.Password = enc.Encrypt( password );
			eUser.IsTempPassword = true;

			if( database.SaveChanges() > 0 )
			{
				( new ResetPasswordEmailSender() ).SubmitResetPasswordEmail( eUser.Email, password );

				return true;
			}

			return false;
		}

		public IEnumerable<UserSummary> GetFullUserList( UserTableParams param, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var userList = database.Users.AsQueryable();

			totalRecords = userList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
				userList = userList.Where( i =>
					i.FirstName.Contains( param.sSearch ) ||
					i.LastName.Contains( param.sSearch ) ||
					i.Email.Contains( param.sSearch ) ||
					i.Company.Name.Contains( param.sSearch ) ||
					i.Company.BusinessUnitName.Contains( param.sSearch ) );
			}
			if( param.accountType != 0 )
			{
				userList = userList.Where( u => u.AccountType == param.accountType );
			}
			if( !PaoliWebUser.CurrentUser.IsInRole( PaoliWebUser.PaoliWebRole.SuperAdmin ) )
			{
				userList = userList.Where( i => i.AccountType != PaoliWebUser.PaoliWebRole.SuperAdmin );
			}
			if( param.territoryId.HasValue )
			{
				userList = userList.Where( c => c.Company.TerritoryID == param.territoryId.Value );
			}

			displayedRecords = userList.Count();

			string sortCol = param.sColumns.Split( ',' )[param.iSortCol_0];

			IQueryable<User> filteredAndSorted = null;
			switch( sortCol.ToLower() )
			{
				case "firstname":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = userList.OrderBy( v => v.FirstName );
					}
					else
					{
						filteredAndSorted = userList.OrderByDescending( v => v.FirstName );
					}
					break;
				case "lastname":
				default:
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = userList.OrderBy( v => v.LastName );
					}
					else
					{
						filteredAndSorted = userList.OrderByDescending( v => v.LastName );
					}
					break;
				case "emailaddress":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = userList.OrderBy( v => v.Email );
					}
					else
					{
						filteredAndSorted = userList.OrderByDescending( v => v.Email );
					}
					break;
				case "companyname":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						filteredAndSorted = userList.OrderBy( v => v.Company.Name );
					}
					else
					{
						filteredAndSorted = userList.OrderByDescending( v => v.Company.Name );
					}
					break;
			}

			if( ( displayedRecords > param.iDisplayLength ) && ( param.iDisplayLength > 0 ) )
			{
				filteredAndSorted = filteredAndSorted.Skip( param.iDisplayStart ).Take( param.iDisplayLength );
			}

			return filteredAndSorted.ToList().Select( v => ToUserSummary( v ) );
		}

		public bool ChangePassword( ChangePWDetail pwDetail )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == PaoliWebUser.CurrentUser.UserId );
			if( eUser == null )
			{
				throw new Exception( "Unable to find user." );
			}

			PaoliEncryption enc = new PaoliEncryption( PaoliEncryption.DataPassPhrase );
			if( string.Compare( enc.Decrypt( eUser.Password ), pwDetail.OldPassword, false ) == 0 )
			{
				eUser.Password = enc.Encrypt( pwDetail.NewPassword );
				eUser.IsTempPassword = false;

				return database.SaveChanges() > 0;
			}

			throw new ApplicationException( "Current password does not match our records." );
		}

		public IEnumerable<UserSummary> GetUserListForAccountType( List<int> accountTypes, bool enabledOnly )
		{
			return database.Users
				.Where( u => accountTypes.Contains( u.AccountType ) || !accountTypes.Any() || accountTypes.Contains( 0 ) )
				.Where( u => u.Enabled || !enabledOnly )
				.ToList()
				.OrderBy( u => u.FullName )
				.Select( v => ToUserSummary( v ) );
		}

		public IEnumerable<UserSummary> GetUserListForAccountType( int companyId, int accountType, bool enabledOnly )
		{
			return database.Users
				.Where( u => u.CompanyID == companyId )
				.Where( u => u.AccountType == accountType || accountType == 0 )
				.Where( u => u.Enabled || !enabledOnly )
				.ToList()
				.OrderBy( u => u.FullName )
				.Select( v => ToUserSummary( v ) );
		}

		public UserSubscriptionSummary GetUserSubscriptionSummary( int userId )
		{
			var eUser = database.UserSubscriptions.FirstOrDefault( u => u.UserID == userId );
			if( eUser == null )
			{
				return new UserSubscriptionSummary()
				{
					UserID = userId
				};
			}

			return new UserSubscriptionSummary()
			{
				UserID = userId,
				ProductIntroductions = eUser.ProductIntroductions,
				BehindTheScenes = eUser.BehindTheScenes,
				MeetOurMembers = eUser.MeetOurMembers,
				ProgramChanges = eUser.ProgramChanges,
				PricelistUpdates = eUser.PricelistUpdates,
				QuoteRequests = eUser.QuoteRequests,
				SMSAlerts = eUser.SMSAlerts,
				SMSPhoneNumber = eUser.SMSAlerts ? eUser.SMSPhoneNumber : eUser.User.CellPhone
			};
		}

		public void UpdateUserSubscriptions( UserSubscriptionSummary uSummary )
		{
			var eUser = database.UserSubscriptions.FirstOrDefault( u => u.UserID == uSummary.UserID );
			if( eUser == null )
			{
				eUser = new UserSubscription();
				eUser.UserID = uSummary.UserID;
				database.UserSubscriptions.AddObject( eUser );
			}

			if( uSummary.SMSAlerts && uSummary.SMSPhoneNumber == null )
			{
				throw new ApplicationException( "Please provide a phone number in order to receive SMS Alerts" );
			}

			eUser.ProductIntroductions = uSummary.ProductIntroductions;
			eUser.BehindTheScenes = uSummary.BehindTheScenes;
			eUser.MeetOurMembers = uSummary.MeetOurMembers;
			eUser.ProgramChanges = uSummary.ProgramChanges;
			eUser.PricelistUpdates = uSummary.PricelistUpdates;
			eUser.QuoteRequests = uSummary.QuoteRequests;
			eUser.SMSAlerts = uSummary.SMSAlerts;
			eUser.SMSPhoneNumber = uSummary.SMSAlerts ? uSummary.SMSPhoneNumber : null;

			database.SaveChanges();
		}

		public IEnumerable<UserContactInfo> GetPaoliRepContacts( int userId )
		{
			var theList = new List<UserContactInfo>();

			var eUser = database.Users.FirstOrDefault( u => u.UserID == userId );
			if( eUser != null )
			{
				if( eUser.Company.CompanyType == PaoliWebUser.PaoliCompanyType.Dealer )
				{
					if( eUser.Company.Territory != null )
					{
						theList = eUser.Company
							.Territory
							.Companies
							.Where( c => c.CompanyType == PaoliWebUser.PaoliCompanyType.PaoliRepGroup )
							.SelectMany( c => c.Users )
							.ToList()
							.Select( u => new UserContactInfo() { FullName = u.FullName, EmailAddress = u.Email, PhoneNumber = u.BusinessPhone, CompanyName = u.Company.Name } )
							.ToList();
					}
				}
				else if( eUser.AccountType == PaoliWebUser.PaoliWebRole.PaoliSalesRep )
				{
					theList = new List<UserContactInfo>() { 
						new UserContactInfo() { FullName = "Kevin Street", EmailAddress = "kevin-street@paoli.com", PhoneNumber = "812-865-7135", CompanyName = null },
						new UserContactInfo() { FullName = "Steve Smith", EmailAddress = "steve-smith@paoli.com", PhoneNumber = "812-865-7014", CompanyName = null }
					};
				}
			}

			return theList;
			
		}

		public UserContactInfo GetPaoliMemberContact( int userId )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == userId );
			if( eUser != null )
			{
				if( eUser.Company.PaoliMember != null )
				{
					return new UserContactInfo()
					{
						FullName = eUser.Company.PaoliMember.FullName,
						EmailAddress = eUser.Company.PaoliMember.Email,
						PhoneNumber = eUser.Company.PaoliMember.BusinessPhone,
						CompanyName = eUser.Company.PaoliMember.Company.Name
					};
				}
			}

			return null;

		}

		public PDWModels.Companies.ShippingAddress GetUserAddress( int userId )
		{
			var eUser = database.Users.FirstOrDefault( u => u.UserID == userId );
			if( eUser == null )
			{
				return new PDWModels.Companies.ShippingAddress();
			}

			return new PDWModels.Companies.ShippingAddress()
			{
				CompanyID = eUser.CompanyID,
				ContactAttn = eUser.FullName,
				Name = eUser.Company.Name,
				Address1 = eUser.Address1,
				Address2 = eUser.Address2,
				City = eUser.City,
				State = eUser.State,
				Zip = eUser.Zip,
				Phone = eUser.BusinessPhone,
				ContactEmail = eUser.Email
			};
		}

		public IDToTextItem GetCurrentCompanyInfo()
		{
			var user = database.Users.FirstOrDefault( u => u.UserID == PaoliWebUser.CurrentUser.UserId );
			if( user != null )
			{
				return new IDToTextItem() { ID = user.Company.CompanyID, Text = user.Company.FullName };
			}

			throw new Exception( "Unable to find current user" );
		}
	}
}
