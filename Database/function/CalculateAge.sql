CREATE FUNCTION dbo.CalculateAge
(
    @DateOfBirth DATE
)
RETURNS INT
AS
BEGIN
    DECLARE @Today DATE = GETDATE();
    DECLARE @Age INT;

    -- Calculate age
    SET @Age = DATEDIFF(YEAR, @DateOfBirth, @Today) - 
               CASE 
                   WHEN MONTH(@Today) < MONTH(@DateOfBirth) OR 
                        (MONTH(@Today) = MONTH(@DateOfBirth) AND DAY(@Today) < DAY(@DateOfBirth))
                   THEN 1 
                   ELSE 0 
               END;

    RETURN @Age;
END;
