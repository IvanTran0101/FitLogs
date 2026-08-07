import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { NeoInput } from '../../components/NeoInput'
import { NeoSelect } from '../../components/NeoSelect'
import { PageShell } from '../../components/PageShell'
import { login, logout } from '../../auth/authService'
export function ProfilePage() {
  return (
    <PageShell title="Hồ sơ">
      <NeoCard className="profile-form-card">
        <p className="eyebrow">Mục tiêu cá nhân</p>

        <div className="form-grid">
          <NeoInput
            label="Calories"
            type="number"
            placeholder="2500"
          />

          <NeoInput
            label="Protein"
            type="number"
            placeholder="150"
          />

          <NeoInput
            label="Chiều cao"
            type="number"
            placeholder="175"
          />

          <NeoInput
            label="Cân nặng"
            type="number"
            placeholder="70.0"
          />

          <NeoInput
            label="Ngày sinh"
            type="date"
          />

          <NeoSelect
            label="Mức độ hoạt động"
            defaultValue="moderate"
            options={[
              { label: 'Ít vận động', value: 'low' },
              { label: 'Trung bình', value: 'moderate' },
              { label: 'Năng động', value: 'high' },
            ]}
          />
        </div>

        <NeoButton className="full-width-button">
          Lưu thay đổi
        </NeoButton>
        <NeoButton onClick={login}>Đăng nhập</NeoButton>
        <NeoButton onClick={logout}>Đăng xuất</NeoButton>   
      </NeoCard>
    </PageShell>
  )
}