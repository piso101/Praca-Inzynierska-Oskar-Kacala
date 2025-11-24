import React from 'react';
import styles from './SurveyResultsPage.module.css';
import { useNavigate, useParams } from 'react-router-dom';
import { getSurveyInviteCode } from '../services/questions';

export function SurveyResultsPage() {
  const navigate = useNavigate();
  const params = useParams();
  const surveyId = Number(params.id);

  const [inviteCode, setInviteCode] = React.useState<string | null>(null);
  const [loading, setLoading] = React.useState(true);

  React.useEffect(() => {
    let mounted = true;
    setLoading(true);
    getSurveyInviteCode(surveyId)
      .then(res => {
        if (!mounted) return;
        if (res.success && res.inviteCode) setInviteCode(res.inviteCode);
      })
      .finally(() => mounted && setLoading(false));
    return () => { mounted = false; };
  }, [surveyId]);

  const link = inviteCode ? `${window.location.origin}/join/${encodeURIComponent(inviteCode)}` : '';

  return (
    <div className={styles.root}>
      <div className={styles.container}>
        <h2 className={styles.title}>Survey Invite</h2>
        {loading ? (
          <div>Loading...</div>
        ) : !inviteCode ? (
          <div>Could not fetch invite code.</div>
        ) : (
          <div className={styles.card}>
            <div className={styles.row}><strong>Invite code:</strong> <span>{inviteCode}</span></div>
            <div className={styles.row}><strong>Link:</strong> <span><a href={link}>{link}</a></span></div>
            <button className={styles.btn} onClick={() => navigate(`/join/${inviteCode}`)}>Open user link</button>
          </div>
        )}
      </div>
    </div>
  );
}






