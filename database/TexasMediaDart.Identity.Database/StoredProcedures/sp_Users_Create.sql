CREATE PROCEDURE [dbo].[sp_Users_Create]
    @Email        NVARCHAR(320),
    @PasswordHash NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    SET @Email = LOWER(LTRIM(RTRIM(@Email)));

    IF NULLIF(@Email, '') IS NULL
    BEGIN
        THROW 50001, 'Email is required.', 1;
    END;

    IF NULLIF(@PasswordHash, '') IS NULL
    BEGIN
        THROW 50002, 'Password hash is required.', 1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM [dbo].[Users]
        WHERE [Email] = @Email
    )
    BEGIN
        THROW 50003, 'A user with this email already exists.', 1;
    END;

    INSERT INTO [dbo].[Users]
    (
        [Email],
        [PasswordHash]
    )
    OUTPUT
        INSERTED.[UserId],
        INSERTED.[Email],
        INSERTED.[IsActive],
        INSERTED.[IsDeleted],
        INSERTED.[IsEmailVerified],
        INSERTED.[CreatedDateUtc],
        INSERTED.[ModifiedDateUtc]
    VALUES
    (
        @Email,
        @PasswordHash
    );
END;