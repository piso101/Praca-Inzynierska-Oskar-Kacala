import { useEffect, useState, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { surveyResultsService, SurveyResults, QuestionResult, ElementResult } from '../services/surveyResultsService';
import { getSignedObjectUrl } from '../services/storage';
import heroBg from '../images/Document.png';
import styles from './SurveyResultsPage.module.css';

const BASE_CANVAS_WIDTH = 1920;
const BASE_CANVAS_HEIGHT = 1080;
const CANVAS_WIDTH = 960;
const CANVAS_HEIGHT = 540;

export const SurveyResultsPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [results, setResults] = useState<SurveyResults | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedQuestionIndex, setSelectedQuestionIndex] = useState<number>(0);
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const [copyMessage, setCopyMessage] = useState<string | null>(null);
  const imageCacheRef = useRef<Record<string, HTMLImageElement>>({});

  useEffect(() => {
    const fetchResults = async () => {
      try {
        const data = await surveyResultsService.getSurveyResults(Number(id));
        setResults(data);
        setLoading(false);
      } catch (err: any) {
        setError(err.message || 'Failed to load survey results');
        setLoading(false);
      }
    };

    fetchResults();
  }, [id]);

  useEffect(() => {
    if (results && results.questions.length > 0 && canvasRef.current) {
      let cancelled = false;
      (async () => {
        if (cancelled) return;
        const questionsToRender =
          selectedQuestionIndex === -1
            ? results.questions
            : [results.questions[selectedQuestionIndex]];
        await drawMostSelectedVisualization(questionsToRender);
      })();
      return () => {
        cancelled = true;
      };
    }
  }, [results, selectedQuestionIndex]);

  const handleCopyLink = async () => {
    if (!results?.shareLink) return;
    try {
      await navigator.clipboard.writeText(results.shareLink);
      setCopyMessage('Skopiowano link!');
      setTimeout(() => setCopyMessage(null), 2000);
    } catch {
      setCopyMessage('Nie udało się skopiować');
      setTimeout(() => setCopyMessage(null), 2000);
    }
  };

  const getImage = async (svgName: string): Promise<HTMLImageElement | null> => {
    if (imageCacheRef.current[svgName]) {
      return imageCacheRef.current[svgName];
    }

    try {
      const signedUrl = await getSignedObjectUrl(svgName);
      const img = await new Promise<HTMLImageElement>((resolve, reject) => {
        const imgEl = new window.Image();
        imgEl.crossOrigin = 'anonymous';
        imgEl.onload = () => resolve(imgEl);
        imgEl.onerror = reject;
        imgEl.src = signedUrl;
      });
      imageCacheRef.current[svgName] = img;
      return img;
    } catch {
      return null;
    }
  };

  const getTopElements = (question: QuestionResult): ElementResult[] => {
    if (!question.elements?.length) {
      return [];
    }

    const mostSelected = question.elements.reduce(
      (prev, current) => (current.selectionCount > prev.selectionCount ? current : prev),
      question.elements[0]
    );

    if (!mostSelected || mostSelected.selectionCount === 0) {
      return [];
    }

    return question.elements.filter(
      element => element.selectionCount === mostSelected.selectionCount
    );
  };

  const drawMostSelectedVisualization = async (questionsToRender: QuestionResult[]) => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    // Clear canvas
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Set canvas background
    ctx.fillStyle = '#f8f9fa';
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    let renderedAny = false;

    for (const question of questionsToRender) {
      const topElements = getTopElements(question);
      if (!topElements.length) {
        continue;
      }

      const elementsWithImages = await Promise.all(
        topElements.map(async (element) => ({
          element,
          image: await getImage(element.svgName),
        }))
      );

      for (const { element, image } of elementsWithImages) {
        if (!image) {
          continue;
        }
        await drawElement(ctx, element, image, canvas.width, canvas.height);
        renderedAny = true;
      }
    }

    ctx.fillStyle = '#333';
    ctx.font = 'bold 16px Arial';
    ctx.textAlign = 'center';

    if (!renderedAny) {
      ctx.fillText('Brak odpowiedzi do wyświetlenia', canvas.width / 2, canvas.height / 2);
    } else if (questionsToRender.length > 1) {
      ctx.fillText(
        'Mapa najczęściej wybieranych elementów (wszystkie pytania)',
        canvas.width / 2,
        canvas.height - 20
      );
    } else {
      const question = questionsToRender[0];
      const topElements = getTopElements(question);
      const topCount = topElements[0]?.selectionCount ?? 0;
      const topPercentage = topElements[0]?.selectionPercentage ?? 0;
      ctx.fillText(
        `Najczęściej wybierana odpowiedź: ${topCount} razy (${topPercentage}%)`,
        canvas.width / 2,
        canvas.height - 20
      );
    }
  };

  const drawElement = async (
    ctx: CanvasRenderingContext2D,
    element: ElementResult,
    image: HTMLImageElement,
    canvasWidth: number,
    canvasHeight: number
  ): Promise<void> => {
    return new Promise((resolve) => {
      try {
        ctx.save();

        const fx = BASE_CANVAS_WIDTH / canvasWidth;
        const fy = BASE_CANVAS_HEIGHT / canvasHeight;
        const drawX = element.x / fx;
        const drawY = element.y / fy;
        const drawWidth = (image.width || 1) * (element.scale / fx);
        const drawHeight = (image.height || 1) * (element.scale / fy);

        ctx.translate(drawX, drawY);
        ctx.rotate((element.rotation * Math.PI) / 180);

        ctx.drawImage(image, 0, 0, drawWidth, drawHeight);

        ctx.restore();
        resolve();
      } catch (err) {
        console.error(`Failed to draw image: ${element.svgName}`, err);
        resolve();
      }
    });
  };

  if (loading) {
    return (
      <div className={styles.container}>
        <div className={styles.loading}>Ładowanie wyników...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className={styles.container}>
        <div className={styles.error}>{error}</div>
        <button onClick={() => navigate('/admin/surveys')} className={styles.backButton}>
          Powrót do listy ankiet
        </button>
      </div>
    );
  }

  if (!results) {
    return null;
  }

  const selectedQuestion =
    selectedQuestionIndex === -1 ? null : results.questions[selectedQuestionIndex];

  const overallTopSummary = results.questions.map((question, index) => {
    const topElements = getTopElements(question);
    return {
      questionNumber: index + 1,
      questionText: question.questionText,
      totalAnswers: question.totalAnswers,
      topElements,
    };
  });

  const selectedElements = selectedQuestion?.elements ?? [];

  return (
    <div className={styles.pageWrapper}>
      <div
        className={styles.backdrop}
        style={{ backgroundImage: `url(${heroBg})` }}
        aria-hidden="true"
      />
      <div className={styles.overlay} aria-hidden="true" />

      <div className={styles.container}>
      <header className={styles.header}>
        <button onClick={() => navigate('/admin/surveys')} className={styles.backButton}>
          ← Powrót
        </button>
        <h1>{results.title}</h1>
        <p className={styles.subtitle}>
          Wyniki ankiety • Liczba odpowiedzi: {results.totalResponses}
        </p>
        <div className={styles.shareCard}>
          <div>
            <div className={styles.shareLabel}>Link do udostępniania</div>
            <div className={styles.shareLink}>{results.shareLink}</div>
            <div className={styles.shareCode}>
              Kod zaproszenia: <span>{results.inviteCode}</span>
            </div>
          </div>
          <div className={styles.shareActions}>
            <button className={styles.copyButton} onClick={handleCopyLink}>
              Skopiuj link
            </button>
            {copyMessage && <span className={styles.copyMessage}>{copyMessage}</span>}
          </div>
        </div>
      </header>

      {results.questions.length === 0 ? (
        <div className={styles.noData}>Brak pytań w ankiecie</div>
      ) : (
        <>
          {/* Question Navigation */}
          <div className={styles.questionNav}>
            {results.questions.map((question, index) => (
              <button
                key={question.questionId}
                className={`${styles.questionNavButton} ${index === selectedQuestionIndex ? styles.active : ''}`}
                onClick={() => setSelectedQuestionIndex(index)}
              >
                Pytanie {index + 1}
              </button>
            ))}
            <button
              className={`${styles.questionNavButton} ${selectedQuestionIndex === -1 ? styles.active : ''}`}
              onClick={() => setSelectedQuestionIndex(-1)}
            >
              Mapa globalna
            </button>
          </div>

          {/* Selected Question Content */}
          <div className={styles.questionContent}>
            {selectedQuestionIndex === -1 ? (
              <>
                <h2 className={styles.questionTitle}>Mapa globalna</h2>
                <p className={styles.answerCount}>
                  Wszystkie najczęściej wybierane odpowiedzi nałożone na siebie – finalna propozycja respondentów.
                </p>
              </>
            ) : (
              <>
                <h2 className={styles.questionTitle}>{selectedQuestion?.questionText}</h2>
                <p className={styles.answerCount}>
                  Liczba odpowiedzi: {selectedQuestion?.totalAnswers}
                </p>
              </>
            )}

            {/* Visualization Canvas */}
            <div className={styles.visualizationSection}>
              <h3>
                {selectedQuestionIndex === -1
                  ? 'Mapa wszystkich pytań'
                  : 'Wizualizacja najczęściej wybieranych odpowiedzi'}
              </h3>
              <p className={styles.visualizationHint}>
                Wizualizacja używa tych samych współrzędnych co w ankiecie, więc widzisz dokładnie rozmieszczenie elementów preferowanych przez użytkowników.
              </p>
              <div className={styles.canvasWrapper}>
                <canvas
                  ref={canvasRef}
                  width={CANVAS_WIDTH}
                  height={CANVAS_HEIGHT}
                  className={styles.canvas}
                />
              </div>
            </div>

            {selectedQuestionIndex === -1 ? (
              <div className={styles.overallSummary}>
                <h3>Podsumowanie najczęstszych odpowiedzi</h3>
                <div className={styles.overallList}>
                  {overallTopSummary.map(entry => (
                    <div key={entry.questionNumber} className={styles.overallItem}>
                      <div className={styles.overallQuestion}>
                        <strong>Pytanie {entry.questionNumber}:</strong> {entry.questionText}
                      </div>
                      {entry.topElements.length === 0 ? (
                        <div className={styles.overallInfo}>Brak odpowiedzi</div>
                      ) : (
                        entry.topElements.map(element => (
                          <div key={element.elementId} className={styles.overallInfo}>
                            Opcja {element.orderIndex + 1} • {element.selectionCount} odpowiedzi ({element.selectionPercentage}%)
                            <span className={styles.overallSvg}>{element.svgName}</span>
                          </div>
                        ))
                      )}
                    </div>
                  ))}
                </div>
              </div>
            ) : (
              <>
                {/* Bar Chart */}
                <div className={styles.chartSection}>
                  <h3>Wykres częstotliwości wyboru odpowiedzi</h3>
                  <div className={styles.barChart}>
                    {selectedElements.map((element) => (
                      <div key={element.elementId} className={styles.barWrapper}>
                        <div className={styles.barLabel}>
                          Opcja {element.orderIndex + 1}
                        </div>
                        <div className={styles.barContainer}>
                          <div
                            className={styles.bar}
                            style={{
                              width: `${element.selectionPercentage}%`,
                              backgroundColor: getBarColor(element.selectionPercentage),
                            }}
                          >
                            <span className={styles.barValue}>
                              {element.selectionCount} ({element.selectionPercentage}%)
                            </span>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>

                {/* Detailed Table */}
                <div className={styles.tableSection}>
                  <h3>Szczegółowe statystyki</h3>
                  <table className={styles.table}>
                    <thead>
                      <tr>
                        <th>Opcja</th>
                        <th>Nazwa SVG</th>
                        <th>Liczba wyborów</th>
                        <th>Procent</th>
                      </tr>
                    </thead>
                    <tbody>
                      {selectedElements
                        .sort((a, b) => b.selectionCount - a.selectionCount)
                        .map((element) => (
                          <tr key={element.elementId}>
                            <td>Opcja {element.orderIndex + 1}</td>
                            <td>{element.svgName}</td>
                            <td>{element.selectionCount}</td>
                            <td>{element.selectionPercentage}%</td>
                          </tr>
                        ))}
                    </tbody>
                  </table>
                </div>
              </>
            )}
          </div>
        </>
      )}
      </div>
    </div>
  );
};

function getBarColor(percentage: number): string {
  if (percentage >= 75) return '#4caf50';
  if (percentage >= 50) return '#2196f3';
  if (percentage >= 25) return '#ff9800';
  return '#f44336';
}
