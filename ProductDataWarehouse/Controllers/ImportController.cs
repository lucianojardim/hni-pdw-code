using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PWDRepositories;
using PDWInfrastructure;
using PDWModels.Images;

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
		public ActionResult Typicals( HttpPostedFileBase typicalFile )
		{
			if( ModelState.IsValid )
			{
				try
				{
					ImportRepository iRepo = new ImportRepository();

					iRepo.ImportTypicalFileData( typicalFile.InputStream, typicalFile.ContentLength );

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

		public ActionResult Images()
		{
			return View();
		}

		public JsonResult FullImageList( DataTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			ImageRepository iRepository = new ImageRepository();

			var results = iRepository.GetFullImageList( 
				param, out totalCount, out filteredCount );

			return Json( new
			{
				sEcho = param.sEcho,
				iTotalRecords = totalCount,
				iTotalDisplayRecords = filteredCount,
				aaData = results
			},
				JsonRequestBehavior.AllowGet );
		}

		public ActionResult AddImage()
		{
			return View( new ImageInformation() );
		}

		[HttpPost]
		public ActionResult AddImage( ImageInformation imgInfo, HttpPostedFileBase imageFile )
		{
			if( ModelState.IsValid )
			{
				try
				{
					ImageRepository iRepo = new ImageRepository();

					iRepo.ImportImageFileData( imgInfo, imageFile.InputStream, imageFile.ContentLength, imageFile.FileName, imageFile.ContentType );

					ViewBag.CloseFancyBox = true;

					return View();
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

		public ActionResult EditImage( int id )
		{
			ImageRepository iRepo = new ImageRepository();

			return View( iRepo.GetImageInformation( id ) );
		}

		[HttpPost]
		public ActionResult EditImage( ImageInformation imgInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					ImageRepository iRepo = new ImageRepository();

					iRepo.UpdateImageFile( imgInfo );

					ViewBag.CloseFancyBox = true;

					return View();
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

		public ActionResult UploadImage( int id )
		{
			ImageRepository iRepo = new ImageRepository();

			return View( iRepo.GetImageInformation( id ) );
		}

		[HttpPost]
		public ActionResult UploadImage( int ImageID, HttpPostedFileBase imageFile )
		{
			if( ModelState.IsValid )
			{
				try
				{
					ImageRepository iRepo = new ImageRepository();

					iRepo.UploadImageFile( ImageID, imageFile.InputStream, imageFile.ContentLength, imageFile.FileName, imageFile.ContentType );

					ViewBag.CloseFancyBox = true;

					return View();
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

		public ActionResult DeleteImage( int id )
		{
			ImageRepository iRepo = new ImageRepository();

			iRepo.DeleteImageFile( id );

			return RedirectToAction( "Images" );
		}

		static public IEnumerable<SelectListItem> GetImageTypeList()
		{
			return ImageRepository.ImageTypes.Select( it => new SelectListItem() { Value = it.Abbreviation, Text = it.Description } );
		}
	}

}
