import { apiRequest } from './httpClient'

export type GrantedPolicies = Record<string, boolean>

type ApplicationConfigurationDto = {
  auth?: {
    grantedPolicies?: GrantedPolicies | null
  } | null
}

/** Loads ABP's server-calculated permission decisions for the signed-in user. */
export async function getGrantedPolicies(): Promise<GrantedPolicies> {
  const configuration = await apiRequest<ApplicationConfigurationDto>(
    '/api/abp/application-configuration',
  )

  return configuration.auth?.grantedPolicies ?? {}
}
