if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[main]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[main]
GO

CREATE TABLE [dbo].[main] (
	[username] [char] (10) COLLATE Chinese_PRC_CI_AS NOT NULL ,
	[userpwd] [char] (10) COLLATE Chinese_PRC_CI_AS NOT NULL 
) ON [PRIMARY]
GO

