create table [HomePageContent] (
	[PageID] [int] identity(1,1) not null,
	[ContentData] [nvarchar](max) null,
	CONSTRAINT [pkHomePageContent] PRIMARY KEY CLUSTERED ([PageID])	
) on [PRIMARY]
go