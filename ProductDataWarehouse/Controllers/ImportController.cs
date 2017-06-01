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
using PDWModels.Publications;
using PDWModels.Dealers;
using PDWInfrastructure.Attributes;
using PDWModels;

namespace ProductDataWarehouse.Controllers
{
	public class ImportController : BaseController
    {
		public ActionResult Error()
		{
			return View();
		}

		public ActionResult Logon()
		{
			if( HttpContext.User.Identity.IsAuthenticated )
			{
				return Redirect( PaoliWebUser.CurrentUser.HomePage );
			}

			return View();
		}

		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			Session.Abandon();
			return RedirectToAction( "Index", "Home" );
		}

		public JsonResult KeepAlive()
		{
			return Json( new {
				success = true
			}, JsonRequestBehavior.AllowGet );
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Logon( string userName, string password, string ReturnUrl )
		{
			if( ModelState.IsValid )
			{
				string homePage = "";
				using( var uRepository = new UserRepository() )
				{
					if( uRepository.ValidateUserAccount( userName, password, out homePage ) )
					{
						FormsAuthentication.SetAuthCookie( userName, false );

						if( ReturnUrl != null )
						{
							return Redirect( ReturnUrl );
						}

						return Redirect( homePage );
					}
				}
				Thread.Sleep( 5000 );	// delay to prevent brute force attacks
				ModelState.AddModelError( "", "Invalid email address and/or password." );
			}

			return View();
		}

		public ActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ForgotPassword( string fpEmailAddress )
		{
			try
			{
				using( var uRepository = new UserRepository() )
				{
					ViewBag.EmailSent = ( uRepository.ResetUserPassword( fpEmailAddress ) );
				}
			}
			catch
			{
				ModelState.AddModelError( "error", "Unable to reset password at this time." );
			}

			return View();
		}

		#region Data File Import 
		[PaoliAuthorize( "CanManageImport" )]
		[TempPasswordCheck]
		public ActionResult Index()
        {
            return View();
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PaoliAuthorize( "CanManageImport" )]
		[TempPasswordCheck]
		public ActionResult Index( HttpPostedFileBase csvFile )
		{
			if( ModelState.IsValid )
			{
				if( csvFile != null )
				{
					using( var iRepo = new ImportRepository() )
					{
						iRepo.ImportFileData( csvFile.InputStream, csvFile.ContentLength );

						ViewBag.ImportSuccess = true;

						return View();
					}
				}
				else
				{
					ModelState.AddModelError( "", "PDW file is required to upload new data." );
				}
			}

			return View();
		}
		#endregion

		#region New Home Page Management
		[PaoliAuthorize( "CanManageNewHomePage" )]
		[TempPasswordCheck]
		public ActionResult NewHomePage()
		{
			using( var iRepository = new ImportRepository() )
			{
				return View( iRepository.GetHomePageContent() );
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PaoliAuthorize( "CanManageNewHomePage" )]
		[TempPasswordCheck]
		[ValidateInput( false )]
		public ActionResult NewHomePage( HomePageContentInformation content )
		{
			if( ModelState.IsValid )
			{
				using( var iRepo = new ImportRepository() )
				{
					iRepo.UpsertHomePageContent( content );

					return RedirectToAction( "Index", "Home" );
				}
			}

			return View();
		}
		#endregion

		#region Image Management
		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
		public ActionResult Images()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
		public JsonResult FullImageList( ImageTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			using( var iRepository = new ImageRepository() )
			{
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
		}

		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
		public ActionResult AddImage()
		{
			return View( new ImageInformation() { ImageContent = 1 } );
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
		[ValidateInput( false )]
		public ActionResult AddImage( ImageInformation imgInfo, HttpPostedFileBase imageFile )
		{
			switch( (ImageInformation.ImageContents)imgInfo.ImageContent )
			{
				case ImageInformation.ImageContents.Image:
					imgInfo.FeaturedEdge = null;
					imgInfo.FeaturedPull = null;
					imgInfo.FeaturedTableBase = null;
					imgInfo.FeaturedTableShape = null;
					imgInfo.ControlDescription = null;
					imgInfo.ControlMechanism = null;
					imgInfo.GoToGuidePageNum = 0;
					imgInfo.FeaturedFinish = null;
					if( imgInfo.ImageType != "Env" && imgInfo.ImageType != "WS" )
					{
						imgInfo.ImageApplication = null;
					}
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
						imgInfo.FeaturedTableBase = null;
						imgInfo.FeaturedTableShape = null;
						imgInfo.ControlDescription = null;
						imgInfo.ControlMechanism = null;
						imgInfo.HasPeople = false;
						imgInfo.GoToGuidePageNum = 0;
						imgInfo.FeaturedFinish = null;
						imgInfo.ImageApplication = null;
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
						imgInfo.FeaturedTableBase = null;
						imgInfo.FeaturedTableShape = null;
						imgInfo.ControlDescription = null;
						imgInfo.ControlMechanism = null;
						imgInfo.HasPeople = false;
						imgInfo.GoToGuidePageNum = 0;
						imgInfo.FeaturedFinish = null;
						imgInfo.ImageApplication = null;
					}
					break;
				case ImageInformation.ImageContents.Finish:
					if( imgInfo.FeaturedFinish == null )
					{
						ModelState.AddModelError( "FPRequired", "Featured Finish is required." );
					}
					else
					{
						imgInfo.ImageType = "Det";
						imgInfo.FeaturedPull = null;
						imgInfo.FeaturedEdge = null;
						imgInfo.FeaturedTableBase = null;
						imgInfo.FeaturedTableShape = null;
						imgInfo.ControlDescription = null;
						imgInfo.ControlMechanism = null;
						imgInfo.HasPeople = false;
						imgInfo.GoToGuidePageNum = 0;
						imgInfo.ImageApplication = null;
					}
					break;
				case ImageInformation.ImageContents.TableShape:
					if( imgInfo.FeaturedTableShape == null )
					{
						ModelState.AddModelError( "FPRequired", "Featured Table Shape is required." );
					}
					else
					{
						imgInfo.ImageType = "Det";
						imgInfo.FeaturedPull = null;
						imgInfo.FeaturedEdge = null;
						imgInfo.FeaturedTableBase = null;
						imgInfo.ControlDescription = null;
						imgInfo.ControlMechanism = null;
						imgInfo.HasPeople = false;
						imgInfo.GoToGuidePageNum = 0;
						imgInfo.FeaturedFinish = null;
						imgInfo.ImageApplication = null;
					}
					break;
				case ImageInformation.ImageContents.TableBase:
					if( imgInfo.FeaturedTableBase == null )
					{
						ModelState.AddModelError( "FPRequired", "Featured Table Base is required." );
					}
					else
					{
						imgInfo.ImageType = "Det";
						imgInfo.FeaturedPull = null;
						imgInfo.FeaturedEdge = null;
						imgInfo.FeaturedTableShape = null;
						imgInfo.ControlDescription = null;
						imgInfo.ControlMechanism = null;
						imgInfo.HasPeople = false;
						imgInfo.GoToGuidePageNum = 0;
						imgInfo.FeaturedFinish = null;
						imgInfo.ImageApplication = null;
					}
					break;
				case ImageInformation.ImageContents.ControlMech:
					if( imgInfo.ControlMechanism == null )
					{
						ModelState.AddModelError( "FPRequired", "Control Mechanism is required." );
					}
					else
					{
						imgInfo.ImageType = "Det";
						imgInfo.FeaturedPull = null;
						imgInfo.FeaturedEdge = null;
						imgInfo.FeaturedTableBase = null;
						imgInfo.FeaturedTableShape = null;
						imgInfo.HasPeople = false;
						imgInfo.GoToGuidePageNum = 0;
						imgInfo.FeaturedFinish = null;
						imgInfo.ImageApplication = null;
					}
					break;
				case ImageInformation.ImageContents.GoToGuide:
					imgInfo.ImageType = "Det";
					imgInfo.FeaturedPull = null;
					imgInfo.FeaturedEdge = null;
					imgInfo.FeaturedTableBase = null;
					imgInfo.FeaturedTableShape = null;
					imgInfo.ControlMechanism = null;
					imgInfo.HasPeople = false;
					imgInfo.FeaturedFinish = null;
					imgInfo.ImageApplication = null;
					break;

			}

			if( ModelState.IsValid )
			{
				using( var iRepo = new ImageRepository() )
				{
					iRepo.ImportImageFileData( imgInfo, imageFile.InputStream, imageFile.ContentLength, imageFile.FileName, imageFile.ContentType );

					ViewBag.CloseFancyBox = true;

					return View();
				}
			}
			return View();
		}

		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
		public ActionResult ViewImage( int id )
		{
			using( var iRepo = new ImageRepository() )
			{
				return View( iRepo.GetImageUsage( id ) );
			}
		}

		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
		public ActionResult EditImage( int id )
		{
			using( var iRepo = new ImageRepository() )
			{
				return View( iRepo.GetImageInformation( id ) );
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
		[ValidateInput( false )]
		public ActionResult EditImage( ImageInformation imgInfo )
		{
			switch( (ImageInformation.ImageContents)imgInfo.ImageContent )
			{
				case ImageInformation.ImageContents.Image:
					imgInfo.FeaturedEdge = null;
					imgInfo.FeaturedPull = null;
					imgInfo.FeaturedTableBase = null;
					imgInfo.FeaturedTableShape = null;
					imgInfo.ControlDescription = null;
					imgInfo.ControlMechanism = null;
					imgInfo.GoToGuidePageNum = 0;
					imgInfo.FeaturedFinish = null;
					if( imgInfo.ImageType != "Env" && imgInfo.ImageType != "WS" )
					{
						imgInfo.ImageApplication = null;
					}
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
						imgInfo.FeaturedTableBase = null;
						imgInfo.FeaturedTableShape = null;
						imgInfo.ControlDescription = null;
						imgInfo.ControlMechanism = null;
						imgInfo.HasPeople = false;
						imgInfo.GoToGuidePageNum = 0;
						imgInfo.FeaturedFinish = null;
						imgInfo.ImageApplication = null;
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
						imgInfo.FeaturedTableBase = null;
						imgInfo.FeaturedTableShape = null;
						imgInfo.ControlDescription = null;
						imgInfo.ControlMechanism = null;
						imgInfo.HasPeople = false;
						imgInfo.GoToGuidePageNum = 0;
						imgInfo.FeaturedFinish = null;
						imgInfo.ImageApplication = null;
					}
					break;
				case ImageInformation.ImageContents.Finish:
					if( imgInfo.FeaturedFinish == null )
					{
						ModelState.AddModelError( "FPRequired", "Featured Finish is required." );
					}
					else
					{
						imgInfo.ImageType = "Det";
						imgInfo.FeaturedPull = null;
						imgInfo.FeaturedEdge = null;
						imgInfo.FeaturedTableBase = null;
						imgInfo.FeaturedTableShape = null;
						imgInfo.ControlDescription = null;
						imgInfo.ControlMechanism = null;
						imgInfo.HasPeople = false;
						imgInfo.GoToGuidePageNum = 0;
						imgInfo.ImageApplication = null;
					}
					break;
				case ImageInformation.ImageContents.TableShape:
					if( imgInfo.FeaturedTableShape == null )
					{
						ModelState.AddModelError( "FPRequired", "Featured Table Shape is required." );
					}
					else
					{
						imgInfo.ImageType = "Det";
						imgInfo.FeaturedPull = null;
						imgInfo.FeaturedEdge = null;
						imgInfo.FeaturedTableBase = null;
						imgInfo.ControlDescription = null;
						imgInfo.ControlMechanism = null;
						imgInfo.HasPeople = false;
						imgInfo.GoToGuidePageNum = 0;
						imgInfo.FeaturedFinish = null;
						imgInfo.ImageApplication = null;
					}
					break;
				case ImageInformation.ImageContents.TableBase:
					if( imgInfo.FeaturedTableBase == null )
					{
						ModelState.AddModelError( "FPRequired", "Featured Table Base is required." );
					}
					else
					{
						imgInfo.ImageType = "Det";
						imgInfo.FeaturedPull = null;
						imgInfo.FeaturedEdge = null;
						imgInfo.FeaturedTableShape = null;
						imgInfo.ControlDescription = null;
						imgInfo.ControlMechanism = null;
						imgInfo.HasPeople = false;
						imgInfo.GoToGuidePageNum = 0;
						imgInfo.FeaturedFinish = null;
						imgInfo.ImageApplication = null;
					}
					break;
				case ImageInformation.ImageContents.ControlMech:
					if( imgInfo.ControlMechanism == null )
					{
						ModelState.AddModelError( "FPRequired", "Control Mechanism is required." );
					}
					else
					{
						imgInfo.ImageType = "Det";
						imgInfo.FeaturedPull = null;
						imgInfo.FeaturedEdge = null;
						imgInfo.FeaturedTableBase = null;
						imgInfo.FeaturedTableShape = null;
						imgInfo.HasPeople = false;
						imgInfo.GoToGuidePageNum = 0;
						imgInfo.FeaturedFinish = null;
						imgInfo.ImageApplication = null;
					}
					break;
				case ImageInformation.ImageContents.GoToGuide:
					imgInfo.ImageType = "Det";
					imgInfo.FeaturedPull = null;
					imgInfo.FeaturedEdge = null;
					imgInfo.FeaturedTableBase = null;
					imgInfo.FeaturedTableShape = null;
					imgInfo.ControlMechanism = null;
					imgInfo.HasPeople = false;
					imgInfo.FeaturedFinish = null;
					imgInfo.ImageApplication = null;
					break;

			}

			if( ModelState.IsValid )
			{
				using( var iRepo = new ImageRepository() )
				{
					iRepo.UpdateImageFile( imgInfo );

					ViewBag.CloseFancyBox = true;

					return View();
				}
			}
			return View();
		}

		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
		public ActionResult UploadImage( int id )
		{
			using( var iRepo = new ImageRepository() )
			{
				return View( iRepo.GetImageInformation( id ) );
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
		public ActionResult UploadImage( int ImageID, HttpPostedFileBase imageFile )
		{
			if( ModelState.IsValid )
			{
				using( var iRepo = new ImageRepository() )
				{
					iRepo.UploadImageFile( ImageID, imageFile.InputStream, imageFile.ContentLength, imageFile.FileName, imageFile.ContentType );

					ViewBag.CloseFancyBox = true;

					return View();
				}
			}
			return View();
		}

		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
		public ActionResult DeleteImage( int id )
		{
			using( var iRepo = new ImageRepository() )
			{
				iRepo.DeleteImageFile( id );

				return RedirectToAction( "Images" );
			}
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
			theList.Add( new SelectListItem() { Text = "Table Shape", Value = ((int)ImageInformation.ImageContents.TableShape).ToString() } );
			theList.Add( new SelectListItem() { Text = "Table Base", Value = ((int)ImageInformation.ImageContents.TableBase).ToString() } );
			theList.Add( new SelectListItem() { Text = "Control Mechansim", Value = ((int)ImageInformation.ImageContents.ControlMech).ToString() } );
			theList.Add( new SelectListItem() { Text = "Go To Guide", Value = ((int)ImageInformation.ImageContents.GoToGuide).ToString() } );

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
				new SelectListItem() { Text = "Seating", Value = ((int)ImageInformation.FinishTypes.Seating).ToString() },
			};
		}

		static public IEnumerable<SelectListItem> GetLaminatePatternList()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem() { Text = "Solid", Value = ((int)ImageInformation.LaminatePatterns.Solid).ToString() },
				new SelectListItem() { Text = "Wood Grain", Value = ((int)ImageInformation.LaminatePatterns.WoodGrain).ToString() },
			};
		}

		static public IEnumerable<SelectListItem> GetSeatingGradeList()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem() { Text = "Standard", Value = ((int)ImageInformation.SeatingGrades.Standard).ToString() },
			};
		}

		static public string GetImageApplicationList()
		{
			var list = new List<string>() { "Private Office", "Teaming", "Collaborative", "Training", "Café" };
			return "'" + string.Join( "','", list ) + "'";
		}

		static public IEnumerable<SelectListItem> GetVeneerGradeList()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem() { Text = "Standard", Value = ((int)ImageInformation.VeneerGrades.Standard).ToString() },
			};
		}

		static public IEnumerable<SelectListItem> GetVeneerSpeciesList()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem() { Text = "Cherry", Value = ((int)ImageInformation.VeneerSpecieses.Cherry).ToString() },
				new SelectListItem() { Text = "Maple", Value = ((int)ImageInformation.VeneerSpecieses.Maple).ToString() },
				new SelectListItem() { Text = "Walnut", Value = ((int)ImageInformation.VeneerSpecieses.Walnut).ToString() },
				new SelectListItem() { Text = "Maple Stratawood", Value = ((int)ImageInformation.VeneerSpecieses.MapleStratawood).ToString() },
				new SelectListItem() { Text = "White Stratawood", Value = ((int)ImageInformation.VeneerSpecieses.WhiteStratawood).ToString() },
				new SelectListItem() { Text = "Cherry Stratawood", Value = ((int)ImageInformation.VeneerSpecieses.CherryStratawood).ToString() },
				new SelectListItem() { Text = "Grey Stratawood", Value = ((int)ImageInformation.VeneerSpecieses.GreyStratawood).ToString() },
			};
		}

		/*
		[PaoliAuthorize( "IsSuperAdmin" )]
		public ActionResult RebuildGallery()
		{
			ImageRepository iRepo = new ImageRepository();

			iRepo.RebuildImageGallery();

			return RedirectToAction( "Images" );
		}
*/
		[PaoliAuthorize( "IsSuperAdmin" )]
		[TempPasswordCheck]
		public ActionResult RebuildDBKeywords()
		{
			using( var iRepo = new ImportRepository() )
			{
				iRepo.RebuildDBKeywords();

				return RedirectToAction( "Index" );
			}
		}
		
		#endregion

		[PaoliAuthorize( "CanManageSearchLog" )]
		[TempPasswordCheck]
		public ActionResult SearchLog()
		{
			using( var iRepo = new ImportRepository() )
			{
				var logList = iRepo.GetSearchLogList();

				return View( logList );
			}
		}

		public static IEnumerable<SelectListItem> GetSeriesDDList( bool bIncludeBlank )
		{
			using( var sRepository = new SeriesRepository() )
			{
				var results = sRepository.GetSeriesNameList( null ).OrderBy( v => v.Name );

				List<SelectListItem> theList = new List<SelectListItem>();
				if( bIncludeBlank )
				{
					theList.Add( new SelectListItem() { Text = "", Value = "" } );
				}

				return theList.Union( results.Select( i => new SelectListItem() { Text = i.Name, Value = i.Name } ) );
			}
		}

	}

}
