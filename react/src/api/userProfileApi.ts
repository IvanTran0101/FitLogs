import { apiRequest } from './httpClient'

export type Gender = 0 | 1 | 2
export type FitnessGoal = 1 | 2 | 3 | 4

export type UserProfileDto = {
  id: string
  userId: string
  displayName: string
  gender: Gender
  dateOfBirth: string | null
  heightCm: number | null
  weightKg: number | null
  fitnessGoal: FitnessGoal
  dailyTargetCalories: number | null
  timeZoneId: string
}

export type UpdateUserProfileDto = Omit<UserProfileDto, 'id' | 'userId'>

export function getMyProfile() {
  return apiRequest<UserProfileDto>('/api/app/user-profile/my-profile')
}

export function updateMyProfile(input: UpdateUserProfileDto) {
  return apiRequest<UserProfileDto>('/api/app/user-profile/my-profile', {
    method: 'PUT',
    body: input,
  })
}
