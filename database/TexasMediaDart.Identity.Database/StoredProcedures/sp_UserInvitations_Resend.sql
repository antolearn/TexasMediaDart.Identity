CREATE PROCEDURE [dbo].[sp_UserInvitations_Resend]
    @InvitationId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @TokenHash CHAR(64),
    @ExpiresUtc DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @InvitationId IS NULL
    BEGIN
        THROW 51028, 'Invitation id is required.', 1;
    END;

    IF @OrganizationId IS NULL
    BEGIN
        THROW 51034, 'Organization id is required.', 1;
    END;

    IF NULLIF(LTRIM(RTRIM(@TokenHash)), '') IS NULL
    BEGIN
        THROW 51029, 'Token hash is required.', 1;
    END;

    IF @ExpiresUtc <= SYSUTCDATETIME()
    BEGIN
        THROW 51030, 'Invitation expiration must be in the future.', 1;
    END;

    -- Treat an invitation belonging to another organization
    -- the same as an invitation that does not exist.
    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[UserInvitations]
        WHERE [InvitationId] = @InvitationId
          AND [OrganizationId] = @OrganizationId
    )
    BEGIN
        THROW 51031, 'Invitation was not found.', 1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[UserInvitations]
        WHERE [InvitationId] = @InvitationId
          AND [OrganizationId] = @OrganizationId
          AND [AcceptedUtc] IS NOT NULL
    )
    BEGIN
        THROW 51032, 'Accepted invitations cannot be resent.', 1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[UserInvitations]
        WHERE [InvitationId] = @InvitationId
          AND [OrganizationId] = @OrganizationId
          AND [RevokedUtc] IS NOT NULL
    )
    BEGIN
        THROW 51033, 'Revoked invitations cannot be resent.', 1;
    END;

    UPDATE [dbo].[UserInvitations]
    SET
        [TokenHash] = @TokenHash,
        [ExpiresUtc] = @ExpiresUtc
    WHERE [InvitationId] = @InvitationId
      AND [OrganizationId] = @OrganizationId;

    SELECT
        [InvitationId],
        [Email],
        [OrganizationId],
        [InvitedByIdentityUserId],
        [TokenHash],
        [ExpiresUtc],
        [CreatedIdentityUserId],
        [IdentityCreatedUtc],
        [AcceptedUtc],
        [RevokedUtc],
        [CreatedUtc]
    FROM [dbo].[UserInvitations]
    WHERE [InvitationId] = @InvitationId
      AND [OrganizationId] = @OrganizationId;
END;