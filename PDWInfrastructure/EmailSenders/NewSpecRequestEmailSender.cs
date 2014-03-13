using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure.EmailSenders
{
	public class NewSpecRequestEmailSender : EmailSender
	{
		public class EmailSpecRequestSummary
		{
			public int requestId { get; set; }
			public string requestName { get; set; }
			public string placingNameAndCompany { get; set; }
			public string territoryName { get; set; }
			public string firstName { get; set; }
			public string companyName { get; set; }
			public string projectName { get; set; }
			public string scopeDescription { get; set; }
			public List<string> seriesNames { get; set; }
			public List<string> servicesReqd { get; set; }
		}

		public NewSpecRequestEmailSender( string template )
		{
			TemplateName = template;
		}

		private string TemplateName { get; set; }

		public bool SubmitNewRequestEmail(
			string emailAddress,
			EmailSpecRequestSummary summary )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( TemplateName, out template ) )
				{
					// perform substitutions
					template.Replace( "[{RequestID}]", summary.requestId.ToString() );
					template.Replace( "[{RequestTNum}]", summary.requestName );
					template.Replace( "[{PlacingNameAndCompany}]", summary.placingNameAndCompany );
					template.Replace( "[{TerritoryName}]", summary.territoryName );

					template.Replace( "[{FirstName}]", summary.firstName );
					template.Replace( "[{CompanyName}]", summary.companyName );
					template.Replace( "[{ProjectName}]", summary.projectName );
					template.Replace( "[{ScopeDescription}]", summary.scopeDescription );
					template.Replace( "[{SeriesNames}]", string.Join( ", ", summary.seriesNames ) );
					template.Replace( "[{ServicesReqd}]", string.Join( ", ", summary.servicesReqd ) );

					return SubmitEmail( new List<string>() { emailAddress }, null, null, GetSubject( template ), template );
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
