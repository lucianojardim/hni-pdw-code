alter table [SpecRequests] add 
	[NeedDWGFiles] [bit] not null constraint [defSRDWG] default(0),
	[NeedSP4Files] [bit] not null constraint [defSRSP4] default(0),
	[NeedSIFFiles] [bit] not null constraint [defSRSIF] default(0),
	[NeedXLSFiles] [bit] not null constraint [defSRXLS] default(0),
	[NeedPDFFiles] [bit] not null constraint [defSRPDF] default(0)
go

alter table [SpecRequests] drop constraint [defSRDWG]
alter table [SpecRequests] drop constraint [defSRSP4]
alter table [SpecRequests] drop constraint [defSRSIF]
alter table [SpecRequests] drop constraint [defSRXLS]
alter table [SpecRequests] drop constraint [defSRPDF]
go