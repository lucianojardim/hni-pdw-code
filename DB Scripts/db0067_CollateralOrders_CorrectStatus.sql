
declare @bHasFulfilled bit, @bHasUnfulfilled bit, @bHasPartial bit
declare @OrderID int, @Quantity int, @ShippedQuantity int, @Status int

declare @SPending int, @SPartial int, @SFulfilled int, @SCanceled int
select @SPending = 1, @SPartial = 2, @SFulfilled = 3, @SCanceled = 4

declare c1 cursor local for select OrderID from CollateralOrders where CanceledByUserID is null
open c1
fetch next from c1 into @OrderID
while @@fetch_status = 0
begin
	select @bHasFulfilled = 0, @bHasUnfulfilled = 0, @bHasPartial = 0

	declare c2 cursor local for select co.Quantity, coalesce((select SUM(cosd.Quantity) from CollateralOrderShipmentDetails cosd where cosd.OrderDetailID = co.DetailID), 0) from CollateralOrderDetails co where co.OrderID = @OrderID
	open c2
	fetch next from c2 into @Quantity, @ShippedQuantity
	while @@fetch_status = 0
	begin
		set @bHasFulfilled = case when (@Quantity = @ShippedQuantity) or (@bHasFulfilled = 1) then 1 else 0 end
		set @bHasUnfulfilled = case when (0 = @ShippedQuantity) or (@bHasUnfulfilled = 1) then 1 else 0 end
		set @bHasPartial = case when ( (@ShippedQuantity > 0) and (@Quantity > @ShippedQuantity) ) or (@bHasPartial = 1) then 1 else 0 end
		
		fetch next from c2 into @Quantity, @ShippedQuantity
	end
	close c2
	deallocate c2
	
	set @Status = @SPartial
	if( @bHasFulfilled = 0 and @bHasPartial = 0 )
	begin
		set @Status = @SPending
	end
	else if( @bHasUnfulfilled = 0 and @bHasPartial = 0 )
	begin
		set @Status = @SFulfilled
	end
	
	update CollateralOrders set [Status] = @Status where OrderID = @OrderID

	fetch next from c1 into @OrderID
end

close c1
deallocate c1
