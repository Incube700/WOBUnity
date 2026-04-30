# Ricochet Tanks - Technical Status

**Synced:** 2026-04-30
**Branch observed:** `feature/ricochet-tanks-prototype`
**Prototype root:** `Assets/_Project/RicochetTanks/`  
**Main flow:** `MainMenu.unity -> RicochetTanks_Demo.unity`

This file records what appears implemented in code/assets and what still needs manual Unity verification. It should not be used to invent completion: if Play Mode was not checked, the item stays **Needs Manual Unity Check**.

## 2026-04-30 Single-Player Flow Update

Implemented in code/assets:

- MainMenu is first in build settings and has runtime `Start Game`, `Statistics`, and `Exit` controls.
- `Start Game` starts a fresh local session and loads `RicochetTanks_Demo`.
- Gameplay now supports a simple local round series: `RoundsToWin = 3`, `RoundBreakSeconds = 5`.
- After each round, the HUD shows round result, score, and next-round countdown.
- After one side reaches 3 round wins, the full match/session finishes and the HUD shows `Restart` and `Exit to MainMenu`.
- Round-to-round map support exists through `LocalSessionConfig`, but all default round scenes currently reuse `RicochetTanks_Demo`.
- Local statistics persist through PlayerPrefs JSON key `RicochetTanks.PlayerStatistics.v1`.
- Statistics are counted by `StatisticsTracker` from `SandboxGameplayEvents` and saved only after full session finish.
- No production analytics or external telemetry is used.

Needs Manual Unity Check:

- Complete first-to-3 flow in Play Mode.
- Return to MainMenu and verify statistics/recent games update.
- Restart a finished match and confirm score resets to `0 : 0`.
- Exit to MainMenu and confirm no duplicate subscriptions/UI are created.
- Android/mobile controls still need device verification after this session-flow change.

## Current Playable Prototype Status

Implemented in code/assets:

- `RicochetTanks_Demo.unity` exists and is the current main playable scene.
- Player tank and enemy dummy tank references exist in the scene/code flow.
- Arena, floor, walls/obstacles, spawn points, camera rig, gameplay canvas, and combat feedback root are expected scene objects.
- Top-down orthographic camera is configured through `CameraConfig`.
- Desktop controls are implemented through `DesktopInputReader`, `PlayerTankController`, `TankMovement`, and `TurretAiming`.
- Shooting is implemented through `TankShooter` and `ProjectileFactory`.
- Projectile movement is implemented by a thin `Projectile` runner and projectile system pipeline.
- Wall/obstacle ricochet is implemented through previous-position sphere cast, hit normal reflection, position correction, bounce count, speed reduction, and damage reduction.
- Tank armor hit resolution is implemented through `ProjectileHitDetectionSystem`, `HitResolver`, `TankArmor`, and `ArmorHitInfo`.
- Armor zones are `Front`, `Side`, and `Rear` from tank-local hit normal.
- Effective armor uses `effectiveArmor = armor / max(cos(angle), safeMinCos)`.
- Damage/HP/death are implemented through `TankHealth`.
- Round win, full-session finish, restart, and menu-exit flow are implemented through `SandboxMatchController`.
- Screen HUD exists through `SandboxHudView` and `SandboxHudPresenter`.
- Combat feedback exists through world-space HP bars and floating hit text.
- Projectile trail exists in code/assets.

Manually checked in Unity:

- Scene opens and runs cleanly in Play Mode.
- Current scene size/layout/material state is correct in Unity.
- Player and enemy spawn where expected.
- Camera is strict top-down and frames the playable arena.
- Tank movement feels controlled and does not rotate by itself.
- Turret aims at the mouse ground point and does not spin randomly.
- Projectile fires from the muzzle and follows turret direction.
- Wall/obstacle ricochet works in Play Mode.
- Tank ricochet/no-penetration works in Play Mode.
- HP bars shrink after damage.
- Floating hit text appears for damage, `NO PEN`, and `RICOCHET`.
- Restart does not duplicate event subscriptions, HP bars, or listeners.
- Projectile trail, hit readability, and impact feedback are visually clear.
- Current ricochet speed loss is visible enough.
- Mobile controls are visible and usable when `Input Mode` is set to `Mobile`.
- MainMenu -> RicochetTanks_Demo flow works in Editor.
- Android build launches on device.
- Mobile controls v1 work on Android with arcade left joystick movement.

Still Needs Manual Unity Check:

- Further mobile usability feedback from testers on actual devices.
- HP bar visual polish through prefab tuning if the current prototype bar needs better readability.
- Unity scene/prefab changes are saved and committed after editor-side changes.

## Current Config Values

Values below were read from current config assets. They are status, not new balance recommendations.

### ProjectileConfig

| Value | Current |
|---|---:|
| Speed | 48.8 |
| Bounce speed multiplier | 0.78 |
| Cooldown | 0.8 sec |
| Safe owner time | 0.15 sec |
| Lifetime | 8 sec |
| Min speed | 5 |
| Radius | 0.18 |
| Flight height | 0.6 |
| Spawn offset | 0.35 |
| Position correction skin | 0.01 |
| Trail time | 0.25 |
| Damage | 110 |
| Damage multiplier per bounce | 0.75 |
| Max ricochets | 3 |
| Penetration | 45 |
| Reflectable mask bits | 2305 |
| Hittable mask bits | 512 |

### PlayerTankConfig

| Value | Current |
|---|---:|
| Max HP | 100 |
| Max forward speed | 5 |
| Max reverse speed | 2.5 |
| Acceleration | 10 |
| Brake deceleration | 14 |
| Coast deceleration | 6 |
| Turn speed | 140 deg/sec |
| Turn speed at low velocity | 80 deg/sec |
| Turret rotation speed | 360 deg/sec |
| Input dead zone | 0.05 |
| Front armor | 50 |
| Side armor | 40 |
| Rear armor | 10 |
| Auto ricochet angle | 50.3 deg |

### EnemyTankConfig

| Value | Current |
|---|---:|
| Max HP | 300 |
| Front armor | 50 |
| Side armor | 40 |
| Rear armor | 10 |
| Auto ricochet angle | 60 deg |

Note: player and enemy `AutoRicochetAngle` are currently asymmetric in the assets. This may be intentional or an unverified tuning state. Needs Manual Unity Check / design confirmation.

### CameraConfig

| Value | Current |
|---|---:|
| Orthographic | true |
| Local position | `(0, 14, 0)` |
| Local rotation | `(90, 0, 0)` |
| Orthographic size | 10.08 |
| Near clip | 0.1 |
| Far clip | 50 |

### MatchConfig

| Value | Current |
|---|---|
| Playing label | `Round: Playing` |
| Player wins label | `Player Wins` |
| Enemy wins label | `Enemy Wins` |

## Combat Rules In Code

Armor angle calculation:

```text
dot = Vector3.Dot(-incomingDirection.normalized, hitNormal.normalized)
angle = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f)) * Mathf.Rad2Deg
effectiveArmor = armor / Mathf.Max(cos(angle), safeMinCos)
```

Expected default outcomes from current numbers:

- Front direct hit should not penetrate by default: penetration `45` vs front armor `50`.
- Direct side hit can penetrate: penetration `45` vs side armor `40`.
- Glancing side hit can become no-penetration/ricochet because effective armor rises with angle.
- Rear hit should penetrate: penetration `45` vs rear armor `10`.
- No-penetration/ricochet produces no damage.

## Technical Architecture

The prototype is not DOTS/Entitas. It uses feature-based MonoBehaviours, plain C# presenters/services, and a lightweight ECS-style projectile data/system pipeline.

Main roles:

- `GameplayEntryPoint` is the composition root for `RicochetTanks_Demo`.
- `GameplayEntryPoint` wires scene references, configs, and feature composition.
- `TankCompositionFactory` configures tank runtime components from scene refs/configs.
- `SandboxHudViewFactory` creates fallback HUD only when no serialized HUD view exists.
- `CombatFeedbackComposition` wires HP bars, floating hit text, and VFX presenters.
- `TankFacade` exposes tank feature components.
- `PlayerTankController` reads input and passes commands.
- `TankMovement` handles movement/inertia/hull rotation.
- `TurretAiming` handles turret pivot rotation only.
- `TankShooter` handles shooting/cooldown and delegates projectile spawn.
- `Projectile` is a thin runner/facade.
- Projectile logic is split into small systems under `Gameplay/Projectiles/Systems`.
- Combat math lives in `HitResolver` and `TankArmor`.
- Visual feedback lives in `UI/CombatFeedback`.
- Combat VFX is split behind `CombatVfxFactory` into trail, impact, death, and shot recoil helpers.
- Match/session flow lives under `Gameplay/Match`; UI only displays match state.
- Local statistics live under `Statistics` and subscribe to `SandboxGameplayEvents`.
- Screen HUD remains `SandboxHudView` + `SandboxHudPresenter`.
- `SandboxDebugVisualizer` remains debug-only.
- `SandboxSceneBuilder` is a dev/procedural fallback only; saved scenes/prefabs are the main path.

## Feature Map

| Feature | Status | Code Status | Main Files | Manual Check | Notes |
|---|---|---|---|---|---|
| Demo Scene | Done | Scene asset exists and Play Mode was checked | `RicochetTanks_Demo.unity`, `GameplayEntryPoint` | Owner verified in Unity | Main playable scene |
| Tank Movement | Done | Implemented with inertia/brake/coast/reverse | `TankMovement`, `TankConfig` | Owner verified in Unity | Desktop controls work |
| Turret Aiming | Done | Implemented mouse ground-plane aim | `PlayerTankController`, `TurretAiming` | Owner verified in Unity | Turret independent from hull |
| Shooting | Done | Implemented | `TankShooter`, `ProjectileFactory` | Owner verified in Unity | Cooldown 0.8 sec |
| Projectile Movement | Done | Implemented | `Projectile`, `ProjectileEntity`, `ProjectileMovementSystem` | Owner verified in Unity | Fast visible projectile |
| Wall/Obstacle Ricochet | Done | Implemented | `RicochetDetectionSystem`, `RicochetMoveDirectionReflectSystem` | Owner verified in Unity | Max 3 bounces |
| Tank Armor / NoPen | Done | Implemented | `HitResolver`, `TankArmor`, `ArmorHitInfo` | Owner verified in Unity | Uses effective armor |
| Tank Ricochet | Done | Implemented | `HitResolver`, projectile ricochet systems | Owner verified in Unity | Uses current ricochet request path |
| Damage / HP | Done | Implemented | `TankHealth`, `HitResolver` | Owner verified in Unity | Player 100, enemy 300 |
| Win Series / Result Flow | Needs Manual Unity Check | Implemented in code | `Gameplay/Match/SandboxMatchController`, HUD presenter | Needs first-to-3 Play Mode test | Restart/menu exit added |
| Local Statistics | Needs Manual Unity Check | Implemented in code | `Statistics/*`, `MainMenu/*` | Needs persistence/reset test | PlayerPrefs JSON, local only |
| Combat Feedback UI | Done | Implemented | `UI/CombatFeedback/*`, `WorldHealthBarPrefab` | Owner verified in Unity | HP bar prefab polish may be tuned later |
| Debug Logs | Partial | Implemented with `DebugLogConfig` and debug visualizer | `DebugLogConfig`, `SandboxDebugVisualizer`, projectile systems | Watch console volume | Log tuning still needs Unity check |
| Materials / Visual Polish | Partial | Greybox materials/prefabs exist | Prefabs, scene materials | Inspect for magenta/broken visuals | Visual polish not final |
| Main Menu | Done | Scene and UI flow exist | `MainMenu.unity`, `UI/MainMenu/*`, `SceneLoaderService` | Owner verified in Unity | Play loads demo |
| Mobile Controls | Partial | Prototype implemented | `Input/Mobile/*`, `GameplayEntryPoint` | Owner verified in Mobile mode and Android launch test | Arcade movement v1 works; tester usability feedback still needed |
| Enemy AI | Future | Enemy is dummy | `EnemyDummyTank`, `TankFacade` | N/A | No movement/aim/shoot AI |

## Demo Scene Checklist

- [x] Scene asset exists: `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity`.
- [x] Scene launches in Play Mode - owner verified in Unity.
- [x] Player and enemy spawn correctly - owner verified in Unity.
- [x] Camera top-down framing is correct - owner verified in Unity.
- [x] Tank controls feel correct - owner verified in Unity.
- [x] Shooting works in Play Mode - owner verified in Unity.
- [x] Wall/obstacle ricochet works in Play Mode - owner verified in Unity.
- [x] Tank ricochet/no-penetration works in Play Mode - owner verified in Unity.
- [x] Damage and HP update in Play Mode - owner verified in Unity.
- [x] World HP bars display and shrink - owner verified in Unity.
- [x] Floating hit text appears - owner verified in Unity.
- [x] Win/lose triggers - owner verified in Unity.
- [x] Restart resets round - owner verified in Unity.
- [x] Restart does not duplicate subscriptions or HP bars - owner verified in Unity.

## Known Issues / Risks

- Materials may still need visual polish if any magenta/broken materials appear in Unity.
- Debug logs such as `[SHOT]`, `[HIT]`, `[BOUNCE]`, and `[ARMOR]` can spam the Console when enabled in `DebugLogConfig`.
- Scene and prefab changes made inside Unity must be saved and committed manually.
- HP bar visuals may need later prefab tuning for final readability.
- Enemy AI is not implemented; enemy is still a dummy target.
- Android APK launches and mobile controls v1 work; final mobile UX still needs tester feedback.
- Generated `graphify-out/GRAPH_REPORT.md` is stale and should not override `AI_CONTEXT_GRAPH.md`.
- Existing uncommitted Unity scene/config changes should be reviewed before commit.
- Projectile speed is currently `48.8`; keep it as the current tuning unless design asks for a balance pass.
- Network/multiplayer is future research only.
