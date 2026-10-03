CREATE TABLE [dbo].[EmailVerificationTokens]
(
    [EmailVerificationTokenId] UNIQUEIDENTIFIER
        NOT NULL
        CONSTRAINT [DF_EmailVerificationTokens_EmailVerificationTokenId]
        DEFAULT NEWSEQUENTIALID(),

    [UserId] UNIQUEIDENTIFIER NOT NULL,

    [TokenHash] CHAR(64) NOT NULL,

    [ExpiresUtc] DATETIME2(7) NOT NULL,

    [UsedUtc] DATETIME2(7) NULL,

    [InvalidatedUtc] DATETIME2(7) NULL,

    [CreatedUtc] DATETIME2(7)
        NOT NULL
        CONSTRAINT [DF_EmailVerificationTokens_CreatedUtc]
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT [PK_EmailVerificationTokens]
        PRIMARY KEY CLUSTERED
        (
            [EmailVerificationTokenId]
        ),

    CONSTRAINT [FK_EmailVerificationTokens_Users]
        FOREIGN KEY
        (
            [UserId]
        )
        REFERENCES [dbo].[Users]
        (
            [UserId]
        ),

    CONSTRAINT [CK_EmailVerificationTokens_ExpiresUtc]
        CHECK
        (
            [ExpiresUtc] > [CreatedUtc]
        ),

    CONSTRAINT [CK_EmailVerificationTokens_UsedUtc]
        CHECK
        (
            [UsedUtc] IS NULL
            OR [UsedUtc] >= [CreatedUtc]
        ),

    CONSTRAINT [CK_EmailVerificationTokens_InvalidatedUtc]
        CHECK
        (
            [InvalidatedUtc] IS NULL
            OR [InvalidatedUtc] >= [CreatedUtc]
        ),

    CONSTRAINT [CK_EmailVerificationTokens_Status]
        CHECK
        (
            [UsedUtc] IS NULL
            OR [InvalidatedUtc] IS NULL
        )
);