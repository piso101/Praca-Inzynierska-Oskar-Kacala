import React from 'react';

export function App() {
  const [apiStatus, setApiStatus] = React.useState<string>('...');

  React.useEffect(() => {
    fetch('/api/auth/health')
      .then(r => r.text())
      .then(t => setApiStatus(t))
      .catch(() => setApiStatus('API unreachable'));
  }, []);

  return (
    <div style={{ fontFamily: 'system-ui, sans-serif', padding: 24 }}>
      <h1>UX/UI Survey App</h1>
      <p>Status API: {apiStatus}</p>
    </div>
  );
}


