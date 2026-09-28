CREATE PROCEDURE [dbo].[sp_UserInvitations_AcceptIdentity]
    @TokenHash CHAR(64),
    @PasswordHash NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @TokenHash = LOWER(LTRIM(RTRIM(@TokenHash)));

    IF NULLIF(@TokenHash, '') IS NULL
    BEGIN
        THROW 51014, 'Token hash is required.', 1;
    END;

    IF LEN(@TokenHash) <> 64
    BEGIN
        THROW 51015, 'Token hash must be 64 characters.', 1;
    END;

    IF NULLIF(@PasswordHash, '') IS NULL
    BEGIN
        THROW 51016, 'Password hash is required.', 1;
    END;

    DECLARE
        @InvitationId UNIQUEIDENTIFIER,
        @Email NVARCHAR(320),
        @OrganizationId UNIQUEIDENTIFIER,
        @ExpiresUtc DATETIME2(7),
        @CreatedIdentityUserId UNIQUEIDENTIFIER,
        @IdentityCreatedUtc DATETIME2(7),
        @AcceptedUtc DATETIME2(7),
        @RevokedUtc DATETIME2(7);

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT
            @InvitationId = [InvitationId],
            @Email = [Email],
            @OrganizationId = [OrganizationId],
            @ExpiresUtc = [ExpiresUtc],
            @CreatedIdentityUserId = [CreatedIdentityUserId],
            @IdentityCreatedUtc = [IdentityCreatedUtc],
            @AcceptedUtc = [AcceptedUtc],
            @RevokedUtc = [RevokedUtc]
        FROM [dbo].[UserInvitations] WITH (UPDLOCK, HOLDLOCK)
        WHERE [TokenHash] = @TokenHash;

        IF @InvitationId IS NULL
        BEGIN
            THROW 51017, 'Invitation was not found.', 1;
        END;

        IF @RevokedUtc IS NOT NULL
        BEGIN
            THROW 51018, 'Invitation has been revoked.', 1;
        END;

        IF @AcceptedUtc IS NOT NULL
        BEGIN
            THROW 51019, 'Invitation has already been accepted.', 1;
        END;

        /*
            If the Identity account was already created during a
            previous attempt, return the existing account instead
            of creating another user.

            This makes the Identity portion of invitation acceptance
            idempotent and allows the Main API to retry Organization
            onboarding safely.
        */
        IF @CreatedIdentityUserId IS NOT NULL
        BEGIN
            COMMIT TRANSACTION;

            SELECT
                [InvitationId],
                [Email],
                [OrganizationId],
                [CreatedIdentityUserId],
                [IdentityCreatedUtc],
                [ExpiresUtc]
            FROM [dbo].[UserInvitations]
            WHERE [InvitationId] = @InvitationId;

            RETURN;
        END;

        IF @ExpiresUtc <= SYSUTCDATETIME()
        BEGIN
            THROW 51020, 'Invitation has expired.', 1;
        END;

        /*
            The invitation was originally created only when the
            email did not exist. Recheck while holding the invitation
            lock in case an account was created independently after
            the invitation was issued.
        */
        IF EXISTS
        (
            SELECT 1
            FROM [dbo].[Users]
            WHERE [Email] = @Email
        )
        BEGIN
            THROW 51021, 'A user with this email already exists.', 1;
        END;

        DECLARE @NewUser TABLE
        (
            [UserId] UNIQUEIDENTIFIER NOT NULL
        );

        INSERT INTO [dbo].[Users]
        (
            [Email],
            [PasswordHash]
        )
        OUTPUT
            INSERTED.[UserId]
        INTO @NewUser ([UserId])
        VALUES
        (
            @Email,
            @PasswordHash
        );

        SELECT
            @CreatedIdentityUserId = [UserId]
        FROM @NewUser;

        SET @IdentityCreatedUtc = SYSUTCDATETIME();

        UPDATE [dbo].[UserInvitations]
        SET
            [CreatedIdentityUserId] = @CreatedIdentityUserId,
            [IdentityCreatedUtc] = @IdentityCreatedUtc
        WHERE [InvitationId] = @InvitationId;

        COMMIT TRANSACTION;

        SELECT
            [InvitationId],
            [Email],
            [OrganizationId],
            [CreatedIdentityUserId],
            [IdentityCreatedUtc],
            [ExpiresUtc]
        FROM [dbo].[UserInvitations]
        WHERE [InvitationId] = @InvitationId;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;
    END CATCH;
END;