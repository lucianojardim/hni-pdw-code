using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure.EmailSenders
{
	public class ChangeDealerUserInfoEmailSender : EmailSender
	{
		public bool SubmitRequestEmail(
			string requestingUserName,
			string requestingUserCompany,
			int userID,
			string companyName,

			string firstName,
			string lastName,
			string title,

			string businessAddress1,
			string businessAddress2,
			string businessCity,
			string businessState,
			string businessZip,
			string businessPhone,
			string cellPhone,
			string extension,
			string faxNumber,

			string homeAddress1,
			string homeAddress2,
			string homeCity,
			string homeState,
			string homeZip,
			string homePhone,
			string personalCellPhone )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( "ChangeDealerUserInfo", out template ) )
				{
					// perform substitutions
					template.Replace( "[{RequestingUser}]", requestingUserName );
					template.Replace( "[{RequestingUserCompany}]", requestingUserCompany );
					template.Replace( "[{UserID}]", userID.ToString() );
					template.Replace( "[{CompanyName}]", companyName );
					template.Replace( "[{FirstName}]", firstName );
					template.Replace( "[{LastName}]", lastName );
					template.Replace( "[{Title}]", title );
					template.Replace( "[{BusinessAddress1}]", businessAddress1 );
					template.Replace( "[{BusinessAddress2}]", businessAddress2 );
					template.Replace( "[{BusinessCity}]", businessCity );
					template.Replace( "[{BusinessState}]", businessState );
					template.Replace( "[{BusinessZip}]", businessZip );
					template.Replace( "[{BusinessPhone}]", businessPhone );
					template.Replace( "[{CellPhone}]", cellPhone );
					template.Replace( "[{Extension}]", extension );
					template.Replace( "[{FaxNumber}]", faxNumber );
					template.Replace( "[{HomeAddress1}]", homeAddress1 );
					template.Replace( "[{HomeAddress2}]", homeAddress2 );
					template.Replace( "[{HomeCity}]", homeCity );
					template.Replace( "[{HomeState}]", homeState );
					template.Replace( "[{HomeZip}]", homeZip );
					template.Replace( "[{HomePhone}]", homePhone );
					template.Replace( "[{PersonalCellPhone}]", personalCellPhone );

					return SubmitEmail( new List<string>() { "mypaoli@paoli.com", "paoli@getvitaminj.com" }, null, null, GetSubject( template ), template );
				}
			}
			catch( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine( "Unable to send email: {0}", ex.Message );
			}

			return false;
		}
	}
}
