import { apiFetch } from './http';

export interface ElementResult {
  elementId: number;
  x: number;
  y: number;
  rotation: number;
  scale: number;
  svgName: string;
  selectionCount: number;
  selectionPercentage: number;
  orderIndex: number;
}

export interface QuestionResult {
  questionId: number;
  questionText: string;
  orderIndex: number;
  totalAnswers: number;
  elements: ElementResult[];
}

export interface SurveyResults {
  surveyId: number;
  title: string;
  description?: string;
  totalResponses: number;
  inviteCode: string;
  shareLink: string;
  questions: QuestionResult[];
}

export const surveyResultsService = {
  async getSurveyResults(surveyId: number): Promise<SurveyResults> {
    const res = await apiFetch(`/survey/survey-results?surveyId=${surveyId}`, {
      method: 'GET',
    });
    
    if (!res.ok) {
      throw new Error('Failed to fetch survey results');
    }
    
    return res.json() as Promise<SurveyResults>;
  },
};

