import { PageShell } from '../../components/PageShell'
import { NeoCard } from '../../components/NeoCard'
import { NeoButton } from '../../components/NeoButton'

export function FoodLogPage() {
  return (
    <PageShell title="Ăn uống">
      <NeoCard className="placeholder-card">
        <p className="eyebrow">Bữa ăn hôm nay</p>
        <h2>Food Log</h2>
        <p>Danh sách bữa ăn, món ăn và tổng calories sẽ nằm ở đây.</p>
        <NeoButton>Thêm món</NeoButton>
      </NeoCard>
    </PageShell>
  )
}