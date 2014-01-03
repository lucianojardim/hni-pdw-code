using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace PDWInfrastructure
{
	public class ExchangeMailConfiguration : ConfigurationSection
	{
		[ConfigurationProperty( "Settings" )]
		public ExchangeSettings Settings
		{
			get
			{
				return (ExchangeSettings)this["Settings"];
			}
			set
			{
				this["Settings"] = value;
			}
		}

		public static ExchangeMailConfiguration Config
		{
			get
			{
				return (ExchangeMailConfiguration)ConfigurationManager.GetSection( "exchangeSettings" );
			}
		}

		public class ExchangeSettings : ConfigurationElement
		{
			[ConfigurationProperty( "ServerAddress", IsRequired = true )]
			public string ServerAddress { get { return (string)this["ServerAddress"]; } set { this["ServerAddress"] = value; } }

			[ConfigurationProperty( "Username", IsRequired = true )]
			public string Username { get { return (string)this["Username"]; } set { this["Username"] = value; } }

			[ConfigurationProperty( "Password", IsRequired = true )]
			public string Password { get { return (string)this["Password"]; } set { this["Password"] = value; } }

			[ConfigurationProperty( "Domain", IsRequired = true )]
			public string Domain { get { return (string)this["Domain"]; } set { this["Domain"] = value; } }

			[ConfigurationProperty( "UseExchange", IsRequired = true )]
			public bool UseExchange { get { return (bool)this["UseExchange"]; } set { this["UseExchange"] = value; } }
		}
	}
}
