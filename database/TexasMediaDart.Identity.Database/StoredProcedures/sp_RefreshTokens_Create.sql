CREATE PROCEDURE [dbo].[sp_RefreshTokens_Create]
    @UserId UNIQUEIDENTIFIER,
    @TokenHash NVARCHAR(256),
    @ExpiresDateUtc DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[RefreshTokens]
    (
        [UserId],
        [TokenHash],
        [ExpiresDateUtc]
    )
    VALUES
    (
        @UserId,
        @TokenHash,
        @ExpiresDateUtc
    );

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
    WHERE [TokenHash] = @TokenHash;
END;
GO