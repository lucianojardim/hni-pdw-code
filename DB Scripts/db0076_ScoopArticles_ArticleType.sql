alter table [ScoopArticles] add [ArticleType] [int] not null constraint [defArticleType] default(1)
alter table [ScoopArticles] drop constraint [defArticleType]
go