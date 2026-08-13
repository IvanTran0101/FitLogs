import { useRef, useState } from 'react'
import type { FormEvent } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
import { EmptyState } from '../../components/EmptyState'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { NeoInput } from '../../components/NeoInput'
import { NeoSelect } from '../../components/NeoSelect'
import { PageShell } from '../../components/PageShell'
import { BarcodeScanner } from './BarcodeScanner'
import {
  createFoodLog,
  getFoodProducts,
  lookupFoodProductByBarcode,
  type FoodProductDto,
  type FoodProductLookupResultDto,
  type FoodUnit,
  type MealType,
} from '../../api/foodsApi'

const PRODUCT_PAGE_SIZE = 8

type SelectableFoodProduct = FoodProductDto | FoodProductLookupResultDto

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

/** Creates the browser-local date used when no date was supplied by the daily log page. */
function getTodayInputValue() {
  const today = new Date()
  const year = today.getFullYear()
  const month = String(today.getMonth() + 1).padStart(2, '0')
  const day = String(today.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

/** Accepts only a valid date-input value from the daily-log link. */
function getInitialDate(searchParam: string | null) {
  return searchParam && /^\d{4}-\d{2}-\d{2}$/.test(searchParam)
    ? searchParam
    : getTodayInputValue()
}

/** Creates a datetime-local value while keeping the browser's local calendar date visible. */
function getDateTimeLocalValue(dateInputValue: string) {
  const now = new Date()
  const hours = String(now.getHours()).padStart(2, '0')
  const minutes = String(now.getMinutes()).padStart(2, '0')
  return `${dateInputValue}T${hours}:${minutes}`
}

/** Converts the HTML datetime-local value into the backend date-time string without a timezone shift. */
function toApiDateTime(value: string) {
  return value ? `${value}:00` : null
}

/** Reads the shared product id from either a catalog DTO or a barcode lookup DTO. */
function getSelectableProductId(product: SelectableFoodProduct) {
  return 'found' in product ? product.foodProductId : product.id
}

/** Formats nullable nutrition fields while preserving the backend's unknown values. */
function formatProductNutrition(value: number | null, suffix = '') {
  if (value === null) {
    return '—'
  }

  return `${new Intl.NumberFormat('vi-VN', {
    maximumFractionDigits: 1,
  }).format(value)}${suffix}`
}

/** Renders one product consistently for both search results and barcode lookup results. */
function ProductSummary({
  product,
  sourceLabel,
  selected,
  onSelect,
  disabled,
}: {
  product: SelectableFoodProduct
  sourceLabel?: string
  selected?: boolean
  onSelect?: () => void
  disabled?: boolean
}) {
  const isLookupResult = 'found' in product
  const isActive = isLookupResult || product.isActive
  const productName = product.name ?? 'Sản phẩm chưa có tên'
  const productId = isLookupResult ? product.foodProductId : product.id

  return (
    <article className={`food-product-card${selected ? ' selected' : ''}`}>
      <div className="food-product-header">
        <div>
          <p className="eyebrow">{sourceLabel ?? 'Sản phẩm'}</p>
          <h2>{productName}</h2>
        </div>
        {productId ? <span className="food-product-id">Đã nhận diện</span> : null}
      </div>

      <div className="food-product-meta">
        {product.brand ? <span>{product.brand}</span> : null}
        {product.barcode ? <span>Mã {product.barcode}</span> : null}
        {product.servingSize ? <span>Khẩu phần: {product.servingSize}</span> : null}
      </div>

      <div className="food-product-nutrition">
        <span>{formatProductNutrition(product.caloriesPer100g, ' kcal')}</span>
        <span>P {formatProductNutrition(product.proteinPer100g, 'g')}</span>
        <span>C {formatProductNutrition(product.carbPer100g, 'g')}</span>
        <span>F {formatProductNutrition(product.fatPer100g, 'g')}</span>
      </div>

      {onSelect ? (
        <NeoButton
          className="food-product-select"
          disabled={!isActive || disabled}
          onClick={onSelect}
        >
          {selected ? 'Đã chọn' : 'Chọn món này'}
        </NeoButton>
      ) : null}
    </article>
  )
}

/** Coordinates product selection, typed barcode lookup, and creation of a server-backed food log. */
export function FoodAddPage() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const returnDate = searchParams.get('date')
  const [searchText, setSearchText] = useState('')
  const [submittedSearch, setSubmittedSearch] = useState('')
  const [products, setProducts] = useState<FoodProductDto[]>([])
  const [totalProducts, setTotalProducts] = useState(0)
  const [productPage, setProductPage] = useState(0)
  const [isSearching, setIsSearching] = useState(false)
  const [searchError, setSearchError] = useState<string | null>(null)
  const [hasSearched, setHasSearched] = useState(false)
  const [barcode, setBarcode] = useState('')
  const [lookupResult, setLookupResult] = useState<FoodProductLookupResultDto | null>(null)
  const [lookupError, setLookupError] = useState<string | null>(null)
  const [isLookingUp, setIsLookingUp] = useState(false)
  const [isCameraOpen, setIsCameraOpen] = useState(false)
  const [selectedProductId, setSelectedProductId] = useState<string | null>(null)
  const [selectedProduct, setSelectedProduct] = useState<SelectableFoodProduct | null>(null)
  const [quantity, setQuantity] = useState('100')
  const [unit, setUnit] = useState<FoodUnit>(1)
  const [mealType, setMealType] = useState<MealType>(1)
  const [loggedAt, setLoggedAt] = useState(() =>
    getDateTimeLocalValue(getInitialDate(searchParams.get('date'))),
  )
  const [note, setNote] = useState('')
  const [quantityError, setQuantityError] = useState<string | undefined>()
  const [createError, setCreateError] = useState<string | null>(null)
  const [isCreating, setIsCreating] = useState(false)
  const searchRequestId = useRef(0)
  const lookupRequestId = useRef(0)

  // Loads only one backend page so the browser never downloads the complete food catalog.
  async function loadProductPage(queryText: string, page: number) {
    const requestId = searchRequestId.current + 1
    searchRequestId.current = requestId
    setIsSearching(true)
    setSearchError(null)

    try {
      const result = await getFoodProducts({
        FilterText: queryText || undefined,
        OnlyActive: true,
        SkipCount: page * PRODUCT_PAGE_SIZE,
        MaxResultCount: PRODUCT_PAGE_SIZE,
      })

      if (requestId !== searchRequestId.current) {
        return
      }

      setProducts(result.items ?? [])
      setTotalProducts(result.totalCount)
      setProductPage(page)
      setSubmittedSearch(queryText)
      setHasSearched(true)
    } catch (error) {
      if (requestId !== searchRequestId.current) {
        return
      }

      setSearchError(
        error instanceof Error
          ? error.message
          : 'Không thể tìm sản phẩm thực phẩm.',
      )
      setProducts([])
      setTotalProducts(0)
      setHasSearched(true)
    } finally {
      if (requestId === searchRequestId.current) {
        setIsSearching(false)
      }
    }
  }

  // Starts a new search from page one and keeps the query that produced the visible results.
  function handleSearchSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    void loadProductPage(searchText.trim(), 0)
  }

  // Requests a different page using the same submitted query instead of filtering locally.
  function handleProductPageChange(nextPage: number) {
    if (nextPage < 0 || nextPage * PRODUCT_PAGE_SIZE >= totalProducts) {
      return
    }

    void loadProductPage(submittedSearch, nextPage)
  }

  // Sends one normalized barcode to FitLogs and shares the same flow for typed and camera input.
  async function lookupBarcodeValue(value: string) {
    const normalizedBarcode = value.trim()
    if (!normalizedBarcode) {
      setLookupError('Hãy nhập mã vạch trước khi tra cứu.')
      setLookupResult(null)
      return
    }

    const requestId = lookupRequestId.current + 1
    lookupRequestId.current = requestId
    setIsLookingUp(true)
    setLookupError(null)
    setLookupResult(null)
    setSelectedProductId(null)

    try {
      const result = await lookupFoodProductByBarcode(normalizedBarcode)

      if (requestId !== lookupRequestId.current) {
        return
      }

      setLookupResult(result)
    } catch (error) {
      if (requestId !== lookupRequestId.current) {
        return
      }

      setLookupError(
        error instanceof Error
          ? error.message
          : 'Không thể tra cứu mã vạch này.',
      )
    } finally {
      if (requestId === lookupRequestId.current) {
        setIsLookingUp(false)
      }
    }
  }

  // Prevents the browser form event from submitting whitespace or bypassing the shared lookup flow.
  function handleBarcodeSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    void lookupBarcodeValue(barcode)
  }

  // Fills the manual field and immediately sends the decoded camera value to the existing backend endpoint.
  function handleCameraDetected(value: string) {
    setBarcode(value)
    setIsCameraOpen(false)
    void lookupBarcodeValue(value)
  }

  // Submits only backend-supported fields and lets the server calculate the stored nutrition values.
  async function handleCreateFoodLog(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setCreateError(null)
    setQuantityError(undefined)

    if (!selectedProductId) {
      setCreateError('Hãy chọn một sản phẩm trước khi lưu nhật ký.')
      return
    }

    const parsedQuantity = Number(quantity)
    if (!Number.isFinite(parsedQuantity) || parsedQuantity < 0.01 || parsedQuantity > 999999) {
      setQuantityError('Số lượng phải nằm trong khoảng từ 0.01 đến 999999.')
      return
    }

    setIsCreating(true)

    try {
      await createFoodLog({
        foodProductId: selectedProductId,
        quantity: parsedQuantity,
        unit,
        mealType,
        loggedAt: toApiDateTime(loggedAt),
        note: note.trim() || null,
      })

      const selectedDay = loggedAt.slice(0, 10)
      navigate(`/food?date=${selectedDay}`, { replace: true })
    } catch (error) {
      setCreateError(
        error instanceof Error
          ? error.message
          : 'Không thể lưu nhật ký món ăn.',
      )
    } finally {
      setIsCreating(false)
    }
  }

  // Keeps the selected product and its id together so the form cannot submit a stale product reference.
  function selectProduct(product: SelectableFoodProduct) {
    const productId = getSelectableProductId(product)
    if (!productId) {
      return
    }

    setSelectedProduct(product)
    setSelectedProductId(productId)
    setCreateError(null)
  }

  const hasPreviousPage = productPage > 0
  const hasNextPage = (productPage + 1) * PRODUCT_PAGE_SIZE < totalProducts

  return (
    <PageShell title="Thêm món ăn">
      <div className="food-add-stack">
        <Link
          className="back-link"
          to={returnDate ? `/food?date=${encodeURIComponent(returnDate)}` : '/food'}
        >
          ← Quay lại nhật ký
        </Link>

        <NeoCard className="food-add-card food-search-card">
          <p className="eyebrow">Tìm trong kho sản phẩm</p>
          <h2>Tìm món ăn</h2>
          <p className="food-add-help">
            Tìm theo tên, thương hiệu hoặc mã vạch đã có trong hệ thống.
          </p>
          <form className="food-search-form" onSubmit={handleSearchSubmit}>
            <NeoInput
                label="Tên hoặc từ khoá"
                placeholder="Ví dụ: sữa, yến mạch..."
                value={searchText}
                disabled={isSearching || isCreating}
                onChange={(event) => setSearchText(event.target.value)}
              />
            <NeoButton type="submit" disabled={isSearching || isCreating}>
              {isSearching ? 'Đang tìm...' : 'Tìm sản phẩm'}
            </NeoButton>
          </form>
        </NeoCard>

        {isSearching ? <LoadingState message="Đang tìm sản phẩm..." /> : null}
        {!isSearching && searchError ? (
          <ErrorState
            title="Không tìm được sản phẩm"
            message={searchError}
            action={
              <NeoButton
                disabled={isSearching || isCreating}
                onClick={() => void loadProductPage(submittedSearch, productPage)}
              >
                Thử lại
              </NeoButton>
            }
          />
        ) : null}
        {!isSearching && !searchError && hasSearched && products.length === 0 ? (
          <EmptyState
            title="Không có sản phẩm"
            message="Không tìm thấy sản phẩm đang hoạt động với từ khoá này."
          />
        ) : null}
        {!isSearching && !searchError && products.length > 0 ? (
          <section className="food-product-list" aria-label="Kết quả tìm sản phẩm">
            {products.map((product) => (
              <ProductSummary
                key={product.id}
                product={product}
                selected={selectedProductId === product.id}
                onSelect={() => selectProduct(product)}
                disabled={isCreating}
              />
            ))}
            <div className="food-pagination">
              <NeoButton
                disabled={!hasPreviousPage || isSearching}
                onClick={() => handleProductPageChange(productPage - 1)}
              >
                Trước
              </NeoButton>
              <span>
                Trang {productPage + 1} · {totalProducts} sản phẩm
              </span>
              <NeoButton
                disabled={!hasNextPage || isSearching}
                onClick={() => handleProductPageChange(productPage + 1)}
              >
                Sau
              </NeoButton>
            </div>
          </section>
        ) : null}

        <NeoCard className="food-add-card food-barcode-card">
          <p className="eyebrow">Tra cứu nhanh</p>
          <h2>Nhập mã vạch</h2>
          <p className="food-add-help">
            Nhập hoặc dán mã vạch, hoặc mở camera nếu trình duyệt hỗ trợ đọc mã vạch.
          </p>
          <form className="food-search-form" onSubmit={handleBarcodeSubmit}>
            <NeoInput
              label="Mã vạch"
              inputMode="numeric"
              placeholder="Nhập hoặc dán mã vạch"
              value={barcode}
              disabled={isLookingUp || isCreating}
              onChange={(event) => setBarcode(event.target.value)}
              error={lookupError && !isLookingUp ? lookupError : undefined}
            />
            <NeoButton type="submit" disabled={isLookingUp || isCreating}>
              {isLookingUp ? 'Đang tra cứu...' : 'Tra cứu mã vạch'}
            </NeoButton>
            <NeoButton
              type="button"
              className="camera-button"
              disabled={isLookingUp || isCameraOpen || isCreating}
              onClick={() => {
                setLookupError(null)
                setIsCameraOpen(true)
              }}
            >
              Mở camera
            </NeoButton>
          </form>
        </NeoCard>

        {isCameraOpen ? (
          <BarcodeScanner
            onDetected={handleCameraDetected}
            onClose={() => setIsCameraOpen(false)}
          />
        ) : null}

        {isLookingUp ? <LoadingState message="Đang tra cứu mã vạch..." /> : null}
        {!isLookingUp && lookupError ? (
          <ErrorState
            title="Tra cứu thất bại"
            message={lookupError}
            action={
              <NeoButton
                disabled={isLookingUp || isCreating || !barcode.trim()}
                onClick={() => void lookupBarcodeValue(barcode)}
              >
                Thử lại
              </NeoButton>
            }
          />
        ) : null}
        {!isLookingUp && !lookupError && lookupResult && !lookupResult.found ? (
          <EmptyState
            title="Không tìm thấy mã vạch"
            message="FitLogs không tìm thấy sản phẩm cho mã vạch này."
          />
        ) : null}
        {!isLookingUp && !lookupError && lookupResult?.found ? (
          <ProductSummary
            product={lookupResult}
            sourceLabel={lookupResult.fromCache ? 'Đã có trong hệ thống' : 'Tìm từ Open Food Facts'}
            selected={selectedProductId === lookupResult.foodProductId}
            disabled={isCreating}
            onSelect={
              lookupResult.foodProductId
                ? () => selectProduct(lookupResult)
                : undefined
            }
          />
        ) : null}

        {selectedProductId && selectedProduct ? (
          <NeoCard className="food-add-card food-log-form-card">
            <p className="eyebrow">Đã chọn sản phẩm</p>
            <h2>{selectedProduct.name ?? 'Sản phẩm thực phẩm'}</h2>
            <p className="food-add-help">
              Nhập thông tin nhật ký. Calories và macro sau khi lưu sẽ lấy từ phản hồi máy chủ.
            </p>
            <form className="food-log-form" onSubmit={handleCreateFoodLog} aria-busy={isCreating}>
              <NeoInput
                label="Số lượng"
                type="number"
                min="0.01"
                max="999999"
                step="0.01"
                value={quantity}
                disabled={isCreating}
                onChange={(event) => setQuantity(event.target.value)}
                error={quantityError}
              />
              <div className="food-form-grid">
                <NeoSelect
                  label="Đơn vị"
                  value={String(unit)}
                  disabled={isCreating}
                  onChange={(event) => setUnit(Number(event.target.value) as FoodUnit)}
                  options={FOOD_UNIT_OPTIONS}
                />
                <NeoSelect
                  label="Bữa ăn"
                  value={String(mealType)}
                  disabled={isCreating}
                  onChange={(event) => setMealType(Number(event.target.value) as MealType)}
                  options={MEAL_TYPE_OPTIONS}
                />
              </div>
              <NeoInput
                label="Thời điểm"
                type="datetime-local"
                value={loggedAt}
                disabled={isCreating}
                onChange={(event) => setLoggedAt(event.target.value)}
              />
              <NeoInput
                label="Ghi chú (tuỳ chọn)"
                maxLength={512}
                value={note}
                disabled={isCreating}
                onChange={(event) => setNote(event.target.value)}
              />
              <NeoButton type="submit" disabled={isCreating}>
                {isCreating ? 'Đang lưu...' : 'Lưu nhật ký món ăn'}
              </NeoButton>
            </form>
            {createError ? (
              <ErrorState title="Không thể lưu nhật ký" message={createError} />
            ) : null}
          </NeoCard>
        ) : null}
      </div>
    </PageShell>
  )
}
