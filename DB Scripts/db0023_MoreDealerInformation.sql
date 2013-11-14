alter table [Dealers] add 
	[ProductsHeadline] [nvarchar](200) null,
	[PagesHeadline] [nvarchar](200) null,
	[VideosHeadline] [nvarchar](200) null
go
update [Dealers] set [ProductsHeadline] = N'Products We Discussed', PagesHeadline = N'Pages We Discussed', VideosHeadline = N'Videos You Missed'
go