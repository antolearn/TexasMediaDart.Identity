CREATE PROCEDURE [dbo].[sp_Users_GetById]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

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
    WHERE [UserId] = @UserId;
END;
GO