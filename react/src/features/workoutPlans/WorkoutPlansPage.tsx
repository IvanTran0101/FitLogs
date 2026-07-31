import { PageShell } from '../../components/PageShell'
import { NeoCard } from '../../components/NeoCard'
import { NeoButton } from '../../components/NeoButton'

export function WorkoutPlansPage() {
  return (
    <PageShell title="Kế hoạch">
      <NeoCard className="placeholder-card">
        <p className="eyebrow">Workout Plans</p>
        <h2>Push Day</h2>
        <p>Danh sách kế hoạch tập và plan editor sẽ nằm ở đây.</p>
        <NeoButton>Tạo kế hoạch</NeoButton>
      </NeoCard>
    </PageShell>
  )
}