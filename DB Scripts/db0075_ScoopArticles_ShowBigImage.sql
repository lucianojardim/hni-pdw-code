alter table [ScoopArticles] add [ShowBigImage] [bit] not null constraint [defSBI] default(1)
alter table [ScoopArticles] drop constraint [defSBI]
go