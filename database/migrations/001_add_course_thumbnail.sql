USE skillbridge;
SET @thumbnail_migration = IF(
    EXISTS(SELECT 1 FROM information_schema.columns
           WHERE table_schema = DATABASE() AND table_name = 'courses' AND column_name = 'thumbnail_url'),
    'SELECT 1',
    'ALTER TABLE courses ADD COLUMN thumbnail_url VARCHAR(255) NULL AFTER artwork_key');
PREPARE thumbnail_statement FROM @thumbnail_migration;
EXECUTE thumbnail_statement;
DEALLOCATE PREPARE thumbnail_statement;
