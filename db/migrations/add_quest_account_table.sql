-- Migration: Add quest_account table for email-based authentication
-- Date: 2025-11-24

BEGIN;

-- Create quest_account table
CREATE TABLE IF NOT EXISTS quest_account (
    id BIGSERIAL PRIMARY KEY,
    email TEXT NOT NULL UNIQUE,
    display_name TEXT,
    role INTEGER NOT NULL DEFAULT 2, -- Quest role = 2
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    email_verification_token TEXT,
    token_expires_at TIMESTAMPTZ,
    email_verified_at TIMESTAMPTZ
);

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS idx_quest_account_email ON quest_account(email);
CREATE INDEX IF NOT EXISTS idx_quest_account_verification_token ON quest_account(email_verification_token);

-- Update user_role enum to include Quest role (if not already done)
-- Note: This assumes role is stored as integer (0=Anonymous, 1=Admin, 2=Quest)
COMMENT ON COLUMN quest_account.role IS 'User role: 0=Anonymous, 1=Admin, 2=Quest';

COMMIT;


