CREATE PROCEDURE [dbo].[sp_Users_GetByIds]
    @UserIds [dbo].[GuidList] READONLY
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        U.[UserId],
        U.[Email],
        U.[PasswordHash],
        U.[IsActive],
        U.[IsDeleted],
        U.[IsEmailVerified],
        U.[CreatedDateUtc],
        U.[ModifiedDateUtc]
    FROM [dbo].[Users] U
    INNER JOIN @UserIds I
        ON I.[Id] = U.[UserId]
    ORDER BY U.[Email];
END;
GO