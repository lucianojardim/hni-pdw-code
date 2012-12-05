using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PWDRepositories;

namespace ProductDataWarehouse.Controllers
{
    public class ImportController : Controller
    {
        //
        // GET: /Import/

        public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		public ActionResult Index( HttpPostedFileBase csvFile )
		{
			if( ModelState.IsValid )
			{
				try
				{
					ImportRepository iRepo = new ImportRepository();

					iRepo.ImportFileData( csvFile.InputStream, csvFile.ContentLength );
				}
				catch( Exception ex )
				{
					ModelState.AddModelError( "", ex.Message );
					if( ex.InnerException != null )
					{
						ModelState.AddModelError( "", ex.InnerException.Message );
					}
				}
			}

			return View();
		}

		[HttpPost]
		public ActionResult UploadFile( string imageName, HttpPostedFileBase imageFile, string caption, bool? hasPeople, string imageType, FormCollection theFields )
		{
			if( ModelState.IsValid )
			{
				try
				{
					ImportRepository iRepo = new ImportRepository();

					Dictionary<string, string> cropLocations = new Dictionary<string, string>();
					theFields.AllKeys.Where( k => GetImageRatioList().Select( r => r.Value ).Contains( k ) ).ToList().ForEach( cl => cropLocations.Add( cl, theFields[cl] ) );

					iRepo.ImportImageFileData( imageName, imageFile.InputStream, imageFile.ContentLength, imageFile.FileName,
						caption, hasPeople ?? false, imageType, cropLocations );

					return RedirectToAction( "Index" );
				}
				catch( Exception ex )
				{
					ModelState.AddModelError( "", ex.Message );
					if( ex.InnerException != null )
					{
						ModelState.AddModelError( "", ex.InnerException.Message );
					}
				}
			}
			return View( viewName: "Index" );
		}

		static public IEnumerable<SelectListItem> GetImageTypeList()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem() { Value = "Env", Text = "Environmental" },
				new SelectListItem() { Value = "WS", Text = "White Sweep" },
				new SelectListItem() { Value = "Det", Text = "Detail" },
				new SelectListItem() { Value = "ILD", Text = "Isometric Line Drawing" },
				new SelectListItem() { Value = "OFp", Text = "Overhead Footprint" },
			};
		}

		static public IEnumerable<SelectListItem> GetImageRatioList()
		{
			return ImportRepository.ImageSizes.Select( imgSize => new SelectListItem() { Text = imgSize.Description + ": ", Value = imgSize.Suffix } );
		}

		static public IEnumerable<SelectListItem> GetImageCropLocations()
		{
			return ImportRepository.CropLocations.Select( cl => new SelectListItem() { Text = cl, Value = cl } );
		}
	}

}
