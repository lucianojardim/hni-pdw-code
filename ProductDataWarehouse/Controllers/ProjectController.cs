using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure.Attributes;
using PDWInfrastructure;
using PWDRepositories;
using PDWModels.SpecRequests;

namespace ProductDataWarehouse.Controllers
{
    public class ProjectController : Controller
    {
		public static IEnumerable<SelectListItem> GetProjectStatusList()
		{
			return ProjectStatus.List.Select( i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Text } );
		}

		public static IEnumerable<SelectListItem> GetProjectSuccessList()
		{
			return ProjectSuccess.List.Select( i => new SelectListItem() { Value = i.ID.ToString(), Text = i.Text } );
		}

		public static IEnumerable<SelectListItem> GetProjectCompetitorList()
		{
			return new List<SelectListItem>()
			{
				new SelectListItem() { Text = "" },
				new SelectListItem() { Text = "9 to 5 Seating" },
				new SelectListItem() { Text = "Allsteel" },
				new SelectListItem() { Text = "Coalesse" },
				new SelectListItem() { Text = "Community" },
				new SelectListItem() { Text = "Global" },
				new SelectListItem() { Text = "Gunlocke" },
				new SelectListItem() { Text = "Hayworth" },
				new SelectListItem() { Text = "Herman Miller" },
				new SelectListItem() { Text = "HON" },
				new SelectListItem() { Text = "Indiana Furniture" },
				new SelectListItem() { Text = "JSI (Jasper)" },
				new SelectListItem() { Text = "JOFCO" },
				new SelectListItem() { Text = "Kimball Office" },
				new SelectListItem() { Text = "Knoll" },
				new SelectListItem() { Text = "National" },
				new SelectListItem() { Text = "OFS" },
				new SelectListItem() { Text = "Sitonit" },
				new SelectListItem() { Text = "Steelcase" },
				new SelectListItem() { Text = "Teknion﻿" },
			};
		}

		[PaoliAuthorize( "CanBeLoggedIn" )]
		public JsonResult GetFullEndCustomerList( bool includeInactive = false )
		{
			using( var sRepo = new SpecRequestRepository() )
			{
				var results = sRepo.GetEndCustomerList( includeInactive );

				return Json( results, JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanManageProjects" )]
		[TempPasswordCheck]
		public ActionResult Manage()
        {
            return View();
        }

		[PaoliAuthorize( "CanManageProjects" )]
		[TempPasswordCheck]
		public JsonResult ProjectList( ProjectTableParams paramDetails )
		{
			int totalCount = 0, filteredCount = 0;

			using( var sRepository = new SpecRequestRepository() )
			{
				var results = sRepository.GetUserProjectList(
					paramDetails, out totalCount, out filteredCount );

				return Json( new
				{
					sEcho = paramDetails.sEcho,
					iTotalRecords = totalCount,
					iTotalDisplayRecords = filteredCount,
					aaData = results
				},
					JsonRequestBehavior.AllowGet );
			}
		}

		[PaoliAuthorize( "CanManageProjects" )]
		[TempPasswordCheck]
		public ActionResult Edit( int id )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				ViewBag.BlankInformation = sRepository.GetProjectSeriesInformation();

				return View( sRepository.GetProject( id ) );
			}
		}

		[PaoliAuthorize( "CanManageProjects" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit( ProjectInformation pInfo )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				if( ModelState.IsValid )
				{
					sRepository.Update( pInfo );

					return RedirectToAction( "Manage" );
				}

				ViewBag.BlankInformation = sRepository.GetProjectSeriesInformation();
			}

			return View( pInfo );
		}

		[PaoliAuthorize( "CanManageProjects" )]
		[TempPasswordCheck]
		public ActionResult Add()
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				ViewBag.BlankInformation = sRepository.GetProjectSeriesInformation();

				return View();
			}
		}

		[PaoliAuthorize( "CanManageProjects" )]
		[TempPasswordCheck]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Add( ProjectInformation pInfo )
		{
			using( var sRepository = new SpecRequestRepository() )
			{
				if( ModelState.IsValid )
				{
					sRepository.Add( pInfo );

					return RedirectToAction( "Manage" );
				}

				ViewBag.BlankInformation = sRepository.GetProjectSeriesInformation();
			}

			return View( pInfo );
		}

	}
}
