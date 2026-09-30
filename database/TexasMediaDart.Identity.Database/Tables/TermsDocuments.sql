CREATE TABLE [dbo].[TermsDocuments]
(
    [TermsDocumentId] UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT [DF_TermsDocuments_TermsDocumentId]
        DEFAULT NEWSEQUENTIALID(),

    [Version] NVARCHAR(50) NOT NULL,

    [Title] NVARCHAR(200) NOT NULL,

    [Content] NVARCHAR(MAX) NOT NULL,

    [DocumentHash] NVARCHAR(64) NOT NULL,

    [EffectiveUtc] DATETIME2(7) NOT NULL,

    [IsCurrent] BIT NOT NULL
        CONSTRAINT [DF_TermsDocuments_IsCurrent]
        DEFAULT (0),

    [CreatedUtc] DATETIME2(7) NOT NULL
        CONSTRAINT [DF_TermsDocuments_CreatedUtc]
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT [PK_TermsDocuments]
        PRIMARY KEY CLUSTERED ([TermsDocumentId]),

    CONSTRAINT [UQ_TermsDocuments_Version]
        UNIQUE ([Version]),

    CONSTRAINT [UQ_TermsDocuments_DocumentHash]
        UNIQUE ([DocumentHash]),

    CONSTRAINT [CK_TermsDocuments_DocumentHash_Length]
        CHECK (LEN([DocumentHash]) = 64),

    CONSTRAINT [CK_TermsDocuments_Version_NotEmpty]
        CHECK (LEN(LTRIM(RTRIM([Version]))) > 0),

    CONSTRAINT [CK_TermsDocuments_Title_NotEmpty]
        CHECK (LEN(LTRIM(RTRIM([Title]))) > 0),

    CONSTRAINT [CK_TermsDocuments_Content_NotEmpty]
        CHECK (LEN(LTRIM(RTRIM([Content]))) > 0)
);