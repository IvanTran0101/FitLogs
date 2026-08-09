import { apiRequest } from './httpClient'
import type { PagedResult } from './exercisesApi'

export type WorkoutGoal = 0 | 1 | 2 | 3 | 4

export type WorkoutDifficulty = 0 | 1 | 2

export type WorkoutPlanExerciseDto = {
  id: string
  workoutPlanId: string
  exerciseId: string
  orderIndex: number
  defaultSets: number
  defaultReps: number
  defaultWeightKg: number | null
  restSeconds: number | null
  note: string | null
}

export type WorkoutPlanDto = {
  id: string
  creationTime: string
  creatorId: string | null
  lastModificationTime: string | null
  lastModifierId: string | null
  isDeleted: boolean
  deleterId: string | null
  deletionTime: string | null
  userId: string
  name: string | null
  description: string | null
  goal: WorkoutGoal
  difficulty: WorkoutDifficulty
  isActive: boolean
  isArchived: boolean
  exercises: WorkoutPlanExerciseDto[] | null
}

export type GetWorkoutPlanListQuery = {
  FilterText?: string
  IsArchived?: boolean
  IsActive?: boolean
  Goal?: WorkoutGoal
  Difficulty?: WorkoutDifficulty
  Sorting?: string
  SkipCount?: number
  MaxResultCount?: number
}

export type CreateWorkoutPlanDto = {
  name: string
  description?: string | null
  goal: WorkoutGoal
  difficulty: WorkoutDifficulty
}

export type UpdateWorkoutPlanDto = CreateWorkoutPlanDto & {
  isActive: boolean
}

export type CreateWorkoutPlanExerciseDto = {
  exerciseId: string
  orderIndex: number
  defaultSets: number
  defaultReps: number
  defaultWeightKg?: number | null
  restSeconds?: number | null
  note?: string | null
}

export type UpdateWorkoutPlanExerciseDto = {
  orderIndex: number
  defaultSets: number
  defaultReps: number
  defaultWeightKg?: number | null
  restSeconds?: number | null
  note?: string | null
}

export type ReorderWorkoutPlanExerciseItemDto = {
  workoutPlanExerciseId: string
  orderIndex: number
}

export type ReorderWorkoutPlanExercisesDto = {
  exercises: ReorderWorkoutPlanExerciseItemDto[]
}

export function getWorkoutPlans(query: GetWorkoutPlanListQuery = {}) {
    return apiRequest<PagedResult<WorkoutPlanDto>>('/api/app/workout-plan', {
    query,
  })
}

export function getWorkoutPlan(id: string) {
  return apiRequest<WorkoutPlanDto>(`/api/app/workout-plan/${id}`)
}

export function createWorkoutPlan(input: CreateWorkoutPlanDto) {
  return apiRequest<WorkoutPlanDto>('/api/app/workout-plan', {
    method: 'POST',
    body: input,
  })
}

export function updateWorkoutPlan(id: string, input: UpdateWorkoutPlanDto) {
  return apiRequest<WorkoutPlanDto>(`/api/app/workout-plan/${id}`, {
    method: 'PUT',
    body: input,
  })
}

export function deleteWorkoutPlan(id: string) {
  return apiRequest<void>(`/api/app/workout-plan/${id}`, {
    method: 'DELETE',
  })
}

export function archiveWorkoutPlan(id: string) {
  return apiRequest<void>(`/api/app/workout-plan/${id}/archive`, {
    method: 'POST',
  })
}

export function restoreWorkoutPlan(id: string) {
  return apiRequest<WorkoutPlanDto>(`/api/app/workout-plan/${id}/restore`, {
    method: 'POST',
  })
}

export function addWorkoutPlanExercise(
  id: string,
  input: CreateWorkoutPlanExerciseDto,
) {
  return apiRequest<WorkoutPlanDto>(`/api/app/workout-plan/${id}/exercise`, {
    method: 'POST',
    body: input,
  })
}

export function updateWorkoutPlanExercise(
  id: string,
  workoutPlanExerciseId: string,
  input: UpdateWorkoutPlanExerciseDto,
) {
  return apiRequest<WorkoutPlanDto>(
    `/api/app/workout-plan/${id}/exercise/${workoutPlanExerciseId}`,
    {
      method: 'PUT',
      body: input,
    },
  )
}

export function removeWorkoutPlanExercise(
  id: string,
  workoutPlanExerciseId: string,
) {
  return apiRequest<WorkoutPlanDto>(
    `/api/app/workout-plan/${id}/exercise/${workoutPlanExerciseId}`,
    {
      method: 'DELETE',
    },
  )
}

export function reorderWorkoutPlanExercises(
  id: string,
  input: ReorderWorkoutPlanExercisesDto,
) {
  return apiRequest<WorkoutPlanDto>(
    `/api/app/workout-plan/${id}/reorder-exercises`,
    {
      method: 'POST',
      body: input,
    },
  )
}
