import { getToken } from './auth';
import { getQuestToken } from './emailAuth';

const API_BASE = (import.meta.env.VITE_API_BASE_URL as string) || '';

export async function apiFetch(input: string, init?: RequestInit) {
  // Spróbuj najpierw admin token, potem quest token
  const token = getToken() || getQuestToken();
  const headers = new Headers(init?.headers || {});
  headers.set('Accept', 'application/json');
  if (!headers.has('Content-Type') && init?.body) {
    headers.set('Content-Type', 'application/json');
  }
  if (token && !headers.has('Authorization')) {
    headers.set('Authorization', `Bearer ${token}`);
  }
  const url = input.startsWith('http') ? input : `${API_BASE}${input}`;
  return fetch(url, { ...init, headers });
}






