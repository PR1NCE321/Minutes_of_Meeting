USE [MOM_DB]
GO

/****** Object:  StoredProcedure [dbo].[PR_MOM_MeetingType_Search] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[PR_MOM_MeetingType_Search]
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SELECT 
        [MeetingTypeID],
        [MeetingTypeName],
        [Remarks],
        [Created],
        [Modified]
    FROM [dbo].[MOM_MeetingType]
    WHERE 
        (@SearchText IS NULL OR [MeetingTypeName] LIKE '%' + @SearchText + '%')
    ORDER BY [MeetingTypeName]
END
GO

/****** Object:  StoredProcedure [dbo].[PR_MOM_Department_Search] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[PR_MOM_Department_Search]
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SELECT 
        [DepartmentID],
        [DepartmentName],
        [Created],
        [Modified]
    FROM [dbo].[MOM_Department]
    WHERE 
        (@SearchText IS NULL OR [DepartmentName] LIKE '%' + @SearchText + '%')
    ORDER BY [DepartmentName]
END
GO

/****** Object:  StoredProcedure [dbo].[PR_MOM_MeetingVenue_Search] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[PR_MOM_MeetingVenue_Search]
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SELECT 
        [MeetingVenueID],
        [MeetingVenueName],
        [Created],
        [Modified]
    FROM [dbo].[MOM_MeetingVenue]
    WHERE 
        (@SearchText IS NULL OR [MeetingVenueName] LIKE '%' + @SearchText + '%')
    ORDER BY [MeetingVenueName]
END
GO

/****** Object:  StoredProcedure [dbo].[PR_MOM_Staff_Search] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[PR_MOM_Staff_Search]
    @SearchText VARCHAR(100) = NULL
AS
BEGIN
    SELECT 
        S.[StaffID],
        S.[DepartmentID],
        S.[StaffName],
        S.[MobileNo],
        S.[EmailAddress],
        S.[Remarks],
        S.[Created],
        S.[Modified]
    FROM [dbo].[MOM_Staff] S
    LEFT JOIN [dbo].[MOM_Department] D ON S.[DepartmentID] = D.[DepartmentID]
    WHERE 
        (@SearchText IS NULL OR 
         S.[StaffName] LIKE '%' + @SearchText + '%' OR
         S.[EmailAddress] LIKE '%' + @SearchText + '%' OR
         S.[MobileNo] LIKE '%' + @SearchText + '%' OR
         D.[DepartmentName] LIKE '%' + @SearchText + '%')
    ORDER BY S.[StaffName]
END
GO
