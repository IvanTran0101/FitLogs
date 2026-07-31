import { Navigate, Route, Routes } from 'react-router-dom'
import { DashboardPage } from './features/dashboard/DashboardPage'
import { FoodLogPage } from './features/foodLogs/FoodLogPage'
import { WorkoutPage } from './features/workoutSessions/WorkoutPage'
import { WorkoutPlansPage } from './features/workoutPlans/WorkoutPlansPage'
import { ProfilePage } from './features/userProfile/ProfilePage'

function App() {
  return (
    <Routes>
      <Route path="/" element={<DashboardPage />} />
      <Route path="/food" element={<FoodLogPage />} />
      <Route path="/workout" element={<WorkoutPage />} />
      <Route path="/plans" element={<WorkoutPlansPage />} />
      <Route path="/profile" element={<ProfilePage />} />

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}

export default App