using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure.Attributes;
using PWDRepositories;
using PDWModels.eCollateral;
using PDWInfrastructure;
using System.Configuration;
using System.IO;
using HiQPdf;
using PDWInfrastructure.EmailSenders;

namespace ProductDataWarehouse.Controllers
{
	public class ePublisherController : BaseController
    {
		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
        public ActionResult Manage()
        {
            return View();
        }

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult Add()
		{
			return View( new ECollateralSettings() { IsTemplate = PaoliWebUser.CurrentUser.CanAddECTemplate } );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Add( ECollateralSettings settings )
		{
			if( ModelState.IsValid )
			{
				using( var eRepository = new ECollateralRepository() )
				{
					int itemId = 0;
					eRepository.AddSettings( settings, PaoliWebUser.CurrentUser.UserId, out itemId );

					return RedirectToAction( "SetLayout", new { id = itemId } );
				}
			}

			return View( settings );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult EditSettings( int id )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				var settings = eRepository.GetItemSettings( id );

				settings.IsTemplate &= PaoliWebUser.CurrentUser.CanAddECTemplate;

				return View( settings );
			}
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditSettings( ECollateralSettings settings )
		{
			if( ModelState.IsValid )
			{
				using( var eRepository = new ECollateralRepository() )
				{
					eRepository.EditSettings( settings, PaoliWebUser.CurrentUser.UserId );

					return RedirectToAction( "EditLayout", new { id = settings.ItemID } );
				}
			}

			return View( settings );
		}

		public static IEnumerable<ECollateralRepository.Layouts.LayoutDetails> GetLayouts( int layoutType )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				return eRepository.GetLayoutSelectionDetails( layoutType );
			}
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult SetLayout( int id )
		{
			return View( new ECollateralLayout() { ItemID = id } );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SetLayout( ECollateralLayout layoutInfo )
		{
			if( ModelState.IsValid )
			{
				using( var eRepository = new ECollateralRepository() )
				{
					eRepository.SetLayout( layoutInfo, PaoliWebUser.CurrentUser.UserId );

					return RedirectToAction( "EditLayout", new { id = layoutInfo.ItemID } );
				}
			}

			return View( layoutInfo );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		public JsonResult GetImageList( string imgFilter )
		{
			using( var iRepository = new ImageRepository() )
			{
				return Json( iRepository.Search( imgFilter, true ),
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult EditLayout( int id )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				var details = eRepository.GetItemDetails( id );

				if( details == null )
				{
					return RedirectToAction( "SetLayout", new { id = id } );
				}

				return View( details );
			}
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditLayout( ECollateralDetails dInfo )
		{
			if( ModelState.IsValid )
			{
				using( var eRepository = new ECollateralRepository() )
				{
					bool bNeedVerify;
					eRepository.SetItemSections( dInfo, PaoliWebUser.CurrentUser.UserId, out bNeedVerify );

					return RedirectToAction( bNeedVerify ? "VerifyLayout" : "ViewLayout", new { id = dInfo.ItemID } );
				}
			}

			return View( dInfo );
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult VerifyLayout( int id )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				return View( eRepository.GetItemInformation( id ) );
			}
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult ConfirmLayout( int id )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				eRepository.ConfirmLayout( id, PaoliWebUser.CurrentUser.UserId );

				return RedirectToAction( "ViewLayout", new { id = id } );
			}
		}

		public ActionResult GeneratePDF( string id )
		{
			using( ECollateralRepository eRepository = new ECollateralRepository() )
			{
				ViewBag.BlankWrapper = true;
				return View( "~/Views/Info/Index.cshtml", eRepository.GetItemDetails( id ) );
			}
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public FileResult DownloadPDF( int id )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				var data = eRepository.GetItemInformation( id );
/*
				var pdfEXE = ConfigurationManager.AppSettings["ePubPDFTool"];
				var fileName = Path.Combine( ConfigurationManager.AppSettings["ePubPDFLocation"], Guid.NewGuid() + ".pdf" );
				var customUrl = MyPaoliURLLocal() + "/info/" + data.Settings.CustomURL;

				var process = System.Diagnostics.Process.Start( pdfEXE, string.Format( "\"{0}\" \"{1}\"", customUrl, fileName ) );

				while( !process.HasExited )
				{
					System.Threading.Thread.Sleep( 500 );
				}

				return File( fileName, "application/pdf", string.Format( data.Settings.CustomURL + ".pdf" ) );*/

				// create the HTML to PDF converter
				HtmlToPdf htmlToPdfConverter = new HtmlToPdf();

				htmlToPdfConverter.SerialNumber = "iMDh2djs-7sTh6vrp-+vGwprio-uai6qLC9-vai7uaa5-uqaxsbGx";

				// set PDF page size and orientation
				htmlToPdfConverter.Document.PageSize = PdfPageSize.Letter;
				htmlToPdfConverter.Document.PageOrientation = PdfPageOrientation.Portrait;
				htmlToPdfConverter.BrowserWidth = 1000;

				// set PDF page margins
				htmlToPdfConverter.Document.Margins = new PdfMargins( 10 );

				// enable header
				htmlToPdfConverter.Document.Header.Enabled = true;
				htmlToPdfConverter.Document.Header.Height = 25;

				// enable footer
				htmlToPdfConverter.Document.Footer.Enabled = true;
				
				// set footer height
				htmlToPdfConverter.Document.Footer.Height = 30;

				// set footer background color
				htmlToPdfConverter.Document.Footer.BackgroundColor = System.Drawing.Color.White;

				float pdfPageWidth = htmlToPdfConverter.Document.PageOrientation == PdfPageOrientation.Portrait ?
											htmlToPdfConverter.Document.PageSize.Width : htmlToPdfConverter.Document.PageSize.Height;

				float footerWidth = pdfPageWidth - htmlToPdfConverter.Document.Margins.Left - htmlToPdfConverter.Document.Margins.Right;
				float footerHeight = htmlToPdfConverter.Document.Footer.Height;

				// layout HTML in footer
/*				PdfHtml footerHtml = new PdfHtml( 5, 5, @"View this version online at [URL]", null );
				footerHtml.FitDestHeight = true;
				htmlToPdfConverter.Document.Footer.Layout( footerHtml );
				*/
				// add page numbering
				System.Drawing.Font pageNumberingFont = new System.Drawing.Font( new System.Drawing.FontFamily( "Helvetica" ),
											8, System.Drawing.GraphicsUnit.Pixel );

				PdfLine footerLine = new PdfLine( new System.Drawing.PointF( 0, 0 ), new System.Drawing.PointF( footerWidth - 1, 0 ) );
				footerLine.ForeColor = new PdfColor( 0xAF, 0xAF, 0xAF );
				footerLine.LineStyle.LineWidth = 1.0f;
				htmlToPdfConverter.Document.Footer.Layout( footerLine );

				var customUrl = MyPaoliURL() + "/info/" + data.Settings.CustomURL;
				PdfText urlFooterText = new PdfText( 25, footerHeight - 25, "View online at " + customUrl, pageNumberingFont );
				urlFooterText.HorizontalAlign = PdfTextHAlign.Left;
				urlFooterText.EmbedSystemFont = true;
				urlFooterText.ForeColor = new PdfColor( 102, 102, 102 );
				htmlToPdfConverter.Document.Footer.Layout( urlFooterText );

				PdfText pageNumberingText = new PdfText( 5, footerHeight - 25, "Page {CrtPage} of {PageCount}" + new string( ' ', 15 ), pageNumberingFont );
				pageNumberingText.HorizontalAlign = PdfTextHAlign.Right;
				pageNumberingText.EmbedSystemFont = true;
				pageNumberingText.ForeColor = new PdfColor( 102, 102, 102 );
				htmlToPdfConverter.Document.Footer.Layout( pageNumberingText );

				htmlToPdfConverter.PageCreatingEvent += new PdfPageCreatingDelegate( htmlToPdfConverter_PageCreatingEvent );

				// convert HTML to PDF
				byte[] pdfBuffer = null;

				// convert URL to a PDF memory buffer
				string url = MyPaoliURLLocal() + "/ePublisher/GeneratePDF/" + data.Settings.CustomURL;

				pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory( url );

				return File( pdfBuffer, "application/pdf", data.Settings.CustomURL + ".pdf" );
			}

		}

		private void htmlToPdfConverter_PageCreatingEvent( PdfPageCreatingParams eventParams )
		{
			PdfPage pdfPage = eventParams.PdfPage;
			int pdfPageNumber = eventParams.PdfPageNumber;

			if( pdfPageNumber == 1 )
			{
				pdfPage.DisplayHeader = false;
			}
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult ViewLayout( int id )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				return View( eRepository.GetItemInformation( id ) );
			}
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public JsonResult ShareViaEmail( ShareEPublisherEmailSender.ShareEPublisherDetails details )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				bool bSuccess = ( new ShareEPublisherEmailSender() ).SubmitShareEmail( details );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		[TempPasswordCheck]
		public ActionResult CopyLayout( int id )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				var newId = eRepository.CopyLayout( id, PaoliWebUser.CurrentUser.UserId );

				return RedirectToAction( "EditSettings", new { id = newId } );
			}
		}

		[PaoliAuthorize( "CanReviewECollateral" )]
		public JsonResult UpdateStatus( int itemId, int updateStatus )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				bool bSuccess = eRepository.UpdateStatus( itemId, updateStatus );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		public JsonResult ValidateURL( int itemId, string url )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				bool bSuccess = eRepository.ValidateURL( itemId, url );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		public JsonResult GetMyPagesList( int skipItems, string filterText )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				var theList = eRepository.GetMyPagesList( skipItems, filterText, PaoliWebUser.CurrentUser.UserId );

				return Json( theList,
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanManageECollateral" )]
		public JsonResult GetTemplateList( int skipItems, string filterText )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				var theList = eRepository.GetTemplateList( skipItems, filterText, PaoliWebUser.CurrentUser.CanAddECTemplate );

				return Json( theList,
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanReviewECollateral" )]
		public JsonResult GetReviewPagesList( int skipItems, string filterText )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				var theList = eRepository.GetReviewItemsList( skipItems, filterText );

				return Json( theList,
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanManageAllECollateral" )]
		public JsonResult GetAllPagesList( int skipItems, string filterText )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				var theList = eRepository.GetAllItemsList( skipItems, filterText );

				return Json( theList,
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanManageAllECollateral" )]
		public JsonResult Delete( int id )
		{
			using( var eRepository = new ECollateralRepository() )
			{
				bool bSuccess = eRepository.DeletePage( id );

				return Json( new
				{
					success = bSuccess
				},
					JsonRequestBehavior.AllowGet );
			}
		}
	}
}
