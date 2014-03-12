using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDWInfrastructure.EmailSenders
{
	public class NewCollateralOrderShipmentEmailSender : EmailSender
	{
		public class EmailShipmentSummary
		{
			public int orderID { get; set; }
			public string firstName { get; set; }
			public string shippingName { get; set; }
			public string shippingCompany { get; set; }
			public string shippingAddress1 { get; set; }
			public string shippingAddress2 { get; set; }
			public string shippingCity { get; set; }
			public string shippingState { get; set; }
			public string shippingZip { get; set; }
			public string shippingSpeed { get; set; }
			public List<string> shipmentDetailList { get; set; }

			public string shippingCarrier { get; set; }
			public List<string> trackingNumbers { get; set; }

			public string fullShippingAddress
			{
				get
				{
					var address = shippingName;
					if( ( shippingCompany ?? "" ).Any() )
					{
						address += ( "<br/>" + shippingCompany );
					}
					address += ( "<br/>" + shippingAddress1 );
					if( ( shippingAddress2 ?? "" ).Any() )
					{
						address += ( "<br/>" + shippingAddress2 );
					}
					address += ( "<br/>" + shippingCity + ", " + shippingState + " " + shippingZip );

					return address;
				}
			}
		}

		public NewCollateralOrderShipmentEmailSender( string template )
		{
			TemplateName = template;
		}

		private string TemplateName { get; set; }

		public bool SubmitNewShipmentEmail(
			string emailAddress,
			EmailShipmentSummary summary )
		{
			try
			{
				StringBuilder template;
				if( ReadEmailTemplate( TemplateName, out template ) )
				{
					// perform substitutions
					template.Replace( "[{OrderID}]", summary.orderID.ToString() );

					template.Replace( "[{FirstName}]", summary.firstName );

					template.Replace( "[{ShipmentDetailList}]", string.Join( "<br/>", summary.shipmentDetailList ) );

					template.Replace( "[{FullShippingNameAddress}]", summary.fullShippingAddress );

					template.Replace( "[{ShippingSpeed}]", summary.shippingSpeed );
					template.Replace( "[{ShippingCarrier}]", summary.shippingCarrier );
					template.Replace( "[{TrackingNumber}]", string.Join( ", ", summary.trackingNumbers.Where( t => ( t ?? "" ).Any() ) ) );

					template.Replace( "[{TrackingPlural}]", summary.trackingNumbers.Where( t => ( t ?? "" ).Any() ).Count() > 1 ? "s are" : " is" );

					return SubmitEmail( new List<string>() { emailAddress }, null, null, GetSubject( template ), template.ToString() );
				}
			}
			catch( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine( "Unable to send email: {0}", ex.Message );
			}

			return false;
		}
	}
}
