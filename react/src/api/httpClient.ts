import { API_BASE_URL } from './config'
import { getAccessToken } from '../auth/authService'
type QueryValue = string | number | boolean | null | undefined

type RequestMethod = 'GET' | 'POST' | 'PUT' | 'DELETE'

type RequestOptions = {
  method?: RequestMethod
  body?: unknown
  query?: Record<string, QueryValue>
}

type AbpErrorResponse = {
  error?: {
    message?: string
    details?: string
    validationErrors?: {
      message?: string
      members?: string[]
    }[]
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

async function readErrorMessage(response: Response) {
  const contentType = response.headers.get('content-type') ?? ''

  if (response.redirected && response.url.includes('/Login')) {
    return 'Bạn cần đăng nhập để xem dữ liệu.'
  }

  if (!contentType.includes('application/json')) {
    return `API request failed: ${response.status}`
  }

  const data = (await response.json()) as AbpErrorResponse
  const validationMessage = data.error?.validationErrors?.[0]?.message

  return (
    validationMessage ??
    data.error?.message ??
    data.error?.details ??
    `API request failed: ${response.status}`
  )
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
    throw new Error(await readErrorMessage(response))
  }

  return readResponseJson<TResponse>(response)
}