import React from 'react';
import { useParams } from 'react-router-dom';
import { openSurveyByInviteCode } from '../services/questions';
import { sendLoginEmail } from '../services/emailAuth';

export function JoinSurveyRouter() {
  const params = useParams();
  const inviteCode = params.inviteCode ?? '';
  
  const [surveyId, setSurveyId] = React.useState<number | null>(null);
  const [loading, setLoading] = React.useState(true);
  const [error, setError] = React.useState<string | null>(null);
  
  const [email, setEmail] = React.useState('');
  const [emailSent, setEmailSent] = React.useState(false);
  const [sendingEmail, setSendingEmail] = React.useState(false);
  const [emailError, setEmailError] = React.useState<string | null>(null);

  // Pobierz surveyId z invite code
  React.useEffect(() => {
    let mounted = true;
    (async () => {
      try {
        const id = await openSurveyByInviteCode(inviteCode);
        if (!mounted) return;
        if (!id) {
          setError('Invalid invite code');
        } else {
          setSurveyId(id);
        }
      } catch (err: any) {
        if (!mounted) return;
        setError(err.message || 'Failed to validate invite code');
      } finally {
        if (mounted) setLoading(false);
      }
    })();
    return () => { mounted = false; };
  }, [inviteCode]);

  const handleSendEmail = async () => {
    if (!email || !email.includes('@')) {
      setEmailError('Please enter a valid email address');
      return;
    }
    if (!surveyId) {
      setEmailError('Survey ID not found');
      return;
    }
    
    setSendingEmail(true);
    setEmailError(null);
    
    try {
      const result = await sendLoginEmail(email, surveyId);
      if (result.success) {
        setEmailSent(true);
      } else {
        setEmailError(result.message);
      }
    } catch (error: any) {
      setEmailError(error.message || 'Failed to send email');
    } finally {
      setSendingEmail(false);
    }
  };

  if (loading) {
    return (
      <div style={{ 
        padding: 24, 
        paddingTop: 'calc(var(--nav-height) + 20px)',
        textAlign: 'center' 
      }}>
        <div style={{ fontSize: 48, marginBottom: 16 }}>⏳</div>
        <h2>Validating invite code...</h2>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ 
        padding: 24, 
        paddingTop: 'calc(var(--nav-height) + 20px)',
        textAlign: 'center' 
      }}>
        <div style={{ fontSize: 48, marginBottom: 16 }}>❌</div>
        <h2 style={{ color: '#f66' }}>Error</h2>
        <p style={{ color: '#ccc' }}>{error}</p>
      </div>
    );
  }

  return (
    <div style={{ 
      padding: 24, 
      paddingTop: 'calc(var(--nav-height) + 20px)', 
      maxWidth: 500, 
      margin: '0 auto' 
    }}>
      <div style={{
        background: '#1a1a1a',
        padding: 32,
        borderRadius: 12,
        border: '2px solid rgba(255,255,255,0.1)'
      }}>
        {!emailSent ? (
          <>
            <div style={{ textAlign: 'center', marginBottom: 24 }}>
              <div style={{ fontSize: 48, marginBottom: 16 }}>📋</div>
              <h2 style={{ margin: '0 0 8px' }}>Join Survey</h2>
              <p style={{ color: '#ccc', fontSize: 14 }}>
                You've been invited to participate in a survey
              </p>
            </div>
            
            <p style={{ color: '#ccc', marginBottom: 16, fontSize: 14 }}>
              Enter your email to receive a login link:
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
            
            {emailError && (
              <div style={{ 
                color: '#f66', 
                textAlign: 'center', 
                marginTop: 16,
                padding: 12,
                background: 'rgba(255, 102, 102, 0.1)',
                borderRadius: 8,
                fontSize: 14
              }}>
                {emailError}
              </div>
            )}
          </>
        ) : (
          <div style={{ textAlign: 'center' }}>
            <div style={{ fontSize: 48, marginBottom: 16 }}>📧</div>
            <h3 style={{ margin: '0 0 16px', color: '#4CAF50' }}>Email Sent!</h3>
            <p style={{ color: '#ccc', marginBottom: 16, fontSize: 14 }}>
              Check your inbox for a login link. Click the link in the email to continue to the survey.
            </p>
            <p style={{ color: '#999', fontSize: 12, marginBottom: 16 }}>
              The link will expire in 1 hour.
            </p>
            <button
              onClick={() => { setEmailSent(false); setEmail(''); }}
              style={{
                padding: '10px 20px',
                background: 'transparent',
                color: '#4CAF50',
                border: '2px solid #4CAF50',
                borderRadius: 8,
                cursor: 'pointer',
                fontSize: 14
              }}
            >
              Send to Different Email
            </button>
          </div>
        )}
      </div>
    </div>
  );
}






