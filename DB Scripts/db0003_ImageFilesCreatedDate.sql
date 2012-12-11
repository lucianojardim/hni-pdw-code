update ImageFiles set CreatedDate = DATEADD( day, 0-imageid, CreatedDate )
select * from ImageFiles