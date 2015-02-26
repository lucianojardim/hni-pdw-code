using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PDWInfrastructure.EmailSenders
{
	public class RequestDeactivationEmailSender : EmailSender
	{
		public bool SubmitRequestEmail(
			string requestingUserName,
			string requestingUserCompany,
			string requestingUserEmail,
			int userId,
			string userName,
			string companyName,
			string reason )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( "RequestDeactivation", out template ) )
				{
					// perform substitutions
					template.Replace( "[{RequestingUser}]", requestingUserName );
					template.Replace( "[{RequestingUserCompany}]", requestingUserCompany );
					template.Replace( "[{RequestingUserEmail}]", requestingUserEmail );
					template.Replace( "[{UserID}]", userId.ToString() );
					template.Replace( "[{UserName}]", userName );
					template.Replace( "[{CompanyName}]", companyName );
					template.Replace( "[{Reasons}]", reason );

					return SubmitEmail( new List<string>() { "mypaoli@paoli.com", "paoli@getvitaminj.com" }, null, null, GetSubject( template ), template );
				}
			}
			catch( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine( "Unable to send email: {0}", ex.Message );
			}

			return false;
		}

		public bool SubmitCompanyRequestEmail(
			string requestingUserName,
			string requestingUserCompany,
			string requestingUserEmail,
			int companyId,
			string companyName,
			string reason,
			string masterNumber,
			string baseNumber )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( "RequestCompanyDeactivation", out template ) )
				{
					// perform substitutions
					template.Replace( "[{RequestingUser}]", requestingUserName );
					template.Replace( "[{RequestingUserCompany}]", requestingUserCompany );
					template.Replace( "[{RequestingUserEmail}]", requestingUserEmail );
					template.Replace( "[{CompanyID}]", companyId.ToString() );
					template.Replace( "[{CompanyName}]", companyName );
					template.Replace( "[{Reasons}]", reason );
					template.Replace( "[{MasterNumber}]", masterNumber );
					template.Replace( "[{BaseNumber}]", baseNumber );

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
