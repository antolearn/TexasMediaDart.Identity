CREATE PROCEDURE [dbo].[sp_EmailVerificationToken_Create]
    @UserId UNIQUEIDENTIFIER,
    @TokenHash CHAR(64),
    @ExpiresUtc DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @UtcNow DATETIME2(7) = SYSUTCDATETIME();

    ------------------------------------------------------------
    -- Validate token input
    ------------------------------------------------------------
    IF NULLIF(LTRIM(RTRIM(@TokenHash)), '') IS NULL
    BEGIN
        THROW 50010,
            'Email verification token hash is required.',
            1;
    END;

    IF LEN(@TokenHash) <> 64
    BEGIN
        THROW 50011,
            'Email verification token hash must contain 64 characters.',
            1;
    END;

    IF @ExpiresUtc <= @UtcNow
    BEGIN
        THROW 50012,
            'Email verification token expiration must be in the future.',
            1;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        ------------------------------------------------------------
        -- Lock the user row.
        --
        -- This serializes token creation/resend operations for
        -- the same user.
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
            THROW 50013,
                'User does not exist or is not active.',
                1;
        END;

        ------------------------------------------------------------
        -- Do not issue another verification token once the
        -- email address has already been verified.
        ------------------------------------------------------------
        IF EXISTS
        (
            SELECT 1
            FROM [dbo].[Users]
            WHERE [UserId] = @UserId
              AND [IsEmailVerified] = 1
        )
        BEGIN
            THROW 50014,
                'Email address is already verified.',
                1;
        END;

        ------------------------------------------------------------
        -- Recheck expiration after acquiring the lock.
        ------------------------------------------------------------
        SET @UtcNow = SYSUTCDATETIME();

        IF @ExpiresUtc <= @UtcNow
        BEGIN
            THROW 50015,
                'Email verification token expiration must be in the future.',
                1;
        END;

        ------------------------------------------------------------
        -- Invalidate previous active verification token.
        ------------------------------------------------------------
        UPDATE [dbo].[EmailVerificationTokens]
        SET [InvalidatedUtc] = @UtcNow
        WHERE [UserId] = @UserId
          AND [UsedUtc] IS NULL
          AND [InvalidatedUtc] IS NULL;

        ------------------------------------------------------------
        -- Create replacement/current verification token.
        ------------------------------------------------------------
        INSERT INTO [dbo].[EmailVerificationTokens]
        (
            [UserId],
            [TokenHash],
            [ExpiresUtc]
        )
        VALUES
        (
            @UserId,
            @TokenHash,
            @ExpiresUtc
        );

        ------------------------------------------------------------
        -- Return metadata before committing.
        ------------------------------------------------------------
        DECLARE @CreatedToken TABLE
        (
            [EmailVerificationTokenId] UNIQUEIDENTIFIER,
            [UserId] UNIQUEIDENTIFIER,
            [ExpiresUtc] DATETIME2(7),
            [CreatedUtc] DATETIME2(7)
        );

        INSERT INTO @CreatedToken
        SELECT
            [EmailVerificationTokenId],
            [UserId],
            [ExpiresUtc],
            [CreatedUtc]
        FROM [dbo].[EmailVerificationTokens]
        WHERE [TokenHash] = @TokenHash;

        COMMIT TRANSACTION;

        SELECT
            [EmailVerificationTokenId],
            [UserId],
            [ExpiresUtc],
            [CreatedUtc]
        FROM @CreatedToken;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;
    END CATCH;
END;