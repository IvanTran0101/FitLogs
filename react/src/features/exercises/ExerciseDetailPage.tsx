import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import {
  getEquipments,
  getExerciseBySlug,
  getMuscleGroups,
  type EquipmentDto,
  type ExerciseDto,
  type MuscleGroupDto,
} from '../../api/exercisesApi'
import { EmptyState } from '../../components/EmptyState'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoCard } from '../../components/NeoCard'
import { PageShell } from '../../components/PageShell'
import {
  formatDifficulty,
  formatTrackingType,
  getNameById,
} from './exerciseFormatters'



export function ExerciseDetailPage() {
  const { exerciseId } = useParams()
const [exercise, setExercise] = useState<ExerciseDto | null>(null)
const [muscleGroups, setMuscleGroups] = useState<MuscleGroupDto[]>([])
const [equipments, setEquipments] = useState<EquipmentDto[]>([])
const [isLoading, setIsLoading] = useState(true)
const [errorMessage, setErrorMessage] = useState<string | null>(null)

useEffect(() => {
  async function loadExerciseDetail() {
    if (!exerciseId) {
      setIsLoading(false)
      setErrorMessage('Thiếu slug bài tập.')
      return
    }

    try {
      setIsLoading(true)
      setErrorMessage(null)

      const [exerciseResult, muscleGroupResult, equipmentResult] = await Promise.all([
        getExerciseBySlug(exerciseId),
        getMuscleGroups(),
        getEquipments(),
      ])

      setExercise(exerciseResult)
      setMuscleGroups(muscleGroupResult.items ?? [])
      setEquipments(equipmentResult.items ?? [])
    } catch (error) {
      setExercise(null)
      setErrorMessage(
        error instanceof Error ? error.message : 'Không thể tải chi tiết bài tập.',
      )
    } finally {
      setIsLoading(false)
    }
  }

  void loadExerciseDetail()
}, [exerciseId])

if (isLoading) {
  return (
    <PageShell title="Chi tiết bài tập">
      <LoadingState message="Đang tải chi tiết bài tập..." />
    </PageShell>
  )
}

if (errorMessage) {
  return (
    <PageShell title="Chi tiết bài tập">
      <ErrorState message={errorMessage} />
    </PageShell>
  )
}

if (!exercise) {
  return (
    <PageShell title="Chi tiết bài tập">
      <EmptyState
        title="Không tìm thấy bài tập"
        message="Bài tập này không tồn tại hoặc đã bị xoá."
      />

      <Link className="neo-button link-button" to="/exercises">
        Quay lại thư viện
      </Link>
    </PageShell>
  )
}

  const mediaUrl = exercise.gifUrl || exercise.imageUrl
  const exerciseName = exercise.name ?? 'Bài tập chưa đặt tên'
  const exerciseSlug = exercise.slug ?? exercise.id
  const muscleGroupName = getNameById(muscleGroups, exercise.primaryMuscleGroupId)
  const equipmentName = getNameById(equipments, exercise.equipmentId)

  return (
    <PageShell title="Chi tiết bài tập">
      <NeoCard className="exercise-detail-card">
        <Link className="back-link" to="/exercises">
          ← Quay lại thư viện
        </Link>
        <div className="exercise-detail-header">
          <div>
            <p className="eyebrow">Exercise</p>
            <h2>{exerciseName}</h2>
            <span className="exercise-slug">/{exerciseSlug}</span>
          </div>

          <span className={exercise.isActive ? 'exercise-status' : 'exercise-status inactive'}>
            {exercise.isActive ? 'Active' : 'Inactive'}
          </span>
        </div>

        <div className="exercise-detail-hero">
          {mediaUrl ? (
            <img src={mediaUrl} alt={exerciseName} />
          ) : (
            <span>▣</span>
          )}
        </div>

        <div className="exercise-tags">
          <span>{muscleGroupName}</span>
          <span>{equipmentName}</span>
          <span>{formatDifficulty(exercise.difficulty)}</span>
          <span>{formatTrackingType(exercise.trackingType)}</span>
        </div>

        <section className="exercise-detail-section">
          <h3>Mô tả</h3>
          <p>{exercise.description || 'Chưa có mô tả.'}</p>
        </section>

        <section className="exercise-detail-section">
          <h3>Hướng dẫn</h3>
          <p>{exercise.instructions || 'Chưa có hướng dẫn.'}</p>
        </section>

        <section className="exercise-detail-section">
          <h3>Mẹo form</h3>
          <p>{exercise.formTips || 'Chưa có mẹo form.'}</p>
        </section>

        <section className="exercise-detail-section">
          <h3>Lỗi thường gặp</h3>
          <p>{exercise.commonMistakes || 'Chưa có dữ liệu lỗi thường gặp.'}</p>
        </section>

        <div className="exercise-detail-actions">
          <Link className="neo-button link-button" to="/workout">
            Thêm vào buổi tập
          </Link>

          <Link className="neo-button link-button secondary-link-button" to="/plans">
            Chọn kế hoạch để thêm bài
          </Link>
        </div>
      </NeoCard>
    </PageShell>
  )
}