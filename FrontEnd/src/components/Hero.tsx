import React from 'react';
import styles from './Hero.module.css';
import bg from '../images/Document.png';

type HeroProps = {
  title: string;
  subtitle?: string;
};

export function Hero({ title, subtitle }: HeroProps) {
  return (
    <section className={styles.root} aria-label="Intro">
      <div
        className={styles.backdrop}
        style={{ backgroundImage: `url(${bg})` }}
        aria-hidden="true"
      />
      <div className={styles.overlay} aria-hidden="true" />
      <div className={styles.content}>
        <h1 className={styles.title}>{title}</h1>
        {subtitle ? <p className={styles.subtitle}>{subtitle}</p> : null}
      </div>
    </section>
  );
}


