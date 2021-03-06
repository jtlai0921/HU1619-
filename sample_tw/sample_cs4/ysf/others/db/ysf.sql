if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[footorder]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[footorder]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[productdetail]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[productdetail]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[promotion]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[promotion]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[seatorder]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[seatorder]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[userlist]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[userlist]
GO

CREATE TABLE [dbo].[footorder] (
	[userName] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[foodName] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[foodNum] [int] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[productdetail] (
	[productId] [int] NOT NULL ,
	[productNum] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[productKind] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[productName] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[productMaterialList] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[productSubMaterialList] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[productFunction] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[productMake] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[productMakePath] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[productPic] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[productPrice] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[productPicPath] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[promotion] (
	[promotionId] [char] (10) COLLATE Chinese_PRC_CI_AS NOT NULL ,
	[productName] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NOT NULL ,
	[promotionDate] [tinyint] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[seatorder] (
	[id] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[fReserve] [smalldatetime] NULL ,
	[tReserve] [smalldatetime] NULL ,
	[reserved] [bit] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[userlist] (
	[userName] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[userSex] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[userBirth] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[userEMail] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[userTel] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[userProvince] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[userZIP] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[userAdd] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[userID] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[roles] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL ,
	[userPass] [nvarchar] (50) COLLATE Chinese_PRC_CI_AS NULL 
) ON [PRIMARY]
GO

