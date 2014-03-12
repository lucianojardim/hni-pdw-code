using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure.EmailSenders
{
	public class ErrorEmailSender : EmailSender
	{
		public bool SubmitErrorEmail(
			Exception exp )
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

					return SubmitEmail( new List<string>() { "matt.james@wddsoftware.com" }, null, null, GetSubject( template ), template.ToString() );
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
