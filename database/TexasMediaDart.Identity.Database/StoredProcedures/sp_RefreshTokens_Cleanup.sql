CREATE PROCEDURE [dbo].[sp_RefreshTokens_Cleanup]
    @RetentionDays INT = 30
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CutoffDateUtc DATETIME2(7);

    SET @CutoffDateUtc =
        DATEADD(
            DAY,
            -@RetentionDays,
            SYSUTCDATETIME());

    DELETE FROM [dbo].[RefreshTokens]
    WHERE
        (
            [ExpiresDateUtc] < SYSUTCDATETIME()
            OR [IsRevoked] = 1
        )
        AND [CreatedDateUtc] < @CutoffDateUtc;

    SELECT
        @@ROWCOUNT AS DeletedCount;
END;
GO