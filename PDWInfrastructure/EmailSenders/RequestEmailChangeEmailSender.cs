using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure.EmailSenders
{
	public class RequestEmailChangeEmailSender : EmailSender
	{
		public bool SubmitRequestEmail(
			string requestingUserName,
			string requestingUserCompany,
			string requestingUserEmail,
			int userId,
			string userName,
			string companyName,
			string newAddress )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( "RequestEmailChange", out template ) )
				{
					// perform substitutions
					template.Replace( "[{RequestingUser}]", requestingUserName );
					template.Replace( "[{RequestingUserCompany}]", requestingUserCompany );
					template.Replace( "[{RequestingUserEmail}]", requestingUserEmail );
					template.Replace( "[{UserID}]", userId.ToString() );
					template.Replace( "[{UserName}]", userName );
					template.Replace( "[{CompanyName}]", companyName );
					template.Replace( "[{NewAddress}]", newAddress );

					return SubmitEmail( new List<string>() { "mypaoli@paoli.com" }, null, null, GetSubject( template ), template );
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
