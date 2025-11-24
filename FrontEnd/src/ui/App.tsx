import React from 'react';
import { Navbar } from '../components/Navbar';
import { Routes, Route, useLocation, useNavigate } from 'react-router-dom';
import { HomePage } from '../pages/HomePage';
import { LoginPage } from '../pages/LoginPage';
import { FormsPage } from '../pages/FormsPage';
import { CreateSurveyPage } from '../pages/CreateSurveyPage';
import { EditSurveyPage } from '../pages/EditSurveyPage';
import { getToken, logout } from '../services/auth';
import { Toast } from '../components/Toast';
import { QuestionsListPage } from '../pages/QuestionsListPage';
import { CreateQuestionPage } from '../pages/CreateQuestionPage';
import { EditQuestionPage } from '../pages/EditQuestionPage';
import { QuestionElementsPage } from '../pages/QuestionElementsPage';
import { CreateQuestionElementPage } from '../pages/CreateQuestionElementPage';
import { EditQuestionElementPage } from '../pages/EditQuestionElementPage';
import { SurveyResultsPage } from '../pages/SurveyResultsPage';
import { JoinSurveyRouter } from '../pages/JoinSurveyRouter';
import { PublicSurveyPage } from '../pages/PublicSurveyPage';
import { PublicSurveyStartPage } from '../pages/PublicSurveyStartPage';
import { EmailLoginPage } from '../pages/EmailLoginPage';

export function App() {
  const location = useLocation();
  const navigate = useNavigate();
  const path = location.pathname;
  const isAuthed = Boolean(getToken());
  const [toast, setToast] = React.useState<string | null>(null);
  return (
    <>
      <Navbar
        links={[
          { label: 'Home', href: '/', ariaCurrent: path === '/' },
          ...(isAuthed ? [{ label: 'Forms', href: '/forms', ariaCurrent: path === '/forms' }] : []),
          isAuthed
            ? { label: 'Logout', href: '#logout', onClick: () => { logout(); setToast('You have been logged out'); navigate('/'); } }
            : { label: 'Login', href: '/login', ariaCurrent: path === '/login' },
        ]}
      />
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/forms" element={<FormsPage />} />
        <Route path="/forms/create" element={<CreateSurveyPage />} />
        <Route path="/forms/:id/edit" element={<EditSurveyPage />} />
        <Route path="/forms/:id/questions" element={<QuestionsListPage />} />
        <Route path="/forms/:id/questions/create" element={<CreateQuestionPage />} />
        <Route path="/forms/:id/questions/:questionId/edit" element={<EditQuestionPage />} />
        <Route path="/forms/:id/questions/:questionId/elements" element={<QuestionElementsPage />} />
        <Route path="/forms/:id/questions/:questionId/elements/create" element={<CreateQuestionElementPage />} />
        <Route path="/forms/:id/questions/:questionId/elements/:elementId/edit" element={<EditQuestionElementPage />} />
        <Route path="/forms/:id/results" element={<SurveyResultsPage />} />
        <Route path="/join/:inviteCode" element={<JoinSurveyRouter />} />
        <Route path="/public/survey/:surveyId" element={<PublicSurveyPage />} />
        <Route path="/public/survey/:surveyId/start" element={<PublicSurveyStartPage />} />
        <Route path="/email-login" element={<EmailLoginPage />} />
        <Route path="/login" element={<LoginPage />} />
      </Routes>
      <Toast open={!!toast} message={toast ?? ''} onClose={() => setToast(null)} />
    </>
  );
}


