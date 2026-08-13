import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { NeoInput } from '../../components/NeoInput'
import { NeoSelect } from '../../components/NeoSelect'
import { PageShell } from '../../components/PageShell'
import { useAuth } from '../../auth/useAuth'
import { FITLOGS_PERMISSIONS } from '../../auth/permissions'
import { PermissionGate } from '../../components/PermissionGate'
import {
  getMyProfile,
  updateMyProfile,
  type FitnessGoal,
  type Gender,
  type UpdateUserProfileDto,
  type UserProfileDto,
} from '../../api/profileApi'

const GENDER_OPTIONS = [
  { label: 'Nam', value: '0' },
  { label: 'Nữ', value: '1' },
  { label: 'Không muốn tiết lộ', value: '2' },
]

const FITNESS_GOAL_OPTIONS = [
  { label: 'Giảm cân', value: '1' },
  { label: 'Duy trì cân nặng', value: '2' },
  { label: 'Tăng cơ', value: '3' },
  { label: 'Cải thiện thể lực', value: '4' },
]

type ProfileFormState = {
  displayName: string
  gender: Gender
  dateOfBirth: string
  heightCm: string
  weightKg: string
  fitnessGoal: FitnessGoal
  dailyTargetCalories: string
  timeZoneId: string
}

/** Converts a nullable server date into the date-input format without applying a timezone shift. */
function toDateInputValue(value: string | null) {
  return value ? value.slice(0, 10) : ''
}

/** Converts an optional numeric input into the nullable decimal/integer field expected by the API. */
function parseOptionalNumber(value: string) {
  const trimmedValue = value.trim()
  if (!trimmedValue) {
    return null
  }

  const parsedValue = Number(trimmedValue)
  return Number.isFinite(parsedValue) ? parsedValue : Number.NaN
}

/** Maps the backend profile response into editable strings while keeping enum values numeric. */
function toFormState(profile: UserProfileDto): ProfileFormState {
  return {
    displayName: profile.displayName ?? '',
    gender: profile.gender,
    dateOfBirth: toDateInputValue(profile.dateOfBirth),
    heightCm: profile.heightCm === null ? '' : String(profile.heightCm),
    weightKg: profile.weightKg === null ? '' : String(profile.weightKg),
    fitnessGoal: profile.fitnessGoal,
    dailyTargetCalories:
      profile.dailyTargetCalories === null ? '' : String(profile.dailyTargetCalories),
    timeZoneId: profile.timeZoneId || 'UTC',
  }
}

/** Builds the exact update DTO and keeps blank optional fields as null instead of fake zeroes. */
function toUpdateDto(form: ProfileFormState): UpdateUserProfileDto {
  return {
    displayName: form.displayName.trim(),
    gender: form.gender,
    dateOfBirth: form.dateOfBirth || null,
    heightCm: parseOptionalNumber(form.heightCm),
    weightKg: parseOptionalNumber(form.weightKg),
    fitnessGoal: form.fitnessGoal,
    dailyTargetCalories: parseOptionalNumber(form.dailyTargetCalories),
    timeZoneId: form.timeZoneId.trim(),
  }
}

/** Coordinates profile loading, editable form state, validation, and backend persistence. */
export function ProfilePage() {
  const { login, logout } = useAuth()
  const [form, setForm] = useState<ProfileFormState | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [loadError, setLoadError] = useState<string | null>(null)
  const [saveError, setSaveError] = useState<string | null>(null)
  const [validationError, setValidationError] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)

  useEffect(() => {
    let isCurrentRequest = true

    // Loads the signed-in user's profile and fills the form from the normalized server response.
    async function loadProfile() {
      setIsLoading(true)
      setLoadError(null)

      try {
        const profile = await getMyProfile()
        if (isCurrentRequest) {
          setForm(toFormState(profile))
        }
      } catch (error) {
        if (isCurrentRequest) {
          setLoadError(
            error instanceof Error
              ? error.message
              : 'Không thể tải hồ sơ người dùng.',
          )
        }
      } finally {
        if (isCurrentRequest) {
          setIsLoading(false)
        }
      }
    }

    void loadProfile()

    return () => {
      isCurrentRequest = false
    }
  }, [reloadToken])

  // Re-runs the profile request after a transient backend or network failure.
  function retryLoad() {
    setReloadToken((currentToken) => currentToken + 1)
  }

  // Validates frontend-friendly ranges before sending the exact backend update contract.
  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!form || isSaving) {
      return
    }

    setSaveError(null)
    setValidationError(null)
    const updateDto = toUpdateDto(form)
    // Normalize optional numeric fields so validation treats omitted values as blank/null.
    const heightCm = updateDto.heightCm ?? null
    const weightKg = updateDto.weightKg ?? null
    const dailyTargetCalories = updateDto.dailyTargetCalories ?? null

    if (!updateDto.displayName) {
      setValidationError('Tên hiển thị không được để trống.')
      return
    }

    if (!updateDto.timeZoneId) {
      setValidationError('Múi giờ không được để trống.')
      return
    }

    if (
      heightCm !== null &&
      (!Number.isFinite(heightCm) || heightCm < 50 || heightCm > 250)
    ) {
      setValidationError('Chiều cao phải nằm trong khoảng từ 50 đến 250 cm.')
      return
    }

    if (
      weightKg !== null &&
      (!Number.isFinite(weightKg) || weightKg < 20 || weightKg > 300)
    ) {
      setValidationError('Cân nặng phải nằm trong khoảng từ 20 đến 300 kg.')
      return
    }

    if (
      dailyTargetCalories !== null &&
      (!Number.isInteger(dailyTargetCalories) ||
        dailyTargetCalories < 800 ||
        dailyTargetCalories > 6000)
    ) {
      setValidationError('Calories mục tiêu phải nằm trong khoảng từ 800 đến 6000.')
      return
    }

    if (
      Number.isNaN(heightCm) ||
      Number.isNaN(weightKg) ||
      Number.isNaN(dailyTargetCalories)
    ) {
      setValidationError('Các giá trị số phải là số hợp lệ.')
      return
    }

    setIsSaving(true)
    try {
      const updatedProfile = await updateMyProfile(updateDto)
      setForm(toFormState(updatedProfile))
    } catch (error) {
      setSaveError(
        error instanceof Error
          ? error.message
          : 'Không thể lưu hồ sơ người dùng.',
      )
    } finally {
      setIsSaving(false)
    }
  }

  // Updates one form field without discarding values entered in the other profile fields.
  function updateField<TKey extends keyof ProfileFormState>(
    key: TKey,
    value: ProfileFormState[TKey],
  ) {
    setForm((currentForm) =>
      currentForm ? { ...currentForm, [key]: value } : currentForm,
    )
    setSaveError(null)
    setValidationError(null)
  }

  return (
    <PageShell title="Hồ sơ">
      {isLoading ? <LoadingState message="Đang tải hồ sơ..." /> : null}
      {!isLoading && loadError ? (
        <ErrorState
          title="Không tải được hồ sơ"
          message={loadError}
          action={<NeoButton onClick={retryLoad}>Thử lại</NeoButton>}
        />
      ) : null}
      {!isLoading && !loadError && form ? (
        <NeoCard className="profile-form-card">
          <p className="eyebrow">Thông tin cá nhân</p>
          <h2>Hồ sơ của bạn</h2>

          <form className="form-grid" onSubmit={handleSubmit} aria-busy={isSaving}>
            <NeoInput
              label="Tên hiển thị"
              type="text"
              maxLength={100}
              value={form.displayName}
              disabled={isSaving}
              onChange={(event) => updateField('displayName', event.target.value)}
            />

            <NeoSelect
              label="Giới tính"
              value={String(form.gender)}
              options={GENDER_OPTIONS}
              disabled={isSaving}
              onChange={(event) => updateField('gender', Number(event.target.value) as Gender)}
            />

            <NeoInput
              label="Ngày sinh"
              type="date"
              value={form.dateOfBirth}
              disabled={isSaving}
              onChange={(event) => updateField('dateOfBirth', event.target.value)}
            />

            <div className="profile-field-grid">
              <NeoInput
                label="Chiều cao (cm)"
                type="number"
                min={50}
                max={250}
                step="0.1"
                value={form.heightCm}
                disabled={isSaving}
                onChange={(event) => updateField('heightCm', event.target.value)}
              />
              <NeoInput
                label="Cân nặng (kg)"
                type="number"
                min={20}
                max={300}
                step="0.1"
                value={form.weightKg}
                disabled={isSaving}
                onChange={(event) => updateField('weightKg', event.target.value)}
              />
            </div>

            <NeoSelect
              label="Mục tiêu"
              value={String(form.fitnessGoal)}
              options={FITNESS_GOAL_OPTIONS}
              disabled={isSaving}
              onChange={(event) =>
                updateField('fitnessGoal', Number(event.target.value) as FitnessGoal)
              }
            />

            <NeoInput
              label="Calories mục tiêu mỗi ngày"
              type="number"
              min={800}
              max={6000}
              step={1}
              value={form.dailyTargetCalories}
              disabled={isSaving}
              onChange={(event) => updateField('dailyTargetCalories', event.target.value)}
            />

            <NeoInput
              label="Múi giờ IANA"
              type="text"
              maxLength={64}
              placeholder="Asia/Ho_Chi_Minh hoặc UTC"
              value={form.timeZoneId}
              disabled={isSaving}
              onChange={(event) => updateField('timeZoneId', event.target.value)}
            />

            <p className="profile-timezone-help">
              Múi giờ giúp backend xác định đúng ngày cho nhật ký ăn uống và dashboard.
            </p>

            {validationError ? (
              <p className="profile-form-error" role="alert">
                {validationError}
              </p>
            ) : null}
            {saveError ? <ErrorState title="Không thể lưu hồ sơ" message={saveError} /> : null}

            <PermissionGate permission={FITLOGS_PERMISSIONS.userProfiles.update}>
              <NeoButton className="full-width-button" type="submit" disabled={isSaving}>
                {isSaving ? 'Đang lưu...' : 'Lưu thay đổi'}
              </NeoButton>
            </PermissionGate>
          </form>

          <div className="profile-auth-actions">
            <NeoButton onClick={() => void login()}>Đăng nhập</NeoButton>
            <NeoButton onClick={() => void logout()}>Đăng xuất</NeoButton>
          </div>
        </NeoCard>
      ) : null}
    </PageShell>
  )
}
