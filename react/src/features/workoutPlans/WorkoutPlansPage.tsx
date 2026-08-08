import { useEffect, useState } from 'react'
import {
  getWorkoutPlans,
  type WorkoutDifficulty,
  type WorkoutGoal,
  type WorkoutPlanDto,
} from '../../api/workoutPlansApi'
import { EmptyState } from '../../components/EmptyState'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
//import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { PageShell } from '../../components/PageShell'
import { Link } from 'react-router-dom'
function formatWorkoutGoal(goal: WorkoutGoal) {
  if (goal === 0) {
    return 'General'
  }

  if (goal === 1) {
    return 'Strength'
  }

  if (goal === 2) {
    return 'Hypertrophy'
  }

  if (goal === 3) {
    return 'Endurance'
  }

  return 'Weight Loss'
}

function formatWorkoutDifficulty(difficulty: WorkoutDifficulty) {
  if (difficulty === 0) {
    return 'Dễ'
  }

  if (difficulty === 1) {
    return 'Trung bình'
  }

  return 'Khó'
}

export function WorkoutPlansPage() {
  const [plans, setPlans] = useState<WorkoutPlanDto[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  useEffect(() => {
    async function loadWorkoutPlans() {
      try {
        setIsLoading(true)
        setErrorMessage(null)

        const result = await getWorkoutPlans({
          IsArchived: false,
          MaxResultCount: 100,
        })

        setPlans(result.items ?? [])
      } catch (error) {
        setPlans([])
        setErrorMessage(
          error instanceof Error ? error.message : 'Không thể tải kế hoạch tập.',
        )
      } finally {
        setIsLoading(false)
      }
    }

    void loadWorkoutPlans()
  }, [])

  if (isLoading) {
    return (
      <PageShell title="Kế hoạch">
        <LoadingState message="Đang tải kế hoạch tập..." />
      </PageShell>
    )
  }

  if (errorMessage) {
    return (
      <PageShell title="Kế hoạch">
        <ErrorState message={errorMessage} />
      </PageShell>
    )
  }

  if (plans.length === 0) {
    return (
      <PageShell title="Kế hoạch">
        <EmptyState
          title="Chưa có kế hoạch tập"
          message="Tạo workout plan đầu tiên để bắt đầu xây dựng lịch tập."
          action={
          <Link className="neo-button link-button" to="/plans/new">
            Tạo kế hoạch
          </Link>
        }
        />
      </PageShell>
    )
  }

  return (
    <PageShell title="Kế hoạch">
      <div className="plan-list-actions">
        <Link className="neo-button link-button" to="/plans/new">
          Tạo kế hoạch
        </Link>
      </div>

      <section className="exercise-list">
        {plans.map((plan) => (
          <Link key={plan.id} className="exercise-card-link" to={`/plans/${plan.id}`}>
          <NeoCard key={plan.id} className="plan-list-card">
            <div className="exercise-card-content">
              <div className="exercise-card-header">
                <h2>{plan.name ?? 'Kế hoạch chưa đặt tên'}</h2>
                <span className={plan.isActive ? 'exercise-status' : 'exercise-status inactive'}>
                  {plan.isActive ? 'Active' : 'Inactive'}
                </span>
              </div>

              <p>{plan.description ?? 'Chưa có mô tả.'}</p>

              <div className="exercise-tags plan-list-tags">
                <span>{formatWorkoutGoal(plan.goal)}</span>
                <span>{formatWorkoutDifficulty(plan.difficulty)}</span>
                <span>{plan.exercises?.length ?? 0} bài</span>
              </div>
            </div>
          </NeoCard>
        </Link>
        ))}
      </section>
    </PageShell>
  )
}
