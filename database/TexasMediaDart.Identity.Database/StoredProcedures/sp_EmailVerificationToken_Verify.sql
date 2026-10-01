CREATE PROCEDURE [dbo].[sp_EmailVerificationToken_Verify]
    @TokenHash CHAR(64)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @UtcNow DATETIME2(7) = SYSUTCDATETIME();
    DECLARE @EmailVerificationTokenId UNIQUEIDENTIFIER;
    DECLARE @UserId UNIQUEIDENTIFIER;
    DECLARE @ExpiresUtc DATETIME2(7);
    DECLARE @UsedUtc DATETIME2(7);
    DECLARE @InvalidatedUtc DATETIME2(7);

    BEGIN TRY
        BEGIN TRANSACTION;

        ------------------------------------------------------------
        -- Locate and lock the verification token.
        --
        -- UPDLOCK prevents two concurrent requests from
        -- successfully consuming the same token.
        ------------------------------------------------------------
        SELECT
            @EmailVerificationTokenId = [EmailVerificationTokenId],
            @UserId = [UserId],
            @ExpiresUtc = [ExpiresUtc],
            @UsedUtc = [UsedUtc],
            @InvalidatedUtc = [InvalidatedUtc]
        FROM [dbo].[EmailVerificationTokens] WITH (UPDLOCK, HOLDLOCK)
        WHERE [TokenHash] = @TokenHash;

        ------------------------------------------------------------
        -- Token does not exist
        ------------------------------------------------------------
        IF @EmailVerificationTokenId IS NULL
        BEGIN
            THROW 50020,
                'Email verification token is invalid.',
                1;
        END;

        ------------------------------------------------------------
        -- Token was already consumed
        ------------------------------------------------------------
        IF @UsedUtc IS NOT NULL
        BEGIN
            THROW 50021,
                'Email verification token has already been used.',
                1;
        END;

        ------------------------------------------------------------
        -- Token was superseded/revoked
        ------------------------------------------------------------
        IF @InvalidatedUtc IS NOT NULL
        BEGIN
            THROW 50022,
                'Email verification token is no longer valid.',
                1;
        END;

        ------------------------------------------------------------
        -- Token expired
        ------------------------------------------------------------
        IF @ExpiresUtc <= @UtcNow
        BEGIN
            THROW 50023,
                'Email verification token has expired.',
                1;
        END;

        ------------------------------------------------------------
        -- Make sure the associated user is still valid
        ------------------------------------------------------------
        IF NOT EXISTS
        (
            SELECT 1
            FROM [dbo].[Users] WITH (UPDLOCK, HOLDLOCK)
            WHERE [UserId] = @UserId
              AND [IsDeleted] = 0
              AND [IsActive] = 1
        )
        BEGIN
            THROW 50024,
                'User does not exist or is not active.',
                1;
        END;

        ------------------------------------------------------------
        -- Verify the user's email
        ------------------------------------------------------------
        UPDATE [dbo].[Users]
        SET
            [IsEmailVerified] = 1,
            [ModifiedDateUtc] = @UtcNow
        WHERE [UserId] = @UserId
          AND [IsDeleted] = 0
          AND [IsActive] = 1;

        ------------------------------------------------------------
        -- Consume this token
        ------------------------------------------------------------
        UPDATE [dbo].[EmailVerificationTokens]
        SET [UsedUtc] = @UtcNow
        WHERE [EmailVerificationTokenId] =
              @EmailVerificationTokenId;

        ------------------------------------------------------------
        -- Invalidate any other outstanding tokens for this user
        ------------------------------------------------------------
        UPDATE [dbo].[EmailVerificationTokens]
        SET [InvalidatedUtc] = @UtcNow
        WHERE [UserId] = @UserId
          AND [EmailVerificationTokenId] <>
              @EmailVerificationTokenId
          AND [UsedUtc] IS NULL
          AND [InvalidatedUtc] IS NULL;

        COMMIT TRANSACTION;

        ------------------------------------------------------------
        -- Return verified user information
        ------------------------------------------------------------
        SELECT
            [UserId],
            [Email],
            [IsEmailVerified]
        FROM [dbo].[Users]
        WHERE [UserId] = @UserId;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;
    END CATCH;
END;