alter table [Companies] add [PaoliMemberID] [int] null constraint [fkCompany_PaoliMember] foreign key references [Users](UserID)
go
