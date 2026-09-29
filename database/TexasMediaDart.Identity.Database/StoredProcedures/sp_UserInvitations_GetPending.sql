CREATE PROCEDURE [dbo].[sp_UserInvitations_GetPending]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    IF @OrganizationId IS NULL
    BEGIN
        THROW 51035, 'Organization id is required.', 1;
    END;

    SELECT
        [InvitationId],
        [Email],
        [OrganizationId],
        [InvitedByIdentityUserId],
        [ExpiresUtc],
        [CreatedUtc]
    FROM [dbo].[UserInvitations]
    WHERE [OrganizationId] = @OrganizationId
      AND [AcceptedUtc] IS NULL
      AND [RevokedUtc] IS NULL
      AND [ExpiresUtc] > SYSUTCDATETIME()
    ORDER BY [CreatedUtc] DESC;
END;