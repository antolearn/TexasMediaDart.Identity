CREATE TABLE [dbo].[UserTermsAcceptances]
(
    [Id] UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT [DF_UserTermsAcceptances_Id]
        DEFAULT NEWSEQUENTIALID(),

    [UserId] UNIQUEIDENTIFIER NOT NULL,

    [TermsDocumentId] UNIQUEIDENTIFIER NOT NULL,

    [TermsVersion] NVARCHAR(50) NOT NULL,

    [TermsDocumentHash] NVARCHAR(64) NOT NULL,

    [AcceptedUtc] DATETIME2(7) NOT NULL
        CONSTRAINT [DF_UserTermsAcceptances_AcceptedUtc]
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT [PK_UserTermsAcceptances]
        PRIMARY KEY CLUSTERED ([Id]),

    CONSTRAINT [FK_UserTermsAcceptances_Users]
        FOREIGN KEY ([UserId])
        REFERENCES [dbo].[Users] ([UserId]),

    CONSTRAINT [FK_UserTermsAcceptances_TermsDocuments]
        FOREIGN KEY ([TermsDocumentId])
        REFERENCES [dbo].[TermsDocuments] ([TermsDocumentId]),

    CONSTRAINT [UQ_UserTermsAcceptances_User_TermsDocument]
        UNIQUE ([UserId], [TermsDocumentId]),

    CONSTRAINT [CK_UserTermsAcceptances_TermsDocumentHash_Length]
        CHECK (LEN([TermsDocumentHash]) = 64)
);