import { apiRequest } from './httpClient'

/** Gender values used by the backend profile aggregate. */
export type Gender = 0 | 1 | 2

/** Fitness-goal values used by the backend profile aggregate. */
export type FitnessGoal = 1 | 2 | 3 | 4

export type UserProfileDto = {
  id: string
  userId: string
  displayName: string | null
  gender: Gender
  dateOfBirth: string | null
  heightCm: number | null
  weightKg: number | null
  fitnessGoal: FitnessGoal
  dailyTargetCalories: number | null
  timeZoneId: string
}

export type UpdateUserProfileDto = {
  displayName: string
  gender: Gender
  dateOfBirth?: string | null
  heightCm?: number | null
  weightKg?: number | null
  fitnessGoal: FitnessGoal
  dailyTargetCalories?: number | null
  timeZoneId: string
}

/** Loads the signed-in user's profile; the backend creates a default profile when it is missing. */
export function getMyProfile() {
  return apiRequest<UserProfileDto>('/api/app/user-profile/my-profile')
}

/** Saves the editable profile fields and returns the backend's normalized profile response. */
export function updateMyProfile(input: UpdateUserProfileDto) {
  return apiRequest<UserProfileDto>('/api/app/user-profile/my-profile', {
    method: 'PUT',
    body: input,
  })
}
