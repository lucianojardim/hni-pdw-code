alter table [Users] drop column [UserType]
go
alter table [Users] add [AccountType] [int] not null constraint [defAcctType] default(0)
go
update [Users] set [AccountType] = 1 where [Role] = 'Super Admin'
update [Users] set [AccountType] = 3 where [Role] = 'Paoli Staff Marketing'
update [Users] set [AccountType] = 4 where [Role] = 'Paoli Staff Spec Team'
update [Users] set [AccountType] = 6 where [Role] = 'Paoli Staff Support'
update [Users] set [AccountType] = 7 where [Role] = 'Paoli Staff Sales Rep'
update [Users] set [AccountType] = 9 where [Role] = 'Dealer Sales Rep'
update [Users] set [AccountType] = 8 where [Role] = 'Dealer Principle'
go
alter table [Users] drop column [Role]
go
