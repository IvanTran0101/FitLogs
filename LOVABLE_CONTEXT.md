# LOVABLE_CONTEXT.md

This document describes the current React frontend in this repository so another AI tool can continue development without rebuilding or replacing the existing work.

Repository root: `/Volumes/Kingsman/Projects/Personal/FitLogs/FitLogs`
Frontend root: `/Volumes/Kingsman/Projects/Personal/FitLogs/FitLogs/react`
Backend: existing ASP.NET Core + ABP Framework application, treated as the source of truth for API contracts, authentication, authorization, validation, business logic, and persistence.

---

# 1. Project Overview

FitLogs is a fitness tracking application. The existing frontend is a mobile-first React app for interacting with an ABP backend.

Current frontend role:

- Display dashboard-style fitness summaries.
- Browse exercise library data from the backend.
- Manage workout plans through backend REST APIs.
- Start building UI flows for workout sessions, food logs, and user profile/settings.
- Handle OIDC login/logout against the ABP backend.

Current frontend maturity:

- Exercise Library and Workout Plan flows are the most developed.
- Authentication wiring exists but has no app-wide auth context or route guard yet.
- Food Log, Dashboard, and Workout Session pages are mostly placeholders or static UI; Profile now loads and saves real backend data.
- API integration is handwritten, not generated.
- The app uses a consistent Neo-Brutalist visual style with shared components and a mobile shell.

Major user flows currently visible:

- Browse exercises.
- View exercise detail by slug/id route param.
- List workout plans.
- Create/edit workout plan.
- View workout plan detail.
- Add exercises to a workout plan with per-exercise target inputs.
- Remove exercises from a workout plan.
- Reorder workout plan exercises using up/down buttons.
- Edit an exercise target inside a workout plan.
- Login/logout through ABP/OpenIddict OIDC redirect flow.

Relationship to backend:

- The frontend calls ABP REST endpoints under `/api/app/...`.
- Backend DTOs and behavior are the source of truth.
- Frontend types are handwritten from observed/OpenAPI contracts and should be kept aligned with backend Swagger/OpenAPI.
- Do not implement backend business rules in React except minimal UX validation.

---

# 2. Technology Stack

Actual technologies present in `/react/package.json`:

- `react` `^19.2.7`: UI rendering.
- `react-dom` `^19.2.7`: browser DOM rendering via `createRoot`.
- `react-router-dom` `^7.18.2`: client-side routing with `BrowserRouter`, `Routes`, `Route`, `Link`, `NavLink`, `useParams`, `useNavigate`.
- `vite` `^8.1.1`: dev server and production build.
- `typescript` `~6.0.2`: TypeScript compilation through `tsc -b`.
- `oidc-client-ts` `^3.5.0`: OIDC authorization code flow against ABP/OpenIddict.
- `oxlint` `^1.71.0`: linting via `npm run lint`.
- `@vitejs/plugin-react` `^6.0.3`: Vite React plugin.

Not present:

- No Tailwind CSS.
- No Redux/Zustand/Jotai/MobX.
- No React Query/TanStack Query/SWR.
- No Axios.
- No form library such as React Hook Form/Formik.
- No validation library such as Zod/Yup.
- No UI component library.
- No icon library; icons are plain text/emoji/symbols.
- No frontend test framework currently configured.

Build scripts:

- `npm run dev`: Vite dev server.
- `npm run build`: `tsc -b && vite build`.
- `npm run lint`: Oxlint.
- `npm run preview`: Vite preview.

---

# 3. Project Structure

Important structure under `/react/src`:

```text
src/
├── App.tsx
├── main.tsx
├── api/
│   ├── config.ts
│   ├── httpClient.ts
│   ├── exercisesApi.ts
│   ├── workoutPlansApi.ts
│   └── openapi.json
├── auth/
│   ├── authService.ts
│   ├── AuthCallbackPage.tsx
│   └── AuthLogoutCallbackPage.tsx
├── components/
│   ├── BottomNav.tsx
│   ├── EmptyState.tsx
│   ├── ErrorState.tsx
│   ├── LoadingState.tsx
│   ├── NeoButton.tsx
│   ├── NeoCard.tsx
│   ├── NeoInput.tsx
│   ├── NeoSelect.tsx
│   └── PageShell.tsx
├── features/
│   ├── dashboard/
│   ├── exercises/
│   ├── foodLogs/
│   ├── userProfile/
│   ├── workoutPlans/
│   └── workoutSessions/
└── styles/
    └── global.css
```

Responsibilities:

- `main.tsx`: mounts React, imports global CSS, wraps app in `BrowserRouter`.
- `App.tsx`: declares all client-side routes.
- `api/`: handwritten API client and DTO typings. `openapi.json` is a downloaded Swagger/OpenAPI file, not generated TypeScript code.
- `auth/`: OIDC client setup and callback pages.
- `components/`: shared UI primitives and layout components.
- `features/`: feature/page-level components. Organization is feature-based.
- `styles/global.css`: all app CSS, design tokens, layout, page-specific classes, and Neo-Brutalist styling.

There are no `hooks/`, `contexts/`, `layouts/`, `providers/`, `types/`, or generated API client directories at this time.

---

# 4. Application Architecture

Current architecture is simple and page-driven:

```text
BrowserRouter
  ↓
App.tsx Routes
  ↓
Feature Page Component
  ↓
Shared UI Components + local useState/useEffect
  ↓
Handwritten API Service Module
  ↓
apiRequest() fetch wrapper
  ↓
ABP REST API
```

Details:

- Pages own their local UI state with `useState`.
- Server data is loaded directly inside page components via `useEffect`.
- Mutations are called directly from event handlers inside page components.
- There is no shared server-state cache.
- There is no global auth provider yet.
- Shared UI components are mostly presentational wrappers around HTML elements.
- `PageShell` provides the mobile app frame, header, page title area, scrollable content, and bottom nav.
- API logic is centralized enough to avoid raw `fetch` in feature pages.

Architecture style:

- Feature-based page folders with a small shared component layer.
- No complex abstraction yet; this is intentional and should be preserved unless there is a concrete need.

---

# 5. Routing

Routes are declared in `/react/src/App.tsx`.

| Route | Page | Auth Required | Status | Notes |
| --- | --- | --- | --- | --- |
| `/` | `DashboardPage` | Not enforced | Partial | Static/mock dashboard data. |
| `/food` | `FoodLogPage` | Not enforced | Partial | Placeholder UI only. |
| `/workout` | `WorkoutPage` | Not enforced | Partial | Static cards linking to exercise library/picker. Workout session integration not implemented. |
| `/plans` | `WorkoutPlansPage` | Backend-enforced only | Implemented/Partial | Loads workout plans from API, handles loading/error/empty. |
| `/profile` | `ProfilePage` | Not enforced | Partial | Static profile form plus login/logout buttons. No profile API integration. |
| `/exercises` | `ExerciseLibraryPage` | Backend-enforced only | Implemented | Loads exercises, muscle groups, equipment from API and filters locally. |
| `/exercises/:exerciseId` | `ExerciseDetailPage` | Backend-enforced only | Implemented/Partial | Uses `getExerciseBySlug(exerciseId)`. Detail actions mostly links/placeholders. |
| `/exercise-picker` | `ExercisePickerPage` | Not enforced | Partial | Standalone picker logs selection to console when no `planId`. |
| `/plans/new` | `WorkoutPlanEditorPage` | Backend-enforced only | Implemented/Partial | Create plan form and submit. |
| `/plans/:planId` | `WorkoutPlanDetailPage` | Backend-enforced only | Implemented/Partial | Loads plan detail, shows exercises, remove/reorder actions. |
| `/plans/:planId/edit` | `WorkoutPlanEditorPage` | Backend-enforced only | Implemented/Partial | Edit plan form and submit. |
| `/plans/:planId/add-exercises` | `ExercisePickerPage` | Backend-enforced only | Implemented/Partial | Adds selected exercises to plan with target inputs. |
| `/plans/:planId/exercises/:workoutPlanExerciseId/edit` | `WorkoutPlanExerciseEditorPage` | Backend-enforced only | Implemented/Partial | Edits target sets/reps/kg/rest/note/orderIndex. |
| `/auth/callback` | `AuthCallbackPage` | Public callback | Implemented/Partial | Completes OIDC login and navigates home. |
| `/auth/logout-callback` | `AuthLogoutCallbackPage` | Public callback | Implemented/Partial | Completes OIDC logout and navigates home. |
| `*` | `Navigate to /` | Public | Implemented | Catch-all redirect. |

Route guards:

- No frontend route guard component exists.
- Auth/authorization is currently enforced by backend responses.
- Routes are public from React's perspective.

Layouts:

- Each page manually renders `PageShell`.
- There is no nested layout route structure.

---

# 6. Authentication

Authentication is implemented in `/react/src/auth/authService.ts` using `oidc-client-ts`.

Configuration comes from Vite env variables in `/react/.env.local`:

```text
VITE_API_BASE_URL=https://localhost:44377
VITE_AUTHORITY=https://localhost:44377
VITE_OIDC_CLIENT_ID=FitLogs_App
VITE_OIDC_REDIRECT_URI=http://localhost:5173/auth/callback
VITE_OIDC_POST_LOGOUT_REDIRECT_URI=http://localhost:5173/auth/logout-callback
VITE_OIDC_SCOPE=openid profile email roles FitLogs
```

Login flow:

- `ProfilePage` has a `NeoButton` that calls `login()`.
- `login()` calls `userManager.signinRedirect()`.
- ABP/OpenIddict handles the login UI server-side.
- After login, browser returns to `/auth/callback`.
- `AuthCallbackPage` calls `handleLoginCallback()` and navigates to `/`.

Logout flow:

- `ProfilePage` has a `NeoButton` that calls `logout()`.
- `logout()` calls `userManager.signoutRedirect()`.
- `/auth/logout-callback` calls `handleLogoutCallback()` and navigates to `/`.

Access token handling:

- OIDC user is stored in `window.localStorage` using `WebStorageStateStore`.
- `getAccessToken()` calls `userManager.getUser()` and returns `user.access_token` if present and not expired.
- `apiRequest()` attaches `Authorization: Bearer <token>` when a token exists.

Cookies/credentials:

- `apiRequest()` uses `credentials: 'include'` for fetch.
- This was likely added for ABP/CORS/cookie interop during development.

Refresh token handling:

- No explicit refresh-token or silent-renew implementation is present.
- If the stored user is expired, `getAccessToken()` returns `null`.

Authentication context/provider:

- No React auth context exists.
- No `useAuth` hook exists.
- Components do not subscribe to login state.

Protected routes:

- No protected-route implementation exists.
- Unauthorized API calls show backend/API error messages through page-level `ErrorState` where implemented.

HTTP 401/login handling:

- `httpClient.ts` detects redirects to `/Login` and returns `Bạn cần đăng nhập để xem dữ liệu.`.
- For JSON ABP errors, it extracts validation or error messages.
- There is no automatic login redirect on 401.

Current limitations:

- Login/logout buttons are only on `ProfilePage`.
- No user display name, account menu, or auth state UI.
- No route-level auth protection.
- No token refresh/silent renew configured.
- No frontend permission checks.

---

# 7. Authorization / Permissions

Frontend authorization is currently minimal.

Implemented:

- The frontend sends bearer tokens when available.
- Backend ABP authorization decides whether requests are allowed.

Not implemented:

- No frontend permission API calls.
- No ABP permission names are imported or checked in React.
- No role checks.
- No route-level permission guard.
- No component-level permission guard.

Important rule:

- Do not bypass or duplicate backend authorization. Frontend may add UX-level hiding/disabling later, but backend remains authoritative.

---

# 8. API Integration

API base URL:

- `/react/src/api/config.ts` reads `VITE_API_BASE_URL`.
- `.env.local` currently sets `https://localhost:44377`.
- If missing, config throws `Missing VITE_API_BASE_URL environment variable.`.

API client:

- `/react/src/api/httpClient.ts` exports `apiRequest<TResponse>()`.
- Uses native `fetch`, not Axios.
- Supports methods: `GET`, `POST`, `PUT`, `DELETE`.
- Supports query objects with primitive query values.
- Skips empty/null/undefined query params.
- Adds `Accept: application/json`.
- Adds `Content-Type: application/json` when body exists.
- Adds `Authorization: Bearer <token>` when OIDC token exists.
- Uses `credentials: 'include'`.
- Parses `204 No Content` as `undefined`.
- Parses JSON only when response content-type includes `application/json`.
- Extracts ABP error shape from `error.message`, `error.details`, and first `validationErrors` item.

OpenAPI:

- `/react/src/api/openapi.json` exists and is a downloaded Swagger/OpenAPI JSON file.
- It is not currently used to generate TypeScript clients.
- Handwritten API modules should be checked against this file or live Swagger before adding new endpoints/types.

Important API modules:

| Frontend Service | Backend Area | Main Responsibilities |
| --- | --- | --- |
| `exercisesApi.ts` | Exercises, Muscle Groups, Equipment | Exercise list/detail/selectable list and reference data. |
| `workoutPlansApi.ts` | Workout Plans | Plan list/detail/create/update/delete/archive/restore, add/update/remove/reorder plan exercises. |
| `httpClient.ts` | Shared HTTP | Fetch wrapper, auth header, JSON parsing, ABP error handling. |
| `config.ts` | Environment config | Reads backend base URL. |

Actual endpoint paths used by frontend:

Exercise/reference:

- `GET /api/app/exercise/{id}`
- `GET /api/app/exercise/by-slug`
- `GET /api/app/exercise`
- `GET /api/app/exercise/selectable-list`
- `GET /api/app/muscle-group`
- `GET /api/app/equipment`

Workout plan:

- `GET /api/app/workout-plan`
- `GET /api/app/workout-plan/{id}`
- `POST /api/app/workout-plan`
- `PUT /api/app/workout-plan/{id}`
- `DELETE /api/app/workout-plan/{id}`
- `POST /api/app/workout-plan/{id}/archive`
- `POST /api/app/workout-plan/{id}/restore`
- `POST /api/app/workout-plan/{id}/exercise`
- `PUT /api/app/workout-plan/{id}/exercise/{workoutPlanExerciseId}`
- `DELETE /api/app/workout-plan/{id}/exercise/{workoutPlanExerciseId}`
- `POST /api/app/workout-plan/{id}/reorder-exercises`

Endpoint paths present in OpenAPI but not integrated in frontend yet include food log, food product, user profile, and workout session endpoints.

Pagination conventions:

- `PagedResult<TItem>` is handwritten with `items: TItem[] | null` and `totalCount: number`.
- Query DTOs use ABP-style names like `FilterText`, `SkipCount`, `MaxResultCount`, `Sorting`, `IsActive`.
- Current pages usually request `MaxResultCount: 100` or `200` and then filter locally.

DTO typing:

- Handwritten in `exercisesApi.ts` and `workoutPlansApi.ts`.
- Not generated from OpenAPI.
- Should be updated only after checking backend Swagger/OpenAPI.

---

# 9. Data Models and TypeScript Types

Types are handwritten and duplicated from backend/OpenAPI understanding.

Exercise domain source file: `/react/src/api/exercisesApi.ts`

Important types:

- `PagedResult<TItem>`: ABP paged result shape used by current API modules.
- `ExerciseDifficulty = 1 | 2 | 3`.
- `ExerciseTrackingType = 1 | 2 | 3 | 4`.
- `ExerciseDto`: id, name, slug, description, primaryMuscleGroupId, equipmentId, difficulty, trackingType, image/gif URLs, instructions, formTips, commonMistakes, isActive.
- `MuscleGroupDto`: id, name, code, description, displayOrder, isActive.
- `EquipmentDto`: id, name, code, description, displayOrder, isActive.
- `GetExerciseListQuery`: ABP-style filter/paging/sorting fields.
- `GetReferenceListQuery`: filter/paging/sorting for reference lists.

Workout plan domain source file: `/react/src/api/workoutPlansApi.ts`

Important types:

- `WorkoutGoal = 0 | 1 | 2 | 3 | 4`.
- `WorkoutDifficulty = 0 | 1 | 2`.
- `WorkoutPlanExerciseDto`: id, workoutPlanId, exerciseId, orderIndex, defaultSets, defaultReps, defaultWeightKg, restSeconds, note.
- `WorkoutPlanDto`: ABP audit fields, userId, name, description, goal, difficulty, isActive, isArchived, exercises.
- `CreateWorkoutPlanDto` / `UpdateWorkoutPlanDto`.
- `CreateWorkoutPlanExerciseDto`.
- `UpdateWorkoutPlanExerciseDto`.
- `ReorderWorkoutPlanExercisesDto` and item DTO.

Form-local types:

- `WorkoutPlanFormState`, `WorkoutPlanFormErrors` in `WorkoutPlanEditorPage.tsx`.
- `TargetFormState`, `TargetFormErrors` in `WorkoutPlanExerciseEditorPage.tsx`.
- `SelectedExerciseTarget` in `ExercisePickerPage.tsx`.

OpenAPI contains additional backend schemas not yet typed in frontend:

- Food logs/products.
- User profile.
- Workout sessions and exercise sets.

---

# 10. Current Features

## Dashboard

Status:

- Partial.

Existing functionality:

- Static card UI for calories, macros, workout, and stats.

Important components:

- `DashboardPage`.
- `PageShell`, `NeoCard`, `NeoButton`.

Backend integration:

- None.

Known limitations:

- Data is static/hardcoded.
- No dashboard API module.

Possible next frontend tasks:

- Define dashboard requirements from backend APIs.
- Replace hardcoded data with real food/profile/workout data when APIs are ready.

## Exercise Library

Status:

- Implemented, with limitations.

Existing functionality:

- Loads active exercises from API.
- Loads muscle groups and equipment reference lists.
- Local text, muscle group, and equipment filtering.
- Loading, error, and empty states.
- Links each exercise to detail route by `slug ?? id`.

Important components:

- `ExerciseLibraryPage`.
- `exerciseFormatters.tsx`.
- Shared `NeoInput`, `NeoSelect`, `NeoCard`, states.

Important services:

- `getExercises()`.
- `getMuscleGroups()`.
- `getEquipments()`.

Backend integration:

- Real API integration exists.

Known limitations:

- Filtering is local after fetching up to 100 items.
- No pagination UI.
- No create/edit/admin exercise management UI.

Possible next frontend tasks:

- Add server-side paging/search if data grows.
- Improve detail actions for adding to plan/session.

## Exercise Detail

Status:

- Implemented/Partial.

Existing functionality:

- Loads exercise by slug route param using `getExerciseBySlug`.
- Loads muscle group/equipment names.
- Displays media if `gifUrl` or `imageUrl` exists.
- Displays description, instructions, form tips, common mistakes, difficulty, tracking type.
- Loading/error/empty states.

Important components:

- `ExerciseDetailPage`.

Backend integration:

- Real API integration exists for read-only detail.

Known limitations:

- `Thêm vào buổi tập` links to `/workout`, not an actual add-to-session action.
- `Chọn kế hoạch để thêm bài` links to `/plans`, not a direct plan picker from detail.

Possible next frontend tasks:

- Add choose-plan flow from exercise detail.
- Add active workout/session integration later.

## Exercise Picker

Status:

- Implemented/Partial.

Existing functionality:

- Loads selectable exercises and reference lists.
- Supports local search/filter by text, muscle group, equipment.
- Supports selecting multiple exercises.
- When used under `/plans/:planId/add-exercises`, opens per-selected-exercise target form for sets, reps, kg, rest seconds, note.
- Adds selected exercises to a workout plan via API.
- Navigates back to plan detail after submit.
- Shows fixed footer submit button.

Important components:

- `ExercisePickerPage`.

Important services:

- `getSelectableExercises()`.
- `getWorkoutPlan()`.
- `addWorkoutPlanExercise()`.

Backend integration:

- Real integration for adding exercises to plan when `planId` exists.

Known limitations:

- Standalone `/exercise-picker` has no real destination; it logs selected IDs to console.
- Minimal validation only via input `min`; no explicit frontend error messages before submit for picker target fields.
- Adds exercises sequentially in a loop; no bulk API currently used.

Possible next frontend tasks:

- Remove or repurpose standalone picker route.
- Add validation messages for target inputs.
- Reuse picker for workout session add flow once session APIs are integrated.

## Workout Plans List

Status:

- Implemented/Partial.

Existing functionality:

- Loads non-archived plans from backend.
- Displays plan cards with active status, description, goal, difficulty, exercise count.
- Handles loading/error/empty.
- Links to plan detail.

Important components:

- `WorkoutPlansPage`.

Important services:

- `getWorkoutPlans()`.

Backend integration:

- Real API integration exists.

Known limitations:

- No list filtering UI yet.
- No pagination UI.
- No archive/restore UI from list.

Possible next frontend tasks:

- Add filter/search for plans.
- Add archive/restore/delete controls if desired.

## Workout Plan Detail

Status:

- Implemented/Partial.

Existing functionality:

- Loads plan detail and selectable exercise catalog.
- Shows plan metadata.
- Hides edit/add controls when plan is archived.
- Shows empty state for no exercises.
- Lists exercises in `orderIndex` order.
- Shows target sets/reps/kg/rest/note.
- Allows remove from plan with `window.confirm`.
- Allows reorder with up/down buttons.

Important components:

- `WorkoutPlanDetailPage`.

Important services:

- `getWorkoutPlan()`.
- `getSelectableExercises()`.
- `removeWorkoutPlanExercise()`.
- `reorderWorkoutPlanExercises()`.

Backend integration:

- Real API integration exists.

Known limitations:

- Exercise names are resolved by fetching selectable exercise catalog and matching by `exerciseId`; if catalog misses an exercise, UI shows `Không rõ`.
- No direct link/button to edit target is currently visible in the rendered plan exercise card, despite route/page existing.
- Archive/delete/restore actions are available in API layer but not surfaced here.
- Reorder behavior depends on backend support for safe unique-index reordering.

Possible next frontend tasks:

- Add edit target button to each plan exercise.
- Improve archived plan UX.
- Add archive/delete/restore controls.

## Workout Plan Editor

Status:

- Implemented/Partial.

Existing functionality:

- Create and edit plan using one page.
- Loads existing plan in edit mode.
- Frontend validates non-empty name.
- Submits create/update API.
- Navigates to saved plan detail.
- Shows backend errors.

Important components:

- `WorkoutPlanEditorPage`.

Important services:

- `createWorkoutPlan()`.
- `updateWorkoutPlan()`.
- `getWorkoutPlan()`.

Backend integration:

- Real API integration exists.

Known limitations:

- Only basic name validation in frontend.
- Does not explicitly prevent editing archived plan on this page if user navigates directly; backend should still enforce.

Possible next frontend tasks:

- Load plan and disable form when archived if backend contract requires that UX.
- Add clearer server validation display if multiple validation errors are returned.

## Workout Plan Exercise Target Editor

Status:

- Implemented/Partial.

Existing functionality:

- Loads plan and target exercise by `workoutPlanExerciseId`.
- Loads selectable exercise catalog to resolve exercise name.
- Edits orderIndex, defaultSets, defaultReps, defaultWeightKg, restSeconds, note.
- Validates sets/reps positive and kg/rest non-negative.
- Submits update API and navigates back to plan detail.

Important components:

- `WorkoutPlanExerciseEditorPage`.

Important services:

- `getWorkoutPlan()`.
- `getSelectableExercises()`.
- `updateWorkoutPlanExercise()`.

Backend integration:

- Real API integration exists.

Known limitations:

- Route exists but current plan detail UI does not expose a visible edit target link/button.
- Editing `orderIndex` here may overlap with separate reorder UX.

Possible next frontend tasks:

- Add edit target action from plan detail.
- Consider whether order editing should be removed from this form and kept only in reorder controls.

## Workout Sessions

Status:

- Not implemented / Placeholder.

Existing functionality:

- `WorkoutPage` shows static cards.
- Links to exercise library and standalone picker.

Backend integration:

- None in frontend.
- OpenAPI contains workout session endpoints, but no `workoutSessionsApi.ts` exists.

Known limitations:

- No active session page.
- No create/start session API calls.
- No current exercise/set flow.
- No complete/uncomplete set integration.

Possible next frontend tasks:

- Create `workoutSessionsApi.ts` from Swagger/OpenAPI.
- Build active workout page using real DTOs.

## Food Logs

Status:

- Not implemented / Placeholder.

Existing functionality:

- Static placeholder card.

Backend integration:

- None in frontend.
- OpenAPI contains food log and food product endpoints.

Known limitations:

- No food product search, log creation, daily summary, update/delete.

Possible next frontend tasks:

- Create food API module from Swagger/OpenAPI.
- Build food log list/editor flows.

## User Profile

Status:

- Partial placeholder.

Existing functionality:

- Static profile form inputs.
- Login/logout buttons.

Backend integration:

- Auth redirect calls are real.
- No user profile API integration.
- OpenAPI contains `/api/app/user-profile/my-profile`.

Known limitations:

- Form fields are not loaded or saved.
- No `userProfilesApi.ts`.
- No auth state display.

Possible next frontend tasks:

- Create user profile API module from Swagger/OpenAPI.
- Load/save profile fields.
- Show logged-in user state.

---

# 11. Reusable Components

Shared components live in `/react/src/components`.

- `PageShell`: app frame with fixed-style header/title, scrollable content, and bottom nav. Every main page uses this directly.
- `BottomNav`: five-tab bottom navigation using `NavLink`. Routes: Home, Food, Training, Plan, Profile.
- `NeoCard`: simple wrapper rendering `div.neo-card` plus optional extra class.
- `NeoButton`: button wrapper applying `neo-button`; default `type="button"`.
- `NeoInput`: labeled input with `error` display and ARIA attributes. Extends `InputHTMLAttributes<HTMLInputElement>`.
- `NeoSelect`: labeled select with options and optional error. Extends `SelectHTMLAttributes<HTMLSelectElement>`.
- `LoadingState`: card-style skeleton/loading message.
- `ErrorState`: card-style error message with optional title/action.
- `EmptyState`: card-style empty message with optional action.

Conventions:

- Reuse shared components before creating new equivalents.
- Buttons that submit forms should explicitly pass `type="submit"` because `NeoButton` defaults to `button`.
- Navigation links are plain `Link` elements styled with `neo-button link-button` classes.

---

# 12. Design System and UI Style

The implemented visual style is Neo-Brutalism.

Design tokens in `/react/src/styles/global.css`:

```css
--color-bg: #fff8e7;
--color-text: #0b0b0b;
--color-border: #0b0b0b;
--color-blue: #1463ff;
--color-pink: #ff4f9a;
--color-yellow: #ffe14d;
--color-lime: #b7f34b;
--color-white: #ffffff;
--border-thick: 3px solid var(--color-border);
--shadow-hard: 5px 5px 0 var(--color-border);
```

Style characteristics:

- Bold black borders, usually `3px`.
- Hard shadows without blur.
- High-contrast blocks: blue, pink, yellow, lime, white.
- Heavy uppercase typography.
- Minimal/no border radius.
- Physical active state via translate + reduced shadow.
- Mobile app shell centered at `width: min(100%, 430px)`.
- Fixed bottom nav.
- Page content is the scroll container; header/title remain visually fixed within shell.

Important reusable classes:

- `.page-shell`, `.app-header`, `.page-title`, `.page-content`.
- `.bottom-nav`, `.bottom-nav-item`, `.bottom-nav-item.active`.
- `.neo-card`, `.neo-button`, `.neo-field`, `.neo-input`.
- `.state-card`, `.empty-state`, `.error-state`, `.loading-state`.
- `.exercise-list`, `.exercise-card`, `.exercise-tags`, `.exercise-status`.
- `.plan-list-card`, `.plan-list-tags`.
- `.picker-card`, `.picker-toggle`, `.picker-target-form`, `.picker-footer`.
- `.form-stack`, `.editor-card`, `.placeholder-card`.

No Tailwind conventions exist; styling is global CSS class-based.

---

# 13. Responsive Design

Current responsive behavior:

- Mobile-first fixed app-shell width: `min(100%, 430px)`.
- The app is centered on larger screens.
- `body` uses `overflow: hidden` and `.page-content` scrolls vertically.
- Bottom nav is fixed to the viewport bottom at the same max width as shell.
- Header and page title remain outside the scroll area.
- Most grids are small fixed grids: macro cards 3 columns, stats 3 columns, filters 2 columns, picker target form 2 columns.

Breakpoints:

- No CSS media queries or explicit breakpoints currently exist.

Navigation:

- Bottom nav is always present through `PageShell`.
- Top menu button exists visually but has no behavior.

Responsive limitations:

- Some two/three-column mobile grids may become tight on very narrow devices.
- Desktop layout intentionally remains phone-width, not expanded.
- Some pages are placeholders and not fully tuned.

---

# 14. Forms and Validation

Form library:

- None.

Validation library:

- None.

Input components:

- `NeoInput` and `NeoSelect` are used for most inputs.
- Textareas are plain `<textarea className="neo-input">` inside `label.neo-field`.

Frontend validation strategy:

- Minimal UX validation only.
- `WorkoutPlanEditorPage` checks name is not empty.
- `WorkoutPlanExerciseEditorPage` checks sets/reps >= 1 and kg/rest >= 0 if provided.
- Picker target inputs use HTML `type="number"` and `min`, but no explicit validation messages.

Backend validation handling:

- `httpClient.ts` reads ABP validation error response and displays the first validation error message where pages catch errors.
- Backend remains source of truth for business validation.

Do not duplicate backend rules in React:

- Do not reimplement domain invariants, authorization, uniqueness, owner checks, archived-plan rules, or persistence rules in the frontend.
- Only add minimal UI validation to improve user experience before submit.

---

# 15. Error / Loading / Empty States

Existing patterns:

- Page components generally store `isLoading` and `errorMessage` in local state.
- Loading renders `LoadingState` inside `PageShell`.
- API failures render `ErrorState`.
- Empty data renders `EmptyState`.
- Mutations usually set `isSubmitting` or `mutatingExerciseId` and disable buttons.

Existing state components:

- `LoadingState`: skeleton card and message.
- `ErrorState`: error card with `!` icon, title, message, optional action.
- `EmptyState`: empty card with square symbol, title, message, optional action.

Notifications/toasts:

- No toast/notification system exists.
- Remove confirmation currently uses `window.confirm`.

API failure behavior:

- Errors are thrown from `apiRequest()` as `Error` with a human-readable message.
- Pages catch errors and set `errorMessage`.

---

# 16. State Management

Local UI state:

- `useState` is used extensively for form state, loading/error flags, selected picker items, filters, and mutation flags.

Server state:

- Loaded with `useEffect` directly in page components.
- No cache, query invalidation, or shared data layer exists.
- Pages reload data when route params change or on mount.

Global state:

- No React global state store.
- OIDC user is persisted by `oidc-client-ts` in `localStorage`, but there is no React auth context.
- No theme state.

Where state should live based on current codebase:

- Page-specific forms and filters should stay in the page component unless they are reused.
- Shared API request concerns should remain in `api/httpClient.ts`.
- New domain API functions should be added to an appropriate module under `api/` rather than inline raw fetch calls in pages.

---

# 17. Important Coding Conventions

Observed conventions:

- Component files use PascalCase: `WorkoutPlansPage.tsx`, `NeoButton.tsx`.
- Feature folders are domain-oriented and plural where appropriate: `features/exercises`, `features/workoutPlans`.
- API modules are camelCase plural: `exercisesApi.ts`, `workoutPlansApi.ts`.
- DTO/type names use PascalCase and usually mirror backend DTO names.
- Query types use ABP property casing like `MaxResultCount`, `FilterText`, `IsActive`.
- Shared components use named exports.
- Pages use named exports.
- `App.tsx` default-exports `App`.
- Imports use relative paths; no path aliases configured.
- CSS is global class-based; no CSS modules.
- Forms use controlled inputs.
- Backend calls go through API modules and `apiRequest()`.
- Route params are read with `useParams()`.
- Navigation after mutations uses `useNavigate()`.

Inconsistencies/technical notes:

- Formatting is not fully consistent across files.
- Some types are local inside pages instead of shared.
- Some route/page names are singular/plural mixed, but current routing should be preserved.

---

# 18. Things Lovable MUST NOT Do

Repository-specific guardrails:

- Do not create a new frontend app inside or beside `/react`.
- Do not replace `PageShell`, `BottomNav`, or existing Neo components without explicit instruction.
- Do not replace the global Neo-Brutalist CSS direction with Tailwind, shadcn, Material UI, Bootstrap, or another design system unless explicitly requested.
- Do not remove the ABP/OIDC auth flow.
- Do not remove `apiRequest()` or bypass it with scattered raw `fetch` calls.
- Do not assume OpenAPI JSON is generated TypeScript; it is only a contract reference file.
- Do not hardcode backend production URLs; use Vite env config.
- Do not turn placeholder pages into fake-complete pages with mock production data.

General rules:

- Do not create a new backend.
- Do not introduce Supabase unless explicitly requested.
- Do not replace the ABP backend.
- Do not implement backend business rules in React.
- Do not invent API endpoints.
- Do not invent DTO fields.
- Do not bypass authentication.
- Do not bypass backend authorization.
- Do not duplicate existing reusable components.
- Do not rewrite the whole frontend architecture just to implement one screen.
- Do not replace established libraries without a clear reason.
- Do not remove existing working features.
- Do not hardcode fake production data.
- Do not put API calls directly everywhere inside UI components if the project already has an API/service layer.

---

# 19. Known Technical Debt

Actual technical debt observed:

- No generated API client; DTO types are handwritten and can drift from backend.
- `openapi.json` is large and checked into `src/api`, but no generation workflow exists.
- No auth context/provider; UI cannot easily react to login state.
- No protected routes or permission guards.
- No token refresh/silent renew flow.
- `FoodLogPage`, `WorkoutPage`, and `DashboardPage` remain mostly placeholders/static; `ProfilePage` and the dashboard API layer are connected to backend contracts.
- `ExercisePickerPage` has a standalone route that only logs selected exercise IDs when no `planId` exists.
- Some code formatting/indentation is inconsistent.
- Some repeated helper functions exist, such as exercise name lookup in multiple files.
- `WorkoutPlanDetailPage` resolves exercise names by fetching all selectable exercises, which may fail for inactive/missing exercises.
- `WorkoutPlanExerciseEditorPage` exists but plan detail currently does not expose a visible edit target action.
- No toast/notification system.
- No tests configured.
- No server-state cache; repeated pages may refetch the same reference lists.
- CSS is a single growing global file and can become hard to maintain.
- Some page-specific class names reuse older generic names such as `placeholder-card`.

Do not fix these as part of context/handover documentation unless explicitly asked in a later task.

---

# 20. Current Development Status

| Area | Status | Notes |
| --- | --- | --- |
| App shell | Implemented | `PageShell`, fixed bottom nav, scrollable content. |
| Routing | Implemented/Partial | Routes exist; no nested layouts or guards. |
| Authentication | Partial | OIDC redirect flow exists; no auth context/guard/refresh. |
| Authorization | Backend only | No frontend permission checks. |
| API client | Implemented/Partial | `apiRequest()` handles fetch, bearer token, ABP errors; no generated client. |
| OpenAPI | Available | `src/api/openapi.json` exists as reference only. |
| Design system | Implemented/Partial | Neo-Brutalist global CSS and reusable components. |
| Dashboard | Implemented/Partial | Real API integration with date selection, loading/error/empty states, and incomplete-profile CTA; route protection remains. |
| Exercise Library | Implemented | Real API read integration and filters. |
| Exercise Detail | Implemented/Partial | Real read integration; action buttons not fully wired. |
| Exercise Picker | Implemented/Partial | Real add-to-plan integration; standalone route incomplete. |
| Workout Plans List | Implemented/Partial | Real list API; no paging/filter UI. |
| Workout Plan Editor | Implemented/Partial | Create/update real API; minimal validation. |
| Workout Plan Detail | Implemented/Partial | Detail, add, remove, reorder; edit-target link not surfaced. |
| Workout Plan Exercise Editor | Implemented/Partial | Real update API; route exists. |
| Workout Sessions | Not implemented | Placeholder UI; OpenAPI endpoints exist. |
| Food Logs | Not implemented | Placeholder UI; OpenAPI endpoints exist. |
| Food Products | Not implemented | OpenAPI endpoints exist; no frontend module/page. |
| User Profile | Implemented/Partial | `profileApi.ts` powers real load/save form; auth hardening and route protection remain. |
| Testing | Not implemented | No test tooling beyond TypeScript build and lint. |

---

# 21. Recommended Next Steps

P0 — required to make core flows work:

- Add visible edit-target action in `WorkoutPlanDetailPage` linking to `/plans/:planId/exercises/:workoutPlanExerciseId/edit`.
- Finish authentication UX: show login state, move login/logout controls into a better place, and add a simple protected-route strategy if needed.
- Create API modules only after checking OpenAPI for the next aggregate to implement.
- Implement Workout Session API layer and active workout flow next if that is the product priority.

P1 — important:

- Harden authentication and add protected-route behavior for user-specific pages.
- Implement food log/product API layer and replace `FoodLogPage` placeholder.
- Improve `ExercisePickerPage` validation for target fields before submit.
- Add plan filtering/search/pagination if plan count grows.
- Add archive/restore/delete UI for workout plans if backend behavior is confirmed.

P2 — polish/improvements:

- Extract repeated formatter/helper functions.
- Consider a small auth context after auth UX requirements stabilize.
- Consider a server-state library only when repeated fetching/cache invalidation becomes painful.
- Split `global.css` into organized sections or files if it becomes hard to maintain.
- Add a toast system if mutation success/error feedback becomes common.
- Add frontend tests later for critical form and API-flow behavior.

---

# Upcoming Development — Phase 4 to Phase 8

Executive summary:

- Phase 4 moves the app from workout plan preparation into actual workout execution through Workout Session APIs.
- Phase 5 replaces the food placeholder with food product search and user-specific food logging.
- Phase 6 connects user profile and dashboard screens to real backend data.
- Phase 7 hardens React SPA authentication around the existing OIDC/OpenIddict direction.
- Phase 8 normalizes mobile UX, forms, states, navigation, and the existing Neo-Brutalist design.

Important interpretation:

- This roadmap is product intent, not proof that every frontend file already exists.
- Local OpenAPI currently contains endpoints/schemas for workout sessions, food logs/products, user profile, and dashboard, but the frontend has not yet typed or integrated those domains.
- Before implementation, Lovable/Codex must verify live Swagger/OpenAPI and backend behavior because the backend remains authoritative.
- Do not implement the roadmap from this document alone; use it to guide the next safe incremental task.

---

# Phase 4 — Workout Session Aggregate

Status:

- Next.

Current repository state:

- `react/src/features/workoutSessions/WorkoutPage.tsx` exists but is placeholder/static UI.
- No `react/src/api/workoutSessionsApi.ts` exists.
- No `ActiveWorkoutPage` exists.
- No route exists for active workout detail beyond `/workout`.
- Workout plan flows exist and can become the entry point for “start workout”.
- `react/src/api/openapi.json` currently contains WorkoutSession endpoints and schemas, but these are not yet converted into TypeScript frontend types.

Backend verification required:

- Confirm `FitLogs.Workouts.WorkoutSessionDto` fields and `exercises` shape.
- Confirm `FitLogs.Workouts.WorkoutSessionExerciseDto` fields and nested `sets` shape.
- Confirm `FitLogs.Workouts.ExerciseSetDto`, `AddExerciseSetDto`, and `UpdateExerciseSetDto` fields.
- Confirm enum meanings for `FitLogs.Workouts.WorkoutSessionStatus` values `[0,1,2]`.
- Confirm enum meanings for `FitLogs.Workouts.WorkoutSessionExerciseStatus` values `[0,1,2,3]`.
- Confirm business rule for one active session per user.
- Confirm completed/cancelled session edit restrictions.
- Confirm whether `GET /api/app/workout-session/active` returns `null`, `404`, or an empty body when there is no active session.
- Confirm whether “start from plan” should call `POST /api/app/workout-session` with `workoutPlanId` from `CreateWorkoutSessionDto`.
- Confirm whether current exercise navigation is controlled only by backend endpoints.

Relevant local OpenAPI evidence:

- `GET /api/app/workout-session`
- `POST /api/app/workout-session`
- `GET /api/app/workout-session/active`
- `GET /api/app/workout-session/{id}`
- `DELETE /api/app/workout-session/{id}`
- `POST /api/app/workout-session/{id}/cancel`
- `POST /api/app/workout-session/{id}/complete`
- `GET /api/app/workout-session/{id}/current-exercise`
- `POST /api/app/workout-session/{id}/move-to-next-exercise`
- `POST /api/app/workout-session/{id}/move-to-previous-exercise`
- `POST /api/app/workout-session/{id}/skip-current-exercise`
- `POST /api/app/workout-session/{id}/exercise`
- `PUT /api/app/workout-session/{id}/exercise/{workoutSessionExerciseId}`
- `DELETE /api/app/workout-session/{id}/exercise/{workoutSessionExerciseId}`
- `POST /api/app/workout-session/{id}/set/{workoutSessionExerciseId}`
- `PUT /api/app/workout-session/{id}/set`
- `DELETE /api/app/workout-session/{id}/set`
- `POST /api/app/workout-session/{id}/complete-set`
- `POST /api/app/workout-session/{id}/uncomplete-set`

Planned frontend files/areas:

- Add `react/src/api/workoutSessionsApi.ts` following `workoutPlansApi.ts` and `exercisesApi.ts` conventions.
- Extend routes in `react/src/App.tsx` only after deciding page names/paths.
- Replace or extend `react/src/features/workoutSessions/WorkoutPage.tsx`.
- Likely add `react/src/features/workoutSessions/ActiveWorkoutPage.tsx`.
- Reuse `ExercisePickerPage` patterns only after confirming session add-exercise DTOs.
- Reuse `NeoCard`, `NeoButton`, `NeoInput`, `EmptyState`, `LoadingState`, `ErrorState`, and `PageShell`.

Expected user flow:

```text
Workout Plan Detail
→ Start Workout
→ Active Workout
→ Current Exercise
→ Set Management
→ Finish/Cancel
→ Result/History if backend supports history/list UX
```

Dependencies:

- Existing `workoutPlansApi.ts` and `WorkoutPlanDetailPage`.
- Existing `apiRequest()` for bearer token and ABP error handling.
- Existing exercise catalog APIs for resolving exercise names, unless WorkoutSession DTOs already provide names.
- Backend OpenAPI/Swagger verification.
- Auth token flow should work for user-specific session APIs; route protection can be added later but backend authorization remains required.

Definition of done:

- `workoutSessionsApi.ts` exists with DTOs/enums and functions matching verified backend contracts.
- `/workout` or a new active-workout route displays active session from real API.
- No-active-session state uses `EmptyState` and a CTA to choose/start from a workout plan.
- Workout plan detail can start a workout and navigate to active workout.
- Current exercise UI displays backend current exercise and its sets.
- Next/previous/skip controls call real API and refresh state.
- Set add/update/remove/complete/uncomplete operations call real API and handle loading/error states.
- Complete/cancel session require confirmation and make completed/cancelled sessions read-only in UI.
- Backend business errors are surfaced clearly, not hidden behind generic messages.

Risks:

- OpenAPI confirms endpoints exist locally, but exact enum semantics and no-active-session behavior must be verified.
- Exercise names may require joining with exercise catalog unless backend session DTO includes display names.
- Session set fields in local OpenAPI are reps/weight/rpe/note, not duration/distance; duration/distance must not be implemented unless backend DTOs support them.
- Starting from a workout plan must follow backend constraints; do not infer client-side session generation rules from plan data.

---

# Phase 5 — Food Product Search + Daily Food Log

Status:

- Planned. Backend contracts have now been audited against the current C# source and `react/src/api/openapi.json`.
- `react/src/features/foodLogs/FoodLogPage.tsx` is still placeholder-only.
- No food API client, product search flow, barcode flow, daily log integration, or food-log editor exists in React.

## Objective

Replace the static `/food` page with a backend-connected daily nutrition log. A signed-in user must be able to select a date, view the server-calculated daily totals, search or scan for a food product, create a log entry, and edit or delete an owned entry.

Food-product catalog administration is not part of the end-user Phase 5 slice. The backend exposes product mutation operations, but their authorization contract is incomplete and must be resolved before an administration UI is planned.

## Backend evidence

Primary source files:

- `src/FitLogs.Application/Foods/FoodProductAppService.cs`
- `src/FitLogs.Application/Foods/FoodProductLookupAppService.cs`
- `src/FitLogs.Application/Foods/FoodLogAppService.cs`
- `src/FitLogs.Domain/Foods/FoodLogManager.cs`
- `src/FitLogs.Domain/Foods/FoodLog.cs`
- `src/FitLogs.Domain/Foods/FoodProduct.cs`
- `src/FitLogs.EntityFrameworkCore/Foods/EfCoreFoodLogRepository.cs`
- `src/FitLogs.Application.Contracts/Foods/`
- `react/src/api/openapi.json`

## Barcode-to-food-log audit result

The backend portion of the intended flow exists, but the complete product flow is not implemented in React:

| Intended step | Actual implementation | Status |
| --- | --- | --- |
| Barcode scanner sends a barcode to FitLogs | No scanner or food API client exists in React. `FoodLogPage` is static. | Missing frontend work |
| FitLogs receives the barcode | `POST /api/app/food-product-lookup/lookup-by-barcode` accepts a `barcode` query parameter. | Implemented backend |
| FitLogs calls Open Food Facts | `FoodProductLookupAppService.LookupByBarcodeAsync` calls `IOpenFoodFactsClient.GetByBarcodeAsync`. | Implemented backend |
| Open Food Facts response is mapped | `OpenFoodFactsClient` maps a limited `product`/`nutriments` shape into `OpenFoodFactsProductResult`. | Partially implemented; nutrient coverage is limited |
| Product is cached or created | `FindByBarcodeAsync` checks the database first; a miss creates a `FoodProduct` with source `OpenFoodFacts` and persists it. | Implemented backend |
| Product and macros are displayed | `FoodProductLookupResultDto` supports display, but no React consumer exists. | Missing frontend work |
| User selects quantity/unit/meal/date | Backend DTOs support these fields, but no React form exists. | Missing frontend work |
| FoodLog is created | `FoodLogAppService.CreateAsync` validates the active product, calculates nutrition, applies optional overrides, and persists the log. | Implemented backend |
| Daily log and totals refresh | `by-date` and `daily-summary` endpoints exist, but `/food` does not call either endpoint. | Missing frontend work |

Conclusion: the frontend must call FitLogs only. It must not call Open Food Facts directly, duplicate the external mapping, or calculate final log macros independently.

## External Open Food Facts client audit

Evidence:

- Interface: `src/FitLogs.Application.Contracts/Foods/IOpenFoodFactsClient.cs`, `IOpenFoodFactsClient.GetByBarcodeAsync(string barcode)`.
- Implementation: `src/FitLogs.ExternalServices/OpenFoodFacts/OpenFoodFactsClient.cs`, `OpenFoodFactsClient.GetByBarcodeAsync`.
- Registration: `src/FitLogs.ExternalServices/FitLogsExternalServicesModule.cs`.
- JSON models: `src/FitLogs.ExternalServices/OpenFoodFacts/OpenFoodFactsResponse.cs`.

Actual request behavior:

- Base address is hardcoded to `https://world.openfoodfacts.org/`.
- The request path is `api/v2/product/{barcode}.json`, producing `https://world.openfoodfacts.org/api/v2/product/{barcode}.json`.
- The barcode is interpolated into the URL by the client. The application service trims whitespace before calling the client, but the client itself has no additional URL encoding or barcode-format validation.
- `HttpClient.Timeout` is configured to 10 seconds in `FitLogsExternalServicesModule`.
- No `User-Agent` header is configured in the client registration or request.
- No retry, backoff, circuit breaker, rate-limit handling, or explicit response logging is configured.
- No `CancellationToken` is accepted by `IOpenFoodFactsClient` or passed to `GetFromJsonAsync`; request cancellation is therefore not part of the application contract.
- `GetFromJsonAsync<OpenFoodFactsResponse>` is used directly. Non-success HTTP responses, transport errors, timeout cancellation, and malformed JSON can escape as exceptions; there is no client-level normalization into a FitLogs result type.

Open Food Facts response handling:

| External response condition | Current behavior |
| --- | --- |
| `status == 1` and `product != null` with nonblank `product_name` | Maps to `OpenFoodFactsProductResult`. |
| `status == 0` | Returns `null`. The application service returns `Found = false`. |
| Any status other than `1` | Returns `null`; status meaning is not preserved. |
| `status == 1` but missing `product` | Returns `null`. |
| Product exists but `product_name` is null/blank | Returns `null`; no product is created. |
| `nutriments` is null | Product is still mapped; calories become `0`, macros become `null`. |
| `energy-kcal_100g` is missing | Calories become `0` because of `?? 0`. Missing energy is therefore indistinguishable from a verified zero-calorie value. |
| Protein, carbohydrate, or fat field is missing | The nullable value remains `null`; it is not converted to zero by the external client. |
| Malformed JSON or HTTP/network failure | Exception propagates from the client; no documented FitLogs-specific error envelope or fallback is created here. |

The current implementation has no explicit Open Food Facts rate-limit interpretation. HTTP 429, 5xx, and other non-success responses are `REQUIRES VERIFICATION` at the API boundary because the client does not catch and classify them.

## Open Food Facts nutrition mapping audit

The external model supports only these JSON fields:

| Open Food Facts field | Backend property | Basis | Result |
| --- | --- | --- | --- |
| `nutriments.energy-kcal_100g` | `CaloriesPer100g` | Per 100g | Mapped; missing values become numeric zero. |
| `nutriments.proteins_100g` | `ProteinPer100g` | Per 100g | Mapped as nullable decimal. |
| `nutriments.carbohydrates_100g` | `CarbPer100g` | Per 100g | Mapped as nullable decimal. |
| `nutriments.fat_100g` | `FatPer100g` | Per 100g | Mapped as nullable decimal. |
| `product.serving_size` | `ServingSize` | Descriptive text | Preserved as nullable text; not parsed. |

Not supported by the current external model, `OpenFoodFactsProductResult`, `FoodProduct`, or `FoodProductDto`:

- Saturated fat.
- Sugar.
- Fiber.
- Sodium.
- Salt.
- `energy-kcal_100ml` or other per-100ml variants.
- Per-serving nutrient values.
- Numeric serving quantity or serving unit.
- A nutrition basis/unit field that distinguishes grams, milliliters, and servings.

These nutrients and basis variants must not be added to the frontend types or UI as if they were available. They are `BACKEND MISSING` for the current contract.

The mapping also does not fall back from `energy-kcal_100g` to an energy-kilojoule field. The backend therefore cannot promise calorie data when the kcal field is absent; today it persists zero calories instead. Whether zero should be treated as unknown rather than a valid value requires a backend/data-contract decision.

## Backend product persistence and cache audit

`FoodProductLookupAppService.LookupByBarcodeAsync` performs this sequence:

1. `NormalizeBarcode` trims the input and rejects blank values with `FitLogs:FoodProduct:BarcodeInvalid`.
2. `IFoodProductRepository.FindByBarcodeAsync` searches for an exact barcode match.
3. A match is returned with `Found = true` and `FromCache = true`; no external refresh occurs.
4. A miss calls Open Food Facts.
5. A non-null external result is passed to `FoodProductManager.CreateAsync` with `FoodProductSource.OpenFoodFacts` and `Clock.Now`.
6. The new entity is inserted with `autoSave: true`.
7. The response is mapped to `FoodProductLookupResultDto` with `Found = true` and `FromCache = false`.
8. A null external result returns `Found = false`, `FromCache = false`, and the normalized barcode; no `FoodProduct` is stored.

Persistence facts:

- `FoodProduct` stores `CaloriesPer100g`, nullable protein/carbohydrate/fat per 100g, descriptive `ServingSize`, `Source`, `LastSyncedAt`, `IsActive`, and `IsVerified`.
- Open Food Facts products are active by default and not verified by default; `FoodProduct` sets `IsVerified = true` only for `FoodProductSource.System`.
- `FoodProductManager.CheckBarcodeAsync` trims the barcode and rejects a duplicate with `FitLogs:FoodProduct:006` before insert/update.
- EF Core also creates a unique filtered index on non-null `Barcode` in `FitLogsDbContext`; concurrent first-time lookups can still race between the application check and database insert. The resulting database exception is not normalized by the lookup service.
- There is no TTL, freshness policy, automatic revalidation, or cache invalidation. `RefreshFromOpenFoodFactsAsync` is an explicit separate operation.
- Refresh keeps the existing product ID, updates display/nutrition fields, sets source to `OpenFoodFacts`, updates `LastSyncedAt`, and sets `IsVerified = false`. A missing external product raises `FitLogs:FoodLog:FoodProductNotFoundFromOpenFoodFacts`.

## Macro calculation audit

`src/FitLogs.Domain/Foods/FoodLogManager.cs` contains the only current calculation:

```text
factor = quantity / 100
calories = product.CaloriesPer100g × factor
protein = product.ProteinPer100g × factor
carb = product.CarbPer100g × factor
fat = product.FatPer100g × factor
```

This is correct only when `quantity` is grams and the product values are per 100g. `FoodUnit` is stored and enum-validated, but it is not used in `CalculateNutrition`. Selecting `Milliliter`, `Serving`, or `Piece` does not change the formula. `ServingSize` is never converted into grams or a numeric serving amount.

Examples of current behavior (not desired product semantics):

- Product with `200` kcal per 100g and quantity `50`, unit `Gram` → `100` kcal.
- Same product with quantity `50`, unit `Serving` → also `100` kcal.
- Same product with quantity `50`, unit `Piece` → also `100` kcal.

The frontend must send the selected `Quantity` and `Unit` and display the backend-returned `FoodLogDto` values. It must not present the current formula as a correct serving/piece/ml conversion and must not silently “fix” it in React. Accurate non-gram nutrition requires a backend contract and data-model change.

Nutrition override behavior:

- `CreateFoodLogDto` and `UpdateFoodLogDto` accept optional `OverrideCalories`, `OverrideProtein`, `OverrideCarb`, and `OverrideFat`.
- `FoodLogAppService.ApplyNutritionOverrides` replaces only supplied values and keeps calculated values for omitted fields.
- `FoodLog.SetNutrition` rejects negative values, but the override DTOs do not declare range attributes; final validation is domain-level.
- The standard end-user barcode flow should not expose override fields unless product requirements explicitly call for them. If exposed, the UI must label them as manual overrides and never use them to disguise missing external nutrients.

## Authorization, errors, and observability audit

- `FoodLogAppService` and `FoodProductAppService` have class-level `[Authorize]`.
- `FoodProductLookupAppService` has no explicit `[Authorize]` attribute. Swagger lists 401/403 responses, but the effective anonymous/authenticated behavior is `REQUIRES VERIFICATION` against a running API.
- Food-log get/update/delete operations enforce current-user ownership with `FitLogs:Food:FoodLogAccessDenied`.
- Lookup-specific business errors include `FitLogs:FoodProduct:BarcodeInvalid`, `FitLogs:FoodLog:FoodProductNotFoundFromOpenFoodFacts`, and `FitLogs:FoodProduct:006` for duplicate barcode (use the declarations in `FitLogsDomainErrorCodes` when wiring frontend mappings).
- The external client does not log request IDs, barcode outcomes, status codes, latency, or rate-limit headers in its own code. Whether ABP auditing or host logging captures the service call is `REQUIRES VERIFICATION`.
- No circuit breaker, retry queue, background refresh job, stale-data fallback, or user-facing “external service unavailable” domain error exists in this flow.
- Frontend error handling must distinguish invalid barcode, normal product-not-found (`Found = false`), HTTP/auth failure, timeout/network failure, malformed external response surfaced as a server failure, inactive product, and food-log validation/ownership failure.

## Frontend barcode-to-food-log implementation plan

Current React evidence:

- `react/src/features/foodLogs/FoodLogPage.tsx` renders only hardcoded placeholder content.
- `react/src/App.tsx` exposes only `/food`; there is no add-food, product result, barcode, or edit-log route.
- No `react/src/api/foodsApi.ts` exists.
- No React code references `FoodProductLookupResultDto`, `FoodLogDto`, or the barcode endpoint.
- `react/src/api/httpClient.ts` is the required transport and already attaches the OIDC bearer token when available.

Planned vertical slice:

1. Add one handwritten `foodsApi.ts` using the exact lookup query, product list/detail, food-log list/summary, create, update, and delete contracts. Do not call Open Food Facts from React.
2. Add a barcode entry surface under `/food/add`. The baseline flow accepts typed/pasted barcode input. Camera scanning is a planned extension and requires a frontend camera-permission flow, an approved barcode decoder, mobile-browser testing, and HTTPS/secure-context support.
3. Submit the barcode to FitLogs and branch on `Found`/`FromCache`; render only fields returned by `FoodProductLookupResultDto`.
4. Keep the selected `FoodProductId` and let the user enter `Quantity`, `Unit`, `MealType`, optional `LoggedAt`, and optional `Note` from the exact create DTO.
5. Submit `POST /api/app/food-log`, then reload both `GET /api/app/food-log/by-date` and `GET /api/app/food-log/daily-summary` for the selected date. Do not calculate totals locally.
6. Add edit/delete for owned logs using `GET/PUT/DELETE /api/app/food-log/{id}` and refresh list/summary after mutation.
7. Add loading, empty, retry, validation, unauthorized/forbidden, external failure, and duplicate-submit states.

Required frontend states:

- Barcode idle, submitting, cached match, external match, normal not-found, invalid barcode, timeout/network failure, and server failure.
- Product result with nullable brand/image/macros/serving text; no fake zero values for missing nullable macros.
- Food-log quantity/unit/meal/date validation using backend-supported fields only.
- Empty daily log with server-provided zero summary.
- Successful mutation refresh and stale-data prevention when selected date changes.

## Barcode and nutrition QA matrix

The implementation plan must include contract tests or manual API checks for:

| Scenario | Expected verification |
| --- | --- |
| Known barcode already cached | Lookup returns `Found = true`, `FromCache = true`, no new product row. |
| Known barcode not cached, Open Food Facts `status = 1` | Product fields and four supported per-100g nutrients map, product persists with `Source = OpenFoodFacts`, lookup returns `FromCache = false`. |
| Open Food Facts `status = 0` | Lookup returns `Found = false`; no product or log is created. |
| `status = 1` with missing product or blank product name | Lookup behaves as not-found; exact HTTP/domain envelope is verified. |
| Missing `nutriments` or missing `energy-kcal_100g` | Confirm current zero-calorie/null-macro behavior and decide whether backend must distinguish unknown from zero before frontend release. |
| Missing protein/carbohydrate/fat fields | Confirm nulls survive external mapping, persistence, DTO mapping, and daily summary. |
| Per-serving/per-100ml-only external data | Confirm current client does not map it; frontend must not infer a conversion. |
| Open Food Facts 429, 5xx, timeout, malformed JSON | Record actual FitLogs response and user-facing error mapping; retry/rate-limit policy is currently missing. |
| Blank or whitespace barcode | `FitLogs:FoodProduct:BarcodeInvalid`; no external request. |
| Duplicate concurrent barcode creation | Confirm unique-index behavior and whether the API returns a normalized business error or an unhandled server failure. |
| Inactive product used for log creation/update | `FitLogs:FoodProduct:007` rejection; no log mutation. |
| Gram quantity calculation | Verify `per100g × grams / 100`. |
| Serving/piece/ml quantity calculation | Confirm current backend uses the same formula and keep this as a documented blocker, not frontend logic. |
| Food-log empty day | Empty list plus zero daily totals. |
| Food-log ownership violation | Verify authorization/business error and ensure React does not reveal or mutate another user's data. |

## Phase 5 acceptance update

Phase 5 is complete only when:

- The backend-to-Open Food Facts mapping is documented as limited to `energy-kcal_100g`, `proteins_100g`, `carbohydrates_100g`, `fat_100g`, and `serving_size`.
- The frontend never calls Open Food Facts directly and uses only the FitLogs lookup endpoint.
- `/food` has no mock nutrition values and loads the selected-day list and summary from the backend.
- A barcode result can be selected into a food-log form, and the create request uses the exact backend DTO fields.
- Macro totals and entry macros are always rendered from backend responses after create/update/delete.
- Missing nullable nutrients remain visibly unknown/absent; the UI does not convert them to zero without an explicit product decision.
- Unit semantics and timezone behavior are visibly documented as backend limitations until verified or corrected.
- All external, validation, authorization, ownership, inactive-product, empty-result, and mutation-refresh states are covered.
- No product-administration controls are shipped until the missing FoodProducts permission enforcement and lookup authorization are resolved.

### Food-product read and lookup APIs used by Phase 5

| Method and route | Request | Response | Confirmed behavior |
| --- | --- | --- | --- |
| `GET /api/app/food-product` | Query: `FilterText`, `OnlyActive`, `Sorting`, `SkipCount`, `MaxResultCount` | `PagedResultDto<FoodProductDto>` | Searches name, brand, or barcode. `OnlyActive` defaults to `true`. Default sorting is name ascending. |
| `GET /api/app/food-product/{id}` | UUID path parameter | `FoodProductDto` | Returns one product or a repository not-found error. |
| `POST /api/app/food-product-lookup/lookup-by-barcode?barcode=...` | `barcode` query parameter | `FoodProductLookupResultDto` | Returns a cached product when present. Otherwise queries Open Food Facts and persists a new product when found. A normal not-found result has `found=false`; it is not a fabricated product. |

The following product-management operations exist but are excluded from the end-user food-log slice:

- `POST /api/app/food-product`
- `PUT /api/app/food-product/{id}`
- `DELETE /api/app/food-product/{id}`
- `POST /api/app/food-product/{id}/activate`
- `POST /api/app/food-product/{id}/deactivate`
- `POST /api/app/food-product/{id}/verify`
- `POST /api/app/food-product/{id}/unverify`
- `POST /api/app/food-product-lookup/refresh-from-open-food-facts/{foodProductId}`

`ActivateAsync`, `UnverifyAsync`, and `DeleteAsync` are public methods on `FoodProductAppService` and appear in Swagger even though they are missing from `IFoodProductAppService`. The runtime API surface and the application-service implementation are the evidence for these routes; the interface is incomplete.

### Food-log APIs used by Phase 5

| Method and route | Request | Response | Confirmed behavior |
| --- | --- | --- | --- |
| `GET /api/app/food-log/by-date?Date=...` | `Date` is an OpenAPI `date-time` query value | `FoodLogDto[]` | Returns only the current user's entries, ordered by `loggedAt`. An empty day returns an empty array. |
| `GET /api/app/food-log/daily-summary?Date=...` | Same `Date` query contract | `DailyFoodNutritionSummaryDto` | Returns server-summed calories, protein, carbohydrate, and fat. An empty day returns zero totals. |
| `GET /api/app/food-log/{id}` | UUID path parameter | `FoodLogDto` | Returns an owned log. A log owned by another user is rejected. |
| `POST /api/app/food-log` | JSON `CreateFoodLogDto` | `FoodLogDto` | Creates an entry for `CurrentUser`; omitted `loggedAt` uses the backend clock. |
| `PUT /api/app/food-log/{id}` | JSON `UpdateFoodLogDto` | `FoodLogDto` | Recalculates nutrition from the selected active product and quantity, then applies any supplied nutrition overrides. Omitted `loggedAt` preserves the existing timestamp. |
| `DELETE /api/app/food-log/{id}` | UUID path parameter | No content | Deletes an owned entry. Duplicate deletion resolves as a missing-resource error; there is no idempotency contract. |

## Actual DTO and enum contracts

`FoodProductDto`:

- `id: Guid`
- `barcode: string?`
- `name: string`
- `brand: string?`
- `imageUrl: string?`
- `caloriesPer100g: decimal`
- `proteinPer100g: decimal?`
- `carbPer100g: decimal?`
- `fatPer100g: decimal?`
- `servingSize: string?`
- `source: FoodProductSource`
- `lastSyncedAt: DateTime?`
- `isActive: bool`
- `isVerified: bool`

`FoodProductLookupResultDto`:

- `found: bool`
- `fromCache: bool`
- `foodProductId: Guid?`
- `barcode`, `name`, `brand`, `imageUrl`, and `servingSize`: nullable strings
- `caloriesPer100g`, `proteinPer100g`, `carbPer100g`, and `fatPer100g`: nullable decimals

`FoodLogDto`:

- `id`, `userId`, `foodProductId`
- `foodName`, `quantity`, `unit`
- `calories`, `protein`, `carb`, `fat`
- `mealType`, `loggedAt`, `note`

`CreateFoodLogDto` and `UpdateFoodLogDto` have the same writable fields:

- Required: `foodProductId`, `quantity`, `unit`, `mealType`
- Optional: `loggedAt`, `note`, `overrideCalories`, `overrideProtein`, `overrideCarb`, `overrideFat`

`DailyFoodNutritionSummaryDto` contains `date`, `totalCalories`, `totalProtein`, `totalCarb`, and `totalFat`.

Actual enum values:

| Enum | Values |
| --- | --- |
| `FoodUnit` | `Gram = 1`, `Milliliter = 2`, `Serving = 3`, `Piece = 4` |
| `MealType` | `Breakfast = 1`, `Lunch = 2`, `Dinner = 3`, `Snack = 4`, `PreWorkout = 5`, `PostWorkout = 6` |
| `FoodProductSource` | `Manual = 1`, `OpenFoodFacts = 2`, `System = 3` |

## Business rules the frontend must respect

- `quantity` is a JSON decimal and is validated by the DTO from `0.01` through `999999`.
- A food log can only reference an active food product. Inactive products are rejected with `FitLogs:FoodProduct:007`.
- The backend stores a food-name snapshot and calculated nutrition on each log; the frontend must render the returned `FoodLogDto` rather than recomputing persisted values.
- Base nutrition is calculated as `product per-100g value * (quantity / 100)`.
- **Important backend limitation:** the calculation ignores `FoodUnit`. Gram, Milliliter, Serving, and Piece all currently use the same `quantity / 100` formula.
- `servingSize` is descriptive text only. It is not parsed or used in food-log nutrition calculation.
- The frontend must not invent conversions for milliliters, servings, or pieces and must not simulate a corrected business rule. Accurate non-gram calculation requires a backend contract change.
- Nutrition overrides are supported by the API. Phase 5 should not expose them in the standard end-user form unless the product explicitly requires manual override UX; hidden frontend calculations are prohibited.
- `loggedAt` defaults to the backend clock on create and remains unchanged when omitted on update.
- Date filtering uses `date.Date` through a half-open server-side range `[day start, next day start)`. No user-timezone conversion contract exists.
- The frontend must send an explicit selected-day `Date` value and label timezone behavior as `REQUIRES VERIFICATION`; it must not claim that days are grouped in the user's local timezone.
- Food-log get/update/delete operations check `CurrentUser` ownership. List and summary queries are also restricted to `CurrentUser`.
- Barcode input is trimmed. Blank barcode produces `FitLogs:FoodProduct:BarcodeInvalid`.
- A successful external lookup may create a shared `FoodProduct` record. Retrying the same barcode normally returns that cached product.
- Product deletion deactivates a product when food logs reference it; otherwise it hard-deletes the product. This is backend administration behavior and is not duplicated in React.

## Authentication, permissions, and blockers

- `FoodLogAppService` and `FoodProductAppService` have class-level `[Authorize]`; the React food flow requires a signed-in user.
- Food-log operations have ownership checks but no named fine-grained permissions.
- `FitLogsPermissions.FoodProducts` constants exist, but the current permission definition provider does not register the FoodProducts permission tree and `FoodProductAppService` does not apply those permission names.
- `FoodProductLookupAppService` has no explicit `[Authorize]` attribute. Whether anonymous barcode lookup is intentional is `REQUIRES VERIFICATION`.
- Do not expose create/update/delete/activate/deactivate/verify/unverify/refresh product-management controls until the backend authorization policy is corrected or explicitly approved.
- Exact runtime status codes for business exceptions and authorization failures must be verified against a running API. The frontend must already distinguish 401, 403, validation failures, missing resources, ABP business codes, and network/external-service failures.

## Frontend scope and routes

Keep `/food` as the main route and use the existing mobile shell. Add frontend routes only as needed for focused forms:

| Route | Purpose |
| --- | --- |
| `/food` | Selected-day log list, totals, meal grouping, and add-food entry point. |
| `/food/add` | Product search and barcode lookup, followed by the create-log form. |
| `/food/logs/:foodLogId/edit` | Load one owned entry and submit update or delete. |
| `/food/products/:foodProductId` | Optional read-only product detail used during selection; do not add management controls. |

Create one `react/src/api/foodsApi.ts` module containing the verified DTOs, enums, query types, and endpoint functions. Do not generate an API client and do not split the same contracts across duplicate modules.

Recommended components under the existing food feature:

- Daily date selector.
- Daily nutrition summary card.
- Meal-type section/list.
- Food-log row with edit action.
- Product search field and paged result list.
- Barcode lookup form/result state. The first slice accepts typed/pasted barcode values. A camera-scanning extension must handle camera permissions, decoder setup, mobile browser differences, HTTPS/secure-context requirements, and a typed-input fallback.
- Create/update food-log form using only backend-supported fields.
- Delete confirmation.

Use current page-local `useState`/`useEffect` conventions and `apiRequest()` for this phase. Do not introduce a query-cache, form, validation, or state-management library solely for Phase 5. After every successful create/update/delete, reload both the selected-day list and daily summary from the backend.

## Required UI states and failure behavior

- Daily log: initial loading, populated, empty day, load error, and date-change loading.
- Product search: idle, searching, paged results, no result, request error, and inactive product protection.
- Barcode: idle, submitting, cached match, external match, `found=false`, invalid barcode, and external-service/network failure.
- Create/update: field validation, submitting, backend validation failure, product inactive, missing product/log, authorization failure, and success navigation.
- Delete: confirmation, deleting, missing/already-deleted resource, authorization failure, and success refresh.
- Disable duplicate mutation submissions while a request is in progress.
- Preserve entered form values when a recoverable API error occurs.
- Never replace a backend rejection with silently calculated or mocked data.

## Camera scanning extension

Status:

- Planned follow-up within Phase 5; not included in the first typed/pasted barcode slice.

Requirements:

- Request camera access through the browser permission flow and handle denied, unavailable, and already-in-use cameras.
- Add and approve a barcode decoder dependency or browser-supported decoder before implementation; no decoder dependency currently exists.
- Require an HTTPS secure context in production. `localhost` may be used for local development where the browser permits camera access.
- Test on supported mobile Chrome and Safari versions, including portrait/landscape behavior, permission prompts, camera cleanup, and narrow-screen layout.
- Stop camera tracks when leaving the scanner or after a successful read; do not upload or persist camera frames.
- Send only the decoded barcode string to FitLogs through the existing lookup endpoint.
- Keep typed/pasted barcode entry available as a fallback when camera access or decoding fails.

## Recommended implementation order

1. Add verified `foodsApi.ts` types, enum labels, list/detail/lookup/log/summary/mutation functions, and food-related ABP error mappings.
2. Replace `/food` placeholder content with selected-date list and daily summary calls.
3. Group returned logs by `MealType` in the presentation layer and add loading, empty, error, and retry states.
4. Build paged product search using `FilterText`, `OnlyActive: true`, `SkipCount`, and `MaxResultCount`; do not download the full catalog for local filtering.
5. Add typed/pasted barcode lookup and handle cached, external, and not-found results distinctly.
6. Add the camera-scanning extension after the permission, decoder, HTTPS, and mobile-browser prerequisites are satisfied; preserve typed/pasted fallback behavior.
7. Build create-log form and refresh both list and summary from server response data.
8. Build owned-log edit and delete flow with confirmation and mutation guards.
9. Complete mobile, keyboard, accessibility, and narrow-screen validation.

## Dependencies

- App-wide authentication state and protected-route behavior from the authentication phase.
- Existing `apiRequest()`, `ApiError`, and OIDC token handling.
- Existing `PageShell`, `NeoInput`, `NeoSelect`, `NeoButton`, `NeoCard`, `LoadingState`, `EmptyState`, and `ErrorState`.
- Backend availability and Open Food Facts network availability for uncached barcode lookups.
- Camera scanning prerequisites: browser camera permissions, an approved barcode decoder, HTTPS/secure context, and mobile Chrome/Safari test coverage.
- Backend clarification or change before any accurate Serving, Piece, or Milliliter nutrition UX is promised.

## Acceptance criteria

- `/food` contains no hardcoded nutrition or placeholder log data.
- A signed-in user can select a date and see the exact list and totals returned by the two daily endpoints.
- An empty day displays a useful empty state with zero server totals and an add-food action.
- Product search uses backend paging and returns active products only in the selection flow.
- Typed/pasted barcode lookup correctly distinguishes cache hit, external hit, normal not-found, invalid input, and request failure.
- If camera scanning is enabled, it requests permission clearly, handles denied/unavailable cameras, decodes on supported mobile browsers, stops camera tracks safely, and falls back to typed/pasted input.
- Create and update payloads contain only fields from `CreateFoodLogDto` or `UpdateFoodLogDto`.
- All four `FoodUnit` and six `MealType` enum values use their exact numeric values.
- The UI does not calculate unit conversions or treat `servingSize` as a numeric conversion factor.
- The server-returned `FoodLogDto` and daily summary are reloaded after every mutation.
- Users cannot edit or delete another user's entry; 401/403 responses produce an authorization-specific UI state.
- Product-management actions remain absent until their permission contract is verified.
- All forms and result lists have loading, empty, validation, error, retry, and duplicate-submit protection as applicable.
- The flow remains usable on narrow mobile screens and with keyboard navigation.

## Technical risks and unresolved blockers

- **Unit semantics:** the backend accepts four units but calculates all nutrition as though quantity were a per-100g multiplier. Non-gram accuracy is blocked by a backend change.
- **Timezone semantics:** daily queries use server-side `DateTime.Date` boundaries without an explicit user-timezone policy.
- **Authorization:** food-product administration permissions are declared incompletely and are not enforced by the product app service; barcode lookup is not explicitly authenticated.
- **Contract drift:** the product application-service interface omits public operations exposed by the runtime conventional controller.
- **External dependency:** uncached barcode lookup depends on Open Food Facts and may fail even when the FitLogs API is otherwise healthy.
- **Concurrency:** barcode uniqueness is enforced in persistence/domain logic, so concurrent first-time lookups for the same barcode can still require runtime error handling. No optimistic concurrency token is exposed to the React client.

---

# Phase 6 — User Profile + Dashboard

Status:

- Phase 6.1–6.6 completed; Phase 7 authentication hardening is next.

Current ProfilePage state:

- `react/src/features/userProfile/ProfilePage.tsx` loads the signed-in profile, validates basic UX ranges, and saves through the backend.
- `react/src/api/profileApi.ts` contains the verified profile DTOs, enum types, and GET/PUT operations.
- The form uses only backend-supported fields: display name, gender, date of birth, height, weight, fitness goal, daily calories, and IANA timezone.
- Complex profile mapping, validation, loading, and persistence functions include simple explanatory comments.

Current DashboardPage state:

- `react/src/features/dashboard/DashboardPage.tsx` loads real combined dashboard summaries, supports an optional selected date, and shows loading/error/empty states.
- Nutrition, macro, meal, and completed-workout cards use only backend DTO values; unsupported placeholder streak, progress, weight, and plan-name cards were removed.
- The dashboard loads the profile independently and shows a `/profile` CTA when height, weight, or daily calorie target is missing.
- `react/src/api/dashboardApi.ts` contains the verified dashboard DTOs and four read operations.
- No dashboard route protection exists.

Current profile API availability:

- Local OpenAPI contains `GET /api/app/user-profile/my-profile` and `PUT /api/app/user-profile/my-profile`.
- Local OpenAPI schemas include `UserProfileDto` and `UpdateUserProfileDto`.
- Fields observed locally: displayName, gender, dateOfBirth, heightCm, weightKg, fitnessGoal, dailyTargetCalories, and timeZoneId.
- `Gender`: 0 Male, 1 Female, 2 Private. `FitnessGoal`: 1 LoseWeight, 2 MaintainWeight, 3 GainMuscle, 4 ImproveFitness.
- `GetMyProfileAsync` creates a default profile when one does not exist, so the frontend can render the returned profile instead of treating a missing profile as a 404.

Current dashboard API availability:

- Local OpenAPI contains dashboard endpoints:
  - `GET /api/app/dashboard/today`
  - `GET /api/app/dashboard/daily?Date=...`
  - `GET /api/app/dashboard/daily-nutrition?Date=...`
  - `GET /api/app/dashboard/daily-workout?Date=...`
- Local OpenAPI schemas include `DailyDashboardDto`, `DailyNutritionSummaryDto`, `DailyWorkoutSummaryDto`, and `MealCaloriesBreakdownDto`.
- `dashboardApi.ts` consumes these endpoints through the shared `apiRequest` client.

Missing-profile behavior:

- Confirmed create-on-read: `GET /my-profile` creates and returns a default profile when needed.
- Nullable optional fields are rendered as blank inputs and sent back as `null` when left blank.
- Dashboard renders an explicit no-data state when both summaries are empty.
- Because the dashboard DTO has no profile-completeness flag, the frontend derives a narrow CTA from missing height, weight, or daily target values without replacing backend validation.

Dependency between profile and dashboard:

- Dashboard nutrition target appears likely related to `dailyTargetCalories`, but frontend must verify via dashboard DTOs and backend behavior.
- Dashboard should not compute business summaries if backend already provides them.
- Profile should be implemented before relying on profile-driven dashboard targets.

Planned frontend areas:

- Replace static DashboardPage cards with real dashboard API data.

Definition of done:

- Profile page loads current profile, handles create-on-read/null fields, validates only basic UX constraints, and saves through backend API. **Completed in Phase 6.3.**
- Dashboard API layer exposes combined, nutrition-only, workout-only, and optional-date reads. **Completed in Phase 6.4.**
- Dashboard page loads real dashboard data, handles missing nutrition/workout data, and shows loading/error/empty states. **Completed in Phase 6.5.**
- No dashboard values remain hardcoded as production data. **Completed in Phase 6.5.**
- Dashboard does not crash when profile or daily logs are missing, and incomplete profiles receive a profile CTA. **Completed in Phase 6.6.**

Risks:

- Dashboard route protection remains unresolved and belongs to Phase 7 authentication hardening.
- Auth route protection becomes more important as these pages become user-specific.

---

# Phase 7 — React SPA Authentication

Status:

- In Progress / Planned hardening.

## Current authentication state

- `oidc-client-ts` is already installed and used.
- `react/src/auth/authService.ts` configures `UserManager` with `response_type: 'code'`.
- `WebStorageStateStore` stores OIDC user data in `window.localStorage`.
- `login()`, `logout()`, `handleLoginCallback()`, `handleLogoutCallback()`, `getCurrentUser()`, and `getAccessToken()` exist.
- `AuthCallbackPage` and `AuthLogoutCallbackPage` exist and are routed.
- `apiRequest()` attaches `Authorization: Bearer <token>` when `getAccessToken()` returns a non-expired token.
- `apiRequest()` also uses `credentials: 'include'`.
- `ProfilePage` has login/logout buttons.
- No React auth context/provider exists.
- No route guard exists.
- No silent renew/refresh handling is implemented.
- No permission/role checks are implemented in frontend.

## Target authentication state

- Target production strategy: OIDC Authorization Code Flow + PKCE, assuming this remains compatible with backend OpenIddict configuration.
- Current `oidc-client-ts` usage is aligned with the target direction but should be verified for PKCE behavior/configuration and OpenIddict client settings.
- Development/local auth may remain looser if that is a conscious project decision, but production target must be documented separately.

## Backend prerequisites

Must be verified in backend/OpenIddict, not invented in React:

- React SPA client exists, currently expected by env as `FitLogs_App`.
- Redirect URI includes `http://localhost:5173/auth/callback` for dev.
- Post-logout redirect URI includes `http://localhost:5173/auth/logout-callback` for dev.
- Allowed scopes include the values requested by `VITE_OIDC_SCOPE`: `openid profile email roles FitLogs`.
- Grant type supports authorization code flow for SPA usage.
- PKCE requirements are correctly configured.
- CORS allows the React dev origin `http://localhost:5173` where needed.
- Backend authorization policy is intentional for catalog read APIs: keep public or restore `[Authorize]` is a backend/product decision.

Do not implement backend changes from Lovable unless explicitly requested.

## Frontend target architecture

Preserve the current architecture:

```text
React pages/routes
→ auth layer (`authService.ts`, future auth context if needed)
→ `apiRequest()` centralized token attachment
→ ABP REST API
```

Target improvements:

- Keep token lookup and attachment centralized in `httpClient.ts` / auth layer.
- Add auth state access through a small provider/hook only when UI/route guards require it.
- Add a protected route wrapper only after deciding which routes should require login.
- Handle expired tokens centrally instead of adding token checks in every page.
- Avoid custom username/password forms unless backend explicitly provides and requires them.

## Route protection

Likely user-specific routes based on current app and roadmap:

- `/profile`
- `/food`
- `/plans`
- `/plans/new`
- `/plans/:planId`
- `/plans/:planId/edit`
- `/plans/:planId/add-exercises`
- `/plans/:planId/exercises/:workoutPlanExerciseId/edit`
- `/workout`
- future active workout/session routes
- `/` dashboard if it shows user-specific data

Potentially public routes:

- `/exercises`
- `/exercises/:exerciseId`

Catalog authorization is unresolved and must be confirmed with backend/product owner.

## Security constraints

Lovable MUST NOT:

- Build custom username/password authentication if backend already uses OpenIddict/OIDC.
- Invent auth endpoints.
- Bypass backend authorization.
- Put secrets into frontend code.
- Implement security-sensitive business rules only on the client.
- Scatter token handling across pages/components.
- Disable backend auth/CORS/antiforgery as a frontend workaround.

---

# Phase 8 — UX Polish + Mobile Quality

Status:

- Planned, ongoing polish only after core flows are functional.

Audit scope:

- Responsive behavior on narrow mobile widths, tablet, desktop, and the current `min(100%, 430px)` max-width shell.
- Form UX: disabled submit while submitting, validation messages, backend business errors displayed clearly.
- Navigation: active bottom tab, back links for detail/editor screens, sensible post-mutation navigation.
- Loading state consistency using `LoadingState`.
- Error state consistency using `ErrorState`.
- Empty state consistency using `EmptyState`.
- Neo-Brutalism consistency: black borders, hard shadows, high-contrast colors, bold uppercase typography, clear physical button states.

Current repository state:

- Shared state components already exist.
- `PageShell` provides a scrollable `.page-content` while header/title and bottom nav stay fixed visually.
- Some pages already use loading/error/empty states well, especially exercises and workout plans.
- Placeholder pages and future pages still need consistent states.

Constraints:

- This phase should not redesign the entire application.
- Do not add complex animations or visual effects inconsistent with current Neo-Brutalist direction.
- Prefer normalizing existing UI classes/components over introducing a new UI library.

Definition of done:

- Core user flows have consistent loading/error/empty handling.
- Forms disable while submitting and show clear validation/API errors.
- Mobile layouts do not overflow at common narrow widths.
- Bottom nav active states and back navigation are correct.
- Visual style is consistent across implemented aggregates.

---

# Roadmap Dependency Order

```text
Existing Exercise + Workout Plan foundations
↓
Phase 4.1 Verify WorkoutSession contracts
↓
Phase 4.2 Build workoutSessionsApi.ts
↓
Phase 4.3–4.9 Active workout/session execution UX
↓
Workout UX complete enough for real training flow

Food OpenAPI verification
↓
foodsApi.ts
↓
FoodLogPage real API
↓
Food search/add/edit flows

UserProfile OpenAPI verification
↓
profileApi.ts
↓
ProfilePage real API
↓
Dashboard API verification
↓
dashboardApi.ts + DashboardPage real API

Current OIDC foundation
↓
Phase 7 auth hardening and route protection
↓
Protect user-specific areas

Core functionality
↓
Phase 8 UX polish/mobile consistency
```

Where Phase 7 authentication can be deferred:

- It can be partially deferred while building read-only catalog pages and development-only flows.
- It becomes blocking for reliable user-specific flows: workout sessions, food logs, profile, dashboard, and private workout plans.
- API calls may work during development when already logged in, but Lovable must not treat that as production-grade route protection.

---

# Roadmap Priority Matrix

## P0 — Core functional work

- Phase 4.1: verify WorkoutSession contracts.
- Phase 4.2: create `workoutSessionsApi.ts` after verification.
- Phase 4.3–4.6: active workout, start workout, current exercise, and set management using real APIs.
- Phase 5.1–5.5: verify food contracts, create `foodsApi.ts`, implement real FoodLog page/search/add/edit.
- Phase 6.1–6.3: completed profile verification, `profileApi.ts`, and real ProfilePage integration.

## P1 — Production readiness

- Phase 4.7–4.9: session exercise management, finish/cancel, business-rule error handling.
- Phase 6.4: dashboard API contract/module completed. Phase 6.5: real DashboardPage completed. Phase 6.6: missing-profile/incomplete-profile fallback completed. Phase 7 is next.
- Phase 7: OIDC hardening, auth state UX, protected routes, expired-token handling, backend auth/CORS/OpenIddict verification.
- Consistent handling of backend business errors for user-specific aggregates.

## P2 — UX polish

- Phase 8 responsive audit.
- Form UX consistency.
- Navigation polish.
- Loading/error/empty consistency.
- Neo-Brutalist normalization.
- Optional component/style cleanup only where it reduces duplication or prevents UI bugs.

---

# Backend Verification Checklist

| Domain | Must Verify | Why |
| --- | --- | --- |
| WorkoutSession | `WorkoutSessionDto`, `CreateWorkoutSessionDto`, status enum meanings, active-session response behavior, start-from-plan behavior | Required before building `workoutSessionsApi.ts` and Active Workout UI. |
| WorkoutSessionExercise | `WorkoutSessionExerciseDto`, `AddWorkoutSessionExerciseDto`, `UpdateWorkoutSessionExerciseDto`, status enum meanings, add/update/remove constraints | Required before adding exercise management inside sessions. |
| ExerciseSet | `ExerciseSetDto`, `AddExerciseSetDto`, `UpdateExerciseSetDto`, complete/uncomplete endpoints and supported fields | Required before rendering set forms; local OpenAPI supports reps/weight/rpe/note, not duration/distance. |
| FoodProduct | `FoodProductDto`, search query fields, barcode lookup behavior, product source/verified/active semantics | Required before product search and barcode UI. |
| FoodLog | `FoodLogDto`, create/update DTOs, `FoodUnit`, `MealType`, date filtering, daily summary behavior | Required before real food log page and editor. |
| UserProfile | `UserProfileDto`, `UpdateUserProfileDto`, `Gender`, `FitnessGoal`, missing-profile behavior | Verified and implemented in Phase 6.1–6.3. |
| Dashboard | `DailyDashboardDto`, nutrition/workout summary DTOs, date query behavior, missing profile/log behavior | Verified and implemented in Phase 6.4–6.6. |
| Authentication/OpenIddict | SPA client, redirect/logout URIs, scopes, code+PKCE, CORS, catalog authorization decision | Required before production-grade React route protection and reliable user-specific flows. |

Actual local references available:

- `react/src/api/openapi.json`
- `react/src/api/httpClient.ts`
- `react/src/api/exercisesApi.ts`
- `react/src/api/workoutPlansApi.ts`
- `react/src/auth/authService.ts`

---

# Planned API Modules

## `workoutSessionsApi.ts`

Purpose:

- Encapsulate WorkoutSession DTOs, enums, query types, and endpoint functions.

Existing equivalent:

- None. Use `workoutPlansApi.ts` as the closest pattern.

Backend verification:

- Required for all session DTOs/enums/endpoints and business-state behavior.

Consumers:

- `WorkoutPage`, future `ActiveWorkoutPage`, workout session exercise/set editors if created.

Notes:

- Do not add duration/distance set fields unless backend DTOs support them.
- Keep all session lifecycle calls centralized in this module.

## `foodsApi.ts`

Purpose:

- Encapsulate FoodProduct, FoodLog, barcode lookup, nutrition summary DTOs/enums, and endpoint functions.

Existing equivalent:

- None.

Backend verification:

- Required for `FoodUnit`, `MealType`, product search query, barcode lookup, and log create/update fields.

Consumers:

- `FoodLogPage`, future food search/add/editor components.

Notes:

- Keep FoodProduct catalog/source data separate from FoodLog user-specific entries.

## `profileApi.ts`

Purpose:

- Encapsulate current user profile get/update APIs and DTOs/enums.

Existing equivalent:

- None.

Backend verification:

- Completed for profile fields, enum labels, timezone support, and create-on-read missing-profile behavior.

Consumers:

- `ProfilePage`, possibly `DashboardPage` if dashboard needs profile fallback/CTA.

Notes:

- ProfilePage now matches the verified local profile contract; keep dashboard implementation driven by its own DTOs.

## `dashboardApi.ts`

Purpose:

- Encapsulate dashboard daily/today/nutrition/workout summary APIs.

Existing equivalent:

- None.

Backend verification:

- Completed for daily dashboard DTOs, date query format, and endpoint operations. Missing-data UI semantics remain for DashboardPage integration.

Consumers:

- `DashboardPage`.

Notes:

- The API module is implemented; DashboardPage should consume backend summaries instead of reproducing business calculations.

---

# Business Rules the Frontend Must Respect

| Rule | Status | Frontend Handling |
| --- | --- | --- |
| Backend is authoritative for ownership/authorization. | CONFIRMED by project architecture | UI may hide/disable controls, but backend must enforce. |
| Workout plan archived state should prevent editing. | CONFIRMED by existing `WorkoutPlanDetailPage` UX and backend-oriented flow | Keep edit/add controls disabled/hidden when `isArchived`; backend remains authoritative. |
| Workout plan exercises have ordered `orderIndex`. | CONFIRMED by existing `workoutPlansApi.ts` and UI reorder flow | UI sends full reorder payload and disables impossible up/down moves. |
| One active workout session per user. | NEEDS BACKEND VERIFICATION | If backend returns business error, show clear message; do not enforce only client-side. |
| Completed/cancelled workout sessions cannot be modified. | NEEDS BACKEND VERIFICATION | Disable editing based on verified status enum; backend remains authoritative. |
| Completed/cancelled session read-only UI. | PLANNED / NEEDS BACKEND VERIFICATION | Requires enum mapping before UI implementation. |
| User can operate only own workout sessions, plans, food logs, and profile. | NEEDS BACKEND VERIFICATION for each aggregate, expected ABP behavior | Route/UI guards are UX only; backend authorization must enforce. |
| FoodProduct is catalog/source nutrition data. | CONFIRMED by local OpenAPI shape | Do not treat product records as user-specific logs. |
| FoodLog is user-specific logged consumption. | CONFIRMED by local OpenAPI `FoodLogDto.userId` | Food log UI should require authenticated user-specific API calls. |
| Dashboard values should come from backend summaries. | CONFIRMED by dashboard contracts | Display backend totals; only derive presentation-only percentage/labels in the UI. |
| Profile may be missing/incomplete. | CONFIRMED create-on-read; completeness is inferred from nullable fields | Show the dashboard profile CTA and keep null fields safe; backend remains authoritative. |

---

# Planned User Flows

## Start and complete workout

Implemented:

- Workout plan detail exists.
- Workout page placeholder exists.

Partial:

- No start-workout button/API integration yet.

Missing:

- `workoutSessionsApi.ts`, active session page, current exercise UI, complete/cancel flow, readonly completed/cancelled behavior.

## Add/edit workout set

Implemented:

- Nothing in frontend.

Partial:

- Local OpenAPI contains set endpoints and DTOs.

Missing:

- Set list UI, add form, update form, complete/uncomplete controls, remove control.

## Add/remove exercise during session

Implemented:

- Plan exercise picker patterns exist.

Partial:

- Local OpenAPI contains session exercise add/update/remove endpoints.

Missing:

- Session-specific picker/management UI and DTO mapping.

## Log food

Implemented:

- Food page route and placeholder UI.

Partial:

- Local OpenAPI contains food product/log endpoints.

Missing:

- Food API module, date selection, summary, product search, add-log form.

## Edit food log

Implemented:

- Nothing in frontend.

Partial:

- Local OpenAPI contains update/delete endpoints.

Missing:

- Food log editor UI and mutation handling.

## Update profile

Implemented:

- Real profile load/save form using the verified API contract.
- Login/logout buttons and null-safe optional fields.

Partial:

- Authentication route protection remains a Phase 7 concern.

Missing:

- None for Phase 6; profile completeness CTA is surfaced from the dashboard.

## View dashboard

Implemented:

- Real dashboard summaries with selected-date loading.
- Loading, API error, no-data, and incomplete-profile states.

Partial:

- Authentication route protection remains.

Missing:

- None for Phase 6.

## Login and access protected page

Implemented:

- OIDC redirect login/logout and bearer token attachment.

Partial:

- Backend handles unauthorized requests; frontend displays API errors where pages catch them.

Missing:

- Auth context, protected routes, expired-token handling, user state UI.

---

# Phase-by-Phase Definition of Done

| Phase | Definition of Done |
| --- | --- |
| Phase 4 — Workout Session | Verified session DTOs/enums; `workoutSessionsApi.ts`; active session loading/no-active empty state; start from plan; current exercise display; next/previous/skip; set add/update/remove/complete/uncomplete; add/remove session exercises if supported; complete/cancel confirmations; read-only terminal states; backend business errors surfaced clearly. |
| Phase 5 — Food Product + Food Log | Verified food DTOs/enums; `foodsApi.ts`; real date-based FoodLogPage; daily summary; product search/barcode lookup if supported; add/update/delete log entries; empty/error states for no entries/product not found/barcode failure. |
| Phase 6 — User Profile + Dashboard | Verified profile/dashboard DTOs; `profileApi.ts`; real ProfilePage load/save; missing-profile fallback; `dashboardApi.ts` if backend support confirmed; DashboardPage uses real API data and handles missing/no-data/error states. |
| Phase 7 — React SPA Authentication | Verified OpenIddict SPA client/scopes/redirects/CORS; current OIDC flow preserved; auth state available where needed; token handling centralized; protected user-specific routes; expired/unauthorized handling; no custom auth endpoints or frontend secrets. |
| Phase 8 — UX Polish + Mobile Quality | Responsive audit complete; forms consistently disable during submit; backend errors visible; shared state components used; navigation/back behavior consistent; Neo-Brutalist visual language normalized; no broad redesign. |

---

# Recommended Implementation Sequence

## 1. Verify WorkoutSession API contracts

Goal:

- Confirm actual DTOs, enums, endpoints, and no-active/terminal-state behavior.

Why now:

- Phase 4 is the next core workflow and depends entirely on backend contracts.

Backend prerequisite:

- Live Swagger/OpenAPI available from backend.

Frontend prerequisite:

- Existing `openapi.json`, `apiRequest()`, workout plan pages.

Definition of done:

- Documented DTO/enums/endpoints for `workoutSessionsApi.ts`; no guessed fields.

## 2. Create `workoutSessionsApi.ts`

Goal:

- Centralize session DTOs and API calls.

Why now:

- Prevents raw fetch calls inside workout session UI.

Backend prerequisite:

- Step 1 completed.

Frontend prerequisite:

- Follow `workoutPlansApi.ts` conventions.

Definition of done:

- TypeScript module compiles and exposes only verified operations.

## 3. Build active workout read flow

Goal:

- Show current active session or EmptyState with CTA to choose a plan.

Why now:

- Establishes session state before mutations.

Backend prerequisite:

- Active endpoint behavior verified.

Frontend prerequisite:

- `workoutSessionsApi.ts`.

Definition of done:

- Real API data renders with loading/error/empty states.

## 4. Add start workout from plan detail

Goal:

- Start a session using selected workout plan and navigate to active workout.

Why now:

- Connects existing WorkoutPlan aggregate to Session aggregate.

Backend prerequisite:

- `CreateWorkoutSessionDto` and start-from-plan behavior verified.

Frontend prerequisite:

- Active workout route/page exists.

Definition of done:

- Start button calls API, handles already-active-session business error, navigates correctly on success.

## 5. Implement current exercise and set management

Goal:

- Make active workout usable during training.

Why now:

- This is the core workout execution loop.

Backend prerequisite:

- Current exercise and set endpoints verified.

Frontend prerequisite:

- Active workout page and session API module.

Definition of done:

- User can view current exercise, manage sets, complete/uncomplete sets, and navigate exercises with backend state updates.

## 6. Implement finish/cancel and read-only terminal states

Goal:

- Complete the session lifecycle.

Why now:

- Prevents editing invalid session states and closes the core flow.

Backend prerequisite:

- Status enum meanings and complete/cancel behavior verified.

Frontend prerequisite:

- Active workout mutation UI exists.

Definition of done:

- Confirmations exist, terminal states are read-only, backend errors display clearly.

## 7. Verify and implement food APIs and FoodLogPage

Goal:

- Replace placeholder Food Log with real daily logging.

Why now:

- Food tracking is the next major aggregate after workout execution.

Backend prerequisite:

- Food DTOs, enums, product search, barcode behavior verified.

Frontend prerequisite:

- Existing shared form/state components.

Definition of done:

- Date-based entries, daily summary, product search/add, update/delete are real API-backed.

## 8. Verify and implement Profile API

Goal:

- Replace profile placeholder with real settings/profile form.

Why now:

- Dashboard targets and user-specific UX likely depend on profile data.

Backend prerequisite:

- `UserProfileDto`, `UpdateUserProfileDto`, missing-profile behavior verified.

Frontend prerequisite:

- Auth flow works enough to call user-specific API.

Definition of done:

- Profile loads/saves real data and handles null/missing fields.

## 9. Verify and implement Dashboard API

Goal:

- Replace static dashboard with backend summaries.

Why now:

- Dashboard should summarize completed food/workout/profile work.

Backend prerequisite:

- Dashboard DTOs and missing-data behavior verified.

Frontend prerequisite:

- Food/profile/workout data flows are at least partially real.

Definition of done:

- Dashboard shows real data and meaningful no-data/missing-profile states.

## 10. Harden React SPA auth and route protection

Goal:

- Make user-specific routes reliable and production-oriented.

Why now:

- Once more user-specific flows exist, relying only on backend error pages is poor UX.

Backend prerequisite:

- OpenIddict SPA client/scopes/CORS verified.

Frontend prerequisite:

- Current `authService.ts` and `httpClient.ts` remain centralized.

Definition of done:

- Protected routes, auth state UX, expired-token handling, and centralized token behavior are implemented without custom auth hacks.

## 11. UX polish and mobile audit

Goal:

- Normalize all implemented flows.

Why now:

- Polish after core flows avoids reworking placeholder UI repeatedly.

Backend prerequisite:

- None beyond stable APIs for completed features.

Frontend prerequisite:

- Core flows implemented.

Definition of done:

- Consistent mobile behavior, states, forms, navigation, and Neo-Brutalist visuals.

---

# Lovable Rules for This Roadmap

Lovable MUST:

1. Read `LOVABLE_CONTEXT.md` before every roadmap task.
2. Work on one roadmap item at a time.
3. Inspect existing code before creating files.
4. Verify backend contracts before implementing API-dependent UI.
5. Reuse existing HTTP client.
6. Reuse existing API conventions.
7. Reuse existing UI components.
8. Preserve the Neo-Brutalist visual language already present.
9. Surface backend business errors clearly.
10. Keep backend as authority for business rules.
11. Keep completed/cancelled resources read-only when backend state requires it.
12. Keep user-specific features compatible with future/current OIDC authentication.
13. Avoid unrelated refactors.
14. Do not introduce Supabase.
15. Do not create a second backend.
16. Do not invent endpoints.
17. Do not invent DTO fields.
18. Do not invent enum values.
19. Do not hardcode fake production data.
20. Do not add large libraries without a demonstrated need.

---

# Next Task for Lovable

Recommended task:

- Verify WorkoutSession API contracts before implementing the Active Workout UI.

Why:

- Workout Plan is already the strongest implemented aggregate.
- Phase 4 is the next roadmap phase.
- Local OpenAPI contains WorkoutSession endpoints and schemas, but no frontend session API module exists yet.
- Implementing UI before verifying enum meanings, no-active behavior, and terminal-state rules would cause rework.

Files to inspect:

- `react/src/api/openapi.json`
- `react/src/api/httpClient.ts`
- `react/src/api/workoutPlansApi.ts`
- `react/src/features/workoutSessions/WorkoutPage.tsx`
- `react/src/features/workoutPlans/WorkoutPlanDetailPage.tsx`
- `react/src/App.tsx`

Backend contracts to verify:

- `FitLogs.Workouts.WorkoutSessionDto`
- `FitLogs.Workouts.WorkoutSessionExerciseDto`
- `FitLogs.Workouts.ExerciseSetDto`
- `FitLogs.Workouts.CreateWorkoutSessionDto`
- `FitLogs.Workouts.AddWorkoutSessionExerciseDto`
- `FitLogs.Workouts.UpdateWorkoutSessionExerciseDto`
- `FitLogs.Workouts.AddExerciseSetDto`
- `FitLogs.Workouts.UpdateExerciseSetDto`
- `FitLogs.Workouts.WorkoutSessionStatus`
- `FitLogs.Workouts.WorkoutSessionExerciseStatus`
- `GET /api/app/workout-session/active` no-active response behavior
- Business errors for already-active session and completed/cancelled modification

Expected frontend changes after verification:

- Add `react/src/api/workoutSessionsApi.ts`.
- Add or update workout session page route(s).
- Replace `WorkoutPage` placeholder with active-session loading/empty/error/success states.
- Add start-workout action from plan detail only after `CreateWorkoutSessionDto` behavior is confirmed.

Definition of done:

- A documented/typed API contract for workout sessions exists in frontend code.
- No invented fields or enum labels are used.
- Active workout implementation can proceed with low rework risk.

What NOT to change:

- Do not modify backend.
- Do not redesign the whole workout UI.
- Do not create fake session data.
- Do not bypass `apiRequest()`.
- Do not add a new state management or UI library.

# 22. Instructions for Lovable

You are continuing an existing frontend, not creating a new application.

Before modifying anything:

1. Read this file.
2. Inspect existing files related to the requested feature.
3. Reuse existing components and patterns.
4. Preserve routing and application architecture.
5. Use the existing API layer under `/react/src/api`.
6. Treat ABP backend APIs and DTO contracts as the source of truth.
7. Ask for or inspect Swagger/OpenAPI definitions rather than inventing endpoints or fields.
8. Make incremental changes.
9. Keep TypeScript strict and consistent with existing types.
10. Preserve the current Neo-Brutalist design language.
11. Avoid unrelated refactors.
12. Clearly identify files modified for each task.
13. Do not change backend code unless explicitly requested.
14. Do not add new dependencies unless there is a concrete need and the user approves.
15. Do not replace working code with a new architecture.

---

# 23. Quick Context for AI

## AI QUICK CONTEXT

CURRENT STATE

- App: FitLogs fitness tracking frontend.
- Frontend root: `/Volumes/Kingsman/Projects/Personal/FitLogs/FitLogs/react`.
- Stack: React 19, TypeScript 6, Vite 8, react-router-dom 7, oidc-client-ts, Oxlint.
- No Tailwind, no Redux, no React Query, no Axios, no form/validation library, no UI library.
- Backend: ASP.NET Core + ABP Framework at `https://localhost:44377` in dev.
- Backend is source of truth for auth, authorization, validation, business logic, persistence, DTOs, enums.
- Entrypoint: `src/main.tsx`; routes: `src/App.tsx`; layout: `PageShell` + `BottomNav`.
- API layer: handwritten modules in `src/api`; all calls should go through `apiRequest()`.
- Current API modules: `exercisesApi.ts`, `workoutPlansApi.ts`; planned modules must follow this pattern.
- `src/api/openapi.json` exists as a local Swagger/OpenAPI reference, not generated TypeScript code.
- Auth: `authService.ts` uses `oidc-client-ts`, localStorage user store, OIDC code flow, bearer token attachment in `httpClient.ts`.
- Missing auth pieces: auth context, protected routes, expired-token/silent-renew handling, permission checks.
- Design: mobile-first Neo-Brutalism in `src/styles/global.css`; preserve `min(100%, 430px)` shell, black borders, hard shadows, bold colors.
- Implemented most: Exercise Library and Workout Plan aggregate.
- Partial/static: Dashboard, FoodLog, Profile, Workout Session.

NEXT PHASE

- Phase 4 is next: Workout Session Aggregate.
- Recommended next task: verify WorkoutSession API contracts before implementing Active Workout UI.
- Do not build session UI around guessed DTOs or enum meanings.

ROADMAP THROUGH PHASE 8

- Phase 4: Workout Session API/module, active workout, start from plan, current exercise, set management, finish/cancel, business-rule errors.
- Phase 5: FoodProduct + FoodLog API/module, food search/barcode if supported, date-based food log, add/edit/delete entries, daily summary.
- Phase 6: UserProfile + Dashboard API/modules, real profile form, real dashboard cards, missing-profile fallback.
- Phase 7: React SPA auth hardening around current OIDC/OpenIddict direction, protected user-specific routes, centralized token/expired handling.
- Phase 8: mobile UX polish, form consistency, navigation, shared loading/error/empty states, Neo-Brutalist normalization.

BACKEND DEPENDENCIES

- Verify live Swagger/OpenAPI before implementing backend-dependent UI.
- Local OpenAPI shows WorkoutSession, FoodLog/FoodProduct, UserProfile, and Dashboard endpoints, but frontend modules do not exist for these yet.
- Confirm enum meanings before rendering labels.
- Confirm no-active-session, missing-profile, barcode failure, and completed/cancelled session behavior.
- Backend remains authoritative; frontend guards are UX only.

AUTH TARGET

- Target production strategy: OIDC Authorization Code Flow + PKCE, if confirmed compatible with backend OpenIddict client.
- Current dependency `oidc-client-ts` is already installed and used.
- Verify OpenIddict SPA client `FitLogs_App`, redirect/logout URIs, scopes, grant/PKCE settings, and CORS.
- Do not build custom username/password auth or invent auth endpoints.

DESIGN CONSTRAINTS

- Reuse `NeoCard`, `NeoButton`, `NeoInput`, `NeoSelect`, `EmptyState`, `LoadingState`, `ErrorState`, `PageShell`, `BottomNav`.
- Preserve global CSS Neo-Brutalist style; no Tailwind/shadcn/MUI rewrite.
- Make incremental changes; no broad architecture rewrite.

NEXT RECOMMENDED TASK

- Verify WorkoutSession contracts and document exact DTOs/enums/endpoints for `workoutSessionsApi.ts`.
- Files to inspect: `src/api/openapi.json`, `src/api/httpClient.ts`, `src/api/workoutPlansApi.ts`, `src/features/workoutSessions/WorkoutPage.tsx`, `src/features/workoutPlans/WorkoutPlanDetailPage.tsx`, `src/App.tsx`.
- Expected next frontend change after verification: create `src/api/workoutSessionsApi.ts` and then build active workout read flow.
- Do not modify backend, create fake data, invent endpoints/fields/enums, bypass `apiRequest()`, or add large libraries without a demonstrated need.
