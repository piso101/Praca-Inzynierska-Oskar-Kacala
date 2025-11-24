export type LoginRequest = {
  email: string;
  password: string;
};

export type LoginResponse = {
  success: boolean;
  token?: string;
  message?: string;
};

const API_BASE = (import.meta.env.VITE_API_BASE_URL as string) || '';
const AUTH_TOKEN_KEY = 'auth_token';

export function getToken(): string | null {
  try {
    return localStorage.getItem(AUTH_TOKEN_KEY);
  } catch {
    return null;
  }
}

export function saveToken(token: string) {
  try {
    localStorage.setItem(AUTH_TOKEN_KEY, token);
  } catch {
    // ignore storage errors (private mode, etc.)
  }
}

export function logout() {
  try {
    localStorage.removeItem(AUTH_TOKEN_KEY);
  } catch {
    // ignore
  }
}

export async function login(request: LoginRequest): Promise<LoginResponse> {
  const res = await fetch(`${API_BASE}/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email: request.email, password: request.password })
  });
  if (!res.ok) {
    // Attempt to parse error body if provided
    try {
      const data = (await res.json()) as LoginResponse;
      return data?.success === true ? data : { success: false, message: data?.message ?? 'Login failed' };
    } catch {
      return { success: false, message: res.status === 401 ? 'Invalid credentials' : 'Login failed' };
    }
  }
  const data = (await res.json()) as LoginResponse;
  if (data.success && data.token) {
    saveToken(data.token);
  }
  return data;
}

type GoogleAuthResponse = {
  token?: string;
  user?: unknown;
};

export async function loginWithGoogle(idToken: string): Promise<{ success: boolean; token?: string; message?: string }> {
  const res = await fetch(`${API_BASE}/auth/google`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ token: idToken })
  });
  if (!res.ok) {
    try {
      const err = (await res.json()) as { error?: string };
      return { success: false, message: err?.error ?? 'Google login failed' };
    } catch {
      return { success: false, message: res.status === 401 ? 'Invalid Google token' : 'Google login failed' };
    }
  }
  const data = (await res.json()) as GoogleAuthResponse;
  if (data?.token) {
    saveToken(data.token);
    return { success: true, token: data.token };
  }
  return { success: false, message: 'No token returned' };
}


