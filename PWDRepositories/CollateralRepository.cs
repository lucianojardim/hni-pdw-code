using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using PDWInfrastructure;
using PDWModels.Collateral;
using System.IO;
using System.Drawing;
using System.Configuration;

namespace PWDRepositories
{
	public class CollateralRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		public CollateralRepository()
		{
		}

		public CollateralSummary ToCollateralSummary( CollateralItem cItem )
		{
			return new CollateralSummary()
			{
				CollateralID = cItem.CollateralID,
				Name = cItem.Name,
				CollateralType = cItem.IsGroup ? "Bundle" : cItem.CollateralType.Name,
				Status = cItem.StatusValue,
				Quantity = cItem.ComputeQuantity,
				IsGroup = cItem.IsGroup
			};
		}

		public IEnumerable<CollateralSummary> GetFullCollateralList( CollateralTableParams param, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var collateralList = database.CollateralItems.AsQueryable();

			totalRecords = collateralList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
				collateralList = collateralList.Where( i =>
					i.Name.Contains( param.sSearch ) );
			}
			if( param.collateralType > 0 )
			{
				collateralList = collateralList.Where( c => c.CollateralTypeID == param.collateralType );
			}
			else if( param.collateralType == -1 )
			{
				collateralList = collateralList.Where( c => c.IsGroup );
			}

			displayedRecords = collateralList.Count();

			var localList = collateralList.ToList().Select( c => ToCollateralSummary( c ) ).AsQueryable();

			string sortCol = param.sColumns.Split( ',' )[param.iSortCol_0];

			switch( sortCol.ToLower() )
			{
				case "name":
				default:
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						localList = localList.OrderBy( v => v.Name );
					}
					else
					{
						localList = localList.OrderByDescending( v => v.Name );
					}
					break;
				case "collateraltype":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						localList = localList.OrderBy( v => v.CollateralType );
					}
					else
					{
						localList = localList.OrderByDescending( v => v.CollateralType );
					}
					break;
				case "status":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						localList = localList.OrderBy( v => v.Status );
					}
					else
					{
						localList = localList.OrderByDescending( v => v.Status );
					}
					break;
				case "quantity":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						localList = localList.OrderBy( v => v.Quantity );
					}
					else
					{
						localList = localList.OrderByDescending( v => v.Quantity );
					}
					break;
			}

			if( ( displayedRecords > param.iDisplayLength ) && ( param.iDisplayLength > 0 ) )
			{
				localList = localList.Skip( param.iDisplayStart ).Take( param.iDisplayLength );
			}

			return localList;
		}


		public bool AddCollateral( CollateralInformation cInfo, Stream collStream, string fileName )
		{
			CollateralItem newItem = new CollateralItem();

			newItem.CollateralID = cInfo.CollateralID;
			newItem.Name = cInfo.Name;
			newItem.CollateralTypeID = cInfo.CollateralTypeID;
			newItem.Description = cInfo.Description;
			newItem.LeadTime = cInfo.LeadTime;
			newItem.Weight = cInfo.Weight;
			newItem.StatusDate = cInfo.StatusDate;
			newItem.Quantity = cInfo.Quantity;
			newItem.Price = cInfo.Price;
			newItem.Shipping = cInfo.Shipping;

			if( collStream != null )
			{
				newItem.ImageFileName = Guid.NewGuid().ToString() + Path.GetExtension( fileName );

				Image fullSizeImg = Image.FromStream( collStream );
				fullSizeImg.Save( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"],
					newItem.ImageFileName ) );
			}

			database.CollateralItems.AddObject( newItem );

			if( cInfo is CollateralGroupInformation )
			{
				newItem.CollateralTypeID = null;
				newItem.IsGroup = true;

				foreach( var groupCollateral in ( cInfo as CollateralGroupInformation ).GroupItems )
				{
					var childItem = database.CollateralItems.FirstOrDefault( ci => ci.CollateralID == groupCollateral.ItemID );
					if( childItem == null )
					{
						throw new Exception( "Unable to find Collateral." );
					}

					var newcgi = new CollateralGroupItem()
					{
						GroupCollateralItem = newItem,
						ChildCollateralItem = childItem,
						Quantity = groupCollateral.Quantity
					};
					database.CollateralGroupItems.AddObject( newcgi );
				}
			}

			return database.SaveChanges() > 0;
		}

		public CollateralInformation GetCollateral( int collateralId )
		{
			var cInfo = database.CollateralItems.FirstOrDefault( c => c.CollateralID == collateralId );
			if( cInfo == null )
			{
				throw new Exception( "Unable to find collateral." );
			}

			return new CollateralInformation()
			{
				CollateralID = cInfo.CollateralID,
				Name = cInfo.Name,
				CollateralTypeID = cInfo.CollateralTypeID,
				Description = cInfo.Description,
				LeadTime = cInfo.LeadTime,
				Weight = cInfo.Weight,
				StatusDate = cInfo.StatusDate,
				Quantity = cInfo.Quantity,
				ImageFileName = cInfo.ImageFileName,
				Price = cInfo.Price,
				Shipping = cInfo.Shipping
			};
		}

		public CollateralGroupInformation GetCollateralGroup( int collateralId )
		{
			var cInfo = database.CollateralItems.FirstOrDefault( c => c.CollateralID == collateralId );
			if( cInfo == null )
			{
				throw new Exception( "Unable to find collateral." );
			}
			if( !cInfo.IsGroup )
			{
				throw new Exception( "Unable to find collateral." );
			}
			return new CollateralGroupInformation()
			{
				CollateralID = cInfo.CollateralID,
				Name = cInfo.Name,
				CollateralTypeID = cInfo.CollateralTypeID,
				Description = cInfo.Description,
				LeadTime = cInfo.LeadTime,
				Weight = cInfo.Weight,
				StatusDate = cInfo.StatusDate,
				Quantity = cInfo.Quantity,
				ImageFileName = cInfo.ImageFileName,
				Price = cInfo.Price,
				Shipping = cInfo.Shipping,
				GroupItems = cInfo.CollateralGroupItems
					.Select( cgi => new CollateralGroupInformation.GroupInfoDetail()
					{
						ItemID = cgi.ChildCollateralItem.CollateralID,
						Quantity = cgi.Quantity
					} ).ToList()
			};
		}

		public bool UpdateCollateral( CollateralInformation cInfo, Stream collStream, string fileName )
		{
			var cItem = database.CollateralItems.FirstOrDefault( c => c.CollateralID == cInfo.CollateralID );
			if( cItem == null )
			{
				throw new Exception( "Unable to find collateral." );
			}

			cItem.CollateralID = cInfo.CollateralID;
			cItem.Name = cInfo.Name;
			cItem.CollateralTypeID = cInfo.CollateralTypeID;
			cItem.Description = cInfo.Description;
			cItem.LeadTime = cInfo.LeadTime;
			cItem.Weight = cInfo.Weight;
			cItem.StatusDate = cInfo.StatusDate;
			cItem.Quantity = cInfo.Quantity;
			cItem.Price = cInfo.Price;
			cItem.Shipping = cInfo.Shipping;

			if( collStream != null )
			{
				cItem.ImageFileName = Guid.NewGuid().ToString() + Path.GetExtension( fileName );

				Image fullSizeImg = Image.FromStream( collStream );
				fullSizeImg.Save( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"],
					cItem.ImageFileName ) );
			}

			if( cInfo is CollateralGroupInformation )
			{
				cItem.CollateralTypeID = null;
				cItem.IsGroup = true;

				cItem.CollateralGroupItems.ToList().ForEach( cgi => database.DeleteObject( cgi ) );

				foreach( var groupCollateral in ( cInfo as CollateralGroupInformation ).GroupItems )
				{
					var childItem = database.CollateralItems.FirstOrDefault( ci => ci.CollateralID == groupCollateral.ItemID );
					if( childItem == null )
					{
						throw new Exception( "Unable to find Collateral." );
					}

					var newcgi = new CollateralGroupItem()
					{
						GroupCollateralItem = cItem,
						ChildCollateralItem = childItem,
						Quantity = groupCollateral.Quantity
					};
					database.CollateralGroupItems.AddObject( newcgi );
				}
			}

			return database.SaveChanges() > 0;
		}

		public bool DeleteCollateral( int collateralId )
		{
			var cItem = database.CollateralItems.FirstOrDefault( c => c.CollateralID == collateralId );
			if( cItem == null )
			{
				throw new Exception( "Unable to find collateral." );
			}

			database.DeleteObject( cItem );

			return database.SaveChanges() > 0;
		}

		public Dictionary<int, string> GetCollateralTypeList()
		{
			return database.CollateralTypes
				.OrderBy( c => c.Name )
				.ToDictionary( c => c.CollateralTypeID, c => c.Name );
		}

		public Dictionary<int, string> GetCollateralList( bool includeGroups )
		{
			return database.CollateralItems
				.Where( c => !c.IsGroup || includeGroups )
				.OrderBy( c => c.Name )
				.ToDictionary( c => c.CollateralID, c => c.Name );
		}

		public bool AddCollateralShipment( List<CollateralShipmentDetail> theList )
		{
			foreach( var detail in theList )
			{
				var cItem = database.CollateralItems.FirstOrDefault( c => c.CollateralID == detail.CollateralID );
				if( cItem == null )
				{
					throw new Exception( "Unable to find collateral." );
				}

				cItem.Quantity += detail.Quantity;
			}

			return database.SaveChanges() > 0;
		}

		#region Order Information
		public NewOrderInformation BlankOrderInformation()
		{
			var orderInfo = new NewOrderInformation();

			orderInfo.OrderDetails =
				database.CollateralItems
					.OrderBy( c => c.Name )
					.ToList()
					.Select( ci => new NewOrderInformation.OrderDetail()
					{
						CollateralID = ci.CollateralID,
						CollateralTypeID = ci.CollateralTypeID.HasValue ? ci.CollateralTypeID.Value : -1,
						Description = ci.Description,
						Name = ci.Name,
						Quantity = 0,
						Status = ci.StatusValue
					} )
					.ToList();

			return orderInfo;
		}

		public bool AddCollateralOrder( NewOrderInformation orderInfo )
		{
			var newOrder = new CollateralOrder();

			newOrder.RequestingParty = orderInfo.RequestingParty;
			switch( newOrder.RequestingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					newOrder.PaoliMemberID = orderInfo.PaoliMemberID;
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					newOrder.PaoliRepGroupID = orderInfo.PaoliRepGroupID;
					newOrder.PaoliRepGroupMemberID = orderInfo.PaoliRepGroupMemberID;
					break;
				case NewOrderInformation.RPDealer:
					newOrder.DealerID = orderInfo.DealerID;
					newOrder.DealerMemberID = orderInfo.DealerMemberID;
					break;
				case NewOrderInformation.RPEndUser:
					newOrder.EndUserFirstName = orderInfo.EndUserFirstName;
					newOrder.EndUserLastName = orderInfo.EndUserLastName;
					newOrder.EndUserPhoneNumber = orderInfo.EndUserPhoneNumber;
					newOrder.EndUserEMailAddress = orderInfo.EndUserEMailAddress;
					break;
			}
			newOrder.ShippingType = orderInfo.ShippingType;
			newOrder.ShippingAddressType = orderInfo.ShippingAddressType;
			newOrder.ShippingFedexAccount = orderInfo.ShippingFedexAccount;
			newOrder.ShippingFirstName = orderInfo.ShippingFirstName;
			newOrder.ShippingLastName = orderInfo.ShippingLastName;
			newOrder.ShippingCompanyName = orderInfo.ShippingCompanyName;
			newOrder.ShippingAddress1 = orderInfo.ShippingAddress1;
			newOrder.ShippingAddress2 = orderInfo.ShippingAddress2;
			newOrder.ShippingCity = orderInfo.ShippingCity;
			newOrder.ShippingState = orderInfo.ShippingState;
			newOrder.ShippingZip = orderInfo.ShippingZip;
			newOrder.ShippingPhoneNumber = orderInfo.ShippingPhoneNumber;
			newOrder.ShippingEmailAddress = orderInfo.ShippingEmailAddress;
			newOrder.OrderDate = DateTime.UtcNow;

			foreach( var detail in orderInfo.OrderDetails.Where( o => o.Quantity > 0 ) )
			{
				var newCollateral = database.CollateralItems.FirstOrDefault( c => c.CollateralID == detail.CollateralID );
				if( newCollateral != null )
				{
					if( newCollateral.IsGroup )
					{
						newCollateral.CollateralGroupItems.ToList().ForEach( c => c.ChildCollateralItem.Quantity -= (c.Quantity * detail.Quantity) );
					}
					else
					{
						newCollateral.Quantity -= detail.Quantity;
					}
					newOrder.CollateralOrderDetails.Add( new CollateralOrderDetail()
					{
						CollateralID = detail.CollateralID,
						Quantity = detail.Quantity
					} );
				}
			}

			database.CollateralOrders.AddObject( newOrder );

			return database.SaveChanges() > 0;
		}

		public IEnumerable<CollateralOrderSummary> GetFullCollateralOrderList( CollateralOrderTableParams param, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var collateralList = database.CollateralOrders.AsQueryable();

			totalRecords = collateralList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
			}

			displayedRecords = collateralList.Count();

			string sortCol = param.sColumns.Split( ',' )[param.iSortCol_0];

			switch( sortCol.ToLower() )
			{
				case "editbuttons":
				case "orderid":
				default:
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						collateralList = collateralList.OrderBy( v => v.OrderID );
					}
					else
					{
						collateralList = collateralList.OrderByDescending( v => v.OrderID );
					}
					break;
				case "requestingparty":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						collateralList = collateralList.OrderBy( v => v.OrderID );
					}
					else
					{
						collateralList = collateralList.OrderByDescending( v => v.OrderID );
					}
					break;
				case "orderdate":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						collateralList = collateralList.OrderBy( v => v.OrderDate );
					}
					else
					{
						collateralList = collateralList.OrderByDescending( v => v.OrderDate );
					}
					break;
				case "status":
/*					if( param.sSortDir_0.ToLower() == "asc" )
					{
						collateralList = collateralList.OrderBy( v => v.Quantity );
					}
					else
					{
						collateralList = collateralList.OrderByDescending( v => v.Quantity );
					}*/
					break;
			}

			if( ( displayedRecords > param.iDisplayLength ) && ( param.iDisplayLength > 0 ) )
			{
				collateralList = collateralList.Skip( param.iDisplayStart ).Take( param.iDisplayLength );
			}

			return collateralList.ToList().Select( c => ToCollateralOrderSummary( c ) );
		}

		private CollateralOrderSummary ToCollateralOrderSummary( CollateralOrder c )
		{
			return new CollateralOrderSummary()
			{
				OrderID = c.OrderID,
				OrderDate = c.OrderDate,
				Status = "Pending",
				RequestingParty = ( c.RequestingParty == NewOrderInformation.RPPaoliMember ) ?
					( ( c.PaoliMemberID.HasValue ) ? (c.PaoliMember.FullName) : "" ) : 
					((c.RequestingParty == NewOrderInformation.RPPaoliRepresentative) ? 
						(c.PaoliSalesRep.Name + ((c.PaoliRepGroupMemberID.HasValue) ? (" (" + c.PaoliSalesRepMember.FullName + ")") : "")) : 
						((c.RequestingParty == NewOrderInformation.RPDealer) ?
							( c.Dealer.Name + ( ( c.DealerMemberID.HasValue ) ? ( " (" + c.DealerMember.FullName + ")" ) : "" ) ) : 
							(c.EndUserFirstName + " " + c.EndUserLastName)))
			};
		}

		public NewOrderInformation GetOrderInformation( int id )
		{
			var eOrder = database.CollateralOrders.FirstOrDefault( co => co.OrderID == id );
			if( eOrder == null )
			{
				throw new Exception( "Unable to find Collateral Order" );
			}

			var orderInfo = new NewOrderInformation();
			orderInfo.OrderID = eOrder.OrderID;
			orderInfo.RequestingParty = eOrder.RequestingParty;
			switch( orderInfo.RequestingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					orderInfo.PaoliMemberID = eOrder.PaoliMemberID;
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					orderInfo.PaoliRepGroupID = eOrder.PaoliRepGroupID;
					orderInfo.PaoliRepGroupMemberID = eOrder.PaoliRepGroupMemberID;
					break;
				case NewOrderInformation.RPDealer:
					orderInfo.DealerID = eOrder.DealerID;
					orderInfo.DealerMemberID = eOrder.DealerMemberID;
					break;
				case NewOrderInformation.RPEndUser:
					orderInfo.EndUserFirstName = eOrder.EndUserFirstName;
					orderInfo.EndUserLastName = eOrder.EndUserLastName;
					orderInfo.EndUserPhoneNumber = eOrder.EndUserPhoneNumber;
					orderInfo.EndUserEMailAddress = eOrder.EndUserEMailAddress;
					break;
			}
			orderInfo.ShippingType = eOrder.ShippingType;
			orderInfo.ShippingAddressType = eOrder.ShippingAddressType;
			orderInfo.ShippingFedexAccount = eOrder.ShippingFedexAccount;
			orderInfo.ShippingFirstName = eOrder.ShippingFirstName;
			orderInfo.ShippingLastName = eOrder.ShippingLastName;
			orderInfo.ShippingCompanyName = eOrder.ShippingCompanyName;
			orderInfo.ShippingAddress1 = eOrder.ShippingAddress1;
			orderInfo.ShippingAddress2 = eOrder.ShippingAddress2;
			orderInfo.ShippingCity = eOrder.ShippingCity;
			orderInfo.ShippingState = eOrder.ShippingState;
			orderInfo.ShippingZip = eOrder.ShippingZip;
			orderInfo.ShippingPhoneNumber = eOrder.ShippingPhoneNumber;
			orderInfo.ShippingEmailAddress = eOrder.ShippingEmailAddress;
			orderInfo.OrderDate = DateTime.UtcNow;

			orderInfo.OrderDetails =
				database.CollateralItems
					.OrderBy( c => c.Name )
					.ToList()
					.Select( ci => new NewOrderInformation.OrderDetail()
					{
						CollateralID = ci.CollateralID,
						CollateralTypeID = ci.CollateralTypeID.HasValue ? ci.CollateralTypeID.Value : -1,
						Description = ci.Description,
						Name = ci.Name,
						Quantity = eOrder.CollateralOrderDetails.Where( cod => cod.CollateralID == ci.CollateralID ).Select( d => d.Quantity ).FirstOrDefault(),
						Status = ci.StatusValue
					} )
					.ToList();

			return orderInfo;
		}

		public bool UpdateCollateralOrder( NewOrderInformation orderInfo )
		{
			var eOrder = database.CollateralOrders.FirstOrDefault( co => co.OrderID == orderInfo.OrderID );
			if( eOrder == null )
			{
				throw new Exception( "Unable to find Collateral Order" );
			}

			eOrder.RequestingParty = orderInfo.RequestingParty;
			switch( eOrder.RequestingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					eOrder.PaoliMemberID = orderInfo.PaoliMemberID;
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					eOrder.PaoliRepGroupID = orderInfo.PaoliRepGroupID;
					eOrder.PaoliRepGroupMemberID = orderInfo.PaoliRepGroupMemberID;
					break;
				case NewOrderInformation.RPDealer:
					eOrder.DealerID = orderInfo.DealerID;
					eOrder.DealerMemberID = orderInfo.DealerMemberID;
					break;
				case NewOrderInformation.RPEndUser:
					eOrder.EndUserFirstName = orderInfo.EndUserFirstName;
					eOrder.EndUserLastName = orderInfo.EndUserLastName;
					eOrder.EndUserPhoneNumber = orderInfo.EndUserPhoneNumber;
					eOrder.EndUserEMailAddress = orderInfo.EndUserEMailAddress;
					break;
			}
			eOrder.ShippingType = orderInfo.ShippingType;
			eOrder.ShippingAddressType = orderInfo.ShippingAddressType;
			eOrder.ShippingFedexAccount = orderInfo.ShippingFedexAccount;
			eOrder.ShippingFirstName = orderInfo.ShippingFirstName;
			eOrder.ShippingLastName = orderInfo.ShippingLastName;
			eOrder.ShippingCompanyName = orderInfo.ShippingCompanyName;
			eOrder.ShippingAddress1 = orderInfo.ShippingAddress1;
			eOrder.ShippingAddress2 = orderInfo.ShippingAddress2;
			eOrder.ShippingCity = orderInfo.ShippingCity;
			eOrder.ShippingState = orderInfo.ShippingState;
			eOrder.ShippingZip = orderInfo.ShippingZip;
			eOrder.ShippingPhoneNumber = orderInfo.ShippingPhoneNumber;
			eOrder.ShippingEmailAddress = orderInfo.ShippingEmailAddress;

			foreach( var detail in eOrder.CollateralOrderDetails.ToList() )
			{
				var eCollateral = database.CollateralItems.FirstOrDefault( c => c.CollateralID == detail.CollateralID );
				if( eCollateral != null )
				{
					var uiDetail = orderInfo.OrderDetails.FirstOrDefault( o => o.CollateralID == detail.CollateralID );
					if( ( uiDetail != null ) && ( uiDetail.Quantity > 0 ) )
					{
						if( eCollateral.IsGroup )
						{
							eCollateral.CollateralGroupItems.ToList().ForEach( c => c.ChildCollateralItem.Quantity -= ( c.Quantity * ( uiDetail.Quantity - detail.Quantity ) ) );
						}
						else
						{
							eCollateral.Quantity -= (uiDetail.Quantity - detail.Quantity);
						}

						detail.Quantity = uiDetail.Quantity;
					}
					else
					{
						if( eCollateral.IsGroup )
						{
							eCollateral.CollateralGroupItems.ToList().ForEach( c => c.ChildCollateralItem.Quantity += ( c.Quantity * detail.Quantity ) );
						}
						else
						{
							eCollateral.Quantity += detail.Quantity;
						}
						database.DeleteObject( detail );
					}
				}
			}

			foreach( var detail in orderInfo.OrderDetails
				.Where( o => o.Quantity > 0 && !eOrder.CollateralOrderDetails.Any( c => c.CollateralID == o.CollateralID ) ) )
			{
				var newCollateral = database.CollateralItems.FirstOrDefault( c => c.CollateralID == detail.CollateralID );
				if( newCollateral != null )
				{
					if( newCollateral.IsGroup )
					{
						newCollateral.CollateralGroupItems.ToList().ForEach( c => c.ChildCollateralItem.Quantity -= ( c.Quantity * detail.Quantity ) );
					}
					else
					{
						newCollateral.Quantity -= detail.Quantity;
					}
					eOrder.CollateralOrderDetails.Add( new CollateralOrderDetail()
					{
						CollateralID = detail.CollateralID,
						Quantity = detail.Quantity
					} );
				}
			}

			return database.SaveChanges() > 0;
		}

		#endregion
	}
}
