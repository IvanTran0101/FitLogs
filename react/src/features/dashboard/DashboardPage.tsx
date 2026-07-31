import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { PageShell } from '../../components/PageShell'

export function DashboardPage() {
  return (
    <PageShell title="Hôm nay">
      <NeoCard className="calorie-card">
        <div>
          <p className="eyebrow">Calo</p>
          <strong>1.890</strong>
          <span>/ 2.400 kcal</span>
        </div>

        <div className="progress-ring">
          79%
        </div>
      </NeoCard>

      <section className="macro-grid">
        <article className="macro-card protein">
          <span>Protein</span>
          <strong>142g</strong>
          <small>/ 160g</small>
        </article>

        <article className="macro-card carb">
          <span>Carb</span>
          <strong>210g</strong>
          <small>/ 260g</small>
        </article>

        <article className="macro-card fat">
          <span>Fat</span>
          <strong>58g</strong>
          <small>/ 70g</small>
        </article>
      </section>

      <NeoCard className="workout-card">
        <div>
          <p className="eyebrow">Buổi tập hôm nay</p>
          <h2>Push Day</h2>
          <span>Ngực · Vai · Tay sau</span>
        </div>

        <NeoButton>
          Bắt đầu
        </NeoButton>
      </NeoCard>

      <section className="stats-grid">
        <article className="stat-card blue">
          <span>Streak</span>
          <strong>12</strong>
          <small>ngày</small>
        </article>

        <article className="stat-card lime">
          <span>Tiến độ</span>
          <strong>78%</strong>
          <small>tuần này</small>
        </article>

        <article className="stat-card yellow">
          <span>Cân nặng</span>
          <strong>72.4</strong>
          <small>kg</small>
        </article>
      </section>
    </PageShell>
  )
}