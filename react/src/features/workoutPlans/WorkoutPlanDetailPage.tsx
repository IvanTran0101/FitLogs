import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import {
  getSelectableExercises,
  type ExerciseDto,
} from '../../api/exercisesApi'
import {
  getWorkoutPlan,
  removeWorkoutPlanExercise,
  reorderWorkoutPlanExercises,
  type WorkoutPlanDto,
  type WorkoutPlanExerciseDto,
} from '../../api/workoutPlansApi'
import { EmptyState } from '../../components/EmptyState'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoCard } from '../../components/NeoCard'
import { PageShell } from '../../components/PageShell'

function getExerciseName(exercises: ExerciseDto[], exerciseId: string) {
    return (
        exercises.find((exercise) => exercise.id === exerciseId)?.name ??
     'Không rõ'
    )
}

function formatWeight(weightKg: number | null) {
    if (weightKg === null)
    {
        return 'Chưa đặt kg'
    }
    return `${weightKg} kg`
}
function formatRest(restSeconds: number | null) {
    if (restSeconds === null)
    {
        return 'Chưa đặt giây'
    }
    return `${restSeconds} giây`
}

function sortPlanExercises(exercises: WorkoutPlanExerciseDto[]) {
  return [...exercises].sort((first, second) => first.orderIndex - second.orderIndex)
}

function moveExercise(
  exercises: WorkoutPlanExerciseDto[],
  fromIndex: number,
  toIndex: number,
) {
  const nextExercises = [...exercises]
  const [movedExercise] = nextExercises.splice(fromIndex, 1)

  nextExercises.splice(toIndex, 0, movedExercise)

  return nextExercises
}

export function WorkoutPlanDetailPage() {
  const { planId } = useParams()

  const [plan, setPlan] = useState<WorkoutPlanDto | null>(null)
  const [exerciseCatalog, setExerciseCatalog] = useState<ExerciseDto[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [mutatingExerciseId, setMutatingExerciseId] = useState<string | null>(null)
  useEffect(() => {
    async function loadPlanDetail() {
      if (!planId) {
        setIsLoading(false)
        setErrorMessage('Thiếu id kế hoạch tập.')
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

        setPlan(planResult)
        setExerciseCatalog(exerciseResult.items ?? [])
      } catch (error) {
        setPlan(null)
        setErrorMessage(
          error instanceof Error ? error.message : 'Không thể tải kế hoạch tập.',
        )
      } finally {
        setIsLoading(false)
      }
    }

    void loadPlanDetail()
  }, [planId])

  async function handleRemoveExercise(planExerciseId: string) {
    if (!plan) {
      return
    }

    const shouldRemove = window.confirm('Xoá bài tập này khỏi kế hoạch?')

    if (!shouldRemove) {
      return
    }

    try {
      setMutatingExerciseId(planExerciseId)
      setErrorMessage(null)

      const updatedPlan = await removeWorkoutPlanExercise(plan.id, planExerciseId)

      setPlan(updatedPlan)
    } catch (error) {
      setErrorMessage(
        error instanceof Error ? error.message : 'Không thể xoá bài tập khỏi plan.',
      )
    } finally {
      setMutatingExerciseId(null)
    }
  }

  if (isLoading) {
    return (
      <PageShell title="Chi tiết kế hoạch">
        <LoadingState message="Đang tải kế hoạch tập..." />
      </PageShell>
    )
  }

  if (errorMessage) {
    return (
      <PageShell title="Chi tiết kế hoạch">
        <ErrorState message={errorMessage} />
      </PageShell>
    )
  }

  if (!plan) {
    return (
      <PageShell title="Chi tiết kế hoạch">
        <EmptyState
          title="Không tìm thấy kế hoạch"
          message="Kế hoạch tập này không tồn tại hoặc đã bị xoá."
          action={
            <Link className="neo-button link-button" to="/plans">
              Quay lại danh sách
            </Link>
          }
        />
      </PageShell>
    )
  }

  const planExercises = sortPlanExercises(plan.exercises ?? [])

async function handleMoveExercise(fromIndex: number, toIndex: number) {
  if (!plan) {
    return
  }

  const currentExercises = sortPlanExercises(plan.exercises ?? [])

  if (toIndex < 0 || toIndex >= currentExercises.length) {
    return
  }

  const movedExercises = moveExercise(currentExercises, fromIndex, toIndex)

  try {
    setMutatingExerciseId(currentExercises[fromIndex].id)
    setErrorMessage(null)

    const updatedPlan = await reorderWorkoutPlanExercises(plan.id, {
      exercises: movedExercises.map((exercise, index) => ({
        workoutPlanExerciseId: exercise.id,
        orderIndex: index + 1,
      })),
    })

    setPlan(updatedPlan)
  } catch (error) {
    setErrorMessage(
      error instanceof Error ? error.message : 'Không thể đổi thứ tự bài tập.',
    )
  } finally {
    setMutatingExerciseId(null)
  }
}

  return (
    <PageShell title="Chi tiết kế hoạch">
      <NeoCard className="placeholder-card">
        <Link className="back-link" to="/plans">
          ← Quay lại kế hoạch
        </Link>

        <p className="eyebrow">Workout Plan</p>
        <h2>{plan.name ?? 'Kế hoạch chưa đặt tên'}</h2>
        <p>{plan.description ?? 'Chưa có mô tả.'}</p>

        <div className="exercise-tags">
          <span>{plan.isActive ? 'Active' : 'Inactive'}</span>
          <span>{plan.isArchived ? 'Archived' : 'Available'}</span>
          <span>{planExercises.length} bài</span>
        </div>

        {plan.isArchived ? (
          <p>Plan đã archived nên không thể chỉnh sửa.</p>
        ) : (
          <>
            <Link className="neo-button link-button" to={`/plans/${plan.id}/edit`}>
              Sửa kế hoạch
            </Link>

            <Link
              className="neo-button link-button secondary-link-button"
              to={`/plans/${plan.id}/add-exercises`}
            >
              Thêm bài tập
            </Link>
          </>
        )}
      </NeoCard>

     {planExercises.length === 0 ? (
          <EmptyState
            title="Plan chưa có bài tập"
            message={
              plan.isArchived
                ? 'Plan này đã archived và không có bài tập nào.'
                : 'Thêm bài tập vào plan để bắt đầu dùng kế hoạch này.'
            }
            action={
              plan.isArchived ? null : (
                <Link
                  className="neo-button link-button"
                  to={`/plans/${plan.id}/add-exercises`}
                >
                  Thêm bài tập
                </Link>
              )
            }
          />
        ) : (
        <section className="exercise-list">
          {planExercises.map((planExercise, index) => (
            <NeoCard key={planExercise.id} className="plan-exercise-card">
              <div className="exercise-card-content">
                <div className="exercise-card-header">
                  <h2>{getExerciseName(exerciseCatalog, planExercise.exerciseId)}</h2>
                  <span className="exercise-status">
                    #{planExercise.orderIndex}
                  </span>
                </div>

                <div className="exercise-tags">
                  <span>{planExercise.defaultSets} sets</span>
                  <span>{planExercise.defaultReps} reps</span>
                  <span>{formatWeight(planExercise.defaultWeightKg)}</span>
                  <span>{formatRest(planExercise.restSeconds)}</span>
                </div>
                
                {planExercise.note ? <p>{planExercise.note}</p> : null}
                <div className="plan-exercise-actions">
                <button
                  className="neo-button plan-exercise-action-button"
                  type="button"
                  disabled={index === 0 || mutatingExerciseId === planExercise.id}
                  onClick={() => handleMoveExercise(index, index - 1)}
                >
                  ↑ Lên
                </button>

                <button
                  className="neo-button plan-exercise-action-button danger"
                  type="button"
                  disabled={mutatingExerciseId === planExercise.id}
                  onClick={() => handleRemoveExercise(planExercise.id)}
                >
                  {mutatingExerciseId === planExercise.id ? 'Đang xoá...' : 'Xoá'}
                </button>

                <button
                  className="neo-button plan-exercise-action-button"
                  type="button"
                  disabled={index === planExercises.length - 1 || mutatingExerciseId === planExercise.id}
                  onClick={() => handleMoveExercise(index, index + 1)}
                >
                  ↓ Xuống
                </button>
              </div>
              </div>
            </NeoCard>
          ))}
        </section>
      )}
    </PageShell>
  )
}