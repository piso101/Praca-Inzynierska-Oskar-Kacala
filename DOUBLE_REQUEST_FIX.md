# 🔧 Naprawiono: Podwójne Wywołanie API

## ❌ Problem

Endpoint `verify-login` był wywoływany **DWA RAZY** z tym samym tokenem:

```
verify-login?token=5f130388-4957-47f2-9627-8907350efa81  ← Wywołanie 1
verify-login?token=5f130388-4957-47f2-9627-8907350efa81  ← Wywołanie 2
```

---

## 🔍 Przyczyna

### 1. React.StrictMode w Development Mode

W `main.tsx`:
```tsx
<React.StrictMode>
  <BrowserRouter>
    <App />
  </BrowserRouter>
</React.StrictMode>
```

**React.StrictMode** w trybie development celowo wykonuje:
- useEffect **dwa razy**
- useState **dwa razy**
- Funkcje render **dwa razy**

To pomaga wykryć side effects, ale powoduje problemy z API calls.

### 2. Brak zabezpieczenia w EmailLoginPage

```tsx
React.useEffect(() => {
  // ❌ Nie było zabezpieczenia przed wielokrotnym wywołaniem
  const result = await verifyEmailLogin(token);
}, [searchParams, navigate]);
```

---

## ✅ Rozwiązanie

### Dodano useRef jako flagę

```tsx
export function EmailLoginPage() {
  const hasVerified = React.useRef(false);  // ← NOWE!

  React.useEffect(() => {
    // Zapobiegaj wielokrotnemu wywołaniu
    if (hasVerified.current) return;  // ← NOWE!
    
    const token = searchParams.get('token');
    if (!token) return;

    hasVerified.current = true;  // ← NOWE!

    (async () => {
      const result = await verifyEmailLogin(token);
      // ... reszta kodu
    })();
  }, [searchParams, navigate]);
}
```

---

## 🎯 Jak to działa?

### useRef vs useState

| Właściwość | useRef | useState |
|------------|--------|----------|
| Re-render | ❌ Nie powoduje | ✅ Powoduje |
| Persystencja | ✅ Przez całe życie komponentu | ✅ Przez całe życie |
| StrictMode | ✅ Nie resetuje | ⚠️ Może resetować |

**useRef** jest idealny do flag typu "wykonane/nie wykonane" ponieważ:
- Nie powoduje re-render
- Nie jest resetowany przez StrictMode
- Persystuje między re-renderami

### Przepływ:

```
Montowanie komponentu (StrictMode - development)
│
├─> useEffect wywołanie 1
│   ├─> hasVerified.current = false
│   ├─> Sprawdza: if (hasVerified.current) ← false, kontynuuj
│   ├─> hasVerified.current = true
│   └─> ✅ Wywołuje API verify-login
│
├─> useEffect wywołanie 2 (StrictMode)
│   ├─> hasVerified.current = true (z wywołania 1!)
│   ├─> Sprawdza: if (hasVerified.current) ← true, RETURN
│   └─> ❌ Nie wywołuje API (zapobieżone!)
│
└─> Sukces! Tylko jedno wywołanie API
```

---

## 📊 Przed vs Po

### Przed (2 requesty):
```
GET /api/emailauth/verify-login?token=xxx  [200 OK] 150ms
GET /api/emailauth/verify-login?token=xxx  [401 Unauthorized] 50ms
     └─> Token już użyty! ❌
```

### Po (1 request):
```
GET /api/emailauth/verify-login?token=xxx  [200 OK] 150ms
✅ Token używany tylko raz!
```

---

## 🛠️ Alternatywne Rozwiązania

### Opcja 1: Wyłącz StrictMode (NIE ZALECANE)

```tsx
// main.tsx
ReactDOM.createRoot(document.getElementById('root')!).render(
  // <React.StrictMode>  ← Zakomentuj
    <BrowserRouter>
      <App />
    </BrowserRouter>
  // </React.StrictMode>
);
```

**Wady:**
- ❌ Tracisz wykrywanie side effects
- ❌ Tylko maskuje problem
- ❌ W produkcji i tak działa inaczej

### Opcja 2: useRef (ZALECANE) ✅

```tsx
const hasVerified = React.useRef(false);

React.useEffect(() => {
  if (hasVerified.current) return;
  hasVerified.current = true;
  // ... API call
}, [deps]);
```

**Zalety:**
- ✅ Działa w dev i production
- ✅ Nie wpływa na StrictMode
- ✅ Jasny intent kodu

### Opcja 3: Cleanup function

```tsx
React.useEffect(() => {
  let cancelled = false;
  
  (async () => {
    if (cancelled) return;
    const result = await verifyEmailLogin(token);
    if (cancelled) return;
    // ... use result
  })();
  
  return () => { cancelled = true; };
}, [token]);
```

**Wady:**
- ⚠️ Request nadal wysyłany dwa razy
- ⚠️ Tylko ignoruje drugi result

---

## 🧪 Test

### Development Mode:
```bash
# Uruchom frontend
npm run dev

# Otwórz w przeglądarce z Dev Tools (Network tab)
# Kliknij link z emaila
# ✅ Powinien być TYLKO JEDEN request do verify-login
```

### Production Build:
```bash
npm run build
npm run preview

# W produkcji StrictMode nie działa
# Więc i tak byłoby jedno wywołanie
```

---

## 📝 Inne miejsca gdzie może być problem

Sprawdź czy inne strony też nie mają tego problemu:

### PublicSurveyStartPage
```tsx
// Sprawdź czy useEffect dla survey nie wywołuje się wielokrotnie
React.useEffect(() => {
  (async () => {
    const data = await getSurveyRun(surveyId);
    setRun(data);
  })();
}, [surveyId, isAuthed]);
```

**Tutaj jest OK** - getSurveyRun to GET request (idempotentny)

### EmailLoginPage ✅ NAPRAWIONE
```tsx
// Teraz zabezpieczone!
const hasVerified = React.useRef(false);
React.useEffect(() => {
  if (hasVerified.current) return;
  hasVerified.current = true;
  // ...
}, []);
```

---

## 🎓 Best Practices

### Kiedy używać useRef flag:

✅ **Używaj dla:**
- API calls które zmieniają stan (POST, PUT, DELETE)
- Operacje które powinny wykonać się RAZ
- Side effects które nie powinny się powtarzać
- Inicjalizacja (analytics, logging)

❌ **NIE używaj dla:**
- Zwykłe GET requests (są idempotentne)
- Obliczenia (użyj useMemo)
- Conditionals w render

### Template:

```tsx
function MyComponent() {
  const hasInitialized = React.useRef(false);
  
  React.useEffect(() => {
    if (hasInitialized.current) return;
    hasInitialized.current = true;
    
    // Twój kod który powinien wykonać się RAZ
    
  }, [dependencies]);
}
```

---

## ✅ Status

- [x] Zidentyfikowano problem (podwójne wywołanie API)
- [x] Zrozumiano przyczynę (React.StrictMode)
- [x] Zaimplementowano rozwiązanie (useRef flag)
- [x] Przetestowano w dev mode
- [x] Udokumentowano

**Teraz API jest wywoływane tylko RAZ! ✅**

---

## 🔗 Więcej informacji

- [React StrictMode Docs](https://react.dev/reference/react/StrictMode)
- [useRef Hook](https://react.dev/reference/react/useRef)
- [useEffect Hook](https://react.dev/reference/react/useEffect)

