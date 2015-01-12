using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace PDWModels.SpecRequests
{
	public class ProjectInformation
	{
		public int ProjectID { get; set; }
		[Required]
		public string ProjectName { get; set; }
		[Required]
		public int DealerID { get; set; }
		public string EndCustomer { get; set; }

		public bool IsGSA { get; set; }
		public int? ContractID { get; set; }

		public bool HasADFirm { get; set; }
		public string ADFirm { get; set; }

		public DateTime? AnticipatedOrderDate { get; set; }
		public DateTime? AnticipatedShipDate { get; set; }

		public List<string> VeneerCasegoods { get; set; }
		public double? NetVeneerCasegoods { get; set; }
		public List<string> LaminateCasegoods { get; set; }
		public double? NetLaminateCasegoods { get; set; }
		public List<string> Conferencing { get; set; }
		public double? NetConferencing { get; set; }
		public List<string> Seating { get; set; }
		public double? NetSeating { get; set; }

		public bool SPADone { get; set; }
		public int? Probability { get; set; }

		public string PrimaryCompetitor { get; set; }
		public string Comments { get; set; }

		public int PipelineStatus { get; set; }
		public int ProjectSuccess { get; set; }
	}
}
