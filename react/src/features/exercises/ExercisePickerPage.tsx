import { useEffect, useState } from 'react'
import {
  getEquipments,
  getMuscleGroups,
  getSelectableExercises,
  type EquipmentDto,
  type ExerciseDto,
  type MuscleGroupDto,
} from '../../api/exercisesApi'
import { EmptyState } from '../../components/EmptyState'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { NeoInput } from '../../components/NeoInput'
import { NeoSelect } from '../../components/NeoSelect'
import { PageShell } from '../../components/PageShell'
import { useToast } from '../../components/useToast'
import { getNameById } from './exerciseFormatters'
import { Link, useNavigate, useParams, useSearchParams } from 'react-router-dom'
import {
  addWorkoutPlanExercise,
  getWorkoutPlan,
} from '../../api/workoutPlansApi'
import {
  addWorkoutSessionExercise,
  getActiveWorkoutSession,
  type WorkoutSessionDto,
} from '../../api/workoutSessionsApi'

type SelectedExerciseTarget = {
  defaultSets: string
  defaultReps: string
  defaultWeightKg: string
  restSeconds: string
  note: string
}

function createDefaultExerciseTarget(): SelectedExerciseTarget {
  return {
    defaultSets: '3',
    defaultReps: '10',
    defaultWeightKg: '',
    restSeconds: '90',
    note: '',
  }
}

// Validates picker targets before any request is sent, so one invalid selection cannot partially mutate a plan or session.
function getSelectedExerciseTargetError(target: SelectedExerciseTarget) {
  const defaultSets = Number(target.defaultSets)
  const defaultReps = Number(target.defaultReps)
  const defaultWeightKg =
    target.defaultWeightKg.trim().length === 0
      ? null
      : Number(target.defaultWeightKg)
  const restSeconds =
    target.restSeconds.trim().length === 0 ? null : Number(target.restSeconds)

  if (!Number.isInteger(defaultSets) || defaultSets < 1) {
    return 'Số sets mục tiêu phải là số nguyên từ 1.'
  }

  if (!Number.isInteger(defaultReps) || defaultReps < 1) {
    return 'Số reps mục tiêu phải là số nguyên từ 1.'
  }

  if (
    defaultWeightKg !== null &&
    (!Number.isFinite(defaultWeightKg) || defaultWeightKg < 0)
  ) {
    return 'Khối lượng mục tiêu phải lớn hơn hoặc bằng 0.'
  }

  if (
    restSeconds !== null &&
    (!Number.isInteger(restSeconds) || restSeconds < 0)
  ) {
    return 'Thời gian nghỉ phải là số nguyên từ 0.'
  }

  if (target.note.trim().length > 500) {
    return 'Ghi chú không được dài quá 500 ký tự.'
  }

  return null
}


export function ExercisePickerPage() {
  const { planId } = useParams()
  const [searchParams] = useSearchParams()
  const destinationMode = planId
    ? 'plan'
    : searchParams.get('mode') === 'session'
      ? 'session'
      : null
  const isSessionMode = destinationMode === 'session'
  const navigate = useNavigate()
  const { showToast } = useToast()
  const [selectedExerciseTargets, setSelectedExerciseTargets] = useState<
    Record<string, SelectedExerciseTarget>
  >({})
  const selectedExerciseIds = Object.keys(selectedExerciseTargets)
  const [exercises, setExercises] = useState<ExerciseDto[]>([])
  const [muscleGroups, setMuscleGroups] = useState<MuscleGroupDto[]>([])
  const [equipments, setEquipments] = useState<EquipmentDto[]>([])
  const [existingExerciseIds, setExistingExerciseIds] = useState<string[]>([])
  const [activeSession, setActiveSession] = useState<WorkoutSessionDto | null>(null)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)

  async function handleAddExercises() {
    if (isSubmitting || selectedExerciseIds.length === 0) {
      return
    }

    if (!destinationMode) {
      setErrorMessage('Mở picker từ một kế hoạch hoặc buổi tập đang diễn ra.')
      return
    }

    for (const exerciseId of selectedExerciseIds) {
      const targetError = getSelectedExerciseTargetError(
        selectedExerciseTargets[exerciseId],
      )
      if (targetError) {
        setErrorMessage(targetError)
        return
      }
    }

    try {
      setIsSubmitting(true)
      setErrorMessage(null)

      if (isSessionMode) {
        const session = await getActiveWorkoutSession()
        if (!session) {
          setActiveSession(null)
          setErrorMessage('Không có buổi tập đang diễn ra để thêm bài tập.')
          return
        }

        const sessionExercises = session.exercises ?? []
        const existingIds = new Set(sessionExercises.map((exercise) => exercise.exerciseId))
        const duplicateExerciseIds = selectedExerciseIds.filter((exerciseId) =>
          existingIds.has(exerciseId),
        )

        if (duplicateExerciseIds.length > 0) {
          setExistingExerciseIds(Array.from(existingIds))
          setSelectedExerciseTargets((currentTargets) => {
            const nextTargets = { ...currentTargets }
            duplicateExerciseIds.forEach((exerciseId) => delete nextTargets[exerciseId])
            return nextTargets
          })
          setErrorMessage('Một hoặc nhiều bài tập đã có trong buổi tập và đã được bỏ chọn.')
          return
        }

        const existingExercisesCount = sessionExercises.length
        for (const [index, exerciseId] of selectedExerciseIds.entries()) {
          const target = selectedExerciseTargets[exerciseId]
          await addWorkoutSessionExercise(session.id, {
            exerciseId,
            orderIndex: existingExercisesCount + index + 1,
            targetSets: Number(target.defaultSets),
            targetReps: Number(target.defaultReps),
            targetWeightKg:
              target.defaultWeightKg.trim().length === 0
                ? null
                : Number(target.defaultWeightKg),
            restSeconds:
              target.restSeconds.trim().length === 0
                ? null
                : Number(target.restSeconds),
            note: target.note.trim().length === 0 ? null : target.note.trim(),
          })
        }

        showToast(
          selectedExerciseIds.length === 1
            ? 'Đã thêm bài tập vào buổi tập.'
            : `Đã thêm ${selectedExerciseIds.length} bài tập vào buổi tập.`,
        )
        navigate('/workout', { replace: true })
        return
      }

      if (!planId) {
        setErrorMessage('Không xác định được kế hoạch cần thêm bài tập.')
        return
      }

      const plan = await getWorkoutPlan(planId)
      const planExercises = plan.exercises ?? []
      const existingIds = new Set(planExercises.map((exercise) => exercise.exerciseId))
      const duplicateExerciseIds = selectedExerciseIds.filter((exerciseId) =>
        existingIds.has(exerciseId),
      )

      if (duplicateExerciseIds.length > 0) {
        setExistingExerciseIds(Array.from(existingIds))
        setSelectedExerciseTargets((currentTargets) => {
          const nextTargets = { ...currentTargets }
          duplicateExerciseIds.forEach((exerciseId) => delete nextTargets[exerciseId])
          return nextTargets
        })
        setErrorMessage('Một hoặc nhiều bài tập đã có trong kế hoạch và đã được bỏ chọn.')
        return
      }

      for (const [index, exerciseId] of selectedExerciseIds.entries()) {
        const target = selectedExerciseTargets[exerciseId]

        await addWorkoutPlanExercise(planId, {
          exerciseId,
          orderIndex: planExercises.length + index + 1,
          defaultSets: Number(target.defaultSets),
          defaultReps: Number(target.defaultReps),
          defaultWeightKg:
            target.defaultWeightKg.trim().length === 0
              ? null
              : Number(target.defaultWeightKg),
          restSeconds:
            target.restSeconds.trim().length === 0
              ? null
              : Number(target.restSeconds),
          note: target.note.trim().length === 0 ? null : target.note.trim(),
        })
      }

      showToast(
        selectedExerciseIds.length === 1
          ? 'Đã thêm bài tập vào kế hoạch.'
          : `Đã thêm ${selectedExerciseIds.length} bài tập vào kế hoạch.`,
      )
      navigate(`/plans/${planId}`, { replace: true })
    } catch (error) {
      setErrorMessage(
        error instanceof Error
          ? error.message
          : isSessionMode
            ? 'Không thể thêm bài tập vào buổi tập.'
            : 'Không thể thêm bài tập vào kế hoạch.',
      )
    } finally {
      setIsSubmitting(false)
    }
  }

  useEffect(() => {
    let cancelled = false

    async function loadPickerData() {
      try {
        setIsLoading(true)
        setErrorMessage(null)
        setSelectedExerciseTargets({})
        setActiveSession(null)

        const [exerciseResult, muscleGroupResult, equipmentResult] = await Promise.all([
          getSelectableExercises({
            IsActive: true,
            MaxResultCount: 100,
          }),
          getMuscleGroups(),
          getEquipments(),
        ])

        if (cancelled) {
          return
        }

        setExercises(exerciseResult.items ?? [])
        setMuscleGroups(muscleGroupResult.items ?? [])
        setEquipments(equipmentResult.items ?? [])

        if (destinationMode === 'plan' && planId) {
          const plan = await getWorkoutPlan(planId)
          if (cancelled) {
            return
          }
          setExistingExerciseIds(
            (plan.exercises ?? []).map((exercise) => exercise.exerciseId),
          )
        } else if (isSessionMode) {
          const session = await getActiveWorkoutSession()
          if (cancelled) {
            return
          }
          setActiveSession(session)
          setExistingExerciseIds(
            (session?.exercises ?? []).map((exercise) => exercise.exerciseId),
          )
        } else {
          setExistingExerciseIds([])
        }
      } catch (error) {
        if (cancelled) {
          return
        }
        setExercises([])
        setErrorMessage(
          error instanceof Error ? error.message : 'Không thể tải danh sách bài tập.',
        )
      } finally {
        if (!cancelled) {
          setIsLoading(false)
        }
      }
    }

    void loadPickerData()
    return () => {
      cancelled = true
    }
  }, [destinationMode, isSessionMode, planId, reloadToken])

    const [filterText, setFilterText] = useState('')
    const [muscleGroupFilter, setMuscleGroupFilter] = useState('')
    const [equipmentFilter, setEquipmentFilter] = useState('')

    // Re-runs the picker data requests after a transient backend or network failure.
    function retryLoad() {
      setReloadToken((currentToken) => currentToken + 1)
    }
    function toggleExercise(exerciseId: string) {
      if (isSubmitting || existingExerciseIds.includes(exerciseId)) {
        return
      }

      const isSelected = exerciseId in selectedExerciseTargets

      if (isSelected) {
        const nextTargets = { ...selectedExerciseTargets }
        delete nextTargets[exerciseId]

        setSelectedExerciseTargets(nextTargets)
        return
      }

      setSelectedExerciseTargets({
        ...selectedExerciseTargets,
        [exerciseId]: createDefaultExerciseTarget(),
      })
    }

    function updateExerciseTarget(
      exerciseId: string,
      field: keyof SelectedExerciseTarget,
      value: string,
    ) {
      if (isSubmitting) {
        return
      }

      setSelectedExerciseTargets({
        ...selectedExerciseTargets,
        [exerciseId]: {
          ...selectedExerciseTargets[exerciseId],
          [field]: value,
        },
      })
    }
    const normalizedFilterText = filterText.trim().toLowerCase()

    const hasActiveFilters =
  normalizedFilterText.length > 0 ||
  muscleGroupFilter.length > 0 ||
  equipmentFilter.length > 0

const filteredExercises = exercises.filter((exercise) => {
  const matchesText =
    normalizedFilterText.length === 0 ||
    (exercise.name ?? '').toLowerCase().includes(normalizedFilterText)

  const matchesMuscleGroup =
    muscleGroupFilter.length === 0 ||
    exercise.primaryMuscleGroupId === muscleGroupFilter

  const matchesEquipment =
    equipmentFilter.length === 0 ||
    exercise.equipmentId === equipmentFilter

  return matchesText && matchesMuscleGroup && matchesEquipment
})
const hasDestination =
  destinationMode === 'plan' || (isSessionMode && activeSession !== null)
return (
    <PageShell title={isSessionMode ? 'Thêm bài vào buổi' : 'Chọn bài tập'}>
      <Link className="back-link" to={planId ? `/plans/${planId}` : '/workout'}>
        ← {planId ? 'Quay lại kế hoạch' : 'Quay lại buổi tập'}
      </Link>

      <section className="exercise-filter-panel">
        <NeoInput
        label="Tìm bài tập"
        placeholder="Bench press, squat..."
        value={filterText}
        disabled={isLoading || isSubmitting}
        onChange={(event) => setFilterText(event.target.value)}
        />

        <div className="exercise-filter-grid">
          <NeoSelect
            label="Nhóm cơ"
            value={muscleGroupFilter}
            disabled={isLoading || isSubmitting}
            onChange={(event) => setMuscleGroupFilter(event.target.value)}
            options={[
              { label: 'Tất cả', value: '' },
              ...muscleGroups.map((muscleGroup) => ({
                label: muscleGroup.name ?? 'Không tên',
                value: muscleGroup.id,
              })),
            ]}
            />

          <NeoSelect
            label="Thiết bị"
            value={equipmentFilter}
            disabled={isLoading || isSubmitting}
            onChange={(event) => setEquipmentFilter(event.target.value)}
            options={[
              { label: 'Tất cả', value: '' },
              ...equipments.map((equipment) => ({
                label: equipment.name ?? 'Không tên',
                value: equipment.id,
              })),
            ]}
            />
        </div>
      </section>

      {isLoading ? (
          <LoadingState message="Đang tải danh sách bài tập..." />
        ) : errorMessage ? (
          <ErrorState
            message={errorMessage}
            action={<NeoButton onClick={retryLoad}>Thử lại</NeoButton>}
          />
        ) : !destinationMode ? (
          <EmptyState
            title="Chưa chọn nơi thêm bài tập"
            message="Mở picker từ một kế hoạch hoặc từ buổi tập đang diễn ra để lưu lựa chọn."
            action={
              <Link className="neo-button link-button" to="/workout">
                Quay lại buổi tập
              </Link>
            }
          />
        ) : isSessionMode && !activeSession ? (
          <EmptyState
            title="Chưa có buổi tập đang diễn ra"
            message="Bắt đầu hoặc tiếp tục một buổi tập trước khi thêm bài tập."
            action={
              <Link className="neo-button link-button" to="/workout">
                Quay lại buổi tập
              </Link>
            }
          />
        ) : filteredExercises.length === 0 ? (
          <EmptyState
            title={hasActiveFilters ? 'Không tìm thấy bài' : 'Chưa có bài tập để chọn'}
            message={
              hasActiveFilters
                ? 'Thử đổi từ khoá, nhóm cơ hoặc thiết bị.'
                : 'Backend đang trả danh sách rỗng. Hãy thêm hoặc seed dữ liệu bài tập.'
            }
          />
        ) : (
        <section className="picker-list">
            {filteredExercises.map((exercise) => {
            const isSelected = selectedExerciseIds.includes(exercise.id)
            const isAlreadyInDestination = existingExerciseIds.includes(exercise.id)

            return (
                <NeoCard
                key={exercise.id}
                className={
                  isAlreadyInDestination
                    ? 'picker-card already-in-plan'
                    : isSelected
                      ? 'picker-card selected'
                      : 'picker-card'
                }
                >
                <h2>{exercise.name ?? 'Bài tập chưa đặt tên'}</h2>
                <div className="exercise-tags">
                  <span>{getNameById(muscleGroups, exercise.primaryMuscleGroupId)}</span>
                  <span>{getNameById(equipments, exercise.equipmentId)}</span>
                  {isAlreadyInDestination ? (
                    <span>{isSessionMode ? 'Đã có trong buổi' : 'Đã có trong plan'}</span>
                  ) : null}
                </div>

                <button
                    className={
                      isAlreadyInDestination
                        ? 'picker-toggle already-in-plan'
                        : isSelected
                          ? 'picker-toggle selected'
                          : 'picker-toggle'
                    }
                    type="button"
                    disabled={isAlreadyInDestination || isSubmitting}
                    onClick={() => toggleExercise(exercise.id)}
                    aria-label={
                      isAlreadyInDestination
                        ? isSessionMode
                          ? 'Bài tập đã có trong buổi tập'
                          : 'Bài tập đã có trong plan'
                        : isSelected
                          ? 'Bỏ chọn bài tập'
                          : 'Chọn bài tập'
                    }
                >
                    {isAlreadyInDestination ? '✓' : isSelected ? '✓' : '+'}
                </button>
                {isSelected ? (
                  <div className="picker-target-form">
                    <NeoInput
                      label="Sets"
                      type="number"
                      min={1}
                      value={selectedExerciseTargets[exercise.id].defaultSets}
                      disabled={isSubmitting}
                      onChange={(event) =>
                        updateExerciseTarget(exercise.id, 'defaultSets', event.target.value)
                      }
                    />

                    <NeoInput
                      label="Reps"
                      type="number"
                      min={1}
                      value={selectedExerciseTargets[exercise.id].defaultReps}
                      disabled={isSubmitting}
                      onChange={(event) =>
                        updateExerciseTarget(exercise.id, 'defaultReps', event.target.value)
                      }
                    />

                    <NeoInput
                      label="Kg"
                      type="number"
                      min={0}
                      placeholder="Tuỳ chọn"
                      value={selectedExerciseTargets[exercise.id].defaultWeightKg}
                      disabled={isSubmitting}
                      onChange={(event) =>
                        updateExerciseTarget(exercise.id, 'defaultWeightKg', event.target.value)
                      }
                    />

                    <NeoInput
                      label="Nghỉ"
                      type="number"
                      min={0}
                      placeholder="Giây"
                      value={selectedExerciseTargets[exercise.id].restSeconds}
                      disabled={isSubmitting}
                      onChange={(event) =>
                        updateExerciseTarget(exercise.id, 'restSeconds', event.target.value)
                      }
                    />

                    <NeoInput
                      label="Ghi chú"
                      placeholder="Tempo, form cue..."
                      value={selectedExerciseTargets[exercise.id].note}
                      disabled={isSubmitting}
                      onChange={(event) =>
                        updateExerciseTarget(exercise.id, 'note', event.target.value)
                      }
                    />
                  </div>
                ) : null}
                </NeoCard>
            )
            })}
        </section>
        )}

      
      {hasDestination ? (
        <div className="picker-footer" aria-busy={isSubmitting}>
          <NeoButton
            className="full-width-button"
            disabled={selectedExerciseIds.length === 0 || isSubmitting}
            onClick={handleAddExercises}
          >
            {isSubmitting
              ? 'Đang thêm...'
              : `${isSessionMode ? 'Thêm vào buổi' : 'Thêm vào kế hoạch'} (${selectedExerciseIds.length})`}
          </NeoButton>
        </div>
      ) : null}
    </PageShell>
  )
}
