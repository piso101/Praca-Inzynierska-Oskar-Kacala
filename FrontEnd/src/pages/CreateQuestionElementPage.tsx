import React from 'react';
import styles from './CreateQuestionElementPage.module.css';
import { useNavigate, useParams } from 'react-router-dom';
import { Stage, Layer, Rect, Image as KonvaImage, Transformer } from 'react-konva';
import { addQuestionElement } from '../services/questions';
import { uploadFileToS3 } from '../services/storage';

function useImageObject(objectUrl: string | null) {
  const [img, setImg] = React.useState<HTMLImageElement | null>(null);
  React.useEffect(() => {
    if (!objectUrl) {
      setImg(null);
      return;
    }
    const image = new window.Image();
    image.onload = () => setImg(image);
    image.src = objectUrl;
    return () => {
      setImg(null);
    };
  }, [objectUrl]);
  return img;
}

export function CreateQuestionElementPage() {
  const navigate = useNavigate();
  const params = useParams();
  const surveyId = Number(params.id);
  const questionId = Number(params.questionId);

  // canvas size with 16:9 ratio
  const width = 960;
  const height = 540;

  const [fileUrl, setFileUrl] = React.useState<string | null>(null);
  const [fileObj, setFileObj] = React.useState<File | null>(null);
  const [fileName, setFileName] = React.useState<string>('uploaded-image');
  const imageObj = useImageObject(fileUrl);

  // image node state
  const [imagePos, setImagePos] = React.useState({ x: width / 2 - 100, y: height / 2 - 75 });
  const [imageSize, setImageSize] = React.useState({ width: 200, height: 150 });
  const [rotation, setRotation] = React.useState(0);
  const imageRef = React.useRef<any>(null);
  const trRef = React.useRef<any>(null);

  React.useEffect(() => {
    if (trRef.current && imageRef.current) {
      trRef.current.nodes([imageRef.current]);
      trRef.current.getLayer()?.batchDraw();
    }
  }, [imageObj]);

  // Fit the uploaded image into the sandbox while preserving its aspect ratio
  React.useEffect(() => {
    if (!imageObj) return;
    const iw = imageObj.width || 1;
    const ih = imageObj.height || 1;
    const scale = Math.min(width / iw, height / ih);
    const newW = Math.max(5, iw * scale);
    const newH = Math.max(5, ih * scale);
    setImageSize({ width: newW, height: newH });
    setImagePos({ x: (width - newW) / 2, y: (height - newH) / 2 });
    setRotation(0);
  }, [imageObj]);

  function onFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    const f = e.target.files?.[0];
    if (!f) return;
    const url = URL.createObjectURL(f);
    setFileUrl(url);
    setFileObj(f);
    setFileName(f.name || 'uploaded-image');
  }

  return (
    <div className={styles.root}>
      <div className={styles.container}>
        <h2 className={styles.title}>Create Element</h2>

        <div className={styles.toolbar}>
          <input type="file" accept="image/png,image/jpeg,image/svg+xml" onChange={onFileChange} />
          <div />
        </div>

        <div className={styles.sandboxWrap}>
          <div className={styles.sandboxInner}>
            <Stage width={width} height={height}>
              <Layer>
                {/* white border to visualize the 1920x1080 ratio (scaled) */}
                <Rect x={0} y={0} width={width} height={height} stroke="#ffffff" strokeWidth={2} />
                {imageObj ? (
                  <>
                    <KonvaImage
                      ref={imageRef}
                      image={imageObj}
                      x={imagePos.x}
                      y={imagePos.y}
                      width={imageSize.width}
                      height={imageSize.height}
                      rotation={rotation}
                      draggable
                      onDragEnd={(e) => setImagePos({ x: e.target.x(), y: e.target.y() })}
                      onTransformEnd={() => {
                        const node = imageRef.current;
                        const scaleX = node.scaleX();
                        const scaleY = node.scaleY();
                        node.scaleX(1);
                        node.scaleY(1);
                        setImageSize({
                          width: Math.max(5, node.width() * scaleX),
                          height: Math.max(5, node.height() * scaleY),
                        });
                        setRotation(node.rotation());
                        setImagePos({ x: node.x(), y: node.y() });
                      }}
                    />
                    <Transformer
                      ref={trRef}
                      rotateEnabled
                      enabledAnchors={['top-left','top-right','bottom-left','bottom-right']}
                      boundBoxFunc={(oldBox, newBox) => {
                        // limit resize
                        if (newBox.width < 5 || newBox.height < 5) {
                          return oldBox;
                        }
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
          <button className={styles.btn} onClick={() => navigate(`/forms/${surveyId}/questions/${questionId}/elements`)}>
            Cancel
          </button>
          <button
            className={styles.btn}
            onClick={async () => {
              if (!imageObj || !fileObj) { alert('Please upload an image first'); return; }
              // Base coordinate system 1920x1080
              const baseW = 1920;
              const baseH = 1080;
              const fx = baseW / width;
              const fy = baseH / height;
              const naturalScale = imageSize.width / (imageObj.width || 1);
              const scaledX = imagePos.x * fx;
              const scaledY = imagePos.y * fy;
              const scale = naturalScale * fx; // approximate uniform scale in base space
              // upload file to S3/MinIO
              let objectKey = `${surveyId}/${questionId}/${Date.now()}_${fileName}`;
              try {
                const uploaded = await uploadFileToS3(fileObj, objectKey);
                objectKey = uploaded.key;
              } catch (err: any) {
                alert(`Upload failed: ${err?.message ?? 'unknown error'}`);
                return;
              }
              const payload = {
                questionId,
                x: scaledX,
                y: scaledY,
                rotation,
                scale,
                orderIndex: 1,
                svgName: objectKey
              };
              const res = await addQuestionElement(payload);
              if (res.success) {
                navigate(`/forms/${surveyId}/questions/${questionId}/elements`);
              } else {
                alert('Saving element failed');
              }
            }}
          >
            Confirm
          </button>
        </div>
      </div>
    </div>
  );
}


