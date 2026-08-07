import type { ExerciseDto } from '../../api/exercisesApi'

export function getNameById<TItem extends { id: string; name: string | null }>(
  items: TItem[],
  id: string,
) {
  return items.find((item) => item.id === id)?.name ?? 'Không rõ'
}

export function formatDifficulty(difficulty: ExerciseDto['difficulty']) {
  if (difficulty === 1) {
    return 'Dễ'
  }

  if (difficulty === 2) {
    return 'Trung bình'
  }

  return 'Nâng cao'
}

export function formatTrackingType(trackingType: ExerciseDto['trackingType']) {
  if (trackingType === 1) {
    return 'Reps & Weight'
  }

  if (trackingType === 2) {
    return 'Reps only'
  }

  if (trackingType === 3) {
    return 'Duration'
  }

  return 'Distance & Duration'
}