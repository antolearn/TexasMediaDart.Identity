CREATE PROCEDURE [dbo].[sp_Users_CreateWithTermsAcceptance]
    @Email NVARCHAR(320),
    @PasswordHash NVARCHAR(500),
    @VerificationTokenHash CHAR(64),
    @VerificationExpiresUtc DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @UserId UNIQUEIDENTIFIER;
    DECLARE @TermsDocumentId UNIQUEIDENTIFIER;
    DECLARE @TermsVersion NVARCHAR(50);
    DECLARE @TermsDocumentHash NVARCHAR(64);
    DECLARE @RegistrationUtc DATETIME2(7);

    DECLARE @CreatedUser TABLE
    (
        [UserId] UNIQUEIDENTIFIER NOT NULL
    );

    ------------------------------------------------------------
    -- Validate verification token information
    ------------------------------------------------------------
    IF NULLIF(LTRIM(RTRIM(@VerificationTokenHash)), '') IS NULL
    BEGIN
        THROW 50030,
            'Email verification token hash is required.',
            1;
    END;

    IF LEN(@VerificationTokenHash) <> 64
    BEGIN
        THROW 50031,
            'Email verification token hash must contain 64 characters.',
            1;
    END;

    IF @VerificationExpiresUtc <= SYSUTCDATETIME()
    BEGIN
        THROW 50032,
            'Email verification token expiration must be in the future.',
            1;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        ------------------------------------------------------------
        -- Get the current Terms and Conditions
        ------------------------------------------------------------
        SELECT
            @TermsDocumentId = [TermsDocumentId],
            @TermsVersion = [Version],
            @TermsDocumentHash = [DocumentHash]
        FROM [dbo].[TermsDocuments]
        WHERE [IsCurrent] = 1;

        IF @TermsDocumentId IS NULL
        BEGIN
            THROW 50001,
                'No current Terms and Conditions document is configured.',
                1;
        END;

        ------------------------------------------------------------
        -- Use one UTC timestamp for this registration
        ------------------------------------------------------------
        SET @RegistrationUtc = SYSUTCDATETIME();

        ------------------------------------------------------------
        -- Validate expiration against the actual registration time
        ------------------------------------------------------------
        IF @VerificationExpiresUtc <= @RegistrationUtc
        BEGIN
            THROW 50032,
                'Email verification token expiration must be in the future.',
                1;
        END;

        ------------------------------------------------------------
        -- Create user
        --
        -- UserId is intentionally omitted.
        -- Users.UserId DEFAULT NEWSEQUENTIALID() generates it.
        --
        -- New users start with IsEmailVerified = 0.
        ------------------------------------------------------------
        INSERT INTO [dbo].[Users]
        (
            [Email],
            [PasswordHash],
            [IsActive],
            [IsDeleted],
            [IsEmailVerified],
            [CreatedDateUtc]
        )
        OUTPUT inserted.[UserId]
            INTO @CreatedUser ([UserId])
        VALUES
        (
            @Email,
            @PasswordHash,
            1,
            0,
            0,
            @RegistrationUtc
        );

        ------------------------------------------------------------
        -- Capture generated UserId
        ------------------------------------------------------------
        SELECT
            @UserId = [UserId]
        FROM @CreatedUser;

        IF @UserId IS NULL
        BEGIN
            THROW 50002,
                'User creation failed to return a UserId.',
                1;
        END;

        ------------------------------------------------------------
        -- Record exact Terms accepted by this user
        ------------------------------------------------------------
        INSERT INTO [dbo].[UserTermsAcceptances]
        (
            [UserId],
            [TermsDocumentId],
            [TermsVersion],
            [TermsDocumentHash],
            [AcceptedUtc]
        )
        VALUES
        (
            @UserId,
            @TermsDocumentId,
            @TermsVersion,
            @TermsDocumentHash,
            @RegistrationUtc
        );

        ------------------------------------------------------------
        -- Create initial email verification token
        --
        -- Only the SHA-256 hash is stored.
        -- The raw token must never be persisted.
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
            @VerificationTokenHash,
            @VerificationExpiresUtc
        );

        ------------------------------------------------------------
        -- Commit user + Terms acceptance + verification token
        -- as one atomic registration transaction
        ------------------------------------------------------------
        COMMIT TRANSACTION;

        ------------------------------------------------------------
        -- Return newly created user
        ------------------------------------------------------------
        SELECT
            [UserId],
            [Email],
            [PasswordHash],
            [IsActive],
            [IsDeleted],
            [IsEmailVerified],
            [CreatedDateUtc],
            [ModifiedDateUtc]
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