# Codex Handoff

## Playable Scenes

- `Assets/_Project/RicochetTanks/Scenes/MainMenu.unity` is the first enabled build scene.
- `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity` is the playable prototype scene loaded by `Start Game`.
- `Assets/_Project/RicochetTanks/Scenes/Sandbox.unity` exists but is disabled in build settings.
- `Assets/_Project/Features/UI/Scenes/UISandbox.unity` is experimental UI-only and must stay separate from the playable flow.

## Current Working Features

- MainMenu runtime UI has `Start Game`, `Statistics`, and `Exit`.
- MainMenu `Statistics` shows local all-time counters and recent matches from PlayerPrefs JSON.
- Demo gameplay now runs a local win-series match: first side to 3 round wins finishes the full session.
- Between rounds, the current arena scene is reloaded after a 5 second break while score/statistics stay in a lightweight local session service.
- The final gameplay HUD shows result controls for `Restart` and `Exit to MainMenu`.
- Combat, armor, ricochet formulas, projectile behavior, tank movement, mobile controls, laser aim, and tank visuals were not intentionally changed.

## Experimental Features Not Connected

- `Assets/_Project/Features/UI` remains a future UI/PvP shell using mock services.
- No real networking, PvP, lobby backend, relay, Photon, Netcode, ranking, chat, or account system is implemented.
- Experimental UI is not referenced by gameplay/combat/tank classes and is not replacing the current playable HUD.

## Local Statistics

- Storage key: `RicochetTanks.PlayerStatistics.v1`.
- Storage format: local PlayerPrefs JSON via `PlayerStatisticsRepository`.
- Saves happen only after a full best-of-3-style session finishes, not after each shot or hit.
- Recent history stores the last 10 completed sessions with result, round score, shots, hits, accuracy, damage dealt/taken, and date/time.
- This is not production analytics and sends no external telemetry.

## Known Bugs / Risks

- Round-to-round map selection is only config-ready; all configured defaults currently reuse `RicochetTanks_Demo`.
- Round reset uses scene reload instead of in-place tank/projectile reset to avoid disturbing current movement inertia internals.
- No enemy AI exists yet; the enemy remains a dummy tank.
- Manual Play Mode verification is still required after compile: desktop controls, Android/mobile controls, shooting, ricochet/armor, result flow, restart, and menu exit.

## Last Codex Session Changes

- Added local session config/service for first-to-3 round series.
- Added local statistics data, repository, tracker, and composition.
- Wired statistics through `SandboxGameplayEvents` and `GameplayEntryPoint`.
- Extended gameplay HUD with score/status/result controls and exit-to-menu callback.
- Extended MainMenu fallback UI with statistics panel and reset.
- Added enemy AI FSM plan document; no AI implementation.

## Files Needing Manual Review

- `Assets/_Project/RicochetTanks/Scripts/Infrastructure/Bootstrap/GameplayEntryPoint.cs`
- `Assets/_Project/RicochetTanks/Scripts/Gameplay/Match/SandboxMatchController.cs`
- `Assets/_Project/RicochetTanks/Scripts/Gameplay/Match/LocalMatchSessionService.cs`
- `Assets/_Project/RicochetTanks/Scripts/Statistics/*`
- `Assets/_Project/RicochetTanks/Scripts/UI/MainMenu/*`
- `Assets/_Project/RicochetTanks/Scripts/UI/Sandbox/*`
- `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity`

## Next 5 Safe Tasks

1. Manually play `MainMenu -> Start Game -> RicochetTanks_Demo` on desktop.
2. Finish a full first-to-3 session and verify final result buttons.
3. Return to MainMenu and verify statistics persisted, then reset statistics.
4. Reopen the project/app and confirm statistics still load.
5. Test Android/mobile controls without changing movement or input tuning.

## What Not To Touch Next

- Do not implement networking or real PvP yet.
- Do not alter ricochet, armor, damage, projectile, movement, or mobile arcade control formulas.
- Do not replace the current playable HUD with the experimental UI.
- Do not add new maps until the single-arena session loop is manually verified.
- Do not add ranked mode, chat, accounts, economy, or matchmaking backend.
