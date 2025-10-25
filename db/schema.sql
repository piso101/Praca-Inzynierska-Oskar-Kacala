BEGIN;

-- Typy i role użytkowników
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'user_role') THEN
        CREATE TYPE user_role AS ENUM ('anonymous', 'admin');
    END IF;
END$$;

-- Tabela użytkowników (administratorzy oraz anonimowi uczestnicy)
CREATE TABLE IF NOT EXISTS user_account (
    id BIGSERIAL PRIMARY KEY,
    role user_role NOT NULL DEFAULT 'anonymous',
    display_name TEXT,
    email TEXT UNIQUE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
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

-- Tabela pytań w ankiecie
CREATE TABLE IF NOT EXISTS survey_question (
    id BIGSERIAL PRIMARY KEY,
    survey_id BIGINT NOT NULL REFERENCES survey(id) ON DELETE CASCADE,
    question_text TEXT NOT NULL,
    order_index INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    CONSTRAINT uq_survey_question_order UNIQUE (survey_id, order_index)
);

-- Elementy (modele) przypięte do pytania: ID, x, y, rotacja, skala, nazwa pliku SVG
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
    -- Unikalność pary (id, question_id) do wsparcia złożonego klucza obcego
    CONSTRAINT uq_question_element_id_question UNIQUE (id, question_id)
);

-- Odpowiedź użytkownika na ankietę (anonimowa lub powiązana z użytkownikiem)
CREATE TABLE IF NOT EXISTS survey_response (
    id BIGSERIAL PRIMARY KEY,
    survey_id BIGINT NOT NULL REFERENCES survey(id) ON DELETE CASCADE,
    user_id BIGINT REFERENCES user_account(id) ON DELETE SET NULL,
    submitted_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    metadata JSONB
);

-- Wybory w ramach odpowiedzi: dla każdego pytania wskazany element
CREATE TABLE IF NOT EXISTS response_answer (
    id BIGSERIAL PRIMARY KEY,
    response_id BIGINT NOT NULL REFERENCES survey_response(id) ON DELETE CASCADE,
    question_id BIGINT NOT NULL REFERENCES survey_question(id) ON DELETE CASCADE,
    selected_element_id BIGINT NOT NULL REFERENCES question_element(id) ON DELETE RESTRICT,
    CONSTRAINT uq_response_question UNIQUE (response_id, question_id),
    -- Wymuś, aby wybrany element należał do tego samego pytania
    CONSTRAINT fk_answer_element_belongs_to_question
        FOREIGN KEY (selected_element_id, question_id)
        REFERENCES question_element (id, question_id)
        ON DELETE RESTRICT
);

-- Indeksy pomocnicze
CREATE INDEX IF NOT EXISTS idx_survey_question_survey ON survey_question (survey_id);
CREATE INDEX IF NOT EXISTS idx_question_element_question ON question_element (question_id);
CREATE INDEX IF NOT EXISTS idx_survey_response_survey ON survey_response (survey_id);
CREATE INDEX IF NOT EXISTS idx_survey_response_user ON survey_response (user_id);
CREATE INDEX IF NOT EXISTS idx_response_answer_response ON response_answer (response_id);
CREATE INDEX IF NOT EXISTS idx_response_answer_question ON response_answer (question_id);
CREATE INDEX IF NOT EXISTS idx_response_answer_selected_element ON response_answer (selected_element_id);

COMMIT;


