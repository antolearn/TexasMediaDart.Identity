CREATE PROCEDURE [dbo].[sp_RefreshTokens_GetByTokenHash]
    @TokenHash NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

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