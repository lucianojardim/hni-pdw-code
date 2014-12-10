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
			string companyName,
			string reason )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( "RequestDeactivation", out template ) )
				{
					// perform substitutions
					template.Replace( "[{UserName}]", requestingUserName );
					template.Replace( "[{CompanyName}]", companyName );

					{
						string pattern = Regex.Escape( "[{ReasonTemplate}]" ) + "(.*?)" + Regex.Escape( "[{/ReasonTemplate}]" );
						var matching = Regex.Match( template.ToString(), pattern, RegexOptions.Multiline );

						string fullReason = string.Empty;
						if( matching.Success && ( reason ?? "" ).Any() )
						{
							fullReason = matching.Groups[1].ToString().Replace( "[{Reasons}]", reason );
						}
						template.Replace( "[{ReasonText}]", fullReason );
					}

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
