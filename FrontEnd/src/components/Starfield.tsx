import React from 'react';
import styles from './Starfield.module.css';

function generateBoxShadows(count: number, color: string): string {
  const parts: string[] = [];
  for (let i = 0; i < count; i++) {
    const x = Math.floor(Math.random() * 2000);
    const y = Math.floor(Math.random() * 2000);
    parts.push(`${x}px ${y}px ${color}`);
  }
  return parts.join(', ');
}

export function Starfield() {
    
  const small = React.useMemo(() => generateBoxShadows(700, '#000'), []);
  const medium = React.useMemo(() => generateBoxShadows(200,'#000'), []);
  const big = React.useMemo(() => generateBoxShadows(100,'#000'), []);

  return (
    <div className={styles.root} aria-hidden="true">
      {/* layer 1 */}
      <div className={styles.layer} style={{ boxShadow: small }} />
      <div className={styles.layer} style={{ boxShadow: small, transform: 'translateY(2000px)' }} />
      {/* layer 2 */}
      <div className={styles.layer2} style={{ boxShadow: medium }} />
      <div className={styles.layer2} style={{ boxShadow: medium, transform: 'translateY(2000px)' }} />
      {/* layer 3 */}
      <div className={styles.layer3} style={{ boxShadow: big }} />
      <div className={styles.layer3} style={{ boxShadow: big, transform: 'translateY(2000px)' }} />
    </div>
  );
}


