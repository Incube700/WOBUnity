# AI_CONTEXT_GRAPH.md - compact project map

Purpose: give future Codex/AI sessions a short, current map before scanning the whole repository.

## Current Snapshot

- Project: **World of Balance / Ricochet Tanks**
- Repository branch: `feature/ricochet-tanks-prototype`
- Engine: Unity 6
- Main scene: `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity`
- Prototype root: `Assets/_Project/RicochetTanks/`
- Current source of truth: `docs/GDD.md`
- Root `GDD.md` is only a redirect.
- `graphify-out/GRAPH_REPORT.md` is stale; prefer this file and `docs/GDD.md`.

## Current Playable Goal

Open `RicochetTanks_Demo.unity`, press Play, control the player tank, aim/shoot, bounce projectiles from walls, resolve armor hits against the enemy dummy, show HP/combat feedback, trigger win/lose, and restart.

Anything not verified inside Unity should be marked **Needs Manual Unity Check**.

## Scene Graph

```text
RicochetTanks_Demo.unity
  SceneContext / GameplayEntryPoint
  ArenaRoot
    Floor
    Walls
    Obstacles
  SpawnPoints
    PlayerSpawnPoint
    EnemySpawnPoint
  PlayerTank
  EnemyDummyTank
  CameraRig
  GameplayCanvas
  CombatFeedbackRoot (resolved/created by GameplayEntryPoint when needed)
```

`RicochetTanks_Demo` is the primary editable/playable scene. `Sandbox.unity` remains a legacy/procedural fallback and is disabled in Build Settings.

## Architecture Map

The project is not DOTS/Entitas. It uses feature-based MonoBehaviours, plain C# presenters/services, and a lightweight ECS-style projectile data/system pipeline.

```text
GameplayEntryPoint
  -> composition root for RicochetTanks_Demo
  -> resolves scene refs/configs
  -> configures ProjectileFactory, tanks, HUD, match controller, combat feedback
  -> should stay thin; no gameplay rules or animation logic

TankFacade
  -> TankMovement
  -> TurretAiming
  -> TankShooter
  -> TankHealth
  -> TankArmor
  -> PlayerTankController (enabled only for player)

PlayerTankController
  -> reads DesktopInputReader
  -> passes throttle/turn/aim/fire commands

Projectile
  -> thin MonoBehaviour runner
  -> owns ProjectileEntity
  -> ticks ProjectileSystemPipeline in FixedUpdate

ProjectileSystemPipeline
  -> SavePreviousProjectilePositionSystem
  -> ProjectileMovementSystem
  -> RicochetDetectionSystem
  -> ProjectileHitDetectionSystem
  -> RicochetPositionCorrectionSystem
  -> RicochetMoveDirectionReflectSystem
  -> RicochetRotationReflectSystem
  -> RicochetSpendBounceSystem
  -> RicochetSpeedReduceSystem
  -> RicochetDamageReduceSystem
  -> RicochetEventPublishSystem
  -> RicochetCleanupSystem
  -> ProjectileLifetimeSystem
  -> ProjectileDestroySystem

HitResolver / TankArmor
  -> armor zone, angle, effective armor, penetration/no-penetration/ricochet

SandboxGameplayEvents
  -> ProjectileSpawned
  -> ProjectileHit
  -> ProjectileBounced
  -> HitResolved
  -> CombatFeedbackRequested
  -> MatchStarted
  -> MatchFinished
  -> RestartRequested

UI/Sandbox
  -> SandboxHudView
  -> SandboxHudPresenter
  -> SandboxMatchController

UI/CombatFeedback
  -> CombatFeedbackFactory
  -> CombatFeedbackPresenter
  -> TankHealthBarView
  -> TankHealthBarPresenter
  -> FloatingHitTextView
```

## Current Implemented Features

- Main playable scene asset exists.
- Player tank and enemy dummy tank exist in scene asset.
- Arena, walls, obstacles, spawn points, camera rig, and gameplay canvas exist in scene asset.
- Desktop controls: W/S or arrows for throttle/brake/reverse, A/D or arrows for hull turning, mouse aim, LMB/Space fire, R restart.
- Tank movement has acceleration, braking/coasting, reverse, controlled Y-only hull turning.
- Turret aim is independent and projected to the ground plane.
- Projectile shooting uses muzzle forward direction.
- Projectiles use previous-position sphere cast for hits.
- Wall/obstacle ricochet uses reflection by hit normal.
- Tank armor uses front/side/rear zones from local hit normal.
- Effective armor uses `armor / max(cos(angle), safeMinCos)`.
- No-penetration and armor ricochet produce no damage.
- HP and death are handled by `TankHealth`.
- Win/lose is handled by `SandboxMatchController`.
- Combat feedback uses event-driven world HP bars and floating hit text.

## Current Config Numbers

From `Assets/_Project/RicochetTanks/Configs/ProjectileConfig.asset`:

- Projectile speed: `22`
- Bounce speed multiplier: `0.78`
- Cooldown: `0.8`
- Owner safe time: `0.15`
- Lifetime: `8`
- Min speed: `5`
- Radius: `0.18`
- Flight height: `0.6`
- Spawn offset: `0.35`
- Damage: `110`
- Damage multiplier per bounce: `0.75`
- Max ricochets: `3`
- Penetration: `45`

From tank configs:

- Player HP: `100`
- Enemy HP: `300`
- Armor: front `50`, side `40`, rear `10`
- Auto ricochet angle: `60`
- Player max forward speed: `5`
- Player max reverse speed: `2.5`
- Player acceleration: `10`
- Player brake deceleration: `14`
- Player coast deceleration: `6`
- Player hull turn speed: `140`
- Player low-velocity turn speed: `80`
- Player turret rotation speed: `360`
- Player input dead zone: `0.05`

From camera/match configs:

- Camera local position: `(0, 14, 0)`
- Camera local euler angles: `(90, 0, 0)`
- Camera orthographic size: `8.59`
- Match labels: `Round: Playing`, `Player Wins`, `Enemy Wins`

## Do Not Touch Without Reason

- Ricochet math and projectile movement systems.
- Armor balance and hit resolution math.
- Tank movement/controls.
- Combat feedback UI behavior.
- Scene layout and scene generator.
- Materials/prefabs unless a serialized reference is broken.
- GDD/README unless the task is documentation sync.

## Important Coding Rules

- Keep systems small and feature-based.
- Do not put UI logic in gameplay systems.
- Do not put gameplay logic in views.
- `GameplayEntryPoint` is a composition root, not a gameplay logic holder.
- Use named methods for event subscription when unsubscribe is needed.
- No lambdas for event subscriptions unless stored/disposed safely.
- Private fields use `_camelCase`.
- No Singleton.
- Avoid broad refactors.
- Do not instantiate UI from `HitResolver` or projectile gameplay math.
- `SandboxDebugVisualizer` remains debug-only.

## Known Risks

- Manual Unity Play Mode verification is still required.
- HP bars/floating hit text/restart duplication need manual check.
- Debug logs can spam Console: `[SHOT]`, `[HIT]`, `[BOUNCE]`, `[ARMOR]`.
- Materials may need visual polish if Unity shows broken/magenta visuals.
- Scene/prefab/config edits made in Unity must be saved and committed.
- Enemy AI is not implemented; enemy is a dummy target.
- Main menu/bootstrap scene flow is not the current reliable launch path; open `RicochetTanks_Demo` directly.
- There were accidental `.zip` archive assets under `Assets`; they should stay removed.

## Next Safe Tasks

1. Manually verify HP bars, floating hit text, and restart in Unity.
2. Add a `DebugLogConfig` or logging toggle.
3. Clean visual/material readability.
4. Stabilize arena size/layout only if the scene is wrong.
5. Improve simple 3D tank visual prefabs.
6. Add simple enemy behavior as a separate feature.
7. Polish main menu/restart flow.
8. Add mobile controls later.

## Files To Read First

1. `AGENTS.md`
2. `README.md`
3. `docs/GDD.md`
4. `AI_CONTEXT_GRAPH.md`
5. `Assets/_Project/RicochetTanks/Scripts/Infrastructure/Bootstrap/GameplayEntryPoint.cs`
6. `Assets/_Project/RicochetTanks/Scripts/Gameplay/Projectiles/Systems/ProjectileSystemPipeline.cs`
7. `Assets/_Project/RicochetTanks/Scripts/Gameplay/Combat/HitResolver.cs`
8. `Assets/_Project/RicochetTanks/Scripts/Gameplay/Combat/TankArmor.cs`
