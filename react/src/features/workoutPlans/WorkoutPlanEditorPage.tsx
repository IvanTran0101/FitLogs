import { useEffect, useState } from 'react'
import type { SyntheticEvent } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import {
  createWorkoutPlan,
  getWorkoutPlan,
  updateWorkoutPlan,
  activateWorkoutPlan,
  deactivateWorkoutPlan,
  type CreateWorkoutPlanDto,
  type WorkoutDifficulty,
  type WorkoutGoal,
} from '../../api/workoutPlansApi'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { NeoInput } from '../../components/NeoInput'
import { NeoSelect } from '../../components/NeoSelect'
import { PageShell } from '../../components/PageShell'
import { PermissionGate } from '../../components/PermissionGate'
import { FITLOGS_PERMISSIONS } from '../../auth/permissions'

type WorkoutPlanFormState = {
  name: string
  description: string
  goal: string
  difficulty: string
  isActive: boolean
}

type WorkoutPlanFormErrors = {
  name?: string
}

const defaultFormState: WorkoutPlanFormState = {
  name: '',
  description: '',
  goal: '0',
  difficulty: '0',
  isActive: true,
}

function validateForm(form: WorkoutPlanFormState): WorkoutPlanFormErrors {
  const errors: WorkoutPlanFormErrors = {}

  if (form.name.trim().length === 0) {
    errors.name = 'Tên kế hoạch không được để trống.'
  }

  return errors
}

function toWorkoutPlanInput(form: WorkoutPlanFormState): CreateWorkoutPlanDto {
  return {
    name: form.name.trim(),
    description: form.description.trim() || null,
    goal: Number(form.goal) as WorkoutGoal,
    difficulty: Number(form.difficulty) as WorkoutDifficulty,
  }
}

export function WorkoutPlanEditorPage() {
  const { planId } = useParams()
  const navigate = useNavigate()
  const isEditMode = Boolean(planId)

  const [form, setForm] = useState<WorkoutPlanFormState>(defaultFormState)
  const [errors, setErrors] = useState<WorkoutPlanFormErrors>({})
  const [isLoading, setIsLoading] = useState(isEditMode)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [loadError, setLoadError] = useState<string | null>(null)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)

  useEffect(() => {
    async function loadPlanForEdit() {
      if (!planId) {
        return
      }

      try {
        setIsLoading(true)
        setLoadError(null)

        const plan = await getWorkoutPlan(planId)

        setForm({
          name: plan.name ?? '',
          description: plan.description ?? '',
          goal: String(plan.goal),
          difficulty: String(plan.difficulty),
          isActive: plan.isActive,
        })
      } catch (error) {
        setLoadError(
          error instanceof Error ? error.message : 'Không thể tải kế hoạch tập.',
        )
      } finally {
        setIsLoading(false)
      }
    }

    void loadPlanForEdit()
  }, [planId, reloadToken])

  // Re-runs the edit-form request after a transient backend or network failure.
  function retryLoad() {
    setReloadToken((currentToken) => currentToken + 1)
  }

async function handleSubmit(
  event: SyntheticEvent<HTMLFormElement, SubmitEvent>,
) {    event.preventDefault()

    const nextErrors = validateForm(form)
    setErrors(nextErrors)

    if (Object.keys(nextErrors).length > 0) {
      return
    }

    try {
      setIsSubmitting(true)
      setErrorMessage(null)

      const createInput = toWorkoutPlanInput(form)
      let savedPlan =
        isEditMode && planId
          ? await updateWorkoutPlan(planId, createInput)
          : await createWorkoutPlan(createInput)

      if (isEditMode && planId && savedPlan.isActive !== form.isActive) {
        savedPlan = form.isActive
          ? await activateWorkoutPlan(planId)
          : await deactivateWorkoutPlan(planId)
      }

      navigate(`/plans/${savedPlan.id}`, { replace: true })
    } catch (error) {
      setErrorMessage(
        error instanceof Error ? error.message : 'Không thể lưu kế hoạch tập.',
      )
    } finally {
      setIsSubmitting(false)
    }
  }

  if (isLoading) {
    return (
      <PageShell title={isEditMode ? 'Sửa kế hoạch' : 'Tạo kế hoạch'}>
        <LoadingState message="Đang tải form kế hoạch..." />
      </PageShell>
    )
  }

  if (loadError) {
    return (
      <PageShell title={isEditMode ? 'Sửa kế hoạch' : 'Tạo kế hoạch'}>
        <ErrorState
          title="Không tải được kế hoạch"
          message={loadError}
          action={<NeoButton onClick={retryLoad}>Thử lại</NeoButton>}
        />
      </PageShell>
    )
  }

  return (
    <PageShell title={isEditMode ? 'Sửa kế hoạch' : 'Tạo kế hoạch'}>
      <NeoCard className="editor-card">
        <Link className="back-link" to={isEditMode && planId ? `/plans/${planId}` : '/plans'}>
          ← {isEditMode ? 'Quay lại chi tiết kế hoạch' : 'Quay lại kế hoạch'}
        </Link>

        <p className="eyebrow">Workout Plan Editor</p>
        <h2>{isEditMode ? 'Sửa kế hoạch' : 'Tạo kế hoạch mới'}</h2>

        {errorMessage ? <ErrorState message={errorMessage} /> : null}

        <PermissionGate
          permission={
            isEditMode
              ? FITLOGS_PERMISSIONS.workoutPlans.update
              : FITLOGS_PERMISSIONS.workoutPlans.create
          }
          fallback={
            <ErrorState
              title="Không có quyền chỉnh sửa"
              message="Tài khoản hiện tại không được phép thay đổi workout plan."
            />
          }
        >
          <form className="form-stack" onSubmit={handleSubmit} aria-busy={isSubmitting}>
          <NeoInput
            label="Tên kế hoạch"
            placeholder="Push Day, Leg Day..."
            value={form.name}
            error={errors.name}
            disabled={isSubmitting}
            onChange={(event) =>
              setForm({
                ...form,
                name: event.target.value,
              })
            }
          />

          <label className="neo-field">
            <span className="neo-field-label">Mô tả</span>
            <textarea
              className="neo-input"
              rows={4}
              placeholder="Mục tiêu hoặc ghi chú ngắn cho kế hoạch này..."
              value={form.description}
              disabled={isSubmitting}
              onChange={(event) =>
                setForm({
                  ...form,
                  description: event.target.value,
                })
              }
            />
          </label>

          <NeoSelect
            label="Mục tiêu"
            value={form.goal}
            disabled={isSubmitting}
            onChange={(event) =>
              setForm({
                ...form,
                goal: event.target.value,
              })
            }
            options={[
              { label: 'General', value: '0' },
              { label: 'Tăng cơ', value: '1' },
              { label: 'Giảm mỡ', value: '2' },
              { label: 'Sức mạnh', value: '3' },
              { label: 'Sức bền', value: '4' },
            ]}
          />

          <NeoSelect
            label="Độ khó"
            value={form.difficulty}
            disabled={isSubmitting}
            onChange={(event) =>
              setForm({
                ...form,
                difficulty: event.target.value,
              })
            }
            options={[
              { label: 'Dễ', value: '0' },
              { label: 'Trung bình', value: '1' },
              { label: 'Khó', value: '2' },
            ]}
          />

          {isEditMode && <label className="neo-checkbox-row">
            <input
              type="checkbox"
              checked={form.isActive}
              disabled={isSubmitting}
              onChange={(event) =>
                setForm({
                  ...form,
                  isActive: event.target.checked,
                })
              }
            />
            <span>Plan đang active</span>
          </label>}

          <NeoButton type="submit" className="full-width-button" disabled={isSubmitting}>
            {isSubmitting ? 'Đang lưu...' : 'Lưu kế hoạch'}
          </NeoButton>
          </form>
        </PermissionGate>
      </NeoCard>
    </PageShell>
  )
}
