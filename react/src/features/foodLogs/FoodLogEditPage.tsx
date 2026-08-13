import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { Link, useNavigate, useParams, useSearchParams } from 'react-router-dom'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { NeoInput } from '../../components/NeoInput'
import { NeoSelect } from '../../components/NeoSelect'
import { PageShell } from '../../components/PageShell'
import {
  deleteFoodLog,
  getFoodLog,
  updateFoodLog,
  type FoodLogDto,
  type FoodUnit,
  type MealType,
} from '../../api/foodsApi'

const FOOD_UNIT_OPTIONS = [
  { label: 'Gram (g)', value: '1' },
  { label: 'Milliliter (ml)', value: '2' },
  { label: 'Suất', value: '3' },
  { label: 'Cái', value: '4' },
]

const MEAL_TYPE_OPTIONS = [
  { label: 'Bữa sáng', value: '1' },
  { label: 'Bữa trưa', value: '2' },
  { label: 'Bữa tối', value: '3' },
  { label: 'Ăn nhẹ', value: '4' },
  { label: 'Trước tập', value: '5' },
  { label: 'Sau tập', value: '6' },
]

/** Keeps the server's date-time components without applying an unverified timezone conversion. */
function toDateTimeLocalValue(value: string) {
  const match = value.match(/^(\d{4}-\d{2}-\d{2}T\d{2}:\d{2})/)
  return match?.[1] ?? ''
}

/** Converts the HTML datetime-local value into the backend's date-time string. */
function toApiDateTime(value: string) {
  return value ? `${value}:00` : null
}

/** Coordinates loading, editing, deleting, and refreshing one owned food log. */
export function FoodLogEditPage() {
  const { foodLogId } = useParams<{ foodLogId: string }>()
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const [foodLog, setFoodLog] = useState<FoodLogDto | null>(null)
  const [quantity, setQuantity] = useState('')
  const [unit, setUnit] = useState<FoodUnit>(1)
  const [mealType, setMealType] = useState<MealType>(1)
  const [loggedAt, setLoggedAt] = useState('')
  const [note, setNote] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [isDeleting, setIsDeleting] = useState(false)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [formError, setFormError] = useState<string | null>(null)
  const [quantityError, setQuantityError] = useState<string | undefined>()
  const [reloadToken, setReloadToken] = useState(0)
  const returnDate = searchParams.get('date')

  // Loads the owned log once and copies only editable backend fields into the form state.
  useEffect(() => {
    let isCurrentRequest = true

    async function loadFoodLog() {
      if (!foodLogId) {
        setErrorMessage('Không tìm thấy mã nhật ký món ăn.')
        setIsLoading(false)
        return
      }

      setIsLoading(true)
      setErrorMessage(null)

      try {
        const result = await getFoodLog(foodLogId)
        if (!isCurrentRequest) {
          return
        }

        setFoodLog(result)
        setQuantity(String(result.quantity))
        setUnit(result.unit)
        setMealType(result.mealType)
        setLoggedAt(toDateTimeLocalValue(result.loggedAt))
        setNote(result.note ?? '')
      } catch (error) {
        if (!isCurrentRequest) {
          return
        }

        setErrorMessage(
          error instanceof Error
            ? error.message
            : 'Không thể tải nhật ký món ăn.',
        )
      } finally {
        if (isCurrentRequest) {
          setIsLoading(false)
        }
      }
    }

    void loadFoodLog()

    return () => {
      isCurrentRequest = false
    }
  }, [foodLogId, reloadToken])

  // Re-runs the food-log request after a transient backend or network failure.
  function retryLoad() {
    setReloadToken((currentToken) => currentToken + 1)
  }

  // Validates the exact DTO range before updating so recoverable errors keep the user's input visible.
  async function handleSave(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!foodLogId) {
      return
    }

    setFormError(null)
    setQuantityError(undefined)
    const parsedQuantity = Number(quantity)
    if (!Number.isFinite(parsedQuantity) || parsedQuantity < 0.01 || parsedQuantity > 999999) {
      setQuantityError('Số lượng phải nằm trong khoảng từ 0.01 đến 999999.')
      return
    }

    setIsSaving(true)
    try {
      await updateFoodLog(foodLogId, {
        foodProductId: foodLog?.foodProductId ?? '',
        quantity: parsedQuantity,
        unit,
        mealType,
        loggedAt: toApiDateTime(loggedAt),
        note: note.trim() || null,
      })

      const selectedDay = loggedAt.slice(0, 10) || searchParams.get('date') || ''
      navigate(selectedDay ? `/food?date=${selectedDay}` : '/food', { replace: true })
    } catch (error) {
      setFormError(
        error instanceof Error
          ? error.message
          : 'Không thể cập nhật nhật ký món ăn.',
      )
    } finally {
      setIsSaving(false)
    }
  }

  // Confirms deletion and returns to the selected day only after the backend confirms success.
  async function handleDelete() {
    if (!foodLogId || isDeleting) {
      return
    }

    if (!window.confirm('Bạn có chắc muốn xoá nhật ký món ăn này không?')) {
      return
    }

    setFormError(null)
    setIsDeleting(true)
    try {
      await deleteFoodLog(foodLogId)
      const selectedDay = loggedAt.slice(0, 10) || searchParams.get('date') || ''
      navigate(selectedDay ? `/food?date=${selectedDay}` : '/food', { replace: true })
    } catch (error) {
      setFormError(
        error instanceof Error
          ? error.message
          : 'Không thể xoá nhật ký món ăn.',
      )
    } finally {
      setIsDeleting(false)
    }
  }

  return (
    <PageShell title="Sửa món ăn">
      <div className="food-add-stack">
        <Link className="back-link" to={returnDate ? `/food?date=${returnDate}` : '/food'}>
          ← Quay lại nhật ký
        </Link>

        {isLoading ? <LoadingState message="Đang tải nhật ký món ăn..." /> : null}
        {!isLoading && errorMessage ? (
          <ErrorState
            title="Không tải được nhật ký"
            message={errorMessage}
            action={<NeoButton onClick={retryLoad}>Thử lại</NeoButton>}
          />
        ) : null}
        {!isLoading && !errorMessage && foodLog ? (
          <NeoCard className="food-add-card food-log-form-card">
            <p className="eyebrow">Món ăn</p>
            <h2>{foodLog.foodName}</h2>
            <p className="food-add-help">
              Calories và macro sẽ được tính lại ở máy chủ khi bạn lưu.
            </p>
            <form className="food-log-form" onSubmit={handleSave} aria-busy={isSaving || isDeleting}>
              <NeoInput
                label="Số lượng"
                type="number"
                min="0.01"
                max="999999"
                step="0.01"
                value={quantity}
                disabled={isSaving || isDeleting}
                onChange={(event) => setQuantity(event.target.value)}
                error={quantityError}
              />
              <div className="food-form-grid">
                <NeoSelect
                  label="Đơn vị"
                  value={String(unit)}
                  disabled={isSaving || isDeleting}
                  onChange={(event) => setUnit(Number(event.target.value) as FoodUnit)}
                  options={FOOD_UNIT_OPTIONS}
                />
                <NeoSelect
                  label="Bữa ăn"
                  value={String(mealType)}
                  disabled={isSaving || isDeleting}
                  onChange={(event) => setMealType(Number(event.target.value) as MealType)}
                  options={MEAL_TYPE_OPTIONS}
                />
              </div>
              <NeoInput
                label="Thời điểm"
                type="datetime-local"
                value={loggedAt}
                disabled={isSaving || isDeleting}
                onChange={(event) => setLoggedAt(event.target.value)}
              />
              <NeoInput
                label="Ghi chú (tuỳ chọn)"
                maxLength={512}
                value={note}
                disabled={isSaving || isDeleting}
                onChange={(event) => setNote(event.target.value)}
              />
              <div className="food-edit-actions">
                <NeoButton type="submit" disabled={isSaving || isDeleting}>
                  {isSaving ? 'Đang lưu...' : 'Lưu thay đổi'}
                </NeoButton>
                <NeoButton
                  className="danger-button"
                  type="button"
                  disabled={isSaving || isDeleting}
                  onClick={() => void handleDelete()}
                >
                  {isDeleting ? 'Đang xoá...' : 'Xoá nhật ký'}
                </NeoButton>
              </div>
            </form>
            {formError ? <ErrorState title="Thao tác thất bại" message={formError} /> : null}
          </NeoCard>
        ) : null}
      </div>
    </PageShell>
  )
}
