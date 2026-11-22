# Survey Builder

Aplikacja webowa do tworzenia ankiet, budowania pytań z elementów graficznych i zbierania odpowiedzi. Projekt powstał jako praca inżynierska.

## Funkcje

- tworzenie i edycja ankiet oraz pytań;
- rozmieszczanie i porządkowanie elementów graficznych w pytaniach;
- publiczny udział w ankiecie oraz logowanie uczestników przez e-mail;
- panel administracyjny i podgląd wyników;
- przechowywanie plików w zgodnym z S3 magazynie MinIO.

## Technologie

- **Frontend:** React, TypeScript, Vite, AWS SDK for JavaScript;
- **Backend:** ASP.NET Core 8, Entity Framework Core, PostgreSQL;
- **Pliki:** MinIO;
- **Uruchamianie lokalne:** Docker Compose.

## Uruchomienie lokalne

Wymagane: Docker Desktop z Compose, Node.js 20+ (dla pracy bez Dockera) oraz .NET SDK 8 (dla pracy bez Dockera).

1. Skopiuj `.env.example` do `.env` i ustaw własne, losowe wartości w każdym polu oznaczonym `REPLACE_`. Nie używaj tych danych poza lokalnym środowiskiem.
2. W katalogu głównym uruchom:

   ```powershell
   docker compose up --build
   ```

3. Otwórz:
   - frontend: <http://localhost:5173>;
   - API i Swagger: <http://localhost:8080/swagger>;
   - konsolę MinIO: <http://localhost:9001>.

PostgreSQL inicjalizuje schemat z plików w `db/`. Dane PostgreSQL i MinIO pozostają lokalne; `data/minio/` nie jest częścią repozytorium. Skrypt nie tworzy wspólnego konta administratora. Do lokalnego utworzenia administratora służy endpoint `POST /api/Auth/create-admin`, zabezpieczony wartością `OWNER_PASSWORD`.

Bez Dockera backend wymaga ustawienia konfiguracji przez .NET User Secrets, np. `dotnet user-secrets set "Jwt:Key" ...`, `dotnet user-secrets set "HashingSystem:DefaultHashingSurveyKey" ...`, `dotnet user-secrets set "ConnectionStrings:DefaultConnection" ...` oraz `dotnet user-secrets set "OwnerPassword" ...`. Frontend wymaga lokalnych wartości `VITE_S3_ACCESS_KEY` i `VITE_S3_SECRET_KEY` w ignorowanym pliku `.env.local`.

## Układ repozytorium

- `BackEnd/` — API, domena, persystencja i integracje;
- `FrontEnd/` — aplikacja React;
- `db/` — schemat i migracje SQL;
- `docker-compose.yml` — lokalne usługi aplikacji.

## Bezpieczeństwo i prywatność

- Przed udostępnieniem repozytorium **unieważnij i wygeneruj ponownie** wszystkie klucze i hasła, które kiedykolwiek trafiły do historii: klucz API i OAuth Google, hasło aplikacji pocztowej, klucz JWT, klucz haszujący, hasło właściciela oraz lokalne dane dostępowe do usług. Samo usunięcie ich z bieżących plików nie unieważnia starych wartości.
- Pliki `.env*`, klucze prywatne i lokalne dane MinIO są wykluczone z Git. Nie dodawaj danych użytkowników, odpowiedzi ani zrzutów bazy danych do repozytorium.
- **Nie umieszczaj produkcyjnych kluczy w `VITE_*`.** Zmienne Vite trafiają do kodu przeglądarki; w tym projekcie dostęp do MinIO z frontendu jest przeznaczony wyłącznie do lokalnego środowiska demonstracyjnego. Przed wdrożeniem produkcyjnym zastąp go operacją po stronie backendu, np. krótkotrwałymi adresami presigned.
- Projekt jest skonfigurowany do lokalnego developmentu, a nie do bezpośredniego wdrożenia publicznego. Przed takim wdrożeniem wymagany jest przegląd uwierzytelniania, przechowywania haseł, CORS, HTTPS i uprawnień magazynu plików.
- Zgłaszanie problemów bezpieczeństwa: nie publikuj szczegółów w publicznym issue; użyj prywatnego kanału zgłoszeń repozytorium, jeśli jest dostępny. Zobacz [SECURITY.md](./SECURITY.md).

## Licencja

Repozytorium nie zawiera licencji. Publiczna widoczność kodu sama w sobie nie nadaje prawa do jego ponownego wykorzystania; przed publikacją wybierz i dodaj odpowiednią licencję, jeśli chcesz udzielić takich praw.
