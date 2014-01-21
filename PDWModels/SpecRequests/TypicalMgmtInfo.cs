using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace PDWModels.SpecRequests
{
	public class TypicalMgmtInfo
	{
		public int TypicalID { get; set; }
		public int RequestID { get; set; }
		public string Name { get; set; }

		[DisplayName( "List Price" )]
		[Required]
		public int ListPrice { get; set; }
		[DisplayName( "Additional Series" )]
		public string SeriesList { get; set; }
		[DisplayName( "Available for In2" )]
		public bool AvailableForIn2 { get; set; }
		[DisplayName( "Footprint" )]
		public string Footprint { get; set; }
		[DisplayName( "Featured Series" )]
		[StringLength( 50 )]
		public string FeaturedSeries { get; set; }
		[DisplayName( "Material" )]
		public string Material { get; set; }
		[DisplayName( "Finish" )]
		public string Finish { get; set; }
		[DisplayName( "Notes" )]
		public string Notes { get; set; }
		[DisplayName( "Isometric Image" )]
		public string RenderingImage { get; set; }
		[DisplayName( "Additional Images" )]
		public string AdditionalImages { get; set; }

		[DisplayName( "XLS File" )]
		public HttpPostedFileBase xlsFile { get; set; }
		[DisplayName( "SIF File" )]
		public HttpPostedFileBase sifFile { get; set; }
		[DisplayName( "SP4 File" )]
		public HttpPostedFileBase sp4File { get; set; }
		[DisplayName( "PDF File" )]
		public HttpPostedFileBase pdfFile { get; set; }
		[DisplayName( "DWG File" )]
		public HttpPostedFileBase dwgFile { get; set; }

		public string xlsFileName { get; set; }
		public string sifFileName { get; set; }
		public string sp4FileName { get; set; }
		public string pdfFileName { get; set; }
		public string dwgFileName { get; set; }
	}
}
