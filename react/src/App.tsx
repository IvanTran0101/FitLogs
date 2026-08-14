import { useEffect } from 'react'
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
import { ProtectedRoute } from './auth/ProtectedRoute'
import { getCurrentUser } from './auth/authService'
import { getMyProfile, updateMyProfile } from './api/userProfileApi'

function App() {
  useEffect(() => {
    let cancelled = false

    // Synchronize the browser's IANA timezone so backend local-day queries match the user's device.
    async function synchronizeTimeZone() {
      const browserTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone
      if (!browserTimeZone) {
        return
      }

      try {
        if (!await getCurrentUser()) {
          return
        }

        const profile = await getMyProfile()
        if (cancelled || profile.timeZoneId === browserTimeZone) {
          return
        }

        await updateMyProfile({ ...profile, timeZoneId: browserTimeZone })
      } catch {
        // Time-zone synchronization is opportunistic; dashboard requests still use the stored default if it fails.
      }
    }

    void synchronizeTimeZone()
    return () => {
      cancelled = true
    }
  }, [])

  return (
    <Routes>
      <Route path="/" element={<ProtectedRoute><DashboardPage /></ProtectedRoute>} />
      <Route path="/food" element={<ProtectedRoute><FoodLogPage /></ProtectedRoute>} />
      <Route path="/food/add" element={<ProtectedRoute><FoodAddPage /></ProtectedRoute>} />
      <Route path="/food/logs/:foodLogId/edit" element={<ProtectedRoute><FoodLogEditPage /></ProtectedRoute>} />
      <Route path="/workout" element={<ProtectedRoute><WorkoutPage /></ProtectedRoute>} />
      <Route path="/plans" element={<ProtectedRoute><WorkoutPlansPage /></ProtectedRoute>} />
      <Route path="/profile" element={<ProtectedRoute><ProfilePage /></ProtectedRoute>} />
      <Route path="/exercises" element={<ExerciseLibraryPage />} />
      <Route path="/exercises/:exerciseId" element={<ExerciseDetailPage />} />
      <Route path="/exercise-picker" element={<ProtectedRoute><ExercisePickerPage /></ProtectedRoute>} />
      <Route path="/plans/new" element={<ProtectedRoute><WorkoutPlanEditorPage /></ProtectedRoute>} />
      <Route path="/plans/:planId" element={<ProtectedRoute><WorkoutPlanDetailPage /></ProtectedRoute>} />
      <Route path="/plans/:planId/edit" element={<ProtectedRoute><WorkoutPlanEditorPage /></ProtectedRoute>} />
      <Route path="/plans/:planId/add-exercises" element={<ProtectedRoute><ExercisePickerPage /></ProtectedRoute>} />
      <Route
        path="/plans/:planId/exercises/:workoutPlanExerciseId/edit"
        element={<ProtectedRoute><WorkoutPlanExerciseEditorPage /></ProtectedRoute>}
      />
      <Route path="/auth/callback" element={<AuthCallbackPage />} />
      <Route path="/auth/silent-callback" element={<AuthCallbackPage />} />
      <Route path="/auth/logout-callback" element={<AuthLogoutCallbackPage />} />
      <Route path="*" element={<Navigate to="/" replace />} />

    </Routes>
  )
}

export default App
