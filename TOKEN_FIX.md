# 🔧 Naprawiono: Brak Bearer Token w Requestach

## ❌ Problem

Request do `/api/survey/read-survey?surveyId=1` zwracał **401 Unauthorized** bez nagłówka Authorization:

```http
GET /api/survey/read-survey?surveyId=1 HTTP/1.1
Host: localhost:5173
Accept: application/json
❌ BRAK: Authorization: Bearer ...
```

Backend odpowiadał:
```http
HTTP/1.1 401 Unauthorized
www-authenticate: Bearer
```

---

## 🔍 Przyczyna

### Problem z kluczami localStorage

**Mieliśmy DWA różne tokeny w localStorage:**

| Token | Klucz localStorage | Używany przez |
|-------|-------------------|---------------|
| Admin JWT | `'auth_token'` | `getToken()` w `auth.ts` |
| Quest JWT | `'quest_token'` | `getQuestToken()` w `emailAuth.ts` |

**apiFetch używał tylko Admin token:**

```tsx
// http.ts - PRZED:
export async function apiFetch(input: string, init?: RequestInit) {
  const token = getToken();  // ← Tylko 'auth_token'!
  // ...
  if (token) {
    headers.set('Authorization', `Bearer ${token}`);
  }
}
```

**Gdy Quest użytkownik próbował pobrać Survey:**
```
1. User zalogowany jako Quest (ma 'quest_token')
2. apiFetch wywołuje getToken() → szuka 'auth_token' → null ❌
3. Request wysłany BEZ Authorization header
4. Backend: 401 Unauthorized
```

---

## ✅ Rozwiązanie

### Zaktualizowano apiFetch żeby sprawdzał OBA tokeny

```tsx
// http.ts - PO:
import { getToken } from './auth';
import { getQuestToken } from './emailAuth';  // ← NOWE!

export async function apiFetch(input: string, init?: RequestInit) {
  // Spróbuj najpierw admin token, potem quest token
  const token = getToken() || getQuestToken();  // ← NAPRAWIONE!
  
  const headers = new Headers(init?.headers || {});
  headers.set('Accept', 'application/json');
  
  if (token && !headers.has('Authorization')) {
    headers.set('Authorization', `Bearer ${token}`);
  }
  
  const url = input.startsWith('http') ? input : `${API_BASE}${input}`;
  return fetch(url, { ...init, headers });
}
```

---

## 🎯 Jak to działa teraz

### Scenariusz 1: Admin zalogowany

```
1. getToken() → 'eyJhbGc...' (admin JWT) ✅
2. getQuestToken() nie wykonuje się (|| short-circuit)
3. Authorization: Bearer eyJhbGc... (admin token)
```

### Scenariusz 2: Quest użytkownik zalogowany

```
1. getToken() → null (brak admin token)
2. getQuestToken() → 'eyJhbGc...' (quest JWT) ✅
3. Authorization: Bearer eyJhbGc... (quest token)
```

### Scenariusz 3: Nikt nie zalogowany

```
1. getToken() → null
2. getQuestToken() → null
3. Request BEZ Authorization header (dla publicznych endpointów)
```

---

## 📊 Przed vs Po

### Przed (401 Unauthorized):

```http
GET /api/survey/read-survey?surveyId=1
Accept: application/json
❌ Authorization: (brak)

↓

401 Unauthorized
www-authenticate: Bearer
```

### Po (200 OK):

```http
GET /api/survey/read-survey?surveyId=1
Accept: application/json
✅ Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

↓

200 OK
{
  "id": 1,
  "title": "Survey Title",
  "questions": [...]
}
```

---

## 🧪 Test

### Test 1: Quest użytkownik pobiera Survey

```bash
# 1. Zaloguj się jako Quest (przez email)
# localStorage będzie miał: quest_token = "..."

# 2. Otwórz DevTools → Network tab

# 3. Przejdź do /public/survey/1/start

# 4. Sprawdź request do /api/survey/read-survey
# Powinien mieć nagłówek:
# Authorization: Bearer eyJhbGc...
```

**Oczekiwany rezultat:**
- ✅ Status: 200 OK
- ✅ Authorization header obecny
- ✅ Survey załadowany

### Test 2: Admin pobiera Survey

```bash
# 1. Zaloguj się jako Admin (/login)
# localStorage będzie miał: auth_token = "..."

# 2. Przejdź do /forms/1/questions

# 3. Sprawdź requesty
# Powinny mieć Authorization z admin tokenem
```

**Oczekiwany rezultat:**
- ✅ Admin requesty działają
- ✅ Nie wpływa na Quest flow

---

## 🔄 Alternatywne Rozwiązania (NIE zastosowane)

### Opcja 1: Jeden klucz dla obu tokenów ❌

```tsx
// Użyj tego samego klucza 'auth_token' dla obu
saveQuestToken(token) {
  localStorage.setItem('auth_token', token);  // ← Konflikt!
}
```

**Wady:**
- ❌ Nie można być zalogowanym jako Admin i Quest jednocześnie
- ❌ Logout Admina wyloguje Quest
- ❌ Trudne do debugowania

### Opcja 2: Osobny apiQuestFetch ❌

```tsx
// Stwórz nową funkcję
export async function apiQuestFetch(input: string, init?: RequestInit) {
  const token = getQuestToken();
  // ...
}

// Użyj w questions.ts
const res = await apiQuestFetch('/survey/read-survey?surveyId=1');
```

**Wady:**
- ❌ Duplikacja kodu
- ❌ Trzeba pamiętać której funkcji użyć
- ❌ Łatwo popełnić błąd

### Opcja 3: Fallback na oba tokeny ✅ (WYBRANE)

```tsx
const token = getToken() || getQuestToken();
```

**Zalety:**
- ✅ Działa dla Admin i Quest
- ✅ Jeden apiFetch dla wszystkiego
- ✅ Automatyczne fallback
- ✅ Łatwe w utrzymaniu

---

## 📝 Wpływ na Kod

### Pliki zmienione:
- ✅ `FrontEnd/src/services/http.ts` - dodano fallback na quest token

### Pliki NIE zmienione:
- ✅ `questions.ts` - nadal używa apiFetch (działa automatycznie)
- ✅ `surveys.ts` - nadal używa apiFetch
- ✅ `auth.ts` - bez zmian
- ✅ `emailAuth.ts` - bez zmian

### Backward compatible:
- ✅ Istniejący kod Admin działa bez zmian
- ✅ Istniejący kod Quest działa bez zmian
- ✅ Nowe funkcje automatycznie wspierają oba tokeny

---

## 🎓 Best Practices

### Kiedy używać apiFetch?

**✅ Zawsze używaj apiFetch dla:**
- Requestów do `/api/*` endpointów
- Requestów wymagających autoryzacji
- Standardowych CRUD operacji

```tsx
// ✅ Dobre
const res = await apiFetch('/survey/read-survey?surveyId=1');

// ❌ Źle (brak tokenu)
const res = await fetch('/api/survey/read-survey?surveyId=1');
```

### Kiedy używać fetch bezpośrednio?

**✅ Tylko dla:**
- Publicznych endpointów nie wymagających auth
- External API (poza twoim backendem)
- Specjalnych przypadków (custom headers)

```tsx
// ✅ OK - publiczny endpoint
const res = await fetch('/open-survey?inviteCode=xxx');

// ✅ OK - external API
const res = await fetch('https://api.github.com/users');
```

---

## 🔐 Security Notes

### Token Priority

**Admin token ma priorytet:**
```tsx
const token = getToken() || getQuestToken();
         //    ↑ sprawdzany pierwszy
```

Jeśli ktoś ma oba tokeny (teoretycznie):
- Admin token będzie użyty
- Quest token będzie ignorowany

**To jest OK** ponieważ:
- ✅ Admin ma więcej uprawnień
- ✅ Admin może wykonać wszystko co Quest
- ✅ Backend weryfikuje role w JWT

### Token Expiration

**Oba tokeny wygasają po 12h:**
```csharp
// Backend: JwtTokenService.cs
expires: DateTime.UtcNow.AddHours(12)
```

**Brak automatycznego refresh:**
- ❌ Obecnie nie ma refresh token logic
- ⚠️ Po 12h użytkownik musi się zalogować ponownie

**Future improvement:**
- Dodać refresh tokens
- Auto-refresh przed wygaśnięciem
- Sliding expiration

---

## ✅ Status

- [x] Zidentyfikowano problem (brak Bearer token)
- [x] Zrozumiano przyczynę (dwa osobne klucze localStorage)
- [x] Zaimplementowano rozwiązanie (fallback w apiFetch)
- [x] Przetestowano lokalnie
- [x] Backward compatible z existing code

**Quest users mogą teraz pobierać Survey! ✅**

---

## 🔗 Powiązane Pliki

- `FrontEnd/src/services/http.ts` - apiFetch z fallback
- `FrontEnd/src/services/auth.ts` - getToken() dla Admin
- `FrontEnd/src/services/emailAuth.ts` - getQuestToken() dla Quest
- `FrontEnd/src/services/questions.ts` - używa apiFetch
- `BackEnd/Presentation/Controllers/SurveyController.cs` - wymaga autoryzacji

---

## 🎉 Gotowe!

System autoryzacji działa teraz poprawnie dla:
- ✅ Admin users (auth_token)
- ✅ Quest users (quest_token)
- ✅ Automatyczny fallback w apiFetch
- ✅ Wszystkie API calls mają Bearer token

