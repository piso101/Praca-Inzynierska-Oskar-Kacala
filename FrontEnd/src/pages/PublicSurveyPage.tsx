import React from 'react';
import styles from './PublicSurveyPage.module.css';
import { useNavigate, useParams } from 'react-router-dom';
import { getSurveyDetails } from '../services/questions';

export function PublicSurveyPage() {
  const params = useParams();
  const surveyId = Number(params.surveyId);
  const navigate = useNavigate();
  const [title, setTitle] = React.useState<string | null>(null);
  const [description, setDescription] = React.useState<string | null>(null);
  const [loading, setLoading] = React.useState(true);

  React.useEffect(() => {
    let mounted = true;
    setLoading(true);
    getSurveyDetails(surveyId)
      .then(d => {
        if (!mounted) return;
        setTitle(d?.title ?? null);
        setDescription(d?.description ?? null);
      })
      .finally(() => mounted && setLoading(false));
    return () => { mounted = false; };
  }, [surveyId]);

  return (
    <div className={styles.root}>
      <div className={styles.container}>
        {loading ? (
          'Loading...'
        ) : title ? (
          <>
            <h1 className={styles.title}>{title}</h1>
            <p className={styles.lead}>
              Przed Tobą krótka ankieta. Przeciągaj, klikaj i wybieraj elementy zgodnie z instrukcją na ekranie.
              Twoje odpowiedzi są anonimowe i posłużą do analizy UX. Zajmie to tylko kilka minut.
            </p>
            {description ? <div className={styles.desc}>{description}</div> : null}
            <div className={styles.actions}>
              <button className={styles.btn} onClick={() => navigate(`/public/survey/${surveyId}/start`)}>Zacznij</button>
            </div>
          </>
        ) : (
          'Survey not found'
        )}
      </div>
    </div>
  );
}


