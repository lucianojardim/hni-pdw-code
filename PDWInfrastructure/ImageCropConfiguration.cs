using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace PDWInfrastructure
{
	public class ImageCropConfiguration : ConfigurationSection
	{
		[ConfigurationProperty( "ImageSizes" )]
		public CropSettingCollection ImageSizes
		{
			get
			{
				return (CropSettingCollection)this["ImageSizes"];
			}
			set
			{
				this["ImageSizes"] = value;
			}
		}

		[ConfigurationCollection( typeof( CropSettingElement ), AddItemName = "CropSetting", CollectionType = ConfigurationElementCollectionType.BasicMap )]
		public class CropSettingCollection : ConfigurationElementCollection
		{
			protected override ConfigurationElement CreateNewElement()
			{
				return new CropSettingElement();
			}

			protected override object GetElementKey( ConfigurationElement element )
			{
				if( element == null )
					throw new ArgumentNullException( "element" );

				return ((CropSettingElement)element).Suffix;
			}
		}

		public class CropSettingElement : ConfigurationElement
		{
			[ConfigurationProperty( "Description", DefaultValue = "", IsRequired = true )]
			public string Description { get { return (string)this["Description"]; } set { this["Description"] = value; } }

			[ConfigurationProperty( "Suffix", DefaultValue = "", IsRequired = true, IsKey = true )]
			public string Suffix { get { return (string)this["Suffix"]; } set { this["Suffix"] = value; } }

			[ConfigurationProperty( "RatioWidth", DefaultValue = "1", IsRequired = true )]
			public int RatioWidth { get { return (int)this["RatioWidth"]; } set { this["RatioWidth"] = value; } }

			[ConfigurationProperty( "RatioHeight", DefaultValue = "1", IsRequired = true )]
			public int RatioHeight { get { return (int)this["RatioHeight"]; } set { this["RatioHeight"] = value; } }

			[ConfigurationProperty( "MaxHeight", DefaultValue = "100", IsRequired = true )]
			public int MaxHeight { get { return (int)this["MaxHeight"]; } set { this["MaxHeight"] = value; } }

			[ConfigurationProperty( "MaxWidth", DefaultValue = "100", IsRequired = true )]
			public int MaxWidth { get { return (int)this["MaxWidth"]; } set { this["MaxWidth"] = value; } }

			[ConfigurationProperty( "CropType", DefaultValue = "", IsRequired = true )]
			public string CropType { get { return (string)this["CropType"]; } set { this["CropType"] = value; } }
		}
	}
}
