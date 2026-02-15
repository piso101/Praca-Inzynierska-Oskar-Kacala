## 3.4. Scenariusze Użytkowania Aplikacji

Ta podsekcja przedstawia szczegółowe scenariusze użycia aplikacji ankietowej z perspektywy różnych aktorów systemu. Te scenariusze dostarczają wglądu w to, jak użytkownicy wchodzą w interakcję z systemem, a także jak system zachowuje się w normalnych i wyjątkowych sytuacjach.

## 3.4.1. Scenariusze dla Administratora

### Scenariusz A1: Rejestracja Administratora

* **Aktor**: Administrator  
* **Cel**: Utworzenie konta administracyjnego w systemie  
* **Główny Scenariusz**:

  1. Administrator otwiera formularz rejestracyjny. 2. Wprowadza adres e-mail i hasło. 3. System weryfikuje dane wejściowe. 4. System tworzy konto administratora. 5. Administrator otrzymuje potwierdzenie rejestracji.  
* **Alternatywne Scenariusze**:

  * E-mail już istnieje w systemie.  
  * Dane nie spełniają wymagań walidacyjnych.  

### Scenariusz A2: Logowanie Administratora

* **Aktor**: Administrator  
* **Cel**: Uzyskanie dostępu do panelu administracyjnego  
* **Główny Scenariusz**:

  1. Administrator wprowadza e-mail i hasło. 2. System weryfikuje dane. 3. System generuje token JWT. 4. Administrator jest zalogowany.  
* **Alternatywne Scenariusze**:

  * Nieprawidłowe dane logowania.  
  * Konto jest zablokowane lub nieaktywne.  

### Scenariusz A3: Tworzenie Ankiety

* **Aktor**: Administrator  
* **Cel**: Utworzenie nowej ankiety  
* **Główny Scenariusz**:

  1. Administrator wybiera opcję "Utwórz ankietę". 2. Wprowadza nazwę i opis ankiety. 3. System zapisuje ankietę w bazie danych.  
* **Alternatywne Scenariusze**:

  * Brak wymaganych danych.  

### Scenariusz A4: Edytowanie Ankiety

* **Aktor**: Administrator  
* **Cel**: Modyfikacja istniejącej ankiety  
* **Główny Scenariusz**:

  1. Administrator wybiera ankietę z listy. 2. Wprowadza zmiany. 3. System zapisuje zmodyfikowane dane.  
* **Alternatywne Scenariusze**:

  * Ankieta nie istnieje.  

### Scenariusz A5: Usuwanie Ankiety

* **Aktor**: Administrator  
* **Cel**: Usunięcie ankiety  
* **Główny Scenariusz**:

  1. Administrator wybiera ankietę. 2. Potwierdza operację usunięcia. 3. System usuwa ankietę i powiązane dane.  
* **Alternatywne Scenariusze**:

  * Brak uprawnień.  

### Scenariusz A6: Dodawanie Pytania

* **Aktor**: Administrator  
* **Cel**: Rozszerzenie ankiety o nowe pytania  
* **Główny Scenariusz**:

  1. Administrator wybiera ankietę. 2. Dodaje nowe pytanie. 3. System zapisuje pytanie.  
* **Alternatywne Scenariusze**:

  * Nieprawidłowy typ pytania.  

### Scenariusz A7: Edytowanie Pytania

* **Aktor**: Administrator  
* **Cel**: Aktualizacja treści pytania  
* **Główny Scenariusz**:

  1. Administrator wybiera pytanie. 2. Wprowadza zmiany. 3. System zapisuje zmiany.  
* **Alternatywne Scenariusze**:

  * Pytanie nie istnieje.  

### Scenariusz A8: Usuwanie Pytania

* **Aktor**: Administrator  
* **Cel**: Usunięcie pytania z ankiety  
* **Główny Scenariusz**:

  1. Administrator wybiera pytanie. 2. Potwierdza usunięcie. 3. System usuwa pytanie.  
* **Alternatywne Scenariusze**:

  * Pytanie jest powiązane z odpowiedziami.  

### Scenariusz A9: Zmiana Kolejności Pytań

* **Aktor**: Administrator  
* **Cel**: Organizacja pytań  
* **Główny Scenariusz**:

  1. Administrator przeciąga i upuszcza pytania. 2. System zapisuje nową kolejność.  
* **Alternatywne Scenariusze**:

  * Błąd zapisu kolejności.  

### Scenariusz A10: Zarządzanie Elementami Wizualnymi

* **Aktor**: Administrator  
* **Cel**: Dostosowanie wyglądu ankiety  
* **Główny Scenariusz**:

  1. Administrator dodaje lub edytuje element wizualny. 2. System zapisuje zmiany.  
* **Alternatywne Scenariusze**:

  * Nieobsługiwany format pliku.  

### Scenariusz A11: Generowanie Kodów Zaproszeń

* **Aktor**: Administrator  
* **Cel**: Udostępnienie ankiety użytkownikom  
* **Główny Scenariusz**:

  1. Administrator generuje kod zaproszenia. System zapisuje kod. * **Alternatywne scenariusze**:

  * Przekroczono limit kodu. ### Scenariusz A12: Przegląd wyników ankiety. * **Aktor**: Administrator. * **Cel**: Analiza odpowiedzi. * **Główny scenariusz**:

  1. Administrator wybiera ankietę. System prezentuje wyniki. * **Alternatywne scenariusze**:

  * Brak odpowiedzi. ### Scenariusz A13: Eksportowanie wyników. * **Aktor**: Administrator. * **Cel**: Pobranie danych do analizy zewnętrznej. * **Główny scenariusz**:

  1. Administrator wybiera format eksportu. System generuje plik. * **Alternatywne scenariusze**:

  * Błąd generowania pliku. ### Scenariusz A14: Wylogowanie. * **Aktor**: Administrator. * **Cel**: Zakończenie sesji. * **Główny scenariusz**:

  1. Administrator wybiera opcję wylogowania. System unieważnia token JWT. ---. ## 3.4.2. Scenariusze dla użytkownika ankiety (Quest). ### Scenariusz Q1: Dostęp do ankiety za pomocą kodu zaproszenia. * **Aktor**: Użytkownik ankiety. * **Cel**: Uzyskanie dostępu do ankiety. * **Główny scenariusz**:

  1. Użytkownik wprowadza kod zaproszenia. System weryfikuje kod. Ankieta jest udostępniana. * **Alternatywne scenariusze**:

  * Nieprawidłowy lub wygasły kod. ### Scenariusz Q2: Logowanie za pomocą e-maila. * **Aktor**: Użytkownik ankiety. * **Cel**: Autoryzacja użytkownika. * **Główny scenariusz**:

  1. Użytkownik podaje e-mail. 2. System wysyła link do logowania. 3. Użytkownik uzyskuje dostęp. * **Alternatywne scenariusze**:

  * E-mail nie istnieje. ### Scenariusz Q3: Przeglądanie ankiety. * **Aktor**: Użytkownik ankiety. * **Cel**: Zapoznanie się z ankietą. * **Główny scenariusz**:

  1. System wyświetla pytania. * **Alternatywne scenariusze**:

  * Ankieta nieaktywna. ### Scenariusz Q4: Wypełnianie ankiety. * **Aktor**: Użytkownik ankiety. * **Cel**: Udzielenie odpowiedzi. * **Główny scenariusz**:

  1. Użytkownik udziela odpowiedzi. System zapisuje dane. * **Alternatywne scenariusze**:

  * Brak odpowiedzi na wymagane pytania. ### Scenariusz Q5: Zapisywanie postępu ankiety. * **Aktor**: Użytkownik ankiety. * **Cel**: Kontynuacja później. * **Główny scenariusz**:

  1. System zapisuje częściowe odpowiedzi. * **Alternatywne scenariusze**:

  * Błąd zapisu danych. ### Scenariusz Q6: Zakończenie ankiety. * **Aktor**: Użytkownik ankiety. * **Cel**: Finalizacja odpowiedzi. * **Główny scenariusz**:

  1. Użytkownik przesyła ankietę. 2. System potwierdza zapis. ---. ## 3.4.3. Scenariusze publiczne. ### Scenariusz P1: Przeglądanie listy ankiet. * **Aktor**: Użytkownik publiczny. * **Cel**: Zapoznanie się z dostępnymi ankietami. * **Główny scenariusz**:

  1. System wyświetla listę ankiet. ### Scenariusz P2: Przeglądanie szczegółów ankiety. * **Aktor**: Użytkownik publiczny. * **Cel**: Uzyskanie informacji o ankiecie. * **Główny scenariusz**:

  1. Użytkownik wybiera ankietę. System prezentuje szczegóły. ---. ## 3.4.4. Scenariusze systemowe. ### Scenariusz S1: Automatyczne tworzenie konta. * **Aktor**: System. * **Cel**: Utworzenie konta użytkownika. * **Główny scenariusz**:

  1. System wykrywa nowego użytkownika. 2. System automatycznie tworzy konto. ### Scenariusz S2: Weryfikacja tokenu JWT. * **Aktor**: System. * **Cel**: Zapewnienie bezpieczeństwa. * **Główny scenariusz**:

  1. System sprawdza ważność tokenu. ### Scenariusz S3: Walidacja danych. * **Aktor**: System. * **Cel**: Zapewnienie poprawności danych. * **Główny scenariusz**:

  1. System weryfikuje dane wejściowe. ---. ## 3.4.5. Scenariusze błędów. ### Scenariusz E1: Błąd bazy danych. * **Aktor**: System. * **Cel**: Obsługa awarii. * **Główny scenariusz**:

  1. System wykrywa błąd połączenia. 2. System rejestruje błąd. 3. Użytkownik otrzymuje wiadomość. ### Scenariusz E2: Błąd SMTP. * **Aktor**: System. * **Cel**: Obsługa błędów wysyłania e-maili. * **Główny scenariusz**:

  1. System wykrywa błąd wysyłania. 2. System ponawia próbę lub rejestruje błąd. ### Scenariusz E3: Błąd MinIO. * **Aktor**: System. * **Cel**: Obsługa błędów plików. * **Główny scenariusz**:

  1. System wykrywa błąd odczytu/zapisu pliku. 2. System rejestruje zdarzenie. ---. ## 3.4.6. Diagramy przepływu. Diagramy przepływu mają być zmontowane w celu wsparcia dodatkowych prac projektowych, które będą przedstawiać:

* proces wypełniania ankiety przez użytkownika,  
* proces tworzenia ankiety przez administratora. Te diagramy reprezentują graficzny model działań i interakcji między użytkownikami a systemem.