alter table [Users] add [DefaultShippingAddress] [int] not null constraint [defShippingAddress] default(1)
alter table [Users] drop constraint [defShippingAddress]
go