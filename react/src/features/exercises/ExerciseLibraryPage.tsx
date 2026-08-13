import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { EmptyState } from '../../components/EmptyState'
import { NeoCard } from '../../components/NeoCard'
import { NeoButton } from '../../components/NeoButton'
import { NeoInput } from '../../components/NeoInput'
import { NeoSelect } from '../../components/NeoSelect'
import { PageShell } from '../../components/PageShell'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import {
  getEquipments,
  getExercises,
  getMuscleGroups,
  type EquipmentDto,
  type ExerciseDto,
  type MuscleGroupDto,
} from '../../api/exercisesApi'
import { formatDifficulty, getNameById } from './exerciseFormatters'



export function ExerciseLibraryPage() {

  const [exercises, setExercises] = useState<ExerciseDto[]>([])
  const [muscleGroups, setMuscleGroups] = useState<MuscleGroupDto[]>([])
  const [equipments, setEquipments] = useState<EquipmentDto[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [reloadToken, setReloadToken] = useState(0)

  useEffect(() => {
  async function loadExercises() {
    try {
      setIsLoading(true)
      setErrorMessage(null)

      const [exerciseResult, muscleGroupResult, equipmentResult] = await Promise.all([
        getExercises({
          IsActive: true,
          MaxResultCount: 100,
        }),
        getMuscleGroups(),
        getEquipments(),
      ])

      setExercises(exerciseResult.items ?? [])
      setMuscleGroups(muscleGroupResult.items ?? [])
      setEquipments(equipmentResult.items ?? [])
    } catch (error) {
      setErrorMessage(error instanceof Error ? error.message : 'Không thể tải bài tập.')
    } finally {
      setIsLoading(false)
    }
  }

  void loadExercises()
}, [reloadToken])

  // Re-runs the same catalog request after a transient backend or network failure.
  function retryLoad() {
    setReloadToken((currentToken) => currentToken + 1)
  }
  
    const [filterText, setFilterText] = useState('')
    const [muscleGroupFilter, setMuscleGroupFilter] = useState('')
    const [equipmentFilter, setEquipmentFilter] = useState('')
    
    const normalizedFilterText = filterText.trim().toLowerCase()
    const hasActiveFilters =
      normalizedFilterText.length > 0 ||
      muscleGroupFilter.length > 0 ||
      equipmentFilter.length > 0

    const filteredExercises = exercises.filter((exercise) => {
      const matchesText =
        normalizedFilterText.length === 0 ||
        (exercise.name ?? '').toLowerCase().includes(normalizedFilterText)

      const matchesMuscleGroup =
        muscleGroupFilter.length === 0 ||
        exercise.primaryMuscleGroupId === muscleGroupFilter

      const matchesEquipment =
        equipmentFilter.length === 0 ||
        exercise.equipmentId === equipmentFilter

      return matchesText && matchesMuscleGroup && matchesEquipment
    })
  return (
    <PageShell title="Thư viện bài tập">
      <section className="exercise-filter-panel">
        <NeoInput
            label="Tìm bài tập"
            placeholder="Bench press, squat..."
            value={filterText}
            onChange={(event) => setFilterText(event.target.value)}
            />

        <div className="exercise-filter-grid">
          <NeoSelect
            label="Nhóm cơ"
            value={muscleGroupFilter}
            onChange={(event) => setMuscleGroupFilter(event.target.value)}
            options={[
              { label: 'Tất cả', value: '' },
              ...muscleGroups.map((muscleGroup) => ({
                label: muscleGroup.name ?? 'Không tên',
                value: muscleGroup.id,
              })),
            ]}
          />

          <NeoSelect
            label="Thiết bị"
            value={equipmentFilter}
            onChange={(event) => setEquipmentFilter(event.target.value)}
            options={[
              { label: 'Tất cả', value: '' },
              ...equipments.map((equipment) => ({
                label: equipment.name ?? 'Không tên',
                value: equipment.id,
              })),
            ]}
          />
        </div>
      </section>

      {isLoading ? (
        <LoadingState message="Đang tải thư viện bài tập..." />
      ) : errorMessage ? (
        <ErrorState
          message={errorMessage}
          action={<NeoButton onClick={retryLoad}>Thử lại</NeoButton>}
        />
      ) : filteredExercises.length === 0 ? (
        <EmptyState
          title={hasActiveFilters ? 'Không tìm thấy bài' : 'Chưa có bài tập'}
          message={
            hasActiveFilters
              ? 'Thử đổi từ khoá, nhóm cơ hoặc thiết bị.'
              : 'Backend đang trả danh sách rỗng. Hãy thêm hoặc seed dữ liệu bài tập.'
          }
        />
        ) : (
        <section className="exercise-list">
            {filteredExercises.map((exercise) => (
            <Link
                key={exercise.id}
                className="exercise-card-link"
                to={`/exercises/${exercise.slug ?? exercise.id}`}
            >
                <NeoCard className="exercise-card">
                <div className="exercise-thumb">
                    ▣
                </div>

                <div className="exercise-card-content">
                    <div className="exercise-card-header">
                    <h2>{exercise.name ?? 'Bài tập chưa đặt tên'}</h2>
                    <span className="exercise-status">
                        {exercise.isActive ? 'Active' : 'Inactive'}
                    </span>
                    </div>

                    <div className="exercise-tags">
                    <span>{getNameById(muscleGroups, exercise.primaryMuscleGroupId)}</span>
                    <span>{getNameById(equipments, exercise.equipmentId)}</span>
                    <span>{formatDifficulty(exercise.difficulty)}</span>
                    </div>
                </div>
                </NeoCard>
            </Link>
            ))}
        </section>
        )}
    </PageShell>
  )
}
