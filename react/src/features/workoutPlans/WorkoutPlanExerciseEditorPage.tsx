import { useEffect, useState } from 'react'
import type { SyntheticEvent } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import {
  getSelectableExercises,
  type ExerciseDto,
} from '../../api/exercisesApi'
import {
  getWorkoutPlan,
  updateWorkoutPlanExercise,
  type UpdateWorkoutPlanExerciseDto,
  type WorkoutPlanExerciseDto,
} from '../../api/workoutPlansApi'
import { EmptyState } from '../../components/EmptyState'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { NeoInput } from '../../components/NeoInput'
import { PageShell } from '../../components/PageShell'
import { PermissionGate } from '../../components/PermissionGate'
import { useToast } from '../../components/useToast'
import { FITLOGS_PERMISSIONS } from '../../auth/permissions'

type TargetFormState = {
  orderIndex: string
  defaultSets: string
  defaultReps: string
  defaultWeightKg: string
  restSeconds: string
  note: string
}

type TargetFormErrors = {
  defaultSets?: string
  defaultReps?: string
  defaultWeightKg?: string
  restSeconds?: string
}

function getExerciseName(exercises: ExerciseDto[], exerciseId: string) {
  return exercises.find((exercise) => exercise.id === exerciseId)?.name ?? 'Bài tập không rõ'
}

function toFormState(planExercise: WorkoutPlanExerciseDto): TargetFormState {
  return {
    orderIndex: String(planExercise.orderIndex),
    defaultSets: String(planExercise.defaultSets),
    defaultReps: String(planExercise.defaultReps),
    defaultWeightKg: planExercise.defaultWeightKg === null ? '' : String(planExercise.defaultWeightKg),
    restSeconds: planExercise.restSeconds === null ? '' : String(planExercise.restSeconds),
    note: planExercise.note ?? '',
  }
}

function validateForm(form: TargetFormState): TargetFormErrors {
  const errors: TargetFormErrors = {}

  if (Number(form.defaultSets) < 1) {
    errors.defaultSets = 'Sets phải lớn hơn hoặc bằng 1.'
  }

  if (Number(form.defaultReps) < 1) {
    errors.defaultReps = 'Reps phải lớn hơn hoặc bằng 1.'
  }

  if (form.defaultWeightKg !== '' && Number(form.defaultWeightKg) < 0) {
    errors.defaultWeightKg = 'Kg không được âm.'
  }

  if (form.restSeconds !== '' && Number(form.restSeconds) < 0) {
    errors.restSeconds = 'Thời gian nghỉ không được âm.'
  }

  return errors
}

function toUpdateInput(form: TargetFormState): UpdateWorkoutPlanExerciseDto {
  return {
    orderIndex: Number(form.orderIndex),
    defaultSets: Number(form.defaultSets),
    defaultReps: Number(form.defaultReps),
    defaultWeightKg: form.defaultWeightKg === '' ? null : Number(form.defaultWeightKg),
    restSeconds: form.restSeconds === '' ? null : Number(form.restSeconds),
    note: form.note.trim() || null,
  }
}

export function WorkoutPlanExerciseEditorPage() {
  const { planId, workoutPlanExerciseId } = useParams()
  const navigate = useNavigate()
  const { showToast } = useToast()

  const [planExercise, setPlanExercise] = useState<WorkoutPlanExerciseDto | null>(null)
  const [isPlanArchived, setIsPlanArchived] = useState(false)
  const [exerciseCatalog, setExerciseCatalog] = useState<ExerciseDto[]>([])
  const [form, setForm] = useState<TargetFormState | null>(null)
  const [errors, setErrors] = useState<TargetFormErrors>({})
  const [isLoading, setIsLoading] = useState(true)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)

  useEffect(() => {
    async function loadEditorData() {
      if (!planId || !workoutPlanExerciseId) {
        setIsLoading(false)
        setErrorMessage('Thiếu thông tin bài tập trong plan.')
        return
      }

      try {
        setIsLoading(true)
        setErrorMessage(null)

        const [planResult, exerciseResult] = await Promise.all([
          getWorkoutPlan(planId),
          getSelectableExercises({
            IsActive: true,
            MaxResultCount: 200,
          }),
        ])

        const foundExercise =
          planResult.exercises?.find((exercise) => exercise.id === workoutPlanExerciseId) ?? null

        setPlanExercise(foundExercise)
        setIsPlanArchived(planResult.isArchived)
        setExerciseCatalog(exerciseResult.items ?? [])
        setForm(foundExercise ? toFormState(foundExercise) : null)
      } catch (error) {
        setErrorMessage(
          error instanceof Error ? error.message : 'Không thể tải target bài tập.',
        )
      } finally {
        setIsLoading(false)
      }
    }

    void loadEditorData()
  }, [planId, workoutPlanExerciseId, reloadToken])

  // Re-runs the target request after a transient backend or network failure.
  function retryLoad() {
    setReloadToken((currentToken) => currentToken + 1)
  }

  async function handleSubmit(
    event: SyntheticEvent<HTMLFormElement, SubmitEvent>,
  ) {
    event.preventDefault()

    if (!planId || !workoutPlanExerciseId || !form) {
      return
    }

    const nextErrors = validateForm(form)
    setErrors(nextErrors)

    if (Object.keys(nextErrors).length > 0) {
      return
    }

    try {
      setIsSubmitting(true)
      setErrorMessage(null)

      await updateWorkoutPlanExercise(
        planId,
        workoutPlanExerciseId,
        toUpdateInput(form),
      )

      showToast('Đã cập nhật mục tiêu bài tập.')
      navigate(`/plans/${planId}`, { replace: true })
    } catch (error) {
      setErrorMessage(
        error instanceof Error ? error.message : 'Không thể cập nhật target bài tập.',
      )
    } finally {
      setIsSubmitting(false)
    }
  }

  if (isLoading) {
    return (
      <PageShell title="Sửa target">
        <LoadingState message="Đang tải target bài tập..." />
      </PageShell>
    )
  }

  if (errorMessage) {
    return (
      <PageShell title="Sửa target">
        <ErrorState
          message={errorMessage}
          action={<NeoButton onClick={retryLoad}>Thử lại</NeoButton>}
        />
      </PageShell>
    )
  }

  if (!planExercise || !form || !planId) {
    return (
      <PageShell title="Sửa target">
        <EmptyState
          title="Không tìm thấy bài tập"
          message="Bài tập này không còn nằm trong workout plan."
          action={
            <Link className="neo-button link-button" to={planId ? `/plans/${planId}` : '/plans'}>
              Quay lại plan
            </Link>
          }
        />
      </PageShell>
    )
  }

  if (isPlanArchived) {
    return (
      <PageShell title="Sửa target">
        <EmptyState
          title="Plan đã archived"
          message="Plan đã archived nên target bài tập chỉ có thể xem, không thể chỉnh sửa."
          action={
            <Link className="neo-button link-button" to={`/plans/${planId}`}>
              Quay lại plan
            </Link>
          }
        />
      </PageShell>
    )
  }

  return (
    <PageShell title="Sửa target">
      <NeoCard className="placeholder-card">
        <Link className="back-link" to={`/plans/${planId}`}>
          ← Quay lại plan
        </Link>

        <p className="eyebrow">Exercise Target</p>
        <h2>{getExerciseName(exerciseCatalog, planExercise.exerciseId)}</h2>

        <PermissionGate
          permission={FITLOGS_PERMISSIONS.workoutPlans.manageExercises}
          fallback={
            <ErrorState
              title="Không có quyền chỉnh sửa"
              message="Tài khoản hiện tại không được phép chỉnh sửa bài tập trong plan."
            />
          }
        >
          <form className="form-stack" onSubmit={handleSubmit} aria-busy={isSubmitting}>
          <NeoInput
            label="Thứ tự"
            type="number"
            min={1}
            value={form.orderIndex}
            disabled={isSubmitting}
            onChange={(event) =>
              setForm({
                ...form,
                orderIndex: event.target.value,
              })
            }
          />

          <NeoInput
            label="Sets"
            type="number"
            min={1}
            value={form.defaultSets}
            error={errors.defaultSets}
            disabled={isSubmitting}
            onChange={(event) =>
              setForm({
                ...form,
                defaultSets: event.target.value,
              })
            }
          />

          <NeoInput
            label="Reps"
            type="number"
            min={1}
            value={form.defaultReps}
            error={errors.defaultReps}
            disabled={isSubmitting}
            onChange={(event) =>
              setForm({
                ...form,
                defaultReps: event.target.value,
              })
            }
          />

          <NeoInput
            label="Kg mục tiêu"
            type="number"
            min={0}
            step="0.5"
            placeholder="Bỏ trống nếu không dùng kg"
            value={form.defaultWeightKg}
            error={errors.defaultWeightKg}
            disabled={isSubmitting}
            onChange={(event) =>
              setForm({
                ...form,
                defaultWeightKg: event.target.value,
              })
            }
          />

          <NeoInput
            label="Nghỉ giữa set"
            type="number"
            min={0}
            placeholder="Ví dụ 90"
            value={form.restSeconds}
            error={errors.restSeconds}
            disabled={isSubmitting}
            onChange={(event) =>
              setForm({
                ...form,
                restSeconds: event.target.value,
              })
            }
          />

          <label className="neo-field">
            <span className="neo-field-label">Ghi chú</span>
            <textarea
              className="neo-input"
              rows={4}
              value={form.note}
              disabled={isSubmitting}
              onChange={(event) =>
                setForm({
                  ...form,
                  note: event.target.value,
                })
              }
            />
          </label>

          <NeoButton className="full-width-button" disabled={isSubmitting}>
            {isSubmitting ? 'Đang lưu...' : 'Lưu target'}
          </NeoButton>
          </form>
        </PermissionGate>
      </NeoCard>
    </PageShell>
  )
}
