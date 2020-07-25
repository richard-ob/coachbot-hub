/****** Object:  UserDefinedFunction [dbo].[GenerateDateRange]    Script Date: 25/07/2020 17:27:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GenerateDateRange]
(@StartDate AS DATE,
 @EndDate AS   DATE,
 @Interval AS  INT
)
RETURNS @Dates TABLE(DateValue DATE)
AS
BEGIN
    DECLARE @CUR_DATE DATE
    SET @CUR_DATE = @StartDate
    WHILE @CUR_DATE <= @EndDate BEGIN
        INSERT INTO @Dates VALUES(@CUR_DATE)
        SET @CUR_DATE = DATEADD(DAY, @Interval, @CUR_DATE)
    END
    RETURN;
END;
GO

