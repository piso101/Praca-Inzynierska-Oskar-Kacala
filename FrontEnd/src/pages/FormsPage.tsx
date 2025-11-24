import React from 'react';
import styles from './FormsPage.module.css';
import { getToken } from '../services/auth';
import { getSurveys, SurveyListItem } from '../services/surveys';
import { useNavigate } from 'react-router-dom';

export function FormsPage() {
  const token = getToken();
  const [items, setItems] = React.useState<SurveyListItem[]>([]);
  const [loading, setLoading] = React.useState(false);
  const navigate = useNavigate();

  React.useEffect(() => {
    let mounted = true;
    if (!token) return;
    setLoading(true);
    getSurveys()
      .then(res => {
        if (mounted) setItems(res);
      })
      .finally(() => mounted && setLoading(false));
    return () => {
      mounted = false;
    };
  }, [token]);

  return (
    <div className={styles.root}>
      <div className="container">
        <h2 className={styles.title}>Owned UX Forms:</h2>
        <div className={styles.actions}>
          <button className={styles.createBtn} onClick={() => navigate('/forms/create')}>
            Create New
          </button>
        </div>
      </div>
      {!token ? (
        <div className={styles.empty}>Please log in to view your forms.</div>
      ) : (
        <div className={styles.tableWrap}>
          <table className={styles.table}>
            <thead>
              <tr>
                <th className={styles.th}>Title</th>
                <th className={styles.th}>Description</th>
                <th className={styles.th}>CreatedAt</th>
                <th className={styles.th}>Edit</th>
                <th className={styles.th}>Results</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr className={styles.row}>
                  <td className={styles.td} colSpan={7}>Loading...</td>
                </tr>
              ) : items.length === 0 ? (
                <tr className={styles.row}>
                  <td className={styles.td} colSpan={7}>No surveys found.</td>
                </tr>
              ) : (
                items.map(s => (
                  <tr key={s.id} className={styles.row}>
                    <td className={styles.td}>{s.title}</td>
                    <td className={`${styles.td} ${styles.desc}`}>
                      <span className={styles.ellipsis} title={s.description ?? ''}>
                        {s.description ?? '—'}
                      </span>
                    </td>
                    <td className={styles.td}>{s.createdAt ? new Date(s.createdAt).toLocaleDateString() : '—'}</td>
                    <td className={styles.td}>
                      <button className={`${styles.btn} ${styles.btnEdit}`} onClick={() => navigate(`/forms/${s.id}/edit`)}>Edit</button>
                    </td>
                    <td className={styles.td}>
                      <button className={`${styles.btn} ${styles.btnResults}`} onClick={() => navigate(`/forms/${s.id}/results`)}>Results</button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}


