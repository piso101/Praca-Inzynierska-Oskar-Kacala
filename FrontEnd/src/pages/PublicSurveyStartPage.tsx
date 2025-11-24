import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getSurveyRun, SurveyRun, SurveyRunElement } from '../services/questions';
import { getSignedObjectUrl } from '../services/storage';
import { Stage, Layer, Rect, Image as KonvaImage } from 'react-konva';
import { ConfirmDialog } from '../components/ConfirmDialog';
import { sendLoginEmail, getQuestToken, clearQuestToken } from '../services/emailAuth';
import { submitSurveyResponse } from '../services/surveyResponse';

type ImageCache = Record<string, HTMLImageElement>;

export function PublicSurveyStartPage() {
  const params = useParams();
  const navigate = useNavigate();
  const surveyId = Number(params.surveyId);

  const baseWidth = 960;
  const baseHeight = 540;
  const [stageW, setStageW] = React.useState(baseWidth);
  const [stageH, setStageH] = React.useState(baseHeight);
  const containerRef = React.useRef<HTMLDivElement>(null);
  const fx = 1920 / stageW;
  const fy = 1080 / stageH;

  const [isAuthed, setIsAuthed] = React.useState<boolean>(Boolean(getQuestToken()));
  const [authError, setAuthError] = React.useState<string | null>(null);
  const [email, setEmail] = React.useState('');
  const [emailSent, setEmailSent] = React.useState(false);
  const [sendingEmail, setSendingEmail] = React.useState(false);

  const [run, setRun] = React.useState<SurveyRun | null>(null);
  const [loading, setLoading] = React.useState(true);
  const [qIndex, setQIndex] = React.useState(0);
  const [eIndex, setEIndex] = React.useState(0);
  const [answers, setAnswers] = React.useState<Record<number, number>>({}); // questionId -> elementId
  const [images, setImages] = React.useState<ImageCache>({});
  const [prevImages, setPrevImages] = React.useState<ImageCache>({});
  const [finishOpen, setFinishOpen] = React.useState(false);
  const [submitting, setSubmitting] = React.useState(false);

  const handleSendEmail = async () => {
    if (!email || !email.includes('@')) {
      setAuthError('Please enter a valid email address');
      return;
    }
    setSendingEmail(true);
    setAuthError(null);
    try {
      const result = await sendLoginEmail(email, surveyId);
      if (result.success) {
        setEmailSent(true);
      } else {
        setAuthError(result.message);
      }
    } catch (error: any) {
      setAuthError(error.message || 'Failed to send email');
    } finally {
      setSendingEmail(false);
    }
  };

  const handleLogout = () => {
    clearQuestToken();
    setIsAuthed(false);
    navigate('/');
  };

  React.useEffect(() => {
    let mounted = true;
    if (!isAuthed) {
      setLoading(false);
      return () => { mounted = false; };
    }
    (async () => {
      setLoading(true);
      const data = await getSurveyRun(surveyId);
      if (!mounted) return;
      setRun(data);
      setLoading(false);
      if (data?.questions?.[0]?.elements?.length) {
        setEIndex(0);
      }
    })();
    return () => { mounted = false; };
  }, [surveyId, isAuthed]);

  // prefetch signed URLs and images for current question
  React.useEffect(() => {
    let cancelled = false;
    (async () => {
      const question = run?.questions?.[qIndex];
      if (!question) return;
      const newCache: ImageCache = {};
      for (const el of question.elements) {
        try {
          const url = await getSignedObjectUrl(el.svgName);
          const img = await new Promise<HTMLImageElement>((resolve, reject) => {
            const im = new window.Image();
            im.crossOrigin = 'anonymous';
            im.onload = () => resolve(im);
            im.onerror = reject;
            im.src = url;
          });
          newCache[el.svgName] = img;
        } catch {
          // ignore
        }
      }
      if (!cancelled) setImages(newCache);
    })();
    return () => { cancelled = true; };
  }, [run, qIndex]);

  // Responsive stage size (keeps 16:9 ratio, scales to container width)
  React.useEffect(() => {
    const update = () => {
      const cw = containerRef.current?.clientWidth ?? baseWidth;
      const maxW = Math.max(320, Math.min(cw, 1200));
      const w = maxW;
      const h = Math.round(w * (baseHeight / baseWidth));
      setStageW(w);
      setStageH(h);
    };
    update();
    window.addEventListener('resize', update);
    return () => window.removeEventListener('resize', update);
  }, []);

  // Prepare derived data before any early returns to keep hook order stable
  const questions = run?.questions ?? [];
  const total = questions.length;
  const current = questions[qIndex];
  const elements = current?.elements ?? [];
  const currentElement: SurveyRunElement | undefined = elements[eIndex];

  const prevAnswers = questions.slice(0, qIndex).map((q) => {
    const elId = answers[q.id];
    const el = q.elements.find(e => e.id === elId);
    return { q, el };
  });

  const selectCurrent = () => {
    if (!current || !currentElement) return;
    setAnswers(prev => ({ ...prev, [current.id]: currentElement.id }));
  };

  // question-level navigation
  const goPrev = () => {
    if (qIndex > 0) {
      setQIndex(qIndex - 1);
      setEIndex(0);
    }
  };

  const goNext = () => {
    selectCurrent();
    if (qIndex < total - 1) {
      setQIndex(qIndex + 1);
      setEIndex(0);
    } else {
      setFinishOpen(true);
    }
  };

  const handleSubmitResponse = async () => {
    setSubmitting(true);
    try {
      const result = await submitSurveyResponse({
        surveyId,
        answers,
      });
      if (result.success) {
        alert('Thank you! Your response has been submitted successfully.');
        navigate('/');
      } else {
        alert(result.message || 'Failed to submit response');
      }
    } catch (error: any) {
      alert(error.message || 'Failed to submit response');
    } finally {
      setSubmitting(false);
      setFinishOpen(false);
    }
  };

  // element-level navigation within current question (via side arrows)
  const prevElement = () => {
    if (eIndex > 0) setEIndex(eIndex - 1);
  };
  const nextElement = () => {
    if (eIndex < elements.length - 1) setEIndex(eIndex + 1);
  };

  // Prefetch images for previous answers to render in background
  React.useEffect(() => {
    let cancelled = false;
    (async () => {
      const needed = prevAnswers.map(pa => pa.el?.svgName).filter(Boolean) as string[];
      const missing = needed.filter(k => !prevImages[k]);
      if (missing.length === 0) return;
      const cache: ImageCache = {};
      for (const key of missing) {
        try {
          const url = await getSignedObjectUrl(key);
          const img = await new Promise<HTMLImageElement>((resolve, reject) => {
            const im = new window.Image();
            im.crossOrigin = 'anonymous';
            im.onload = () => resolve(im);
            im.onerror = reject;
            im.src = url;
          });
          cache[key] = img;
        } catch {}
      }
      if (!cancelled) setPrevImages(prev => ({ ...prev, ...cache }));
    })();
    return () => { cancelled = true; };
  }, [answers, qIndex]);

  if (!isAuthed) {
    return (
      <div style={{ padding: 24, paddingTop: 'calc(var(--nav-height) + 20px)', maxWidth: 500, margin: '0 auto' }}>
        <div style={{
          background: '#1a1a1a',
          padding: 32,
          borderRadius: 12,
          border: '2px solid rgba(255,255,255,0.1)'
        }}>
          <h2 style={{ textAlign: 'center', margin: '0 0 24px' }}>Login to Start Survey</h2>
          
          {!emailSent ? (
            <>
              <p style={{ textAlign: 'center', color: '#ccc', marginBottom: 24 }}>
                Enter your email to receive a login link
              </p>
              <input
                type="email"
                placeholder="your.email@example.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSendEmail()}
                disabled={sendingEmail}
                style={{
                  width: '100%',
                  padding: 12,
                  fontSize: 16,
                  borderRadius: 8,
                  border: '2px solid rgba(255,255,255,0.2)',
                  background: '#000',
                  color: '#fff',
                  marginBottom: 16,
                  boxSizing: 'border-box'
                }}
              />
              <button
                onClick={handleSendEmail}
                disabled={sendingEmail}
                style={{
                  width: '100%',
                  padding: 12,
                  fontSize: 16,
                  background: sendingEmail ? '#555' : '#4CAF50',
                  color: '#fff',
                  border: 'none',
                  borderRadius: 8,
                  cursor: sendingEmail ? 'not-allowed' : 'pointer',
                  fontWeight: 'bold'
                }}
              >
                {sendingEmail ? 'Sending...' : 'Send Login Link'}
              </button>
            </>
          ) : (
            <div style={{ textAlign: 'center' }}>
              <div style={{ fontSize: 48, marginBottom: 16 }}>📧</div>
              <h3 style={{ margin: '0 0 16px', color: '#4CAF50' }}>Email Sent!</h3>
              <p style={{ color: '#ccc', marginBottom: 16 }}>
                Check your inbox for a login link. Click the link in the email to continue.
              </p>
              <button
                onClick={() => { setEmailSent(false); setEmail(''); }}
                style={{
                  padding: '10px 20px',
                  background: 'transparent',
                  color: '#4CAF50',
                  border: '2px solid #4CAF50',
                  borderRadius: 8,
                  cursor: 'pointer'
                }}
              >
                Send to Different Email
              </button>
            </div>
          )}
          
          {authError && (
            <div style={{ 
              color: '#f66', 
              textAlign: 'center', 
              marginTop: 16,
              padding: 12,
              background: 'rgba(255, 102, 102, 0.1)',
              borderRadius: 8
            }}>
              {authError}
            </div>
          )}
        </div>
      </div>
    );
  }

  if (loading) return <div style={{ padding: 24 }}>Loading...</div>;
  if (!run) return <div style={{ padding: 24 }}>Survey not found</div>;

  return (
    <div style={{ padding: 24, paddingTop: 'calc(var(--nav-height) + 20px)' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 12 }}>
        <div style={{ color: '#ccc' }}>
          Question {qIndex + 1} out of {total}
        </div>
        <button
          onClick={handleLogout}
          style={{
            padding: '6px 12px',
            background: 'transparent',
            color: '#f66',
            border: '1px solid #f66',
            borderRadius: 6,
            cursor: 'pointer',
            fontSize: 14
          }}
        >
          Logout
        </button>
      </div>
      <h2 style={{ textAlign: 'center', margin: '0 0 16px' }}>{current?.questionText}</h2>

      <div ref={containerRef} style={{ display: 'flex', justifyContent: 'center', position: 'relative' }}>

        <Stage width={stageW} height={stageH}>
          <Layer>
            <Rect x={0} y={0} width={stageW} height={stageH} stroke="#ffffff" strokeWidth={2} />
            {/* Background overlays of previous answers at their saved positions */}
            {prevAnswers.map(({ el }, idx) => {
              if (!el) return null;
              const img = prevImages[el.svgName];
              if (!img) return null;
              return (
                <KonvaImage
                  key={`prev-${idx}`}
                  image={img}
                  x={el.x / fx}
                  y={el.y / fy}
                  width={(img.width || 1) * (el.scale / fx)}
                  height={(img.height || 1) * (el.scale / fy)}
                  rotation={el.rotation}
                  opacity={0.35}
                  listening={false}
                />
              );
            })}
            {currentElement && images[currentElement.svgName] ? (
              <KonvaImage
                image={images[currentElement.svgName]}
                x={currentElement.x / fx}
                y={currentElement.y / fy}
                width={(images[currentElement.svgName].width || 1) * (currentElement.scale / fx)}
                height={(images[currentElement.svgName].height || 1) * (currentElement.scale / fy)}
                rotation={currentElement.rotation}
                listening={false}
              />
            ) : null}
          </Layer>
        </Stage>
                {/* Left element arrow */}
                <button
          aria-label="Previous element"
          onClick={prevElement}
          style={{
            position: 'absolute', left: `calc(50% - ${stageW / 2 + 40}px)`, top: '50%', transform: 'translateY(-50%)',
            width: 48, height: 48, borderRadius: 12, border: '2px solid rgba(255,255,255,0.8)',
            background: '#000000', color: '#fff', cursor: 'pointer'
          }}
        >
          ‹
        </button>
        {/* Right element arrow */}
        <button
          aria-label="Next element"
          onClick={nextElement}
          style={{
            position: 'absolute', right: `calc(50% - ${stageW / 2 + 40}px)`, top: '50%', transform: 'translateY(-50%)',
            width: 48, height: 48, borderRadius: 12, border: '2px solid rgba(255,255,255,0.8)',
            background: '#000000', color: '#fff', cursor: 'pointer'
          }}
        >
          ›
        </button>
      </div>

      {/* Navigation controls */}
      <div style={{ display: 'flex', gap: 16, justifyContent: 'center', marginTop: 16 }}>
        <button onClick={goPrev} style={{ padding: '10px 16px', background: '#000000', color: '#fff', border: '2px solid rgba(255,255,255,0.8)', borderRadius: 10 }}>Previous</button>
        <button onClick={goNext} style={{ padding: '10px 16px', background: '#000000', color: '#fff', border: '2px solid rgba(255,255,255,0.8)', borderRadius: 10 }}>
          {qIndex === total - 1 && eIndex === elements.length - 1 ? 'Finish form' : 'Next question'}
        </button>
      </div>

      {/* Previous answers */}
      

      <ConfirmDialog
        open={finishOpen}
        title="Submit Survey Response"
        message="Are you sure you want to submit your answers? You cannot change them after submission."
        confirmLabel={submitting ? "Submitting..." : "Submit"}
        cancelLabel="Cancel"
        onCancel={() => setFinishOpen(false)}
        onConfirm={handleSubmitResponse}
      />
    </div>
  );
}


