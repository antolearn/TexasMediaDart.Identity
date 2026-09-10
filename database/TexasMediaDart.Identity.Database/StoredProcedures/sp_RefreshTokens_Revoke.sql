CREATE PROCEDURE [dbo].[sp_RefreshTokens_Revoke]
    @TokenHash NVARCHAR(256),
    @ReplacedByTokenHash NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[RefreshTokens]
    SET
        [IsRevoked] = 1,
        [RevokedDateUtc] = SYSUTCDATETIME(),
        [ReplacedByTokenHash] = @ReplacedByTokenHash
    WHERE
        [TokenHash] = @TokenHash
        AND [IsRevoked] = 0;

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