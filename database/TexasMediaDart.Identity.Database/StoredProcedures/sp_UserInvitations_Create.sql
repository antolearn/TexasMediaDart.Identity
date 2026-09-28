CREATE PROCEDURE [dbo].[sp_UserInvitations_Create]
    @Email                    NVARCHAR(320),
    @OrganizationId           UNIQUEIDENTIFIER,
    @InvitedByIdentityUserId  UNIQUEIDENTIFIER,
    @TokenHash                CHAR(64),
    @ExpiresUtc               DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Email = LOWER(LTRIM(RTRIM(@Email)));
    SET @TokenHash = LOWER(LTRIM(RTRIM(@TokenHash)));

    IF NULLIF(@Email, '') IS NULL
    BEGIN
        THROW 51001, 'Email is required.', 1;
    END;

    IF @OrganizationId IS NULL
    BEGIN
        THROW 51002, 'Organization id is required.', 1;
    END;

    IF @InvitedByIdentityUserId IS NULL
    BEGIN
        THROW 51003, 'Invited by identity user id is required.', 1;
    END;

    IF NULLIF(@TokenHash, '') IS NULL
    BEGIN
        THROW 51004, 'Token hash is required.', 1;
    END;

    IF LEN(@TokenHash) <> 64
    BEGIN
        THROW 51005, 'Token hash must be 64 characters.', 1;
    END;

    IF @ExpiresUtc <= SYSUTCDATETIME()
    BEGIN
        THROW 51006, 'Invitation expiration must be in the future.', 1;
    END;

    IF NOT EXISTS
    (
        SELECT 1
        FROM [dbo].[Users]
        WHERE [UserId] = @InvitedByIdentityUserId
          AND [IsActive] = 1
          AND [IsDeleted] = 0
    )
    BEGIN
        THROW 51007, 'Inviting identity user was not found or is inactive.', 1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[Users]
        WHERE [Email] = @Email
          AND [IsDeleted] = 0
    )
    BEGIN
        THROW 51008, 'A user with this email already exists.', 1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[UserInvitations]
        WHERE [Email] = @Email
          AND [OrganizationId] = @OrganizationId
          AND [AcceptedUtc] IS NULL
          AND [RevokedUtc] IS NULL
          AND [ExpiresUtc] > SYSUTCDATETIME()
    )
    BEGIN
        THROW 51009, 'A pending invitation already exists for this email and organization.', 1;
    END;

    INSERT INTO [dbo].[UserInvitations]
    (
        [Email],
        [OrganizationId],
        [InvitedByIdentityUserId],
        [TokenHash],
        [ExpiresUtc]
    )
    OUTPUT
        INSERTED.[InvitationId],
        INSERTED.[Email],
        INSERTED.[OrganizationId],
        INSERTED.[InvitedByIdentityUserId],
        INSERTED.[TokenHash],
        INSERTED.[ExpiresUtc],
        INSERTED.[AcceptedUtc],
        INSERTED.[RevokedUtc],
        INSERTED.[CreatedUtc]
    VALUES
    (
        @Email,
        @OrganizationId,
        @InvitedByIdentityUserId,
        @TokenHash,
        @ExpiresUtc
    );
END;
GO