CREATE PROCEDURE [dbo].[sp_UserInvitations_GetPendingByEmail]
    @Email          NVARCHAR(320),
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SET @Email = LOWER(LTRIM(RTRIM(@Email)));

    IF NULLIF(@Email, '') IS NULL
    BEGIN
        THROW 51012, 'Email is required.', 1;
    END;

    IF @OrganizationId IS NULL
    BEGIN
        THROW 51013, 'Organization id is required.', 1;
    END;

    SELECT TOP (1)
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
    WHERE [Email] = @Email
      AND [OrganizationId] = @OrganizationId
      AND [AcceptedUtc] IS NULL
      AND [RevokedUtc] IS NULL
      AND [ExpiresUtc] > SYSUTCDATETIME()
    ORDER BY [CreatedUtc] DESC;
END;
GO