import { PageShell } from '../../components/PageShell'
import { NeoCard } from '../../components/NeoCard'
import { NeoButton } from '../../components/NeoButton'

export function WorkoutPage() {
  return (
    <PageShell title="Buổi tập">
      <NeoCard className="placeholder-card">
        <p className="eyebrow">Active Workout</p>
        <h2>Bench Press</h2>
        <p>Current exercise, set table, next/skip controls sẽ nằm ở đây.</p>
        <NeoButton>Thêm set</NeoButton>
      </NeoCard>
    </PageShell>
  )
}