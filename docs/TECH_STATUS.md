# Ricochet Tanks - Technical Status

**Synced:** 2026-04-28  
**Branch observed:** `main`
**Prototype root:** `Assets/_Project/RicochetTanks/`  
**Main scene:** `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity`

This file records what appears implemented in code/assets and what still needs manual Unity verification. It should not be used to invent completion: if Play Mode was not checked, the item stays **Needs Manual Unity Check**.

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
- Win/lose/restart request flow is implemented through `SandboxMatchController`.
- Screen HUD exists through `SandboxHudView` and `SandboxHudPresenter`.
- Combat feedback exists through world-space HP bars and floating hit text.
- Projectile trail exists in code/assets.

Needs Manual Unity Check:

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
- Unity scene/prefab changes are saved and committed after editor-side changes.
- Projectile trail, hit readability, and impact feedback are visually clear.
- Current ricochet speed loss is visible enough.
- Minimal VFX/recoil/mobile controls are not implemented yet.

## Current Config Values

Values below were read from current config assets. They are status, not new balance recommendations.

### ProjectileConfig

| Value | Current |
|---|---:|
| Speed | 22 |
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
- `GameplayEntryPoint` wires scene references, configs, tank components, HUD, match controller, projectile factory, and combat feedback.
- `TankFacade` exposes tank feature components.
- `PlayerTankController` reads input and passes commands.
- `TankMovement` handles movement/inertia/hull rotation.
- `TurretAiming` handles turret pivot rotation only.
- `TankShooter` handles shooting/cooldown and delegates projectile spawn.
- `Projectile` is a thin runner/facade.
- Projectile logic is split into small systems under `Gameplay/Projectiles/Systems`.
- Combat math lives in `HitResolver` and `TankArmor`.
- Visual feedback lives in `UI/CombatFeedback`.
- Screen HUD remains `SandboxHudView` + `SandboxHudPresenter`.
- `SandboxDebugVisualizer` remains debug-only.

## Feature Map

| Feature | Status | Code Status | Main Files | Manual Check | Notes |
|---|---|---|---|---|---|
| Demo Scene | Needs Manual Check | Scene asset exists | `RicochetTanks_Demo.unity`, `GameplayEntryPoint` | Open scene and press Play | Main playable scene |
| Tank Movement | Needs Manual Check | Implemented with inertia/brake/coast/reverse | `TankMovement`, `TankConfig` | Test W/S/A/D | Current feel needs Unity check |
| Turret Aiming | Needs Manual Check | Implemented mouse ground-plane aim | `PlayerTankController`, `TurretAiming` | Move mouse while hull turns | Turret independent from hull |
| Shooting | Needs Manual Check | Implemented | `TankShooter`, `ProjectileFactory` | LMB/Space fires from muzzle | Cooldown 0.8 sec |
| Projectile Movement | Needs Manual Check | Implemented | `Projectile`, `ProjectileEntity`, `ProjectileMovementSystem` | Watch projectile path | XZ plane |
| Wall/Obstacle Ricochet | Needs Manual Check | Implemented | `RicochetDetectionSystem`, `RicochetMoveDirectionReflectSystem` | Shoot wall/obstacle at angle | Max 3 bounces |
| Tank Armor / NoPen | Needs Manual Check | Implemented | `HitResolver`, `TankArmor`, `ArmorHitInfo` | Test front and glancing side hits | Uses effective armor |
| Tank Ricochet | Needs Manual Check | Implemented | `HitResolver`, projectile ricochet systems | Test high-angle tank hit | Uses current ricochet request path |
| Damage / HP | Needs Manual Check | Implemented | `TankHealth`, `HitResolver` | Penetrating hit changes HP | Player 100, enemy 300 |
| Win/Lose | Needs Manual Check | Implemented | `SandboxMatchController`, HUD presenter | Kill player/enemy | Restart still needs check |
| Combat Feedback UI | Needs Manual Check | Implemented | `UI/CombatFeedback/*`, `WorldHealthBarPrefab` | HP bars and hit text | Restart duplication needs check |
| Debug Logs | Partial | Implemented with `DebugLogConfig` and debug visualizer | `DebugLogConfig`, `SandboxDebugVisualizer`, projectile systems | Watch console volume | Log tuning still needs Unity check |
| Materials / Visual Polish | Partial | Greybox materials/prefabs exist | Prefabs, scene materials | Inspect for magenta/broken visuals | Visual polish not final |
| Main Menu | Partial | UI scripts exist | `UI/MainMenu/*`, `SceneLoaderService` | Check scene availability/build flow | Direct demo scene launch is reliable path |
| Mobile Controls | Future | Not implemented | None yet | N/A | Desktop first |
| Enemy AI | Future | Enemy is dummy | `EnemyDummyTank`, `TankFacade` | N/A | No movement/aim/shoot AI |

## Demo Scene Checklist

- [x] Scene asset exists: `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity`.
- [ ] Scene launches in Play Mode - Needs Manual Unity Check.
- [ ] Player and enemy spawn correctly - Needs Manual Unity Check.
- [ ] Camera top-down framing is correct - Needs Manual Unity Check.
- [ ] Tank controls feel correct - Needs Manual Unity Check.
- [ ] Shooting works in Play Mode - Needs Manual Unity Check.
- [ ] Wall/obstacle ricochet works in Play Mode - Needs Manual Unity Check.
- [ ] Tank ricochet/no-penetration works in Play Mode - Needs Manual Unity Check.
- [ ] Damage and HP update in Play Mode - Needs Manual Unity Check.
- [ ] World HP bars display and shrink - Needs Manual Unity Check.
- [ ] Floating hit text appears - Needs Manual Unity Check.
- [ ] Win/lose triggers - Needs Manual Unity Check.
- [ ] Restart resets round - Needs Manual Unity Check.
- [ ] Restart does not duplicate subscriptions or HP bars - Needs Manual Unity Check.

## Known Issues / Risks

- Materials may still need visual polish if any magenta/broken materials appear in Unity.
- Debug logs such as `[SHOT]`, `[HIT]`, `[BOUNCE]`, and `[ARMOR]` can spam the Console when enabled in `DebugLogConfig`.
- Scene and prefab changes made inside Unity must be saved and committed manually.
- Combat feedback restart duplication must be manually checked in Unity.
- Enemy AI is not implemented; enemy is still a dummy target.
- Menu/bootstrap flow is not final for the current playable demo; direct scene launch is the reliable path.
- Generated `graphify-out/GRAPH_REPORT.md` is stale and should not override `AI_CONTEXT_GRAPH.md`.
- Existing uncommitted Unity scene/config changes should be reviewed before commit.
- Mobile controls are design-only in `docs/MOBILE_CONTROLS.md`.
- Recoil/knockback feeling is not implemented.
- Network/multiplayer is future research only.
