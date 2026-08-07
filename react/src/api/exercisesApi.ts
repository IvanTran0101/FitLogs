import { apiRequest } from './httpClient'

export type PagedResult<TItem> = {
  items: TItem[] | null
  totalCount: number
}

export type ExerciseDifficulty = 1 | 2 | 3

export type ExerciseTrackingType = 1 | 2 | 3 | 4

export type ExerciseDto = {
  id: string
  name: string | null
  slug: string | null
  description: string | null
  primaryMuscleGroupId: string
  equipmentId: string
  difficulty: ExerciseDifficulty
  trackingType: ExerciseTrackingType
  imageUrl: string | null
  gifUrl: string | null
  instructions: string | null
  formTips: string | null
  commonMistakes: string | null
  isActive: boolean
}

export type MuscleGroupDto = {
  id: string
  name: string | null
  code: string | null
  description: string | null
  displayOrder: number
  isActive: boolean
}

export type EquipmentDto = {
  id: string
  name: string | null
  code: string | null
  description: string | null
  displayOrder: number
  isActive: boolean
}

export type GetExerciseListQuery = {
  FilterText?: string
  MuscleGroupId?: string
  EquipmentId?: string
  Difficulty?: ExerciseDifficulty
  TrackingType?: ExerciseTrackingType
  IsActive?: boolean
  Sorting?: string
  SkipCount?: number
  MaxResultCount?: number
}

export type GetReferenceListQuery = {
  FilterText?: string
  IsActive?: boolean
  Sorting?: string
  SkipCount?: number
  MaxResultCount?: number
}

export function getExercise(id: string) {
  return apiRequest<ExerciseDto>(`/api/app/exercise/${id}`)
}

export function getExerciseBySlug(slug: string) {
  return apiRequest<ExerciseDto>('/api/app/exercise/by-slug', {
    query: { slug },
  })
}

export function getExercises(query: GetExerciseListQuery = {}) {
  return apiRequest<PagedResult<ExerciseDto>>('/api/app/exercise', {
    query,
  })
}

export function getSelectableExercises(query: GetExerciseListQuery = {}) {
  return apiRequest<PagedResult<ExerciseDto>>('/api/app/exercise/selectable-list', {
    query,
  })
}

export function getMuscleGroups(query: GetReferenceListQuery = {}) {
  return apiRequest<PagedResult<MuscleGroupDto>>('/api/app/muscle-group', {
    query: {
      IsActive: true,
      Sorting: 'displayOrder asc',
      MaxResultCount: 100,
      ...query,
    },
  })
}

export function getEquipments(query: GetReferenceListQuery = {}) {
  return apiRequest<PagedResult<EquipmentDto>>('/api/app/equipment', {
    query: {
      IsActive: true,
      Sorting: 'displayOrder asc',
      MaxResultCount: 100,
      ...query,
    },
  })
}