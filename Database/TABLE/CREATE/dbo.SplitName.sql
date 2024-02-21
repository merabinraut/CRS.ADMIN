CREATE FUNCTION dbo.SplitName
(
    @FullName NVARCHAR(MAX)
)
RETURNS @NameParts TABLE 
(
    FirstName NVARCHAR(MAX),
    LastName NVARCHAR(MAX)
)
AS
BEGIN
    DECLARE @FirstSpaceIndex INT;
    DECLARE @FirstName NVARCHAR(MAX);
    DECLARE @LastName NVARCHAR(MAX);

    -- Find the first space in the full name
    SET @FirstSpaceIndex = CHARINDEX(' ', @FullName);

    -- If there's no space, set the entire name as the first name
    IF @FirstSpaceIndex = 0
    BEGIN
        SET @FirstName = @FullName;
        SET @LastName = NULL;
    END
    ELSE
    BEGIN
        -- Extract the first name and last name
        SET @FirstName = LEFT(@FullName, @FirstSpaceIndex - 1);
        SET @LastName = SUBSTRING(@FullName, @FirstSpaceIndex + 1, LEN(@FullName) - @FirstSpaceIndex);
    END;

    -- Insert the first name and last name into the result table
    INSERT INTO @NameParts (FirstName, LastName) VALUES (@FirstName, @LastName);

    RETURN;
END
