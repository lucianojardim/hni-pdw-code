alter table [SpecRequests] add
	[PaoliSalesRepMemberID] [int] null constraint [fkSpecRequest_PSRMember] foreign key references [Users](UserID),
	
	[EndCustomer] [nvarchar](100) null,
	[ProjectSize] [nvarchar](100) null,
	[QuoteDueDate] [datetime] null,
	
	[NeedFloorplanSpecs] [bit] not null constraint [defNeedFloorplanSpecs] default(0),
	[Need3DDrawing] [bit] not null constraint [defNeed3DDrawing] default(0),
	[NeedValueEng] [bit] not null constraint [defNeedValueEng] default(0),
	[NeedPhotoRendering] [bit] not null constraint [defNeedPhotoRendering] default(0),
	[Need2DDrawing] [bit] not null constraint [defNeed2DDrawing] default(0),
	[NeedAuditSpecs] [bit] not null constraint [defNeedAuditSpecs] default(0),
	
	[Casegoods] [nvarchar](max) null,
	[Conferencing] [nvarchar](max) null,
	[Seating] [nvarchar](max) null,
	[Finishes] [nvarchar](max) null,
	[OtherFinishDetails] [nvarchar](max) null,
	[Grommets] [bit] not null constraint [defGrommets] default(0),
	[GrommetDetails] [ntext] null,
	[DrawerOption] [nvarchar](100) null,
	[FabricGrade] [nvarchar](5) null,
	[FabricDetails] [nvarchar](100) null,
	
	[SpecialRequests] [ntext] null
go

alter table [SpecRequests] drop constraint [defNeedFloorplanSpecs]
alter table [SpecRequests] drop constraint [defNeed3DDrawing]
alter table [SpecRequests] drop constraint [defNeedValueEng]
alter table [SpecRequests] drop constraint [defNeedPhotoRendering]
alter table [SpecRequests] drop constraint [defNeed2DDrawing]
alter table [SpecRequests] drop constraint [defNeedAuditSpecs]
alter table [SpecRequests] drop constraint [defGrommets]
go

update SpecRequests set [Casegoods] = null, [Conferencing] = null, [Seating] = null 
go
set nocount on
go
declare @catCasegoods int, @catSeating int, @catTables int
select @catCasegoods = CategoryId from Categories where Name = 'Casegood'
select @catSeating = CategoryId from Categories where Name = 'Seating'
select @catTables = CategoryId from Categories where Name = 'Tables'

declare @reqId int, @idx int, @newIdx int, @newCat int
declare @fSeries nvarchar(100), @seriesList nvarchar(max), @fullList nvarchar(max), @newSeries nvarchar(100)
declare @fullCasegoods nvarchar(max), @fullSeating nvarchar(max), @fullTables nvarchar(max)
declare c1 cursor local for select RequestID, FeaturedSeries, SeriesList from SpecRequests 
open c1
fetch next from c1 into @reqId, @fSeries, @seriesList
while @@fetch_status = 0
begin
	set @fullList = @fSeries + ',' + ISNULL(@seriesList,N'')
	set @idx = 0
	select @fullCasegoods = null, @fullSeating = null, @fullTables = null
	
	while( @idx < LEN(@fullList) )
	begin
		set @newCat = null
		set @newIdx = CHARINDEX( ',', @fullList, @idx )
		if( @newIdx = 0 )
		begin
			set @newIdx = LEN(@fullList) + 1
		end
--		print 'do substring ['+cast(LEN(@fullList) as nvarchar(10))+'] ['+cast(@idx as nvarchar(10))+'] ['+cast(@newIdx as nvarchar(10))+']' + @fullList
		set @newSeries = SUBSTRING(@fullList, @idx, @newIdx - @idx)
		if( LEN(@newSeries) > 0 )
		begin
			select @newCat = CategoryId from Serieses where Name = rtrim(ltrim(@newSeries))
			if( @newCat = @catCasegoods )
			begin
				set @fullCasegoods = isnull(@fullCasegoods, '') + (case when @fullCasegoods <> '' then ',' else '' end) + @newSeries
			end
			else if( @newCat = @catSeating )
			begin
				set @fullSeating = isnull(@fullSeating, '') + (case when @fullSeating <> '' then ',' else '' end) + @newSeries
			end
			else if( @newCat = @catTables )
			begin
				set @fullTables = isnull(@fullTables, '') + (case when @fullTables <> '' then ',' else '' end) + @newSeries
			end
			else
			begin
				print 'unable to find [' + @newseries + ']'
			end
		end
		set @idx = @newIdx + 1
	end
	
	update SpecRequests set [Casegoods] = @fullCasegoods, [Conferencing] = @fullTables, [Seating] = @fullSeating 
		where RequestID = @reqId

	fetch next from c1 into @reqId, @fSeries, @seriesList
end
close c1
deallocate c1
go