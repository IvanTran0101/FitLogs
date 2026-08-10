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

/** Identifies optional profile values needed for useful dashboard targets and summaries. */
export type ProfileCompletionField = 'heightCm' | 'weightKg' | 'dailyTargetCalories'

/** Finds the dashboard-relevant profile values that are still blank in a server response. */
export function getProfileCompletionMissingFields(
  profile: UserProfileDto,
): ProfileCompletionField[] {
  const missingFields: ProfileCompletionField[] = []

  if (profile.heightCm === null || profile.heightCm === undefined) {
    missingFields.push('heightCm')
  }

  if (profile.weightKg === null || profile.weightKg === undefined) {
    missingFields.push('weightKg')
  }

  if (
    profile.dailyTargetCalories === null ||
    profile.dailyTargetCalories === undefined
  ) {
    missingFields.push('dailyTargetCalories')
  }

  return missingFields
}

/** Returns true when all dashboard-relevant optional profile values have been provided. */
export function isUserProfileComplete(profile: UserProfileDto) {
  return getProfileCompletionMissingFields(profile).length === 0
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
