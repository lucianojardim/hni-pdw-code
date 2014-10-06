using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWInfrastructure.EmailSenders;
using PDWDBContext;
using System.Configuration;

namespace PWDRepositories
{
	public abstract class BaseRepository
	{
		public EmailSender.EmailFromDetails GetPaoliMemberFromDetails( Company company )
		{
			if( company != null && company.PaoliMemberID.HasValue )
			{
				return new EmailSender.EmailFromDetails()
				{
					FromFirstName = company.PaoliMember.FirstName,
					FromFullName = company.PaoliMember.FullName,
					FromJobTitle = company.PaoliMember.Title,
					FromEmailAddress = company.PaoliMember.Email
				};
			}

			return new EmailSender.EmailFromDetails()
			{
				FromFirstName = ConfigurationManager.AppSettings["EmailFromFirstName"],
				FromFullName = ConfigurationManager.AppSettings["EmailFromFullName"],
				FromJobTitle = ConfigurationManager.AppSettings["EmailFromJobTitle"],
				FromEmailAddress = ConfigurationManager.AppSettings["EmailFromEmailAddress"]
			};
		}
	}
}
