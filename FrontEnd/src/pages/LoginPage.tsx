import React from 'react';
import styles from './LoginPage.module.css';
import bg from '../images/Document.png';
import { login } from '../services/auth';
import { useNavigate } from 'react-router-dom';

export function LoginPage() {
  const navigate = useNavigate();
  const [email, setEmail] = React.useState('');
  const [password, setPassword] = React.useState('');
  const [loading, setLoading] = React.useState(false);
  const [error, setError] = React.useState<string | null>(null);
  const [showPassword, setShowPassword] = React.useState(false);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    if (!email || !password) {
      setError('Email and password required');
      return;
    }
    setLoading(true);
    try {
      const res = await login({ email, password });
      if (!res.success || !res.token) {
        setError(res.message ?? 'Invalid credentials');
        setLoading(false);
        return;
      }
      navigate('/forms');
    } catch (err) {
      setError('Login failed');
      setLoading(false);
    }
  }

  return (
    <main className={styles.root}>
      <section className={styles.left}>
        <h1 className={styles.brand}>Userly</h1>
        <form className={styles.form} onSubmit={onSubmit}>
          <div className={styles.row}>
            <label className={styles.label} htmlFor="email">Email Address</label>
            <input
              id="email"
              className={styles.input}
              type="email"
              placeholder="you@example.com"
              value={email}
              onChange={e => setEmail(e.target.value)}
              autoComplete="email"
              required
            />
          </div>
          <div className={styles.row}>
            <label className={styles.label} htmlFor="password">Password</label>
            <div className={styles.inputWrap}>
              <input
                id="password"
                className={styles.input}
                type={showPassword ? 'text' : 'password'}
                placeholder="************"
                value={password}
                onChange={e => setPassword(e.target.value)}
                autoComplete="current-password"
                required
              />
              <button
                type="button"
                className={styles.toggle}
                aria-label={showPassword ? 'Hide password' : 'Show password'}
                aria-pressed={showPassword}
                onClick={() => setShowPassword(v => !v)}
              >
                {showPassword ? (
                  <svg width="22" height="22" viewBox="0 0 24 24" fill="none">
                    <path d="M3 3l18 18" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
                    <path d="M10.58 10.58A3 3 0 0012 15a3 3 0 002.42-4.42M9.88 5.09A10.46 10.46 0 0112 5c5.52 0 10 4 11 7-0.28.88-1.02 2.28-2.35 3.64M6.35 6.35C4.6 7.7 3.28 9.35 3 12c.3.94 1.15 2.52 2.86 3.95 1.71 1.44 4.09 3.05 6.14 3.05 1.05 0 2.17-.32 3.29-.89" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                  </svg>
                ) : (
                  <svg width="22" height="22" viewBox="0 0 24 24" fill="none">
                    <path d="M1 12s4-7 11-7 11 7 11 7-4 7-11 7S1 12 1 12z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" />
                    <circle cx="12" cy="12" r="3" stroke="currentColor" strokeWidth="2" />
                  </svg>
                )}
              </button>
            </div>
            <a className={styles.forgot} href="#">Forgot Password?</a>
          </div>
          {error ? <div className={styles.error} role="alert">{error}</div> : null}
          <button className={styles.button} type="submit" disabled={loading}>
            {loading ? 'Logging in...' : 'Login'}
          </button>
        </form>
      </section>
      <section className={styles.right} aria-hidden="true">
        <div
          className={styles.bg}
          style={{ backgroundImage: `url(${bg})` }}
        />
      </section>
    </main>
  );
}


