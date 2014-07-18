﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using System.Text.RegularExpressions;
using PDWModels.eCollateral;
using PDWInfrastructure;

namespace PWDRepositories
{
	public class ECollateralRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		public class StatusTypes
		{
			public const int Incomplete = 1;
			public const int New = 2;
			public const int Unapproved = 3;
			public const int Approved = 4;

			public static Dictionary<int, string> StatusList = new Dictionary<int, string>()
			{
				{ Incomplete, "Incomplete" },
				{ New, "New" },
				{ Unapproved, "Unapproved" },
				{ Approved, "Approved" },
			};
		}

		public class LayoutTypes
		{
			public const int HTMLPage = 1;
			public const int PDFPage = 2;

			public static Dictionary<int, List<int>> LayoutList = new Dictionary<int, List<int>>()
			{
				{ HTMLPage, new List<int>() { Layouts.Layout12ColumnSidebar, Layouts.Layout13ColumnSidebar, Layouts.Layout12LeftDetailSidebar } },
				{ PDFPage, new List<int>() }
			};

			public static Dictionary<int, string> LayoutTypeList = new Dictionary<int, string>()
			{
				{ HTMLPage, "HTML Landing Page" },
				{ PDFPage, "Printable PDF" },
			};
		}

		public class Layouts
		{
			public const int Layout12ColumnSidebar = 1;
			public const int Layout13ColumnSidebar = 2;
			public const int Layout12LeftDetailSidebar = 3;

			public static Dictionary<int, string> LayoutTitles = new Dictionary<int, string>()
			{
				{ Layout12ColumnSidebar, "1:2 Column + Sidebar" },
				{ Layout13ColumnSidebar, "1:3 Column + Bottom Bar" },
				{ Layout12LeftDetailSidebar, "1:Left Detail + Sidebar" },
			};

			public static Dictionary<int, int> LayoutSections = new Dictionary<int, int>()
			{
				{ Layout12ColumnSidebar, 6 },
				{ Layout13ColumnSidebar, 7 },
				{ Layout12LeftDetailSidebar, 6 },
			};

			public static Dictionary<int, string> LayoutEditViews = new Dictionary<int, string>()
			{
				{ Layout12ColumnSidebar, "Layout12ColumnSidebar" },
				{ Layout13ColumnSidebar, "Layout13ColumnSidebar" },
				{ Layout12LeftDetailSidebar, "Layout12LeftDetailSidebar" },
			};

			public static Dictionary<int, string> LayoutDisplayViews = new Dictionary<int, string>()
			{
				{ Layout12ColumnSidebar, "Display12ColumnSidebar" },
				{ Layout13ColumnSidebar, "Display13ColumnSidebar" },
				{ Layout12LeftDetailSidebar, "Display12LeftDetailSidebar" },
			};

			public static Dictionary<int, string> LayoutImages = new Dictionary<int, string>()
			{
				{ Layout12ColumnSidebar, "layout1.png" },
				{ Layout13ColumnSidebar, "layout1.png" },
				{ Layout12LeftDetailSidebar, "layout1.png" },
			};
		}

		public ECollateralRepository()
		{
		}
		
		public bool ValidateURL( int itemId, string url )
		{
			if( !Regex.IsMatch( url, "^[A-Za-z0-9\\-_]+$" ) )
				return false;

			if( database.eCollateralItems.Any( e => e.ItemID != itemId && e.URLText == url ) )
				return false;

			return true;
		}

		public bool AddSettings( ECollateralSettings settings, int userId, out int itemId )
		{
			itemId = 0;

			if( !ValidateURL( 0, settings.CustomURL ) )
			{
				throw new Exception( "URL is invalid" );
			}

			var newItem = new eCollateralItem();
			newItem.CreatedByUserID = userId;
			newItem.CreatedByDateTime = DateTime.UtcNow;
			newItem.LastModifiedByUserID = userId;
			newItem.LastModifiedByDateTime = DateTime.UtcNow;

			newItem.IsTemplate = settings.IsTemplate;
			newItem.FileName = settings.FileName;
			newItem.URLText = settings.CustomURL;
			newItem.DealershipID = settings.DealershipID;
			if( settings.DealershipPOCID != 0 )
			{
				newItem.DealershipPOCID = settings.DealershipPOCID;
			}
			else
			{
				newItem.DealershipPOCName = settings.DealershipPOCName;
				newItem.DealershipPOCEmail = settings.DealershipPOCEmail;
				newItem.DealershipPOCPhone = settings.DealershipPOCPhone;
				newItem.DealershipPOCAcctType = settings.DealershipPOCAcctType;
			}
			newItem.CustomerName = settings.CustomerName;
			newItem.ProjectName = settings.ProjectName;

			newItem.Status = StatusTypes.Incomplete;

			database.eCollateralItems.AddObject( newItem );

			if( database.SaveChanges() > 0 )
			{
				database.Refresh( System.Data.Objects.RefreshMode.StoreWins, newItem );

				itemId = newItem.ItemID;

				return true;
			}

			throw new Exception( "Unable to save eCollateral Item" );
		}

		public bool EditSettings( ECollateralSettings settings, int userId )
		{
			if( !ValidateURL( 0, settings.CustomURL ) )
			{
				throw new Exception( "URL is invalid" );
			}

			var dbItem = database.eCollateralItems.FirstOrDefault( i => i.ItemID == settings.ItemID );
			if( dbItem != null )
			{
				dbItem.LastModifiedByUserID = userId;
				dbItem.LastModifiedByDateTime = DateTime.UtcNow;

				dbItem.IsTemplate = settings.IsTemplate;
				dbItem.FileName = settings.FileName;
				dbItem.URLText = settings.CustomURL;
				dbItem.DealershipID = settings.DealershipID;
				if( settings.DealershipPOCID != 0 )
				{
					dbItem.DealershipPOCID = settings.DealershipPOCID;

					dbItem.DealershipPOCName = null;
					dbItem.DealershipPOCEmail = null;
					dbItem.DealershipPOCPhone = null;
					dbItem.DealershipPOCAcctType = null;
				}
				else
				{
					dbItem.DealershipPOCName = settings.DealershipPOCName;
					dbItem.DealershipPOCEmail = settings.DealershipPOCEmail;
					dbItem.DealershipPOCPhone = settings.DealershipPOCPhone;
					dbItem.DealershipPOCAcctType = settings.DealershipPOCAcctType;

					dbItem.DealershipPOCID = null;
				}
				dbItem.CustomerName = settings.CustomerName;
				dbItem.ProjectName = settings.ProjectName;

				dbItem.Status = (dbItem.Status == StatusTypes.Incomplete) ? dbItem.Status : StatusTypes.New;

				return database.SaveChanges() > 0;
			}
			throw new Exception( "Unable to save eCollateral Item" );
		}

		public bool SetLayout( ECollateralLayout layoutInfo, int userId )
		{
			var dbItem = database.eCollateralItems.FirstOrDefault( i => i.ItemID == layoutInfo.ItemID );
			if( dbItem != null )
			{
				if( dbItem.LayoutID != layoutInfo.LayoutID )
				{
					dbItem.LayoutID = layoutInfo.LayoutID;
					foreach( var contentType in LayoutTypes.LayoutList.Keys )
					{
						if( LayoutTypes.LayoutList[contentType].Contains( layoutInfo.LayoutID ) )
						{
							dbItem.ContentType = contentType;
							break;
						}
					}
					dbItem.LastModifiedByUserID = userId;
					dbItem.LastModifiedByDateTime = DateTime.UtcNow;

					foreach( var dbSection in dbItem.eCollateralSections.ToList() )
					{
						database.DeleteObject( dbSection );
					}
				}

				return database.SaveChanges() > 0;
			}

			throw new Exception( "Unable to find eCollateral" );
		}

		private ECollateralSettings ToECollateralSettings( eCollateralItem dbItem )
		{
			var retItem = new ECollateralSettings()
			{
				ItemID = dbItem.ItemID,
				IsTemplate = dbItem.IsTemplate,
				FileName = dbItem.FileName,
				CustomURL = dbItem.URLText
			};

			if( !dbItem.IsTemplate )
			{
				retItem.PaoliSalesRepGroupID = dbItem.Company.Territory.SalesRepCompanyID;
				retItem.DealershipID = dbItem.DealershipID;
				retItem.DealershipPOCID = dbItem.DealershipPOCID;
				retItem.DealershipPOCName = dbItem.DealershipPOCName;
				retItem.DealershipPOCEmail = dbItem.DealershipPOCEmail;
				retItem.DealershipPOCPhone = dbItem.DealershipPOCPhone;
				retItem.DealershipPOCAcctType = dbItem.DealershipPOCAcctType;
				retItem.CustomerName = dbItem.CustomerName;
				retItem.ProjectName = dbItem.ProjectName;
				retItem.PaoliSalesRepGroupName = dbItem.Company.Territory.SalesRepCompanyName;
				retItem.DealershipName = dbItem.Company.Name;
				retItem.DealershipPOCMember = dbItem.DealershipPOCID.HasValue ? dbItem.DealershipPOC.FullName : null;
				retItem.DealershipPOCAcctTypeName = dbItem.DealershipPOCAcctType.HasValue ? PaoliWebUser.PaoliWebRole.RoleList[dbItem.DealershipPOCAcctType.Value] : null;
			}

			return retItem;
		}

		private ECollateralDetails ToECollateralDetails( eCollateralItem dbItem )
		{
			var dInfo = new ECollateralDetails();
			dInfo.ItemID = dbItem.ItemID;
			dInfo.LayoutName = Layouts.LayoutTitles[dbItem.LayoutID.Value];
			dInfo.EditViewName = Layouts.LayoutEditViews[dbItem.LayoutID.Value];
			dInfo.DisplayViewName = Layouts.LayoutDisplayViews[dbItem.LayoutID.Value];
			dInfo.Details = new List<ECollateralDetails.DetailItem>();

			for( int i = 0; i < Layouts.LayoutSections[dbItem.LayoutID.Value]; i++ )
			{
				dInfo.Details.Add( new ECollateralDetails.DetailItem() );
			}

			foreach( var dbDetail in dbItem.eCollateralSections )
			{
				dInfo.Details[dbDetail.Sequence] = new ECollateralDetails.DetailItem()
				{
					ImageID = dbDetail.ImageID,
					Content = System.Web.HttpUtility.HtmlDecode( dbDetail.Content ),
					ImageFileName = dbDetail.ImageID.HasValue ? dbDetail.ImageFile.ThumbnailImageData( "l16to9" ).FileName : null
				};
			}

			return dInfo;
		}

		public ECollateralDetails GetItemDetails( int id )
		{
			var dbItem = database.eCollateralItems.FirstOrDefault( i => i.ItemID == id );
			if( dbItem != null )
			{
				return ToECollateralDetails( dbItem );
			}

			throw new Exception( "Unable to find eCollateral" );
			
		}

		public ECollateralDetails GetItemDetails( string id )
		{
			var dbItem = database.eCollateralItems
				.FirstOrDefault( i => (i.URLText == id) && (new List<int>() { StatusTypes.New, StatusTypes.Approved }.Contains( i.Status )) );
			if( dbItem != null )
			{
				return ToECollateralDetails( dbItem );
			}

			throw new Exception( "Unable to find eCollateral" );

		}

		public ECollateralInformation GetItemInformation( int id )
		{
			var dbItem = database.eCollateralItems.FirstOrDefault( i => i.ItemID == id );
			if( dbItem != null )
			{
				return new ECollateralInformation()
				{
					Settings = ToECollateralSettings( dbItem ),
					DetailInfo = ToECollateralDetails( dbItem )
				};
			}

			throw new Exception( "Unable to find eCollateral" );
		}

		public ECollateralSettings GetItemSettings( int id )
		{
			var dbItem = database.eCollateralItems.FirstOrDefault( i => i.ItemID == id );
			if( dbItem != null )
			{
				return ToECollateralSettings( dbItem );
			}

			throw new Exception( "Unable to find eCollateral" );
		}

		public bool SetItemSections( ECollateralDetails sectionInfo, int userId )
		{
			var dbItem = database.eCollateralItems.FirstOrDefault( i => i.ItemID == sectionInfo.ItemID );
			if( dbItem != null )
			{
				dbItem.LastModifiedByDateTime = DateTime.UtcNow;
				dbItem.LastModifiedByUserID = userId;

				for( int i = 0; i < Layouts.LayoutSections[dbItem.LayoutID.Value]; i++ )
				{
					var uiSection = sectionInfo.Details[i];
					var dbSection = dbItem.eCollateralSections.FirstOrDefault( s => s.Sequence == i );
					if( dbSection == null )
					{
						if( uiSection.ImageID != null || ( uiSection.Content ?? "" ).Any() )
						{
							dbItem.eCollateralSections.Add( new eCollateralSection()
							{
								ImageID = uiSection.ImageID,
								Content = uiSection.Content,
								Sequence = i
							} );
						}
					}
					else if( uiSection.ImageID != null || ( uiSection.Content ?? "" ).Any() )
					{
						dbSection.ImageID = uiSection.ImageID;
						dbSection.Content = uiSection.Content;
					}
					else
					{
						database.DeleteObject( dbSection );
					}

				}

				return database.SaveChanges() > 0;
			}

			throw new Exception( "Unable to find eCollateral" );
		}

		public bool ConfirmLayout( int id, int userId )
		{
			var dbItem = database.eCollateralItems.FirstOrDefault( i => i.ItemID == id );
			if( dbItem != null )
			{
				dbItem.Status = dbItem.IsTemplate ? StatusTypes.Approved : StatusTypes.New;
				dbItem.LastModifiedByUserID = userId;
				dbItem.LastModifiedByDateTime = DateTime.UtcNow;

				return database.SaveChanges() > 0;
			}

			throw new Exception( "Unable to find eCollateral" );
		}

		public int CopyLayout( int id, int userId )
		{
			var dbItem = database.eCollateralItems.FirstOrDefault( i => i.ItemID == id );
			if( dbItem != null )
			{
				var newItem = new eCollateralItem();
				newItem.CreatedByUserID = userId;
				newItem.CreatedByDateTime = DateTime.UtcNow;
				newItem.LastModifiedByUserID = userId;
				newItem.LastModifiedByDateTime = DateTime.UtcNow;

				newItem.IsTemplate = dbItem.IsTemplate;
				newItem.FileName = dbItem.FileName;
				newItem.URLText = "";
				newItem.DealershipID = dbItem.DealershipID;
				if( dbItem.DealershipPOCID != 0 )
				{
					newItem.DealershipPOCID = dbItem.DealershipPOCID;
				}
				else
				{
					newItem.DealershipPOCName = dbItem.DealershipPOCName;
					newItem.DealershipPOCEmail = dbItem.DealershipPOCEmail;
					newItem.DealershipPOCPhone = dbItem.DealershipPOCPhone;
					newItem.DealershipPOCAcctType = dbItem.DealershipPOCAcctType;
				}
				newItem.CustomerName = dbItem.CustomerName;
				newItem.ProjectName = dbItem.ProjectName;
				newItem.LayoutID = dbItem.LayoutID;
				newItem.ContentType = dbItem.ContentType;

				newItem.Status = StatusTypes.Incomplete;

				foreach( var dbSection in dbItem.eCollateralSections )
				{
					newItem.eCollateralSections.Add( new eCollateralSection()
					{
						ImageID = dbSection.ImageID,
						Content = dbSection.Content,
						Sequence = dbSection.Sequence
					} );
				}

				database.eCollateralItems.AddObject( newItem );

				if( database.SaveChanges() > 0 )
				{
					database.Refresh( System.Data.Objects.RefreshMode.StoreWins, newItem );

					return newItem.ItemID;
				}
			}

			throw new Exception( "Unable to find eCollateral" );
		}

		public bool VerifyURLPermission( string url, PaoliWebUser user )
		{
			if( user.IsPaoliUser )
			{
				return true;
			}

			var dbItem = database.eCollateralItems
				.FirstOrDefault( i => ( i.URLText == url ) );
			if( dbItem != null )
			{
				if( dbItem.IsTemplate )
				{
					return true;
				}

				var dbUser = database.Users.FirstOrDefault( u => u.UserID == user.UserId );
				if( user.IsDealerUser )
				{
					// either created by, or given permission to
					if( dbItem.CreatedByUser.CompanyID == dbUser.CompanyID || 
						dbItem.DealershipID == dbUser.CompanyID )
					{
						return true;
					}
				}
				else if( user.IsInRole( PaoliWebUser.PaoliWebRole.PaoliSalesRep ) )
				{
					if( dbItem.CreatedByUser.CompanyID == dbUser.CompanyID ||
						dbItem.CreatedByUser.Company.TerritoryID == dbUser.Company.TerritoryID ||
						dbItem.Company.TerritoryID == dbUser.Company.TerritoryID )
					{
						return true;
					}
				}
			}

			return false;
		}

		private ECollateralSummary ToECollateralSummary( eCollateralItem dbItem )
		{
			return new ECollateralSummary()
			{
				ItemID = dbItem.ItemID,
				FileName = dbItem.FileName,
				LastModifiedDate = dbItem.LastModifiedByDateTime,
				AuthorName = dbItem.CreatedByUser.FullName,
				AuthorImage = dbItem.CreatedByUser.ImageFileName,
				Dealership = !dbItem.IsTemplate ? dbItem.Company.Name : null,
				CustomerName = !dbItem.IsTemplate ? dbItem.CustomerName : null,
				ProjectName = !dbItem.IsTemplate ? dbItem.ProjectName : null,
				ContentType = dbItem.ContentType.HasValue ? LayoutTypes.LayoutTypeList[dbItem.ContentType.Value] : "None",
				LayoutImage = dbItem.LayoutID.HasValue ? Layouts.LayoutImages[dbItem.LayoutID.Value] : "None",
				LayoutName = dbItem.LayoutID.HasValue ? Layouts.LayoutTitles[dbItem.LayoutID.Value] : "None",
				Status = StatusTypes.StatusList[dbItem.Status],
				HasLayout = dbItem.LayoutID.HasValue,
				HasSections = dbItem.eCollateralSections.Any(),
				IsComplete = dbItem.Status != StatusTypes.Incomplete

			};
		}

		public IEnumerable<ECollateralSummary> GetMyPagesList( int skipItems, string filterText, int userId )
		{
			var theList = database.eCollateralItems
				.Where( i => i.CreatedByUserID == userId )
				.Where( i => !i.IsTemplate )
				.Where( i => i.FileName.Contains( filterText ) || i.URLText.Contains( filterText ) )
				.OrderByDescending( i => i.LastModifiedByDateTime );

			return theList
				.Skip( skipItems ).Take( 30 )
				.ToList()
				.Select( i => ToECollateralSummary( i ) );
		}

		public IEnumerable<ECollateralSummary> GetTemplateList( int skipItems, string filterText, bool showIncomplete )
		{
			var theList = database.eCollateralItems
				.Where( i => i.Status == StatusTypes.Approved || (showIncomplete && (i.Status == StatusTypes.Incomplete)) )
				.Where( i => i.IsTemplate )
				.Where( i => i.FileName.Contains( filterText ) || i.URLText.Contains( filterText ) )
				.OrderByDescending( i => i.LastModifiedByDateTime );

			return theList
				.Skip( skipItems ).Take( 30 )
				.ToList()
				.Select( i => ToECollateralSummary( i ) );
		}

		public IEnumerable<ECollateralSummary> GetReviewItemsList( int skipItems, string filterText )
		{
			var theList = database.eCollateralItems
				.Where( i => i.Status == StatusTypes.New || i.Status == StatusTypes.Unapproved )
				.Where( i => !i.IsTemplate )
				.Where( i => i.FileName.Contains( filterText ) || i.URLText.Contains( filterText ) )
				.OrderByDescending( i => i.LastModifiedByDateTime );

			return theList
				.Skip( skipItems ).Take( 30 )
				.ToList()
				.Select( i => ToECollateralSummary( i ) );
		}
	}
}