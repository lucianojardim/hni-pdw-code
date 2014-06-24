create table [ScoopArticles] (
	[ArticleID] [int] identity(1,1) not null,
	[BigHeadline] [nvarchar](500) null,
	[BigText] [ntext] null,
	[BigImageURL] [nvarchar](200) null,
	[SmallHeadline] [nvarchar](500) null,
	[SmallText] [ntext] null,
	[SmallImageURL] [nvarchar](200) null,
	[ContentBlock] [ntext] null,
	[Title] [nvarchar](200) null,
	[PubDate] [datetime] null,
	[AuthorName] [nvarchar](100) null,
	[Rank] [int] not null,
	CONSTRAINT [pkScoopArticles] PRIMARY KEY CLUSTERED ([ArticleID])	
) on [primary]