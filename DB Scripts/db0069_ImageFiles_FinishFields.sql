alter table [ImageFiles] add
	[LaminatePattern] [int] null,
	[LaminateType] [int] null,
	[VeneerGrade] [int] null,
	[VeneerSpecies] [int] null,
	[VeneerColorTone] [int] null
go

update [ImageFiles] set [LaminatePattern] = [FinishSubType] where [FinishType] = 2
update [ImageFiles] set [VeneerSpecies] = [FinishSubType] where [FinishType] = 1
go

alter table [ImageFiles] drop column [FinishSubType]
go

alter table [ImageFiles] drop column [LaminateType]
go

alter table [ImageFiles] add 
	[LaminateIsHPL] [bit] not null constraint [defLHPL] default(0),
	[LaminateIsLPL] [bit] not null constraint [defLLPL] default(0)
go

alter table [ImageFiles] drop constraint [defLHPL]
alter table [ImageFiles] drop constraint [defLLPL]
go

alter table [ImageFiles] drop column [VeneerColorTone]
go
