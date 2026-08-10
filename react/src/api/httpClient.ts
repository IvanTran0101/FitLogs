import { API_BASE_URL } from './config'
import { clearUserSession, getAccessToken } from '../auth/authService'
type QueryValue = string | number | boolean | null | undefined

type RequestMethod = 'GET' | 'POST' | 'PUT' | 'DELETE'

type RequestOptions = {
  method?: RequestMethod
  body?: unknown
  query?: Record<string, QueryValue>
}

type AbpErrorResponse = {
  error?: {
    code?: string
    message?: string
    details?: string
    validationErrors?: {
      message?: string
      members?: string[]
    }[]
  }
}

const BUSINESS_ERROR_MESSAGES: Record<string, string> = {
  'FitLogs:UserProfile:InvalidDateOfBirth':
    'Ngày sinh không hợp lệ.',
  'FitLogs:UserProfile:InvalidHeightCm':
    'Chiều cao phải nằm trong khoảng hợp lệ.',
  'FitLogs:UserProfile:InvalidWeightKg':
    'Cân nặng phải nằm trong khoảng hợp lệ.',
  'FitLogs:UserProfile:InvalidDailyTargetCalories':
    'Calories mục tiêu phải nằm trong khoảng hợp lệ.',
  'FitLogs:UserProfile:InvalidGender':
    'Giới tính không hợp lệ.',
  'FitLogs:UserProfile:InvalidFitnessGoal':
    'Mục tiêu thể chất không hợp lệ.',
  'FitLogs:UserProfile:InvalidTimeZoneId':
    'Múi giờ không hợp lệ.',
  'FitLogs:UserProfile:AlreadyExists':
    'Hồ sơ người dùng đã tồn tại.',
  'FitLogs:UserProfile:NotFound':
    'Không tìm thấy hồ sơ người dùng.',
  'FitLogs:UserProfile:ForbiddenProfileAccess':
    'Bạn không có quyền truy cập hồ sơ này.',
  'FitLogs:Food:InvalidQuantity':
    'Số lượng món ăn không hợp lệ. Hãy nhập giá trị lớn hơn 0.',
  'FitLogs:Food:ProductNotFound':
    'Không tìm thấy sản phẩm thực phẩm đã chọn.',
  'FitLogs:FoodProduct:001':
    'Calories của sản phẩm không được âm.',
  'FitLogs:FoodProduct:002':
    'Protein của sản phẩm không được âm.',
  'FitLogs:FoodProduct:003':
    'Carbohydrate của sản phẩm không được âm.',
  'FitLogs:FoodProduct:004':
    'Chất béo của sản phẩm không được âm.',
  'FitLogs:FoodProduct:005':
    'Nguồn sản phẩm thực phẩm không hợp lệ.',
  'FitLogs:FoodProduct:006':
    'Mã vạch này đã tồn tại trong hệ thống.',
  'FitLogs:FoodProduct:007':
    'Sản phẩm thực phẩm này đang bị tắt.',
  'FitLogs:FoodProduct:BarcodeInvalid':
    'Mã vạch không hợp lệ hoặc đang bị bỏ trống.',
  'FitLogs:FoodProduct:UpstreamProductNotFound':
    'Không tìm thấy sản phẩm với mã vạch này.',
  'FitLogs:FoodProduct:UpstreamInvalidData':
    'Open Food Facts trả về dữ liệu sản phẩm không hợp lệ.',
  'FitLogs:FoodProduct:UpstreamTimeout':
    'Open Food Facts phản hồi quá lâu. Hãy thử lại sau.',
  'FitLogs:FoodProduct:UpstreamUnavailable':
    'Open Food Facts hiện không khả dụng. Hãy thử lại sau.',
  'FitLogs:FoodProduct:PersistenceFailed':
    'Không thể lưu sản phẩm vào danh mục cục bộ. Hãy thử lại.',
  'FitLogs:FoodProduct:NutritionUnknown':
    'Sản phẩm chưa có dữ liệu calories nên chưa thể ghi nhật ký.',
  'FitLogs:FoodLog:001':
    'Không xác định được người dùng cho nhật ký món ăn.',
  'FitLogs:FoodLog:002':
    'Số lượng món ăn phải lớn hơn 0.',
  'FitLogs:FoodLog:003':
    'Calories của nhật ký món ăn không được âm.',
  'FitLogs:FoodLog:004':
    'Protein của nhật ký món ăn không được âm.',
  'FitLogs:FoodLog:005':
    'Carbohydrate của nhật ký món ăn không được âm.',
  'FitLogs:FoodLog:006':
    'Chất béo của nhật ký món ăn không được âm.',
  'FitLogs:FoodLog:007':
    'Thời điểm ghi món ăn là bắt buộc.',
  'FitLogs:FoodLog:008':
    'Đơn vị món ăn không hợp lệ.',
  'FitLogs:FoodLog:009':
    'Loại bữa ăn không hợp lệ.',
  'FitLogs:FoodLog:010':
    'Sản phẩm thực phẩm là bắt buộc.',
  'FitLogs:FoodLog:0001':
    'Không xác định được người dùng cho nhật ký món ăn.',
  'FitLogs:FoodLog:0002':
    'Sản phẩm thực phẩm là bắt buộc.',
  'FitLogs:FoodLog:0004':
    'Số lượng món ăn không hợp lệ.',
  'FitLogs:FoodLog:0005':
    'Thông tin dinh dưỡng không hợp lệ.',
  'FitLogs:FoodLog:0006':
    'Bạn không có quyền chỉnh sửa nhật ký món ăn này.',
  'FitLogs:FoodLog:FoodLogAccessDenied':
    'Bạn không có quyền truy cập nhật ký món ăn này.',
  'FitLogs:FoodLog:FoodProductNotFoundFromOpenFoodFacts':
    'Không tìm thấy sản phẩm này từ Open Food Facts.',
  'FitLogs:Workout:UserHasInProgressWorkoutSession':
    'Bạn đã có một buổi tập đang diễn ra. Hãy tiếp tục hoặc kết thúc buổi đó trước khi bắt đầu buổi mới.',
  'FitLogs:Workout:WorkoutPlanIsInactive':
    'Kế hoạch này đang tắt và không thể bắt đầu buổi tập.',
  'FitLogs:Workout:WorkoutPlanNotBelongToUser':
    'Bạn không có quyền sử dụng kế hoạch tập này.',
  'FitLogs:Workout:WorkoutPlanAccessDenied':
    'Bạn không có quyền truy cập kế hoạch tập này.',
  'FitLogs:Workout:ExerciseIsInactive':
    'Bài tập này đã bị tắt và không thể thêm vào buổi tập.',
  'FitLogs:Workout:WorkoutSessionStatusIsNotInProgress':
    'Buổi tập đã kết thúc và không thể tiếp tục chỉnh sửa.',
  'FitLogs:Workout:SessionNotInProgress':
    'Buổi tập đã kết thúc và không thể tiếp tục chỉnh sửa.',
  'FitLogs:Workout:CurrentWorkoutSessionExerciseNotFound':
    'Buổi tập chưa có bài tập hiện tại.',
  'FitLogs:Workout:NextWorkoutSessionExerciseNotFound':
    'Bạn đang ở bài tập cuối cùng.',
  'FitLogs:Workout:PreviousWorkoutSessionExerciseNotFound':
    'Bạn đang ở bài tập đầu tiên.',
  'FitLogs:Workout:WorkoutSessionExerciseAlreadyExists':
    'Bài tập này đã có trong buổi tập.',
  'FitLogs:Workout:WorkoutSessionExerciseOrderIndexAlreadyExists':
    'Thứ tự bài tập bị trùng. Hãy thử lại với thứ tự khác.',
  'FitLogs:Workout:WorkoutSessionExerciseNotFound':
    'Không tìm thấy bài tập trong buổi tập.',
  'FitLogs:Workout:ExerciseSetAlreadyCompleted':
    'Set này đã được hoàn thành.',
  'FitLogs:Workout:ExerciseSetNumberAlreadyExists':
    'Số set này đã tồn tại trong bài tập.',
  'FitLogs:Workout:ExerciseSetNotFound':
    'Không tìm thấy set trong bài tập.',
  'FitLogs:Workout:InvalidExerciseSetNumber':
    'Số set phải là số nguyên hợp lệ.',
  'FitLogs:Workout:InvalidExerciseSetWeight':
    'Khối lượng set không hợp lệ.',
  'FitLogs:Workout:InvalidExerciseSetReps':
    'Số reps của set không hợp lệ.',
  'FitLogs:Workout:InvalidExerciseSetRpe':
    'RPE của set phải nằm trong khoảng cho phép.',
  'FitLogs:Workout:InvalidWorkoutSessionExerciseTargetSets':
    'Số set mục tiêu không hợp lệ.',
  'FitLogs:Workout:InvalidWorkoutSessionExerciseTargetReps':
    'Số reps mục tiêu không hợp lệ.',
  'FitLogs:Workout:InvalidWorkoutSessionExerciseTargetWeightKg':
    'Khối lượng mục tiêu không hợp lệ.',
  'FitLogs:Workout:InvalidWorkoutSessionExerciseRestSeconds':
    'Thời gian nghỉ không hợp lệ.',
  'FitLogs:Workout:InvalidWorkoutSessionEndedAt':
    'Thời điểm kết thúc buổi tập không hợp lệ.',
  'FitLogs:Workout:InvalidWorkoutSessionStartedAt':
    'Thời điểm bắt đầu buổi tập không hợp lệ.',
  'FitLogs:Workout:WorkoutPlanMustHaveAtLeastOneExercise':
    'Kế hoạch phải có ít nhất một bài tập trước khi bắt đầu.',
  'FitLogs:Workout:WorkoutPlanNameAlreadyExists':
    'Tên kế hoạch này đã được bạn sử dụng. Hãy chọn tên khác.',
  'FitLogs:Workout:PlanWorkoutNameNotAllowed':
    'Buổi tập từ kế hoạch không nhận tên riêng; hãy bỏ trống tên.',
  'FitLogs:Workout:WorkoutPlanExerciseAlreadyExists':
    'Bài tập này đã có trong kế hoạch.',
  'FitLogs:Workout:WorkoutPlanNotFound':
    'Không tìm thấy kế hoạch tập.',
  'FitLogs:Workout:CompletedWorkoutSessionCannotBeDeleted':
    'Buổi tập đã hoàn thành và không thể xoá.',
  'FitLogs:Workout:WorkoutSessionAccessDenied':
    'Bạn không có quyền truy cập buổi tập này.',
}

export class ApiError extends Error {
  readonly code: string | undefined
  readonly status: number

  constructor(message: string, status: number, code?: string) {
    super(message)
    this.name = 'ApiError'
    this.code = code
    this.status = status
  }
}

function buildUrl(path: string, query?: RequestOptions['query']) {
  const url = new URL(path, API_BASE_URL)

  if (!query) {
    return url.toString()
  }

  for (const [key, value] of Object.entries(query)) {
    if (value === null || value === undefined || value === '') {
      continue
    }

    url.searchParams.set(key, String(value))
  }

  return url.toString()
}

/** Detects an expired/unauthorized API response, including ABP redirects to its login page. */
function isUnauthorizedResponse(response: Response) {
  return response.status === 401 || (response.redirected && response.url.includes('/Login'))
}

/** Clears the local session without hiding the original API error if storage cleanup fails. */
async function clearSessionAfterUnauthorized() {
  try {
    await clearUserSession()
  } catch {
    // The caller still needs the original 401/login-redirect error for its UI state.
  }
}

// This function turns an ABP error response into a clear user message while preserving its backend code.
async function readErrorInfo(response: Response) {
  const contentType = response.headers.get('content-type') ?? ''

  if (response.redirected && response.url.includes('/Login')) {
    return {
      message: 'Bạn cần đăng nhập để xem dữ liệu.',
      code: undefined,
    }
  }

  if (!contentType.includes('application/json')) {
    return {
      message: `API request failed: ${response.status}`,
      code: undefined,
    }
  }

  const data = (await response.json()) as AbpErrorResponse
  const code = data.error?.code
  const validationMessage = data.error?.validationErrors?.[0]?.message
  const businessMessage = code ? BUSINESS_ERROR_MESSAGES[code] : undefined

  return {
    message:
      businessMessage ??
      validationMessage ??
      data.error?.details ??
      (code
        ? `Backend đã từ chối thao tác (${code}).`
        : data.error?.message) ??
      `API request failed: ${response.status}`,
    code,
  }
}

async function readResponseJson<TResponse>(response: Response): Promise<TResponse> {
  if (response.status === 204) {
    return undefined as TResponse
  }

  const contentType = response.headers.get('content-type') ?? ''

  if (!contentType.includes('application/json')) {
    return undefined as TResponse
  }

  return response.json() as Promise<TResponse>
}

export async function apiRequest<TResponse>(
  path: string,
  options: RequestOptions = {},
): Promise<TResponse> {
  const headers: HeadersInit = {
    Accept: 'application/json',
  }
  const token = await getAccessToken()

  
  if (options.body !== undefined) {
    headers['Content-Type'] = 'application/json'
  }
  if (token) {
    headers.Authorization = `Bearer ${token}`
  }
  const response = await fetch(buildUrl(path, options.query), {
    method: options.method ?? 'GET',
    headers,
    credentials: 'include',
    body: options.body !== undefined ? JSON.stringify(options.body) : undefined,
  })

  if (!response.ok || (response.redirected && response.url.includes('/Login'))) {
    if (isUnauthorizedResponse(response)) {
      await clearSessionAfterUnauthorized()
    }

    const errorInfo = await readErrorInfo(response)
    throw new ApiError(errorInfo.message, response.status, errorInfo.code)
  }

  return readResponseJson<TResponse>(response)
}
