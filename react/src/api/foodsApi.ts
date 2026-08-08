import { apiRequest } from './httpClient'
import type { PagedResult } from './exercisesApi'

/** Values accepted by the backend for the quantity unit on a food log. */
export type FoodUnit = 1 | 2 | 3 | 4

/** Meal categories accepted by the backend when a food log is created or edited. */
export type MealType = 1 | 2 | 3 | 4 | 5 | 6

/** Identifies where a food product's data came from. */
export type FoodProductSource = 1 | 2 | 3

export type FoodProductDto = {
  id: string
  barcode: string | null
  name: string
  brand: string | null
  imageUrl: string | null
  caloriesPer100g: number
  proteinPer100g: number | null
  carbPer100g: number | null
  fatPer100g: number | null
  servingSize: string | null
  source: FoodProductSource
  lastSyncedAt: string | null
  isActive: boolean
  isVerified: boolean
}

export type FoodProductLookupResultDto = {
  found: boolean
  fromCache: boolean
  foodProductId: string | null
  barcode: string | null
  name: string | null
  brand: string | null
  imageUrl: string | null
  caloriesPer100g: number | null
  proteinPer100g: number | null
  carbPer100g: number | null
  fatPer100g: number | null
  servingSize: string | null
}

export type FoodLogDto = {
  id: string
  userId: string
  foodProductId: string
  foodName: string
  quantity: number
  unit: FoodUnit
  calories: number
  protein: number | null
  carb: number | null
  fat: number | null
  mealType: MealType
  loggedAt: string
  note: string | null
}

export type DailyFoodNutritionSummaryDto = {
  date: string
  totalCalories: number
  totalProtein: number
  totalCarb: number
  totalFat: number
}

export type GetFoodProductListQuery = {
  FilterText?: string
  OnlyActive?: boolean
  Sorting?: string
  SkipCount?: number
  MaxResultCount?: number
}

/** The date must be sent as the backend's date-time query value. */
export type GetFoodLogListQuery = {
  Date: string
}

export type CreateFoodLogDto = {
  foodProductId: string
  quantity: number
  unit: FoodUnit
  mealType: MealType
  loggedAt?: string | null
  note?: string | null
  overrideCalories?: number | null
  overrideProtein?: number | null
  overrideCarb?: number | null
  overrideFat?: number | null
}

export type UpdateFoodLogDto = CreateFoodLogDto

/** Loads a paged product list using the server-side search and paging contract. */
export function getFoodProducts(query: GetFoodProductListQuery = {}) {
  return apiRequest<PagedResult<FoodProductDto>>('/api/app/food-product', {
    query,
  })
}

/** Loads one product by id so a selected product can be displayed or re-used. */
export function getFoodProduct(id: string) {
  return apiRequest<FoodProductDto>(`/api/app/food-product/${id}`)
}

/** Sends a barcode to FitLogs; the backend handles its cache and Open Food Facts lookup. */
export function lookupFoodProductByBarcode(barcode: string) {
  return apiRequest<FoodProductLookupResultDto>(
    '/api/app/food-product-lookup/lookup-by-barcode',
    {
      method: 'POST',
      query: { barcode },
    },
  )
}

/** Loads the signed-in user's food entries for one selected calendar date. */
export function getFoodLogsByDate(date: string) {
  return apiRequest<FoodLogDto[]>('/api/app/food-log/by-date', {
    query: { Date: date },
  })
}

/** Loads server-calculated nutrition totals for the selected date. */
export function getDailyFoodSummary(date: string) {
  return apiRequest<DailyFoodNutritionSummaryDto>('/api/app/food-log/daily-summary', {
    query: { Date: date },
  })
}

/** Loads one owned food log for the edit screen. */
export function getFoodLog(id: string) {
  return apiRequest<FoodLogDto>(`/api/app/food-log/${id}`)
}

/** Creates a food log; nutrition is calculated and returned by the backend. */
export function createFoodLog(input: CreateFoodLogDto) {
  return apiRequest<FoodLogDto>('/api/app/food-log', {
    method: 'POST',
    body: input,
  })
}

/** Updates an owned food log using only fields supported by UpdateFoodLogDto. */
export function updateFoodLog(id: string, input: UpdateFoodLogDto) {
  return apiRequest<FoodLogDto>(`/api/app/food-log/${id}`, {
    method: 'PUT',
    body: input,
  })
}

/** Deletes an owned food log; callers should refresh the list and summary afterward. */
export function deleteFoodLog(id: string) {
  return apiRequest<void>(`/api/app/food-log/${id}`, {
    method: 'DELETE',
  })
}
