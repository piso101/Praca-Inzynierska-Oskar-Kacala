import { S3Client, PutObjectCommand, HeadBucketCommand, CreateBucketCommand, GetObjectCommand } from '@aws-sdk/client-s3';
import { getSignedUrl } from '@aws-sdk/s3-request-presigner';

const S3_ENDPOINT = import.meta.env.VITE_S3_ENDPOINT || 'http://localhost:9000';
const S3_REGION = import.meta.env.VITE_S3_REGION || 'us-east-1';
const S3_ACCESS_KEY = import.meta.env.VITE_S3_ACCESS_KEY;
const S3_SECRET_KEY = import.meta.env.VITE_S3_SECRET_KEY;
const S3_BUCKET = import.meta.env.VITE_S3_BUCKET || 'my-bucket';

if (!S3_ACCESS_KEY || !S3_SECRET_KEY) {
  throw new Error('Configure local-only VITE_S3_ACCESS_KEY and VITE_S3_SECRET_KEY values; frontend credentials are public.');
}

export const s3 = new S3Client({
  endpoint: S3_ENDPOINT,
  region: S3_REGION,
  credentials: {
    accessKeyId: S3_ACCESS_KEY,
    secretAccessKey: S3_SECRET_KEY
  },
  forcePathStyle: true
});

async function ensureBucketExists(bucket: string) {
  try {
    await s3.send(new HeadBucketCommand({ Bucket: bucket }));
    return;
  } catch {
    try {
      await s3.send(new CreateBucketCommand({ Bucket: bucket }));
    } catch {
      // ignore if already exists or no permission
    }
  }
}

export async function uploadFileToS3(file: File, key?: string): Promise<{ bucket: string; key: string }> {
  const objectKey = key ?? file.name;
  await ensureBucketExists(S3_BUCKET);
  // Avoid ReadableStream issues in browsers by sending raw bytes
  const bytes = new Uint8Array(await file.arrayBuffer());
  const command = new PutObjectCommand({
    Bucket: S3_BUCKET,
    Key: objectKey,
    Body: bytes,
    ContentType: file.type || 'application/octet-stream'
  });
  await s3.send(command);
  return { bucket: S3_BUCKET, key: objectKey };
}

export async function getSignedObjectUrl(key: string, expiresInSeconds = 3600): Promise<string> {
  const cmd = new GetObjectCommand({ Bucket: S3_BUCKET, Key: key });
  return await getSignedUrl(s3, cmd, { expiresIn: expiresInSeconds });
}


