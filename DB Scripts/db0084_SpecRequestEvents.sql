create table [SpecRequestEvents] (
	[EventID] [int] identity(1,1) not null,
	[RequestID] [int] not null CONSTRAINT [FK_Events_SpecRequests] FOREIGN KEY REFERENCES [dbo].[SpecRequests] ([RequestID]),
	[UserID] [int] not null CONSTRAINT [FK_Events_Users] FOREIGN KEY REFERENCES [dbo].[Users] ([UserID]),
	[EventDate] [datetime] not null,
	[EventType] [int] not null,
	
	CONSTRAINT [pkSpecRequestEvents] PRIMARY KEY CLUSTERED ([EventID])	
) on [PRIMARY]
go

insert into [SpecRequestEvents] (RequestID, UserID, EventDate, EventType)
	(select RequestID, CreatedByUserID, RequestDate, 1 from [SpecRequests] where CreatedByUserId is not null)
insert into [SpecRequestEvents] (RequestID, UserID, EventDate, EventType)
	(select RequestID, CanceledByUserID, CanceledDateTime, 2 from [SpecRequests] where IsCanceled = 1)
insert into [SpecRequestEvents] (RequestID, UserID, EventDate, EventType)
	(select RequestID, CompletedByUserID, CompletedDateTime, 3 from [SpecRequests] where IsCompleted = 1 and CompletedByUserID is not null)
go

alter table [SpecRequests] drop constraint [fkSRequest_CreatedBy]
alter table [SpecRequests] drop constraint [fkSpecRequest_CanceledUser]
alter table [SpecRequests] drop constraint [fkSpecRequest_CompletedUser]
alter table [SpecRequests] drop column [CreatedByUserID]
alter table [SpecRequests] drop column [RequestDate]
alter table [SpecRequests] drop column [CanceledByUserID]
alter table [SpecRequests] drop column [CanceledDateTime]
alter table [SpecRequests] drop column [CompletedByUserID]
alter table [SpecRequests] drop column [CompletedDateTime]
go