using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PWDRepositories;
using PDWInfrastructure;
using PDWModels.Images;
using System.Threading;
using System.Web.Security;

namespace ProductDataWarehouse.Controllers
{
    public class ImportController : Controller
    {
		public ActionResult Logon()
		{
			if( HttpContext.User.Identity.IsAuthenticated )
			{
				return RedirectToAction( "Index" );
			}

			return View();
		}

		[HttpPost]
		public ActionResult Logon( string userName, string password )
		{
			if( ModelState.IsValid )
			{
				if( userName == "paoliUser" &&
					password == "Paol!P@$$" )
				{
					FormsAuthentication.SetAuthCookie( userName, false );
					return RedirectToAction( "Index" );
				}
				Thread.Sleep( 5000 );	// delay to prevent brute force attacks
				ModelState.AddModelError( "", "Invalid username and/or password." );
			}

			return View();
		}

		[Authorize]
        public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		[Authorize]
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
		[Authorize]
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

		[Authorize]
		public ActionResult Images()
		{
			return View();
		}

		[Authorize]
		public JsonResult FullImageList( ImageTableParams param )
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

		[Authorize]
		public ActionResult AddImage()
		{
			return View( new ImageInformation() { ImageContent = 1 } );
		}

		[HttpPost]
		[Authorize]
		public ActionResult AddImage( ImageInformation imgInfo, HttpPostedFileBase imageFile )
		{
			switch( (ImageInformation.ImageContents)imgInfo.ImageContent )
			{
				case ImageInformation.ImageContents.Image:
					imgInfo.FeaturedEdge = null;
					imgInfo.FeaturedPull = null;
					break;
				case ImageInformation.ImageContents.Edge:
					if( imgInfo.FeaturedEdge == null )
					{
						ModelState.AddModelError( "FPRequired", "Featured Edge is required." );
					}
					else
					{
						imgInfo.ImageType = "Det";
						imgInfo.FeaturedPull = null;
						imgInfo.HasPeople = false;
					}
					break;
				case ImageInformation.ImageContents.Pull:
					if( imgInfo.FeaturedPull == null )
					{
						ModelState.AddModelError( "FPRequired", "Featured Pull is required." );
					}
					else
					{
						imgInfo.ImageType = "Det";
						imgInfo.FeaturedEdge = null;
						imgInfo.HasPeople = false;
					}
					break;
				case ImageInformation.ImageContents.Finish:
					break;
			}

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

		[Authorize]
		public ActionResult EditImage( int id )
		{
			ImageRepository iRepo = new ImageRepository();

			return View( iRepo.GetImageInformation( id ) );
		}

		[HttpPost]
		[Authorize]
		public ActionResult EditImage( ImageInformation imgInfo )
		{
			switch( (ImageInformation.ImageContents)imgInfo.ImageContent )
			{
				case ImageInformation.ImageContents.Image:
					imgInfo.FeaturedEdge = null;
					imgInfo.FeaturedPull = null;
					break;
				case ImageInformation.ImageContents.Edge:
					if( imgInfo.FeaturedEdge == null )
					{
						ModelState.AddModelError( "FPRequired", "Featured Edge is required." );
					}
					else
					{
						imgInfo.ImageType = "Det";
						imgInfo.FeaturedPull = null;
						imgInfo.HasPeople = false;
					}
					break;
				case ImageInformation.ImageContents.Pull:
					if( imgInfo.FeaturedPull == null )
					{
						ModelState.AddModelError( "FPRequired", "Featured Pull is required." );
					}
					else
					{
						imgInfo.ImageType = "Det";
						imgInfo.FeaturedEdge = null;
						imgInfo.HasPeople = false;
					}
					break;
				case ImageInformation.ImageContents.Finish:
					break;
			}

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

		[Authorize]
		public ActionResult UploadImage( int id )
		{
			ImageRepository iRepo = new ImageRepository();

			return View( iRepo.GetImageInformation( id ) );
		}

		[HttpPost]
		[Authorize]
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

		[Authorize]
		public ActionResult DeleteImage( int id )
		{
			ImageRepository iRepo = new ImageRepository();

			iRepo.DeleteImageFile( id );

			return RedirectToAction( "Images" );
		}

		static public IEnumerable<SelectListItem> GetImageContentList( bool bIncludeAll )
		{
			var theList = new List<SelectListItem>();

			if( bIncludeAll )
			{
				theList.Add( new SelectListItem() { Text = "All", Value = ((int)ImageInformation.ImageContents.All).ToString() } );
			}

			theList.Add( new SelectListItem() { Text = "Image", Value = ((int)ImageInformation.ImageContents.Image).ToString() } );
			theList.Add( new SelectListItem() { Text = "Edge", Value = ((int)ImageInformation.ImageContents.Edge).ToString() } );
			theList.Add( new SelectListItem() { Text = "Pull", Value = ((int)ImageInformation.ImageContents.Pull).ToString() } );
			theList.Add( new SelectListItem() { Text = "Finish", Value = ((int)ImageInformation.ImageContents.Finish).ToString() } );

			return theList;
		}

		static public IEnumerable<SelectListItem> GetImageTypeList()
		{
			return ImageRepository.ImageTypes.Select( it => new SelectListItem() { Value = it.Abbreviation, Text = it.Description } );
		}

		static public IEnumerable<SelectListItem> GetFinishTypeList()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem() { Text = "Veneer", Value = ((int)ImageInformation.FinishTypes.Veneer).ToString() },
				new SelectListItem() { Text = "Laminate", Value = ((int)ImageInformation.FinishTypes.Laminate).ToString() },
			};
		}

		static public IEnumerable<SelectListItem> GetFinishSubTypeList()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem() { Text = "Cherry Veneer", Value = ((int)ImageInformation.FinishSubTypes.CherryVeneer).ToString() },
				new SelectListItem() { Text = "Maple Veneer", Value = ((int)ImageInformation.FinishSubTypes.MapleVeneer).ToString() },
				new SelectListItem() { Text = "Walnut Veneer", Value = ((int)ImageInformation.FinishSubTypes.WalnutVeneer).ToString() },
				new SelectListItem() { Text = "High Pressure Laminate", Value = ((int)ImageInformation.FinishSubTypes.HighPressureLaminate).ToString() },
				new SelectListItem() { Text = "Low Pressure Laminate", Value = ((int)ImageInformation.FinishSubTypes.LowPressureLaminate).ToString() },
			};
		}

		[Authorize]
		public ActionResult RebuildGallery()
		{
			ImageRepository iRepo = new ImageRepository();

			iRepo.RebuildImageGallery();

			return RedirectToAction( "Images" );
		}

		[Authorize]
		public ActionResult SearchLog()
		{
			ImportRepository iRepo = new ImportRepository();

			var logList = iRepo.GetSearchLogList();

			return View( logList );
		}
	}

}
