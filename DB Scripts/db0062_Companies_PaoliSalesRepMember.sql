alter table [Companies] add [PaoliSalesRepMemberID] [int] null constraint [fkCompany_PaoliSalesRepMember] foreign key references [Users](UserID)
go
