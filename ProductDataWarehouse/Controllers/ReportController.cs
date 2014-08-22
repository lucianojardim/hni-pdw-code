using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDWInfrastructure.Attributes;
using PWDRepositories;
using System.IO;
using CsvHelper;

namespace ProductDataWarehouse.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/

		[PaoliAuthorize( "CanViewReports" )]
		[TempPasswordCheck]
		public ActionResult Index()
        {
            return View();
        }

		[PaoliAuthorize( "CanViewReports" )]
		[TempPasswordCheck]
		public FileStreamResult SpecRequestList()
		{
			SpecRequestRepository sRepo = new SpecRequestRepository();

			var theData = sRepo.GetExportList();

			return new FileStreamResult( WriteCsvToMemory( theData ), "text/csv" ) { FileDownloadName = "specrequests.csv" };
		}

		[PaoliAuthorize( "CanViewReports" )]
		[TempPasswordCheck]
		public FileStreamResult CollateralOrderList()
		{
			CollateralRepository cRepo = new CollateralRepository();

			var theData = cRepo.GetExportList();

			return new FileStreamResult( WriteCsvToMemory( theData ), "text/csv" ) { FileDownloadName = "collateralorders.csv" };
		}

		[PaoliAuthorize( "CanViewReports" )]
		[TempPasswordCheck]
		public FileStreamResult UserActivity()
		{
			UserRepository uRepo = new UserRepository();

			var theData = uRepo.GetExportList();

			return new FileStreamResult( WriteCsvToMemory( theData ), "text/csv" ) { FileDownloadName = "useractivity.csv" };
		}

		private Stream WriteCsvToMemory( IEnumerable<object> records )
		{
			using( var memoryStream = new MemoryStream() )
			{
				using( var streamWriter = new StreamWriter( memoryStream ) )
				{
					using( var csvWriter = new CsvWriter( streamWriter ) )
					{
						csvWriter.WriteRecords( records );
						streamWriter.Flush();

						var result = memoryStream.ToArray();

						return new MemoryStream( result );

					}
				}
			}
		}
    }
}
