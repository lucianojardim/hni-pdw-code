alter table [Serieses] add [IsActive] bit not null constraint [defactive] default(1)
go
alter table [Serieses] drop constraint [defactive]
go