CREATE TABLE [VideoLinks](
	[VideoID] [int] IDENTITY(1,1) NOT NULL,
	[Display] [nvarchar](200) NOT NULL,
	[YouTubeID] [nvarchar](20) NOT NULL,
	CONSTRAINT [pkVideoLinks] PRIMARY KEY CLUSTERED ([VideoID])
) ON [PRIMARY]
GO

CREATE TABLE [Dealers](
	[DealerID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[URL] [nvarchar](200) NOT NULL,
	[MainContent] [ntext] NOT NULL,
	[FeaturedVideoID] [int] NOT NULL CONSTRAINT [fkDealer_VideoLink] FOREIGN KEY REFERENCES [VideoLinks] ([VideoID]),
	CONSTRAINT [pkDealers] PRIMARY KEY CLUSTERED ([DealerID])
) ON [PRIMARY]
GO

CREATE TABLE [PageLinks](
	[PageID] [int] IDENTITY(1,1) NOT NULL,
	[Display] [nvarchar](200) NOT NULL,
	[URLLocation] [nvarchar](200) NOT NULL,
	CONSTRAINT [pkPageLinks] PRIMARY KEY CLUSTERED ([PageID])
) ON [PRIMARY]
GO

CREATE TABLE [DealerVideoLinks](
	[DealerID] [int] NOT NULL CONSTRAINT [fkDVL_Dealer] FOREIGN KEY REFERENCES [Dealers] ([DealerID]),
	[VideoID] [int] NOT NULL CONSTRAINT [fkDVL_VideoLink] FOREIGN KEY REFERENCES [VideoLinks] ([VideoID]),
	CONSTRAINT [pkDealerVideoLinks] PRIMARY KEY CLUSTERED ([DealerID], [VideoID])
) ON [PRIMARY]
GO

CREATE TABLE [DealerPageLinks](
	[DealerID] [int] NOT NULL CONSTRAINT [fkDPL_Dealer] FOREIGN KEY REFERENCES [Dealers] ([DealerID]),
	[PageID] [int] NOT NULL CONSTRAINT [fkDPL_PageLink] FOREIGN KEY REFERENCES [PageLinks] ([PageID]),
	CONSTRAINT [pkDealerPageLinks] PRIMARY KEY CLUSTERED ([DealerID], [PageID])
) ON [PRIMARY]
GO

CREATE TABLE [DealerFeaturedProducts](
	[DealerID] [int] NOT NULL CONSTRAINT [fkDFP_Dealer] FOREIGN KEY REFERENCES [Dealers] ([DealerID]),
	[SeriesName] [nvarchar](50) NOT NULL,
	CONSTRAINT [pkDealerFeaturedProducts] PRIMARY KEY CLUSTERED ([DealerID], [SeriesName])
) ON [PRIMARY]
GO
