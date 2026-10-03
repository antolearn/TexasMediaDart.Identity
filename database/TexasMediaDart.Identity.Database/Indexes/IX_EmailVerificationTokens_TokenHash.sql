CREATE UNIQUE NONCLUSTERED INDEX
    [IX_EmailVerificationTokens_TokenHash]
ON [dbo].[EmailVerificationTokens]
(
    [TokenHash]
);