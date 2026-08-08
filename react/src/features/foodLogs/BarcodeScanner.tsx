import { useCallback, useEffect, useRef, useState } from 'react'
import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'

type BarcodeDetectorResult = {
  rawValue?: string
}

type BarcodeDetectorLike = {
  detect(video: HTMLVideoElement): Promise<BarcodeDetectorResult[]>
}

type BarcodeDetectorConstructor = new () => BarcodeDetectorLike

declare global {
  interface Window {
    BarcodeDetector?: BarcodeDetectorConstructor
  }
}

type BarcodeScannerProps = {
  onDetected: (barcode: string) => void
  onClose: () => void
}

type ScannerState = 'starting' | 'scanning' | 'unsupported' | 'error'

/** Stops every camera track so the mobile camera indicator turns off after closing or scanning. */
function stopStream(stream: MediaStream | null) {
  stream?.getTracks().forEach((track) => track.stop())
}

/** Opens the rear camera, verifies a supported decoder, and scans frames until a barcode is found. */
export function BarcodeScanner({ onDetected, onClose }: BarcodeScannerProps) {
  const videoRef = useRef<HTMLVideoElement | null>(null)
  const streamRef = useRef<MediaStream | null>(null)
  const animationFrameRef = useRef<number | null>(null)
  const isDetectingRef = useRef(false)
  const detectorRef = useRef<BarcodeDetectorLike | null>(null)
  const onDetectedRef = useRef(onDetected)
  const [scannerState, setScannerState] = useState<ScannerState>('starting')
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  onDetectedRef.current = onDetected

  // Cancels the animation loop and releases the camera whenever the scanner leaves the page.
  const cleanupScanner = useCallback(() => {
    if (animationFrameRef.current !== null) {
      cancelAnimationFrame(animationFrameRef.current)
      animationFrameRef.current = null
    }

    stopStream(streamRef.current)
    streamRef.current = null

    if (videoRef.current) {
      videoRef.current.srcObject = null
    }
  }, [])

  // Reads video frames one at a time so decoder calls do not overlap on slower mobile devices.
  const scanFrame = useCallback(async function scanFrameLoop() {
    const video = videoRef.current
    const detector = detectorRef.current

    if (!video || !detector || video.readyState < HTMLMediaElement.HAVE_ENOUGH_DATA) {
      animationFrameRef.current = requestAnimationFrame(() => void scanFrameLoop())
      return
    }

    if (!isDetectingRef.current) {
      isDetectingRef.current = true
      try {
        const results = await detector.detect(video)
        const barcode = results.find((result) => result.rawValue?.trim())?.rawValue?.trim()

        if (barcode) {
          cleanupScanner()
          onDetectedRef.current(barcode)
          return
        }
      } catch {
        // A frame can fail to decode while the camera is moving; keep scanning subsequent frames.
      } finally {
        isDetectingRef.current = false
      }
    }

    animationFrameRef.current = requestAnimationFrame(() => void scanFrameLoop())
  }, [cleanupScanner])

  // Requests permission only after the scanner is rendered and maps common browser failures to clear UI text.
  useEffect(() => {
    let isCurrent = true

    async function startScanner() {
      const isLocalDevelopment = ['localhost', '127.0.0.1'].includes(window.location.hostname)
      if (!window.isSecureContext && !isLocalDevelopment) {
        setScannerState('unsupported')
        setErrorMessage('Camera chỉ hoạt động trên HTTPS hoặc localhost.')
        return
      }

      if (!navigator.mediaDevices?.getUserMedia || !window.BarcodeDetector) {
        setScannerState('unsupported')
        setErrorMessage('Trình duyệt này chưa hỗ trợ camera hoặc đọc mã vạch.')
        return
      }

      try {
        detectorRef.current = new window.BarcodeDetector()
        const stream = await navigator.mediaDevices.getUserMedia({
          audio: false,
          video: {
            facingMode: { ideal: 'environment' },
          },
        })

        if (!isCurrent || !videoRef.current) {
          stopStream(stream)
          return
        }

        streamRef.current = stream
        videoRef.current.srcObject = stream
        await videoRef.current.play()
        setScannerState('scanning')
        animationFrameRef.current = requestAnimationFrame(() => void scanFrame())
      } catch (error) {
        if (!isCurrent) {
          return
        }

        cleanupScanner()
        setScannerState('error')
        if (error instanceof DOMException && error.name === 'NotAllowedError') {
          setErrorMessage('Camera bị từ chối. Hãy cấp quyền camera hoặc nhập mã vạch thủ công.')
        } else if (error instanceof DOMException && error.name === 'NotFoundError') {
          setErrorMessage('Không tìm thấy camera trên thiết bị này.')
        } else {
          setErrorMessage('Không thể mở camera. Hãy thử lại hoặc nhập mã vạch thủ công.')
        }
      }
    }

    void startScanner()

    return () => {
      isCurrent = false
      cleanupScanner()
    }
  }, [cleanupScanner, scanFrame])

  return (
    <NeoCard className="barcode-scanner-card">
      <div className="barcode-scanner-header">
        <div>
          <p className="eyebrow">Quét mã vạch</p>
          <h2>{scannerState === 'scanning' ? 'Đưa mã vào khung' : 'Camera scanner'}</h2>
        </div>
        <NeoButton onClick={onClose}>Đóng</NeoButton>
      </div>

      {scannerState === 'scanning' ? (
        <div className="barcode-video-frame">
          <video
            ref={videoRef}
            autoPlay
            muted
            playsInline
            aria-label="Hình ảnh camera để quét mã vạch"
          />
          <span className="barcode-scan-guide" aria-hidden="true" />
        </div>
      ) : (
        <div className="barcode-scanner-message" role="status" aria-live="polite">
          <strong>{scannerState === 'starting' ? 'Đang mở camera...' : 'Không thể quét mã'}</strong>
          <span>{errorMessage ?? 'Vui lòng chờ trong giây lát.'}</span>
        </div>
      )}

      <p className="barcode-scanner-fallback">
        Không quét được? Đóng camera và nhập mã vạch bằng tay bên dưới.
      </p>
    </NeoCard>
  )
}
