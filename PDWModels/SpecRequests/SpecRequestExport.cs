using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWModels.SpecRequests
{
	public class SpecRequestExport
	{
		public string Name { get; set; }

		public string DealerName { get; set; }
		public string DealerSalesRepName { get; set; }
		public string DealerSalesRepContact { get; set; }
		public string SalesRepGroupName { get; set; }
		public string SalesRepMemberName { get; set; }
		public string SalesRepMemberContact { get; set; }
		public string SpecTeamMemberName { get; set; }
		public string DealerPointOfContact { get; set; }
		public string DealerPointOfContactEmail { get; set; }
		public string DealerPointOfContactPhone { get; set; }
		public string DealerPointOfContactAcctTypeName { get; set; }

		public string EndCustomer { get; set; }
		public string ProjectName { get; set; }
		public string ProjectSize { get; set; }
		public bool IsGSA { get; set; }
		public string ContractName { get; set; }
		public bool AvailableForIn2 { get; set; }

		public bool NeedFloorplanSpecs { get; set; }
		public bool NeedValueEng { get; set; }
		public bool NeedPhotoRendering { get; set; }
		public bool Need2DDrawing { get; set; }
		public bool NeedAuditSpecs { get; set; }

		public string Notes { get; set; }
		public bool NeedDWGFiles { get; set; }
		public bool NeedSP4Files { get; set; }
		public bool NeedSIFFiles { get; set; }
		public bool NeedXLSFiles { get; set; }
		public bool NeedPDFFiles { get; set; }

		public string Casegoods { get; set; }
		public string Conferencing { get; set; }
		public string Seating { get; set; }
		public string Finishes { get; set; }
		public string LaminatePreference { get; set; }
		public string OtherFinishDetails { get; set; }
		public bool Grommets { get; set; }
		public string GrommetDetails { get; set; }
		public string GrommetPosition { get; set; }
		public string DrawerOption { get; set; }
		public string FabricGrade { get; set; }
		public string Fabric { get; set; }

		public string SpecialRequests { get; set; }

		public string SavedLocation { get; set; }
		public int? ListPrice { get; set; }
		public bool Received { get; set; }
		public bool IsCompleted { get; set; }
		public bool IsCanceled { get; set; }
		public bool IsOnHold { get; set; }
		public string SPLQuote { get; set; }
		public bool IsGoodForWeb { get; set; }
		public string Footprint { get; set; }
		public string SpecTeamNotes { get; set; }
		public string InternalNotes { get; set; }

		public string addlFileList { get; set; }
		public string specTeamFileList { get; set; }

		public DateTime? CreatedDate { get; set; }
		public string CreatedByUser { get; set; }
		public string CreatedByUserCompany { get; set; }
		public string CreatedByUserPhone { get; set; }
		public string CreatedByUserEmail { get; set; }

		public DateTime? CanceledDate { get; set; }
		public string CanceledByUser { get; set; }

	}
}
