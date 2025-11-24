import React from 'react';
import styles from './EditSurveyPage.module.css';
import { deleteSurvey, getSurvey, updateSurvey } from '../services/surveys';
import { useNavigate, useParams } from 'react-router-dom';
import { ConfirmDialog } from '../components/ConfirmDialog';

export function EditSurveyPage() {
  const navigate = useNavigate();
  const params = useParams();
  const id = Number(params.id);

  const [title, setTitle] = React.useState('');
  const [description, setDescription] = React.useState('');
  const [createdAt, setCreatedAt] = React.useState<string>('');
  const [publishedAt, setPublishedAt] = React.useState<string>('');
  const [isActive, setIsActive] = React.useState(true);
  const [loading, setLoading] = React.useState(true);
  const [error, setError] = React.useState<string | null>(null);
  const [confirmOpen, setConfirmOpen] = React.useState(false);

  React.useEffect(() => {
    let mounted = true;
    (async () => {
      const data = await getSurvey(id);
      if (!mounted) return;
      if (!data) {
        setError('Survey not found');
        setLoading(false);
        return;
      }
      setTitle(data.title ?? '');
      setDescription(data.description ?? '');
      setCreatedAt(data.createdAt ? data.createdAt.substring(0, 10) : '');
      setPublishedAt(data.publishedAt ? data.publishedAt.substring(0, 10) : '');
      setLoading(false);
    })();
    return () => { mounted = false; };
  }, [id]);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    if (!title.trim()) { setError('Title is required'); return; }
    const ok = await updateSurvey({ id, title: title.trim(), description: description.trim() || undefined, isActive });
    if (!ok) { setError('Update failed'); return; }
    navigate('/forms');
  }

  async function onDelete() {
    setConfirmOpen(false);
    const ok = await deleteSurvey(id);
    if (ok) navigate('/forms');
  }

  if (loading) return <div className={styles.root}><div className={styles.container}>Loading...</div></div>;

  return (
    <div className={styles.root}>
      <form className={styles.container} onSubmit={onSubmit}>
        <h2 className={styles.title}>Edit Survey:</h2>
        <div className={styles.grid}>
          <div className={styles.field}>
            <label className={styles.label} htmlFor="title">Title</label>
            <input id="title" className={styles.input} value={title} onChange={e => setTitle(e.target.value)} />
          </div>
          <div className={styles.field}>
            <label className={styles.label} htmlFor="createdAt">CreatedAt</label>
            <input id="createdAt" type="date" className={styles.date} value={createdAt} onChange={e => setCreatedAt(e.target.value)} disabled />
          </div>
          <div className={styles.field}>
            <label className={styles.label} htmlFor="description">Description</label>
            <textarea id="description" className={styles.textarea} value={description} onChange={e => setDescription(e.target.value)} />
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
          <button type="submit" className={styles.btn}>Confirm</button>
          <button type="button" className={styles.btn} onClick={() => navigate('/forms')}>Cancel</button>
          <button type="button" className={`${styles.btn} ${styles.danger}`} onClick={() => setConfirmOpen(true)}>Delete</button>
          <button type="button" className={styles.btn} onClick={() => navigate(`/forms/${id}/questions`)}>Show Questions</button>
        </div>
      </form>
      <ConfirmDialog
        open={confirmOpen}
        title="Delete survey"
        message="Are you sure you want to delete this survey?"
        confirmLabel="Delete"
        cancelLabel="Cancel"
        onCancel={() => setConfirmOpen(false)}
        onConfirm={onDelete}
      />
    </div>
  );
}


