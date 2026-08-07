import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getActiveWorkoutSession, type WorkoutSessionDto } from '../../api/workoutSessionsApi'
import { EmptyState } from '../../components/EmptyState'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoCard } from '../../components/NeoCard'
import { PageShell } from '../../components/PageShell'

function formatStartedAt(startedAt: string) {
  const date = new Date(startedAt)

  if (Number.isNaN(date.getTime())) {
    return startedAt
  }

  return new Intl.DateTimeFormat('vi-VN', {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(date)
}

function getCurrentExercisePosition(session: WorkoutSessionDto) {
  const exercises = session.exercises ?? []
  const currentIndex = exercises.findIndex(
    (exercise) => exercise.id === session.currentWorkoutSessionExerciseId,
  )

  if (currentIndex < 0) {
    return null
  }

  return `${currentIndex + 1}/${exercises.length}`
}

export function WorkoutPage() {
  const [activeSession, setActiveSession] = useState<WorkoutSessionDto | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    async function loadActiveSession() {
      try {
        setIsLoading(true)
        setErrorMessage(null)

        const session = await getActiveWorkoutSession()

        setActiveSession(session ?? null)
      } catch (error) {
        setActiveSession(null)
        setErrorMessage(
          error instanceof Error
            ? error.message
            : 'Không thể tải buổi tập hiện tại.',
        )
      } finally {
        setIsLoading(false)
      }
    }

    void loadActiveSession()
  }, [])

  const exerciseCount = activeSession?.exercises?.length ?? 0
  const currentExercisePosition = activeSession
    ? getCurrentExercisePosition(activeSession)
    : null

  const exerciseLibraryCard = (
    <NeoCard className="training-action-card lime">
      <p className="eyebrow">Exercise Library</p>
      <h2>Thư viện bài tập</h2>
      <p>Tìm bài theo nhóm cơ, thiết bị và thêm vào buổi tập hoặc kế hoạch.</p>

      <Link className="neo-button link-button" to="/exercises">
        Mở thư viện
      </Link>

      <Link className="neo-button link-button secondary-link-button" to="/exercise-picker">
        Mở picker
      </Link>
    </NeoCard>
  )

  if (isLoading) {
    return (
      <PageShell title="Buổi tập">
        <LoadingState message="Đang tải buổi tập hiện tại..." />
      </PageShell>
    )
  }

  if (errorMessage) {
    return (
      <PageShell title="Buổi tập">
        <ErrorState message={errorMessage} />
      </PageShell>
    )
  }

  if (!activeSession) {
    return (
      <PageShell title="Buổi tập">
        <div className="training-action-grid">
          <EmptyState
            title="Chưa có buổi tập đang diễn ra"
            message="Chọn một kế hoạch tập để bắt đầu buổi tập mới."
            action={
              <Link className="neo-button link-button" to="/plans">
                Chọn kế hoạch tập
              </Link>
            }
          />

          {exerciseLibraryCard}
        </div>
      </PageShell>
    )
  }

  return (
    <PageShell title="Buổi tập">
      <div className="training-action-grid">
        <NeoCard className="training-action-card blue">
          <p className="eyebrow">Active Workout</p>
          <h2>{activeSession.name ?? 'Buổi tập hiện tại'}</h2>
          <p>Đang diễn ra từ {formatStartedAt(activeSession.startedAt)}.</p>

          <div className="exercise-tags">
            <span>Đang tập</span>
            <span>{exerciseCount} bài tập</span>
            <span>
              {currentExercisePosition
                ? `Bài hiện tại ${currentExercisePosition}`
                : 'Chưa chọn bài hiện tại'}
            </span>
          </div>

          {exerciseCount === 0 ? (
            <p>Buổi tập này chưa có bài tập.</p>
          ) : (
            <p>Chi tiết bài tập và thao tác trong buổi tập sẽ được bổ sung tiếp theo.</p>
          )}
        </NeoCard>

        {exerciseLibraryCard}
      </div>
    </PageShell>
  )
}
