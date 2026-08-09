import fs from 'node:fs'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

const scriptDirectory = path.dirname(fileURLToPath(import.meta.url))
const openApiPath = path.resolve(scriptDirectory, '../src/api/openapi.json')
const document = JSON.parse(fs.readFileSync(openApiPath, 'utf8'))
const schemas = document.components.schemas

schemas['FitLogs.UserProfiles.UpdateUserProfileDto'].required = ['displayName', 'timeZoneId']
schemas['FitLogs.UserProfiles.UpdateUserProfileDto'].properties.timeZoneId = {
  maxLength: 64,
  minLength: 1,
  type: 'string',
}
schemas['FitLogs.UserProfiles.UserProfileDto'].properties.timeZoneId = {
  type: 'string',
}

fs.writeFileSync(openApiPath, `${JSON.stringify(document, null, 2)}\n`)
console.log('Updated checked-in OpenAPI dashboard time-zone contract.')
