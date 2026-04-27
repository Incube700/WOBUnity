# World of Balance / Ricochet Tanks — GDD

**Version:** 1.0 — Unified Prototype Source of Truth  
**Date:** 2026-04-27  
**Engine:** Unity 6  
**Active branch:** `feature/ricochet-tanks-prototype`  
**Main prototype root:** `Assets/_Project/RicochetTanks/`

This document is the main source of truth for the current Ricochet Tanks prototype. Older root-level documents remain useful as history, but this file defines the current design and technical target.

## 1. Краткое описание

| Topic | Value |
|---|---|
| Название | World of Balance / Мир баланса / Ricochet Tanks |
| Жанр | Top-down / light-isometric arcade tank duel |
| Камера | Strict top-down for Milestone 1; light isometric can be tested later |
| Основной режим | 1v1: player tank vs enemy dummy |
| Главная фишка | Positioning, hull angle, turret angle, ricochet shots |
| Платформа прототипа | Desktop first, mobile controls later |
| Canonical demo scene | `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity` |
| Legacy/dev fallback scene | `Assets/_Project/RicochetTanks/Scenes/Sandbox.unity` |

The current playable target is small: one arena, two tanks, visible fast projectiles, ricochets, HP, win/lose, and restart.

## 2. Видение игры

The player controls a tank in a compact arena and wins by reading geometry. A good shot is not only a quick click: it is a decision about position, hull angle, turret angle, wall normals, cover, and bounce count.

The intended feeling is a clean tactical arcade duel: readable, tense, and slightly cerebral. The player should quickly understand why a projectile bounced, why a hit damaged a tank, and why a risky angle failed.

Ricochet Tanks differs from a simple shooting prototype because walls and armor angles are part of the weapon. A projectile can hit directly, bounce into a hidden target, glance off armor, or eventually return to the shooter.

## 3. Core Gameplay Loop

1. Player moves around the arena.
2. Player rotates hull to control movement and future armor angle.
3. Player aims the turret independently.
4. Player fires from the muzzle point.
5. Projectile travels visibly across the XZ plane.
6. Projectile checks the path from previous position to current position.
7. Projectile hits a tank, expires, or ricochets from a reflective surface.
8. Ricochets change direction, update rotation, spend bounce count, and reduce damage.
9. Tank HP changes after valid penetration.
10. Enemy death ends the round with player win; player death ends with player lose.
11. Restart resets the sandbox.

## 4. Player Tank

The player tank must have:

- hull/body;
- turret;
- muzzle/fire point;
- forward/back movement relative to hull direction;
- hull rotation;
- independent turret aiming;
- shooting;
- health;
- damage receiving;
- death/deactivation.

Current code status:

- Runtime tank objects are created by `SandboxSceneBuilder`.
- `TankFacade` links the tank parts and exposes small feature components.
- `TankMovement` handles Rigidbody movement and hull rotation.
- `TurretAiming` rotates the turret toward the mouse aim point.
- `TankShooter` owns cooldown and delegates projectile spawning.
- `TankHealth` owns HP, health events, and deactivation on death.
- `TankArmor` exists for basic front/side/rear and ricochet angle checks.

Architecture target:

- `TankFacade` is allowed as Unity/ECS bridge.
- It must not become a large gameplay brain.
- Movement, aiming, shooting, health, armor, and future AI stay separated.

## 5. Enemy / Dummy Tank

The demo enemy is a dummy tank, not a real bot yet.

Enemy requirements:

- exists in the sandbox scene;
- has health;
- receives damage;
- can die/deactivate;
- is suitable for direct hit, ricochet hit, armor, and win-condition tests.

Current code status:

- `SandboxSceneBuilder` creates `Enemy Dummy Tank` at the top-right spawn.
- Enemy uses the same `TankFacade`, `TankHealth`, `TankArmor`, and hitbox setup as the player.
- Enemy controller is disabled through `TankFacade.SetPlayerControlled(false)`.
- No enemy movement, aiming, shooting, or FSM AI exists yet.

Future AI must be added as a separate feature, not mixed into the dummy tank setup.

## 6. Projectile

Projectile requirements:

- spawns from the muzzle point;
- has visible sphere/trail representation;
- has direction;
- has speed;
- has damage;
- has lifetime;
- has limited ricochet count;
- checks the traveled segment every physics tick;
- damages tanks on valid hit;
- can hit the shooter after owner-safe time;
- destroys/deactivates after hit, lifetime, low speed, or exhausted bounces.

Current code status after this GDD update:

- `ProjectileFactory` creates `ProjectileConfig.ProjectilePrefab` when assigned, with primitive fallback for legacy sandbox.
- `ProjectileFactory` can use `ProjectileConfig.ProjectilePrefab` from the inspector.
- `Projectile` is now a Unity runner/facade for a local projectile entity.
- Projectile data lives in small component structs.
- Projectile logic runs through `ProjectileSystemPipeline`.
- Collision uses `Physics.SphereCastAll` with `ProjectileConfig` layer masks.
- Ricochet reflection uses `Vector3.Reflect` through `RicochetCalculator`.
- Max ricochets are configurable.
- Projectile speed falls after bounce.
- Damage falls by `ProjectileConfig.DamageMultiplierPerBounce` after each ricochet.
- Projectile rotation is updated after ricochet.
- Movement, detection, ricochet application, hit resolution, and destroy logic are split into systems.

Architecture target:

- `Projectile` should be a Unity runner/facade for the projectile entity.
- Projectile data should live in a small runtime data object.
- Logic should be split into small systems called from the fixed tick pipeline.
- A full Unity DOTS package is not installed, so this prototype uses a lightweight ECS-style local pipeline until a real ECS package is introduced.

## 7. Ricochet System

Ricochet is a required Milestone 1 mechanic.

### 7.1 Required Physics Behavior

Each fixed tick:

1. Save the projectile previous position before movement.
2. Move the projectile by `direction * speed * fixedDeltaTime`.
3. Check the path from previous position to current position.
4. Use `Raycast`/`SphereCast` from previous position toward current position.
5. On hit, record:
   - `hit.point`;
   - `hit.normal`;
   - incoming direction before reflection.
6. Create a one-tick ricochet request/event.
7. Separate systems apply position correction, direction reflection, rotation update, bounce spending, damage reduction, and cleanup.

### 7.2 Required Fixed Tick Order

The projectile fixed pipeline should run in this order:

| Order | System | Responsibility |
|---:|---|---|
| 1 | `SavePreviousProjectilePositionSystem` | Stores position before movement |
| 2 | `ProjectileMovementSystem` | Moves projectile to the predicted current position |
| 3 | `RicochetDetectionSystem` | Casts previous->current path and creates request only |
| 4 | `ProjectileHitDetectionSystem` | Resolves tank hit requests before wall ricochet apply |
| 5 | `RicochetPositionCorrectionSystem` | Places projectile at `hit.point + hit.normal * offset` |
| 6 | `RicochetMoveDirectionReflectSystem` | Uses `Vector3.Reflect(incomingDirection, hitNormal).normalized` |
| 7 | `RicochetRotationReflectSystem` | Rotates projectile along the XZ-plane movement direction |
| 8 | `RicochetSpendBounceSystem` | Decreases remaining bounce count |
| 9 | `RicochetSpeedReduceSystem` | Preserves existing speed-loss tuning |
| 10 | `RicochetDamageReduceSystem` | Applies `damage *= 0.75f` by default |
| 11 | `RicochetEventPublishSystem` | Publishes bounce count, speed, damage, and normal |
| 12 | `RicochetCleanupSystem` | Clears one-tick request/event |
| 13 | `ProjectileLifetimeSystem` | Destroys expired projectiles |
| 14 | `ProjectileDestroySystem` | Destroys or deactivates requested projectiles |

Tank-armor ricochet may use the same ricochet request data as wall ricochet. Detection and apply logic must stay separate.

### 7.3 Ricochet Values

| Value | Base |
|---|---:|
| Max bounces | 3 |
| Position offset | projectile radius + small skin |
| Speed multiplier per bounce | 0.78 |
| Damage multiplier per bounce | 0.75 |
| Example damage | 100 -> 75 -> 56.25 -> 42.1875 |

These values must come from config/runtime data, not hardcoded in systems.

## 8. Armor Angle / Damage Rules

Armor angle is part of the game identity, but it can stay simple in Milestone 1.

Current implemented rule:

- `TankArmor` detects front/side/rear from contact normal.
- `HitResolver` checks auto ricochet angle, current penetration, and effective armor.
- If the angle is too sharp, the projectile ricochets instead of dealing damage.
- If penetration is lower than effective armor, the hit becomes `NoPen`.
- On penetration, the current projectile damage cap is applied.

Prototype balance values from the combat GDD:

| Value | Prototype Balance |
|---|---:|
| Projectile damage cap | 110 |
| Projectile penetration | 45 |
| Projectile kinetic factor | 1.0 |
| Front armor | 50 |
| Side armor | 40 |
| Rear armor | 10 |
| Auto ricochet angle | 70 degrees |

GDD target rules:

- Near-perpendicular face hit: full damage.
- Borderline hit: reduced damage or no penetration, depending on future balance.
- Sharp hit: ricochet or no damage.
- Exact corner hit: always ricochet.
- Tank ricochet uses the same movement reflection pipeline as wall ricochet.
- Returning projectiles may damage the shooter after owner-safe time.

Roadmap note:

- Current armor math is a prototype foundation, not the final damage model.
- Future iterations should add kinetic speed-based penetration, better corner detection, damage falloff, and armor-zone debug visualization.

## 9. Arena

Canonical sandbox layout:

| Object | Position | Notes |
|---|---|---|
| Arena center | `(0, 0, 0)` | XZ gameplay plane |
| Player spawn | bottom-left, around `(-3.75, 0, -3.75)` | faces center |
| Enemy spawn | top-right, around `(3.75, 0, 3.75)` | faces center |
| Arena size | about 10x10 | greybox floor |
| North/South/East/West walls | perimeter | reflective colliders |
| Center obstacle | center | reflective block/cover |

Current code status:

- `SandboxSceneBuilder.CreateArena` procedurally creates the floor, grid, walls, and center block.
- `RicochetTanks_Demo` uses editable wall/block prefab instances generated under `Assets/_Project/RicochetTanks/Prefabs/`.
- Floor collider is removed.
- Walls and obstacle keep colliders and are reflective by default.
- `RicochetReflectable`, `Tank`, `Projectile`, and `Obstacle` layers are defined in `ProjectSettings/TagManager.asset`.
- `ProjectileConfig` owns reflectable and hittable masks.

Future:

- Move color/material choices into config or visual factory if visuals grow.

## 10. Win/Lose Conditions

Required demo result:

- enemy dies: player win;
- player dies: enemy win/player lose.

Current code status:

- `SandboxMatchController` subscribes to `TankHealth.Died`.
- Enemy death raises `PlayerWins`.
- Player death raises `EnemyWins`.
- HUD shows round result.
- Restart reloads the active scene.

## 11. Controls

Desktop controls:

| Input | Action |
|---|---|
| `W/S` or `Up/Down` | Move forward/back relative to hull |
| `A/D` or `Left/Right` | Rotate hull |
| Mouse | Aim turret |
| Left Mouse / `Space` | Fire |
| `R` | Restart |
| Restart button | Restart |

Current code status:

- `DesktopInputReader` provides tank movement, mouse aim, fire, and restart checks.
- `PlayerTankController` reads desktop input and forwards data to movement/aiming/shooting components.

Future mobile controls:

- left virtual joystick for movement;
- right aim zone or assisted aim;
- fire button;
- restart button;
- separate mobile input view/layer, not inside tank logic.

## 12. Camera

Milestone 1 camera:

- orthographic;
- strict top-down;
- sees the full 10x10 arena;
- keeps projectile and tank direction readable.

Current code status:

- `SandboxSceneBuilder.CreateCamera` creates an orthographic camera at `(0, 14, 0)`, rotation `(90, 0, 0)`, size about `6.25`.

## 13. Technical Architecture

### 13.1 Folder Structure

Active prototype code is isolated here:

```text
Assets/_Project/RicochetTanks/
  Scenes/
  Scripts/
    Configs/
    Gameplay/
      Combat/
      Debug/
      Events/
      Projectiles/
      Tanks/
    Infrastructure/
      Bootstrap/
      SceneLoading/
    Input/
      Desktop/
    UI/
      MainMenu/
      Sandbox/
```

Legacy assets exist outside `_Project`:

- `Assets/Prefabs/Enemy.prefab`
- `Assets/Prefabs/Player.prefab`
- `Assets/Resources/Prefabs/Bullet.prefab`
- old generic Unity/2D settings and sprites

They are not part of the active Ricochet Tanks flow unless explicitly reintroduced.

### 13.2 ECS / Data-Oriented Direction

Audit result:

- No `com.unity.entities` package is installed.
- No Entitas/LeoECS/Morpeh/Svelto dependency was found.
- No generated `EntityAPI` folder or generator menu was found.
- Current gameplay is feature-based MonoBehaviour plus plain C# services/helpers.

Current decision:

- Do not add a heavy ECS package in this iteration.
- Use a lightweight ECS-style local pipeline for projectile/ricochet:
  - components/data objects hold projectile state;
  - small systems hold logic;
  - `Projectile` MonoBehaviour is only a runner/facade/view bridge;
  - `ProjectileFactory` creates the Unity view and initializes data.

If a real ECS framework is added later, the same component/system split should map cleanly to it.

### 13.3 Bootstrap and Scene Flow

Current code status:

- Runtime auto-load/auto-create callbacks are disabled.
- `RicochetTanks_Demo.unity` is opened explicitly in the Unity Editor for prototype testing.
- `GameplayEntryPoint` composes the editable demo scene from serialized scene references and configs.
- `SandboxBootstrapper`/`SandboxSceneBuilder` remain as a legacy procedural fallback, but they no longer create themselves globally on scene load.

Status and target:

- Bootstrap scene -> Main Menu -> Sandbox is the long-term flow from `AGENTS.md`.
- `ProjectBootstrapper` no longer redirects scenes at runtime.
- For current editor-friendly testing, `RicochetTanks_Demo.unity` is primary.
- `Sandbox.unity` remains a procedural fallback only if the scene contains a `SandboxBootstrapper`.
- `Bootstrap.unity` and `MainMenu.unity` scene assets still need final generation/build-setting work.

### 13.4 Fixed Tick Pipeline

Current status:

- Projectile has one Unity `FixedUpdate` wrapper.
- Movement, cast detection, ricochet, damage, lifetime, and destroy logic are delegated to `ProjectileSystemPipeline`.

Target:

- Keep one Unity lifecycle wrapper, but delegate fixed tick work to ordered systems.
- Register projectile systems in one place, close to projectile feature code.
- Do not create a parallel global update loop for the whole project in this iteration.

### 13.5 MonoBehaviour Rules

Allowed:

- view/facade/binder;
- Unity lifecycle runner;
- scene object references;
- colliders, renderers, gizmos;
- bootstrap composition.

Not allowed:

- one large gameplay brain;
- UI button handlers that apply damage/spawn gameplay directly;
- duplicated manager architecture;
- hidden `FindObjectOfType` in gameplay loops.

## 14. Scene Assembly / Editor Workflow

The editor-friendly workflow is the primary workflow going forward.

### 14.1 Main Demo Scene

Primary scene:

```text
Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity
```

The scene should contain:

- `SceneContext / GameplayEntryPoint`;
- `ArenaRoot`;
- `Floor`;
- `Walls`;
- `Obstacles`;
- `SpawnPoints/PlayerSpawnPoint`;
- `SpawnPoints/EnemySpawnPoint`;
- `PlayerTank`;
- `EnemyDummyTank`;
- `CameraRig`;
- `GameplayCanvas`.

`GameplayEntryPoint` only composes dependencies: configs, scene references, factories, HUD presenter, match controller, input, and projectile pipeline. Gameplay rules stay in feature classes/systems.

### 14.2 Prefabs

Required prefab assets:

| Prefab | Purpose |
|---|---|
| `PlayerTankPrefab` | Player view/root with hull, turret, muzzle, collider, and tank components |
| `EnemyDummyTankPrefab` | Enemy dummy view/root with same tank feature components |
| `ProjectilePrefab` | Visible projectile view used by `ProjectileFactory` |
| `WallSegmentPrefab` | Reflective arena wall segment with collider/layer |
| `ArenaBlockPrefab` | Reflective arena obstacle block with collider/layer |
| `GameplayCanvasPrefab` | HUD canvas with HP, last hit, round result, controls, restart |

The generator creates these under:

```text
Assets/_Project/RicochetTanks/Prefabs/
```

### 14.3 Config Assets

Required config assets:

| Config | Inspector-tunable values |
|---|---|
| `PlayerTankConfig` | HP, move speed, hull rotation speed, turret rotation speed, armor values, auto ricochet angle |
| `EnemyTankConfig` | HP, dummy movement values, turret values, armor values |
| `ProjectileConfig` | projectile prefab, speed, damage, lifetime, max ricochets, damage multiplier, position offset, masks |
| `MatchConfig` | round labels/result strings |
| `CameraConfig` | camera position, rotation, orthographic settings, clip planes, background |

The generator creates these under:

```text
Assets/_Project/RicochetTanks/Configs/
```

### 14.4 Layers

Required project layers:

| Layer | Use |
|---|---|
| `RicochetReflectable` | walls and ricochet surfaces |
| `Tank` | player and enemy hitboxes |
| `Projectile` | projectile view/root |
| `Obstacle` | optional separate obstacle layer |

`ProjectileConfig.ReflectableMask` should include reflective surfaces. `ProjectileConfig.HittableMask` should include tanks. The projectile cast uses the combined mask.

### 14.5 Generating The Editor-Friendly Scene

Editor generator:

```text
Tools/Ricochet Tanks/Generate Editor-Friendly Demo
```

Code path:

```text
Assets/_Project/RicochetTanks/Editor/RicochetTanksEditorAssetGenerator.cs
```

The generator creates/updates prefabs, configs, the demo scene, and adds the demo scene to Build Settings. It is manual-only and does not auto-run on editor load.

Audit note: this Codex environment could not complete Unity batchmode verification because Unity licensing failed in headless mode. The generated `.unity/.prefab/.asset` files are present in the workspace; rerun the menu command only when you intentionally want to rebuild them.

### 14.6 Building A New Test Level

1. Duplicate `RicochetTanks_Demo.unity`.
2. Move `PlayerSpawnPoint` and `EnemySpawnPoint` instead of changing code.
3. Place wall/obstacle prefab instances under `ArenaRoot/Walls` and `ArenaRoot/Obstacles`.
4. Keep reflective objects on `RicochetReflectable` or another layer included in `ProjectileConfig.ReflectableMask`.
5. Keep tanks on `Tank` or another layer included in `ProjectileConfig.HittableMask`.
6. Tune tank/projectile/camera values in config assets.
7. Press Play.

## 15. Feature Map

| Feature | GDD Status | Code Status | Main Components / Data | Main Systems / Classes | Notes |
|---|---|---|---|---|---|
| Project isolation | Required | Implemented | `_Project/RicochetTanks` root | Folder structure | Legacy assets remain outside root |
| Editor demo scene | Required | Implemented | `GameplayEntryPoint` scene refs | `RicochetTanksEditorAssetGenerator` | Generated as `RicochetTanks_Demo` and editable in the inspector |
| Sandbox scene | Legacy fallback | Partial | `SandboxSceneContext` | `SandboxBootstrapper`, `SandboxSceneBuilder` | No longer auto-created globally |
| Bootstrap -> MainMenu -> Sandbox | Required long-term | Partial | Scene names | `ProjectBootstrapper`, `MainMenuBootstrapper` | Old auto-load disabled; explicit scene flow still pending |
| Arena | Required | Implemented | `ArenaConfig`, scene objects | `SandboxSceneBuilder`, `GameplayEntryPoint` | Demo scene uses editable objects |
| Player tank | Required | Implemented | `TankFacade`, `TankHealth`, `TankArmor` | `TankMovement`, `TurretAiming`, `TankShooter`, `PlayerTankController` | Not ECS yet |
| Desktop input | Required | Implemented | Input state from reader | `DesktopInputReader`, `PlayerTankController` | Uses old `UnityEngine.Input` API |
| Enemy dummy | Required | Implemented | `TankFacade`, `TankHealth`, `TankArmor` | Dummy has no controller/AI | Good for hit tests |
| Projectile visible | Required | Implemented | Projectile primitive/trail | `ProjectileFactory` | Generated at runtime |
| Projectile movement | Required | Implemented | `MoveDirectionComponent`, `MoveSpeedComponent` | `ProjectileMovementSystem` | Called by `ProjectileSystemPipeline` |
| Previous position path check | Required | Implemented | `PreviousPositionComponent` | `SavePreviousProjectilePositionSystem`, `RicochetDetectionSystem` | Uses previous->current cast |
| Ricochet detection | Required | Implemented | `RicochetRequestComponent` | `RicochetDetectionSystem` | Detection creates request only |
| Ricochet reflection | Required | Implemented | `MoveDirectionComponent`, `RicochetRequestComponent` | `RicochetMoveDirectionReflectSystem`, `RicochetCalculator` | Uses `Vector3.Reflect` |
| Ricochet rotation update | Required | Implemented | `MoveDirectionComponent` | `RicochetRotationReflectSystem` | XZ-plane rotation |
| Ricochet bounce count | Required | Implemented | `RicochetComponent.BouncesLeft`, `RicochetCount` | `RicochetSpendBounceSystem` | Max comes from `ProjectileConfig` |
| Damage reduced after bounce | Required | Implemented | `DamageComponent`, `RicochetComponent` | `RicochetDamageReduceSystem` | Default multiplier `0.75` |
| Projectile lifetime | Required | Implemented | `LifetimeComponent` | `ProjectileLifetimeSystem` | System split |
| Tank damage | Required | Implemented | `TankHealth`, `ArmorHitInfo` | `HitResolver` | HP/damage now support floats |
| Armor angle | Required foundation | Partial | `TankArmor` values | `HitResolver` | Basic auto ricochet exists |
| Win/Lose | Required | Implemented | `MatchResult` | `SandboxMatchController` | HUD event-driven |
| HUD | Required | Implemented | Health/events | `SandboxHudView`, `SandboxHudPresenter` | UI does not apply gameplay |
| Restart | Required | Implemented | Restart event | `SandboxMatchController`, `SceneLoaderService` | Reloads active scene |
| Debug visualization | Helpful | Implemented | `DebugVisualizationConfig` | `SandboxDebugVisualizer` | Toggle config exists |
| Mobile controls | Future | Missing | None | None | Roadmap |
| Editor-friendly prefabs/configs | Required | Generator implemented | prefab/config assets | `RicochetTanksEditorAssetGenerator`, `GameplayEntryPoint` | Generated by Unity Editor |
| Real ECS / Entity API generation | Optional future | Missing | None | None | No package/generator found |

## 16. Implementation Status

### Done

- Prototype isolated under `Assets/_Project/RicochetTanks`.
- Procedural sandbox scene exists.
- Editor-friendly `RicochetTanks_Demo.unity`, prefabs, and configs exist.
- Editor generator exists as a manual menu command.
- Old runtime auto-load and auto-create callbacks are disabled.
- 10x10 arena, walls, grid, and center obstacle exist.
- Player tank exists and is controllable.
- Enemy dummy exists.
- Projectile is visible.
- Projectile moves fast through `ProjectileMovementSystem`.
- Projectile uses previous-position-to-current-position cast checks.
- Basic wall/obstacle ricochet exists through split systems.
- Projectile rotation updates after ricochet.
- Projectile damage falls by 25% after ricochet by default.
- Basic armor-angle ricochet exists.
- Enemy receives damage and can die.
- HUD shows HP, last hit, round result, controls, and restart.
- Restart works through active scene reload.

### Partial

- Unity Play Mode/compile verification could not be executed in this headless environment because Unity licensing failed.
- Armor rules exist as a simple prototype only.
- MainMenu code exists, but active bootstrap/build settings do not currently expose full Bootstrap -> MainMenu -> Sandbox flow.

### Missing

- `docs/GDD.md` was missing before this update.
- Mobile controls are not implemented.
- Enemy AI/FSM is not implemented.
- Entity API generator is not present.

### Bugs / Risks

- Unity batchmode could not be used for verification in this environment because licensing initialization timed out.
- Manual Unity verification is still required after opening `RicochetTanks_Demo.unity`.
- GitHub issue `#9` could not be read from this environment because `gh` is unavailable and the public issue URL is inaccessible.

### Next Iteration

- Add or finalize `Bootstrap.unity` and `MainMenu.unity`.
- Confirm generated Build Settings scene list inside Unity Editor.
- Add mobile input view.
- Add enemy FSM after first playable loop is stable.

## 17. Demo Scene Checklist

Manual checks for `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity`:

- [ ] Player tank exists.
- [ ] Enemy dummy exists.
- [ ] Arena walls exist.
- [ ] Center obstacle exists.
- [ ] Projectile is visible.
- [ ] Projectile moves.
- [ ] Projectile checks previous position -> current position.
- [ ] Projectile ricochets from wall.
- [ ] Projectile ricochets from center obstacle.
- [ ] Projectile rotation follows new direction after bounce.
- [ ] Projectile damage falls after each bounce.
- [ ] Projectile disappears after max bounces.
- [ ] Projectile damages enemy.
- [ ] Projectile after ricochet damages enemy with reduced damage.
- [ ] Projectile does not get stuck in wall.
- [ ] Position offset prevents repeated same-point ricochet.
- [ ] Enemy death shows player win.
- [ ] Player death shows enemy win/player lose.
- [ ] Restart key works.
- [ ] Restart button works.
- [ ] Scene enters Play Mode without compile errors.

## 18. Roadmap

### MVP Сейчас

- Keep `Sandbox.unity` playable.
- Refactor projectile/ricochet into small systems and data.
- Add 25% damage loss after every ricochet.
- Add explicit previous position data.
- Add projectile rotation update after bounce.
- Generate and use `RicochetTanks_Demo.unity` as the main editor workflow.
- Keep direct scene Play support.
- Keep README short and point to this GDD.

### Следующая Итерация

- Add Bootstrap and MainMenu scenes to the active build flow.
- Add layers/masks for reflective surfaces and tanks.
- Add better projectile destroy/deactivation/pooling.
- Add simple editor/play-mode verification notes.
- Add better debug readouts for current projectile damage and bounces left.

### Polish

- Muzzle flash.
- Sparks on ricochet.
- Impact marks.
- Floating damage/result text.
- Audio.
- Better greybox materials.

### Future Features

- Enemy FSM AI.
- Enemy shooting.
- Mobile controls.
- Kinetic penetration and speed-based damage.
- More arenas.
- Local PvP.
- Online multiplayer only after the prototype is stable.
