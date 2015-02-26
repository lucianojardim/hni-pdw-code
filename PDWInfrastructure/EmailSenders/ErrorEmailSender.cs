using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace PDWInfrastructure.EmailSenders
{
	public class ErrorEmailSender : EmailSender
	{
		public bool SubmitErrorEmail(
			Exception exp,
			string user,
			string url, 
			NameValueCollection formData )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( "ErrorEmail", out template ) )
				{
					string excList = "";
					Exception e = exp;
					while( e != null )
					{
						excList += ( "<li>" + e.Message + "</li>" );
						e = exp.InnerException;
					}
					template.Replace( "[{ExceptionList}]", excList );
					template.Replace( "[{StackTraceList}]", exp.StackTrace.Replace( "\n", "<br/>" ) );
					template.Replace( "[{VisitedURL}]", url );
					template.Replace( "[{CurrentUser}]", user );

					var formList = "";
					foreach( var key in formData )
					{
						if( key.ToString().ToLower() != "password" )
						{
							formList += string.Format( "<li>{0}: {1}</li>", key, string.Join( ", ", formData.GetValues( key.ToString() ) ) );
						}
					}
					if( !formList.Any() )
					{
						formList = "None";
					}

					template.Replace( "[{FormDataList}]", formList );

					return SubmitEmail( new List<string>() { "matt.james@wddsoftware.com" }, null, null, GetSubject( template ), template );
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
