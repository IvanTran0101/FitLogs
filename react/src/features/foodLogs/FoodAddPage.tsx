import { useRef, useState } from 'react'
import type { FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { EmptyState } from '../../components/EmptyState'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { NeoInput } from '../../components/NeoInput'
import { PageShell } from '../../components/PageShell'
import {
  getFoodProducts,
  lookupFoodProductByBarcode,
  type FoodProductDto,
  type FoodProductLookupResultDto,
} from '../../api/foodsApi'

const PRODUCT_PAGE_SIZE = 8

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
}: {
  product: FoodProductDto | FoodProductLookupResultDto
  sourceLabel?: string
  selected?: boolean
  onSelect?: () => void
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
          disabled={!isActive}
          onClick={onSelect}
        >
          {selected ? 'Đã chọn' : 'Chọn món này'}
        </NeoButton>
      ) : null}
    </article>
  )
}

/** Provides paged catalog search and typed barcode lookup before the log form is introduced. */
export function FoodAddPage() {
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
  const [selectedProductId, setSelectedProductId] = useState<string | null>(null)
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

  // Sends only the typed barcode to FitLogs; camera input is intentionally deferred to a later phase.
  async function handleBarcodeSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const normalizedBarcode = barcode.trim()

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

  const hasPreviousPage = productPage > 0
  const hasNextPage = (productPage + 1) * PRODUCT_PAGE_SIZE < totalProducts

  return (
    <PageShell title="Thêm món ăn">
      <div className="food-add-stack">
        <Link className="back-link" to="/food">
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
              onChange={(event) => setSearchText(event.target.value)}
            />
            <NeoButton type="submit" disabled={isSearching}>
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
              <NeoButton onClick={() => void loadProductPage(submittedSearch, productPage)}>
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
                onSelect={() => setSelectedProductId(product.id)}
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
            Bản đầu tiên nhận mã vạch được nhập hoặc dán thủ công. Camera sẽ được thêm ở phase sau.
          </p>
          <form className="food-search-form" onSubmit={handleBarcodeSubmit}>
            <NeoInput
              label="Mã vạch"
              inputMode="numeric"
              placeholder="Nhập hoặc dán mã vạch"
              value={barcode}
              onChange={(event) => setBarcode(event.target.value)}
              error={lookupError && !isLookingUp ? lookupError : undefined}
            />
            <NeoButton type="submit" disabled={isLookingUp}>
              {isLookingUp ? 'Đang tra cứu...' : 'Tra cứu mã vạch'}
            </NeoButton>
          </form>
        </NeoCard>

        {isLookingUp ? <LoadingState message="Đang tra cứu mã vạch..." /> : null}
        {!isLookingUp && lookupError ? (
          <ErrorState title="Tra cứu thất bại" message={lookupError} />
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
            onSelect={
              lookupResult.foodProductId
                ? () => setSelectedProductId(lookupResult.foodProductId)
                : undefined
            }
          />
        ) : null}

        {selectedProductId ? (
          <NeoCard className="food-selection-note">
            <strong>Đã chọn sản phẩm.</strong>
            <span>Biểu mẫu số lượng và bữa ăn sẽ được bổ sung ở phase tiếp theo.</span>
          </NeoCard>
        ) : null}
      </div>
    </PageShell>
  )
}
