CREATE PROCEDURE [dbo].[sp_UserInvitations_Finalize]
    @InvitationId UNIQUEIDENTIFIER,
    @IdentityUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @InvitationId IS NULL
    BEGIN
        THROW 51022, 'Invitation id is required.', 1;
    END;

    IF @IdentityUserId IS NULL
    BEGIN
        THROW 51023, 'Identity user id is required.', 1;
    END;

    DECLARE
        @CreatedIdentityUserId UNIQUEIDENTIFIER,
        @AcceptedUtc DATETIME2(7),
        @RevokedUtc DATETIME2(7);

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT
            @CreatedIdentityUserId = [CreatedIdentityUserId],
            @AcceptedUtc = [AcceptedUtc],
            @RevokedUtc = [RevokedUtc]
        FROM [dbo].[UserInvitations] WITH (UPDLOCK, HOLDLOCK)
        WHERE [InvitationId] = @InvitationId;

        IF @CreatedIdentityUserId IS NULL
        BEGIN
            IF NOT EXISTS
            (
                SELECT 1
                FROM [dbo].[UserInvitations]
                WHERE [InvitationId] = @InvitationId
            )
            BEGIN
                THROW 51024, 'Invitation was not found.', 1;
            END;

            THROW 51025,
                'Identity account has not been created for this invitation.',
                1;
        END;

        IF @CreatedIdentityUserId <> @IdentityUserId
        BEGIN
            THROW 51026,
                'Identity user does not match the invitation.',
                1;
        END;

        IF @RevokedUtc IS NOT NULL
        BEGIN
            THROW 51027, 'Invitation has been revoked.', 1;
        END;

        /*
            Idempotent retry.

            If finalization already succeeded, return the existing
            invitation state without changing AcceptedUtc.
        */
        IF @AcceptedUtc IS NOT NULL
        BEGIN
            COMMIT TRANSACTION;

            SELECT
                [InvitationId],
                [Email],
                [OrganizationId],
                [CreatedIdentityUserId],
                [IdentityCreatedUtc],
                [AcceptedUtc]
            FROM [dbo].[UserInvitations]
            WHERE [InvitationId] = @InvitationId;

            RETURN;
        END;

        SET @AcceptedUtc = SYSUTCDATETIME();

        UPDATE [dbo].[UserInvitations]
        SET [AcceptedUtc] = @AcceptedUtc
        WHERE [InvitationId] = @InvitationId;

        COMMIT TRANSACTION;

        SELECT
            [InvitationId],
            [Email],
            [OrganizationId],
            [CreatedIdentityUserId],
            [IdentityCreatedUtc],
            [AcceptedUtc]
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