alter table [eCollateralItems] add [IncludeDealerImage] [bit] not null constraint [defIDI] default(0)
alter table [eCollateralItems] drop constraint [defIDI]
go
