alter table [ImageFiles] add [LaminateIsTFL] [bit] NOT NULL constraint [defIsTFL] default(0)
alter table [ImageFiles] drop constraint [defIsTFL]
go
update [ImageFiles] set [LaminateIsTFL] = [LaminateIsLPL]
alter table [ImageFiles] drop column [LaminateIsLPL]
go

update [SpecRequests] set [LaminatePreference] = 'TFL' where [LaminatePreference] = 'LPL'
go