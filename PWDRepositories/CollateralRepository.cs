﻿using System;
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

			orderInfo.PaoliMemberID = null;
			orderInfo.DealerID = null;
			orderInfo.DealerMemberID = null;
			orderInfo.PaoliRepGroupID = null;
			orderInfo.PaoliRepGroupMemberID = null;
			orderInfo.SPPaoliMemberID = null;
			orderInfo.SPDealerID = null;
			orderInfo.SPDealerMemberID = null;
			orderInfo.SPPaoliRepGroupID = null;
			orderInfo.SPPaoliRepGroupMemberID = null;

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
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					newOrder.PaoliRepGroupID = orderInfo.PaoliRepGroupID == 0 ? null : orderInfo.PaoliRepGroupID;
					newOrder.PaoliRepGroupMemberID = orderInfo.PaoliRepGroupMemberID == 0 ? null : orderInfo.PaoliRepGroupMemberID;
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
			newOrder.ShippingParty = orderInfo.ShippingParty;
			switch( newOrder.ShippingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					newOrder.SPPaoliMemberID = orderInfo.SPPaoliMemberID == 0 ? null : orderInfo.SPPaoliMemberID;
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					newOrder.SPPaoliRepGroupID = orderInfo.SPPaoliRepGroupID == 0 ? null : orderInfo.SPPaoliRepGroupID;
					newOrder.SPPaoliRepGroupMemberID = orderInfo.SPPaoliRepGroupMemberID == 0 ? null : orderInfo.SPPaoliRepGroupMemberID;
					break;
				case NewOrderInformation.RPDealer:
					newOrder.SPDealerID = orderInfo.SPDealerID == 0 ? null : orderInfo.SPDealerID;
					newOrder.SPDealerMemberID = orderInfo.SPDealerMemberID == 0 ? null : orderInfo.SPDealerMemberID;
					break;
				case NewOrderInformation.RPEndUser:
					newOrder.SPEndUserFirstName = orderInfo.SPEndUserFirstName;
					newOrder.SPEndUserLastName = orderInfo.SPEndUserLastName;
					newOrder.SPEndUserPhoneNumber = orderInfo.SPEndUserPhoneNumber;
					newOrder.SPEndUserEMailAddress = orderInfo.SPEndUserEMailAddress;
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
			newOrder.CreatedByUserID = PaoliWebUser.CurrentUser.UserId;

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

		public IEnumerable<CollateralOrderSummary> GetFullCollateralOrderListForUser( CollateralOrderTableParams param, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var collateralList = database.CollateralOrders
				.AsQueryable();

			var user = database.Users.FirstOrDefault( u => u.UserID == PaoliWebUser.CurrentUser.UserId );
			if( user == null )
			{
				throw new Exception( "Unable to find current user" );
			}

			if( PaoliWebUser.CurrentUser.IsInRole( PaoliWebUser.PaoliWebRole.PaoliSalesRep ) )
			{
				// all orders with PaoliRepGroupID, SPPaoliRepGroupID
				collateralList = collateralList.Where( c =>
					( ( c.RequestingParty == NewOrderInformation.RPPaoliRepresentative ) && ( c.PaoliRepGroupID == user.CompanyID ) ) ||
					( ( c.ShippingParty == NewOrderInformation.RPPaoliRepresentative ) && ( c.SPPaoliRepGroupID == user.CompanyID ) ) ||
					( ( c.RequestingParty == NewOrderInformation.RPDealer ) && ( c.Dealer.TerritoryID == user.Company.TerritoryID ) ) ||
					( ( c.ShippingParty == NewOrderInformation.RPDealer ) && ( c.SPDealer.TerritoryID == user.Company.TerritoryID ) ) );
			}
			else if( PaoliWebUser.CurrentUser.IsDealerUser )
			{
				collateralList = collateralList.Where( c =>
					( ( c.RequestingParty == NewOrderInformation.RPDealer ) && ( c.Dealer.CompanyID == user.CompanyID ) ) ||
					( ( c.ShippingParty == NewOrderInformation.RPDealer ) && ( c.SPDealer.CompanyID == user.CompanyID ) ) );
			}

			totalRecords = collateralList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
				int tryOrderNum = 0;
				int.TryParse( param.sSearch, out tryOrderNum );

				collateralList = collateralList
					.Where( cOrder => ( cOrder.OrderID == tryOrderNum || tryOrderNum == 0 ) && 
						(cOrder.RequestingPartyName.Contains(param.sSearch) || 
						 cOrder.ShippingPartyName.Contains(param.sSearch)));
			}
			if( param.hideFulfilled )
			{
				collateralList = collateralList.Where( c => c.Status == NewOrderInformation.SPartial || c.Status == NewOrderInformation.SPending );
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
						collateralList = collateralList.OrderBy( v => v.RequestingPartyName );
					}
					else
					{
						collateralList = collateralList.OrderByDescending( v => v.RequestingPartyName );
					}
					break;
				case "shippingparty":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						collateralList = collateralList.OrderBy( v => v.ShippingPartyName );
					}
					else
					{
						collateralList = collateralList.OrderByDescending( v => v.ShippingPartyName );
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
		
		public IEnumerable<CollateralOrderSummary> GetFullCollateralOrderList( CollateralOrderTableParams param, out int totalRecords, out int displayedRecords )
		{
			totalRecords = 0;
			displayedRecords = 0;

			var collateralList = database.CollateralOrders.AsQueryable();

			totalRecords = collateralList.Count();

			if( !string.IsNullOrEmpty( param.sSearch ) )
			{
				int tryOrderNum = 0;
				int.TryParse( param.sSearch, out tryOrderNum );

				collateralList = collateralList
					.Where( cOrder => ( cOrder.OrderID == tryOrderNum || tryOrderNum == 0 ) && 
						(cOrder.RequestingPartyName.Contains(param.sSearch) || 
						 cOrder.ShippingPartyName.Contains(param.sSearch)));
			}
			if( param.hideFulfilled )
			{
				collateralList = collateralList.Where( c => c.Status == NewOrderInformation.SPartial || c.Status == NewOrderInformation.SPending );
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
						collateralList = collateralList.OrderBy( v => v.RequestingPartyName );
					}
					else
					{
						collateralList = collateralList.OrderByDescending( v => v.RequestingPartyName );
					}
					break;
				case "shippingparty":
					if( param.sSortDir_0.ToLower() == "asc" )
					{
						collateralList = collateralList.OrderBy( v => v.ShippingPartyName );
					}
					else
					{
						collateralList = collateralList.OrderByDescending( v => v.ShippingPartyName );
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
				RequestingParty = c.RequestingPartyName,
				ShippingParty = c.ShippingPartyName,
				CanEdit = ( ( c.Status != NewOrderInformation.SFulfilled ) && ( c.Status != NewOrderInformation.SCanceled ) )
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
			orderInfo.RequestingPartyName = eOrder.RequestingPartyName;

			orderInfo.ShippingParty = eOrder.ShippingParty;
			switch( orderInfo.ShippingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					orderInfo.SPPaoliMemberID = eOrder.SPPaoliMemberID;
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					orderInfo.SPPaoliRepGroupID = eOrder.SPPaoliRepGroupID;
					orderInfo.SPPaoliRepGroupMemberID = eOrder.SPPaoliRepGroupMemberID;
					break;
				case NewOrderInformation.RPDealer:
					orderInfo.SPDealerID = eOrder.SPDealerID;
					orderInfo.SPDealerMemberID = eOrder.SPDealerMemberID;
					break;
				case NewOrderInformation.RPEndUser:
					orderInfo.SPEndUserFirstName = eOrder.SPEndUserFirstName;
					orderInfo.SPEndUserLastName = eOrder.SPEndUserLastName;
					orderInfo.SPEndUserPhoneNumber = eOrder.SPEndUserPhoneNumber;
					orderInfo.SPEndUserEMailAddress = eOrder.SPEndUserEMailAddress;
					break;
			}
			orderInfo.ShippingPartyName = eOrder.ShippingPartyName;

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
					// find and add/update the single item
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

			UpdateOrderStatus( eOrder );

			return database.SaveChanges() > 0;
		}

		public PendingOrderInformation GetPendingOrder( int id, bool showShippedDetails )
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
				PaoliRepGroup = dbOrder.PaoliRepGroupID.HasValue ? dbOrder.PaoliSalesRep.FullName : "None",
				PaoliRepGroupMember = dbOrder.PaoliRepGroupMemberID.HasValue ? dbOrder.PaoliSalesRepMember.FullName : "None",
				Dealer = dbOrder.DealerID.HasValue ? dbOrder.Dealer.FullName : "None",
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
				CreatedByCompany = dbOrder.CreatedByUser.Company.FullName,
				CreatedByEmailAddress = dbOrder.CreatedByUser.Email,
				CreatedByPhoneNumber = dbOrder.CreatedByUser.BusinessPhone,

				CanceledByUserName = dbOrder.CanceledByUserID.HasValue ? dbOrder.CanceledByUser.FullName : null,
				CanceledOnDateTime = dbOrder.CanceledOnDateTime
			};

			switch( dbOrder.RequestingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					if( dbOrder.PaoliMemberID.HasValue )
					{
						retOrder.RPUserName = dbOrder.PaoliMember.FullName;
						retOrder.RPCompany = dbOrder.PaoliMember.Company.FullName;
						retOrder.RPEmailAddress = dbOrder.PaoliMember.Email;
						retOrder.RPPhoneNumber = dbOrder.PaoliMember.BusinessPhone;
					}
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					if( dbOrder.PaoliRepGroupMemberID.HasValue )
					{
						retOrder.RPUserName = dbOrder.PaoliSalesRepMember.FullName;
						retOrder.RPCompany = dbOrder.PaoliSalesRepMember.Company.FullName;
						retOrder.RPEmailAddress = dbOrder.PaoliSalesRepMember.Email;
						retOrder.RPPhoneNumber = dbOrder.PaoliSalesRepMember.BusinessPhone;
					}
					else if( dbOrder.PaoliRepGroupID.HasValue )
					{
						retOrder.RPCompany = dbOrder.PaoliSalesRep.FullName;
						retOrder.RPEmailAddress = dbOrder.PaoliSalesRep.ContactEmail;
						retOrder.RPPhoneNumber = dbOrder.PaoliSalesRep.Phone;
					}
					break;
				case NewOrderInformation.RPDealer:
					if( dbOrder.DealerMemberID.HasValue )
					{
						retOrder.RPUserName = dbOrder.DealerMember.FullName;
						retOrder.RPCompany = dbOrder.DealerMember.Company.FullName;
						retOrder.RPEmailAddress = dbOrder.DealerMember.Email;
						retOrder.RPPhoneNumber = dbOrder.DealerMember.BusinessPhone;
					}
					else if( dbOrder.DealerID.HasValue )
					{
						retOrder.RPCompany = dbOrder.Dealer.FullName;
						retOrder.RPEmailAddress = dbOrder.Dealer.ContactEmail;
						retOrder.RPPhoneNumber = dbOrder.Dealer.Phone;
					}
					break;
				case NewOrderInformation.RPEndUser:
					retOrder.RPUserName = dbOrder.EndUserFirstName + " " + dbOrder.EndUserLastName;
					retOrder.RPEmailAddress = dbOrder.EndUserEMailAddress;
					retOrder.RPPhoneNumber = dbOrder.EndUserPhoneNumber;
					break;
			}

			switch( dbOrder.ShippingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					if( dbOrder.SPPaoliMemberID.HasValue )
					{
						retOrder.SPUserName = dbOrder.SPPaoliMember.FullName;
						retOrder.SPCompany = dbOrder.SPPaoliMember.Company.FullName;
						retOrder.SPEmailAddress = dbOrder.SPPaoliMember.Email;
						retOrder.SPPhoneNumber = dbOrder.SPPaoliMember.BusinessPhone;
					}
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					if( dbOrder.SPPaoliRepGroupMemberID.HasValue )
					{
						retOrder.SPUserName = dbOrder.SPPaoliSalesRepMember.FullName;
						retOrder.SPCompany = dbOrder.SPPaoliSalesRepMember.Company.FullName;
						retOrder.SPEmailAddress = dbOrder.SPPaoliSalesRepMember.Email;
						retOrder.SPPhoneNumber = dbOrder.SPPaoliSalesRepMember.BusinessPhone;
					}
					else if( dbOrder.SPPaoliRepGroupID.HasValue )
					{
						retOrder.SPCompany = dbOrder.SPPaoliSalesRep.FullName;
						retOrder.SPEmailAddress = dbOrder.SPPaoliSalesRep.ContactEmail;
						retOrder.SPPhoneNumber = dbOrder.SPPaoliSalesRep.Phone;
					}
					break;
				case NewOrderInformation.RPDealer:
					if( dbOrder.SPDealerMemberID.HasValue )
					{
						retOrder.SPUserName = dbOrder.SPDealerMember.FullName;
						retOrder.SPCompany = dbOrder.SPDealerMember.Company.FullName;
						retOrder.SPEmailAddress = dbOrder.SPDealerMember.Email;
						retOrder.SPPhoneNumber = dbOrder.SPDealerMember.BusinessPhone;
					}
					else if( dbOrder.SPDealerID.HasValue )
					{
						retOrder.SPCompany = dbOrder.SPDealer.FullName;
						retOrder.SPEmailAddress = dbOrder.SPDealer.ContactEmail;
						retOrder.SPPhoneNumber = dbOrder.SPDealer.Phone;
					}
					break;
				case NewOrderInformation.RPEndUser:
					retOrder.SPUserName = dbOrder.SPEndUserFirstName + " " + dbOrder.SPEndUserLastName;
					retOrder.SPEmailAddress = dbOrder.SPEndUserEMailAddress;
					retOrder.SPPhoneNumber = dbOrder.SPEndUserPhoneNumber;
					break;
			}

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

			if( !showShippedDetails )
			{
				retOrder.OrderDetails.Where( a => a.RemainingQuantity <= 0 ).ToList()
					.ForEach( a => retOrder.OrderDetails.Remove( a ) );
			}

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
					ShippingDate = shipment.ShipmentDate,
					ShippingTypeID = dbOrder.ShippingType
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
			dbShipment.ShipmentDate = DateTime.UtcNow;

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

			UpdateOrderStatus( dbOrder );

			return database.SaveChanges() > 0;
		}

		private void UpdateOrderStatus( CollateralOrder dbOrder )
		{
			bool bHasFulfilled = false, bHasUnfulfilled = false, bHasPartial = false;

			foreach( var detail in dbOrder.CollateralOrderDetails )
			{
				var shippedQuantity = detail.CollateralOrderShipmentDetails.Sum( d => d.Quantity );
				bHasFulfilled |= (detail.Quantity == shippedQuantity);
				bHasUnfulfilled |= (0 == shippedQuantity);
				bHasPartial |= ( (shippedQuantity > 0) && (detail.Quantity > shippedQuantity) );
			}

			dbOrder.Status = NewOrderInformation.SPartial;
			if( !bHasFulfilled && !bHasPartial )
			{
				dbOrder.Status = NewOrderInformation.SPending;
			}
			else if( !bHasUnfulfilled && !bHasPartial )
			{
				dbOrder.Status = NewOrderInformation.SFulfilled;
			}
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

			foreach( var dbDetail in dbOrder.CollateralOrderDetails )
			{
				// delete db item
				var eCollateral = database.CollateralItems.FirstOrDefault( c => c.CollateralID == dbDetail.CollateralID );
				eCollateral.Quantity += ( dbDetail.Quantity - dbDetail.CollateralOrderShipmentDetails.Sum( s => s.Quantity ) );
			}

			dbOrder.CanceledByUserID = PaoliWebUser.CurrentUser.UserId;
			dbOrder.CanceledOnDateTime = DateTime.UtcNow;

			return database.SaveChanges() > 0;			
		}

		public ShipmentInformation GetOrderShipment( int id )
		{
			var shipment = database.CollateralOrderShipments.FirstOrDefault( s => s.ShipmentID == id );
			if( shipment == null )
			{
				throw new Exception( "Unable to find Shipment information" );
			}

			var sInfo = new ShipmentInformation()
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
				ShippingDate = shipment.ShipmentDate,
				ShippingTypeID = shipment.CollateralOrder.ShippingType,
				OrderDate = shipment.CollateralOrder.OrderDate

			};

			switch( shipment.CollateralOrder.RequestingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					if( shipment.CollateralOrder.PaoliMemberID.HasValue )
					{
						sInfo.RPUserName = shipment.CollateralOrder.PaoliMember.FullName;
						sInfo.RPCompany = shipment.CollateralOrder.PaoliMember.Company.FullName;
						sInfo.RPEmailAddress = shipment.CollateralOrder.PaoliMember.Email;
						sInfo.RPPhoneNumber = shipment.CollateralOrder.PaoliMember.BusinessPhone;
					}
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					if( shipment.CollateralOrder.PaoliRepGroupMemberID.HasValue )
					{
						sInfo.RPUserName = shipment.CollateralOrder.PaoliSalesRepMember.FullName;
						sInfo.RPCompany = shipment.CollateralOrder.PaoliSalesRepMember.Company.FullName;
						sInfo.RPEmailAddress = shipment.CollateralOrder.PaoliSalesRepMember.Email;
						sInfo.RPPhoneNumber = shipment.CollateralOrder.PaoliSalesRepMember.BusinessPhone;
					}
					else if( shipment.CollateralOrder.PaoliRepGroupID.HasValue )
					{
						sInfo.RPCompany = shipment.CollateralOrder.PaoliSalesRep.FullName;
						sInfo.RPEmailAddress = shipment.CollateralOrder.PaoliSalesRep.ContactEmail;
						sInfo.RPPhoneNumber = shipment.CollateralOrder.PaoliSalesRep.Phone;
					}
					break;
				case NewOrderInformation.RPDealer:
					if( shipment.CollateralOrder.DealerMemberID.HasValue )
					{
						sInfo.RPUserName = shipment.CollateralOrder.DealerMember.FullName;
						sInfo.RPCompany = shipment.CollateralOrder.DealerMember.Company.FullName;
						sInfo.RPEmailAddress = shipment.CollateralOrder.DealerMember.Email;
						sInfo.RPPhoneNumber = shipment.CollateralOrder.DealerMember.BusinessPhone;
					}
					else if( shipment.CollateralOrder.DealerID.HasValue )
					{
						sInfo.RPCompany = shipment.CollateralOrder.Dealer.FullName;
						sInfo.RPEmailAddress = shipment.CollateralOrder.Dealer.ContactEmail;
						sInfo.RPPhoneNumber = shipment.CollateralOrder.Dealer.Phone;
					}
					break;
				case NewOrderInformation.RPEndUser:
					sInfo.RPUserName = shipment.CollateralOrder.EndUserFirstName + " " + shipment.CollateralOrder.EndUserLastName;
					sInfo.RPEmailAddress = shipment.CollateralOrder.EndUserEMailAddress;
					sInfo.RPPhoneNumber = shipment.CollateralOrder.EndUserPhoneNumber;
					break;
			}

			switch( shipment.CollateralOrder.ShippingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					if( shipment.CollateralOrder.SPPaoliMemberID.HasValue )
					{
						sInfo.SPUserName = shipment.CollateralOrder.SPPaoliMember.FullName;
						sInfo.SPCompany = shipment.CollateralOrder.SPPaoliMember.Company.FullName;
						sInfo.SPEmailAddress = shipment.CollateralOrder.SPPaoliMember.Email;
						sInfo.SPPhoneNumber = shipment.CollateralOrder.SPPaoliMember.BusinessPhone;
					}
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					if( shipment.CollateralOrder.SPPaoliRepGroupMemberID.HasValue )
					{
						sInfo.SPUserName = shipment.CollateralOrder.SPPaoliSalesRepMember.FullName;
						sInfo.SPCompany = shipment.CollateralOrder.SPPaoliSalesRepMember.Company.FullName;
						sInfo.SPEmailAddress = shipment.CollateralOrder.SPPaoliSalesRepMember.Email;
						sInfo.SPPhoneNumber = shipment.CollateralOrder.SPPaoliSalesRepMember.BusinessPhone;
					}
					else if( shipment.CollateralOrder.SPPaoliRepGroupID.HasValue )
					{
						sInfo.SPCompany = shipment.CollateralOrder.SPPaoliSalesRep.FullName;
						sInfo.SPEmailAddress = shipment.CollateralOrder.SPPaoliSalesRep.ContactEmail;
						sInfo.SPPhoneNumber = shipment.CollateralOrder.SPPaoliSalesRep.Phone;
					}
					break;
				case NewOrderInformation.RPDealer:
					if( shipment.CollateralOrder.SPDealerMemberID.HasValue )
					{
						sInfo.SPUserName = shipment.CollateralOrder.SPDealerMember.FullName;
						sInfo.SPCompany = shipment.CollateralOrder.SPDealerMember.Company.FullName;
						sInfo.SPEmailAddress = shipment.CollateralOrder.SPDealerMember.Email;
						sInfo.SPPhoneNumber = shipment.CollateralOrder.SPDealerMember.BusinessPhone;
					}
					else if( shipment.CollateralOrder.SPDealerID.HasValue )
					{
						sInfo.SPCompany = shipment.CollateralOrder.SPDealer.FullName;
						sInfo.SPEmailAddress = shipment.CollateralOrder.SPDealer.ContactEmail;
						sInfo.SPPhoneNumber = shipment.CollateralOrder.SPDealer.Phone;
					}
					break;
				case NewOrderInformation.RPEndUser:
					sInfo.SPUserName = shipment.CollateralOrder.SPEndUserFirstName + " " + shipment.CollateralOrder.SPEndUserLastName;
					sInfo.SPEmailAddress = shipment.CollateralOrder.SPEndUserEMailAddress;
					sInfo.SPPhoneNumber = shipment.CollateralOrder.SPEndUserPhoneNumber;
					break;
			}

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

			return sInfo;
		}

		#endregion
	}
}
