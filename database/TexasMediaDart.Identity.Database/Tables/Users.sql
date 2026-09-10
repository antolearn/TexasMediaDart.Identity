CREATE TABLE [dbo].[Users]
(
    [UserId] UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT [DF_Users_UserId] DEFAULT NEWSEQUENTIALID(),

    [Email] NVARCHAR(320) NOT NULL,

    [PasswordHash] NVARCHAR(500) NOT NULL,

    [IsActive] BIT NOT NULL
        CONSTRAINT [DF_Users_IsActive] DEFAULT (1),

    [IsDeleted] BIT NOT NULL
        CONSTRAINT [DF_Users_IsDeleted] DEFAULT (0),

    [IsEmailVerified] BIT NOT NULL
        CONSTRAINT [DF_Users_IsEmailVerified] DEFAULT (0),

    [CreatedDateUtc] DATETIME2(7) NOT NULL
        CONSTRAINT [DF_Users_CreatedDateUtc] DEFAULT SYSUTCDATETIME(),

    [ModifiedDateUtc] DATETIME2(7) NULL,

    CONSTRAINT [PK_Users]
        PRIMARY KEY CLUSTERED ([UserId]),

    CONSTRAINT [UQ_Users_Email]
        UNIQUE ([Email])
);