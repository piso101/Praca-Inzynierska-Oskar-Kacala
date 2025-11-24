import React from 'react';
import styles from './EditQuestionPage.module.css';
import { useNavigate, useParams } from 'react-router-dom';
import { getQuestionsForSurvey, updateQuestion } from '../services/questions';

export function EditQuestionPage() {
  const navigate = useNavigate();
  const params = useParams();
  const surveyId = Number(params.id);
  const questionId = Number(params.questionId);

  const [title, setTitle] = React.useState('');
  const [loading, setLoading] = React.useState(true);
  const [error, setError] = React.useState<string | null>(null);
  const [saving, setSaving] = React.useState(false);

  React.useEffect(() => {
    let mounted = true;
    (async () => {
      const list = await getQuestionsForSurvey(surveyId);
      if (!mounted) return;
      const q = list.find(x => x.id === questionId);
      if (!q) {
        setError('Question not found');
      } else {
        setTitle(q.questionText);
      }
      setLoading(false);
    })();
    return () => { mounted = false; };
  }, [surveyId, questionId]);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    if (!title.trim()) { setError('Title is required'); return; }
    setSaving(true);
    const ok = await updateQuestion({ id: questionId, questionText: title.trim() });
    setSaving(false);
    if (ok) {
      navigate(`/forms/${surveyId}/questions`);
    } else {
      setError('Update failed');
    }
  }

  if (loading) return <div className={styles.root}><div className={styles.container}>Loading...</div></div>;

  return (
    <div className={styles.root}>
      <form className={styles.container} onSubmit={onSubmit}>
        <h2 className={styles.title}>Edit Question</h2>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="title">Title</label>
          <input id="title" className={styles.input} value={title} onChange={e => setTitle(e.target.value)} />
        </div>
        {error ? <div style={{ color: '#ffb3b3' }}>{error}</div> : null}
        <div className={styles.actions}>
          <button type="submit" className={styles.btn} disabled={saving}>{saving ? 'Saving...' : 'Confirm'}</button>
          <button type="button" className={styles.btn} onClick={() => navigate(`/forms/${surveyId}/questions`)}>Cancel</button>
          <button type="button" className={styles.btn} onClick={() => navigate(`/forms/${surveyId}/questions/${questionId}/elements`)}>Show question elements</button>
        </div>
      </form>
    </div>
  );
}


