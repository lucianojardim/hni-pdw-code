EXEC sp_rename 'eCollateralItems.DealershipPOCName', 'OldDealershipPOCName', 'COLUMN';
EXEC sp_rename 'eCollateralItems.DealershipPOCEmail', 'OldDealershipPOCEmail', 'COLUMN';
EXEC sp_rename 'eCollateralItems.DealershipPOCPhone', 'OldDealershipPOCPhone', 'COLUMN';
EXEC sp_rename 'eCollateralItems.DealershipPOCAcctType', 'OldDealershipPOCAcctType', 'COLUMN';
go