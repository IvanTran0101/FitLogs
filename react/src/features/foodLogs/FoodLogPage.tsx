import { useEffect, useMemo, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { EmptyState } from '../../components/EmptyState'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { NeoInput } from '../../components/NeoInput'
import { PageShell } from '../../components/PageShell'
import {
  getDailyFoodSummary,
  getFoodLogsByDate,
  type DailyFoodNutritionSummaryDto,
  type FoodLogDto,
  type FoodUnit,
  type MealType,
} from '../../api/foodsApi'
import { getTodayDashboard } from '../../api/dashboardApi'

const MEAL_SECTIONS: { value: MealType; label: string }[] = [
  { value: 1, label: 'Bữa sáng' },
  { value: 2, label: 'Bữa trưa' },
  { value: 3, label: 'Bữa tối' },
  { value: 4, label: 'Ăn nhẹ' },
  { value: 5, label: 'Trước tập' },
  { value: 6, label: 'Sau tập' },
]

const UNIT_LABELS: Record<FoodUnit, string> = {
  1: 'g',
  2: 'ml',
  3: 'suất',
  4: 'cái',
}

/** Accepts only an explicit date-input value; the default day comes from the backend. */
function getInitialDate(searchParam: string | null) {
  return searchParam && /^\d{4}-\d{2}-\d{2}$/.test(searchParam)
    ? searchParam
    : ''
}

/** Sends the selected calendar day as a date-time without silently converting its timezone. */
function toApiDateTime(dateInputValue: string) {
  return `${dateInputValue}T00:00:00`
}

/** Formats backend decimal values without inventing precision or replacing null with zero. */
function formatNutrition(value: number | null | undefined, suffix = '') {
  if (value === null || value === undefined) {
    return '—'
  }

  return `${new Intl.NumberFormat('vi-VN', {
    maximumFractionDigits: 1,
  }).format(value)}${suffix}`
}

/** Converts a meal enum to the label shown in the daily log sections. */
function getMealLabel(mealType: MealType) {
  return MEAL_SECTIONS.find((section) => section.value === mealType)?.label ?? 'Khác'
}

/** Converts the backend unit enum into a short quantity label for each food row. */
function getUnitLabel(unit: FoodUnit) {
  return UNIT_LABELS[unit] ?? 'đơn vị'
}

/** Groups logs by meal while keeping the stable backend ordering within each meal. */
function groupLogsByMeal(logs: FoodLogDto[]) {
  return MEAL_SECTIONS.map((section) => ({
    ...section,
    logs: logs.filter((log) => log.mealType === section.value),
  })).filter((section) => section.logs.length > 0)
}

/** Coordinates date selection, parallel API loading, server totals, and meal-grouped rendering. */
export function FoodLogPage() {
  const [searchParams] = useSearchParams()
  const initialDate = getInitialDate(searchParams.get('date'))
  const [selectedDate, setSelectedDate] = useState(
    () => initialDate,
  )
  const [isDateReady, setIsDateReady] = useState(Boolean(initialDate))
  const [foodLogs, setFoodLogs] = useState<FoodLogDto[]>([])
  const [summary, setSummary] = useState<DailyFoodNutritionSummaryDto | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)

  useEffect(() => {
    if (isDateReady) {
      return
    }

    let isCurrentRequest = true

    // Uses the dashboard's backend-resolved local date so both pages agree on "today".
    async function resolveTodayDate() {
      setIsLoading(true)
      setErrorMessage(null)

      try {
        const dashboard = await getTodayDashboard()
        if (isCurrentRequest) {
          setSelectedDate(dashboard.date)
          setIsDateReady(true)
        }
      } catch (error) {
        if (isCurrentRequest) {
          setErrorMessage(
            error instanceof Error
              ? error.message
              : 'Không thể xác định ngày hiện tại.',
          )
          setIsLoading(false)
        }
      }
    }

    void resolveTodayDate()

    return () => {
      isCurrentRequest = false
    }
  }, [isDateReady, reloadToken])

  useEffect(() => {
    if (!isDateReady || !selectedDate) {
      return
    }

    let isCurrentRequest = true

    // Both requests use the same selected date so the list and totals cannot represent different days.
    async function loadFoodDay() {
      setIsLoading(true)
      setErrorMessage(null)

      try {
        const apiDate = toApiDateTime(selectedDate)
        const [logs, dailySummary] = await Promise.all([
          getFoodLogsByDate(apiDate),
          getDailyFoodSummary(apiDate),
        ])

        if (!isCurrentRequest) {
          return
        }

        setFoodLogs(logs ?? [])
        setSummary(dailySummary)
      } catch (error) {
        if (!isCurrentRequest) {
          return
        }

        setErrorMessage(
          error instanceof Error
            ? error.message
            : 'Không thể tải nhật ký ăn uống của ngày này.',
        )
        setFoodLogs([])
        setSummary(null)
      } finally {
        if (isCurrentRequest) {
          setIsLoading(false)
        }
      }
    }

    void loadFoodDay()

    return () => {
      isCurrentRequest = false
    }
  }, [isDateReady, reloadToken, selectedDate])

  const mealSections = useMemo(() => groupLogsByMeal(foodLogs), [foodLogs])

  // Incrementing this token makes the effect request the same day again without duplicating fetch logic.
  function retrySelectedDate() {
    setReloadToken((currentToken) => currentToken + 1)
  }

  return (
    <PageShell title="Ăn uống">
      <section className="food-page-stack">
        <NeoCard className="food-date-card">
          <NeoInput
            label="Ngày xem nhật ký"
            type="date"
            value={selectedDate}
            disabled={!isDateReady}
            onChange={(event) => setSelectedDate(event.target.value)}
          />
          <p className="food-date-note">
            Tổng dinh dưỡng bên dưới được lấy trực tiếp từ máy chủ.
          </p>
          <Link
            className="neo-button link-button food-add-link"
            to={`/food/add?date=${selectedDate}`}
          >
            Thêm món ăn
          </Link>
        </NeoCard>

        {isLoading ? (
          <LoadingState message="Đang tải nhật ký ăn uống..." />
        ) : errorMessage ? (
          <ErrorState
            title="Không tải được nhật ký"
            message={errorMessage}
            action={<NeoButton onClick={retrySelectedDate}>Thử lại</NeoButton>}
          />
        ) : summary ? (
          <>
            <NeoCard className="food-summary-card">
              <div className="food-summary-total">
                <p className="eyebrow">Tổng calories</p>
                <strong>{formatNutrition(summary.totalCalories)}</strong>
                <span>kcal trong ngày đã chọn</span>
              </div>

              <div className="food-summary-grid">
                <article className="macro-card protein">
                  <span>Protein</span>
                  <strong>{formatNutrition(summary.totalProtein)}</strong>
                  <small>gram</small>
                </article>
                <article className="macro-card carb">
                  <span>Carb</span>
                  <strong>{formatNutrition(summary.totalCarb)}</strong>
                  <small>gram</small>
                </article>
                <article className="macro-card fat">
                  <span>Fat</span>
                  <strong>{formatNutrition(summary.totalFat)}</strong>
                  <small>gram</small>
                </article>
              </div>
            </NeoCard>

            {mealSections.length === 0 ? (
              <EmptyState
                title="Chưa có món ăn"
                message="Ngày này chưa có nhật ký món ăn nào."
              />
            ) : (
              <section className="food-meal-list" aria-label="Nhật ký theo bữa">
                {mealSections.map((section) => (
                  <NeoCard className="food-meal-card" key={section.value}>
                    <div className="food-meal-header">
                      <div>
                        <p className="eyebrow">Bữa ăn</p>
                        <h2>{getMealLabel(section.value)}</h2>
                      </div>
                      <span>{section.logs.length} món</span>
                    </div>

                    <div className="food-log-list">
                      {section.logs.map((log) => (
                        <article className="food-log-row" key={log.id}>
                          <div className="food-log-main">
                            <h3>{log.foodName}</h3>
                            <span>
                              {formatNutrition(log.quantity)} {getUnitLabel(log.unit)}
                            </span>
                          </div>
                          <div className="food-log-nutrition">
                            <strong>{formatNutrition(log.calories)} kcal</strong>
                            <span>
                              P {formatNutrition(log.protein, 'g')} · C{' '}
                              {formatNutrition(log.carb, 'g')} · F{' '}
                              {formatNutrition(log.fat, 'g')}
                            </span>
                          </div>
                          <Link
                            className="food-log-edit-link"
                            to={`/food/logs/${log.id}/edit?date=${selectedDate}`}
                          >
                            Sửa
                          </Link>
                        </article>
                      ))}
                    </div>
                  </NeoCard>
                ))}
              </section>
            )}
          </>
        ) : null}
      </section>
    </PageShell>
  )
}
