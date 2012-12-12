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
								attForSeries.Value = (val ?? "").Any() ? val : (header + " goes here");
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
						default:
							if( (val ?? "").Any() )
							{
								var attData = database.Attributes.FirstOrDefault( a => a.Name == header );
								if( attData == null )
								{
									attData = new PDWDBContext.Attribute();
									attData.Name = header;
									database.Attributes.AddObject( attData );
								}
								var values = val.Split( ',' );
								foreach( var indVal in values.Select( s => s.Trim() ) )
								{
									var optVal = attData.AttributeOptions.FirstOrDefault( ao => ao.Name == indVal );
									if( optVal == null )
									{
										if( indVal.Length > 100 )
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
					foreach( var comboData in otherSeries.Select( o => o.Trim().Split( '-' ) ) )
					{
						if( comboData.Length == 2 )
						{
							string categoryName = comboData[0].Trim();
							string seriesName = comboData[1].Trim();
							var category = database.Categories.FirstOrDefault( c => c.Name == categoryName );
							if( category != null )
							{
								var rSeries = category.Serieses.FirstOrDefault( s => s.Name == seriesName );
								if( rSeries != null )
								{
									rSeries.ParentSerieses.Add( parentSeries );
								}
							}
						}
					}
				}
			}

			database.SaveChanges();
		}
	}
}
