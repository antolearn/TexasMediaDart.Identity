CREATE TABLE [dbo].[RefreshTokens]
(
    [RefreshTokenId] UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT [DF_RefreshTokens_RefreshTokenId]
        DEFAULT NEWSEQUENTIALID(),

    [UserId] UNIQUEIDENTIFIER NOT NULL,

    [TokenHash] NVARCHAR(256) NOT NULL,

    [ExpiresDateUtc] DATETIME2(7) NOT NULL,

    [CreatedDateUtc] DATETIME2(7) NOT NULL
        CONSTRAINT [DF_RefreshTokens_CreatedDateUtc]
        DEFAULT SYSUTCDATETIME(),

    [RevokedDateUtc] DATETIME2(7) NULL,

    [IsRevoked] BIT NOT NULL
        CONSTRAINT [DF_RefreshTokens_IsRevoked]
        DEFAULT (0),

    [ReplacedByTokenHash] NVARCHAR(256) NULL,

    CONSTRAINT [PK_RefreshTokens]
        PRIMARY KEY CLUSTERED ([RefreshTokenId]),

    CONSTRAINT [FK_RefreshTokens_Users]
        FOREIGN KEY ([UserId])
        REFERENCES [dbo].[Users] ([UserId]),

    CONSTRAINT [UQ_RefreshTokens_TokenHash]
        UNIQUE ([TokenHash])
);