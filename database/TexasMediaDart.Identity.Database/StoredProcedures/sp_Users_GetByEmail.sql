CREATE PROCEDURE [dbo].[sp_Users_GetByEmail]
    @Email NVARCHAR(320)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Email = LOWER(LTRIM(RTRIM(@Email)));

    SELECT
        [UserId],
        [Email],
        [PasswordHash],
        [IsActive],
        [IsDeleted],
        [IsEmailVerified],
        [CreatedDateUtc],
        [ModifiedDateUtc]
    FROM [dbo].[Users]
    WHERE [Email] = @Email;
END;