CREATE PROCEDURE [dbo].[sp_RefreshTokens_Rotate]
    @CurrentTokenHash NVARCHAR(256),
    @NewTokenHash NVARCHAR(256),
    @NewExpiresDateUtc DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    DECLARE @RefreshTokenId UNIQUEIDENTIFIER;
    DECLARE @UserId UNIQUEIDENTIFIER;
    DECLARE @IsRevoked BIT;
    DECLARE @ExpiresDateUtc DATETIME2(7);

    SELECT
        @RefreshTokenId = [RefreshTokenId],
        @UserId = [UserId],
        @IsRevoked = [IsRevoked],
        @ExpiresDateUtc = [ExpiresDateUtc]
    FROM [dbo].[RefreshTokens] WITH (UPDLOCK, HOLDLOCK)
    WHERE [TokenHash] = @CurrentTokenHash;

    IF @RefreshTokenId IS NULL
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50001, 'Refresh token not found.', 1;
    END;

    IF @IsRevoked = 1
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50002, 'Refresh token already revoked.', 1;
    END;

    IF @ExpiresDateUtc <= SYSUTCDATETIME()
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50003, 'Refresh token expired.', 1;
    END;

    INSERT INTO [dbo].[RefreshTokens]
    (
        [UserId],
        [TokenHash],
        [ExpiresDateUtc]
    )
    VALUES
    (
        @UserId,
        @NewTokenHash,
        @NewExpiresDateUtc
    );

    UPDATE [dbo].[RefreshTokens]
    SET
        [IsRevoked] = 1,
        [RevokedDateUtc] = SYSUTCDATETIME(),
        [ReplacedByTokenHash] = @NewTokenHash
    WHERE [RefreshTokenId] = @RefreshTokenId;

    COMMIT TRANSACTION;

    SELECT
        [RefreshTokenId],
        [UserId],
        [TokenHash],
        [ExpiresDateUtc],
        [CreatedDateUtc],
        [RevokedDateUtc],
        [IsRevoked],
        [ReplacedByTokenHash]
    FROM [dbo].[RefreshTokens]
    WHERE [TokenHash] = @NewTokenHash;
END;
GO