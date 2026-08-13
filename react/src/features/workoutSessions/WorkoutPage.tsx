import { useEffect, useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import {
  getSelectableExercises,
  getExercise,
  type ExerciseDto,
} from '../../api/exercisesApi'
import {
  addExerciseSet,
  cancelWorkoutSession,
  completeWorkoutSession,
  completeExerciseSet,
  getActiveWorkoutSession,
  getCurrentWorkoutSessionExercise,
  moveToNextWorkoutSessionExercise,
  moveToPreviousWorkoutSessionExercise,
  removeExerciseSet,
  skipCurrentWorkoutSessionExercise,
  uncompleteExerciseSet,
  skipExerciseSet,
  unskipExerciseSet,
  updateExerciseSet,
  addWorkoutSessionExercise,
  updateWorkoutSessionExercise,
  removeWorkoutSessionExercise,
  type AddWorkoutSessionExerciseDto,
  type UpdateWorkoutSessionExerciseDto,
  type AddExerciseSetDto,
  type ExerciseSetDto,
  type UpdateExerciseSetDto,
  type WorkoutSessionDto,
  type WorkoutSessionExerciseDto,
} from '../../api/workoutSessionsApi'
import { EmptyState } from '../../components/EmptyState'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { NeoInput } from '../../components/NeoInput'
import { NeoSelect } from '../../components/NeoSelect'
import { PageShell } from '../../components/PageShell'

type SetDraft = {
  setNumber: string
  weightKg: string
  reps: string
  rpe: string
  note: string
}

type SessionExerciseDraft = {
  exerciseId: string
  targetSets: string
  targetReps: string
  targetWeightKg: string
  restSeconds: string
  note: string
}

function createSessionExerciseDraft(exerciseId = ''): SessionExerciseDraft {
  return {
    exerciseId,
    targetSets: '3',
    targetReps: '10',
    targetWeightKg: '',
    restSeconds: '90',
    note: '',
  }
}

function createSessionExerciseDraftFromExercise(
  exercise: WorkoutSessionExerciseDto,
): SessionExerciseDraft {
  return {
    exerciseId: exercise.exerciseId,
    targetSets: String(exercise.targetSets),
    targetReps: String(exercise.targetReps),
    targetWeightKg:
      exercise.targetWeightKg === null ? '' : String(exercise.targetWeightKg),
    restSeconds: exercise.restSeconds === null ? '' : String(exercise.restSeconds),
    note: exercise.note ?? '',
  }
}

function createSetDraft(
  setNumber: number,
  targetWeightKg: number | null = null,
  targetReps = 1,
): SetDraft {
  return {
    setNumber: String(setNumber),
    weightKg: targetWeightKg === null ? '0' : String(targetWeightKg),
    reps: String(targetReps),
    rpe: '',
    note: '',
  }
}

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

function sortSessionExercises(exercises: WorkoutSessionExerciseDto[]) {
  return [...exercises].sort((first, second) => first.orderIndex - second.orderIndex)
}

function getExerciseStatusLabel(status: WorkoutSessionExerciseDto['status']) {
  switch (status) {
    case 1:
      return 'Đang tập'
    case 2:
      return 'Hoàn thành'
    case 3:
      return 'Đã bỏ qua'
    default:
      return 'Chờ tập'
  }
}

function getSessionStatusLabel(status: WorkoutSessionDto['status']) {
  switch (status) {
    case 1:
      return 'Đã hoàn thành'
    case 2:
      return 'Đã huỷ'
    default:
      return 'Đang tập'
  }
}

function getSetStatusLabel(set: ExerciseSetDto) {
  if (set.isCompleted) {
    return 'Đã hoàn thành'
  }

  if (set.isSkipped) {
    return 'Đã bỏ qua'
  }

  return 'Chưa hoàn thành'
}

function getSetDraftError(draft: SetDraft, mode: 'add' | 'update') {
  const setNumber = Number(draft.setNumber)
  const weightKg = Number(draft.weightKg)
  const reps = Number(draft.reps)
  const rpe = draft.rpe.trim().length > 0 ? Number(draft.rpe) : null

  if (!Number.isInteger(setNumber) || setNumber < 1) {
    return 'Set number phải là số nguyên từ 1.'
  }

  if (!Number.isFinite(weightKg) || weightKg < 0) {
    return 'Khối lượng phải là số lớn hơn hoặc bằng 0.'
  }

  if (!Number.isInteger(reps) || reps < 1) {
    return 'Số reps phải là số nguyên từ 1.'
  }

  const minimumRpe = mode === 'add' ? 1 : 0
  if (rpe !== null && (!Number.isInteger(rpe) || rpe < minimumRpe || rpe > 10)) {
    return `RPE phải nằm trong khoảng ${minimumRpe} đến 10.`
  }

  return null
}

function getSessionExerciseDraftError(draft: SessionExerciseDraft) {
  const targetSets = Number(draft.targetSets)
  const targetReps = Number(draft.targetReps)
  const targetWeightKg = draft.targetWeightKg.trim().length > 0
    ? Number(draft.targetWeightKg)
    : null
  const restSeconds = draft.restSeconds.trim().length > 0
    ? Number(draft.restSeconds)
    : null

  if (!draft.exerciseId) {
    return 'Chọn một bài tập để thêm vào buổi tập.'
  }

  if (!Number.isInteger(targetSets) || targetSets < 1) {
    return 'Số sets mục tiêu phải là số nguyên từ 1.'
  }

  if (!Number.isInteger(targetReps) || targetReps < 1) {
    return 'Số reps mục tiêu phải là số nguyên từ 1.'
  }

  if (targetWeightKg !== null && (!Number.isFinite(targetWeightKg) || targetWeightKg < 0)) {
    return 'Khối lượng mục tiêu phải lớn hơn hoặc bằng 0.'
  }

  if (restSeconds !== null || draft.restSeconds.trim().length > 0) {
    if (restSeconds === null || !Number.isInteger(restSeconds) || restSeconds < 0) {
      return 'Thời gian nghỉ phải là số nguyên từ 0.'
    }
  }

  return null
}

function toSessionExerciseInput(
  draft: SessionExerciseDraft,
  orderIndex: number,
): AddWorkoutSessionExerciseDto {
  return {
    exerciseId: draft.exerciseId,
    orderIndex,
    targetSets: Number(draft.targetSets),
    targetReps: Number(draft.targetReps),
    targetWeightKg:
      draft.targetWeightKg.trim().length === 0
        ? null
        : Number(draft.targetWeightKg),
    restSeconds:
      draft.restSeconds.trim().length === 0
        ? null
        : Number(draft.restSeconds),
    note: draft.note.trim() || null,
  }
}

function toSessionExerciseUpdateInput(
  draft: SessionExerciseDraft,
  orderIndex: number,
): UpdateWorkoutSessionExerciseDto {
  return {
    orderIndex,
    targetSets: Number(draft.targetSets),
    targetReps: Number(draft.targetReps),
    targetWeightKg:
      draft.targetWeightKg.trim().length === 0
        ? null
        : Number(draft.targetWeightKg),
    restSeconds:
      draft.restSeconds.trim().length === 0
        ? null
        : Number(draft.restSeconds),
    note: draft.note.trim() || null,
  }
}

function toSetInput(draft: SetDraft): AddExerciseSetDto {
  const rpe = draft.rpe.trim().length > 0 ? Number(draft.rpe) : null

  return {
    setNumber: Number(draft.setNumber),
    weightKg: Number(draft.weightKg),
    reps: Number(draft.reps),
    rpe,
    note: draft.note.trim() || null,
  }
}

function toUpdateSetInput(draft: SetDraft): UpdateExerciseSetDto {
  return toSetInput(draft)
}

function getNextSetNumber(exercise: WorkoutSessionExerciseDto | null) {
  const sets = exercise?.sets ?? []
  return sets.reduce((highest, set) => Math.max(highest, set.setNumber), 0) + 1
}

function getSetDraftFromSet(set: ExerciseSetDto) {
  return {
    setNumber: String(set.setNumber),
    weightKg: String(set.weightKg),
    reps: String(set.reps),
    rpe: set.rpe === null ? '' : String(set.rpe),
    note: set.note ?? '',
  }
}

export function WorkoutPage() {
  const [activeSession, setActiveSession] = useState<WorkoutSessionDto | null>(null)
  const [currentExercise, setCurrentExercise] =
    useState<WorkoutSessionExerciseDto | null>(null)
  const [exercise, setExercise] = useState<ExerciseDto | null>(null)
  const [exerciseCatalog, setExerciseCatalog] = useState<ExerciseDto[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [actionError, setActionError] = useState<string | null>(null)
  const [sessionOutcomeMessage, setSessionOutcomeMessage] = useState<string | null>(null)
  const [actionLoading, setActionLoading] = useState<string | null>(null)
  const [setDraft, setSetDraft] = useState<SetDraft>(createSetDraft(1))
  const [editingSetId, setEditingSetId] = useState<string | null>(null)
  const [editingSetDraft, setEditingSetDraft] = useState<SetDraft | null>(null)
  const [sessionExerciseDraft, setSessionExerciseDraft] = useState(
    createSessionExerciseDraft(),
  )
  const [editingExerciseId, setEditingExerciseId] = useState<string | null>(null)
  const [editingExerciseDraft, setEditingExerciseDraft] =
    useState<SessionExerciseDraft | null>(null)

  async function loadCurrentExercise(session: WorkoutSessionDto | null) {
    setCurrentExercise(null)
    setExercise(null)
    setEditingSetId(null)
    setEditingSetDraft(null)
    setEditingExerciseId(null)
    setEditingExerciseDraft(null)

    if (!session?.currentWorkoutSessionExerciseId) {
      setSetDraft(createSetDraft(1))
      return
    }

    const nextExercise = await getCurrentWorkoutSessionExercise(session.id)
    setCurrentExercise(nextExercise)
    setSetDraft(
      createSetDraft(
        getNextSetNumber(nextExercise),
        nextExercise.targetWeightKg,
        nextExercise.targetReps,
      ),
    )

    try {
      setExercise(await getExercise(nextExercise.exerciseId))
    } catch {
      // The session remains usable when the optional exercise name lookup fails.
      setExercise(null)
    }
  }

  async function applySession(session: WorkoutSessionDto | null) {
    setActiveSession(session)
    await loadCurrentExercise(session)
  }

  async function loadActiveSession() {
    try {
      setIsLoading(true)
      setErrorMessage(null)
      setActionError(null)
      setSessionOutcomeMessage(null)

      const session = await getActiveWorkoutSession()
      if (session) {
        try {
          const exerciseResult = await getSelectableExercises({
            IsActive: true,
            MaxResultCount: 200,
          })
          setExerciseCatalog(exerciseResult.items ?? [])
        } catch {
          setExerciseCatalog([])
        }
      } else {
        setExerciseCatalog([])
      }
      await applySession(session ?? null)
    } catch (error) {
      setActiveSession(null)
      setCurrentExercise(null)
      setErrorMessage(
        error instanceof Error
          ? error.message
          : 'Không thể tải buổi tập hiện tại.',
      )
    } finally {
      setIsLoading(false)
    }
  }

  useEffect(() => {
    void loadActiveSession()
  }, [])

  async function runSessionAction(
    actionName: string,
    action: (sessionId: string) => Promise<WorkoutSessionDto>,
  ): Promise<boolean> {
    if (!activeSession) {
      return false
    }

    if (activeSession.status !== 0 && !actionName.endsWith('-session')) {
      setActionError('Buổi tập này đã kết thúc và đang ở trạng thái chỉ đọc.')
      return false
    }

    try {
      setActionLoading(actionName)
      setActionError(null)
      const nextSession = await action(activeSession.id)
      await applySession(nextSession)
      return true
    } catch (error) {
      setActionError(
        error instanceof Error ? error.message : 'Không thể cập nhật buổi tập.',
      )
      return false
    } finally {
      setActionLoading(null)
    }
  }

  async function handleAddSet(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!activeSession || !currentExercise) {
      return
    }

    const validationError = getSetDraftError(setDraft, 'add')
    if (validationError) {
      setActionError(validationError)
      return
    }

    await runSessionAction('add-set', (sessionId) =>
      addExerciseSet(sessionId, currentExercise.id, toSetInput(setDraft)),
    )
  }

  async function handleUpdateSet(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!activeSession || !currentExercise || !editingSetId || !editingSetDraft) {
      return
    }

    const validationError = getSetDraftError(editingSetDraft, 'update')
    if (validationError) {
      setActionError(validationError)
      return
    }

    const didUpdate = await runSessionAction('update-set', (sessionId) =>
      updateExerciseSet(
        sessionId,
        currentExercise.id,
        editingSetId,
        toUpdateSetInput(editingSetDraft),
      ),
    )
    if (didUpdate) {
      setEditingSetId(null)
      setEditingSetDraft(null)
    }
  }

  async function handleRemoveSet(set: ExerciseSetDto) {
    if (!activeSession || !currentExercise) {
      return
    }

    if (!window.confirm(`Xoá set ${set.setNumber}?`)) {
      return
    }

    await runSessionAction('remove-set', (sessionId) =>
      removeExerciseSet(sessionId, currentExercise.id, set.id),
    )
  }

  async function handleToggleSet(set: ExerciseSetDto) {
    if (!activeSession || !currentExercise) {
      return
    }

    await runSessionAction(set.isCompleted ? 'uncomplete-set' : 'complete-set', (sessionId) =>
      set.isCompleted
        ? uncompleteExerciseSet(sessionId, currentExercise.id, set.id)
        : completeExerciseSet(sessionId, currentExercise.id, set.id),
    )
  }

  // Toggles whether the selected set is intentionally skipped or available to perform again.
  async function handleToggleSkippedSet(set: ExerciseSetDto) {
    if (!activeSession || !currentExercise) {
      return
    }

    await runSessionAction(set.isSkipped ? 'unskip-set' : 'skip-set', (sessionId) =>
      set.isSkipped
        ? unskipExerciseSet(sessionId, currentExercise.id, set.id)
        : skipExerciseSet(sessionId, currentExercise.id, set.id),
    )
  }

  async function handleAddSessionExercise(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!activeSession) {
      return
    }

    const validationError = getSessionExerciseDraftError(sessionExerciseDraft)
    if (validationError) {
      setActionError(validationError)
      return
    }

    const sessionExercises = activeSession.exercises ?? []
    const nextOrderIndex = sessionExercises.reduce(
      (highest, item) => Math.max(highest, item.orderIndex),
      0,
    ) + 1

    const didAdd = await runSessionAction('add-session-exercise', (sessionId) =>
      addWorkoutSessionExercise(
        sessionId,
        toSessionExerciseInput(sessionExerciseDraft, nextOrderIndex),
      ),
    )

    if (didAdd) {
      setSessionExerciseDraft(createSessionExerciseDraft())
    }
  }

  async function handleUpdateSessionExercise(
    event: FormEvent<HTMLFormElement>,
    sessionExercise: WorkoutSessionExerciseDto,
  ) {
    event.preventDefault()

    if (!activeSession || !editingExerciseDraft) {
      return
    }

    const validationError = getSessionExerciseDraftError(editingExerciseDraft)
    if (validationError) {
      setActionError(validationError)
      return
    }

    const didUpdate = await runSessionAction('update-session-exercise', (sessionId) =>
      updateWorkoutSessionExercise(
        sessionId,
        sessionExercise.id,
        toSessionExerciseUpdateInput(editingExerciseDraft, sessionExercise.orderIndex),
      ),
    )

    if (didUpdate) {
      setEditingExerciseId(null)
      setEditingExerciseDraft(null)
    }
  }

  async function handleRemoveSessionExercise(sessionExercise: WorkoutSessionExerciseDto) {
    if (!activeSession) {
      return
    }

    if (sessionExercise.id === activeSession.currentWorkoutSessionExerciseId) {
      setActionError('Hãy chuyển sang bài khác trước khi xoá bài hiện tại.')
      return
    }

    if (!window.confirm('Xoá bài tập này khỏi buổi tập?')) {
      return
    }

    await runSessionAction('remove-session-exercise', (sessionId) =>
      removeWorkoutSessionExercise(sessionId, sessionExercise.id),
    )
  }

  async function handleCompleteSession() {
    if (!activeSession || activeSession.status !== 0) {
      return
    }

    if (!window.confirm('Hoàn thành buổi tập này? Bạn sẽ không thể chỉnh sửa sau đó.')) {
      return
    }

    try {
      setActionLoading('complete-session')
      setActionError(null)
      await completeWorkoutSession(activeSession.id)
      setActiveSession(null)
      setCurrentExercise(null)
      setExercise(null)
      setExerciseCatalog([])
      setSessionOutcomeMessage('Buổi tập đã được hoàn thành và không còn là buổi tập đang diễn ra.')
    } catch (error) {
      setActionError(
        error instanceof Error ? error.message : 'Không thể hoàn thành buổi tập.',
      )
    } finally {
      setActionLoading(null)
    }
  }

  async function handleCancelSession() {
    if (!activeSession || activeSession.status !== 0) {
      return
    }

    if (
      !window.confirm(
        'Huỷ buổi tập này? Buổi tập sẽ chuyển sang trạng thái chỉ đọc.',
      )
    ) {
      return
    }

    try {
      setActionLoading('cancel-session')
      setActionError(null)
      await cancelWorkoutSession(activeSession.id)
      setActiveSession(null)
      setCurrentExercise(null)
      setExercise(null)
      setExerciseCatalog([])
      setSessionOutcomeMessage('Buổi tập đã được huỷ và không còn là buổi tập đang diễn ra.')
    } catch (error) {
      setActionError(
        error instanceof Error ? error.message : 'Không thể huỷ buổi tập.',
      )
    } finally {
      setActionLoading(null)
    }
  }

  const sessionExercises = sortSessionExercises(activeSession?.exercises ?? [])
  const currentExerciseIndex = currentExercise
    ? sessionExercises.findIndex((item) => item.id === currentExercise.id)
    : -1
  const currentExercisePosition =
    currentExerciseIndex >= 0
      ? `${currentExerciseIndex + 1}/${sessionExercises.length}`
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
        <ErrorState
          message={errorMessage}
          action={
            <NeoButton onClick={() => void loadActiveSession()}>
              Thử lại
            </NeoButton>
          }
        />
      </PageShell>
    )
  }

  if (!activeSession) {
    return (
      <PageShell title="Buổi tập">
        <div className="training-action-grid">
          <EmptyState
            title={sessionOutcomeMessage ? 'Buổi tập đã kết thúc' : 'Chưa có buổi tập đang diễn ra'}
            message={
              sessionOutcomeMessage ??
              'Chọn một kế hoạch tập để bắt đầu buổi tập mới.'
            }
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

  const sets = currentExercise?.sets ?? []
  const completedSetCount = sets.filter((set) => set.isCompleted).length
  const skippedSetCount = sets.filter((set) => set.isSkipped).length
  const pendingSetCount = sets.length - completedSetCount - skippedSetCount
  const isActionLoading = actionLoading !== null
  const isReadOnly = activeSession.status !== 0
  const sessionExerciseIds = new Set(sessionExercises.map((item) => item.exerciseId))
  const availableExercises = exerciseCatalog.filter(
    (item) => !sessionExerciseIds.has(item.id),
  )

  function getSessionExerciseName(sessionExercise: WorkoutSessionExerciseDto) {
    return (
      exerciseCatalog.find((item) => item.id === sessionExercise.exerciseId)?.name ??
      (sessionExercise.exerciseId === exercise?.id ? exercise.name : null) ??
      'Bài tập chưa đặt tên'
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
            <span>{getSessionStatusLabel(activeSession.status)}</span>
            <span>{sessionExercises.length} bài tập</span>
            <span>
              {currentExercisePosition
                ? `Bài hiện tại ${currentExercisePosition}`
              : 'Chưa chọn bài hiện tại'}
            </span>
          </div>

          {isReadOnly ? (
            <p className="read-only-notice">
              Buổi tập đã kết thúc. Dữ liệu bên dưới chỉ có thể xem.
            </p>
          ) : (
            <div className="session-lifecycle-actions">
              <NeoButton
                type="button"
                disabled={isActionLoading}
                onClick={() => void handleCompleteSession()}
              >
                {actionLoading === 'complete-session'
                  ? 'Đang hoàn thành...'
                  : 'Hoàn thành buổi'}
              </NeoButton>
              <NeoButton
                className="danger-button"
                type="button"
                disabled={isActionLoading}
                onClick={() => void handleCancelSession()}
              >
                {actionLoading === 'cancel-session' ? 'Đang huỷ...' : 'Huỷ buổi'}
              </NeoButton>
            </div>
          )}
        </NeoCard>

        {actionError ? (
          <ErrorState title="Không thể cập nhật buổi tập" message={actionError} />
        ) : null}

        {currentExercise ? (
          <NeoCard className="current-exercise-card">
            <div className="current-exercise-header">
              <div>
                <p className="eyebrow">Current Exercise</p>
                <h2>{exercise?.name ?? 'Bài tập hiện tại'}</h2>
              </div>
              <span className="exercise-status">
                {getExerciseStatusLabel(currentExercise.status)}
              </span>
            </div>

            <div className="exercise-tags">
              <span>{currentExercise.targetSets} sets mục tiêu</span>
              <span>{currentExercise.targetReps} reps mục tiêu</span>
              <span>
                {currentExercise.targetWeightKg === null
                  ? 'Tự chọn kg'
                  : `${currentExercise.targetWeightKg} kg mục tiêu`}
              </span>
              {currentExercise.restSeconds !== null ? (
                <span>Nghỉ {currentExercise.restSeconds} giây</span>
              ) : null}
            </div>

            {currentExercise.note ? <p>{currentExercise.note}</p> : null}

            {isReadOnly ? null : (
              <div className="session-navigation">
                <NeoButton
                  className="plan-exercise-action-button"
                  disabled={isActionLoading || currentExerciseIndex <= 0}
                  onClick={() =>
                    void runSessionAction(
                      'previous-exercise',
                      moveToPreviousWorkoutSessionExercise,
                    )
                  }
                >
                  ← Trước
                </NeoButton>
                <NeoButton
                  className="plan-exercise-action-button"
                  disabled={isActionLoading}
                  onClick={() =>
                    void runSessionAction(
                      'skip-exercise',
                      skipCurrentWorkoutSessionExercise,
                    )
                  }
                >
                  Bỏ qua
                </NeoButton>
                <NeoButton
                  className="plan-exercise-action-button"
                  disabled={
                    isActionLoading ||
                    currentExerciseIndex < 0 ||
                    currentExerciseIndex >= sessionExercises.length - 1
                  }
                  onClick={() =>
                    void runSessionAction(
                      'next-exercise',
                      moveToNextWorkoutSessionExercise,
                    )
                  }
                >
                  Sau →
                </NeoButton>
              </div>
            )}

            <section className="set-management-section">
              <div className="current-exercise-header">
                <div>
                  <p className="eyebrow">Exercise Sets</p>
                  <h3>{sets.length} sets</h3>
                </div>
              </div>

              <div className="exercise-tags set-summary" aria-live="polite">
                <span>{completedSetCount} hoàn thành</span>
                <span>{skippedSetCount} bỏ qua</span>
                <span>{pendingSetCount} chờ thực hiện</span>
              </div>

              {sets.length === 0 ? (
                <p className="set-empty-message">Chưa có set nào. Thêm set đầu tiên bên dưới.</p>
              ) : (
                <div className="set-list">
                  {sets.map((set) => (
                    <NeoCard
                      key={set.id}
                      className={
                        set.isCompleted
                          ? 'set-card completed'
                          : set.isSkipped
                            ? 'set-card skipped'
                            : 'set-card'
                      }
                    >
                      {!isReadOnly && editingSetId === set.id && editingSetDraft ? (
                        <form className="set-form" onSubmit={(event) => void handleUpdateSet(event)} aria-busy={isActionLoading}>
                          <strong>Chỉnh sửa set {set.setNumber}</strong>
                          <div className="set-form-grid">
                            <NeoInput
                              label="Set"
                              type="number"
                              min="1"
                              value={editingSetDraft.setNumber}
                              disabled={isActionLoading}
                              onChange={(event) =>
                                setEditingSetDraft({
                                  ...editingSetDraft,
                                  setNumber: event.target.value,
                                })
                              }
                            />
                            <NeoInput
                              label="Kg"
                              type="number"
                              min="0"
                              step="0.1"
                              value={editingSetDraft.weightKg}
                              disabled={isActionLoading}
                              onChange={(event) =>
                                setEditingSetDraft({
                                  ...editingSetDraft,
                                  weightKg: event.target.value,
                                })
                              }
                            />
                            <NeoInput
                              label="Reps"
                              type="number"
                              min="1"
                              value={editingSetDraft.reps}
                              disabled={isActionLoading}
                              onChange={(event) =>
                                setEditingSetDraft({
                                  ...editingSetDraft,
                                  reps: event.target.value,
                                })
                              }
                            />
                            <NeoInput
                              label="RPE"
                              type="number"
                              min="0"
                              max="10"
                              value={editingSetDraft.rpe}
                              disabled={isActionLoading}
                              onChange={(event) =>
                                setEditingSetDraft({
                                  ...editingSetDraft,
                                  rpe: event.target.value,
                                })
                              }
                            />
                          </div>
                          <NeoInput
                            label="Ghi chú"
                            value={editingSetDraft.note}
                            disabled={isActionLoading}
                            onChange={(event) =>
                              setEditingSetDraft({
                                ...editingSetDraft,
                                note: event.target.value,
                              })
                            }
                          />
                          <div className="set-actions">
                            <NeoButton type="submit" disabled={isActionLoading}>
                              Lưu
                            </NeoButton>
                            <NeoButton
                              type="button"
                              disabled={isActionLoading}
                              onClick={() => {
                                setEditingSetId(null)
                                setEditingSetDraft(null)
                              }}
                            >
                              Huỷ
                            </NeoButton>
                          </div>
                        </form>
                      ) : (
                        <>
                          <div className="set-card-header">
                            <strong>Set {set.setNumber}</strong>
                            <span
                              className={`exercise-status ${
                                set.isCompleted
                                  ? 'completed'
                                  : set.isSkipped
                                    ? 'skipped'
                                    : 'pending'
                              }`}
                            >
                              {getSetStatusLabel(set)}
                            </span>
                          </div>
                          <div className="exercise-tags">
                            <span>{set.weightKg} kg</span>
                            <span>{set.reps} reps</span>
                            {set.rpe !== null ? <span>RPE {set.rpe}</span> : null}
                          </div>
                          {set.note ? <p>{set.note}</p> : null}
                          {set.isSkipped ? (
                            <p className="set-state-note">
                              Set này vẫn được giữ lại để bạn có thể thực hiện lại sau.
                            </p>
                          ) : null}
                          {isReadOnly ? null : (
                            <div className="set-actions">
                              <NeoButton
                                type="button"
                                disabled={isActionLoading || set.isSkipped}
                                onClick={() => void handleToggleSet(set)}
                              >
                                {actionLoading ===
                                  (set.isCompleted ? 'uncomplete-set' : 'complete-set')
                                  ? 'Đang cập nhật...'
                                  : set.isCompleted
                                    ? 'Bỏ hoàn thành'
                                    : 'Hoàn thành'}
                              </NeoButton>
                              <NeoButton
                                type="button"
                                disabled={isActionLoading || set.isCompleted}
                                onClick={() => void handleToggleSkippedSet(set)}
                              >
                                {actionLoading ===
                                  (set.isSkipped ? 'unskip-set' : 'skip-set')
                                  ? 'Đang cập nhật...'
                                  : set.isSkipped
                                    ? 'Thực hiện lại'
                                    : 'Bỏ qua set'}
                              </NeoButton>
                              <NeoButton
                                type="button"
                                disabled={isActionLoading}
                                onClick={() => {
                                  setEditingSetId(set.id)
                                  setEditingSetDraft(getSetDraftFromSet(set))
                                  setActionError(null)
                                }}
                              >
                                Sửa
                              </NeoButton>
                              <NeoButton
                                className="danger-button"
                                type="button"
                                disabled={isActionLoading}
                                onClick={() => void handleRemoveSet(set)}
                              >
                                Xoá
                              </NeoButton>
                            </div>
                          )}
                        </>
                      )}
                    </NeoCard>
                  ))}
                </div>
              )}

              {isReadOnly ? null : (
                <form className="set-form add-set-form" onSubmit={(event) => void handleAddSet(event)} aria-busy={isActionLoading}>
                <p className="eyebrow">Add Set</p>
                <div className="set-form-grid">
                  <NeoInput
                    label="Set"
                    type="number"
                    min="1"
                    value={setDraft.setNumber}
                    disabled={isActionLoading}
                    onChange={(event) =>
                      setSetDraft({ ...setDraft, setNumber: event.target.value })
                    }
                  />
                  <NeoInput
                    label="Kg"
                    type="number"
                    min="0"
                    step="0.1"
                    value={setDraft.weightKg}
                    disabled={isActionLoading}
                    onChange={(event) =>
                      setSetDraft({ ...setDraft, weightKg: event.target.value })
                    }
                  />
                  <NeoInput
                    label="Reps"
                    type="number"
                    min="1"
                    value={setDraft.reps}
                    disabled={isActionLoading}
                    onChange={(event) =>
                      setSetDraft({ ...setDraft, reps: event.target.value })
                    }
                  />
                  <NeoInput
                    label="RPE"
                    type="number"
                    min="0"
                    max="10"
                    value={setDraft.rpe}
                    disabled={isActionLoading}
                    onChange={(event) =>
                      setSetDraft({ ...setDraft, rpe: event.target.value })
                    }
                  />
                </div>
                <NeoInput
                  label="Ghi chú"
                  value={setDraft.note}
                  disabled={isActionLoading}
                  onChange={(event) => setSetDraft({ ...setDraft, note: event.target.value })}
                />
                <NeoButton type="submit" disabled={isActionLoading}>
                  {actionLoading === 'add-set' ? 'Đang thêm...' : 'Thêm set'}
                </NeoButton>
                </form>
              )}
            </section>
          </NeoCard>
        ) : (
          <EmptyState
            title="Chưa có bài hiện tại"
            message="Buổi tập chưa có bài tập hiện tại để ghi set."
          />
        )}

        {isReadOnly ? null : (
          <NeoCard className="session-exercise-manager-card">
          <div>
            <p className="eyebrow">Session Exercises</p>
            <h2>Quản lý bài tập trong buổi</h2>
            <p>Thêm bài tập hoặc chỉnh mục tiêu của từng bài trong buổi hiện tại.</p>
          </div>

          <div className="session-exercise-list">
            {sessionExercises.map((sessionExercise) => {
              const isCurrentExercise =
                sessionExercise.id === activeSession.currentWorkoutSessionExerciseId
              const isEditing = editingExerciseId === sessionExercise.id

              return (
                <NeoCard
                  key={sessionExercise.id}
                  className={isCurrentExercise ? 'session-exercise-row current' : 'session-exercise-row'}
                >
                  <div className="session-exercise-row-header">
                    <div>
                      <p className="eyebrow">Bài #{sessionExercise.orderIndex}</p>
                      <h3>{getSessionExerciseName(sessionExercise)}</h3>
                    </div>
                    <span className="exercise-status">
                      {isCurrentExercise ? 'Hiện tại' : getExerciseStatusLabel(sessionExercise.status)}
                    </span>
                  </div>

                  {isEditing && editingExerciseDraft ? (
                    <form
                      className="set-form"
                      onSubmit={(event) =>
                        void handleUpdateSessionExercise(event, sessionExercise)
                      }
                      aria-busy={isActionLoading}
                    >
                      <div className="set-form-grid">
                        <NeoInput
                          label="Sets"
                          type="number"
                          min="1"
                          value={editingExerciseDraft.targetSets}
                          disabled={isActionLoading}
                          onChange={(event) =>
                            setEditingExerciseDraft({
                              ...editingExerciseDraft,
                              targetSets: event.target.value,
                            })
                          }
                        />
                        <NeoInput
                          label="Reps"
                          type="number"
                          min="1"
                          value={editingExerciseDraft.targetReps}
                          disabled={isActionLoading}
                          onChange={(event) =>
                            setEditingExerciseDraft({
                              ...editingExerciseDraft,
                              targetReps: event.target.value,
                            })
                          }
                        />
                        <NeoInput
                          label="Kg"
                          type="number"
                          min="0"
                          step="0.1"
                          value={editingExerciseDraft.targetWeightKg}
                          disabled={isActionLoading}
                          onChange={(event) =>
                            setEditingExerciseDraft({
                              ...editingExerciseDraft,
                              targetWeightKg: event.target.value,
                            })
                          }
                        />
                        <NeoInput
                          label="Nghỉ"
                          type="number"
                          min="0"
                          value={editingExerciseDraft.restSeconds}
                          disabled={isActionLoading}
                          onChange={(event) =>
                            setEditingExerciseDraft({
                              ...editingExerciseDraft,
                              restSeconds: event.target.value,
                            })
                          }
                        />
                      </div>
                      <NeoInput
                        label="Ghi chú"
                        value={editingExerciseDraft.note}
                        disabled={isActionLoading}
                        onChange={(event) =>
                          setEditingExerciseDraft({
                            ...editingExerciseDraft,
                            note: event.target.value,
                          })
                        }
                      />
                      <div className="set-actions">
                        <NeoButton type="submit" disabled={isActionLoading}>
                          {actionLoading === 'update-session-exercise'
                            ? 'Đang lưu...'
                            : 'Lưu mục tiêu'}
                        </NeoButton>
                        <NeoButton
                          type="button"
                          disabled={isActionLoading}
                          onClick={() => {
                            setEditingExerciseId(null)
                            setEditingExerciseDraft(null)
                          }}
                        >
                          Huỷ
                        </NeoButton>
                      </div>
                    </form>
                  ) : (
                    <>
                      <div className="exercise-tags">
                        <span>{sessionExercise.targetSets} sets</span>
                        <span>{sessionExercise.targetReps} reps</span>
                        <span>
                          {sessionExercise.targetWeightKg === null
                            ? 'Tự chọn kg'
                            : `${sessionExercise.targetWeightKg} kg`}
                        </span>
                        {sessionExercise.restSeconds !== null ? (
                          <span>Nghỉ {sessionExercise.restSeconds} giây</span>
                        ) : null}
                      </div>
                      <div className="set-actions">
                        <NeoButton
                          type="button"
                          disabled={isActionLoading}
                          onClick={() => {
                            setEditingExerciseId(sessionExercise.id)
                            setEditingExerciseDraft(
                              createSessionExerciseDraftFromExercise(sessionExercise),
                            )
                            setActionError(null)
                          }}
                        >
                          Sửa mục tiêu
                        </NeoButton>
                        <NeoButton
                          className="danger-button"
                          type="button"
                          disabled={isActionLoading || isCurrentExercise}
                          onClick={() => void handleRemoveSessionExercise(sessionExercise)}
                        >
                          Xoá bài
                        </NeoButton>
                      </div>
                      {isCurrentExercise ? (
                        <p className="session-exercise-help">
                          Chuyển sang bài khác trước khi xoá bài hiện tại.
                        </p>
                      ) : null}
                    </>
                  )}
                </NeoCard>
              )
            })}
          </div>

          <form
            className="set-form add-session-exercise-form"
            onSubmit={(event) => void handleAddSessionExercise(event)}
            aria-busy={isActionLoading}
          >
            <p className="eyebrow">Add Exercise</p>
            {availableExercises.length === 0 ? (
              <p className="session-exercise-help">
                Không còn bài tập active nào để thêm, hoặc thư viện chưa tải được.
              </p>
            ) : (
              <NeoSelect
                label="Bài tập"
                value={sessionExerciseDraft.exerciseId}
                disabled={isActionLoading}
                onChange={(event) =>
                  setSessionExerciseDraft({
                    ...sessionExerciseDraft,
                    exerciseId: event.target.value,
                  })
                }
                options={[
                  { label: 'Chọn bài tập', value: '' },
                  ...availableExercises.map((item) => ({
                    label: item.name ?? 'Bài tập chưa đặt tên',
                    value: item.id,
                  })),
                ]}
              />
            )}
            <div className="set-form-grid">
              <NeoInput
                label="Sets"
                type="number"
                min="1"
                value={sessionExerciseDraft.targetSets}
                disabled={isActionLoading}
                onChange={(event) =>
                  setSessionExerciseDraft({
                    ...sessionExerciseDraft,
                    targetSets: event.target.value,
                  })
                }
              />
              <NeoInput
                label="Reps"
                type="number"
                min="1"
                value={sessionExerciseDraft.targetReps}
                disabled={isActionLoading}
                onChange={(event) =>
                  setSessionExerciseDraft({
                    ...sessionExerciseDraft,
                    targetReps: event.target.value,
                  })
                }
              />
              <NeoInput
                label="Kg"
                type="number"
                min="0"
                step="0.1"
                value={sessionExerciseDraft.targetWeightKg}
                disabled={isActionLoading}
                onChange={(event) =>
                  setSessionExerciseDraft({
                    ...sessionExerciseDraft,
                    targetWeightKg: event.target.value,
                  })
                }
              />
              <NeoInput
                label="Nghỉ"
                type="number"
                min="0"
                value={sessionExerciseDraft.restSeconds}
                disabled={isActionLoading}
                onChange={(event) =>
                  setSessionExerciseDraft({
                    ...sessionExerciseDraft,
                    restSeconds: event.target.value,
                  })
                }
              />
            </div>
            <NeoInput
              label="Ghi chú"
              value={sessionExerciseDraft.note}
              disabled={isActionLoading}
              onChange={(event) =>
                setSessionExerciseDraft({
                  ...sessionExerciseDraft,
                  note: event.target.value,
                })
              }
            />
            <NeoButton
              type="submit"
              disabled={isActionLoading || availableExercises.length === 0}
            >
              {actionLoading === 'add-session-exercise'
                ? 'Đang thêm bài...'
                : 'Thêm vào buổi tập'}
            </NeoButton>
          </form>
          </NeoCard>
        )}

        {exerciseLibraryCard}
      </div>
    </PageShell>
  )
}
