import { useEffect, useState } from 'react'
import {
  getEquipments,
  getMuscleGroups,
  getSelectableExercises,
  type EquipmentDto,
  type ExerciseDto,
  type MuscleGroupDto,
} from '../../api/exercisesApi'
import { EmptyState } from '../../components/EmptyState'
import { ErrorState } from '../../components/ErrorState'
import { LoadingState } from '../../components/LoadingState'
import { NeoButton } from '../../components/NeoButton'
import { NeoCard } from '../../components/NeoCard'
import { NeoInput } from '../../components/NeoInput'
import { NeoSelect } from '../../components/NeoSelect'
import { PageShell } from '../../components/PageShell'
import { getNameById } from './exerciseFormatters'
import { useNavigate, useParams } from 'react-router-dom'
import {
  addWorkoutPlanExercise,
  getWorkoutPlan,
} from '../../api/workoutPlansApi'

type SelectedExerciseTarget = {
  defaultSets: string
  defaultReps: string
  defaultWeightKg: string
  restSeconds: string
  note: string
}

function createDefaultExerciseTarget(): SelectedExerciseTarget {
  return {
    defaultSets: '3',
    defaultReps: '10',
    defaultWeightKg: '',
    restSeconds: '90',
    note: '',
  }
}


export function ExercisePickerPage() {
    const [selectedExerciseTargets, setSelectedExerciseTargets] = useState<
      Record<string, SelectedExerciseTarget>
    >({})
    const selectedExerciseIds = Object.keys(selectedExerciseTargets)
    const [exercises, setExercises] = useState<ExerciseDto[]>([])
    const [muscleGroups, setMuscleGroups] = useState<MuscleGroupDto[]>([])
    const [equipments, setEquipments] = useState<EquipmentDto[]>([])

  const { planId } = useParams()
  const navigate = useNavigate()
  const [isSubmitting, setIsSubmitting] = useState(false)

  
  async function handleAddExercisesToPlan() {
  if (!planId) {
    console.log('Selected exercise ids:', selectedExerciseIds)
    return
  }

  if (selectedExerciseIds.length === 0) {
    return
  }

  try {
    setIsSubmitting(true)
    setErrorMessage(null)

    const plan = await getWorkoutPlan(planId)
    const existingExercisesCount = plan.exercises?.length ?? 0

    for (const [index, exerciseId] of selectedExerciseIds.entries()) {
       const target = selectedExerciseTargets[exerciseId]

        await addWorkoutPlanExercise(planId, {
          exerciseId,
          orderIndex: existingExercisesCount + index + 1,
          defaultSets: Number(target.defaultSets),
          defaultReps: Number(target.defaultReps),
          defaultWeightKg:
            target.defaultWeightKg.trim().length === 0
              ? null
              : Number(target.defaultWeightKg),
          restSeconds:
            target.restSeconds.trim().length === 0
              ? null
              : Number(target.restSeconds),
          note: target.note.trim().length === 0 ? null : target.note.trim(),
        })
    }

    navigate(`/plans/${planId}`)
  } catch (error) {
    setErrorMessage(
      error instanceof Error ? error.message : 'Không thể thêm bài tập vào plan.',
    )
  } finally {
    setIsSubmitting(false)
  }
}
    useEffect(() => {
      async function loadPickerData() {
        try {
          setIsLoading(true)
          setErrorMessage(null)

          const [exerciseResult, muscleGroupResult, equipmentResult] = await Promise.all([
            getSelectableExercises({
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
          setExercises([])
          setErrorMessage(
            error instanceof Error ? error.message : 'Không thể tải danh sách bài tập.',
          )
        } finally {
          setIsLoading(false)
        }
      }

      void loadPickerData()
    }, [])

    const [isLoading, setIsLoading] = useState(true)
    const [errorMessage, setErrorMessage] = useState<string | null>(null)
    const [filterText, setFilterText] = useState('')
    const [muscleGroupFilter, setMuscleGroupFilter] = useState('')
    const [equipmentFilter, setEquipmentFilter] = useState('')
    function toggleExercise(exerciseId: string) {
      const isSelected = exerciseId in selectedExerciseTargets

      if (isSelected) {
        const nextTargets = { ...selectedExerciseTargets }
        delete nextTargets[exerciseId]

        setSelectedExerciseTargets(nextTargets)
        return
      }

      setSelectedExerciseTargets({
        ...selectedExerciseTargets,
        [exerciseId]: createDefaultExerciseTarget(),
      })
    }

    function updateExerciseTarget(
      exerciseId: string,
      field: keyof SelectedExerciseTarget,
      value: string,
    ) {
      setSelectedExerciseTargets({
        ...selectedExerciseTargets,
        [exerciseId]: {
          ...selectedExerciseTargets[exerciseId],
          [field]: value,
        },
      })
    }
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
    <PageShell title="Chọn bài tập">
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
          <LoadingState message="Đang tải danh sách bài tập..." />
        ) : errorMessage ? (
          <ErrorState message={errorMessage} />
        ) : filteredExercises.length === 0 ? (
          <EmptyState
            title={hasActiveFilters ? 'Không tìm thấy bài' : 'Chưa có bài tập để chọn'}
            message={
              hasActiveFilters
                ? 'Thử đổi từ khoá, nhóm cơ hoặc thiết bị.'
                : 'Backend đang trả danh sách rỗng. Hãy thêm hoặc seed dữ liệu bài tập.'
            }
          />
        ) : (
        <section className="picker-list">
            {filteredExercises.map((exercise) => {
            const isSelected = selectedExerciseIds.includes(exercise.id)

            return (
                <NeoCard
                key={exercise.id}
                className={isSelected ? 'picker-card selected' : 'picker-card'}
                >
                <h2>{exercise.name ?? 'Bài tập chưa đặt tên'}</h2>
                <div className="exercise-tags">
                  <span>{getNameById(muscleGroups, exercise.primaryMuscleGroupId)}</span>
                  <span>{getNameById(equipments, exercise.equipmentId)}</span>
                </div>

                <button
                    className={isSelected ? 'picker-toggle selected' : 'picker-toggle'}
                    type="button"
                    onClick={() => toggleExercise(exercise.id)}
                    aria-label={isSelected ? 'Bỏ chọn bài tập' : 'Chọn bài tập'}
                >
                    {isSelected ? '✓' : '+'}
                </button>
                {isSelected ? (
                  <div className="picker-target-form">
                    <NeoInput
                      label="Sets"
                      type="number"
                      min={1}
                      value={selectedExerciseTargets[exercise.id].defaultSets}
                      onChange={(event) =>
                        updateExerciseTarget(exercise.id, 'defaultSets', event.target.value)
                      }
                    />

                    <NeoInput
                      label="Reps"
                      type="number"
                      min={1}
                      value={selectedExerciseTargets[exercise.id].defaultReps}
                      onChange={(event) =>
                        updateExerciseTarget(exercise.id, 'defaultReps', event.target.value)
                      }
                    />

                    <NeoInput
                      label="Kg"
                      type="number"
                      min={0}
                      placeholder="Tuỳ chọn"
                      value={selectedExerciseTargets[exercise.id].defaultWeightKg}
                      onChange={(event) =>
                        updateExerciseTarget(exercise.id, 'defaultWeightKg', event.target.value)
                      }
                    />

                    <NeoInput
                      label="Nghỉ"
                      type="number"
                      min={0}
                      placeholder="Giây"
                      value={selectedExerciseTargets[exercise.id].restSeconds}
                      onChange={(event) =>
                        updateExerciseTarget(exercise.id, 'restSeconds', event.target.value)
                      }
                    />

                    <NeoInput
                      label="Ghi chú"
                      placeholder="Tempo, form cue..."
                      value={selectedExerciseTargets[exercise.id].note}
                      onChange={(event) =>
                        updateExerciseTarget(exercise.id, 'note', event.target.value)
                      }
                    />
                  </div>
                ) : null}
                </NeoCard>
            )
            })}
        </section>
        )}

      
      <div className="picker-footer">
        <NeoButton
          className="full-width-button"
          disabled={selectedExerciseIds.length === 0 || isSubmitting}
          onClick={handleAddExercisesToPlan}
        >
          {isSubmitting
            ? 'Đang thêm...'
            : planId
              ? `Thêm ${selectedExerciseIds.length} bài`
              : `Đã chọn ${selectedExerciseIds.length} bài`}
        </NeoButton>
      </div>
    </PageShell>
  )
}