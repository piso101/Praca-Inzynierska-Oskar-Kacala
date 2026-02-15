# Wymagania Funkcjonalne Aplikacji Ankietowej

## 1. System Uwierzytelniania i Autoryzacji

### 1.1. Uwierzytelnianie Administratora
- **FR-1.1.1**: System umożliwia logowanie administratora za pomocą adresu email i hasła
- **FR-1.1.2**: System weryfikuje, czy użytkownik ma rolę Administratora przed zalogowaniem
- **FR-1.1.3**: System generuje token JWT po pomyślnym zalogowaniu administratora
- **FR-1.1.4**: System umożliwia utworzenie konta administratora z hasłem właściciela (OwnerPassword)
- **FR-1.1.5**: System sprawdza unikalność adresu email przy tworzeniu konta administratora
- **FR-1.1.6**: System umożliwia pobranie informacji o zalogowanym administratorze (endpoint `/api/auth/me`)
- **FR-1.1.7**: System sprawdza aktywność konta administratora przed zalogowaniem

### 1.2. Uwierzytelnianie Użytkownika Ankiety (Quest)
- **FR-1.2.1**: System umożliwia uwierzytelnianie użytkowników ankiety za pomocą adresu email
- **FR-1.2.2**: System wysyła link weryfikacyjny na adres email użytkownika
- **FR-1.2.3**: System generuje unikalny token weryfikacyjny dla każdego żądania logowania
- **FR-1.2.4**: Token weryfikacyjny wygasa po 1 godzinie
- **FR-1.2.5**: System automatycznie tworzy konto QuestAccount, jeśli użytkownik nie istnieje
- **FR-1.2.6**: System weryfikuje token i generuje token JWT po kliknięciu w link weryfikacyjny
- **FR-1.2.7**: System oznacza email jako zweryfikowany po pomyślnym logowaniu
- **FR-1.2.8**: System umożliwia pobranie informacji o zalogowanym użytkowniku ankiety (endpoint `/api/emailauth/me`)
- **FR-1.2.9**: System sprawdza, czy użytkownik już odpowiedział na ankietę przed wysłaniem emaila

### 1.3. Autoryzacja
- **FR-1.3.1**: System wymaga autoryzacji dla operacji administracyjnych (rola Admin)
- **FR-1.3.2**: System wymaga autoryzacji dla operacji związanych z ankietami (role Admin lub Quest)
- **FR-1.3.3**: System sprawdza uprawnienia użytkownika na podstawie tokenu JWT
- **FR-1.3.4**: System blokuje dostęp do zasobów, jeśli użytkownik nie ma odpowiednich uprawnień

## 2. Zarządzanie Ankietami

### 2.1. Tworzenie Ankiety
- **FR-2.1.1**: System umożliwia administratorowi utworzenie nowej ankiety
- **FR-2.1.2**: System wymaga podania tytułu ankiety (pole obowiązkowe)
- **FR-2.1.3**: System umożliwia podanie opcjonalnego opisu ankiety
- **FR-2.1.4**: System automatycznie przypisuje ankietę do administratora, który ją utworzył
- **FR-2.1.5**: System automatycznie ustawia ankietę jako aktywną przy utworzeniu
- **FR-2.1.6**: System zapisuje datę utworzenia ankiety
- **FR-2.1.7**: System zwraca ID utworzonej ankiety

### 2.2. Przeglądanie Ankiet
- **FR-2.2.1**: System umożliwia przeglądanie listy wszystkich aktywnych ankiet (dostęp publiczny)
- **FR-2.2.2**: System umożliwia przeglądanie szczegółów pojedynczej ankiety (tylko aktywne)
- **FR-2.2.3**: System wyświetla tytuł, opis, datę utworzenia i twórcę ankiety
- **FR-2.2.4**: System wyświetla wszystkie pytania ankiety wraz z elementami
- **FR-2.2.5**: System zwraca błąd, jeśli ankieta nie istnieje
- **FR-2.2.6**: System zwraca błąd, jeśli ankieta nie jest aktywna

### 2.3. Edycja Ankiety
- **FR-2.3.1**: System umożliwia edycję tytułu ankiety
- **FR-2.3.2**: System umożliwia edycję opisu ankiety
- **FR-2.3.3**: System umożliwia edycję ankiety tylko przez jej twórcę
- **FR-2.3.4**: System weryfikuje, czy użytkownik jest twórcą ankiety przed edycją
- **FR-2.3.5**: System wymaga podania tytułu przy aktualizacji

### 2.4. Usuwanie Ankiety
- **FR-2.4.1**: System umożliwia usunięcie ankiety
- **FR-2.4.2**: System umożliwia usunięcie ankiety tylko przez jej twórcę
- **FR-2.4.3**: System weryfikuje, czy użytkownik jest twórcą ankiety przed usunięciem
- **FR-2.4.4**: System zwraca błąd, jeśli ankieta nie istnieje

### 2.5. Kody Zaproszeniowe
- **FR-2.5.1**: System generuje unikalny kod zaproszeniowy dla każdej ankiety
- **FR-2.5.2**: System umożliwia odczytanie ID ankiety na podstawie kodu zaproszeniowego
- **FR-2.5.3**: System zwraca błąd, jeśli kod zaproszeniowy jest nieprawidłowy
- **FR-2.5.4**: System umożliwia udostępnienie ankiety za pomocą linku z kodem zaproszeniowym

## 3. Zarządzanie Pytaniami

### 3.1. Tworzenie Pytania
- **FR-3.1.1**: System umożliwia dodanie pytania do ankiety
- **FR-3.1.2**: System wymaga podania treści pytania (pole obowiązkowe)
- **FR-3.1.3**: System automatycznie przypisuje kolejność pytania (OrderIndex)
- **FR-3.1.4**: System automatycznie generuje unikalne ID dla pytania
- **FR-3.1.5**: System zapisuje datę utworzenia pytania
- **FR-3.1.6**: System weryfikuje, czy ankieta istnieje przed dodaniem pytania
- **FR-3.1.7**: System zwraca ID utworzonego pytania

### 3.2. Przeglądanie Pytań
- **FR-3.2.1**: System umożliwia przeglądanie listy wszystkich pytań w ankiecie
- **FR-3.2.2**: System wyświetla pytania w kolejności zgodnej z OrderIndex
- **FR-3.2.3**: System wyświetla treść pytania i jego elementy

### 3.3. Edycja Pytania
- **FR-3.3.1**: System umożliwia edycję treści pytania
- **FR-3.3.2**: System wymaga podania treści pytania przy aktualizacji
- **FR-3.3.3**: System zwraca błąd, jeśli pytanie nie istnieje

### 3.4. Usuwanie Pytania
- **FR-3.4.1**: System umożliwia usunięcie pytania z ankiety
- **FR-3.4.2**: System zwraca błąd, jeśli pytanie nie istnieje

### 3.5. Zmiana Kolejności Pytań
- **FR-3.5.1**: System umożliwia zmianę kolejności pytań w ankiecie
- **FR-3.5.2**: System aktualizuje OrderIndex dla wszystkich pytań zgodnie z nową kolejnością
- **FR-3.5.3**: System wymaga podania listy pytań z nowymi indeksami kolejności

## 4. Zarządzanie Elementami Pytania

### 4.1. Tworzenie Elementu
- **FR-4.1.1**: System umożliwia dodanie elementu wizualnego do pytania
- **FR-4.1.2**: System wymaga podania pozycji X i Y elementu na płaszczyźnie
- **FR-4.1.3**: System wymaga podania rotacji elementu
- **FR-4.1.4**: System wymaga podania skali elementu
- **FR-4.1.5**: System wymaga podania nazwy SVG elementu
- **FR-4.1.6**: System automatycznie przypisuje kolejność elementu (OrderIndex)
- **FR-4.1.7**: System weryfikuje, czy pytanie istnieje przed dodaniem elementu
- **FR-4.1.8**: System zwraca ID utworzonego elementu

### 4.2. Przeglądanie Elementów
- **FR-4.2.1**: System umożliwia przeglądanie listy wszystkich elementów pytania
- **FR-4.2.2**: System wyświetla elementy w kolejności zgodnej z OrderIndex
- **FR-4.2.3**: System umożliwia pobranie pojedynczego elementu po ID

### 4.3. Edycja Elementu
- **FR-4.3.1**: System umożliwia edycję pozycji elementu (X, Y)
- **FR-4.3.2**: System umożliwia edycję rotacji elementu
- **FR-4.3.3**: System umożliwia edycję skali elementu
- **FR-4.3.4**: System umożliwia edycję kolejności elementu (OrderIndex)
- **FR-4.3.5**: System umożliwia edycję nazwy SVG elementu
- **FR-4.3.6**: System zwraca błąd, jeśli element nie istnieje

### 4.4. Usuwanie Elementu
- **FR-4.4.1**: System umożliwia usunięcie elementu z pytania
- **FR-4.4.2**: System zwraca błąd, jeśli element nie istnieje

### 4.5. Zmiana Kolejności Elementów
- **FR-4.5.1**: System umożliwia zmianę kolejności elementów w pytaniu
- **FR-4.5.2**: System aktualizuje OrderIndex dla wszystkich elementów zgodnie z nową kolejnością
- **FR-4.5.3**: System wymaga podania listy elementów z nowymi indeksami kolejności

## 5. Wypełnianie Ankiet

### 5.1. Dostęp do Ankiety
- **FR-5.1.1**: System umożliwia dostęp do ankiety za pomocą kodu zaproszeniowego
- **FR-5.1.2**: System umożliwia dostęp do ankiety za pomocą bezpośredniego linku
- **FR-5.1.3**: System wyświetla ankietę tylko, jeśli jest aktywna
- **FR-5.1.4**: System wymaga uwierzytelnienia użytkownika przed wypełnieniem ankiety

### 5.2. Wysyłanie Emaila z Linkiem Logowania
- **FR-5.2.1**: System umożliwia wysłanie emaila z linkiem logowania do ankiety
- **FR-5.2.2**: System wymaga podania adresu email
- **FR-5.2.3**: System sprawdza, czy użytkownik już odpowiedział na ankietę przed wysłaniem emaila
- **FR-5.2.4**: System blokuje wysłanie emaila, jeśli użytkownik już odpowiedział na ankietę
- **FR-5.2.5**: Email zawiera tytuł i opis ankiety
- **FR-5.2.6**: Email zawiera link logowania z tokenem weryfikacyjnym
- **FR-5.2.7**: Link logowania przekierowuje do strony ankiety po zalogowaniu

### 5.3. Wyświetlanie Ankiety
- **FR-5.3.1**: System wyświetla wszystkie pytania ankiety w kolejności
- **FR-5.3.2**: System wyświetla elementy wizualne każdego pytania na płaszczyźnie
- **FR-5.3.3**: System umożliwia interakcję z elementami wizualnymi (wybór elementu)

### 5.4. Zapis Odpowiedzi
- **FR-5.4.1**: System umożliwia przesłanie odpowiedzi na ankietę
- **FR-5.4.2**: System wymaga wyboru elementu dla każdego pytania
- **FR-5.4.3**: System zapisuje odpowiedzi użytkownika
- **FR-5.4.4**: System zapisuje datę i czas przesłania odpowiedzi
- **FR-5.4.5**: System przypisuje odpowiedź do konta użytkownika (QuestAccount lub UserAccount)
- **FR-5.4.6**: System sprawdza, czy ankieta jest aktywna przed zapisaniem odpowiedzi
- **FR-5.4.7**: System blokuje wielokrotne odpowiedzi na tę samą ankietę przez tego samego użytkownika
- **FR-5.4.8**: System zwraca błąd, jeśli użytkownik już odpowiedział na ankietę
- **FR-5.4.9**: System zwraca błąd, jeśli nie podano żadnych odpowiedzi

## 6. Wyniki Ankiet

### 6.1. Przeglądanie Wyników
- **FR-6.1.1**: System umożliwia przeglądanie wyników ankiety (tylko dla administratorów)
- **FR-6.1.2**: System wyświetla liczbę wszystkich odpowiedzi na ankietę
- **FR-6.1.3**: System wyświetla wyniki dla każdego pytania osobno
- **FR-6.1.4**: System wyświetla liczbę wyborów dla każdego elementu pytania
- **FR-6.1.5**: System wyświetla procent wyborów dla każdego elementu pytania
- **FR-6.1.6**: System wyświetla elementy wizualne z zaznaczoną liczbą wyborów

### 6.2. Wizualizacja Wyników
- **FR-6.2.1**: System wyświetla wizualizację wyników na płaszczyźnie z elementami
- **FR-6.2.2**: System umożliwia przełączanie między pytaniami w wynikach
- **FR-6.2.3**: System umożliwia wyświetlenie globalnej mapy wszystkich pytań
- **FR-6.2.4**: System wyświetla elementy wizualne z ich pozycjami, rotacjami i skalą
- **FR-6.2.5**: System wyświetla liczbę wyborów dla każdego elementu

### 6.3. Udostępnianie Wyników
- **FR-6.3.1**: System generuje kod zaproszeniowy dla ankiety w wynikach
- **FR-6.3.2**: System generuje link udostępniania dla ankiety
- **FR-6.3.3**: System umożliwia skopiowanie linku udostępniania

## 7. Zarządzanie Kontami Użytkowników

### 7.1. Konta Administratorów
- **FR-7.1.1**: System przechowuje informacje o kontach administratorów
- **FR-7.1.2**: System przechowuje email, hasło (hash), nazwę wyświetlaną i rolę administratora
- **FR-7.1.3**: System przechowuje datę utworzenia konta administratora
- **FR-7.1.4**: System umożliwia dezaktywację konta administratora (IsActive)

### 7.2. Konta Użytkowników Ankiet (Quest)
- **FR-7.2.1**: System automatycznie tworzy konto QuestAccount przy pierwszym logowaniu
- **FR-7.2.2**: System przechowuje email i nazwę wyświetlaną użytkownika
- **FR-7.2.3**: System przechowuje token weryfikacyjny i datę wygaśnięcia
- **FR-7.2.4**: System przechowuje datę weryfikacji emaila
- **FR-7.2.5**: System przechowuje datę utworzenia konta
- **FR-7.2.6**: System umożliwia dezaktywację konta użytkownika (IsActive)

## 8. Integracja Email

### 8.1. Wysyłanie Emaili
- **FR-8.1.1**: System wysyła email z linkiem logowania do ankiety
- **FR-8.1.2**: Email zawiera spersonalizowane powitanie z nazwą użytkownika
- **FR-8.1.3**: Email zawiera tytuł i opis ankiety
- **FR-8.1.4**: Email zawiera link logowania z tokenem weryfikacyjnym
- **FR-8.1.5**: System obsługuje błędy wysyłania emaili

## 9. Bezpieczeństwo

### 9.1. Walidacja Danych
- **FR-9.1.1**: System waliduje wszystkie dane wejściowe przed przetworzeniem
- **FR-9.1.2**: System sprawdza, czy wymagane pola są wypełnione
- **FR-9.1.3**: System sprawdza format adresu email
- **FR-9.1.4**: System sprawdza unikalność adresu email przy tworzeniu konta

### 9.2. Ochrona Zasobów
- **FR-9.2.1**: System chroni operacje administracyjne przed nieautoryzowanym dostępem
- **FR-9.2.2**: System chroni edycję i usuwanie ankiet przed nieautoryzowanym dostępem
- **FR-9.2.3**: System weryfikuje, czy użytkownik jest twórcą ankiety przed edycją/usunięciem
- **FR-9.2.4**: System chroni wyniki ankiet przed nieautoryzowanym dostępem

### 9.3. Ograniczenia Funkcjonalne
- **FR-9.3.1**: System blokuje wielokrotne odpowiedzi na tę samą ankietę przez tego samego użytkownika
- **FR-9.3.2**: System sprawdza aktywność konta przed zalogowaniem
- **FR-9.3.3**: System sprawdza ważność tokenu weryfikacyjnego (1 godzina)

## 10. Interfejs Użytkownika

### 10.1. Strony Publiczne
- **FR-10.1.1**: System wyświetla stronę główną (HomePage)
- **FR-10.1.2**: System wyświetla stronę logowania (LoginPage)
- **FR-10.1.3**: System wyświetla stronę dołączenia do ankiety przez kod zaproszeniowy (JoinSurveyRouter)
- **FR-10.1.4**: System wyświetla stronę weryfikacji emaila (EmailLoginPage)
- **FR-10.1.5**: System wyświetla stronę publicznej ankiety (PublicSurveyPage)
- **FR-10.1.6**: System wyświetla stronę startową ankiety publicznej (PublicSurveyStartPage)

### 10.2. Strony Administracyjne
- **FR-10.2.1**: System wyświetla listę ankiet (FormsPage)
- **FR-10.2.2**: System wyświetla stronę tworzenia ankiety (CreateSurveyPage)
- **FR-10.2.3**: System wyświetla stronę edycji ankiety (EditSurveyPage)
- **FR-10.2.4**: System wyświetla listę pytań ankiety (QuestionsListPage)
- **FR-10.2.5**: System wyświetla stronę tworzenia pytania (CreateQuestionPage)
- **FR-10.2.6**: System wyświetla stronę edycji pytania (EditQuestionPage)
- **FR-10.2.7**: System wyświetla listę elementów pytania (QuestionElementsPage)
- **FR-10.2.8**: System wyświetla stronę tworzenia elementu (CreateQuestionElementPage)
- **FR-10.2.9**: System wyświetla stronę edycji elementu (EditQuestionElementPage)
- **FR-10.2.10**: System wyświetla stronę wyników ankiety (SurveyResultsPage)

### 10.3. Nawigacja
- **FR-10.3.1**: System wyświetla pasek nawigacyjny z linkami
- **FR-10.3.2**: System wyświetla linki w zależności od statusu zalogowania użytkownika
- **FR-10.3.3**: System umożliwia wylogowanie użytkownika
- **FR-10.3.4**: System wyświetla komunikaty toast dla użytkownika

## 11. Przechowywanie Danych

### 11.1. Baza Danych
- **FR-11.1.1**: System przechowuje dane ankiet w bazie danych
- **FR-11.1.2**: System przechowuje dane pytań w bazie danych
- **FR-11.1.3**: System przechowuje dane elementów pytań w bazie danych
- **FR-11.1.4**: System przechowuje dane odpowiedzi w bazie danych
- **FR-11.1.5**: System przechowuje dane kont użytkowników w bazie danych
- **FR-11.1.6**: System przechowuje relacje między ankietami, pytaniami, elementami i odpowiedziami

### 11.2. Przechowywanie Plików
- **FR-11.2.1**: System przechowuje pliki SVG elementów w systemie MinIO
- **FR-11.2.2**: System umożliwia pobranie podpisanych URL do plików SVG

## 12. API i Integracje

### 12.1. REST API
- **FR-12.1.1**: System udostępnia REST API dla wszystkich operacji
- **FR-12.1.2**: System zwraca odpowiednie kody HTTP (200, 201, 400, 401, 403, 404, 500)
- **FR-12.1.3**: System zwraca komunikaty błędów w czytelnej formie
- **FR-12.1.4**: System waliduje dane wejściowe i zwraca błędy walidacji

### 12.2. Health Check
- **FR-12.2.1**: System udostępnia endpoint sprawdzający stan API (tylko dla administratorów)
- **FR-12.2.2**: Endpoint zwraca informację o działaniu API

## 13. Obsługa Błędów

### 13.1. Komunikaty Błędów
- **FR-13.1.1**: System zwraca czytelne komunikaty błędów dla użytkownika
- **FR-13.1.2**: System loguje błędy w systemie logowania
- **FR-13.1.3**: System obsługuje błędy związane z wysyłaniem emaili

### 13.2. Walidacja
- **FR-13.2.1**: System waliduje wszystkie dane wejściowe
- **FR-13.2.2**: System zwraca błędy walidacji przed przetworzeniem danych
- **FR-13.2.3**: System sprawdza istnienie zasobów przed operacjami (ankieta, pytanie, element)

## 14. Wydajność i Skalowalność

### 14.1. Optymalizacja
- **FR-14.1.1**: System optymalizuje zapytania do bazy danych
- **FR-14.1.2**: System ładowuje powiązane dane efektywnie (lazy loading/eager loading)

### 14.2. Cache'owanie
- **FR-14.2.1**: System cache'uje obrazy SVG w przeglądarce
- **FR-14.2.2**: System cache'uje podpisane URL do plików

---

**Uwaga**: Niniejszy dokument zawiera wszystkie zidentyfikowane wymagania funkcjonalne na podstawie analizy kodu źródłowego aplikacji. Wymagania są oznaczone unikalnymi identyfikatorami (FR-X.Y.Z) dla łatwego śledzenia i referencji.
