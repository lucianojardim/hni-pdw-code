alter table SeriesImageFiles add [DisplayOrder] [int] not null constraint [defOrder] default(0)
alter table SeriesImageFiles drop constraint [defOrder]
go
update SeriesImageFiles 
	set DisplayOrder = (select count(*) + 1 from SeriesImageFiles sif 
							where SeriesImageFiles.SeriesID = sif.SeriesID and sif.AssociationID < SeriesImageFiles.AssociationID and sif.IsFeatured = 0)
	where SeriesImageFiles.IsFeatured = 0
go
select * from SeriesImageFiles order by SeriesID, IsFeatured desc, DisplayOrder
go
