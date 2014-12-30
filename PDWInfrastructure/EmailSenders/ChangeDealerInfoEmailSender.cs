using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure.EmailSenders
{
	public class ChangeDealerInfoEmailSender : EmailSender
	{
		public bool SubmitRequestEmail(
			string requestingUserName,
			string requestingUserCompany,
			int companyID,
			string companyName,
			string address1,
			string address2,
			string city,
			string state,
			string zip,
			string phone,
			string fax,
			string contactEmail,
			string webSite )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( "ChangeDealerInfo", out template ) )
				{
					// perform substitutions
					template.Replace( "[{RequestingUser}]", requestingUserName );
					template.Replace( "[{RequestingUserCompany}]", requestingUserCompany );
					template.Replace( "[{CompanyID}]", companyID.ToString() );
					template.Replace( "[{CompanyName}]", companyName );
					template.Replace( "[{Address1}]", address1 );
					template.Replace( "[{Address2}]", address2 );
					template.Replace( "[{City}]", city );
					template.Replace( "[{State}]", state );
					template.Replace( "[{Zip}]", zip );
					template.Replace( "[{PhoneNumber}]", phone );
					template.Replace( "[{FaxNumber}]", fax );
					template.Replace( "[{ContactEmail}]", contactEmail );
					template.Replace( "[{WebSite}]", webSite );

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
