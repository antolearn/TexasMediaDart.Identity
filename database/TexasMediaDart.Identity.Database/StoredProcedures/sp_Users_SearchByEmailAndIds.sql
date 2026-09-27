CREATE PROCEDURE [dbo].[sp_Users_SearchByEmailAndIds]
    @UserIds [dbo].[GuidList] READONLY,
    @Email NVARCHAR(320) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @Email = NULLIF(
        LTRIM(RTRIM(@Email)),
        N''
    );

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
    WHERE
        U.[IsDeleted] = 0
        AND
        (
            @Email IS NULL
            OR U.[Email] LIKE N'%' + @Email + N'%'
        )
    ORDER BY
        U.[Email],
        U.[UserId];
END;
GO