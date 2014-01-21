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
				Status = CollateralStatus.DisplayStrings[cItem.Status],
				Quantity = cItem.IsGroup ? cItem.CollateralGroupItems.Min( cgi => cgi.ChildCollateralItem.Quantity / cgi.Quantity ) : cItem.Quantity,
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
			newItem.Status = cInfo.Status;
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
				Status = cInfo.Status,
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
				Status = cInfo.Status,
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
			cItem.Status = cInfo.Status;
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
	}
}
