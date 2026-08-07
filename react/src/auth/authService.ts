import { UserManager, WebStorageStateStore, type User } from 'oidc-client-ts'

const authority = import.meta.env.VITE_AUTHORITY
const clientId = import.meta.env.VITE_OIDC_CLIENT_ID
const redirectUri = import.meta.env.VITE_OIDC_REDIRECT_URI
const postLogoutRedirectUri = import.meta.env.VITE_OIDC_POST_LOGOUT_REDIRECT_URI
const scope = import.meta.env.VITE_OIDC_SCOPE

export const userManager = new UserManager({
  authority,
  client_id: clientId,
  redirect_uri: redirectUri,
  post_logout_redirect_uri: postLogoutRedirectUri,
  response_type: 'code',
  scope,
  userStore: new WebStorageStateStore({
    store: window.localStorage,
  }),
})

export function login() {
  return userManager.signinRedirect()
}

export function logout() {
  return userManager.signoutRedirect()
}

export function handleLoginCallback() {
  return userManager.signinRedirectCallback()
}

export function handleLogoutCallback() {
  return userManager.signoutRedirectCallback()
}

export async function getCurrentUser(): Promise<User | null> {
  return userManager.getUser()
}

export async function getAccessToken() {
  const user = await getCurrentUser()

  if (!user || user.expired) {
    return null
  }

  return user.access_token
}