using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace PDWInfrastructure
{
	public class EncryptionConfiguration : ConfigurationSection
	{
		[ConfigurationProperty( "DataPassPhrase" )]
		public PassPhraseElement DataPassPhrase
		{
			get
			{
				return (PassPhraseElement)this["DataPassPhrase"];
			}
			set
			{
				this["DataPassPhrase"] = value;
			}
		}

		public class PassPhraseElement : ConfigurationElement
		{
			[ConfigurationProperty( "value", DefaultValue = "", IsRequired = true )]
			public String Value
			{
				get
				{
					return (String)this["value"];
				}
				set
				{
					this["value"] = value;
				}
			}
		}
	}
}
