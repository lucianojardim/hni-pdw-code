using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure.EmailSenders
{
	public class CompletedSpecRequestEmailSender : EmailSender
	{
		public class EmailCompleteSpecRequestSummary
		{
			public class FileDetail
			{
				public string fileName { get; set; }
				public string filePath { get; set; }
			}

			public int requestId { get; set; }
			public string requestName { get; set; }
			public string specTeamMember { get; set; }
			public string firstName { get; set; }
			public string projectName { get; set; }
			public string specTeamNotes { get; set; }
			public List<FileDetail> fullFileList { get; set; }
		}

		public bool SubmitCompletedRequestEmail(
			string emailAddress,
			EmailCompleteSpecRequestSummary summary )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( "CompletedSpecRequest", out template ) )
				{
					// perform substitutions
					template.Replace( "[{RequestID}]", summary.requestId.ToString() );
					template.Replace( "[{RequestTNum}]", summary.requestName );
					template.Replace( "[{SpecTeamMember}]", summary.specTeamMember );
					template.Replace( "[{FirstName}]", summary.firstName );
					template.Replace( "[{ProjectName}]", summary.projectName );
					template.Replace( "[{SpecTeamNotes}]", summary.specTeamNotes );
					template.Replace( "[{FullFinalFileList}]", string.Join( "", summary.fullFileList
						.Select( f => string.Format( "<li><a href=\"http://my.paoli.com/Documents/Typicals/{1}/{0}\">{0}</a></li>",
							f.fileName, f.filePath ) ) ) );

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
