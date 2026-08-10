import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { EmptyState } from '../../components/EmptyState'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { NeoInput } from '../../components/NeoInput'
import { PageShell } from '../../components/PageShell'
import {
  getDailyDashboard,
  getTodayDashboard,
  type DailyDashboardDto,
  type MealCaloriesBreakdownDto,
} from '../../api/dashboardApi'
import {
  getMyProfile,
  getProfileCompletionMissingFields,
  isUserProfileComplete,
  type ProfileCompletionField,
} from '../../api/profileApi'

const MEAL_TYPE_LABELS: Record<number, string> = {
  1: 'Bữa sáng',
  2: 'Bữa trưa',
  3: 'Bữa tối',
  4: 'Ăn nhẹ',
  5: 'Trước khi tập',
  6: 'Sau khi tập',
}

const PROFILE_FIELD_LABELS: Record<ProfileCompletionField, string> = {
  heightCm: 'chiều cao',
  weightKg: 'cân nặng',
  dailyTargetCalories: 'calories mục tiêu',
}

/** Formats dashboard numbers with the Vietnamese locale while keeping small macro decimals readable. */
function formatMetric(value: number, maximumFractionDigits = 0) {
  return new Intl.NumberFormat('vi-VN', {
    maximumFractionDigits,
  }).format(value)
}

/** Formats the server's date-only value without shifting it through the user's timezone. */
function formatDashboardDate(value: string) {
  const [year, month, day] = value.split('-').map(Number)
  if (!year || !month || !day) {
    return value
  }

  return new Intl.DateTimeFormat('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
  }).format(new Date(year, month - 1, day))
}

/** Calculates a display-only calorie percentage from backend totals and target values. */
function getCalorieProgress(totalCalories: number, targetCalories: number | null) {
  if (targetCalories === null || targetCalories <= 0) {
    return 0
  }

  return Math.min(100, Math.round((totalCalories / targetCalories) * 100))
}

/** Builds a friendly remaining-calorie message from the backend-calculated balance. */
function formatRemainingCalories(remainingCalories: number | null) {
  if (remainingCalories === null) {
    return 'Chưa đặt mục tiêu calories.'
  }

  if (remainingCalories < 0) {
    return `Đã vượt ${formatMetric(Math.abs(remainingCalories))} kcal`
  }

  return `Còn ${formatMetric(remainingCalories)} kcal`
}

/** Gives each meal row a stable key even when the backend returns an unexpected duplicate category. */
function getMealRowKey(meal: MealCaloriesBreakdownDto, index: number) {
  return `${meal.mealType}-${index}`
}

/** Chooses the server-local-today endpoint or the selected-date dashboard endpoint. */
async function loadDashboardForDate(date: string) {
  return date ? getDailyDashboard(date) : getTodayDashboard()
}

/** Renders backend nutrition and workout summaries with explicit loading, error, and empty states. */
export function DashboardPage() {
  const [selectedDate, setSelectedDate] = useState('')
  const [dashboard, setDashboard] = useState<DailyDashboardDto | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)
  const [profileIsComplete, setProfileIsComplete] = useState<boolean | null>(null)
  const [profileMissingFields, setProfileMissingFields] = useState<ProfileCompletionField[]>([])

  useEffect(() => {
    let isCurrentRequest = true

    // Requests the selected day and updates state only if this request is still the active one.
    async function load() {
      setIsLoading(true)
      setError(null)
      setDashboard(null)
      setProfileIsComplete(null)
      setProfileMissingFields([])

      try {
        const [dashboardResult, profileResult] = await Promise.allSettled([
          loadDashboardForDate(selectedDate),
          getMyProfile(),
        ])

        if (isCurrentRequest) {
          if (dashboardResult.status === 'fulfilled') {
            setDashboard(dashboardResult.value)
          } else {
            setError(
              dashboardResult.reason instanceof Error
                ? dashboardResult.reason.message
                : 'Không thể tải dashboard.',
            )
          }

          if (profileResult.status === 'fulfilled') {
            setProfileIsComplete(isUserProfileComplete(profileResult.value))
            setProfileMissingFields(
              getProfileCompletionMissingFields(profileResult.value),
            )
          }
        }
      } catch (requestError) {
        if (isCurrentRequest) {
          setError(
            requestError instanceof Error
              ? requestError.message
              : 'Không thể tải dashboard.',
          )
        }
      } finally {
        if (isCurrentRequest) {
          setIsLoading(false)
        }
      }
    }

    void load()

    return () => {
      isCurrentRequest = false
    }
  }, [reloadToken, selectedDate])

  const retry = () => setReloadToken((currentToken) => currentToken + 1)

  return (
    <PageShell title="Hôm nay">
      <NeoCard className="dashboard-date-card">
        <NeoInput
          label="Ngày xem"
          type="date"
          value={selectedDate}
          onChange={(event) => setSelectedDate(event.target.value)}
        />
        <p className="dashboard-date-help">
          Để trống để xem ngày hiện tại theo múi giờ trong hồ sơ của bạn.
        </p>
      </NeoCard>

      {isLoading ? <LoadingState message="Đang tải dashboard..." /> : null}
      {!isLoading && error ? (
        <ErrorState
          title="Không tải được dashboard"
          message={error}
          action={<NeoButton onClick={retry}>Thử lại</NeoButton>}
        />
      ) : null}

      {!isLoading && !error && dashboard ? (
        <DashboardContent
          dashboard={dashboard}
          profileIsComplete={profileIsComplete}
          profileMissingFields={profileMissingFields}
        />
      ) : null}
    </PageShell>
  )
}

type DashboardContentProps = {
  dashboard: DailyDashboardDto
  profileIsComplete: boolean | null
  profileMissingFields: ProfileCompletionField[]
}

/** Displays the dashboard cards using only values returned by the backend summary DTOs. */
function DashboardContent({
  dashboard,
  profileIsComplete,
  profileMissingFields,
}: DashboardContentProps) {
  const { nutrition, workout } = dashboard
  const mealBreakdowns = nutrition.caloriesByMealType ?? []
  const hasAnyData = nutrition.hasNutritionData || workout.hasWorkout
  const calorieProgress = getCalorieProgress(
    nutrition.totalCalories,
    nutrition.dailyCaloriesTarget,
  )

  return (
    <section className="dashboard-content" aria-label="Tóm tắt dashboard">
      <p className="dashboard-selected-date">
        {formatDashboardDate(dashboard.date)}
      </p>

      {profileIsComplete === false ? (
        <ProfileCompletionNotice missingFields={profileMissingFields} />
      ) : null}

      {!hasAnyData ? (
        <EmptyState
          title="Chưa có dữ liệu"
          message="Hãy ghi lại món ăn hoặc hoàn thành buổi tập để dashboard có số liệu."
        />
      ) : null}

      <NeoCard className="calorie-card">
        <div>
          <p className="eyebrow">Calo đã dùng</p>
          <strong>{formatMetric(nutrition.totalCalories)}</strong>
          <span>
            {nutrition.hasCaloriesTarget && nutrition.dailyCaloriesTarget !== null
              ? `/ ${formatMetric(nutrition.dailyCaloriesTarget)} kcal`
              : 'kcal'}
          </span>
          <p className="dashboard-balance">
            {formatRemainingCalories(nutrition.remainingCalories)}
          </p>
        </div>

        <div
          className="progress-ring"
          style={{
            background: `conic-gradient(var(--color-lime) ${calorieProgress}%, var(--color-white) 0)`,
          }}
          aria-label={`${calorieProgress}% calories mục tiêu`}
        >
          {nutrition.hasCaloriesTarget ? `${calorieProgress}%` : '—'}
        </div>
      </NeoCard>

      <section className="macro-grid" aria-label="Dinh dưỡng đa lượng">
        <article className="macro-card protein">
          <span>Protein</span>
          <strong>{formatMetric(nutrition.totalProtein, 1)}g</strong>
          <small>Tổng trong ngày</small>
        </article>

        <article className="macro-card carb">
          <span>Carb</span>
          <strong>{formatMetric(nutrition.totalCarbs, 1)}g</strong>
          <small>Tổng trong ngày</small>
        </article>

        <article className="macro-card fat">
          <span>Fat</span>
          <strong>{formatMetric(nutrition.totalFat, 1)}g</strong>
          <small>Tổng trong ngày</small>
        </article>
      </section>

      <NeoCard className="workout-card">
        <div>
          <p className="eyebrow">Buổi tập hoàn thành</p>
          <h2>{workout.completedSessions} buổi</h2>
          <span>
            {workout.hasWorkout
              ? `${workout.totalExercises} bài tập · ${workout.totalSets} set`
              : 'Chưa có buổi tập hoàn thành trong ngày này.'}
          </span>
        </div>
      </NeoCard>

      <section className="stats-grid" aria-label="Chỉ số buổi tập">
        <article className="stat-card blue">
          <span>Thời lượng</span>
          <strong>{formatMetric(workout.totalDurationMinutes, 1)}</strong>
          <small>phút</small>
        </article>

        <article className="stat-card lime">
          <span>Bài tập</span>
          <strong>{formatMetric(workout.totalExercises)}</strong>
          <small>hoàn thành</small>
        </article>

        <article className="stat-card yellow">
          <span>Volume</span>
          <strong>{formatMetric(workout.totalWeightVolume, 1)}</strong>
          <small>kg</small>
        </article>
      </section>

      <NeoCard className="dashboard-meal-card">
        <p className="eyebrow">Calories theo bữa</p>
        {mealBreakdowns.length === 0 ? (
          <p className="dashboard-empty-note">Chưa có dữ liệu theo bữa.</p>
        ) : (
          <div className="dashboard-meal-list">
            {mealBreakdowns.map((meal, index) => (
              <div className="dashboard-meal-row" key={getMealRowKey(meal, index)}>
                <span>{MEAL_TYPE_LABELS[meal.mealType] ?? 'Bữa ăn'}</span>
                <strong>{formatMetric(meal.calories)} kcal</strong>
              </div>
            ))}
          </div>
        )}
      </NeoCard>
    </section>
  )
}

type ProfileCompletionNoticeProps = {
  missingFields: ProfileCompletionField[]
}

/** Explains which dashboard inputs are missing and links the user to the profile editor. */
function ProfileCompletionNotice({ missingFields }: ProfileCompletionNoticeProps) {
  const missingFieldText = missingFields
    .map((field) => PROFILE_FIELD_LABELS[field])
    .join(', ')

  return (
    <NeoCard className="dashboard-profile-card">
      <p className="eyebrow">Hoàn thiện hồ sơ</p>
      <h2>Thiếu {missingFieldText}</h2>
      <p>
        Điền thêm thông tin để dashboard hiển thị mục tiêu calories và số liệu phù hợp hơn.
      </p>
      <Link className="neo-button dashboard-profile-link" to="/profile">
        Mở hồ sơ
      </Link>
    </NeoCard>
  )
}
