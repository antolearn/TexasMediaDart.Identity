CREATE UNIQUE NONCLUSTERED INDEX
    [UX_EmailVerificationTokens_ActiveUser]
ON [dbo].[EmailVerificationTokens]
(
    [UserId]
)
WHERE
    [UsedUtc] IS NULL
    AND [InvalidatedUtc] IS NULL;