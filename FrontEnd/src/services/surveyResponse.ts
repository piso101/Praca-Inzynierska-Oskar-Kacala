import { API_URL } from './config';
import { getQuestToken } from './emailAuth';

export interface SubmitSurveyResponseRequest {
  surveyId: number;
  answers: Record<number, number>; // questionId -> elementId
}

export interface SubmitSurveyResponseResult {
  success: boolean;
  message: string;
  responseId?: number;
}

export async function submitSurveyResponse(request: SubmitSurveyResponseRequest): Promise<SubmitSurveyResponseResult> {
  const token = getQuestToken();
  if (!token) {
    throw new Error('Not authenticated');
  }

  const response = await fetch(`${API_URL}/survey/submit-response`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    const errorData = await response.json().catch(() => ({ message: 'Failed to submit response' }));
    throw new Error(errorData.message || 'Failed to submit response');
  }

  return await response.json();
}


