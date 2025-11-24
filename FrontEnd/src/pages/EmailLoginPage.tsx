import React from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { verifyEmailLogin, saveQuestToken } from '../services/emailAuth';

export function EmailLoginPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const [status, setStatus] = React.useState<'verifying' | 'success' | 'error'>('verifying');
  const [message, setMessage] = React.useState('Verifying your login link...');
  const hasVerified = React.useRef(false);

  React.useEffect(() => {
    // Zapobiegaj wielokrotnemu wywołaniu (React StrictMode w dev mode)
    if (hasVerified.current) return;
    
    const token = searchParams.get('token');
    if (!token) {
      setStatus('error');
      setMessage('No token provided');
      return;
    }

    hasVerified.current = true;

    (async () => {
      try {
        const result = await verifyEmailLogin(token);
        if (result.success && result.token) {
          saveQuestToken(result.token);
          setStatus('success');
          setMessage('Login successful! Redirecting...');
          setTimeout(() => {
            const returnUrl = searchParams.get('returnUrl') || '/';
            navigate(returnUrl);
          }, 2000);
        } else {
          setStatus('error');
          setMessage(result.message || 'Login failed');
        }
      } catch (error: any) {
        setStatus('error');
        setMessage(error.message || 'Invalid or expired login link');
      }
    })();
  }, [searchParams, navigate]);

  return (
    <div style={{ 
      padding: 24, 
      paddingTop: 'calc(var(--nav-height) + 20px)',
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
      justifyContent: 'center',
      minHeight: '60vh'
    }}>
      <div style={{
        textAlign: 'center',
        maxWidth: 500,
        background: '#1a1a1a',
        padding: 32,
        borderRadius: 12,
        border: '2px solid rgba(255,255,255,0.1)'
      }}>
        {status === 'verifying' && (
          <>
            <div style={{ fontSize: 48, marginBottom: 16 }}>⏳</div>
            <h2 style={{ margin: '0 0 16px' }}>Verifying...</h2>
            <p style={{ color: '#ccc' }}>{message}</p>
          </>
        )}
        {status === 'success' && (
          <>
            <div style={{ fontSize: 48, marginBottom: 16 }}>✅</div>
            <h2 style={{ margin: '0 0 16px', color: '#4CAF50' }}>Success!</h2>
            <p style={{ color: '#ccc' }}>{message}</p>
          </>
        )}
        {status === 'error' && (
          <>
            <div style={{ fontSize: 48, marginBottom: 16 }}>❌</div>
            <h2 style={{ margin: '0 0 16px', color: '#f66' }}>Error</h2>
            <p style={{ color: '#ccc' }}>{message}</p>
            <button 
              onClick={() => navigate('/')}
              style={{
                marginTop: 24,
                padding: '10px 24px',
                background: '#4CAF50',
                color: '#fff',
                border: 'none',
                borderRadius: 8,
                cursor: 'pointer',
                fontSize: 16
              }}
            >
              Go to Home
            </button>
          </>
        )}
      </div>
    </div>
  );
}

