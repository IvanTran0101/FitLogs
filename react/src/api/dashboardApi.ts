import { apiRequest } from './httpClient'
import type { MealType } from './foodsApi'

/** Optional local calendar date sent to dashboard endpoints as an ISO date. */
export type DashboardDateQuery = {
  Date?: string | null
}

export type MealCaloriesBreakdownDto = {
  mealType: MealType
  calories: number
  protein: number
  carbs: number
  fat: number
}

export type DailyNutritionSummaryDto = {
  hasNutritionData: boolean
  dailyCaloriesTarget: number | null
  hasCaloriesTarget: boolean
  totalCalories: number
  remainingCalories: number | null
  totalProtein: number
  totalCarbs: number
  totalFat: number
  caloriesByMealType: MealCaloriesBreakdownDto[] | null
}

export type DailyWorkoutSummaryDto = {
  completedSessions: number
  totalExercises: number
  totalSets: number
  totalDurationMinutes: number
  totalWeightVolume: number
  hasWorkout: boolean
}

export type DailyDashboardDto = {
  date: string
  nutrition: DailyNutritionSummaryDto
  workout: DailyWorkoutSummaryDto
}

/** Creates the query shape shared by dashboard endpoints while allowing the backend to choose today's date. */
function buildDateQuery(date?: string): DashboardDateQuery {
  return { Date: date }
}

/** Loads the combined nutrition and workout dashboard for the user's current local day. */
export function getTodayDashboard() {
  return apiRequest<DailyDashboardDto>('/api/app/dashboard/today')
}

/** Loads the combined dashboard for an optional user-selected calendar date. */
export function getDailyDashboard(date?: string) {
  return apiRequest<DailyDashboardDto>('/api/app/dashboard/daily', {
    query: buildDateQuery(date),
  })
}

/** Loads only the nutrition summary for an optional user-selected calendar date. */
export function getDailyNutrition(date?: string) {
  return apiRequest<DailyNutritionSummaryDto>('/api/app/dashboard/daily-nutrition', {
    query: buildDateQuery(date),
  })
}

/** Loads only the completed-workout summary for an optional user-selected calendar date. */
export function getDailyWorkout(date?: string) {
  return apiRequest<DailyWorkoutSummaryDto>('/api/app/dashboard/daily-workout', {
    query: buildDateQuery(date),
  })
}
