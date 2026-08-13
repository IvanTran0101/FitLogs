import { UserManager, WebStorageStateStore, type User } from 'oidc-client-ts'

const authority = import.meta.env.VITE_AUTHORITY
const clientId = import.meta.env.VITE_OIDC_CLIENT_ID
const redirectUri = import.meta.env.VITE_OIDC_REDIRECT_URI
const postLogoutRedirectUri = import.meta.env.VITE_OIDC_POST_LOGOUT_REDIRECT_URI
const scope = import.meta.env.VITE_OIDC_SCOPE
// Reuses the registered login callback unless a separately registered silent callback is provided.
const silentRedirectUri = import.meta.env.VITE_OIDC_SILENT_REDIRECT_URI || redirectUri

export const userManager = new UserManager({
  authority,
  client_id: clientId,
  redirect_uri: redirectUri,
  silent_redirect_uri: silentRedirectUri,
  post_logout_redirect_uri: postLogoutRedirectUri,
  response_type: 'code',
  scope,
  // oidc-client-ts renews with a refresh token when available and otherwise falls back to prompt=none in an iframe.
  automaticSilentRenew: true,
  userStore: new WebStorageStateStore({
    store: window.localStorage,
  }),
})

let renewalPromise: Promise<User | null> | null = null

// Starts OIDC login and optionally carries the protected path to the callback for safe return navigation.
export function login(returnUrl?: string) {
  return userManager.signinRedirect({
    state: returnUrl ? { returnUrl } : undefined,
  })
}

export function logout() {
  return userManager.signoutRedirect()
}

// Removes the locally stored OIDC user after an unauthorized API response so protected routes can react.
export function clearUserSession() {
  return userManager.removeUser()
}

/** Renews the current OIDC user once at a time so concurrent API calls do not start duplicate token requests. */
export async function renewUserSession(): Promise<User | null> {
  const currentUser = await getCurrentUser()
  if (!currentUser) {
    return null
  }

  if (!renewalPromise) {
    renewalPromise = userManager
      .signinSilent()
      .catch(() => null)
      .finally(() => {
        renewalPromise = null
      })
  }

  return renewalPromise
}

export function handleLoginCallback() {
  // signinCallback dispatches normal redirects and silent-renew callbacks using the stored OIDC request type.
  return userManager.signinCallback()
}

export function handleLogoutCallback() {
  return userManager.signoutRedirectCallback()
}

export async function getCurrentUser(): Promise<User | null> {
  return userManager.getUser()
}

export async function getAccessToken() {
  let user = await getCurrentUser()

  if (user?.expired) {
    user = await renewUserSession()
  }

  if (!user || user.expired) {
    return null
  }

  return user.access_token
}
