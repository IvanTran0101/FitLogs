import { apiRequest } from './httpClient'
import type { PagedResult } from './exercisesApi'

export type WorkoutSessionStatus = 0 | 1 | 2

export type WorkoutSessionExerciseStatus = 0 | 1 | 2 | 3

export type WorkoutSessionDto = {
  id: string
  creationTime: string
  creatorId: string | null
  lastModificationTime: string | null
  lastModifierId: string | null
  isDeleted: boolean
  deleterId: string | null
  deletionTime: string | null
  userId: string
  workoutPlanId: string | null
  name: string | null
  startedAt: string
  endedAt: string | null
  status: WorkoutSessionStatus
  note: string | null
  currentWorkoutSessionExerciseId: string | null
  exercises: WorkoutSessionExerciseDto[] | null
}

export type WorkoutSessionExerciseDto = {
  id: string
  workoutSessionId: string
  exerciseId: string
  orderIndex: number
  targetSets: number
  targetReps: number
  targetWeightKg: number | null
  restSeconds: number | null
  note: string | null
  status: WorkoutSessionExerciseStatus
  sets: ExerciseSetDto[] | null
}

export type ExerciseSetDto = {
  id: string
  workoutSessionExerciseId: string
  setNumber: number
  weightKg: number
  reps: number
  rpe: number | null
  note: string | null
  isCompleted: boolean
  completedAt: string | null
  isSkipped: boolean
  skippedAt: string | null
}

export type GetWorkoutSessionListQuery = {
  FilterText?: string
  Status?: WorkoutSessionStatus
  WorkoutPlanId?: string
  StartedFrom?: string
  StartedTo?: string
  Sorting?: string
  SkipCount?: number
  MaxResultCount?: number
}

export type CreateWorkoutSessionDto = {
  workoutPlanId?: string | null
  name?: string | null
  startedAt?: string
  note?: string | null
}

export type StartWorkoutFromPlanDto = {
  workoutPlanId: string
  startedAt?: string | null
  note?: string | null
}

export type StartFreeWorkoutDto = {
  name: string
  startedAt?: string | null
  note?: string | null
}

export type AddWorkoutSessionExerciseDto = {
  exerciseId: string
  orderIndex: number
  targetSets: number
  targetReps: number
  targetWeightKg?: number | null
  restSeconds?: number | null
  note?: string | null
}

export type UpdateWorkoutSessionExerciseDto = {
  orderIndex: number
  targetSets: number
  targetReps: number
  targetWeightKg?: number | null
  restSeconds?: number | null
  note?: string | null
}

export type AddExerciseSetDto = {
  setNumber: number
  weightKg: number
  reps: number
  rpe?: number | null
  note?: string | null
}

export type UpdateExerciseSetDto = {
  setNumber: number
  weightKg: number
  reps: number
  rpe?: number | null
  note?: string | null
}

export function getWorkoutSessions(query: GetWorkoutSessionListQuery = {}) {
  return apiRequest<PagedResult<WorkoutSessionDto>>('/api/app/workout-session', {
    query,
  })
}

export function getWorkoutSession(id: string) {
  return apiRequest<WorkoutSessionDto>(`/api/app/workout-session/${id}`)
}

export function createWorkoutSession(input: CreateWorkoutSessionDto) {
  return apiRequest<WorkoutSessionDto>('/api/app/workout-session', {
    method: 'POST',
    body: input,
  })
}

export function startWorkoutFromPlan(input: StartWorkoutFromPlanDto) {
  return apiRequest<WorkoutSessionDto>('/api/app/workout-session/start-from-plan', {
    method: 'POST',
    body: input,
  })
}

export function startFreeWorkout(input: StartFreeWorkoutDto) {
  return apiRequest<WorkoutSessionDto>('/api/app/workout-session/start-free-workout', {
    method: 'POST',
    body: input,
  })
}

export function deleteWorkoutSession(id: string) {
  return apiRequest<void>(`/api/app/workout-session/${id}`, {
    method: 'DELETE',
  })
}

export function completeWorkoutSession(id: string) {
  return apiRequest<WorkoutSessionDto>(
    `/api/app/workout-session/${id}/complete`,
    {
      method: 'POST',
    },
  )
}

export function cancelWorkoutSession(id: string) {
  return apiRequest<WorkoutSessionDto>(
    `/api/app/workout-session/${id}/cancel`,
    {
      method: 'POST',
    },
  )
}

export function getActiveWorkoutSession() {
  return apiRequest<WorkoutSessionDto | null>('/api/app/workout-session/active')
}

export function getCurrentWorkoutSessionExercise(id: string) {
  return apiRequest<WorkoutSessionExerciseDto>(
    `/api/app/workout-session/${id}/current-exercise`,
  )
}

export function moveToNextWorkoutSessionExercise(id: string) {
  return apiRequest<WorkoutSessionDto>(
    `/api/app/workout-session/${id}/move-to-next-exercise`,
    {
      method: 'POST',
    },
  )
}

export function moveToPreviousWorkoutSessionExercise(id: string) {
  return apiRequest<WorkoutSessionDto>(
    `/api/app/workout-session/${id}/move-to-previous-exercise`,
    {
      method: 'POST',
    },
  )
}

export function skipCurrentWorkoutSessionExercise(id: string) {
  return apiRequest<WorkoutSessionDto>(
    `/api/app/workout-session/${id}/skip-current-exercise`,
    {
      method: 'POST',
    },
  )
}

export function addWorkoutSessionExercise(
  id: string,
  input: AddWorkoutSessionExerciseDto,
) {
  return apiRequest<WorkoutSessionDto>(
    `/api/app/workout-session/${id}/exercise`,
    {
      method: 'POST',
      body: input,
    },
  )
}

export function updateWorkoutSessionExercise(
  id: string,
  workoutSessionExerciseId: string,
  input: UpdateWorkoutSessionExerciseDto,
) {
  return apiRequest<WorkoutSessionDto>(
    `/api/app/workout-session/${id}/exercise/${workoutSessionExerciseId}`,
    {
      method: 'PUT',
      body: input,
    },
  )
}

export function removeWorkoutSessionExercise(
  id: string,
  workoutSessionExerciseId: string,
) {
  return apiRequest<WorkoutSessionDto>(
    `/api/app/workout-session/${id}/exercise/${workoutSessionExerciseId}`,
    {
      method: 'DELETE',
    },
  )
}

export function addExerciseSet(
  id: string,
  workoutSessionExerciseId: string,
  input: AddExerciseSetDto,
) {
  return apiRequest<WorkoutSessionDto>(
    `/api/app/workout-session/${id}/set/${workoutSessionExerciseId}`,
    {
      method: 'POST',
      body: input,
    },
  )
}

export function updateExerciseSet(
  id: string,
  workoutSessionExerciseId: string,
  exerciseSetId: string,
  input: UpdateExerciseSetDto,
) {
  return apiRequest<WorkoutSessionDto>(`/api/app/workout-session/${id}/set`, {
    method: 'PUT',
    query: {
      workoutSessionExerciseId,
      exerciseSetId,
    },
    body: input,
  })
}

export function removeExerciseSet(
  id: string,
  workoutSessionExerciseId: string,
  exerciseSetId: string,
) {
  return apiRequest<WorkoutSessionDto>(`/api/app/workout-session/${id}/set`, {
    method: 'DELETE',
    query: {
      workoutSessionExerciseId,
      exerciseSetId,
    },
  })
}

export function completeExerciseSet(
  id: string,
  workoutSessionExerciseId: string,
  exerciseSetId: string,
) {
  return apiRequest<WorkoutSessionDto>(
    `/api/app/workout-session/${id}/complete-set`,
    {
      method: 'POST',
      query: {
        workoutSessionExerciseId,
        exerciseSetId,
      },
    },
  )
}

export function uncompleteExerciseSet(
  id: string,
  workoutSessionExerciseId: string,
  exerciseSetId: string,
) {
  return apiRequest<WorkoutSessionDto>(
    `/api/app/workout-session/${id}/uncomplete-set`,
    {
      method: 'POST',
      query: {
        workoutSessionExerciseId,
        exerciseSetId,
      },
    },
  )
}

// Marks one set as intentionally skipped and returns the refreshed workout session.
export function skipExerciseSet(
  id: string,
  workoutSessionExerciseId: string,
  exerciseSetId: string,
) {
  return apiRequest<WorkoutSessionDto>(
    `/api/app/workout-session/${id}/skip-set`,
    {
      method: 'POST',
      query: {
        workoutSessionExerciseId,
        exerciseSetId,
      },
    },
  )
}

// Reopens one skipped set so the user can record it as performed.
export function unskipExerciseSet(
  id: string,
  workoutSessionExerciseId: string,
  exerciseSetId: string,
) {
  return apiRequest<WorkoutSessionDto>(
    `/api/app/workout-session/${id}/unskip-set`,
    {
      method: 'POST',
      query: {
        workoutSessionExerciseId,
        exerciseSetId,
      },
    },
  )
}
