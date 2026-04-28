# AI_CONTEXT_GRAPH.md - compact project map

Purpose: give future Codex/AI sessions a short, current map before scanning the whole repository.

## Current Snapshot

- Project: **World of Balance / Ricochet Tanks**
- Repository branch: `main`
- Engine: Unity 6
- Main scene: `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity`
- Prototype root: `Assets/_Project/RicochetTanks/`
- Current source of truth: `docs/GDD_RU.md`
- Root `GDD.md` is only a redirect.
- `docs/GDD.md` is only a compatibility pointer.
- `graphify-out/GRAPH_REPORT.md` is stale; prefer this file, `docs/GDD_RU.md`, and `docs/TECH_STATUS.md`.

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
- Mobile controls are not implemented; design direction is documented in `docs/MOBILE_CONTROLS.md`.
- Minimal VFX/recoil ideas are documented as TODO, not implemented.
- Network/multiplayer is future research only, not an immediate implementation target.

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
- Player auto ricochet angle: `50.3`
- Enemy auto ricochet angle: `60`
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
- Camera orthographic size: `10.08`
- Match labels: `Round: Playing`, `Player Wins`, `Enemy Wins`

## Do Not Touch Without Reason

- Ricochet math and projectile movement systems.
- Armor balance and hit resolution math.
- Tank movement/controls.
- Combat feedback UI behavior.
- Scene layout and scene generator.
- Materials/prefabs unless a serialized reference is broken.
- GDD/README unless the task is documentation sync.
- Do not rewrite controls before `docs/MOBILE_CONTROLS.md` is reviewed.
- Do not implement network/multiplayer yet.

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
- Projectile speed loss after ricochet may be too subtle visually and needs manual review/tuning.
- Mobile landscape controls are required next but are design-only right now.
- VFX/recoil requirements are documented but not implemented.
- Debug logs can spam Console if enabled in `DebugLogConfig`: `[SHOT]`, `[HIT]`, `[BOUNCE]`, `[ARMOR]`.
- Materials may need visual polish if Unity shows broken/magenta visuals.
- Scene/prefab/config edits made in Unity must be saved and committed.
- Enemy AI is not implemented; enemy is a dummy target.
- Main menu/bootstrap scene flow is not the current reliable launch path; open `RicochetTanks_Demo` directly.
- There were accidental `.zip` archive assets under `Assets`; they should stay removed.

## Next Safe Tasks

1. Manually verify HP bars, floating hit text, and restart in Unity.
2. Validate damage, penetration, armor, ricochet, and speed-loss formulas in Play Mode.
3. Review whether ricochet speed loss is visually strong enough.
4. Review `docs/MOBILE_CONTROLS.md` and answer `docs/GD_QUESTIONS.md`.
5. Prototype mobile controls only after the design doc is accepted.
6. Add minimal hit/impact/wreck VFX after combat readability is verified.
7. Add visual-first recoil/knockback after the designer decides scope.
8. Keep network/multiplayer as research only.

## Files To Read First

1. `AGENTS.md`
2. `README.md`
3. `docs/GDD_RU.md`
4. `docs/TECH_STATUS.md`
5. `docs/MOBILE_CONTROLS.md`
6. `docs/GD_QUESTIONS.md`
7. `AI_CONTEXT_GRAPH.md`
8. `Assets/_Project/RicochetTanks/Scripts/Infrastructure/Bootstrap/GameplayEntryPoint.cs`
9. `Assets/_Project/RicochetTanks/Scripts/Gameplay/Projectiles/Systems/ProjectileSystemPipeline.cs`
10. `Assets/_Project/RicochetTanks/Scripts/Gameplay/Combat/HitResolver.cs`
11. `Assets/_Project/RicochetTanks/Scripts/Gameplay/Combat/TankArmor.cs`
