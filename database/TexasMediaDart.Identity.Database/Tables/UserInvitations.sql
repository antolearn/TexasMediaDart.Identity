CREATE TABLE [dbo].[UserInvitations]
(
    [InvitationId] UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT [DF_UserInvitations_InvitationId]
        DEFAULT NEWSEQUENTIALID(),

    [Email] NVARCHAR(320) NOT NULL,

    [OrganizationId] UNIQUEIDENTIFIER NOT NULL,

    [InvitedByIdentityUserId] UNIQUEIDENTIFIER NOT NULL,

    [TokenHash] CHAR(64) NOT NULL,

    [ExpiresUtc] DATETIME2(7) NOT NULL,

    [CreatedIdentityUserId] UNIQUEIDENTIFIER NULL,

    [IdentityCreatedUtc] DATETIME2(7) NULL,

    [AcceptedUtc] DATETIME2(7) NULL,

    [RevokedUtc] DATETIME2(7) NULL,

    [CreatedUtc] DATETIME2(7) NOT NULL
        CONSTRAINT [DF_UserInvitations_CreatedUtc]
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT [PK_UserInvitations]
        PRIMARY KEY CLUSTERED ([InvitationId]),

    CONSTRAINT [UQ_UserInvitations_TokenHash]
        UNIQUE ([TokenHash]),

    CONSTRAINT [FK_UserInvitations_InvitedBy]
        FOREIGN KEY ([InvitedByIdentityUserId])
        REFERENCES [dbo].[Users] ([UserId]),

    CONSTRAINT [FK_UserInvitations_CreatedIdentityUser]
        FOREIGN KEY ([CreatedIdentityUserId])
        REFERENCES [dbo].[Users] ([UserId]),

    CONSTRAINT [CK_UserInvitations_ExpiresUtc]
        CHECK ([ExpiresUtc] > [CreatedUtc]),

    CONSTRAINT [CK_UserInvitations_IdentityCreated]
        CHECK
        (
            (
                [CreatedIdentityUserId] IS NULL
                AND [IdentityCreatedUtc] IS NULL
            )
            OR
            (
                [CreatedIdentityUserId] IS NOT NULL
                AND [IdentityCreatedUtc] IS NOT NULL
            )
        )
);