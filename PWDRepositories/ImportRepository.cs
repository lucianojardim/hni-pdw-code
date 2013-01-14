using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDWDBContext;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using System.Data.Objects;
using System.Configuration;
using PDWModels.Images;
using System.Diagnostics;

namespace PWDRepositories
{
	public class ImportRepository
	{
		private PaoliPDWEntities database = new PaoliPDWEntities();

		public ImportRepository()
		{
		}

		private void DeleteAllObjects( string dbTable )
		{
			database.ExecuteStoreCommand( string.Format( "DELETE FROM {0}", dbTable ) );
		}

		public void ImportFileData( Stream fStream, int fileLength )
		{
			var csvReader = new CsvReader( new StreamReader( fStream ), true );

			DeleteAllObjects( "[SeriesOptionAttributes]" );
			DeleteAllObjects( "[SeriesTextAttributes]" );
			DeleteAllObjects( "[SeriesIntAttributes]" );
			DeleteAllObjects( "[AttributeOptions]" );
			DeleteAllObjects( "[Attributes]" );
			DeleteAllObjects( "[SeriesImageFiles]" );
			DeleteAllObjects( "[RelatedSeries]" );
			DeleteAllObjects( "[SeriesTypicals]" );
			DeleteAllObjects( "[Serieses]" );
			DeleteAllObjects( "[Categories]" );

			Dictionary<int, string> mapRelatedSeries = new Dictionary<int, string>();

			while( csvReader.ReadNextRecord() )
			{
				Series sData = new Series();
				sData.CreatedDate = DateTime.Now;

				string relatedSeries = "";

				foreach( var header in csvReader.GetFieldHeaders() )
				{
					string val = csvReader[header];
					switch( header.ToLower() )
					{
						case "combo":
						case "image-1":
						case "image-2":
						case "image-3":
							// fields to skip
							break;
						case "category":
							{
								var cat = database.Categories.FirstOrDefault( c => c.Name == val );
								if( cat == null )
								{
									cat = new Category();
									cat.Name = val;
									database.Categories.AddObject( cat );
								}
								sData.Category = cat;
							}
							break;
						case "series name":
							sData.Name = val;
							break;
						case "value statement headline":
						case "marketing statement subheadline":
						case "marketing description":
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute();
									attData.Name = header;
									database.Attributes.AddObject( attData );
								}

								var attForSeries = new SeriesTextAttribute();
								attForSeries.Attribute = attData;
								attForSeries.Value = val ?? "";
								attForSeries.Series = sData;
								database.SeriesTextAttributes.AddObject( attForSeries );
							}
							break;
						case "image - featured":
						case "image":
							if( (val ?? "").Any() )
							{
								var values = val.Split( ',' );
								foreach( var indVal in values.Select( s => s.Trim() ) )
								{
									var img = database.ImageFiles.FirstOrDefault( i => i.Name == indVal );
									if( img != null )
									{
										SeriesImageFile sif = new SeriesImageFile();
										sif.IsFeatured = (header.ToLower() == "image - featured");
										sif.ImageFile = img;
										sif.Series = sData;
										database.SeriesImageFiles.AddObject( sif );
									}
								}
							}
							break;
						case "winning assortment":
							if( (val ?? "").Any() )
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == "ranking" );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute();
									attData.Name = "Ranking";
									database.Attributes.AddObject( attData );
								}

								var attForSeries = new SeriesIntAttribute();
								attForSeries.Attribute = attData;
								attForSeries.Value = val.ToLower() == "yes" ? 2 : 1;
								attForSeries.Series = sData;
								database.SeriesIntAttributes.AddObject( attForSeries );
							}
							break;
						case "related series":
							if( (val ?? "").Any() )
							{
								relatedSeries = val;
							}
							break;
						case "price guide":
						case "product technical spec sheet":
						case "spec guide":
						case "gsa spec sheet":
						case "installation guide":
						case "eds sheet":
						case "control guide":
						case "brochure file name":
							if( (val ?? "").Any() )
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute();
									attData.Name = header;
									database.Attributes.AddObject( attData );
								}

								var attForSeries = new SeriesTextAttribute();
								attForSeries.Attribute = attData;
								attForSeries.Value = val;
								attForSeries.Series = sData;
								database.SeriesTextAttributes.AddObject( attForSeries );
							}
							break;
						case "claires 5 favorites":
							if( (val ?? "").Any() )
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute();
									switch( header.ToLower() )
									{
										default:
											attData.DetailItem = true;
											break;
									}
									attData.Name = header;
									database.Attributes.AddObject( attData );
								}
								var values = val.Split( '\r', '\n' );
								foreach( var indVal in values.Select( s => s.Trim() ) )
								{
									if( indVal.Length > 0 )
									{
										var optVal = attData.AttributeOptions.FirstOrDefault( ao => ao.Name == indVal );
										if( optVal == null )
										{
											if( indVal.Length > 500 )
											{
												throw new Exception( string.Format( "Cannot add option value {0} for {1}", indVal, header ) );
											}
											optVal = new AttributeOption();
											optVal.Name = indVal;
											database.AttributeOptions.AddObject( optVal );
											attData.AttributeOptions.Add( optVal );
										}

										var attForSeries = new SeriesOptionAttribute();
										attForSeries.Attribute = attData;
										attForSeries.AttributeOption = optVal;
										attForSeries.Series = sData;
										database.SeriesOptionAttributes.AddObject( attForSeries );
									}
								}
							}
							break;
						default:
							if( (val ?? "").Any() )
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute();
									switch( header.ToLower() )
									{
										default:
											attData.DetailItem = true;
											break;
									}
									attData.Name = header;
									database.Attributes.AddObject( attData );
								}
								var values = val.Split( ',' );
								foreach( var indVal in values.Select( s => s.Trim() ) )
								{
									var optVal = attData.AttributeOptions.FirstOrDefault( ao => ao.Name == indVal );
									if( optVal == null )
									{
										if( indVal.Length > 500 )
										{
											throw new Exception( string.Format( "Cannot add option value {0} for {1}", indVal, header ) );
										}
										optVal = new AttributeOption();
										optVal.Name = indVal;
										database.AttributeOptions.AddObject( optVal );
										attData.AttributeOptions.Add( optVal );
									}

									var attForSeries = new SeriesOptionAttribute();
									attForSeries.Attribute = attData;
									attForSeries.AttributeOption = optVal;
									attForSeries.Series = sData;
									database.SeriesOptionAttributes.AddObject( attForSeries );
								}
							}
							break;
					}
				}
				database.Serieses.AddObject( sData );
				database.SaveChanges();
				database.Refresh( RefreshMode.StoreWins, sData );

				if( relatedSeries.Any() )
				{
					mapRelatedSeries.Add( sData.SeriesID, relatedSeries );
				}
			}

			foreach( int seriesId in mapRelatedSeries.Keys )
			{
				var parentSeries = database.Serieses.FirstOrDefault( s => s.SeriesID == seriesId );
				if( parentSeries != null )
				{
					var otherSeries = mapRelatedSeries[seriesId].Split( ',' );
					foreach( var seriesName in otherSeries.Select( o => o.Trim() ) )
					{
						var rSeries = database.Serieses.FirstOrDefault( s => s.Name == seriesName );
						if( rSeries != null )
						{
							rSeries.ParentSerieses.Add( parentSeries );
						}
						else
						{
							Debug.WriteLine( format: "Unable to find series: {0}", args: seriesName );
						}
					}
				}
			}

			database.SaveChanges();
		}

		public void ImportTypicalFileData( Stream fStream, int fileLength )
		{
			var csvReader = new CsvReader( new StreamReader( fStream ), true );

			DeleteAllObjects( "[TypicalOptionAttributes]" );
			DeleteAllObjects( "[TypicalTextAttributes]" );
			DeleteAllObjects( "[TypicalIntAttributes]" );
			DeleteAllObjects( "[TAttributeOptions]" );
			DeleteAllObjects( "[TAttributes]" );
			DeleteAllObjects( "[TypicalImageFiles]" );
			DeleteAllObjects( "[SeriesTypicals]" );
			DeleteAllObjects( "[Typicals]" );

			while( csvReader.ReadNextRecord() )
			{
				Typical tData = new Typical();
				tData.CreatedDate = DateTime.Now;

				string categoryName = null;
				foreach( var header in csvReader.GetFieldHeaders() )
				{
					string val = csvReader[header];
					switch( header.ToLower() )
					{
						case "category":
							if( (val ?? "").Any() )
							{
								categoryName = val.Trim();
							}
							break;
						case "series name":
							if( (val ?? "").Any() )
							{
								string seriesName = val.Trim();
								var category = database.Categories.FirstOrDefault( c => c.Name == categoryName );
								if( category != null )
								{
									var rSeries = category.Serieses.FirstOrDefault( s => s.Name == seriesName );
									if( rSeries != null )
									{
										SeriesTypical stData = new SeriesTypical();
										stData.IsPrimary = true;
										stData.Series = rSeries;
										stData.Typical = tData;
										database.SeriesTypicals.AddObject( stData );
									}
								}
							}
							break;
						case "typical name":
							tData.Name = val;
							break;
						case "iso main image":
						case "image":
							if( (val ?? "").Any() )
							{
								var values = val.Split( ',' );
								foreach( var indVal in values.Select( s => s.Trim() ) )
								{
									var img = database.ImageFiles.FirstOrDefault( i => i.Name == indVal );
									if( img != null )
									{
										TypicalImageFile sif = new TypicalImageFile();
										sif.IsFeatured = (header.ToLower() == "iso main image");
										sif.ImageFile = img;
										sif.Typical = tData;
										database.TypicalImageFiles.AddObject( sif );
									}
								}
							}
							break;
						case "pricing":
							if( (val ?? "").Any() )
							{
								int price = 0;
								
								if( int.TryParse( val, System.Globalization.NumberStyles.Currency, null, out price ) )
								{
									var attData = database.TAttributes.FirstOrDefault( a => a.Name == header );
									if( attData == null )
									{
										attData = new PDWDBContext.TAttribute();
										attData.Name = header;
										database.TAttributes.AddObject( attData );
									}

									var attForTypical = new TypicalIntAttribute();
									attForTypical.TAttribute = attData;
									attForTypical.Value = price;
									attForTypical.Typical = tData;
									database.TypicalIntAttributes.AddObject( attForTypical );
								}
							}
							break;
						case "price guide":
						case "product technical spec sheet":
						case "spec guide":
						case "gsa spec sheet":
						case "installation guide":
						case "eds sheet":
						case "control guide":
						case "spec & price xls":
						case "spec & price sif":
						case "spec & price sp4":
						case "spec & price pdf":
						case "drawing dwg":
						case "drawing pdf":
							if( (val ?? "").Any() )
							{
								var attData = database.TAttributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.TAttribute();
									attData.Name = header;
									database.TAttributes.AddObject( attData );
								}

								var attForTypical = new TypicalTextAttribute();
								attForTypical.TAttribute = attData;
								attForTypical.Value = val;
								attForTypical.Typical = tData;
								database.TypicalTextAttributes.AddObject( attForTypical );
							}
							break;
						default:
							if( (val ?? "").Any() )
							{
								var attData = database.TAttributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.TAttribute();
									switch( header.ToLower() )
									{
										case "footprint":
											break;
										default:
											attData.DetailItem = true;
											break;
									}
									attData.Name = header;
									database.TAttributes.AddObject( attData );
								}
								var values = val.Split( ',' );
								foreach( var indVal in values.Select( s => s.Trim() ) )
								{
									var optVal = attData.TAttributeOptions.FirstOrDefault( ao => ao.Name == indVal );
									if( optVal == null )
									{
										if( indVal.Length > 500 )
										{
											throw new Exception( string.Format( "Cannot add option value {0} for {1}", indVal, header ) );
										}
										optVal = new TAttributeOption();
										optVal.Name = indVal;
										database.TAttributeOptions.AddObject( optVal );
										attData.TAttributeOptions.Add( optVal );
									}

									var attForTypical = new TypicalOptionAttribute();
									attForTypical.TAttribute = attData;
									attForTypical.TAttributeOption = optVal;
									attForTypical.Typical = tData;
									database.TypicalOptionAttributes.AddObject( attForTypical );
								}
							}
							break;
					}
				}

				database.Typicals.AddObject( tData );
				database.SaveChanges();
			}
		}
	}
}
