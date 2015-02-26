alter table [ImageFiles] add [SeatingGrade] [int] null
go
update [ImageFiles] set [SeatingGrade] = 1 where [FinishType] = 3
go