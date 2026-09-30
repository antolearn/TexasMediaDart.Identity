CREATE PROCEDURE [dbo].[sp_Terms_GetCurrent]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Version],
        [Title],
        [Content],
        [EffectiveUtc]
    FROM [dbo].[TermsDocuments]
    WHERE [IsCurrent] = 1;
END;