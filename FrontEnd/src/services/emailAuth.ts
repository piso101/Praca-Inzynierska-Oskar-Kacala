import { API_URL } from './config';

export interface SendEmailLoginRequest {
  email: string;
  surveyId?: number;
}

export interface SendEmailLoginResponse {
  success: boolean;
  message: string;
}

export interface VerifyEmailLoginResponse {
  success: boolean;
  token?: string;
  message: string;
}

export async function sendLoginEmail(email: string, surveyId?: number): Promise<SendEmailLoginResponse> {
  const response = await fetch(`${API_URL}/emailauth/send-login-email`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({ email, surveyId }),
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({ message: 'Failed to send login email' }));
    throw new Error(errorData.message || 'Failed to send login email');
  }

  return await response.json();
}

export async function verifyEmailLogin(token: string): Promise<VerifyEmailLoginResponse> {
  const response = await fetch(`${API_URL}/emailauth/verify-login?token=${encodeURIComponent(token)}`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({ message: 'Invalid or expired token' }));
    throw new Error(errorData.message || 'Invalid or expired token');
  }

  return await response.json();
}

export async function getQuestUser(token: string): Promise<any> {
  const response = await fetch(`${API_URL}/emailauth/me`, {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
  });

  if (!response.ok) {
    throw new Error('Failed to get user info');
  }

  return await response.json();
}

export function saveQuestToken(token: string): void {
  localStorage.setItem('quest_token', token);
}

export function getQuestToken(): string | null {
  return localStorage.getItem('quest_token');
}

export function clearQuestToken(): void {
  localStorage.removeItem('quest_token');
}

