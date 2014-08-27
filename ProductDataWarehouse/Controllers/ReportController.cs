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
			var theItems = cRepo.GetOrderDetailNameList();

			using( var memoryStream = new MemoryStream() )
			{
				using( var streamWriter = new StreamWriter( memoryStream ) )
				{
					using( var csvWriter = new CsvWriter( streamWriter ) )
					{
						csvWriter.WriteField( "OrderID" );
						csvWriter.WriteField( "OrderDate" );
						csvWriter.WriteField( "Status" );
						csvWriter.WriteField( "CreatedByUserName" );
						csvWriter.WriteField( "CreatedByCompany" );
						csvWriter.WriteField( "RequestingParty" );
						csvWriter.WriteField( "RPUserName" );
						csvWriter.WriteField( "RPCompany" );
						csvWriter.WriteField( "EndUserFirstName" );
						csvWriter.WriteField( "EndUserLastName" );
						csvWriter.WriteField( "EndUserPhoneNumber" );
						csvWriter.WriteField( "EndUserEMailAddress" );
						csvWriter.WriteField( "ShippingType" );
						csvWriter.WriteField( "SPUserName" );
						csvWriter.WriteField( "SPCompany" );
						csvWriter.WriteField( "ShippingAttn" );
						csvWriter.WriteField( "ShippingCompanyName" );
						csvWriter.WriteField( "ShippingAddress1" );
						csvWriter.WriteField( "ShippingAddress2" );
						csvWriter.WriteField( "ShippingCity" );
						csvWriter.WriteField( "ShippingState" );
						csvWriter.WriteField( "ShippingZip" );
						csvWriter.WriteField( "ShippingPhoneNumber" );
						csvWriter.WriteField( "ShippingEmailAddress" );
						csvWriter.WriteField( "CanceledByUserName" );
						csvWriter.WriteField( "CanceledOnDateTime" );

						foreach( var header in theItems )
						{
							csvWriter.WriteField( header );
						}
						csvWriter.NextRecord();

						foreach( var item in theData )
						{
							csvWriter.WriteField( item.OrderID );
							csvWriter.WriteField( item.OrderDate );
							csvWriter.WriteField( item.Status );
							csvWriter.WriteField( item.CreatedByUserName );
							csvWriter.WriteField( item.CreatedByCompany );
							csvWriter.WriteField( item.RequestingParty );
							csvWriter.WriteField( item.RPUserName );
							csvWriter.WriteField( item.RPCompany );
							csvWriter.WriteField( item.EndUserFirstName );
							csvWriter.WriteField( item.EndUserLastName );
							csvWriter.WriteField( item.EndUserPhoneNumber  );
							csvWriter.WriteField( item.EndUserEMailAddress );
							csvWriter.WriteField( item.ShippingType );
							csvWriter.WriteField( item.SPUserName );
							csvWriter.WriteField( item.SPCompany );
							csvWriter.WriteField( item.ShippingAttn );
							csvWriter.WriteField( item.ShippingCompanyName );
							csvWriter.WriteField( item.ShippingAddress1 );
							csvWriter.WriteField( item.ShippingAddress2 );
							csvWriter.WriteField( item.ShippingCity );
							csvWriter.WriteField( item.ShippingState );
							csvWriter.WriteField( item.ShippingZip );
							csvWriter.WriteField( item.ShippingPhoneNumber );
							csvWriter.WriteField( item.ShippingEmailAddress  );
							csvWriter.WriteField( item.CanceledByUserName );
							csvWriter.WriteField( item.CanceledOnDateTime );
							foreach( var header in theItems )
							{
								csvWriter.WriteField( item.ItemNames.Keys.Contains( header ) ? item.ItemNames[header].ToString() : null );
							}
							csvWriter.NextRecord();
						}

						streamWriter.Flush();

						return new FileStreamResult( new MemoryStream( memoryStream.ToArray() ), "text/csv" ) { FileDownloadName = "collateralorders.csv" };
					}
				}
			}
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
