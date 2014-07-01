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
	[AuthorID] [int] not null CONSTRAINT fkScoopArticle_User FOREIGN KEY REFERENCES [Users]([UserID]),
	[Rank] [int] not null,
	CONSTRAINT [pkScoopArticles] PRIMARY KEY CLUSTERED ([ArticleID])	
) on [primary]
go

alter table [Users] add 
	[AuthorCredit] [nvarchar](500) null
go
