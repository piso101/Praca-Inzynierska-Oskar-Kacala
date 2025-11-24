import React from 'react';
import styles from './EditQuestionElementPage.module.css';
import { useNavigate, useParams } from 'react-router-dom';
import { Layer, Rect, Stage, Transformer, Image as KonvaImage } from 'react-konva';
import { getQuestionElementById } from '../services/questions';
import { getSignedObjectUrl } from '../services/storage';

function useHtmlImage(src: string | null) {
  const [img, setImg] = React.useState<HTMLImageElement | null>(null);
  React.useEffect(() => {
    if (!src) { setImg(null); return; }
    const im = new window.Image();
    im.crossOrigin = 'anonymous';
    im.onload = () => setImg(im);
    im.src = src;
    return () => setImg(null);
  }, [src]);
  return img;
}

export function EditQuestionElementPage() {
  const navigate = useNavigate();
  const params = useParams();
  const surveyId = Number(params.id);
  const questionId = Number(params.questionId);
  const elementId = Number(params.elementId);

  const width = 960;
  const height = 540;
  const fx = 1920 / width;
  const fy = 1080 / height;

  const [imageUrl, setImageUrl] = React.useState<string | null>(null);
  const imageObj = useHtmlImage(imageUrl);

  const imageRef = React.useRef<any>(null);
  const trRef = React.useRef<any>(null);

  const [pos, setPos] = React.useState({ x: 0, y: 0 });
  const [size, setSize] = React.useState({ width: 100, height: 100 });
  const [rotation, setRotation] = React.useState(0);
  const [loading, setLoading] = React.useState(true);
  const [error, setError] = React.useState<string | null>(null);
  const [dispScale, setDispScale] = React.useState<number | null>(null);

  React.useEffect(() => {
    (async () => {
      try {
        const el = await getQuestionElementById(elementId);
        if (!el) { setError('Element not found'); setLoading(false); return; }
        // sign url for the key (svgName may be S3 key)
        const url = await getSignedObjectUrl(el.svgName);
        setImageUrl(url);
        // convert base coords 1920x1080 -> canvas
        const x = el.x / fx;
        const y = el.y / fy;
        const ds = el.scale / fx; // display scale used in Create (natural * ds)
        setDispScale(ds);
        setPos({ x, y });
        setRotation(el.rotation);
      } catch (e: any) {
        setError(e?.message ?? 'Failed to load element');
      } finally {
        setLoading(false);
      }
    })();
  }, [elementId]);

  React.useEffect(() => {
    if (!imageObj) return;
    // set rendered size based on natural dimensions and stored display scale
    const naturalW = imageObj.width || 1;
    const naturalH = imageObj.height || 1;
    const ds = dispScale ?? Math.min(width / naturalW, height / naturalH);
    setSize({ width: naturalW * ds, height: naturalH * ds });
    if (trRef.current && imageRef.current) {
      trRef.current.nodes([imageRef.current]);
      trRef.current.getLayer()?.batchDraw();
    }
  }, [imageObj, dispScale]);

  if (loading) return <div className={styles.root}><div className={styles.container}>Loading...</div></div>;
  if (error) return <div className={styles.root}><div className={styles.container} style={{ color: '#ffb3b3' }}>{error}</div></div>;

  return (
    <div className={styles.root}>
      <div className={styles.container}>
        <h2 className={styles.title}>Edit Element</h2>
        <div className={styles.sandboxWrap}>
          <div className={styles.sandboxInner}>
            <Stage width={width} height={height}>
              <Layer>
                <Rect x={0} y={0} width={width} height={height} stroke="#ffffff" strokeWidth={2} />
                {imageObj ? (
                  <>
                    <KonvaImage
                      ref={imageRef}
                      image={imageObj}
                      x={pos.x}
                      y={pos.y}
                      width={size.width}
                      height={size.height}
                      rotation={rotation}
                      draggable
                      onDragEnd={(e) => setPos({ x: e.target.x(), y: e.target.y() })}
                      onTransformEnd={() => {
                        const node = imageRef.current;
                        const scaleX = node.scaleX();
                        const scaleY = node.scaleY();
                        node.scaleX(1);
                        node.scaleY(1);
                        setSize({
                          width: Math.max(5, node.width() * scaleX),
                          height: Math.max(5, node.height() * scaleY),
                        });
                        setRotation(node.rotation());
                        setPos({ x: node.x(), y: node.y() });
                      }}
                    />
                    <Transformer
                      ref={trRef}
                      rotateEnabled
                      enabledAnchors={['top-left','top-right','bottom-left','bottom-right']}
                      boundBoxFunc={(oldBox, newBox) => {
                        if (newBox.width < 5 || newBox.height < 5) return oldBox;
                        return newBox;
                      }}
                    />
                  </>
                ) : null}
              </Layer>
            </Stage>
          </div>
        </div>
        <div className={styles.actions}>
          <button className={styles.btn} onClick={() => navigate(`/forms/${surveyId}/questions/${questionId}/elements`)}>Back</button>
          <button className={styles.btn} onClick={() => alert('Save (placeholder)')}>Save</button>
        </div>
      </div>
    </div>
  );
}


