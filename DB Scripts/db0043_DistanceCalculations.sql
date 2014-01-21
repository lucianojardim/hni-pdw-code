create function DistanceBetweenPoints( @Latitude1 float,@Longitude1 float, @Latitude2 float, @Longitude2 float )
returns float
as
begin
	declare @radius float

	declare @lon1  float
	declare @lon2  float
	declare @lat1  float
	declare @lat2  float

	declare @a float
	declare @distance float

	-- Sets average radius of Earth in Kilometers
	set @radius = 3959.0E

	-- Convert degrees to radians
	set @lon1 = radians( @Longitude1 )
	set @lon2 = radians( @Longitude2 )
	set @lat1 = radians( @Latitude1 )
	set @lat2 = radians( @Latitude2 )

	set @a = sqrt(square(sin((@lat2-@lat1)/2.0E)) + 
		(cos(@lat1) * cos(@lat2) * square(sin((@lon2-@lon1)/2.0E))) )

	set @distance =
		@radius * ( 2.0E *asin(case when 1.0E < @a then 1.0E else @a end ))

	return @distance

end
go

create procedure ClosestShowroom( @lat float, @long float ) as
begin
	select Showrooms.* from Showrooms 
		inner join ZipCodeInfo on Showrooms.Zip = ZipCodeInfo.ZipCode
	order by dbo.DistanceBetweenPoints( @lat, @long, ZipCodeInfo.Latitude, ZipCodeInfo.Longitude )
end
go
/*
exec ClosestShowroom 38.093604, -78.561139 

select * from zipcodeinfo where zipcode in ('46074', '22901')

select dbo.DistanceBetweenPoints( 38.093604, -78.561139, 40.097028, -86.125716 )

select sqrt(square(-78.561139+86.125716) + square(40.097028-38.093604)) * 69
*/