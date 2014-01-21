alter table AttributeOptions alter column [Name] [nvarchar](500) not null
alter table TAttributeOptions alter column [Name] [nvarchar](500) not null
go

alter table SeriesTextAttributes alter column [Value] [nvarchar](2000) not null
alter table TypicalTextAttributes alter column [Value] [nvarchar](2000) not null
go
