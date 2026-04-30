# Codex Handoff

## Playable Scenes

- `Assets/_Project/RicochetTanks/Scenes/MainMenu.unity` is the first enabled build scene.
- `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity` is the playable prototype scene loaded by the Main Menu Play button.
- `Assets/_Project/RicochetTanks/Scenes/Sandbox.unity` exists but is disabled in build settings.
- `Assets/_Project/Features/UI/Scenes/UISandbox.unity` is experimental UI-only and must stay separate from the playable flow.

## Working Features

- Main Menu creates a simple runtime UI and loads `RicochetTanks_Demo`.
- Demo scene contains the tank duel prototype with arena, player tank, enemy dummy, shooting, ricochets, armor/damage resolution, HP HUD, match finish, and restart.
- Mobile arcade controls are present through the mobile input reader/control view path.
- Laser aim is visual-only and should not affect damage, armor, ricochet, projectile movement, or combat formulas.

## Experimental Features Not Connected Yet

- `Assets/_Project/Features/UI` contains a future UI MVP shell: theme config, base UI prefabs, View/Presenter classes, interfaces, mock services, and `UISandbox`.
- The experimental UI uses mock lobby/room/connection services only.
- No real networking, PvP, lobby backend, relay, Photon, Netcode, ranking, chat, or account system is implemented.
- Experimental UI is not referenced by gameplay/combat/tank classes and is not replacing the current HUD.

## Known Bugs / Risks

- This handoff pass could not run a fresh Unity batch compile because the project was already open in another Unity instance. A prior Unity batchmode import/generation pass completed successfully after compiling scripts.
- No automated gameplay smoke test was run in Play Mode during this handoff.
- Manual review is recommended for LaserAim configuration and any ProjectSettings changes before a release build.

## Last Codex Session Changes

- Added isolated experimental UI architecture under `Assets/_Project/Features/UI`.
- Added `UIThemeConfig`, UI base prefabs, and `UISandbox`.
- Kept the playable MainMenu -> RicochetTanks_Demo flow unchanged.
- Removed tracked Unity recovery scene files from `Assets/_Recovery`.
- Confirmed no abandoned Netcode/Photon/Relay package dependency is present in `Packages/manifest.json` or `Packages/packages-lock.json`.

## Files Needing Manual Review

- `Assets/_Project/RicochetTanks/Scripts/Infrastructure/Bootstrap/GameplayEntryPoint.cs`
- `Assets/_Project/RicochetTanks/Scripts/Infrastructure/Composition/TankCompositionFactory.cs`
- `Assets/_Project/RicochetTanks/Scripts/Gameplay/Tanks/Presentation/LaserAimView.cs`
- `ProjectSettings/PackageManagerSettings.asset`
- `Assets/_Project/Features/UI/Scenes/UISandbox.unity`
- `Assets/_Project/Features/UI/Prefabs/*.prefab`

## Next 5 Safe Tasks

1. Close all Unity instances, then run a clean Unity batchmode compile.
2. Manually play `MainMenu -> RicochetTanks_Demo` on desktop.
3. Manually test Android/mobile controls in Editor/device build.
4. Open `UISandbox` and verify mock UI navigation without touching gameplay scenes.
5. Review LaserAim visual settings and confirm they remain presentation-only.

## What Not To Touch Next

- Do not implement networking or real PvP yet.
- Do not alter ricochet, armor, damage, projectile, or movement formulas.
- Do not replace the current playable HUD with the experimental UI.
- Do not add `UISandbox` as the first build scene.
- Do not add ranked mode, chat, accounts, economy, or matchmaking backend.
