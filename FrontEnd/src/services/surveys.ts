import { apiFetch } from './http';

export type SurveyListItem = {
  id: number;
  title: string;
  description?: string | null;
  isActive: boolean;
  createdAt?: string;
  publishedAt?: string | null;
  creatorName?: string | null;
};

export async function getSurveys(): Promise<SurveyListItem[]> {
  const res = await apiFetch('/survey/get-list-of-surveys', { method: 'GET' });
  if (!res.ok) {
    return [];
  }
  const data = await res.json();
  return data as SurveyListItem[];
}

export async function createSurvey(payload: { title: string; description?: string | null }) {
  const res = await apiFetch('/survey/create-survey', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      title: payload.title,
      description: payload.description ?? null
    })
  });
  if (!res.ok) {
    return { success: false as const, surveyId: undefined as number | undefined };
  }
  const data = await res.json();
  return { success: Boolean((data as any)?.success), surveyId: (data as any)?.surveyId as number | undefined };
}

export async function updateSurvey(payload: { id: number; title: string; description?: string | null; isActive: boolean }) {
  const res = await apiFetch('/survey/update-survey', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      id: payload.id,
      title: payload.title,
      description: payload.description ?? null,
      isActive: payload.isActive
    })
  });
  return res.ok;
}

export async function getSurvey(id: number) {
  const res = await apiFetch(`/survey/read-survey?surveyId=${id}`, { method: 'GET' });
  if (!res.ok) return null;
  return res.json() as Promise<{
    title: string;
    description?: string | null;
    createdAt?: string;
    publishedAt?: string | null;
  }>;
}

export async function deleteSurvey(id: number) {
  const res = await apiFetch(`/survey/delete-survey?surveyId=${id}`, { method: 'DELETE' });
  return res.ok;
}


