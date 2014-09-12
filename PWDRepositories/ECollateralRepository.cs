using System;
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
				{ Layout12ColumnSidebar, 8 },
				{ Layout13ColumnSidebar, 10 },
				{ Layout12LeftDetailSidebar, 8 },
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
				{ Layout12ColumnSidebar, "2col-sm.png" },
				{ Layout13ColumnSidebar, "3col-sm.png" },
				{ Layout12LeftDetailSidebar, "left-detail-sm.png" },
			};

			public static Dictionary<int, List<string>> LayoutDefaultSections = new Dictionary<int, List<string>>()
			{
				{ Layout12ColumnSidebar, new List<string>() {
					"&lt;h1&gt;Type A Headline Here&lt;/h1&gt;  &lt;h2&gt;Type a subheadline here or delete it if you don't want one.&lt;/h2&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h3&gt;Useful Links&lt;/h3&gt;  &lt;ul&gt;  &lt;li&gt;Link one&lt;/li&gt;  &lt;li&gt;Link two&lt;/li&gt;  &lt;li&gt;Link three&lt;/li&gt;  &lt;li&gt;Link four&lt;/li&gt;  &lt;/ul&gt;",
					"&lt;h3&gt;More Information&lt;/h3&gt;  &lt;ul&gt;  &lt;li&gt;List item one&lt;/li&gt;  &lt;li&gt;List item two&lt;/li&gt;  &lt;li&gt;List item three&lt;/li&gt;  &lt;li&gt;List item four&lt;/li&gt;  &lt;/ul&gt;",
					"&lt;h3&gt;Contact me&lt;/h3&gt;  &lt;p&gt;Name&lt;br /&gt;Address&lt;br /&gt;City, ST ZIP&lt;/p&gt;  &lt;p&gt;(317) 555-1234&lt;/p&gt;  &lt;p&gt;Email me at jane.doe@email.com&lt;/p&gt;  &lt;p&gt;www.paoli.com&lt;/p&gt;"
				} },
				{ Layout13ColumnSidebar, new List<string>() {
					"&lt;h1&gt;Type A Headline Here&lt;/h1&gt;  &lt;h2&gt;Type a subheadline here or delete it if you don't want one.&lt;/h2&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h3&gt;Useful Links&lt;/h3&gt;  &lt;ul&gt;  &lt;li&gt;Link one&lt;/li&gt;  &lt;li&gt;Link two&lt;/li&gt;  &lt;li&gt;Link three&lt;/li&gt;  &lt;li&gt;Link four&lt;/li&gt;  &lt;/ul&gt;",
					"&lt;h3&gt;More Information&lt;/h3&gt;  &lt;ul&gt;  &lt;li&gt;List item one&lt;/li&gt;  &lt;li&gt;List item two&lt;/li&gt;  &lt;li&gt;List item three&lt;/li&gt;  &lt;li&gt;List item four&lt;/li&gt;  &lt;/ul&gt;",
					"&lt;h3&gt;Contact me&lt;/h3&gt;  &lt;p&gt;Name&lt;br /&gt;Address&lt;br /&gt;City, ST ZIP&lt;/p&gt;  &lt;p&gt;(317) 555-1234&lt;/p&gt;  &lt;p&gt;Email me at jane.doe@email.com&lt;/p&gt;  &lt;p&gt;www.paoli.com&lt;/p&gt;"
				} },
				{ Layout12LeftDetailSidebar, new List<string>() {
					"&lt;h1&gt;Type A Headline Here&lt;/h1&gt;  &lt;h2&gt;Type a subheadline here or delete it if you don't want one.&lt;/h2&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h2&gt;Type A Headline Here&lt;/h2&gt;  &lt;h4&gt;Type a subheadline here or delete it if you don't want one.&lt;/h4&gt;  &lt;p&gt;Your supporting paragraph or bullet points can go here. If you do not want this text box to appear on your published page, simply delete all of the text and leave this blank.&lt;/p&gt;",
					"&lt;h3&gt;Useful Links&lt;/h3&gt;  &lt;ul&gt;  &lt;li&gt;Link one&lt;/li&gt;  &lt;li&gt;Link two&lt;/li&gt;  &lt;li&gt;Link three&lt;/li&gt;  &lt;li&gt;Link four&lt;/li&gt;  &lt;/ul&gt;",
					"&lt;h3&gt;More Information&lt;/h3&gt;  &lt;ul&gt;  &lt;li&gt;List item one&lt;/li&gt;  &lt;li&gt;List item two&lt;/li&gt;  &lt;li&gt;List item three&lt;/li&gt;  &lt;li&gt;List item four&lt;/li&gt;  &lt;/ul&gt;",
					"&lt;h3&gt;Contact me&lt;/h3&gt;  &lt;p&gt;Name&lt;br /&gt;Address&lt;br /&gt;City, ST ZIP&lt;/p&gt;  &lt;p&gt;(317) 555-1234&lt;/p&gt;  &lt;p&gt;Email me at jane.doe@email.com&lt;/p&gt;  &lt;p&gt;www.paoli.com&lt;/p&gt;"
				} },
			};

			public class LayoutDetails
			{
				public int Layout { get; set; }
				public string Name { get; set; }
				public string ImageName { get; set; }
			}
		}

		public ECollateralRepository()
		{
		}

		public IEnumerable<Layouts.LayoutDetails> GetLayoutSelectionDetails( int layoutType )
		{
			var retList = new List<Layouts.LayoutDetails>();
			foreach( var layout in LayoutTypes.LayoutList[layoutType] )
			{
				retList.Add( new Layouts.LayoutDetails()
				{
					Name = Layouts.LayoutTitles[layout],
					Layout = layout,
					ImageName = Layouts.LayoutImages[layout]
				} );
			}

			return retList;
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
			if( settings.DealershipID != 0 )
			{
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
			if( !ValidateURL( settings.ItemID, settings.CustomURL ) )
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
				if( settings.DealershipID != 0 )
				{
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
				}
				else
				{
					dbItem.DealershipID = null;
					dbItem.DealershipPOCID = null;
					dbItem.DealershipPOCName = null;
					dbItem.DealershipPOCEmail = null;
					dbItem.DealershipPOCPhone = null;
					dbItem.DealershipPOCAcctType = null;
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

					var defaultList = Layouts.LayoutDefaultSections[dbItem.LayoutID.Value];
					for( int i = 0; i < defaultList.Count; i++ )
					{
						var dbSection = new eCollateralSection();
						dbSection.Content = defaultList[i];
						dbSection.Sequence = i;

						dbItem.eCollateralSections.Add( dbSection );
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
				Status = StatusTypes.StatusList[dbItem.Status],
				UpdateStatus = dbItem.Status,
				CustomURL = dbItem.URLText
			};

			if( !dbItem.IsTemplate )
			{
				retItem.PaoliSalesRepGroupID = dbItem.DealershipID.HasValue ? dbItem.Company.Territory.SalesRepCompanyID : null;
				retItem.DealershipID = dbItem.DealershipID;
				retItem.DealershipPOCID = dbItem.DealershipPOCID;
				retItem.DealershipPOCName = dbItem.DealershipPOCName;
				retItem.DealershipPOCEmail = dbItem.DealershipPOCEmail;
				retItem.DealershipPOCPhone = dbItem.DealershipPOCPhone;
				retItem.DealershipPOCAcctType = dbItem.DealershipPOCAcctType;
				retItem.CustomerName = dbItem.CustomerName;
				retItem.ProjectName = dbItem.ProjectName;
				retItem.PaoliSalesRepGroupName = dbItem.DealershipID.HasValue ? dbItem.Company.Territory.SalesRepCompanyName : null;
				retItem.DealershipName = dbItem.DealershipID.HasValue ? dbItem.Company.Name : null;
				retItem.DealershipPOCMember = dbItem.DealershipPOCID.HasValue ? dbItem.DealershipPOC.FullName : null;
				retItem.DealershipPOCAcctTypeName = dbItem.DealershipPOCAcctType.HasValue ? PaoliWebUser.PaoliWebRole.RoleList[dbItem.DealershipPOCAcctType.Value] : null;
			}

			return retItem;
		}

		private ECollateralDetails ToECollateralDetails( eCollateralItem dbItem )
		{
			var dInfo = new ECollateralDetails();
			dInfo.ItemID = dbItem.ItemID;
			dInfo.Name = dbItem.FileName;
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
				if( dbItem.LayoutID.HasValue )
				{
					return ToECollateralDetails( dbItem );
				}

				return null;
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

		public bool SetItemSections( ECollateralDetails sectionInfo, int userId, out bool bNeedVerify )
		{
			bNeedVerify = false;

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

				bNeedVerify = ((dbItem.Status == StatusTypes.Incomplete) || (dbItem.Status == StatusTypes.Unapproved));
				if( dbItem.Status == StatusTypes.Approved && !dbItem.IsTemplate )
				{
					dbItem.Status = StatusTypes.New;
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
				if( dbItem.DealershipPOCID.HasValue )
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
				Dealership = !dbItem.IsTemplate && dbItem.DealershipID.HasValue ? dbItem.Company.Name : null,
				CustomerName = !dbItem.IsTemplate ? dbItem.CustomerName : null,
				ProjectName = !dbItem.IsTemplate ? dbItem.ProjectName : null,
				ContentType = dbItem.ContentType.HasValue ? LayoutTypes.LayoutTypeList[dbItem.ContentType.Value] : "None",
				LayoutImage = dbItem.LayoutID.HasValue ? Layouts.LayoutImages[dbItem.LayoutID.Value] : "transparent.png",
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

		public IEnumerable<ECollateralSummary> GetAllItemsList( int skipItems, string filterText )
		{
			var theList = database.eCollateralItems
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

		public bool UpdateStatus( int itemId, int updateStatus )
		{
			var dbItem = database.eCollateralItems.FirstOrDefault( i => i.ItemID == itemId );
			if( dbItem != null )
			{
				dbItem.Status = updateStatus;

				return database.SaveChanges() > 0;
			}
			return false;
		}

		public bool DeletePage( int itemId )
		{
			var dbItem = database.eCollateralItems.FirstOrDefault( i => i.ItemID == itemId );
			if( dbItem != null )
			{
				foreach( var section in dbItem.eCollateralSections.ToList() )
				{
					database.eCollateralSections.DeleteObject( section );
				}

				database.eCollateralItems.DeleteObject( dbItem );

				return database.SaveChanges() > 0;
			}
			return false;
		}
	}
}
