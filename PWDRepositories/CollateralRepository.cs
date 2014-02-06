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
				Status = cInfo.StatusValue,
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
					newOrder.PaoliMemberID = orderInfo.PaoliMemberID == 0 ? null : orderInfo.PaoliMemberID;
					newOrder.PaoliRepGroupID = orderInfo.PaoliRepGroupID == 0 ? null : orderInfo.PaoliRepGroupID;
					newOrder.PaoliRepGroupMemberID = orderInfo.PaoliRepGroupMemberID == 0 ? null : orderInfo.PaoliRepGroupMemberID;
					newOrder.DealerID = orderInfo.DealerID == 0 ? null : orderInfo.DealerID;
					newOrder.DealerMemberID = orderInfo.DealerMemberID == 0 ? null : orderInfo.DealerMemberID;
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					newOrder.PaoliRepGroupID = orderInfo.PaoliRepGroupID == 0 ? null : orderInfo.PaoliRepGroupID;
					newOrder.PaoliRepGroupMemberID = orderInfo.PaoliRepGroupMemberID == 0 ? null : orderInfo.PaoliRepGroupMemberID;
					newOrder.DealerID = orderInfo.DealerID == 0 ? null : orderInfo.DealerID;
					newOrder.DealerMemberID = orderInfo.DealerMemberID == 0 ? null : orderInfo.DealerMemberID;
					break;
				case NewOrderInformation.RPDealer:
					newOrder.DealerID = orderInfo.DealerID == 0 ? null : orderInfo.DealerID;
					newOrder.DealerMemberID = orderInfo.DealerMemberID == 0 ? null : orderInfo.DealerMemberID;
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
			newOrder.ShippingAttn = orderInfo.ShippingAttn;
			newOrder.ShippingCompanyName = orderInfo.ShippingCompanyName;
			newOrder.ShippingAddress1 = orderInfo.ShippingAddress1;
			newOrder.ShippingAddress2 = orderInfo.ShippingAddress2;
			newOrder.ShippingCity = orderInfo.ShippingCity;
			newOrder.ShippingState = orderInfo.ShippingState;
			newOrder.ShippingZip = orderInfo.ShippingZip;
			newOrder.ShippingPhoneNumber = orderInfo.ShippingPhoneNumber;
			newOrder.ShippingEmailAddress = orderInfo.ShippingEmailAddress;
			newOrder.OrderDate = DateTime.UtcNow;
			newOrder.Status = NewOrderInformation.SPending;

			foreach( var detail in orderInfo.OrderDetails.Where( o => o.Quantity > 0 ) )
			{
				var newCollateral = database.CollateralItems.FirstOrDefault( c => c.CollateralID == detail.CollateralID );
				if( newCollateral != null )
				{
					if( newCollateral.IsGroup )
					{
						newCollateral.CollateralGroupItems.ToList().ForEach( c => c.ChildCollateralItem.Quantity -= (c.Quantity * detail.Quantity) );
						newCollateral.CollateralGroupItems.ToList().ForEach( cgi => newOrder.CollateralOrderDetails.Add( new CollateralOrderDetail()
						{
							CollateralID = cgi.CollateralID,
							Quantity = cgi.Quantity * detail.Quantity,
							GroupID = detail.CollateralID
						} ) );
					}
					else
					{
						newCollateral.Quantity -= detail.Quantity;
						newOrder.CollateralOrderDetails.Add( new CollateralOrderDetail()
						{
							CollateralID = detail.CollateralID,
							Quantity = detail.Quantity
						} );
					}
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
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						collateralList = collateralList.OrderBy( v => v.Status );
					}
					else
					{
						collateralList = collateralList.OrderByDescending( v => v.Status );
					}
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
				Status = NewOrderInformation.StatusValues[c.Status],
				RequestingParty = ( c.RequestingParty == NewOrderInformation.RPPaoliMember ) ?
					( ( c.PaoliMemberID.HasValue ) ? (c.PaoliMember.FullName) : "" ) : 
					((c.RequestingParty == NewOrderInformation.RPPaoliRepresentative) ? 
						(c.PaoliSalesRep.Name + ((c.PaoliRepGroupMemberID.HasValue) ? (" (" + c.PaoliSalesRepMember.FullName + ")") : "")) : 
						((c.RequestingParty == NewOrderInformation.RPDealer) ?
							( c.Dealer.Name + ( ( c.DealerMemberID.HasValue ) ? ( " (" + c.DealerMember.FullName + ")" ) : "" ) ) : 
							(c.EndUserFirstName + " " + c.EndUserLastName))),
				CanEdit = ((c.Status != NewOrderInformation.SFulfilled) && (c.Status != NewOrderInformation.SCanceled))
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
					orderInfo.PaoliRepGroupID = eOrder.PaoliRepGroupID;
					orderInfo.PaoliRepGroupMemberID = eOrder.PaoliRepGroupMemberID;
					orderInfo.DealerID = eOrder.DealerID;
					orderInfo.DealerMemberID = eOrder.DealerMemberID;
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					orderInfo.PaoliRepGroupID = eOrder.PaoliRepGroupID;
					orderInfo.PaoliRepGroupMemberID = eOrder.PaoliRepGroupMemberID;
					orderInfo.DealerID = eOrder.DealerID;
					orderInfo.DealerMemberID = eOrder.DealerMemberID;
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
			orderInfo.ShippingAttn = eOrder.ShippingAttn;
			orderInfo.ShippingCompanyName = eOrder.ShippingCompanyName;
			orderInfo.ShippingAddress1 = eOrder.ShippingAddress1;
			orderInfo.ShippingAddress2 = eOrder.ShippingAddress2;
			orderInfo.ShippingCity = eOrder.ShippingCity;
			orderInfo.ShippingState = eOrder.ShippingState;
			orderInfo.ShippingZip = eOrder.ShippingZip;
			orderInfo.ShippingPhoneNumber = eOrder.ShippingPhoneNumber;
			orderInfo.ShippingEmailAddress = eOrder.ShippingEmailAddress;
			orderInfo.OrderDate = eOrder.OrderDate;

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
						Quantity = ci.IsGroup ? 
							eOrder.CollateralOrderDetails.Where( cod => cod.GroupID == ci.CollateralID ).Select( d => d.Quantity / d.CollateralItem.CollateralGroups.FirstOrDefault( cg => cg.GroupID == ci.CollateralID ).Quantity ).FirstOrDefault()
							: eOrder.CollateralOrderDetails.Where( cod => cod.CollateralID == ci.CollateralID && !cod.GroupID.HasValue ).Select( d => d.Quantity ).FirstOrDefault(),
						ShippedQuantity = ci.IsGroup ?
							eOrder.CollateralOrderDetails.Where( cod => cod.GroupID == ci.CollateralID ).Select( d => d.CollateralOrderShipmentDetails.Sum( s => s.Quantity ) / d.CollateralItem.CollateralGroups.FirstOrDefault( cg => cg.GroupID == ci.CollateralID ).Quantity ).DefaultIfEmpty().Max()
							: eOrder.CollateralOrderDetails.Where( cod => cod.CollateralID == ci.CollateralID && !cod.GroupID.HasValue ).Select( d => d.CollateralOrderShipmentDetails.Sum( s => s.Quantity ) ).FirstOrDefault(),
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
					eOrder.PaoliMemberID = orderInfo.PaoliMemberID == 0 ? null : orderInfo.PaoliMemberID;
					eOrder.PaoliRepGroupID = orderInfo.PaoliRepGroupID == 0 ? null : orderInfo.PaoliRepGroupID;
					eOrder.PaoliRepGroupMemberID = orderInfo.PaoliRepGroupMemberID == 0 ? null : orderInfo.PaoliRepGroupMemberID;
					eOrder.DealerID = orderInfo.DealerID == 0 ? null : orderInfo.DealerID;
					eOrder.DealerMemberID = orderInfo.DealerMemberID == 0 ? null : orderInfo.DealerMemberID;
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					eOrder.PaoliRepGroupID = orderInfo.PaoliRepGroupID == 0 ? null : orderInfo.PaoliRepGroupID;
					eOrder.PaoliRepGroupMemberID = orderInfo.PaoliRepGroupMemberID == 0 ? null : orderInfo.PaoliRepGroupMemberID;
					eOrder.DealerID = orderInfo.DealerID == 0 ? null : orderInfo.DealerID;
					eOrder.DealerMemberID = orderInfo.DealerMemberID == 0 ? null : orderInfo.DealerMemberID;
					break;
				case NewOrderInformation.RPDealer:
					eOrder.DealerID = orderInfo.DealerID == 0 ? null : orderInfo.DealerID;
					eOrder.DealerMemberID = orderInfo.DealerMemberID == 0 ? null : orderInfo.DealerMemberID;
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
			eOrder.ShippingAttn = orderInfo.ShippingAttn;
			eOrder.ShippingCompanyName = orderInfo.ShippingCompanyName;
			eOrder.ShippingAddress1 = orderInfo.ShippingAddress1;
			eOrder.ShippingAddress2 = orderInfo.ShippingAddress2;
			eOrder.ShippingCity = orderInfo.ShippingCity;
			eOrder.ShippingState = orderInfo.ShippingState;
			eOrder.ShippingZip = orderInfo.ShippingZip;
			eOrder.ShippingPhoneNumber = orderInfo.ShippingPhoneNumber;
			eOrder.ShippingEmailAddress = orderInfo.ShippingEmailAddress;

			List<int> confirmedDetails = eOrder.CollateralOrderDetails.Select( cod => cod.DetailID ).ToList();
			foreach( var uiDetail in orderInfo.OrderDetails.Where( o => o.Quantity > 0 ) )
			{
				var eCollateral = database.CollateralItems.FirstOrDefault( c => c.CollateralID == uiDetail.CollateralID );
				// find and add/update db item
				if( eCollateral.IsGroup )
				{
					// need to find and add/update child items
					if( eOrder.CollateralOrderDetails.Any( c => c.GroupID == eCollateral.CollateralID ) )
					{
						foreach( var dbDetail in eOrder.CollateralOrderDetails.Where( c => c.GroupID == eCollateral.CollateralID ) )
						{
							var dbGroup = database.CollateralGroupItems.Where( cgi => cgi.CollateralID == dbDetail.CollateralID && cgi.GroupID == dbDetail.GroupID ).FirstOrDefault();
							if( dbGroup != null )
							{
								dbDetail.CollateralItem.Quantity -= ( ( dbGroup.Quantity * uiDetail.Quantity ) - dbDetail.Quantity );
							}
							dbDetail.Quantity = dbGroup.Quantity * uiDetail.Quantity;
							confirmedDetails.Remove( dbDetail.DetailID );
						}
					}
					else
					{
						foreach( var dbItem in database.CollateralGroupItems.Where( c => c.GroupID == eCollateral.CollateralID ) )
						{
							dbItem.ChildCollateralItem.Quantity -= ( dbItem.Quantity * uiDetail.Quantity );
							eOrder.CollateralOrderDetails.Add( new CollateralOrderDetail()
							{
								CollateralID = dbItem.ChildCollateralItem.CollateralID,
								Quantity = ( dbItem.Quantity * uiDetail.Quantity ),
								GroupID = eCollateral.CollateralID
							} );
						}
					}
				}
				else
				{
					// find and update the single item
					var dbDetail = eOrder.CollateralOrderDetails.FirstOrDefault( c => c.CollateralID == eCollateral.CollateralID && !c.GroupID.HasValue );
					if( dbDetail != null )
					{
						eCollateral.Quantity -= ( uiDetail.Quantity - dbDetail.Quantity );
						dbDetail.Quantity = uiDetail.Quantity;
						confirmedDetails.Remove( dbDetail.DetailID );
					}
					else
					{
						eCollateral.Quantity -= uiDetail.Quantity;
						eOrder.CollateralOrderDetails.Add( new CollateralOrderDetail()
						{
							CollateralID = uiDetail.CollateralID,
							Quantity = uiDetail.Quantity
						} );
					}
				}
			}

			foreach( var dbDetail in eOrder.CollateralOrderDetails.Where( cod => confirmedDetails.Contains( cod.DetailID ) ).ToList() )
			{
				// delete db item
				var eCollateral = database.CollateralItems.FirstOrDefault( c => c.CollateralID == dbDetail.CollateralID );
				eCollateral.Quantity += dbDetail.Quantity;
				database.DeleteObject( dbDetail );
			}

			return database.SaveChanges() > 0;
		}

		public PendingOrderInformation GetPendingOrder( int id )
		{
			var dbOrder = database.CollateralOrders.FirstOrDefault( o => o.OrderID == id );
			if( dbOrder == null )
			{
				throw new Exception( "Unable to find Order" );
			}

			var retOrder = new PendingOrderInformation()
			{
				OrderID = dbOrder.OrderID,
				RequestingPartyID = dbOrder.RequestingParty,
				RequestingParty = NewOrderInformation.RequestingParties[dbOrder.RequestingParty],
				PaoliMember = dbOrder.PaoliMemberID.HasValue ? dbOrder.PaoliMember.FullName : "None",
				PaoliRepGroup = dbOrder.PaoliRepGroupID.HasValue ? dbOrder.PaoliSalesRep.Name : "None",
				PaoliRepGroupMember = dbOrder.PaoliRepGroupMemberID.HasValue ? dbOrder.PaoliSalesRepMember.FullName : "None",
				Dealer = dbOrder.DealerID.HasValue ? dbOrder.Dealer.Name : "None",
				DealerMember = dbOrder.DealerMemberID.HasValue ? dbOrder.DealerMember.FullName : "None",
				EndUserFirstName = dbOrder.EndUserFirstName,
				EndUserLastName = dbOrder.EndUserLastName,
				EndUserPhoneNumber = dbOrder.EndUserPhoneNumber,
				EndUserEMailAddress = dbOrder.EndUserEMailAddress,

				ShippingType = NewOrderInformation.ShippingTypes[dbOrder.ShippingType],
				ShippingTypeID = dbOrder.ShippingType,
				ShippingFedexAccount = dbOrder.ShippingFedexAccount,
				ShippingAttn = dbOrder.ShippingAttn,
				ShippingCompanyName = dbOrder.ShippingCompanyName,
				ShippingAddress1 = dbOrder.ShippingAddress1,
				ShippingAddress2 = dbOrder.ShippingAddress2,
				ShippingCity = dbOrder.ShippingCity,
				ShippingState = dbOrder.ShippingState,
				ShippingZip = dbOrder.ShippingZip,
				ShippingPhoneNumber = dbOrder.ShippingPhoneNumber,
				ShippingEmailAddress = dbOrder.ShippingEmailAddress,

				OrderDate = dbOrder.OrderDate,
				Status = NewOrderInformation.StatusValues[dbOrder.Status],

				CreatedByUserName = dbOrder.CreatedByUser.FullName,
				CreatedByCompany = dbOrder.CreatedByUser.Company.Name,
				CreatedByEmailAddress = dbOrder.CreatedByUser.Email,
				CreatedByPhoneNumber = dbOrder.CreatedByUser.BusinessPhone,

				CanceledByUserName = dbOrder.CanceledByUserID.HasValue ? dbOrder.CanceledByUser.FullName : null,
				CanceledOnDateTime = dbOrder.CanceledOnDateTime
			};

			retOrder.OrderDetails = new List<PendingOrderInformation.PendingOrderDetail>();

			foreach( var oDetail in dbOrder.CollateralOrderDetails )
			{
				retOrder.OrderDetails.Add( new PendingOrderInformation.PendingOrderDetail()
				{
					DetailID = oDetail.DetailID,
					CollateralID = oDetail.CollateralID,
					Name = oDetail.CollateralItem.Name,
					Quantity = oDetail.Quantity,
					RemainingQuantity = oDetail.Quantity - oDetail.CollateralOrderShipmentDetails.Sum( c => c.Quantity ),
					GroupID = oDetail.GroupID,
					GroupName = oDetail.GroupID.HasValue ? oDetail.CollateralItemGroup.Name : null,
					CollateralTypeID = oDetail.GroupID.HasValue ? 0 : oDetail.CollateralItem.CollateralTypeID.Value,
					CollateralType = oDetail.GroupID.HasValue ? null : oDetail.CollateralItem.CollateralType.Name
				} );
			}

			retOrder.OrderDetails.Where( a => a.RemainingQuantity <= 0 ).ToList()
				.ForEach( a => retOrder.OrderDetails.Remove( a ) );

			retOrder.OrderDetails = retOrder.OrderDetails
				.OrderByDescending( o => o.GroupID.HasValue )
				.ThenBy( o => o.GroupName )
				.ThenBy( o => o.CollateralType )
				.ThenBy( o => o.Name )
				.ToList();

			retOrder.Shipments = new List<PendingOrderInformation.ShipmentSummary>();

			foreach( var shipment in dbOrder.CollateralOrderShipments )
			{
				var sInfo = new PendingOrderInformation.ShipmentSummary()
				{
					ShipmentID = shipment.ShipmentID,
					Vendor = shipment.Vendor,
					TrackingNumber = shipment.TrackingNumber,
					GLCode = shipment.GLCode,
					ShippingType = shipment.ShippingType,
					ShippingFedexAccount = shipment.ShippingFedexAccount,
					ShippingAttn = shipment.ShippingAttn,
					ShippingCompanyName = shipment.ShippingCompanyName,
					ShippingAddress1 = shipment.ShippingAddress1,
					ShippingAddress2 = shipment.ShippingAddress2,
					ShippingCity = shipment.ShippingCity,
					ShippingState = shipment.ShippingState,
					ShippingZip = shipment.ShippingZip,
					ShippingPhoneNumber = shipment.ShippingPhoneNumber,
					ShippingEmailAddress = shipment.ShippingEmailAddress,
				};

				sInfo.Details = new List<PendingOrderInformation.ShipmentDetailSummary>();

				foreach( var sDetail in shipment.CollateralOrderShipmentDetails )
				{
					sInfo.Details.Add( new PendingOrderInformation.ShipmentDetailSummary()
						{
							OrderDetailID = sDetail.OrderDetailID,
							Name = sDetail.CollateralOrderDetail.CollateralItem.Name,
							Quantity = sDetail.Quantity
						} );
				}

				retOrder.Shipments.Add( sInfo );
			}

			return retOrder;
		}

		public bool AddOrderShipment( int orderID, PendingOrderInformation.ShipmentSummary shipmentInfo )
		{
			var dbOrder = database.CollateralOrders.FirstOrDefault( c => c.OrderID == orderID );
			if( dbOrder == null )
			{
				throw new Exception( "Unable to find order for shipment." );
			}

			var dbShipment = new CollateralOrderShipment();
			dbShipment.Vendor = shipmentInfo.Vendor;
			dbShipment.TrackingNumber = shipmentInfo.TrackingNumber;
			dbShipment.GLCode = shipmentInfo.GLCode;
			dbShipment.ShippingType = NewOrderInformation.ShippingTypes[dbOrder.ShippingType];
			dbShipment.ShippingFedexAccount = dbOrder.ShippingFedexAccount;
			dbShipment.ShippingAttn = dbOrder.ShippingAttn;
			dbShipment.ShippingCompanyName = dbOrder.ShippingCompanyName;
			dbShipment.ShippingAddress1 = dbOrder.ShippingAddress1;
			dbShipment.ShippingAddress2 = dbOrder.ShippingAddress2;
			dbShipment.ShippingCity = dbOrder.ShippingCity;
			dbShipment.ShippingState = dbOrder.ShippingState;
			dbShipment.ShippingZip = dbOrder.ShippingZip;
			dbShipment.ShippingPhoneNumber = dbOrder.ShippingPhoneNumber;
			dbShipment.ShippingEmailAddress = dbOrder.ShippingEmailAddress;

			foreach( var detail in shipmentInfo.Details )
			{
				var dbDetail = dbOrder.CollateralOrderDetails.FirstOrDefault( d => d.DetailID == detail.OrderDetailID );
				if( dbDetail == null )
				{
					throw new Exception( "Unable to find order detail for shipment." );
				}

				var dbShipmentItem = new CollateralOrderShipmentDetail();

				dbShipmentItem.CollateralOrderDetail = dbDetail;
				dbShipmentItem.Quantity = detail.Quantity;

				dbShipment.CollateralOrderShipmentDetails.Add( dbShipmentItem );
			}

			dbOrder.CollateralOrderShipments.Add( dbShipment );

			dbOrder.Status = NewOrderInformation.SFulfilled;
			foreach( var detail in dbOrder.CollateralOrderDetails )
			{
				if( detail.Quantity > detail.CollateralOrderShipmentDetails.Sum( d => d.Quantity ) )
				{
					dbOrder.Status = NewOrderInformation.SPartial;
					break;
				}
			}

			return database.SaveChanges() > 0;
		}

		public bool CancelOrder( int orderID )
		{
			var dbOrder = database.CollateralOrders.FirstOrDefault( c => c.OrderID == orderID );
			if( dbOrder == null )
			{
				return false;
			}

			if( dbOrder.CollateralOrderShipments.Any() )
			{
				dbOrder.Status = NewOrderInformation.SFulfilled;
			}
			else
			{
				dbOrder.Status = NewOrderInformation.SCanceled;
			}

			dbOrder.CanceledByUserID = PaoliWebUser.CurrentUser.UserId;
			dbOrder.CanceledOnDateTime = DateTime.UtcNow;

			return database.SaveChanges() > 0;			
		}

		#endregion
	}
}
