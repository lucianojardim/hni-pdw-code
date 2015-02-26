create function ImagePopularity( @imgId int )
returns int
as
begin
	declare @nRet int
	set @nRet = 0
	
	if exists(select * from SeriesImageFiles sif 
			inner join SeriesIntAttributes sia on sif.SeriesID = sia.SeriesID and sif.ImageID = @imgId
			inner join Attributes a on sia.AttributeID = a.AttributeID and a.Name = 'Ranking'
		where sia.Value = 2)
	begin
		set @nRet = @nRet + 1
	end
	
	if exists( select * from ImageFiles where ImageID = @imgId and ImageType = 'Env' )
	begin
		set @nRet = @nRet + 2
	end
	
	return @nRet
end
go

alter table [ImageFiles] add [Popularity] as dbo.ImagePopularity(ImageID)
go
