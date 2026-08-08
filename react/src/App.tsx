import {ExercisePickerPage} from './features/exercises/ExercisePickerPage'
import { Navigate, Route, Routes } from 'react-router-dom'
import { DashboardPage } from './features/dashboard/DashboardPage'
import { ExerciseDetailPage } from './features/exercises/ExerciseDetailPage'
import { ExerciseLibraryPage } from './features/exercises/ExerciseLibraryPage'
import { FoodLogPage } from './features/foodLogs/FoodLogPage'
import { FoodAddPage } from './features/foodLogs/FoodAddPage'
import { FoodLogEditPage } from './features/foodLogs/FoodLogEditPage'
import { ProfilePage } from './features/userProfile/ProfilePage'
import { WorkoutPage } from './features/workoutSessions/WorkoutPage'
import { WorkoutPlansPage } from './features/workoutPlans/WorkoutPlansPage'
import { WorkoutPlanDetailPage } from './features/workoutPlans/WorkoutPlanDetailPage'
import { WorkoutPlanEditorPage } from './features/workoutPlans/WorkoutPlanEditorPage'
import { WorkoutPlanExerciseEditorPage } from './features/workoutPlans/WorkoutPlanExerciseEditorPage'
import { AuthCallbackPage } from './auth/AuthCallbackPage'
import { AuthLogoutCallbackPage } from './auth/AuthLogoutCallbackPage'
function App() {
  return (
    <Routes>
      <Route path="/" element={<DashboardPage />} />
      <Route path="/food" element={<FoodLogPage />} />
      <Route path="/food/add" element={<FoodAddPage />} />
      <Route path="/food/logs/:foodLogId/edit" element={<FoodLogEditPage />} />
      <Route path="/workout" element={<WorkoutPage />} />
      <Route path="/plans" element={<WorkoutPlansPage />} />
      <Route path="/profile" element={<ProfilePage />} />
      <Route path="/exercises" element={<ExerciseLibraryPage />} />
      <Route path="/exercises/:exerciseId" element={<ExerciseDetailPage />} />
      <Route path="/exercise-picker" element={<ExercisePickerPage />} />
      <Route path="/plans/new" element={<WorkoutPlanEditorPage />} />
      <Route path="/plans/:planId" element={<WorkoutPlanDetailPage />} />
      <Route path="/plans/:planId/edit" element={<WorkoutPlanEditorPage />} />
      <Route path="/plans/:planId/add-exercises" element={<ExercisePickerPage />} />
      <Route
        path="/plans/:planId/exercises/:workoutPlanExerciseId/edit"
        element={<WorkoutPlanExerciseEditorPage />}
      />
      <Route path="/auth/callback" element={<AuthCallbackPage />} />
      <Route path="/auth/logout-callback" element={<AuthLogoutCallbackPage />} />
      <Route path="*" element={<Navigate to="/" replace />} />

    </Routes>
  )
}

export default App
