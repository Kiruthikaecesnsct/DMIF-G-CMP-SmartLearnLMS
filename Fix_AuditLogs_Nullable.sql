---- ============================================================
----  SmartLearn — Fix AuditLogs column nullability
----  Run once in SSMS against SmartLearnDB
----
----  Problem: the AuditLogs table was created with NOT NULL on
----  Category, Username, Details, IpAddress (and possibly Action),
----  but the C# entity maps them as nullable strings. EF Core
----  therefore fails with a DbUpdateException every time it tries
----  to INSERT a row where any of those fields is null.
----
----  This script alters each offending column to NULL.
---- ============================================================

--USE SmartLearnDB;
--GO

---- 1. Action (was NOT NULL due to [Required] annotation)
--IF EXISTS (
--    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
--    WHERE TABLE_NAME = 'AuditLogs'
--      AND COLUMN_NAME = 'Action'
--      AND IS_NULLABLE = 'NO'
--)
--BEGIN
--    ALTER TABLE [dbo].[AuditLogs]
--        ALTER COLUMN [Action] NVARCHAR(50) NULL;
--    PRINT '  Action  -> NULL';
--END
--ELSE PRINT '  Action  already nullable, skipped.';

---- 2. Category
--IF EXISTS (
--    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
--    WHERE TABLE_NAME = 'AuditLogs'
--      AND COLUMN_NAME = 'Category'
--      AND IS_NULLABLE = 'NO'
--)
--BEGIN
--    ALTER TABLE [dbo].[AuditLogs]
--        ALTER COLUMN [Category] NVARCHAR(50) NULL;
--    PRINT '  Category -> NULL';
--END
--ELSE PRINT '  Category already nullable, skipped.';

---- 3. Username
--IF EXISTS (
--    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
--    WHERE TABLE_NAME = 'AuditLogs'
--      AND COLUMN_NAME = 'Username'
--      AND IS_NULLABLE = 'NO'
--)
--BEGIN
--    ALTER TABLE [dbo].[AuditLogs]
--        ALTER COLUMN [Username] NVARCHAR(50) NULL;
--    PRINT '  Username -> NULL';
--END
--ELSE PRINT '  Username already nullable, skipped.';

---- 4. Details
--IF EXISTS (
--    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
--    WHERE TABLE_NAME = 'AuditLogs'
--      AND COLUMN_NAME = 'Details'
--      AND IS_NULLABLE = 'NO'
--)
--BEGIN
--    ALTER TABLE [dbo].[AuditLogs]
--        ALTER COLUMN [Details] NVARCHAR(MAX) NULL;
--    PRINT '  Details  -> NULL';
--END
--ELSE PRINT '  Details  already nullable, skipped.';

---- 5. IpAddress
--IF EXISTS (
--    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
--    WHERE TABLE_NAME = 'AuditLogs'
--      AND COLUMN_NAME = 'IpAddress'
--      AND IS_NULLABLE = 'NO'
--)
--BEGIN
--    ALTER TABLE [dbo].[AuditLogs]
--        ALTER COLUMN [IpAddress] NVARCHAR(45) NULL;
--    PRINT '  IpAddress -> NULL';
--END
--ELSE PRINT '  IpAddress already nullable, skipped.';

---- ============================================================
----  Verify final column definitions
---- ============================================================
--SELECT
--    COLUMN_NAME,
--    DATA_TYPE,
--    CHARACTER_MAXIMUM_LENGTH,
--    IS_NULLABLE
--FROM INFORMATION_SCHEMA.COLUMNS
--WHERE TABLE_NAME = 'AuditLogs'
--ORDER BY ORDINAL_POSITION;
--GO
--=============================================================
--USE SmartLearnDB;

--INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
--VALUES ('20260331042613_AddAuditLogsAndIndexes', '8.0.0');
--========================================================
--USE SmartLearnDB;

---- Check what migrations EF thinks are applied
--SELECT * FROM [dbo].[__EFMigrationsHistory] ORDER BY [MigrationId];

---- Check if AuditLogs table actually exists
--SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
--WHERE TABLE_NAME = 'AuditLogs';

---- Check its current column nullability
--SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
--FROM INFORMATION_SCHEMA.COLUMNS
--WHERE TABLE_NAME = 'AuditLogs'
--ORDER BY ORDINAL_POSITION;
--=====================================================================
USE SmartLearnDB;

-- Drop indexes first (SQL Server won't ALTER a column that has an index on it)
DROP INDEX IF EXISTS [IX_AuditLogs_Action]    ON [dbo].[AuditLogs];
DROP INDEX IF EXISTS [IX_AuditLogs_Timestamp] ON [dbo].[AuditLogs];
DROP INDEX IF EXISTS [IX_AuditLogs_UserId]    ON [dbo].[AuditLogs];

-- Fix the NOT NULL columns
ALTER TABLE [dbo].[AuditLogs] ALTER COLUMN [Action]    NVARCHAR(50)  NULL;
ALTER TABLE [dbo].[AuditLogs] ALTER COLUMN [Category]  NVARCHAR(50)  NULL;
ALTER TABLE [dbo].[AuditLogs] ALTER COLUMN [Username]  NVARCHAR(50)  NULL;
ALTER TABLE [dbo].[AuditLogs] ALTER COLUMN [Details]   NVARCHAR(MAX) NULL;
ALTER TABLE [dbo].[AuditLogs] ALTER COLUMN [IpAddress] NVARCHAR(45)  NULL;

-- Re-create the indexes
CREATE INDEX [IX_AuditLogs_Action]    ON [dbo].[AuditLogs] ([Action]);
CREATE INDEX [IX_AuditLogs_Timestamp] ON [dbo].[AuditLogs] ([Timestamp]);
CREATE INDEX [IX_AuditLogs_UserId]    ON [dbo].[AuditLogs] ([UserId]);

-- Verify — all 5 should now show YES
SELECT COLUMN_NAME, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AuditLogs'
ORDER BY ORDINAL_POSITION;