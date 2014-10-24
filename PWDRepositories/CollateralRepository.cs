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
using PDWInfrastructure.EmailSenders;
using System.Data.Entity;
/*
 * Change Edit order to look like Ship Order - don't need to show all collateral items, just those in the order
 * Can lock down objects in bundle after creation if needed
 * Do we need CollateralOrderDetails.GroupNKID ??
 */

namespace PWDRepositories
{
	public class CollateralRepository : BaseRepository
	{
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
				IsGroup = cItem.IsGroup,
				IsActive = cItem.IsActive
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
			if( !param.showInactive )
			{
				collateralList = collateralList.Where( c => c.IsActive );
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

		public List<string> GetOrderDetailNameList()
		{
			return database.CollateralOrderDetails
				.Select( c => c.GroupNKID.HasValue ? ("Bundle - " + c.GroupName) : (c.CollateralTypeName + " - " + c.ItemName) )
				.Distinct()
				.OrderBy( s => s )
				.ToList();
		}

		public IEnumerable<CollateralOrderExport> GetExportList()
		{
			return database.CollateralOrders
				.OrderBy( r => r.OrderID )
				.ToList()
				.Select( r => ToCollateralOrderExport( r ) );
		}

		private CollateralOrderExport ToCollateralOrderExport( CollateralOrder dbOrder )
		{
			var retOrder = new CollateralOrderExport()
			{
				OrderID = dbOrder.OrderID,
				RequestingParty = NewOrderInformation.RequestingParties[dbOrder.RequestingParty],
				EndUserFirstName = dbOrder.EndUserFirstName,
				EndUserLastName = dbOrder.EndUserLastName,
				EndUserPhoneNumber = dbOrder.EndUserPhoneNumber,
				EndUserEMailAddress = dbOrder.EndUserEMailAddress,

				ShippingType = NewOrderInformation.ShippingTypes[dbOrder.ShippingType],
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

				CanceledByUserName = dbOrder.CanceledByUserID.HasValue ? dbOrder.CanceledByUser.FullName : null,
				CanceledOnDateTime = dbOrder.CanceledOnDateTime,
			};

			var OrderDetails = dbOrder.CollateralOrderDetails
				.Where( c => !c.GroupNKID.HasValue )
				.Select( cod => new NewOrderInformation.OrderDetail()
					{
						CollateralID = cod.CollateralNKID,
						CollateralTypeID = cod.CollateralTypeNKID,
						Name = cod.CollateralTypeName + " - " + cod.ItemName,
						Quantity = cod.Quantity,
						ShippedQuantity = cod.CollateralOrderShipmentDetails.Sum( c => c.Quantity )
					} )
				.ToList();
			OrderDetails.AddRange( dbOrder.CollateralOrderDetails
				.Where( c => c.GroupNKID.HasValue )
				.GroupBy( c => c.GroupNKID )
				.Select( cod => new NewOrderInformation.OrderDetail()
				{
					CollateralID = cod.FirstOrDefault().GroupNKID.Value,
					Name = "Bundle - " + cod.FirstOrDefault().GroupName,
					Quantity = cod.Select( d => d.Quantity / d.GroupQuantity.Value ).FirstOrDefault(),
					ShippedQuantity = cod.Select( d => d.CollateralOrderShipmentDetails.Sum( s => s.Quantity ) / d.GroupQuantity.Value ).DefaultIfEmpty().Max()
				} ) );

			retOrder.ItemNames = OrderDetails
				.ToDictionary( c => c.Name, c => c.Quantity );

			switch( dbOrder.RequestingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					if( dbOrder.PaoliMemberID.HasValue )
					{
						retOrder.RPUserName = dbOrder.PaoliMember.FullName;
						retOrder.RPCompany = dbOrder.PaoliMember.Company.FullName;
					}
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					if( dbOrder.PaoliRepGroupMemberID.HasValue )
					{
						retOrder.RPUserName = dbOrder.PaoliSalesRepMember.FullName;
						retOrder.RPCompany = dbOrder.PaoliSalesRepMember.Company.FullName;
					}
					else if( dbOrder.PaoliRepGroupID.HasValue )
					{
						retOrder.RPCompany = dbOrder.PaoliSalesRep.FullName;
					}
					break;
				case NewOrderInformation.RPDealer:
					if( dbOrder.DealerMemberID.HasValue )
					{
						retOrder.RPUserName = dbOrder.DealerMember.FullName;
						retOrder.RPCompany = dbOrder.DealerMember.Company.FullName;
					}
					else if( dbOrder.DealerID.HasValue )
					{
						retOrder.RPCompany = dbOrder.Dealer.FullName;
					}
					break;
				case NewOrderInformation.RPEndUser:
					retOrder.RPUserName = dbOrder.EndUserFirstName + " " + dbOrder.EndUserLastName;
					break;
			}

			switch( dbOrder.ShippingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					if( dbOrder.SPPaoliMemberID.HasValue )
					{
						retOrder.SPUserName = dbOrder.SPPaoliMember.FullName;
						retOrder.SPCompany = dbOrder.SPPaoliMember.Company.FullName;
					}
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					if( dbOrder.SPPaoliRepGroupMemberID.HasValue )
					{
						retOrder.SPUserName = dbOrder.SPPaoliSalesRepMember.FullName;
						retOrder.SPCompany = dbOrder.SPPaoliSalesRepMember.Company.FullName;
					}
					else if( dbOrder.SPPaoliRepGroupID.HasValue )
					{
						retOrder.SPCompany = dbOrder.SPPaoliSalesRep.FullName;
					}
					break;
				case NewOrderInformation.RPDealer:
					if( dbOrder.SPDealerMemberID.HasValue )
					{
						retOrder.SPUserName = dbOrder.SPDealerMember.FullName;
						retOrder.SPCompany = dbOrder.SPDealerMember.Company.FullName;
					}
					else if( dbOrder.SPDealerID.HasValue )
					{
						retOrder.SPCompany = dbOrder.SPDealer.FullName;
					}
					break;
				case NewOrderInformation.RPEndUser:
					retOrder.SPUserName = dbOrder.SPEndUserFirstName + " " + dbOrder.SPEndUserLastName;
					break;
			}

			return retOrder;
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
			newItem.IsActive = cInfo.IsActive;

			if( collStream != null )
			{
				newItem.ImageFileName = Guid.NewGuid().ToString() + Path.GetExtension( fileName );

				Image fullSizeImg = Image.FromStream( collStream );
				fullSizeImg.Save( Path.Combine( ConfigurationManager.AppSettings["ImageFileLocation"],
					newItem.ImageFileName ) );
			}

			database.CollateralItems.Add( newItem );

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
					database.CollateralGroupItems.Add( newcgi );
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
				Shipping = cInfo.Shipping,
				IsActive = cInfo.IsActive
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
			var retData = new CollateralGroupInformation()
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
				IsActive = cInfo.IsActive,
				GroupItems = cInfo.CollateralGroupItems
					.Where( cgi => cgi.ChildCollateralItem.IsActive )
					.Select( cgi => new CollateralGroupInformation.GroupInfoDetail()
					{
						ItemID = cgi.ChildCollateralItem.CollateralID,
						Quantity = cgi.Quantity
					} ).ToList()
			};

			if( !retData.GroupItems.Any() )
			{
				retData.GroupItems.Add( new CollateralGroupInformation.GroupInfoDetail() );
			}

			return retData;
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
			cItem.IsActive = cInfo.IsActive;

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

				cItem.CollateralGroupItems.ToList().ForEach( cgi => database.CollateralGroupItems.Remove( cgi ) );

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
					database.CollateralGroupItems.Add( newcgi );
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

			database.CollateralItems.Remove( cItem );

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
				.Where( c => c.IsActive )
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
					.Where( c => c.IsActive )
					.OrderBy( c => c.Name )
					.ToList()
					.Select( ci => new NewOrderInformation.OrderDetail()
					{
						CollateralID = ci.CollateralID,
						CollateralTypeID = ci.CollateralTypeID.HasValue ? ci.CollateralTypeID.Value : -1,
						Description = ci.Description,
						Name = ci.Name,
						Quantity = 0,
						Status = ci.StatusValue,
						ImageName = ci.ImageFileName
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
						newCollateral.CollateralGroupItems.Where( cgi => cgi.ChildCollateralItem.IsActive ).ToList().ForEach( c => c.ChildCollateralItem.Quantity -= (c.Quantity * detail.Quantity) );
						newCollateral.CollateralGroupItems.Where( cgi => cgi.ChildCollateralItem.IsActive ).ToList().ForEach( cgi => newOrder.CollateralOrderDetails.Add( new CollateralOrderDetail()
						{
							CollateralNKID = cgi.CollateralID,
							ItemName = cgi.ChildCollateralItem.Name,
							Quantity = cgi.Quantity * detail.Quantity,
							CollateralTypeNKID = cgi.ChildCollateralItem.CollateralTypeID.Value,
							CollateralTypeName = cgi.ChildCollateralItem.CollateralType.Name,
							GroupNKID = detail.CollateralID,
							GroupName = newCollateral.Name,
							GroupQuantity = cgi.Quantity,
						} ) );
					}
					else
					{
						newCollateral.Quantity -= detail.Quantity;
						newOrder.CollateralOrderDetails.Add( new CollateralOrderDetail()
						{
							CollateralNKID = newCollateral.CollateralID,
							ItemName = newCollateral.Name,
							Quantity = detail.Quantity,
							CollateralTypeNKID = newCollateral.CollateralTypeID.Value,
							CollateralTypeName = newCollateral.CollateralType.Name,
						} );
					}
				}
			}

			database.CollateralOrders.Add( newOrder );

			if( database.SaveChanges() > 0 )
			{
				var reloadedOrder = database.CollateralOrders
					.Include( c => c.CreatedByUser )
					.Include( c => c.CreatedByUser.Company )
					.Include( c => c.PaoliMember )
					.Include( c => c.PaoliMember.Company )
					.Include( c => c.PaoliSalesRepMember )
					.Include( c => c.PaoliSalesRepMember.Company )
					.Include( c => c.DealerMember )
					.Include( c => c.DealerMember.Company )
					.Include( c => c.Dealer )
					.Include( c => c.SPPaoliMember )
					.Include( c => c.SPPaoliMember.Company )
					.Include( c => c.SPPaoliSalesRepMember )
					.Include( c => c.SPPaoliSalesRepMember.Company )
					.Include( c => c.SPDealerMember )
					.Include( c => c.SPDealerMember.Company )
					.Include( c => c.SPDealer )
					.FirstOrDefault( c => c.OrderID == newOrder.OrderID );

				EmailSender.EmailTarget createdByEmail = null;
				List<EmailSender.EmailTarget> salesRepEmails = new List<EmailSender.EmailTarget>(), requestedForEmails = new List<EmailSender.EmailTarget>();

				GetEmailList( reloadedOrder, out createdByEmail, salesRepEmails, requestedForEmails );

				( new NewCollateralOrderEmailSender( "NewCollateralOrderCreatedBy" ) ).SubmitNewOrderEmail( createdByEmail.EmailAddress,
					ToEmailOrderSummary( createdByEmail, reloadedOrder ), createdByEmail.FromDetails );

				foreach( var salesRep in salesRepEmails )
				{
					( new NewCollateralOrderEmailSender( "NewCollateralOrderSalesRep" ) ).SubmitNewOrderEmail( salesRep.EmailAddress,
						ToEmailOrderSummary( salesRep, reloadedOrder ), salesRep.FromDetails );
				}

				foreach( var reqParty in requestedForEmails )
				{
					( new NewCollateralOrderEmailSender( "NewCollateralOrderRequestedBy" ) ).SubmitNewOrderEmail( reqParty.EmailAddress,
						ToEmailOrderSummary( reqParty, reloadedOrder ), reqParty.FromDetails );
				}

				return true;
			}

			return false;
		}

		private NewCollateralOrderEmailSender.EmailOrderSummary ToEmailOrderSummary( EmailSender.EmailTarget emailInfo, CollateralOrder orderInfo )
		{
			var summary = new NewCollateralOrderEmailSender.EmailOrderSummary()
					{
						orderID = orderInfo.OrderID,
						firstName = emailInfo.FirstName,
						shippingAddress1 = orderInfo.ShippingAddress1,
						shippingAddress2 = orderInfo.ShippingAddress2,
						shippingCity = orderInfo.ShippingCity,
						shippingCompany = orderInfo.ShippingCompanyName,
						shippingName = orderInfo.ShippingAttn,
						shippingState = orderInfo.ShippingState,
						shippingSpeed = NewOrderInformation.ShippingTypes[orderInfo.ShippingType],
						shippingZip = orderInfo.ShippingZip
					};

			summary.orderDetailList = orderInfo.CollateralOrderDetails
				.Where( cod => !cod.GroupNKID.HasValue )
				.Select( cod => string.Format( "{0}, Qty. {1}", cod.ItemName, cod.Quantity ) ).ToList();
			summary.orderDetailList.AddRange( orderInfo.CollateralOrderDetails
				.Where( cod => cod.GroupNKID.HasValue )
				.GroupBy( cod => cod.GroupNKID )
				.Select( codList => string.Format( "{0}, Qty. {1}", codList.First().GroupName, codList.Max( cod => cod.Quantity / cod.GroupQuantity ) ) ) );
			summary.orderDetailList.Sort();

			switch( orderInfo.RequestingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					if( orderInfo.PaoliMember != null )
					{
						summary.placingNameAndCompany = string.Format( "{0} at {1}", orderInfo.PaoliMember.FullName, orderInfo.PaoliMember.Company.Name );
					}
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					if( orderInfo.PaoliSalesRepMember != null )
					{
						summary.placingNameAndCompany = string.Format( "{0} at {1}", orderInfo.PaoliSalesRepMember.FullName, orderInfo.PaoliSalesRepMember.Company.Name );
					}
					else if( orderInfo.PaoliSalesRep != null )
					{
						summary.placingNameAndCompany = string.Format( "{0}", orderInfo.PaoliSalesRep.Name );
					}
					break;
				case NewOrderInformation.RPDealer:
					if( orderInfo.DealerMember != null )
					{
						summary.placingNameAndCompany = string.Format( "{0} at {1}", orderInfo.DealerMember.FullName, orderInfo.DealerMember.Company.Name );
					}
					else if( orderInfo.Dealer != null )
					{
						summary.placingNameAndCompany = string.Format( "{0}", orderInfo.Dealer.Name );
					}
					break;
				case NewOrderInformation.RPEndUser:
					summary.placingNameAndCompany = string.Format( "{0} {1}", orderInfo.EndUserFirstName, orderInfo.EndUserLastName );
					break;
			}

			switch( orderInfo.ShippingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					if( orderInfo.SPPaoliMember != null )
					{
						summary.receivingNameAndCompany = string.Format( "{0} at {1}", orderInfo.SPPaoliMember.FullName, orderInfo.SPPaoliMember.Company.Name );
					}
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					if( orderInfo.SPPaoliSalesRepMember != null )
					{
						summary.receivingNameAndCompany = string.Format( "{0} at {1}", orderInfo.SPPaoliSalesRepMember.FullName, orderInfo.SPPaoliSalesRepMember.Company.Name );
					}
					else if( orderInfo.SPPaoliSalesRep != null )
					{
						summary.receivingNameAndCompany = string.Format( "{0}", orderInfo.SPPaoliSalesRep.Name );
					}
					break;
				case NewOrderInformation.RPDealer:
					if( orderInfo.SPDealerMember != null )
					{
						summary.receivingNameAndCompany = string.Format( "{0} at {1}", orderInfo.SPDealerMember.FullName, orderInfo.SPDealerMember.Company.Name );
					}
					else if( orderInfo.SPDealer != null )
					{
						summary.receivingNameAndCompany = string.Format( "{0}", orderInfo.SPDealer.Name );
					}
					break;
				case NewOrderInformation.RPEndUser:
					summary.receivingNameAndCompany = string.Format( "{0} {1}", orderInfo.SPEndUserFirstName, orderInfo.SPEndUserLastName );
					break;
			}

			return summary;
		}

		private List<EmailSender.EmailTarget> GetEmailList( CollateralOrder orderInfo )
		{
			EmailSender.EmailTarget createdByEmail = null;
			List<EmailSender.EmailTarget> salesRepEmails = new List<EmailSender.EmailTarget>(), requestedForEmails = new List<EmailSender.EmailTarget>();

			GetEmailList( orderInfo, out createdByEmail, salesRepEmails, requestedForEmails );

			return salesRepEmails.Union( requestedForEmails )
				.Union( new List<EmailSender.EmailTarget>() { createdByEmail } )
				.Distinct()
				.ToList();
		}

		private void GetEmailList( CollateralOrder orderInfo, out EmailSender.EmailTarget createdByEmail, 
			List<EmailSender.EmailTarget> extraSalesRepEmails, List<EmailSender.EmailTarget> requestedForEmails )
		{
			createdByEmail = new EmailSender.EmailTarget()
			{
				EmailAddress = orderInfo.CreatedByUser.Email,
				FirstName = orderInfo.CreatedByUser.FirstName,
				FromDetails = GetPaoliMemberFromDetails( orderInfo.CreatedByUser.Company )
			};

			switch( orderInfo.RequestingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					if( orderInfo.PaoliMember != null )
					{
						if( orderInfo.PaoliMember.Enabled || EmailSender.EmailDisabledUsers )
						{
							requestedForEmails.Add( new EmailSender.EmailTarget()
							{
								EmailAddress = orderInfo.PaoliMember.Email,
								FirstName = orderInfo.PaoliMember.FirstName,
								FromDetails = GetPaoliMemberFromDetails( orderInfo.PaoliMember.Company )
							} );
						}
					}
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					if( orderInfo.PaoliSalesRepMember != null )
					{
						if( orderInfo.PaoliSalesRepMember.Enabled || EmailSender.EmailDisabledUsers )
						{
							requestedForEmails.Add( new EmailSender.EmailTarget()
							{
								EmailAddress = orderInfo.PaoliSalesRepMember.Email,
								FirstName = orderInfo.PaoliSalesRepMember.FirstName,
								FromDetails = GetPaoliMemberFromDetails( orderInfo.PaoliSalesRepMember.Company )
							} );
						}
					}
					break;
				case NewOrderInformation.RPDealer:
					if( orderInfo.DealerMember != null )
					{
						if( orderInfo.DealerMember.Enabled || EmailSender.EmailDisabledUsers )
						{
							requestedForEmails.Add( new EmailSender.EmailTarget()
							{
								EmailAddress = orderInfo.DealerMember.Email,
								FirstName = orderInfo.DealerMember.FirstName,
								FromDetails = GetPaoliMemberFromDetails( orderInfo.DealerMember.Company )
							} );
						}
					}
					if( orderInfo.Dealer != null )
					{
						extraSalesRepEmails.AddRange( database.Companies.Where( c => c.TerritoryID == orderInfo.Dealer.TerritoryID && c.CompanyType == PaoliWebUser.PaoliCompanyType.PaoliRepGroup )
							.SelectMany( srg => srg.Users )
							.Where( u => u.Enabled || EmailSender.EmailDisabledUsers )
							.ToList()
							.Select( srgm => new EmailSender.EmailTarget()
							{
								EmailAddress = srgm.Email,
								FirstName = srgm.FirstName,
								FromDetails = GetPaoliMemberFromDetails( srgm.Company )
							} ) );
					}
					break;
				case NewOrderInformation.RPEndUser:
//					requestedForEmails.Add( new EmailSender.EmailTarget() { EmailAddress = orderInfo.EndUserEMailAddress, FirstName = orderInfo.EndUserFirstName } );
					break;
			}

			switch( orderInfo.ShippingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					if( orderInfo.SPPaoliMember != null )
					{
						if( orderInfo.SPPaoliMember.Enabled || EmailSender.EmailDisabledUsers )
						{
							requestedForEmails.Add( new EmailSender.EmailTarget()
							{
								EmailAddress = orderInfo.SPPaoliMember.Email,
								FirstName = orderInfo.SPPaoliMember.FirstName,
								FromDetails = GetPaoliMemberFromDetails( orderInfo.SPPaoliMember.Company )
							} );
						}
					}
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					if( orderInfo.SPPaoliSalesRepMember != null )
					{
						if( orderInfo.SPPaoliSalesRepMember.Enabled || EmailSender.EmailDisabledUsers )
						{
							requestedForEmails.Add( new EmailSender.EmailTarget()
							{
								EmailAddress = orderInfo.SPPaoliSalesRepMember.Email,
								FirstName = orderInfo.SPPaoliSalesRepMember.FirstName,
								FromDetails = GetPaoliMemberFromDetails( orderInfo.SPPaoliSalesRepMember.Company )
							} );
						}
					}
					break;
				case NewOrderInformation.RPDealer:
					if( orderInfo.SPDealerMember != null )
					{
						if( orderInfo.SPDealerMember.Enabled || EmailSender.EmailDisabledUsers )
						{
							requestedForEmails.Add( new EmailSender.EmailTarget()
							{
								EmailAddress = orderInfo.SPDealerMember.Email,
								FirstName = orderInfo.SPDealerMember.FirstName,
								FromDetails = GetPaoliMemberFromDetails( orderInfo.SPDealerMember.Company )
							} );
						}
					}
					if( orderInfo.SPDealer != null )
					{
						extraSalesRepEmails.AddRange( database.Companies.Where( c => c.TerritoryID == orderInfo.SPDealer.TerritoryID && c.CompanyType == PaoliWebUser.PaoliCompanyType.PaoliRepGroup )
							.SelectMany( srg => srg.Users )
							.Where( u => u.Enabled || EmailSender.EmailDisabledUsers )
							.ToList()
							.Select( srgm => new EmailSender.EmailTarget()
							{
								EmailAddress = srgm.Email,
								FirstName = srgm.FirstName,
								FromDetails = GetPaoliMemberFromDetails( srgm.Company )
							} ) );
					}
					break;
				case NewOrderInformation.RPEndUser:
//					requestedForEmails.Add( new EmailSender.EmailTarget() { EmailAddress = orderInfo.SPEndUserEMailAddress, FirstName = orderInfo.SPEndUserFirstName } );
					break;
			}

			extraSalesRepEmails.RemoveAll( e => requestedForEmails.Any( d => d.EmailAddress == e.EmailAddress ) || ( e.EmailAddress == orderInfo.CreatedByUser.Email ) );
			requestedForEmails.RemoveAll( e => e.EmailAddress == orderInfo.CreatedByUser.Email );
		}

		public IEnumerable<CollateralOrderHPSummary> GetHomePageOrderList( int itemCount )
		{
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

			collateralList = collateralList
				.OrderByDescending( c => c.OrderID )
				.Take( itemCount );

			return collateralList.ToList().Select( c => ToCollateralOrderHPSummary( c ) );
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
					.Where( cOrder => (cOrder.OrderID == tryOrderNum) ||
						cOrder.RequestingPartyName.Contains( param.sSearch ) ||
						cOrder.ShippingPartyName.Contains( param.sSearch ) );
			}
			if( param.hideFulfilled )
			{
				collateralList = collateralList.Where( c => c.Status == NewOrderInformation.SPartial || c.Status == NewOrderInformation.SPending );
			}
			if( param.companyId.HasValue )
			{
				collateralList = collateralList.Where( c =>
					( ( c.RequestingParty == NewOrderInformation.RPPaoliRepresentative ) && ( c.PaoliRepGroupID == param.companyId.Value ) ) ||
					( ( c.ShippingParty == NewOrderInformation.RPPaoliRepresentative ) && ( c.SPPaoliRepGroupID == param.companyId.Value ) ) ||
					( ( c.RequestingParty == NewOrderInformation.RPDealer ) && ( c.Dealer.CompanyID == param.companyId.Value ) ) ||
					( ( c.ShippingParty == NewOrderInformation.RPDealer ) && ( c.SPDealer.CompanyID == param.companyId.Value ) ) );

				totalRecords = collateralList.Count();
			}
			if( param.userId.HasValue )
			{
				collateralList = collateralList.Where( c =>
					( ( c.RequestingParty == NewOrderInformation.RPPaoliMember ) && ( c.PaoliMemberID == param.userId.Value ) ) ||
					( ( c.ShippingParty == NewOrderInformation.RPPaoliMember ) && ( c.SPPaoliMemberID == param.userId.Value ) ) ||
					( ( c.RequestingParty == NewOrderInformation.RPPaoliRepresentative ) && ( c.PaoliRepGroupMemberID == param.userId.Value ) ) ||
					( ( c.ShippingParty == NewOrderInformation.RPPaoliRepresentative ) && ( c.SPPaoliRepGroupMemberID == param.userId.Value ) ) ||
					( ( c.RequestingParty == NewOrderInformation.RPDealer ) && ( c.DealerMemberID == param.userId.Value ) ) ||
					( ( c.ShippingParty == NewOrderInformation.RPDealer ) && ( c.SPDealerMemberID == param.userId.Value ) ) );

				totalRecords = collateralList.Count();
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

		private CollateralOrderHPSummary ToCollateralOrderHPSummary( CollateralOrder dbOrder )
		{
			var summary = new CollateralOrderHPSummary()
			{
				OrderID = dbOrder.OrderID,
				OrderDate = dbOrder.OrderDate,
				Status = NewOrderInformation.StatusValues[dbOrder.Status],
				RequestingParty = dbOrder.RequestingPartyName,
				ShippingParty = dbOrder.ShippingPartyName,
				CanEdit = ( ( dbOrder.Status != NewOrderInformation.SFulfilled ) && ( dbOrder.Status != NewOrderInformation.SCanceled ) ),
				IsOvernight = ( ( dbOrder.ShippingType == NewOrderInformation.STOvernightFedex ) || ( dbOrder.ShippingType == NewOrderInformation.STStdOvernightFedex ) ) &&
					( ( dbOrder.Status == NewOrderInformation.SPending ) || ( dbOrder.Status == NewOrderInformation.SPartial ) )
			};

			switch( dbOrder.RequestingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					if( dbOrder.PaoliMemberID.HasValue )
					{
						summary.RPName = dbOrder.PaoliMember.FullName;
						summary.RPCompany = dbOrder.PaoliMember.Company.FullName;
					}
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					if( dbOrder.PaoliRepGroupMemberID.HasValue )
					{
						summary.RPName = dbOrder.PaoliSalesRepMember.FullName;
						summary.RPCompany = dbOrder.PaoliSalesRepMember.Company.FullName;
					}
					else if( dbOrder.PaoliRepGroupID.HasValue )
					{
						summary.RPCompany = dbOrder.PaoliSalesRep.FullName;
					}
					break;
				case NewOrderInformation.RPDealer:
					if( dbOrder.DealerMemberID.HasValue )
					{
						summary.RPName = dbOrder.DealerMember.FullName;
						summary.RPCompany = dbOrder.DealerMember.Company.FullName;
					}
					else if( dbOrder.DealerID.HasValue )
					{
						summary.RPCompany = dbOrder.Dealer.FullName;
					}
					break;
				case NewOrderInformation.RPEndUser:
					summary.RPName = dbOrder.EndUserFirstName + " " + dbOrder.EndUserLastName;
					break;
			}

			switch( dbOrder.ShippingParty )
			{
				case NewOrderInformation.RPPaoliMember:
					if( dbOrder.SPPaoliMemberID.HasValue )
					{
						summary.SPName = dbOrder.SPPaoliMember.FullName;
						summary.SPCompany = dbOrder.SPPaoliMember.Company.FullName;
					}
					break;
				case NewOrderInformation.RPPaoliRepresentative:
					if( dbOrder.SPPaoliRepGroupMemberID.HasValue )
					{
						summary.SPName = dbOrder.SPPaoliSalesRepMember.FullName;
						summary.SPCompany = dbOrder.SPPaoliSalesRepMember.Company.FullName;
					}
					else if( dbOrder.SPPaoliRepGroupID.HasValue )
					{
						summary.SPCompany = dbOrder.SPPaoliSalesRep.FullName;
					}
					break;
				case NewOrderInformation.RPDealer:
					if( dbOrder.SPDealerMemberID.HasValue )
					{
						summary.SPName = dbOrder.SPDealerMember.FullName;
						summary.SPCompany = dbOrder.SPDealerMember.Company.FullName;
					}
					else if( dbOrder.SPDealerID.HasValue )
					{
						summary.SPCompany = dbOrder.SPDealer.FullName;
					}
					break;
				case NewOrderInformation.RPEndUser:
					summary.SPName = dbOrder.SPEndUserFirstName + " " + dbOrder.SPEndUserLastName;
					break;
			}

			return summary;
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
				CanEdit = ( ( c.Status != NewOrderInformation.SFulfilled ) && ( c.Status != NewOrderInformation.SCanceled ) ),
				IsOvernight = ((c.ShippingType == NewOrderInformation.STOvernightFedex) || (c.ShippingType == NewOrderInformation.STStdOvernightFedex)) &&
					((c.Status == NewOrderInformation.SPending) || (c.Status == NewOrderInformation.SPartial))
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

			orderInfo.OrderDetails = eOrder.CollateralOrderDetails
				.Where( c => !c.GroupNKID.HasValue )
				.Select( cod => new NewOrderInformation.OrderDetail()
					{
						CollateralID = cod.CollateralNKID,
						CollateralTypeID = cod.CollateralTypeNKID,
						Name = cod.ItemName,
						Quantity = cod.Quantity,
						ShippedQuantity = cod.CollateralOrderShipmentDetails.Sum( c => c.Quantity )
					} )
				.ToList();
			orderInfo.OrderDetails.AddRange( eOrder.CollateralOrderDetails
				.Where( c => c.GroupNKID.HasValue )
				.GroupBy( c => c.GroupNKID )
				.Select( cod => new NewOrderInformation.OrderDetail()
				{
					CollateralID = cod.FirstOrDefault().GroupNKID.Value,
					Name = cod.FirstOrDefault().GroupName,
					Quantity = cod.Select( d => d.Quantity / d.GroupQuantity.Value ).FirstOrDefault(),
					ShippedQuantity = cod.Select( d => d.CollateralOrderShipmentDetails.Sum( s => s.Quantity ) / d.GroupQuantity.Value ).DefaultIfEmpty().Max()
				} ) );

			orderInfo.OrderDetails.ForEach( cod =>
				cod.ImageName = database.CollateralItems
					.Where( ci => ci.CollateralID == cod.CollateralID )
					.Select( ci => ci.ImageFileName )
					.FirstOrDefault() );

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

			foreach( var uiDetail in orderInfo.OrderDetails )
			{
				// update the item(s)
				if( eOrder.CollateralOrderDetails.Any( d => d.GroupNKID == uiDetail.CollateralID ) )
				{
					// is group; update all items based on new quantity
					foreach( var bundlePart in eOrder.CollateralOrderDetails.Where( d => d.GroupNKID == uiDetail.CollateralID ).ToList() )
					{
						var dbItem = database.CollateralItems.FirstOrDefault( c => c.CollateralID == bundlePart.CollateralNKID );
						if( dbItem != null )
						{
							dbItem.Quantity -= ( ( bundlePart.GroupQuantity.Value * uiDetail.Quantity ) - bundlePart.Quantity );
						}

						if( uiDetail.Quantity > 0 )
						{
							bundlePart.Quantity = bundlePart.GroupQuantity.Value * uiDetail.Quantity;
						}
						else
						{
							database.CollateralOrderDetails.Remove( bundlePart );
						}
					}
				}
				else
				{
					// is single item; update item based on new quantity
					var dbDetail = eOrder.CollateralOrderDetails.FirstOrDefault( d => d.CollateralNKID == uiDetail.CollateralID && !d.GroupNKID.HasValue );
					if( dbDetail != null )
					{
						var dbItem = database.CollateralItems.FirstOrDefault( c => c.CollateralID == uiDetail.CollateralID );
						if( dbItem != null )
						{
							dbItem.Quantity -= ( uiDetail.Quantity - dbDetail.Quantity );
						}
						if( uiDetail.Quantity > 0 )
						{
							dbDetail.Quantity = uiDetail.Quantity;
						}
						else
						{
							database.CollateralOrderDetails.Remove( dbDetail );
						}
					}
				}
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
					CollateralID = oDetail.CollateralNKID,
					Name = oDetail.ItemName,
					Quantity = oDetail.Quantity,
					RemainingQuantity = oDetail.Quantity - oDetail.CollateralOrderShipmentDetails.Sum( c => c.Quantity ),
					GroupID = oDetail.GroupNKID,
					GroupName = oDetail.GroupNKID.HasValue ? oDetail.GroupName : null,
					CollateralTypeID = oDetail.GroupNKID.HasValue ? 0 : oDetail.CollateralTypeNKID,
					CollateralType = oDetail.GroupNKID.HasValue ? null : oDetail.CollateralTypeName
				} );
			}

			retOrder.OrderDetails.ForEach( cod =>
				cod.ImageName = database.CollateralItems
					.Where( ci => ci.CollateralID == cod.CollateralID )
					.Select( ci => ci.ImageFileName )
					.FirstOrDefault() );

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
					TrackingNumber1 = shipment.TrackingNumber1,
					TrackingNumber2 = shipment.TrackingNumber2,
					TrackingNumber3 = shipment.TrackingNumber3,
					TrackingNumber4 = shipment.TrackingNumber4,
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
							Name = sDetail.CollateralOrderDetail.ItemName,
							Quantity = sDetail.Quantity
						} );
				}

				retOrder.Shipments.Add( sInfo );
			}

			return retOrder;
		}

		public bool AddOrderShipment( int orderID, PendingOrderInformation.ShipmentSummary shipmentInfo )
		{
			var dbOrder = database.CollateralOrders
				.Include( c => c.CreatedByUser )
				.Include( c => c.CreatedByUser.Company )
				.Include( c => c.PaoliMember )
				.Include( c => c.PaoliMember.Company )
				.Include( c => c.PaoliSalesRepMember )
				.Include( c => c.PaoliSalesRepMember.Company )
				.Include( c => c.DealerMember )
				.Include( c => c.DealerMember.Company )
				.Include( c => c.Dealer )
				.Include( c => c.SPPaoliMember )
				.Include( c => c.SPPaoliMember.Company )
				.Include( c => c.SPPaoliSalesRepMember )
				.Include( c => c.SPPaoliSalesRepMember.Company )
				.Include( c => c.SPDealerMember )
				.Include( c => c.SPDealerMember.Company )
				.Include( c => c.SPDealer )
				.Include( c => c.CollateralOrderDetails )
				.FirstOrDefault( c => c.OrderID == orderID );
			if( dbOrder == null )
			{
				throw new Exception( "Unable to find order for shipment." );
			}

			var dbShipment = new CollateralOrderShipment();
			dbShipment.Vendor = shipmentInfo.Vendor;
			dbShipment.TrackingNumber1 = shipmentInfo.TrackingNumber1;
			dbShipment.TrackingNumber2 = shipmentInfo.TrackingNumber2;
			dbShipment.TrackingNumber3 = shipmentInfo.TrackingNumber3;
			dbShipment.TrackingNumber4 = shipmentInfo.TrackingNumber4;
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

			if( database.SaveChanges() > 0 )
			{
				foreach( var emailTarget in GetEmailList( dbOrder ) )
				{
					var summary = ToEmailShipmentSummary( emailTarget, dbShipment );
					if( dbOrder.Status == NewOrderInformation.SPartial )
					{
						( new NewCollateralOrderShipmentEmailSender( "NewCollateralOrderPartialShipment" ) ).SubmitNewShipmentEmail( emailTarget.EmailAddress, summary, emailTarget.FromDetails );
					}
					else if( dbOrder.Status == NewOrderInformation.SFulfilled )
					{
						( new NewCollateralOrderShipmentEmailSender( "NewCollateralOrderShipment" ) ).SubmitNewShipmentEmail( emailTarget.EmailAddress, summary, emailTarget.FromDetails );
					}
				}
				return true;
			}

			return false;
		}

		private NewCollateralOrderShipmentEmailSender.EmailShipmentSummary ToEmailShipmentSummary( EmailSender.EmailTarget emailInfo, CollateralOrderShipment shipmentInfo )
		{
			var summary = new NewCollateralOrderShipmentEmailSender.EmailShipmentSummary()
					{
						orderID = shipmentInfo.OrderID,
						firstName = emailInfo.FirstName,
						shippingAddress1 = shipmentInfo.ShippingAddress1,
						shippingAddress2 = shipmentInfo.ShippingAddress2,
						shippingCity = shipmentInfo.ShippingCity,
						shippingCompany = shipmentInfo.ShippingCompanyName,
						shippingName = shipmentInfo.ShippingAttn,
						shippingState = shipmentInfo.ShippingState,
						shippingSpeed = shipmentInfo.ShippingType,
						shippingZip = shipmentInfo.ShippingZip,
						shippingCarrier = shipmentInfo.Vendor,
						trackingNumbers = new List<string>() { shipmentInfo.TrackingNumber1, shipmentInfo.TrackingNumber2, shipmentInfo.TrackingNumber3, shipmentInfo.TrackingNumber4 },
						shipmentDetailList = shipmentInfo.CollateralOrderShipmentDetails
							.Select( cosd => string.Format( "{0}, Qty. {1}", cosd.CollateralOrderDetail.ItemName, cosd.Quantity ) )
							.ToList()
					};

			return summary;
		}
		private void UpdateOrderStatus( CollateralOrder dbOrder )
		{
			bool bHasFulfilled = false, bHasUnfulfilled = false, bHasPartial = false;

			foreach( var detail in dbOrder.CollateralOrderDetails )
			{
				var shippedQuantity = dbOrder.CollateralOrderShipments
					.SelectMany( s => s.CollateralOrderShipmentDetails )
					.Where( d => d.CollateralOrderDetail.DetailID == detail.DetailID )
					.Sum( d => d.Quantity );
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
				var eCollateral = database.CollateralItems.FirstOrDefault( c => c.CollateralID == dbDetail.CollateralNKID );
				if( eCollateral != null )
				{
					eCollateral.Quantity += ( dbDetail.Quantity - dbDetail.CollateralOrderShipmentDetails.Sum( s => s.Quantity ) );
				}
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
				TrackingNumbers = string.Join( ", ", new List<string>() { shipment.TrackingNumber1,
					shipment.TrackingNumber2,
					shipment.TrackingNumber3,
					shipment.TrackingNumber4 }.Where( s => (s ?? "").Any() ) ),
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
					Name = sDetail.CollateralOrderDetail.ItemName,
					Quantity = sDetail.Quantity
				} );
			}

			return sInfo;
		}

		#endregion
	}
}
