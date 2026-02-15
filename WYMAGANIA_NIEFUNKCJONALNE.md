# Wymagania Niefunkcjonalne Aplikacji Ankietowej

## 1. Wymagania Wydajnościowe

### 1.1. Czas Odpowiedzi
- **NFR-1.1.1**: System powinien odpowiadać na żądania API w czasie nieprzekraczającym 500ms dla 95% żądań
- **NFR-1.1.2**: Operacje odczytu danych (GET) powinny być wykonywane w czasie nieprzekraczającym 300ms
- **NFR-1.1.3**: Operacje zapisu danych (POST/PUT/DELETE) powinny być wykonywane w czasie nieprzekraczającym 1000ms
- **NFR-1.1.4**: Wysyłanie emaila z linkiem logowania powinno być wykonane asynchronicznie, nie blokując odpowiedzi API
- **NFR-1.1.5**: Ładowanie strony ankiety z pytaniami i elementami powinno zakończyć się w czasie nieprzekraczającym 2 sekund

### 1.2. Przepustowość
- **NFR-1.2.1**: System powinien obsługiwać minimum 100 jednoczesnych użytkowników
- **NFR-1.2.2**: System powinien obsługiwać minimum 1000 żądań na minutę
- **NFR-1.2.3**: System powinien obsługiwać minimum 100 jednoczesnych wypełnień ankiet
- **NFR-1.2.4**: System powinien obsługiwać minimum 50 jednoczesnych operacji administracyjnych

### 1.3. Optymalizacja
- **NFR-1.3.1**: System powinien wykorzystywać lazy loading dla powiązanych danych w Entity Framework Core
- **NFR-1.3.2**: System powinien cache'ować obrazy SVG w przeglądarce użytkownika
- **NFR-1.3.3**: System powinien cache'ować podpisane URL do plików w MinIO
- **NFR-1.3.4**: System powinien optymalizować zapytania do bazy danych, unikając N+1 problem
- **NFR-1.3.5**: System powinien wykorzystywać indeksy w bazie danych dla często wyszukiwanych pól (email, surveyId, questionId)

## 2. Wymagania Bezpieczeństwa

### 2.1. Uwierzytelnianie i Autoryzacja
- **NFR-2.1.1**: System powinien wykorzystywać JWT (JSON Web Tokens) do uwierzytelniania użytkowników
- **NFR-2.1.2**: Tokeny JWT powinny być podpisywane algorytmem HMAC SHA-256
- **NFR-2.1.3**: Tokeny JWT dla administratorów powinny wygasać po 12 godzinach
- **NFR-2.1.4**: Tokeny JWT dla użytkowników ankiety (Quest) powinny wygasać po 12 godzinach
- **NFR-2.1.5**: Tokeny weryfikacyjne email powinny wygasać po 1 godzinie
- **NFR-2.1.6**: System powinien walidować czas życia tokenu bez tolerancji czasowej (ClockSkew = 0)
- **NFR-2.1.7**: System powinien implementować autoryzację opartą na rolach (Role-Based Access Control - RBAC)
- **NFR-2.1.8**: System powinien wymagać autoryzacji dla wszystkich operacji administracyjnych

### 2.2. Ochrona Danych
- **NFR-2.2.1**: System powinien przechowywać hasła w formie zahashowanej (obecnie uproszczone, wymagane PBKDF2/BCrypt)
- **NFR-2.2.2**: System powinien walidować wszystkie dane wejściowe przed przetworzeniem
- **NFR-2.2.3**: System powinien chronić przed atakami SQL Injection poprzez użycie parametrówzowanych zapytań (Entity Framework Core)
- **NFR-2.2.4**: System powinien chronić przed atakami XSS (Cross-Site Scripting) poprzez walidację i sanitizację danych
- **NFR-2.2.5**: System powinien implementować CORS (Cross-Origin Resource Sharing) z ograniczonymi dozwolonymi źródłami
- **NFR-2.2.6**: System powinien przechowywać wrażliwe dane konfiguracyjne (klucze JWT, hasła) w zmiennych środowiskowych lub secure storage
- **NFR-2.2.7**: System powinien logować próby nieautoryzowanego dostępu

### 2.3. Bezpieczeństwo Komunikacji
- **NFR-2.3.1**: System powinien wspierać HTTPS w środowisku produkcyjnym
- **NFR-2.3.2**: System powinien wymagać bezpiecznego połączenia dla operacji uwierzytelniania w środowisku produkcyjnym
- **NFR-2.3.3**: System powinien szyfrować dane przesyłane między frontendem a backendem

### 2.4. Bezpieczeństwo Email
- **NFR-2.4.1**: System powinien wykorzystywać bezpieczne połączenie SMTP (port 587 z TLS)
- **NFR-2.4.2**: System powinien przechowywać hasła aplikacji email w bezpieczny sposób
- **NFR-2.4.3**: System powinien walidować adresy email przed wysłaniem wiadomości

## 3. Wymagania Niezawodności

### 3.1. Dostępność
- **NFR-3.1.1**: System powinien mieć dostępność na poziomie minimum 99% (dopuszczalne 7.2 godziny przestoju miesięcznie)
- **NFR-3.1.2**: System powinien automatycznie odtwarzać się po awarii (restart policy w Docker)
- **NFR-3.1.3**: System powinien obsługiwać błędy połączenia z bazą danych i próbować ponownego połączenia

### 3.2. Obsługa Błędów
- **NFR-3.2.1**: System powinien zwracać odpowiednie kody HTTP dla różnych typów błędów (400, 401, 403, 404, 500)
- **NFR-3.2.2**: System powinien zwracać czytelne komunikaty błędów dla użytkowników końcowych
- **NFR-3.2.3**: System powinien logować szczegółowe informacje o błędach dla administratorów
- **NFR-3.2.4**: System powinien obsługiwać błędy związane z wysyłaniem emaili bez przerywania działania aplikacji
- **NFR-3.2.5**: System powinien walidować dane przed zapisem do bazy danych
- **NFR-3.2.6**: System powinien wykorzystywać transakcje bazodanowe dla operacji wymagających spójności danych

### 3.3. Odporność na Błędy
- **NFR-3.3.1**: System powinien obsługiwać sytuacje, gdy baza danych jest tymczasowo niedostępna
- **NFR-3.3.2**: System powinien obsługiwać sytuacje, gdy MinIO jest tymczasowo niedostępny
- **NFR-3.3.3**: System powinien obsługiwać sytuacje, gdy serwer SMTP jest tymczasowo niedostępny
- **NFR-3.3.4**: System powinien zapobiegać utracie danych przy awariach poprzez wykorzystanie transakcji

## 4. Wymagania Skalowalności

### 4.1. Skalowanie Poziome
- **NFR-4.1.1**: System powinien być zaprojektowany do skalowania poziomego (dodawanie instancji backendu)
- **NFR-4.1.2**: System powinien wykorzystywać konteneryzację (Docker) dla łatwego skalowania
- **NFR-4.1.3**: System powinien obsługiwać wiele instancji frontendu
- **NFR-4.1.4**: System powinien wykorzystywać zewnętrzną bazę danych (PostgreSQL), która może być skalowana niezależnie

### 4.2. Skalowanie Pionowe
- **NFR-4.2.1**: System powinien obsługiwać zwiększanie zasobów (CPU, RAM) dla kontenerów
- **NFR-4.2.2**: System powinien efektywnie wykorzystywać zasoby serwera

### 4.3. Skalowanie Danych
- **NFR-4.3.1**: System powinien obsługiwać minimum 10 000 ankiet w bazie danych
- **NFR-4.3.2**: System powinien obsługiwać minimum 100 000 odpowiedzi na ankiety
- **NFR-4.3.3**: System powinien obsługiwać minimum 1 000 000 elementów pytań
- **NFR-4.3.4**: System powinien wykorzystywać partycjonowanie danych dla dużych tabel w przyszłości

## 5. Wymagania Utrzymywalności

### 5.1. Architektura
- **NFR-5.1.1**: System powinien być zbudowany zgodnie z zasadami Clean Architecture (warstwy: Domain, Application, Infrastructure, Presentation)
- **NFR-5.1.2**: System powinien wykorzystywać wzorzec Repository dla dostępu do danych
- **NFR-5.1.3**: System powinien wykorzystywać Dependency Injection dla luźnego sprzężenia komponentów
- **NFR-5.1.4**: System powinien separować logikę biznesową od warstwy prezentacji
- **NFR-5.1.5**: System powinien wykorzystywać DTO (Data Transfer Objects) do transferu danych między warstwami

### 5.2. Kod
- **NFR-5.2.1**: Kod powinien być napisany w języku C# (.NET 8.0) dla backendu
- **NFR-5.2.2**: Kod frontendu powinien być napisany w TypeScript z React 18
- **NFR-5.2.3**: Kod powinien być zgodny z konwencjami nazewnictwa i stylem kodowania
- **NFR-5.2.4**: Kod powinien być podzielony na moduły i komponenty o pojedynczej odpowiedzialności
- **NFR-5.2.5**: Kod powinien zawierać komentarze dla złożonych fragmentów logiki biznesowej

### 5.3. Testowanie
- **NFR-5.3.1**: System powinien być testowalny poprzez wykorzystanie Dependency Injection
- **NFR-5.3.2**: Komponenty powinny być możliwe do testowania jednostkowego
- **NFR-5.3.3**: System powinien wspierać testy integracyjne dla API

## 6. Wymagania Kompatybilności

### 6.1. Platforma Backend
- **NFR-6.1.1**: System backend powinien działać na .NET 8.0
- **NFR-6.1.2**: System backend powinien działać na platformie Linux (kontenery Docker)
- **NFR-6.1.3**: System backend powinien działać na platformie Windows (rozwój lokalny)
- **NFR-6.1.4**: System backend powinien wykorzystywać ASP.NET Core 8.0

### 6.2. Platforma Frontend
- **NFR-6.2.1**: System frontend powinien działać w Node.js 20 lub nowszym
- **NFR-6.2.2**: System frontend powinien wykorzystywać React 18.3 lub nowszy
- **NFR-6.2.3**: System frontend powinien wykorzystywać TypeScript 5.6 lub nowszy
- **NFR-6.2.4**: System frontend powinien wykorzystywać Vite 5.4 lub nowszy do budowania

### 6.3. Baza Danych
- **NFR-6.3.1**: System powinien wykorzystywać PostgreSQL 15 lub nowszy
- **NFR-6.3.2**: System powinien wykorzystywać Entity Framework Core 9.0
- **NFR-6.3.3**: System powinien wykorzystywać Npgsql.EntityFrameworkCore.PostgreSQL 9.0.1

### 6.4. Przechowywanie Plików
- **NFR-6.4.1**: System powinien wykorzystywać MinIO (S3-compatible storage)
- **NFR-6.4.2**: System powinien wykorzystywać AWS SDK dla .NET do komunikacji z MinIO
- **NFR-6.4.3**: System powinien wykorzystywać pre-signed URLs dla bezpiecznego dostępu do plików

### 6.5. Przeglądarki
- **NFR-6.5.1**: System powinien działać w przeglądarkach: Chrome (ostatnie 2 wersje), Firefox (ostatnie 2 wersje), Edge (ostatnie 2 wersje), Safari (ostatnie 2 wersje)
- **NFR-6.5.2**: System powinien wykorzystywać nowoczesne API przeglądarki (Canvas API, Fetch API)
- **NFR-6.5.3**: System powinien wspierać urządzenia mobilne (responsive design)

## 7. Wymagania Użyteczności

### 7.1. Interfejs Użytkownika
- **NFR-7.1.1**: Interfejs użytkownika powinien być intuicyjny i łatwy w użyciu
- **NFR-7.1.2**: Interfejs użytkownika powinien być responsywny i działać na urządzeniach mobilnych
- **NFR-7.1.3**: Interfejs użytkownika powinien wyświetlać komunikaty o błędach w czytelnej formie
- **NFR-7.1.4**: Interfejs użytkownika powinien wyświetlać komunikaty o sukcesie dla operacji
- **NFR-7.1.5**: Interfejs użytkownika powinien wykorzystywać wizualizację canvas (Konva) dla elementów ankiet
- **NFR-7.1.6**: Interfejs użytkownika powinien wspierać drag and drop dla zmiany kolejności elementów

### 7.2. Doświadczenie Użytkownika
- **NFR-7.2.1**: System powinien wyświetlać wskaźniki ładowania dla długotrwałych operacji
- **NFR-7.2.2**: System powinien wyświetlać komunikaty toast dla informacji zwrotnej użytkownika
- **NFR-7.2.3**: System powinien automatycznie przekierowywać użytkownika po zalogowaniu
- **NFR-7.2.4**: System powinien zapamiętywać stan użytkownika (token w localStorage)

## 8. Wymagania Dostępności

### 8.1. Standardy Dostępności
- **NFR-8.1.1**: System powinien wspierać podstawowe wymagania dostępności (WCAG 2.1 Level A)
- **NFR-8.1.2**: System powinien wykorzystywać semantyczne znaczniki HTML
- **NFR-8.1.3**: System powinien zapewniać odpowiedni kontrast kolorów dla tekstu

### 8.2. Obsługa Klawiatury
- **NFR-8.2.1**: System powinien umożliwiać nawigację za pomocą klawiatury
- **NFR-8.2.2**: System powinien wyświetlać focus indicators dla elementów interaktywnych

## 9. Wymagania Przenośności

### 9.1. Konteneryzacja
- **NFR-9.1.1**: System powinien być konteneryzowany za pomocą Docker
- **NFR-9.1.2**: System powinien wykorzystywać docker-compose do orkiestracji kontenerów
- **NFR-9.1.3**: System powinien działać na dowolnej platformie obsługującej Docker (Linux, Windows, macOS)

### 9.2. Konfiguracja
- **NFR-9.2.1**: System powinien wykorzystywać zmienne środowiskowe do konfiguracji
- **NFR-9.2.2**: System powinien wspierać różne środowiska (Development, Production)
- **NFR-9.2.3**: System powinien wykorzystywać pliki appsettings.json dla konfiguracji .NET

## 10. Wymagania Dokumentacji

### 10.1. Dokumentacja API
- **NFR-10.1.1**: System powinien udostępniać dokumentację API poprzez Swagger/OpenAPI
- **NFR-10.1.2**: Dokumentacja API powinna zawierać opisy wszystkich endpointów
- **NFR-10.1.3**: Dokumentacja API powinna zawierać przykłady żądań i odpowiedzi
- **NFR-10.1.4**: Dokumentacja API powinna zawierać informacje o wymaganej autoryzacji

### 10.2. Dokumentacja Kodu
- **NFR-10.2.1**: Kod powinien zawierać komentarze XML dla publicznych API
- **NFR-10.2.2**: Kompleksowe fragmenty logiki powinny być udokumentowane

## 11. Wymagania Monitoringu i Logowania

### 11.1. Logowanie
- **NFR-11.1.1**: System powinien logować wszystkie operacje administracyjne
- **NFR-11.1.2**: System powinien logować błędy z odpowiednim poziomem szczegółowości
- **NFR-11.1.3**: System powinien wykorzystywać strukturalne logowanie (ILogger w .NET)
- **NFR-11.1.4**: System powinien logować próby nieautoryzowanego dostępu
- **NFR-11.1.5**: System powinien logować operacje związane z uwierzytelnianiem

### 11.2. Poziomy Logowania
- **NFR-11.2.1**: System powinien wykorzystywać różne poziomy logowania (Information, Warning, Error)
- **NFR-11.2.2**: System powinien logować informacje na poziomie Information dla operacji biznesowych
- **NFR-11.2.3**: System powinien logować ostrzeżenia na poziomie Warning dla potencjalnych problemów
- **NFR-11.2.4**: System powinien logować błędy na poziomie Error dla wyjątków

### 11.3. Health Checks
- **NFR-11.3.1**: System powinien udostępniać endpoint health check dla monitorowania stanu
- **NFR-11.3.2**: Health check powinien być dostępny tylko dla autoryzowanych użytkowników (Admin)

## 12. Wymagania Konfiguracyjne

### 12.1. Konfiguracja Aplikacji
- **NFR-12.1.1**: System powinien wykorzystywać pliki konfiguracyjne (appsettings.json) dla ustawień
- **NFR-12.1.2**: System powinien wspierać różne konfiguracje dla różnych środowisk
- **NFR-12.1.3**: System powinien wykorzystywać User Secrets dla lokalnego rozwoju
- **NFR-12.1.4**: System powinien wykorzystywać zmienne środowiskowe dla wrażliwych danych

### 12.2. Konfiguracja Bazy Danych
- **NFR-12.2.1**: System powinien konfigurować połączenie z bazą danych przez connection string
- **NFR-12.2.2**: System powinien wspierać migracje bazy danych (Entity Framework Migrations)

### 12.3. Konfiguracja Email
- **NFR-12.3.1**: System powinien konfigurować serwer SMTP przez plik konfiguracyjny
- **NFR-12.3.2**: System powinien wspierać konfigurację Gmail SMTP
- **NFR-12.3.3**: System powinien wykorzystywać MailKit do wysyłania emaili

## 13. Wymagania Integracyjne

### 13.1. Integracja z Zewnętrznymi Serwisami
- **NFR-13.1.1**: System powinien integrować się z serwerem SMTP (Gmail) do wysyłania emaili
- **NFR-13.1.2**: System powinien integrować się z MinIO (S3-compatible) do przechowywania plików
- **NFR-13.1.3**: System powinien wykorzystywać Google APIs (opcjonalnie) dla dodatkowych funkcji

### 13.2. API Integracja
- **NFR-13.2.1**: System powinien udostępniać RESTful API
- **NFR-13.2.2**: System powinien wykorzystywać standardowe metody HTTP (GET, POST, PUT, DELETE)
- **NFR-13.2.3**: System powinien zwracać dane w formacie JSON

## 14. Wymagania Rozwojowe

### 14.1. Środowisko Deweloperskie
- **NFR-14.1.1**: System powinien wspierać hot reload dla backendu (.NET watch)
- **NFR-14.1.2**: System powinien wspierać hot reload dla frontendu (Vite HMR)
- **NFR-14.1.3**: System powinien wykorzystywać Docker Compose dla lokalnego środowiska deweloperskiego

### 14.2. Narzędzia Deweloperskie
- **NFR-14.2.1**: System powinien wykorzystywać Swagger UI do testowania API
- **NFR-14.2.2**: System powinien wspierać debugowanie w IDE (Visual Studio, VS Code)
- **NFR-14.2.3**: System powinien wykorzystywać TypeScript dla type safety w frontendzie

## 15. Wymagania Jakościowe

### 15.1. Jakość Kodu
- **NFR-15.1.1**: Kod powinien być zgodny z najlepszymi praktykami dla .NET i React
- **NFR-15.1.2**: Kod powinien być wolny od ostrzeżeń kompilatora
- **NFR-15.1.3**: Kod powinien wykorzystywać nullable reference types w C#

### 15.2. Jakość Danych
- **NFR-15.2.1**: System powinien zapewniać spójność danych w bazie danych
- **NFR-15.2.2**: System powinien wykorzystywać ograniczenia bazy danych (constraints) dla integralności danych
- **NFR-15.2.3**: System powinien walidować dane przed zapisem do bazy danych

## 16. Wymagania Operacyjne

### 16.1. Deployment
- **NFR-16.1.1**: System powinien być łatwy do wdrożenia za pomocą Docker
- **NFR-16.1.2**: System powinien wspierać wdrożenie w środowisku produkcyjnym
- **NFR-16.1.3**: System powinien wykorzystywać multi-stage builds w Dockerfile dla optymalizacji obrazów

### 16.2. Backup i Recovery
- **NFR-16.2.1**: System powinien wspierać backup bazy danych PostgreSQL
- **NFR-16.2.2**: System powinien wspierać backup danych w MinIO
- **NFR-16.2.3**: System powinien wspierać przywracanie danych z backupu

### 16.3. Zarządzanie Wersjami
- **NFR-16.3.1**: System powinien wykorzystywać migracje bazy danych dla zarządzania schematem
- **NFR-16.3.2**: System powinien wspierać wersjonowanie API (przygotowanie na przyszłość)

---

**Uwaga**: Niniejszy dokument zawiera wszystkie zidentyfikowane wymagania niefunkcjonalne na podstawie analizy kodu źródłowego, konfiguracji i architektury aplikacji. Wymagania są oznaczone unikalnymi identyfikatorami (NFR-X.Y.Z) dla łatwego śledzenia i referencji.

**Legenda skrótów:**
- NFR - Non-Functional Requirement (Wymaganie Niefunkcjonalne)
- JWT - JSON Web Token
- RBAC - Role-Based Access Control
- CORS - Cross-Origin Resource Sharing
- XSS - Cross-Site Scripting
- SQL Injection - Structured Query Language Injection
- HTTPS - HyperText Transfer Protocol Secure
- SMTP - Simple Mail Transfer Protocol
- TLS - Transport Layer Security
- WCAG - Web Content Accessibility Guidelines
- API - Application Programming Interface
- REST - Representational State Transfer
- JSON - JavaScript Object Notation
- HMR - Hot Module Replacement
- CPU - Central Processing Unit
- RAM - Random Access Memory
