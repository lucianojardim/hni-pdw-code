create table [LeadTimeDetail] (
	[LeadTimeID] [int] identity(1,1) not null,
	[Headline] [ntext] null,
	[PromoText] [ntext] null,
	[UserID] [int] null CONSTRAINT [FK_LeadTimeDetail_User] FOREIGN KEY REFERENCES [Users] ([UserID]),
	[LeftSide] [ntext] null,
	[RightSide] [ntext] null,
	
	CONSTRAINT [pkLeadTimeDetail] PRIMARY KEY CLUSTERED ([LeadTimeID])
) on [primary]
go

declare @stanKing int
select @stanKing = userid from Users where LastName = 'king' and FirstName = 'stan'

insert into LeadTimeDetail (Headline, PromoText, UserID, LeftSide, RightSide) values 
	(N'Have a lead time issue?', 
	N'Contact your customer service rep for day to day business. Have a major opportunity or unique circumstance?', 
	@stanKing, 
	N'         	<table border="0">
			  <tr>
			    <th width="75%">Casegoods</th>
			    <th>&nbsp;</th>
			  </tr>
			  <tr>
			    <td width="75%">Series 1</td>
			    <td>Weeks ##1</td>
			  </tr>
			  <tr>
			    <td width="75%">Series 2</td>
			    <td>Weeks ##2</td>
			  </tr>
			</table>
            
           <table border="0">
			  <tr>
			    <th width="75%">Casegoods</th>
			    <th>&nbsp;</th>
			  </tr>
			  <tr>
			    <td width="75%">Series 1</td>
			    <td>Weeks ##1</td>
			  </tr>
			  <tr>
			    <td width="75%">Series 2</td>
			    <td>Weeks ##2</td>
			  </tr>
			</table>
            
           <table border="0">
			  <tr>
			    <th width="75%">Casegoods</th>
			    <th>&nbsp;</th>
			  </tr>
			  <tr>
			    <td width="75%">Series 1</td>
			    <td>Weeks ##1</td>
			  </tr>
			  <tr>
			    <td width="75%">Series 2</td>
			    <td>Weeks ##2</td>
			  </tr>
			</table>', 
	N'         	<table border="0">
			  <tr>
			    <th width="70%">in2 Eligible</th>
			    <th>&nbsp;</th>
			  </tr>
			  <tr>
			    <td width="70%">Series 1</td>
			    <td>Weeks ##1</td>
			  </tr>
			  <tr>
			    <td width="70%">Series 2</td>
			    <td>Weeks ##2</td>
			  </tr>
			</table>
            
           <table border="0">
			  <tr>
			    <th width="70%">in2 Eligible</th>
			    <th>&nbsp;</th>
			  </tr>
			  <tr>
			    <td width="70%">Series 1</td>
			    <td>Weeks ##1</td>
			  </tr>
			  <tr>
			    <td width="70%">Series 2</td>
			    <td>Weeks ##2</td>
			  </tr>
			</table>
           
           <table border="0">
			  <tr>
			    <th width="70%">in2 Eligible</th>
			    <th>&nbsp;</th>
			  </tr>
			  <tr>
			    <td width="70%">Series 1</td>
			    <td>Weeks ##1</td>
			  </tr>
			  <tr>
			    <td width="70%">Series 2</td>
			    <td>Weeks ##2</td>
			  </tr>
			</table>')
go