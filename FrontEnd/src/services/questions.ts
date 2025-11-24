import { apiFetch } from './http';

export type SurveyQuestionItem = {
  id: number;
  surveyId: number;
  questionText: string;
  orderIndex: number;
  createdAt: string;
};

export async function getQuestionsForSurvey(surveyId: number): Promise<SurveyQuestionItem[]> {
    const res = await apiFetch(`/questions/question?surveyId=${surveyId}`, { method: 'GET' });
    if (!res.ok) return [];
    return (await res.json()) as SurveyQuestionItem[];
  }

export async function createQuestion(payload: { surveyId: number; questionText: string }) {
  const res = await apiFetch('/questions/create-question', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      surveyId: payload.surveyId,
      questionText: payload.questionText
    })
  });
  if (!res.ok) return { success: false as const, questionId: undefined as number | undefined };
  const data = await res.json();
  return { success: Boolean((data as any)?.success), questionId: (data as any)?.questionId as number | undefined };
}

export async function deleteQuestion(questionId: number) {
  const res = await apiFetch(`/questions/delete-question?questionId=${questionId}`, {
    method: 'DELETE'
  });
  return res.ok;
}

export async function updateQuestionOrder(surveyId: number, items: Array<{ id: number; orderIndex: number }>) {
  const payload = {
    surveyId,
    questions: items.map(i => ({ Item1: i.id, Item2: i.orderIndex }))
  };
  const res = await apiFetch('/questions/update-question-order', {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });
  return res.ok;
}

export async function updateQuestion(payload: { id: number; questionText: string }) {
  const url = `/questions/update-question?id=${encodeURIComponent(payload.id)}&questionText=${encodeURIComponent(payload.questionText)}`;
  const res = await apiFetch(url, { method: 'PUT' });
  return res.ok;
}

export type QuestionElementItem = {
  id: number;
  orderIndex: number;
  svgName: string;
};

export async function getQuestionElements(questionId: number): Promise<QuestionElementItem[]> {
  const res = await apiFetch(`/questions/question-elements?questionId=${questionId}`, { method: 'GET' });
  if (!res.ok) return [];
  return (await res.json()) as QuestionElementItem[];
}

export type QuestionElementFull = {
  id: number;
  questionId: number;
  x: number;
  y: number;
  rotation: number;
  scale: number;
  orderIndex: number;
  svgName: string;
};

export async function getQuestionElementById(elementId: number): Promise<QuestionElementFull | null> {
  const res = await apiFetch(`/questions/question-element?elementId=${elementId}`, { method: 'GET' });
  if (!res.ok) return null;
  return (await res.json()) as QuestionElementFull;
}

export async function deleteQuestionElement(elementId: number) {
  const res = await apiFetch(`/questions/delete-question-element?elementId=${elementId}`, {
    method: 'DELETE'
  });
  return res.ok;
}

export async function changeQuestionElementOrder(items: Array<{ id: number; orderIndex: number }>) {
  const payload = {
    elements: items.map(i => ({ Item1: i.id, Item2: i.orderIndex }))
  };
  const res = await apiFetch('/questions/change-element-order', {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  });
  return res.ok;
}

export async function getSurveyInviteCode(surveyId: number): Promise<{ success: boolean; inviteCode?: string }> {
  const res = await apiFetch(`/survey/get-survey-invite-code?surveyId=${surveyId}`, { method: 'GET' });
  if (!res.ok) return { success: false };
  const data = await res.json();
  return { success: Boolean(data?.success), inviteCode: data?.inviteCode ?? data?.InviteCode };
}

export async function getSurveyTitle(surveyId: number): Promise<string | null> {
  const res = await apiFetch(`/survey/read-survey?surveyId=${surveyId}`, { method: 'GET' });
  if (!res.ok) return null;
  const data = await res.json();
  return data?.title ?? data?.Title ?? null;
}

export type SurveyDetails = {
  title: string;
  description?: string | null;
};

export async function getSurveyDetails(surveyId: number): Promise<SurveyDetails | null> {
  const res = await apiFetch(`/survey/read-survey?surveyId=${surveyId}`, { method: 'GET' });
  if (!res.ok) return null;
  const data = await res.json();
  return {
    title: data?.title ?? data?.Title ?? '',
    description: data?.description ?? data?.Description ?? null
  };
}

export type SurveyRunElement = {
  id: number;
  x: number;
  y: number;
  rotation: number;
  scale: number;
  orderIndex: number;
  svgName: string;
};

export type SurveyRunQuestion = {
  id: number;
  questionText: string;
  elements: SurveyRunElement[];
};

export type SurveyRun = {
  id?: number;
  title: string;
  description?: string | null;
  questions: SurveyRunQuestion[];
};

export async function getSurveyRun(surveyId: number): Promise<SurveyRun | null> {
  const res = await apiFetch(`/survey/read-survey?surveyId=${surveyId}`, { method: 'GET' });
  if (!res.ok) return null;
  const data = await res.json();
  const questionsRaw = data?.questions ?? data?.Questions ?? [];
  const questions: SurveyRunQuestion[] = (questionsRaw as any[]).map((q: any) => {
    const elementsRaw = q?.elements ?? q?.Elements ?? [];
    const elements: SurveyRunElement[] = (elementsRaw as any[]).map((e: any) => ({
      id: e?.id ?? e?.Id,
      x: e?.x ?? e?.X ?? 0,
      y: e?.y ?? e?.Y ?? 0,
      rotation: e?.rotation ?? e?.Rotation ?? 0,
      scale: e?.scale ?? e?.Scale ?? 1,
      orderIndex: e?.orderIndex ?? e?.OrderIndex ?? 0,
      svgName: e?.svgName ?? e?.SvgName ?? ''
    })).sort((a, b) => (a.orderIndex - b.orderIndex));
    return {
      id: q?.id ?? q?.Id,
      questionText: q?.questionText ?? q?.QuestionText ?? '',
      elements
    };
  });
  return {
    id: data?.id ?? data?.Id,
    title: data?.title ?? data?.Title ?? '',
    description: data?.description ?? data?.Description ?? null,
    questions
  };
}

export async function openSurveyByInviteCode(inviteCode: string): Promise<number | null> {
  const res = await fetch(`/open-survey?inviteCode=${encodeURIComponent(inviteCode)}`, { method: 'GET' });
  if (!res.ok) return null;
  const data = await res.json();
  return data?.surveyId ?? data?.SurveyId ?? null;
}

export async function addQuestionElement(payload: {
  questionId: number;
  x: number;
  y: number;
  rotation: number;
  scale: number;
  orderIndex: number;
  svgName: string;
}) {
  const body = {
    QuestionId: payload.questionId,
    X: payload.x,
    Y: payload.y,
    Rotation: payload.rotation,
    Scale: payload.scale,
    OrderIndex: payload.orderIndex,
    SvgName: payload.svgName
  };
  const res = await apiFetch('/questions/add-element', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  });
  if (!res.ok) return { success: false as const, elementId: undefined as number | undefined };
  const data = await res.json();
  return { success: Boolean((data as any)?.success), elementId: (data as any)?.elementId as number | undefined };
}


