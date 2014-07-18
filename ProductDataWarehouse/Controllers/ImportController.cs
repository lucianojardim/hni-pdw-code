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
    public class ImportController : Controller
    {
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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Logon( string userName, string password, string ReturnUrl )
		{
			if( ModelState.IsValid )
			{
				string homePage = "";
				if( ( new UserRepository().ValidateUserAccount( userName, password, out homePage ) ) )
				{
					FormsAuthentication.SetAuthCookie( userName, false );

					if( ReturnUrl != null )
					{
						return Redirect( ReturnUrl );
					}

					return Redirect( homePage );
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
				ViewBag.EmailSent = ( new UserRepository().ResetUserPassword( fpEmailAddress ) );
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
					try
					{
						ImportRepository iRepo = new ImportRepository();

						iRepo.ImportFileData( csvFile.InputStream, csvFile.ContentLength );

						ViewBag.ImportSuccess = true;

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
			return View( (new ImportRepository()).GetHomePageContent() );
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
				try
				{
					ImportRepository iRepo = new ImportRepository();

					iRepo.UpsertHomePageContent( content );

					return RedirectToAction( "Index", "Home" );
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

		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
		public ActionResult EditImage( int id )
		{
			ImageRepository iRepo = new ImageRepository();

			return View( iRepo.GetImageInformation( id ) );
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

		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
		public ActionResult UploadImage( int id )
		{
			ImageRepository iRepo = new ImageRepository();

			return View( iRepo.GetImageInformation( id ) );
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
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

		[PaoliAuthorize( "CanManageImages" )]
		[TempPasswordCheck]
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
				new SelectListItem() { Text = "Premium", Value = ((int)ImageInformation.SeatingGrades.Premium).ToString() },
			};
		}

		static public IEnumerable<SelectListItem> GetVeneerGradeList()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem() { Text = "Standard", Value = ((int)ImageInformation.VeneerGrades.Standard).ToString() },
				new SelectListItem() { Text = "Premium", Value = ((int)ImageInformation.VeneerGrades.Premium).ToString() },
			};
		}

		static public IEnumerable<SelectListItem> GetVeneerSpeciesList()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem() { Text = "Cherry", Value = ((int)ImageInformation.VeneerSpecieses.Cherry).ToString() },
				new SelectListItem() { Text = "Maple", Value = ((int)ImageInformation.VeneerSpecieses.Maple).ToString() },
				new SelectListItem() { Text = "Walnut", Value = ((int)ImageInformation.VeneerSpecieses.Walnut).ToString() },
				new SelectListItem() { Text = "Rifted Oak", Value = ((int)ImageInformation.VeneerSpecieses.RiftedOak).ToString() },
				new SelectListItem() { Text = "European Walnut", Value = ((int)ImageInformation.VeneerSpecieses.EuropeanWalnut).ToString() },
				new SelectListItem() { Text = "Gray Oak", Value = ((int)ImageInformation.VeneerSpecieses.GrayOak).ToString() },
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
			ImportRepository iRepo = new ImportRepository();

			iRepo.RebuildDBKeywords();

			return RedirectToAction( "Index" );
		}
		
		#endregion

		[PaoliAuthorize( "CanManageSearchLog" )]
		[TempPasswordCheck]
		public ActionResult SearchLog()
		{
			ImportRepository iRepo = new ImportRepository();

			var logList = iRepo.GetSearchLogList();

			return View( logList );
		}

		#region Print Material Management
		[PaoliAuthorize( "CanManagePrintMaterial" )]
		[TempPasswordCheck]
		public ActionResult PrintMaterial()
		{
			return View();
		}

		[PaoliAuthorize( "CanManagePrintMaterial" )]
		[TempPasswordCheck]
		public JsonResult FullPrintMaterialList( DataTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			PublicationRepository pRepository = new PublicationRepository();

			var results = pRepository.GetFullPublicationList(
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

		[PaoliAuthorize( "CanManagePrintMaterial" )]
		[TempPasswordCheck]
		public ActionResult AddPrintMaterial()
		{
			return View( new PublicationInformation() { PublicationDate = DateTime.Now } );
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PaoliAuthorize( "CanManagePrintMaterial" )]
		[TempPasswordCheck]
		public ActionResult AddPrintMaterial( PublicationInformation pubInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					PublicationRepository pRepo = new PublicationRepository();

					pRepo.AddPublication( pubInfo );

					ViewBag.CloseFancyBox = true;

					return View( pubInfo );
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
			return View( pubInfo );
		}

		[PaoliAuthorize( "CanManagePrintMaterial" )]
		[TempPasswordCheck]
		public ActionResult EditPrintMaterial( int id )
		{
			return View( new PublicationRepository().GetPublicationInformation( id ) );
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PaoliAuthorize( "CanManagePrintMaterial" )]
		[TempPasswordCheck]
		public ActionResult EditPrintMaterial( PublicationInformation pubInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					PublicationRepository pRepo = new PublicationRepository();

					pRepo.UpdatePublication( pubInfo );

					ViewBag.CloseFancyBox = true;

					return View( pubInfo );
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
			return View( pubInfo );
		}

		[PaoliAuthorize( "CanManagePrintMaterial" )]
		[TempPasswordCheck]
		public ActionResult PrintMaterialImages( int id )
		{
			return View( new PublicationRepository().GetPublicationInformation( id ) );
		}

		[PaoliAuthorize( "CanManagePrintMaterial" )]
		[TempPasswordCheck]
		public JsonResult NonPubImageList( PubImageTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			ImageRepository iRepository = new ImageRepository();

			var results = iRepository.GetPubImageList(
				param, false, out totalCount, out filteredCount );

			return Json( new
			{
				sEcho = param.sEcho,
				iTotalRecords = totalCount,
				iTotalDisplayRecords = filteredCount,
				aaData = results
			},
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManagePrintMaterial" )]
		[TempPasswordCheck]
		public JsonResult PubImageList( PubImageTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			ImageRepository iRepository = new ImageRepository();

			var results = iRepository.GetPubImageList(
				param, true, out totalCount, out filteredCount );

			return Json( new
			{
				sEcho = param.sEcho,
				iTotalRecords = totalCount,
				iTotalDisplayRecords = filteredCount,
				aaData = results
			},
				JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManagePrintMaterial" )]
		[TempPasswordCheck]
		public JsonResult AddPubImage( int pubId, int imageId, int? pageNumber )
		{
			new PublicationRepository().AddPubImage( pubId, imageId, pageNumber );

			return Json( new { success = true }, JsonRequestBehavior.AllowGet );
		}

		[PaoliAuthorize( "CanManagePrintMaterial" )]
		[TempPasswordCheck]
		public JsonResult RemovePubImage( int pubId, int imageId )
		{
			new PublicationRepository().RemovePubImage( pubId, imageId );

			return Json( new { success = true }, JsonRequestBehavior.AllowGet );
		}
		#endregion

		#region Dealers
		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult ManageDealers()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult ImportDealers()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ImportDealers( HttpPostedFileBase dealerFile )
		{
			if( ModelState.IsValid )
			{
				try
				{
					DealerRepository dRepository = new DealerRepository();

					dRepository.ImportDealers( dealerFile.InputStream );

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

		public static IEnumerable<SelectListItem> GetVideoDDList()
		{
			DealerRepository dRepository = new DealerRepository();

			var results = dRepository.GetFullVideoList().OrderBy( v => v.Name );

			return results.Select( i => new SelectListItem() { Text = i.Name, Value = i.VideoID.ToString() } );
		}

		public static IEnumerable<SelectListItem> GetPageDDList()
		{
			DealerRepository dRepository = new DealerRepository();

			var results = dRepository.GetFullPageList().OrderBy( v => v.Name );

			return results.Select( i => new SelectListItem() { Text = i.Name, Value = i.PageID.ToString() } );
		}

		public static IEnumerable<SelectListItem> GetSeriesDDList( bool bIncludeBlank )
		{
			SeriesRepository sRepository = new SeriesRepository();

			var results = sRepository.GetSeriesNameList( null ).OrderBy( v => v.Name );

			List<SelectListItem> theList = new List<SelectListItem>();
			if( bIncludeBlank )
			{
				theList.Add( new SelectListItem() { Text = "", Value = "" } );
			}

			return theList.Union( results.Select( i => new SelectListItem() { Text = i.Name, Value = i.Name } ) );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public JsonResult FullDealerList( DealerTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			DealerRepository dRepository = new DealerRepository();

			var results = dRepository.GetFullDealerList(
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

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult AddDealer()
		{
			return View( new DealerInformation() );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddDealer( DealerInformation dInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					DealerRepository dRepository = new DealerRepository();

					dRepository.AddDealer( dInfo );

					ViewBag.CloseFancyBox = true;

					return View( dInfo );
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

			return View( dInfo );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult EditDealer( int id )
		{
			DealerRepository dRepository = new DealerRepository();

			var dInfo = dRepository.GetDealer( id );

			return View( dInfo );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditDealer( DealerInformation dInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					DealerRepository dRepository = new DealerRepository();

					dRepository.UpdateDealer( dInfo );

					ViewBag.CloseFancyBox = true;

					return View( dInfo );
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

			return View( dInfo );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public JsonResult DeleteDealer( int id )
		{
			DealerRepository dRepository = new DealerRepository();

			bool bSuccess = dRepository.DeleteDealer( id );

			return Json( new
			{
				success = bSuccess
			},
				JsonRequestBehavior.AllowGet );
		}

		#endregion

		#region Dealer Videos
		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult ManageVideos()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public JsonResult FullVideoList( VideoTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			DealerRepository dRepository = new DealerRepository();

			var results = dRepository.GetFullVideoList(
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

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult AddDealerVideo()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddDealerVideo( DealerVideoInformation dInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					DealerRepository dRepository = new DealerRepository();

					dRepository.AddDealerVideo( dInfo );

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

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult EditDealerVideo( int id )
		{
			DealerRepository dRepository = new DealerRepository();

			var dInfo = dRepository.GetDealerVideo( id );

			return View( dInfo );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditDealerVideo( DealerVideoInformation dInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					DealerRepository dRepository = new DealerRepository();

					dRepository.UpdateDealerVideo( dInfo );

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

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public JsonResult DeleteDealerVideo( int id )
		{
			DealerRepository dRepository = new DealerRepository();

			bool bSuccess = dRepository.DeleteDealerVideo( id );

			return Json( new
			{
				success = bSuccess
			},
				JsonRequestBehavior.AllowGet );
		}

		#endregion

		#region Dealer Pages
		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult ManagePages()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public JsonResult FullDealerPageList( DealerPageTableParams param )
		{
			int totalCount = 0, filteredCount = 0;

			DealerRepository dRepository = new DealerRepository();

			var results = dRepository.GetFullPageList(
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

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult AddDealerPage()
		{
			return View();
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddDealerPage( DealerPageInformation dInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					DealerRepository dRepository = new DealerRepository();

					dRepository.AddDealerPage( dInfo );

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

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public ActionResult EditDealerPage( int id )
		{
			DealerRepository dRepository = new DealerRepository();

			var dInfo = dRepository.GetDealerPage( id );

			return View( dInfo );
		}

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditDealerPage( DealerPageInformation dInfo )
		{
			if( ModelState.IsValid )
			{
				try
				{
					DealerRepository dRepository = new DealerRepository();

					dRepository.UpdateDealerPage( dInfo );

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

		[PaoliAuthorize( "CanManageCompanies" )]
		[TempPasswordCheck]
		public JsonResult DeleteDealerPage( int id )
		{
			DealerRepository dRepository = new DealerRepository();

			bool bSuccess = dRepository.DeleteDealerPage( id );

			return Json( new
			{
				success = bSuccess
			},
				JsonRequestBehavior.AllowGet );
		}

		#endregion
	}

}
