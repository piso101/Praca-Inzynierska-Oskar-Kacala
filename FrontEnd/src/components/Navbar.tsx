import React from 'react';
import styles from './Navbar.module.css';

export type NavLink = {
  label: string;
  href: string;
  ariaCurrent?: boolean;
  onClick?: () => void;
};

type NavbarProps = {
  brand?: string;
  links: NavLink[];
};

export function Navbar({ brand = 'Userly', links }: NavbarProps) {
  const [open, setOpen] = React.useState(false);

  React.useEffect(() => {
    const handler = () => setOpen(false);
    window.addEventListener('resize', handler);
    return () => window.removeEventListener('resize', handler);
  }, []);

  return (
    <nav className={styles.root} aria-label="Main">
      <div className="container">
        <div className={styles.inner}>
          <button
            className={styles.menuButton}
            aria-label="Toggle menu"
            aria-expanded={open}
            onClick={() => setOpen(o => !o)}
          >
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
              <path d="M4 6h16M4 12h16M4 18h16" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
            </svg>
          </button>
          <div className={`${styles.links} ${open ? '' : styles.linksHidden}`} role="menubar">
            {links.map(l => (
              <a
                key={`${l.label}-${l.href}`}
                className={styles.link}
                href={l.href}
                aria-current={l.ariaCurrent ? 'page' : undefined}
                onClick={(e) => {
                  if (l.onClick) {
                    e.preventDefault();
                    l.onClick();
                  }
                }}
                role="menuitem"
              >
                {l.label}
              </a>
            ))}
          </div>
        </div>
      </div>
    </nav>
  );
}


