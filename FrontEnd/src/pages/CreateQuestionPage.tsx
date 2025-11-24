import React from 'react';
import styles from './CreateQuestionPage.module.css';
import { useNavigate, useParams } from 'react-router-dom';
import { createQuestion } from '../services/questions';

export function CreateQuestionPage() {
  const navigate = useNavigate();
  const params = useParams();
  const surveyId = Number(params.id);

  const [title, setTitle] = React.useState('');
  const [loading, setLoading] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    if (!title.trim()) {
      setError('Title is required');
      return;
    }
    setLoading(true);
    try {
      const res = await createQuestion({ surveyId, questionText: title.trim() });
      if (!res.success) {
        setError('Create failed');
        setLoading(false);
        return;
      }
      navigate(`/forms/${surveyId}/questions`);
    } catch {
      setError('Create failed');
      setLoading(false);
    }
  }

  return (
    <div className={styles.root}>
      <form className={styles.container} onSubmit={onSubmit}>
        <div className={styles.toolbar}>
          <h2 className={styles.title}>Create Question</h2>
          <button type="button" className={styles.btn} onClick={() => navigate(`/forms/${surveyId}/questions`)}>Back to list</button>
        </div>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="title">Title</label>
          <input id="title" className={styles.input} value={title} onChange={e => setTitle(e.target.value)} />
        </div>
        {error ? <div style={{ color: '#ffb3b3' }}>{error}</div> : null}
        <div className={styles.actions}>
          <button type="submit" className={styles.btn} disabled={loading}>Confirm</button>
          <button type="button" className={styles.btn} onClick={() => navigate(`/forms/${surveyId}/questions`)}>Cancel</button>
          <button type="button" className={styles.btn} onClick={() => alert('List of elements (placeholder)')}>List of elements</button>
        </div>
      </form>
    </div>
  );
}


