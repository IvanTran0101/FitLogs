import fs from 'node:fs'
import path from 'node:path'
import { fileURLToPath } from 'node:url'

const scriptDirectory = path.dirname(fileURLToPath(import.meta.url))
const openApiPath = path.resolve(scriptDirectory, '../src/api/openapi.json')
const document = JSON.parse(fs.readFileSync(openApiPath, 'utf8'))
const schemas = document.components.schemas

schemas['FitLogs.Foods.FoodProductDataQuality'] = {
  enum: [0, 1, 2],
  type: 'integer',
  format: 'int32',
}

for (const schemaName of [
  'FitLogs.Foods.FoodProducts.CreateUpdateFoodProductDto',
  'FitLogs.Foods.FoodProducts.FoodProductDto',
  'FitLogs.Foods.FoodProducts.FoodProductLookupResultDto',
]) {
  const schema = schemas[schemaName]
  schema.properties.caloriesPer100g.nullable = true
}

for (const schemaName of [
  'FitLogs.Foods.FoodProducts.FoodProductDto',
  'FitLogs.Foods.FoodProducts.FoodProductLookupResultDto',
]) {
  schemas[schemaName].properties.dataQuality = {
    $ref: '#/components/schemas/FitLogs.Foods.FoodProductDataQuality',
  }
}

fs.writeFileSync(openApiPath, `${JSON.stringify(document, null, 2)}\n`)
console.log('Updated checked-in OpenAPI food-product reliability contract.')
