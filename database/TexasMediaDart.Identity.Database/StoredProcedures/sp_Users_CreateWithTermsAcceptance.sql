CREATE PROCEDURE [dbo].[sp_Users_CreateWithTermsAcceptance]
    @Email NVARCHAR(320),
    @PasswordHash NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @UserId UNIQUEIDENTIFIER;
    DECLARE @TermsDocumentId UNIQUEIDENTIFIER;
    DECLARE @TermsVersion NVARCHAR(50);
    DECLARE @TermsDocumentHash NVARCHAR(64);
    DECLARE @AcceptedUtc DATETIME2(7);

    DECLARE @CreatedUser TABLE
    (
        [UserId] UNIQUEIDENTIFIER NOT NULL
    );

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
        SET @AcceptedUtc = SYSUTCDATETIME();

        ------------------------------------------------------------
        -- Create user
        --
        -- UserId is intentionally omitted.
        -- Users.UserId DEFAULT NEWSEQUENTIALID() generates it.
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
            @AcceptedUtc
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
            @AcceptedUtc
        );

        ------------------------------------------------------------
        -- Commit both operations together
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