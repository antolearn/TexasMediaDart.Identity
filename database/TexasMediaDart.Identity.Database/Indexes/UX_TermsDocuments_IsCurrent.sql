CREATE UNIQUE INDEX [UX_TermsDocuments_IsCurrent]
    ON [dbo].[TermsDocuments] ([IsCurrent])
    WHERE [IsCurrent] = 1;