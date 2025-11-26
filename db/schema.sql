BEGIN;

-- Tabela użytkowników z role jako integer
CREATE TABLE IF NOT EXISTS user_account (
    id BIGSERIAL PRIMARY KEY,
    role INTEGER NOT NULL DEFAULT 0,  -- <-- integer zamiast user_role
    display_name TEXT,
    email TEXT UNIQUE,
    password_hash TEXT NOT NULL DEFAULT '',
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- Tabela użytkowników ankiet (Quest) - logowanie przez email
CREATE TABLE IF NOT EXISTS quest_account (
    id BIGSERIAL PRIMARY KEY,
    email TEXT NOT NULL UNIQUE,
    display_name TEXT,
    role INTEGER NOT NULL DEFAULT 2,  -- Quest role = 2
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    email_verification_token TEXT,
    token_expires_at TIMESTAMPTZ,
    email_verified_at TIMESTAMPTZ
);


-- Tabela ankiet
CREATE TABLE IF NOT EXISTS survey (
    id BIGSERIAL PRIMARY KEY,
    title TEXT NOT NULL,
    description TEXT,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_by BIGINT REFERENCES user_account(id) ON DELETE SET NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    published_at TIMESTAMPTZ
);

-- Tabela pytań
CREATE TABLE IF NOT EXISTS survey_question (
    id BIGSERIAL PRIMARY KEY,
    survey_id BIGINT NOT NULL REFERENCES survey(id) ON DELETE CASCADE,
    question_text TEXT NOT NULL,
    order_index INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    CONSTRAINT uq_survey_question_order UNIQUE (survey_id, order_index)
);

-- Elementy pytań
CREATE TABLE IF NOT EXISTS question_element (
    id BIGSERIAL PRIMARY KEY,
    question_id BIGINT NOT NULL REFERENCES survey_question(id) ON DELETE CASCADE,
    x NUMERIC(10,4) NOT NULL,
    y NUMERIC(10,4) NOT NULL,
    rotation NUMERIC(10,4) NOT NULL DEFAULT 0,
    scale NUMERIC(10,4) NOT NULL DEFAULT 1 CHECK (scale > 0),
    svg_name TEXT NOT NULL,
    order_index INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT uq_question_element_order UNIQUE (question_id, order_index),
    CONSTRAINT uq_question_element_id_question UNIQUE (id, question_id)
);

-- Odpowiedzi użytkowników
CREATE TABLE IF NOT EXISTS survey_response (
    id BIGSERIAL PRIMARY KEY,
    survey_id BIGINT NOT NULL REFERENCES survey(id) ON DELETE CASCADE,
    user_id BIGINT REFERENCES user_account(id) ON DELETE SET NULL,  -- Admin user (nullable)
    quest_account_id BIGINT REFERENCES quest_account(id) ON DELETE SET NULL,  -- Quest account (nullable)
    submitted_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    metadata JSONB,
    CONSTRAINT chk_one_user_type CHECK (
        (user_id IS NOT NULL AND quest_account_id IS NULL) OR
        (user_id IS NULL AND quest_account_id IS NOT NULL)
    )
);

-- Wybrane elementy w odpowiedzi
CREATE TABLE IF NOT EXISTS response_answer (
    id BIGSERIAL PRIMARY KEY,
    response_id BIGINT NOT NULL REFERENCES survey_response(id) ON DELETE CASCADE,
    question_id BIGINT NOT NULL REFERENCES survey_question(id) ON DELETE CASCADE,
    selected_element_id BIGINT NOT NULL REFERENCES question_element(id) ON DELETE RESTRICT,
    CONSTRAINT uq_response_question UNIQUE (response_id, question_id),
    CONSTRAINT fk_answer_element_belongs_to_question
        FOREIGN KEY (selected_element_id, question_id)
        REFERENCES question_element (id, question_id)
        ON DELETE RESTRICT
);

-- Indeksy
CREATE INDEX IF NOT EXISTS idx_survey_question_survey ON survey_question (survey_id);
CREATE INDEX IF NOT EXISTS idx_question_element_question ON question_element (question_id);
CREATE INDEX IF NOT EXISTS idx_survey_response_survey ON survey_response (survey_id);
CREATE INDEX IF NOT EXISTS idx_survey_response_user ON survey_response (user_id);
CREATE INDEX IF NOT EXISTS idx_response_answer_response ON response_answer (response_id);
CREATE INDEX IF NOT EXISTS idx_response_answer_question ON response_answer (question_id);
CREATE INDEX IF NOT EXISTS idx_response_answer_selected_element ON response_answer (selected_element_id);
CREATE INDEX IF NOT EXISTS idx_quest_account_email ON quest_account (email);
CREATE INDEX IF NOT EXISTS idx_quest_account_verification_token ON quest_account (email_verification_token);

-- Komentarze
COMMENT ON TABLE quest_account IS 'Użytkownicy ankiet logujący się przez email';
COMMENT ON COLUMN quest_account.role IS 'Rola użytkownika: 0=Anonymous, 1=Admin, 2=Quest';
COMMENT ON COLUMN quest_account.email_verification_token IS 'Token weryfikacyjny wysyłany w emailu (ważny 1h)';

COMMIT;
