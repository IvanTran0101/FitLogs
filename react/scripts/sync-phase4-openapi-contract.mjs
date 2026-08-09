import fs from 'node:fs'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

const scriptDirectory = path.dirname(fileURLToPath(import.meta.url))
const openApiPath = path.resolve(scriptDirectory, '../src/api/openapi.json')
const document = JSON.parse(fs.readFileSync(openApiPath, 'utf8'))
const schemas = document.components.schemas

// This keeps the checked-in reference aligned until the live Swagger document is refreshed from the host.
delete schemas['FitLogs.Workouts.CreateWorkoutPlanDto'].properties.isActive
delete schemas['FitLogs.Workouts.UpdateWorkoutPlanDto'].properties.isActive
schemas['FitLogs.Workouts.CreateWorkoutSessionDto'].required = []
schemas['FitLogs.Workouts.CreateWorkoutSessionDto'].properties.name.nullable = true

schemas['FitLogs.Workouts.StartWorkoutFromPlanDto'] = {
  required: ['workoutPlanId'],
  type: 'object',
  properties: {
    workoutPlanId: { type: 'string', format: 'uuid' },
    startedAt: { type: 'string', format: 'date-time', nullable: true },
    note: { maxLength: 500, minLength: 0, type: 'string', nullable: true },
  },
  additionalProperties: false,
}

schemas['FitLogs.Workouts.StartFreeWorkoutDto'] = {
  required: ['name'],
  type: 'object',
  properties: {
    name: { maxLength: 100, minLength: 0, type: 'string' },
    startedAt: { type: 'string', format: 'date-time', nullable: true },
    note: { maxLength: 500, minLength: 0, type: 'string', nullable: true },
  },
  additionalProperties: false,
}

schemas['FitLogs.Workouts.AddWorkoutPlanExercisesDto'] = {
  required: ['exercises'],
  type: 'object',
  properties: {
    exercises: {
      type: 'array',
      items: { $ref: '#/components/schemas/FitLogs.Workouts.CreateWorkoutPlanExerciseDto' },
      minItems: 1,
    },
  },
  additionalProperties: false,
}

const response = ref => ({
  '200': {
    description: 'OK',
    content: {
      'application/json': { schema: { $ref: `#/components/schemas/${ref}` } },
    },
  },
})

const request = ref => ({
  required: true,
  content: {
    'application/json': { schema: { $ref: `#/components/schemas/${ref}` } },
  },
})

document.paths['/api/app/workout-session/start-from-plan'] = {
  post: {
    tags: ['WorkoutSession'],
    operationId: 'WorkoutSession_StartFromPlanAsync',
    requestBody: request('FitLogs.Workouts.StartWorkoutFromPlanDto'),
    responses: response('FitLogs.Workouts.WorkoutSessionDto'),
  },
}

document.paths['/api/app/workout-session/start-free-workout'] = {
  post: {
    tags: ['WorkoutSession'],
    operationId: 'WorkoutSession_StartFreeWorkoutAsync',
    requestBody: request('FitLogs.Workouts.StartFreeWorkoutDto'),
    responses: response('FitLogs.Workouts.WorkoutSessionDto'),
  },
}

document.paths['/api/app/workout-plan/{id}/exercises'] = {
  post: {
    tags: ['WorkoutPlan'],
    operationId: 'WorkoutPlan_AddExercisesAsync',
    parameters: [{ name: 'id', in: 'path', required: true, schema: { type: 'string', format: 'uuid' } }],
    requestBody: request('FitLogs.Workouts.AddWorkoutPlanExercisesDto'),
    responses: response('FitLogs.Workouts.WorkoutPlanDto'),
  },
}

for (const [route, operationId] of [
  ['/api/app/workout-plan/{id}/activate', 'WorkoutPlan_ActivateAsync'],
  ['/api/app/workout-plan/{id}/deactivate', 'WorkoutPlan_DeactivateAsync'],
]) {
  document.paths[route] = {
    post: {
      tags: ['WorkoutPlan'],
      operationId,
      parameters: [{ name: 'id', in: 'path', required: true, schema: { type: 'string', format: 'uuid' } }],
      responses: response('FitLogs.Workouts.WorkoutPlanDto'),
    },
  }
}

fs.writeFileSync(openApiPath, `${JSON.stringify(document, null, 2)}\n`)
console.log('Updated checked-in OpenAPI contract reference.')
