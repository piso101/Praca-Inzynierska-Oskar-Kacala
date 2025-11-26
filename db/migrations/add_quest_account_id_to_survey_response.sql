-- Migration: Add quest_account_id to survey_response table
-- This allows tracking which quest_account submitted which response
-- and prevents duplicate responses from the same email

BEGIN;

-- Add quest_account_id column
ALTER TABLE survey_response 
ADD COLUMN IF NOT EXISTS quest_account_id BIGINT REFERENCES quest_account(id) ON DELETE SET NULL;

-- Make user_id nullable (it was already nullable in schema)
ALTER TABLE survey_response 
ALTER COLUMN user_id DROP NOT NULL;

-- Add check constraint to ensure only one type of user is set
ALTER TABLE survey_response 
DROP CONSTRAINT IF EXISTS chk_one_user_type;

ALTER TABLE survey_response 
ADD CONSTRAINT chk_one_user_type CHECK (
    (user_id IS NOT NULL AND quest_account_id IS NULL) OR
    (user_id IS NULL AND quest_account_id IS NOT NULL)
);

-- Add index for quest_account_id lookups
CREATE INDEX IF NOT EXISTS idx_survey_response_quest_account ON survey_response (quest_account_id);

-- Add index for checking duplicate responses (quest_account + survey)
CREATE INDEX IF NOT EXISTS idx_survey_response_quest_survey ON survey_response (quest_account_id, survey_id);

COMMIT;


