import React from 'react';
import styles from './CreateSurveyPage.module.css';
import { createSurvey, updateSurvey } from '../services/surveys';
import { useNavigate } from 'react-router-dom';

export function CreateSurveyPage() {
  const navigate = useNavigate();
  const [title, setTitle] = React.useState('');
  const [description, setDescription] = React.useState('');
  const [createdAt, setCreatedAt] = React.useState<string>('');
  const [publishedAt, setPublishedAt] = React.useState<string>('');
  const [isActive, setIsActive] = React.useState(true);
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
      const res = await createSurvey({ title: title.trim(), description: description.trim() || undefined });
      if (!res.success || !res.surveyId) {
        setError('Create failed');
        setLoading(false);
        return;
      }
      // Backend sets IsActive=true by default; if user unchecked, reflect via update
      if (isActive === false) {
        await updateSurvey({ id: res.surveyId, title: title.trim(), description: description.trim() || undefined, isActive });
      }
      navigate('/forms');
    } catch {
      setError('Create failed');
      setLoading(false);
    }
  }

  return (
    <div className={styles.root}>
      <form className={`${styles.container}`} onSubmit={onSubmit}>
        <h2 className={styles.title}>Create New Survey:</h2>
        <div className={styles.grid}>
          <div className={styles.field}>
            <label className={styles.label} htmlFor="title">Title</label>
            <input id="title" className={styles.input} value={title} onChange={e => setTitle(e.target.value)} />
          </div>

          <div className={styles.field}>
            <label className={styles.label} htmlFor="createdAt">CreatedAt</label>
            <input id="createdAt" type="date" className={styles.date} value={createdAt} onChange={e => setCreatedAt(e.target.value)} />
          </div>

          <div className={styles.field}>
            <label className={styles.label} htmlFor="description">Description</label>
            <textarea id="description" className={styles.textarea} value={description} onChange={e => setDescription(e.target.value)} />
          </div>

          <div className={styles.field}>
            <label className={styles.label} htmlFor="publishedAt">Published at</label>
            <input id="publishedAt" type="date" className={styles.date} value={publishedAt} onChange={e => setPublishedAt(e.target.value)} />
          </div>

          <div className={styles.field} style={{ gridColumn: '1 / -1' }}>
            <label className={styles.label}>IsActive</label>
            <div className={styles.checkboxRow}>
              <input id="isActive" type="checkbox" className={styles.checkbox} checked={isActive} onChange={e => setIsActive(e.target.checked)} />
              <label htmlFor="isActive">Active</label>
            </div>
          </div>
        </div>
        {error ? <div style={{ color: '#ffb3b3', marginTop: 12 }}>{error}</div> : null}
        <div className={styles.actions}>
          <button type="submit" className={styles.btn} disabled={loading}>Create New</button>
          <button type="button" className={styles.btn} onClick={() => navigate('/forms')}>Cancel</button>
        </div>
      </form>
    </div>
  );
}






