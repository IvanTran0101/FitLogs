import { Link } from 'react-router-dom'
import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { PageShell } from '../../components/PageShell'

export function WorkoutPage() {
  return (
    <PageShell title="Buổi tập">
      <div className="training-action-grid">
        <NeoCard className="training-action-card blue">
          <p className="eyebrow">Active Workout</p>
          <h2>Buổi tập hiện tại</h2>
          <p>Current exercise, set table, next/skip controls sẽ nằm ở đây.</p>
          <NeoButton>Bắt đầu tập</NeoButton>
        </NeoCard>

        <NeoCard className="training-action-card lime">
          <p className="eyebrow">Exercise Library</p>
          <h2>Thư viện bài tập</h2>
          <p>Tìm bài theo nhóm cơ, thiết bị và thêm vào buổi tập hoặc kế hoạch.</p>

          <Link className="neo-button link-button" to="/exercises">
            Mở thư viện
          </Link>
          
          <Link className="neo-button link-button secondary-link-button" to="/exercise-picker">
            Mở picker
          </Link>
        </NeoCard>
      </div>
    </PageShell>
  )
}