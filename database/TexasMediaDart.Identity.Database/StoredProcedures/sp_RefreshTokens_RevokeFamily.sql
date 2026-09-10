CREATE PROCEDURE [dbo].[sp_RefreshTokens_RevokeFamily]
    @TokenHash NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH TokenChain AS
    (
        -- Start with the token that was reused
        SELECT
            [RefreshTokenId],
            [TokenHash],
            [ReplacedByTokenHash]
        FROM [dbo].[RefreshTokens]
        WHERE [TokenHash] = @TokenHash

        UNION ALL

        -- Follow A -> B -> C -> ...
        SELECT
            rt.[RefreshTokenId],
            rt.[TokenHash],
            rt.[ReplacedByTokenHash]
        FROM [dbo].[RefreshTokens] rt
        INNER JOIN TokenChain tc
            ON rt.[TokenHash] = tc.[ReplacedByTokenHash]
    )
    UPDATE rt
    SET
        rt.[IsRevoked] = 1,
        rt.[RevokedDateUtc] =
            COALESCE(
                rt.[RevokedDateUtc],
                SYSUTCDATETIME())
    FROM [dbo].[RefreshTokens] rt
    INNER JOIN TokenChain tc
        ON rt.[RefreshTokenId] = tc.[RefreshTokenId]
    WHERE rt.[IsRevoked] = 0
    OPTION (MAXRECURSION 100);
END;
GO