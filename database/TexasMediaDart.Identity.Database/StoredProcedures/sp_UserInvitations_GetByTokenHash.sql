CREATE PROCEDURE [dbo].[sp_UserInvitations_GetByTokenHash]
    @TokenHash CHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    SET @TokenHash = LOWER(LTRIM(RTRIM(@TokenHash)));

    IF NULLIF(@TokenHash, '') IS NULL
    BEGIN
        THROW 51010, 'Token hash is required.', 1;
    END;

    IF LEN(@TokenHash) <> 64
    BEGIN
        THROW 51011, 'Token hash must be 64 characters.', 1;
    END;

    SELECT
        [InvitationId],
        [Email],
        [OrganizationId],
        [InvitedByIdentityUserId],
        [TokenHash],
        [ExpiresUtc],
        [AcceptedUtc],
        [RevokedUtc],
        [CreatedUtc]
    FROM [dbo].[UserInvitations]
    WHERE [TokenHash] = @TokenHash;
END;
GO