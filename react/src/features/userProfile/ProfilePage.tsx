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
            label="Tên hiển thị"
            type="text"
            placeholder="Tên của bạn"
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
            label="Mục tiêu"
            defaultValue="4"
            options={[
              { label: 'Giảm cân', value: '1' },
              { label: 'Duy trì cân nặng', value: '2' },
              { label: 'Tăng cơ', value: '3' },
              { label: 'Cải thiện thể lực', value: '4' },
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
