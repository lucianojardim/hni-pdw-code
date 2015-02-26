using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure.EmailSenders
{
	public class ShareEPublisherEmailSender : EmailSender
	{
		public class ShareEPublisherDetails
		{
			public string toList { get; set; }
			public string subject { get; set; }
			public string emailBody { get; set; }
		}

		public bool SubmitShareEmail( ShareEPublisherDetails details )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( "ShareEPublisher", out template ) )
				{
					// perform substitutions
					template.Replace( "[{BodyFromUser}]", System.Web.HttpUtility.HtmlDecode( details.emailBody ) );

					var bRet = true;
					foreach( var email in details.toList.Split( new[] { ',', ';', ' ' } ).Where( s => s.Any() ) )
					{
						bRet &= SubmitEmail( new List<string>() { email }, null, null, details.subject, template );
					}

					return bRet;
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
