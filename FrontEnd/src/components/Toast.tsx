import React from 'react';
import styles from './Toast.module.css';

type ToastProps = {
  open: boolean;
  message: string;
  durationMs?: number;
  onClose?: () => void;
};

export function Toast({ open, message, durationMs = 2000, onClose }: ToastProps) {
  React.useEffect(() => {
    if (!open) return;
    const id = setTimeout(() => onClose?.(), durationMs);
    return () => clearTimeout(id);
  }, [open, durationMs, onClose]);

  return (
    <div className={`${styles.root} ${open ? styles.open : ''}`} role="status" aria-live="polite">
      <p className={styles.message}>{message}</p>
    </div>
  );
}






