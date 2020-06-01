USE [master]
GO
/****** Object:  Database [Club]    Script Date: 2020/01/08 11:01:07 ******/
CREATE DATABASE [Club]
GO
ALTER DATABASE [Club] SET COMPATIBILITY_LEVEL = 130
GO
ALTER DATABASE [Club] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Club] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Club] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Club] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Club] SET ARITHABORT OFF 
GO
ALTER DATABASE [Club] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Club] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Club] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Club] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Club] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Club] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Club] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Club] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Club] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Club] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Club] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Club] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Club] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Club] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Club] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Club] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Club] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Club] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Club] SET  MULTI_USER 
GO
ALTER DATABASE [Club] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Club] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Club] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Club] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Club] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [Club] SET QUERY_STORE = OFF
GO

USE [Club]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO

USE [Club]
GO
/****** Object:  Table [dbo].[Club]    Script Date: 2020/01/08 11:01:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Club](
	[ClubID] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
    [Intro] [nvarchar](200) NULL,
	[RunningTime] [datetime] NULL,
	[IgnoreColumn] [nvarchar](50) NULL
) ON [PRIMARY]
GO

USE [Club]
GO

/****** Object:  UserDefinedTableType [dbo].[ClubType]    Script Date: 2020/01/08 11:18:23 ******/
CREATE TYPE [dbo].[ClubType] AS TABLE(
	[ClubID] [int] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[IsActive] [bit] NULL
	PRIMARY KEY CLUSTERED 
(
	[ClubID] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO
ALTER TABLE [dbo].[Club] ADD  CONSTRAINT [DF_Club_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO

USE [master]
GO
ALTER DATABASE [Club] SET  READ_WRITE 
GO

USE [Club]
GO
INSERT INTO [Club] (ClubID, [Name], IsActive, Intro, RunningTime, IgnoreColumn)
VALUES
(9, N'�d���f', CONVERT(bit, 'True'), N'�d', NULL, NULL),
(10, N'�f�οP', CONVERT(bit, 'True'), N'�f', NULL, NULL),
(11, N'�^�e�k', CONVERT(bit, 'False'), N'�^', NULL, NULL),
(12, N'���G��', CONVERT(bit, 'False'), N'��', NULL, NULL),
(15, N'�ڶ���޳', CONVERT(bit, 'False'), NULL, NULL, NULL),
(16, N'ù�ɧg', CONVERT(bit, 'False'), NULL, NULL, NULL),
(17, N'�d�Q�S', CONVERT(bit, 'True'), NULL, NULL, NULL),
(18, N'���R��', CONVERT(bit, 'False'), N'��', NULL, NULL),
(19, N'�����Q', CONVERT(bit, 'False'), N'��', NULL, NULL),
(20, N'�i�Φm', CONVERT(bit, 'False'), N'�i', NULL, NULL),
(21, N'���{�g', CONVERT(bit, 'True'), N'��', NULL, NULL),
(23, N'���Ͷv', CONVERT(bit, 'False'), N'��', NULL, NULL),
(24, N'�c����', CONVERT(bit, 'False'), NULL, NULL, NULL),
(25, N'�H����', CONVERT(bit, 'True'), NULL, NULL, NULL),
(26, N'����ʹ', CONVERT(bit, 'True'), N'��', NULL, NULL),
(27, N'�x����', CONVERT(bit, 'False'), N'�x', NULL, NULL),
(28, N'�G�K��', CONVERT(bit, 'True'), N'�G', NULL, NULL),
(29, N'�P�Y�u', CONVERT(bit, 'False'), N'�P', NULL, NULL),
(30, N'�L�ɱd', CONVERT(bit, 'False'), N'�L', NULL, NULL),
(31, N'���T��', CONVERT(bit, 'True'), N'��', NULL, NULL),
(32, N'�P����', CONVERT(bit, 'False'), N'�P', NULL, NULL),
(33, N'�\�a�q', CONVERT(bit, 'True'), N'�\', NULL, NULL),
(34, N'���h��', CONVERT(bit, 'True'), N'��', NULL, NULL),
(35, N'���R�y', CONVERT(bit, 'True'), N'��', NULL, N'Ignored'),
(36, N'�s����', CONVERT(bit, 'True'), N'�s', NULL, NULL),
(37, N'�B�a��', CONVERT(bit, 'True'), N'�B', NULL, NULL),
(38, N'�¬��', CONVERT(bit, 'True'), N'��', NULL, NULL),
(39, N'���u��', CONVERT(bit, 'True'), N'��', '2020-01-01 00:00:00.000', NULL)
GO

/****** Object:  Database [Advertisement]    Script Date: 2020/05/18 11:06:57 ******/
CREATE DATABASE [Advertisement]
GO

ALTER DATABASE [Advertisement] SET COMPATIBILITY_LEVEL = 130
GO

ALTER DATABASE [Advertisement] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [Advertisement] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [Advertisement] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [Advertisement] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [Advertisement] SET ARITHABORT OFF 
GO

ALTER DATABASE [Advertisement] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [Advertisement] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [Advertisement] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [Advertisement] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [Advertisement] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [Advertisement] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [Advertisement] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [Advertisement] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [Advertisement] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [Advertisement] SET  DISABLE_BROKER 
GO

ALTER DATABASE [Advertisement] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [Advertisement] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [Advertisement] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [Advertisement] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [Advertisement] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [Advertisement] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [Advertisement] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [Advertisement] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [Advertisement] SET  MULTI_USER 
GO

ALTER DATABASE [Advertisement] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [Advertisement] SET DB_CHAINING OFF 
GO

ALTER DATABASE [Advertisement] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [Advertisement] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [Advertisement] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [Advertisement] SET QUERY_STORE = OFF
GO

USE [Advertisement]
GO

ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO

ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO

ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO

ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO

ALTER DATABASE [Advertisement] SET  READ_WRITE 
GO

USE [Advertisement]
GO

/****** Object:  Table [dbo].[AdvertisementSetting]    Script Date: 2020/05/18 11:08:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AdvertisementSetting](
	[id] [uniqueidentifier] NOT NULL,
	[typeId] [int] NULL,
	[type] [nvarchar](256) NULL,
	[titleId] [int] NULL,
	[title] [nvarchar](512) NULL,
	[imageName] [nvarchar](256) NULL,
	[imageId] [int] NULL,
	[image] [nvarchar](max) NULL,
	[linkId] [int] NULL,
	[linkName] [nvarchar](256) NULL,
	[link] [nvarchar](max) NULL,
	[weightId] [int] NULL,
	[weight] [int] NULL,
	[state] [int] NULL,
	[stateName] [nvarchar](256) NULL,
	[scheduleId] [int] NULL,
	[scheduleName] [nvarchar](256) NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[description] [nvarchar](256) NULL,
	[CreateTime] [datetime] NULL,
	[platformId] [int] NULL,
	[platformName] [nvarchar](50) NULL,
	[LastUpdateTime] [datetime] NULL,
	[AdCode] [nvarchar](max) NULL,
	[OwnerId] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

INSERT INTO [AdvertisementSetting] (id, typeId, type, titleId, title, imageName, imageId, image, linkId, linkName, link, weightId, weight, state, stateName, scheduleId, scheduleName, StartTime, EndTime, description, CreateTime, platformId, platformName, LastUpdateTime, AdCode, OwnerId) VALUES
('df31efe5-b78f-4b4b-954a-0078328e34d2', 6, N'1000x90�����U', 54, N'�~���s�i', N'�~���s�i', 92, N' ', 41, N'�~���s�i', N' ', 10, 100, 4, N'�~���s�i', 0, NULL, NULL, NULL, N'', CONVERT(DATETIME, '2016-02-15 12:10:38.247', 121), 1, N'PC', CONVERT(DATETIME, '2016-02-15 12:10:38.247', 121), NULL, 1)
GO

/****** Object:  Database [Member]    Script Date: 2020/05/20 09:37:33 ******/
CREATE DATABASE [Member]
GO

ALTER DATABASE [Member] SET COMPATIBILITY_LEVEL = 130
GO

ALTER DATABASE [Member] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [Member] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [Member] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [Member] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [Member] SET ARITHABORT OFF 
GO

ALTER DATABASE [Member] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [Member] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [Member] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [Member] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [Member] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [Member] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [Member] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [Member] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [Member] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [Member] SET  DISABLE_BROKER 
GO

ALTER DATABASE [Member] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [Member] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [Member] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [Member] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [Member] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [Member] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [Member] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [Member] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [Member] SET  MULTI_USER 
GO

ALTER DATABASE [Member] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [Member] SET DB_CHAINING OFF 
GO

ALTER DATABASE [Member] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [Member] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [Member] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [Member] SET QUERY_STORE = OFF
GO

USE [Member]
GO

ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO

ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO

ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO

ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO

ALTER DATABASE [Member] SET  READ_WRITE 
GO

USE [Member]
GO

/****** Object:  Table [dbo].[Member]    Script Date: 2020/05/28 18:26:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Member](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Age] [int] NOT NULL,
	[Phone] [varchar](50) NULL,
	[Address] [nvarchar](200) NULL,
	[DepartmentId] [int] NOT NULL,
	[ManagerId] [int] NOT NULL
 CONSTRAINT [PK_Member] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [Member] (Id, Name, Age, Phone, Address, DepartmentId, ManagerId) VALUES
(1, N'Johnny', 18, NULL, NULL, 3, 2),
(2, N'Amy', 17, '0912345678', NULL, 2, 1),
(3, N'ThreeM', 55, NULL, N'aaabbbcccTEST', 1, 1),
(4, N'Flosser', 37, '0987654321', N'XXXYYYZZZtest', -1, 1)
GO

USE [Member]
GO

/****** Object:  Table [dbo].[Department]    Script Date: 2020/05/28 18:27:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Department](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](20) NULL,
 CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


INSERT INTO [Department] (Id, Name) VALUES
(1, N'��P��'),
(2, N'�~�ȳ�'),
(3, N'���ƪ���')
GO
