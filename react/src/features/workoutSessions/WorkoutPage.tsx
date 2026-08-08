import { useEffect, useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import {
  getExercise,
  type ExerciseDto,
} from '../../api/exercisesApi'
import {
  addExerciseSet,
  completeExerciseSet,
  getActiveWorkoutSession,
  getCurrentWorkoutSessionExercise,
  moveToNextWorkoutSessionExercise,
  moveToPreviousWorkoutSessionExercise,
  removeExerciseSet,
  skipCurrentWorkoutSessionExercise,
  uncompleteExerciseSet,
  updateExerciseSet,
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
import { PageShell } from '../../components/PageShell'

type SetDraft = {
  setNumber: string
  weightKg: string
  reps: string
  rpe: string
  note: string
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
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [actionError, setActionError] = useState<string | null>(null)
  const [actionLoading, setActionLoading] = useState<string | null>(null)
  const [setDraft, setSetDraft] = useState<SetDraft>(createSetDraft(1))
  const [editingSetId, setEditingSetId] = useState<string | null>(null)
  const [editingSetDraft, setEditingSetDraft] = useState<SetDraft | null>(null)

  async function loadCurrentExercise(session: WorkoutSessionDto | null) {
    setCurrentExercise(null)
    setExercise(null)
    setEditingSetId(null)
    setEditingSetDraft(null)

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

      const session = await getActiveWorkoutSession()
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

  const sets = currentExercise?.sets ?? []
  const isActionLoading = actionLoading !== null

  return (
    <PageShell title="Buổi tập">
      <div className="training-action-grid">
        <NeoCard className="training-action-card blue">
          <p className="eyebrow">Active Workout</p>
          <h2>{activeSession.name ?? 'Buổi tập hiện tại'}</h2>
          <p>Đang diễn ra từ {formatStartedAt(activeSession.startedAt)}.</p>

          <div className="exercise-tags">
            <span>Đang tập</span>
            <span>{sessionExercises.length} bài tập</span>
            <span>
              {currentExercisePosition
                ? `Bài hiện tại ${currentExercisePosition}`
                : 'Chưa chọn bài hiện tại'}
            </span>
          </div>
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

            <div className="session-navigation">
              <NeoButton
                className="plan-exercise-action-button"
                disabled={
                  isActionLoading || currentExerciseIndex <= 0
                }
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

            <section className="set-management-section">
              <div className="current-exercise-header">
                <div>
                  <p className="eyebrow">Exercise Sets</p>
                  <h3>{sets.length} sets đã ghi</h3>
                </div>
              </div>

              {sets.length === 0 ? (
                <p className="set-empty-message">Chưa có set nào. Thêm set đầu tiên bên dưới.</p>
              ) : (
                <div className="set-list">
                  {sets.map((set) => (
                    <NeoCard
                      key={set.id}
                      className={set.isCompleted ? 'set-card completed' : 'set-card'}
                    >
                      {editingSetId === set.id && editingSetDraft ? (
                        <form className="set-form" onSubmit={(event) => void handleUpdateSet(event)}>
                          <strong>Chỉnh sửa set {set.setNumber}</strong>
                          <div className="set-form-grid">
                            <NeoInput
                              label="Set"
                              type="number"
                              min="1"
                              value={editingSetDraft.setNumber}
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
                            <span className="exercise-status">
                              {set.isCompleted ? 'Đã hoàn thành' : 'Chưa hoàn thành'}
                            </span>
                          </div>
                          <div className="exercise-tags">
                            <span>{set.weightKg} kg</span>
                            <span>{set.reps} reps</span>
                            {set.rpe !== null ? <span>RPE {set.rpe}</span> : null}
                          </div>
                          {set.note ? <p>{set.note}</p> : null}
                          <div className="set-actions">
                            <NeoButton
                              type="button"
                              disabled={isActionLoading}
                              onClick={() => void handleToggleSet(set)}
                            >
                              {set.isCompleted ? 'Bỏ hoàn thành' : 'Hoàn thành'}
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
                        </>
                      )}
                    </NeoCard>
                  ))}
                </div>
              )}

              <form className="set-form add-set-form" onSubmit={(event) => void handleAddSet(event)}>
                <p className="eyebrow">Add Set</p>
                <div className="set-form-grid">
                  <NeoInput
                    label="Set"
                    type="number"
                    min="1"
                    value={setDraft.setNumber}
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
                    onChange={(event) =>
                      setSetDraft({ ...setDraft, weightKg: event.target.value })
                    }
                  />
                  <NeoInput
                    label="Reps"
                    type="number"
                    min="1"
                    value={setDraft.reps}
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
                    onChange={(event) =>
                      setSetDraft({ ...setDraft, rpe: event.target.value })
                    }
                  />
                </div>
                <NeoInput
                  label="Ghi chú"
                  value={setDraft.note}
                  onChange={(event) => setSetDraft({ ...setDraft, note: event.target.value })}
                />
                <NeoButton type="submit" disabled={isActionLoading}>
                  {actionLoading === 'add-set' ? 'Đang thêm...' : 'Thêm set'}
                </NeoButton>
              </form>
            </section>
          </NeoCard>
        ) : (
          <EmptyState
            title="Chưa có bài hiện tại"
            message="Buổi tập chưa có bài tập hiện tại để ghi set."
          />
        )}

        {exerciseLibraryCard}
      </div>
    </PageShell>
  )
}
