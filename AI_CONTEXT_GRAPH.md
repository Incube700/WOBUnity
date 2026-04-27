# AI_CONTEXT_GRAPH.md — compact project map

Purpose: reduce token usage for Codex/AI tools. Read this file before scanning the whole repository.

## Project identity

- Project: **World of Balance / Мир баланса / Ricochet Tanks**
- Owner/brand: **BurnHeartGames**
- Repository: `Incube700/WOBLearnUnity`
- Engine target: **Unity 6**
- Prototype genre: strict top-down tank duel with ricochets
- Current branch for gameplay work: `feature/ricochet-tanks-prototype`

## Current prototype direction

Milestone 1 is a first playable tank duel with one canonical scene:

```text
Open Sandbox.unity -> Play -> move/aim/shoot -> ricochet -> damage/destroy enemy -> restart
```

The active prototype is isolated under:

```text
Assets/_Project/RicochetTanks/
```

## Scene graph

```text
Sandbox.unity
  SandboxBootstrapper (runtime-created)
  SandboxSceneBuilder
  DesktopInputReader
  ProjectileFactory
  Player TankFacade
  Enemy Dummy TankFacade
  SandboxHudView
  SandboxHudPresenter
  SandboxMatchController
```

Legacy scene names such as `Sand Box`, `Bootstrap`, `MainMenu`, and `RicochetTanks_*` are not part of the canonical playable flow.

## Folder graph

```text
Assets/_Project/RicochetTanks/
  Scenes/
    Sandbox.unity
  Scripts/
    Configs/
      ArenaConfig.cs
      TankConfig.cs
      ProjectileConfig.cs
      DebugVisualizationConfig.cs
    Gameplay/
      Debug/
        SandboxDebugVisualizer.cs
      Events/
        SandboxGameplayEvents.cs
    Infrastructure/
      Bootstrap/
      SceneLoading/
    Gameplay/
      Tanks/
      Projectiles/
      Combat/
    Input/
      Desktop/
    UI/
      MainMenu/
      Sandbox/
```

## Gameplay object graph

```text
SandboxBootstrapper
  -> calls SandboxSceneBuilder
  -> owns SandboxGameplayEvents for this scene instance
  -> wires SandboxHudPresenter
  -> wires SandboxMatchController
  -> wires SandboxDebugVisualizer when debug visualization is enabled

SandboxSceneBuilder
  -> creates strict top-down camera
  -> creates 10x10 dark arena, grid, walls, center obstacle
  -> creates camera/light/HUD/EventSystem
  -> creates DesktopInputReader
  -> creates ProjectileFactory
  -> creates Player and Enemy TankFacade

TankFacade
  -> TankHealth
  -> TankArmor
  -> TankMovement
  -> TurretAiming
  -> TankShooter
  -> PlayerTankController (enabled only for player)

TankShooter
  -> ProjectileFactory
  -> spawns Projectile from muzzle

Projectile
  -> ignores owner during safe time
  -> moves deterministically via SphereCast per physics tick
  -> raises ProjectileHit and ProjectileBounced
  -> reflects via RicochetCalculator
  -> asks HitResolver for tank hits

SandboxMatchController
  -> listens to health deaths and restart events
  -> raises MatchStarted and MatchFinished
  -> disables gameplay after match end

SandboxHudPresenter
  -> listens to HealthChanged, HitResolved, MatchStarted, MatchFinished
  -> listens to SandboxHudView.RestartClicked and raises RestartRequested
  -> updates SandboxHudView

SandboxDebugVisualizer
  -> listens to ProjectileSpawned / ProjectileHit / ProjectileBounced / HitResolved / Match events
  -> draws projectile direction, predicted segment, collision normal, bounce count
  -> labels armor zone, hit angle, penetration, effective armor, enemy FSM state
  -> draws spawn points and arena bounds
```

## Event contracts

```text
TankHealth: HealthChanged, Died
ProjectileFactory: ProjectileSpawned
Projectile: ProjectileHit, ProjectileBounced
HitResolver: HitResolved
SandboxMatchController: MatchStarted, MatchFinished, RestartRequested handling
SandboxHudPresenter: UI listener/writer, no gameplay logic
SandboxDebugVisualizer: optional debug listener, no gameplay logic
```

## Input graph

Desktop first:

```text
DesktopInputReader
  movement: W/S or Up/Down forward/back relative to hull
  turn: A/D or Left/Right hull rotation
  aim: mouse world position
  fire: Left Mouse Button or Space
  restart: R
```

Mobile controls are deferred until after Milestone 1.

## Ricochet rules

Milestone 1:

```text
Projectile speed: fast but visible
Projectile visual: bright sphere + TrailRenderer
Max ricochets: 3
Projectile damage: 35
Projectile penetration: 100
Reload time: 0.8 sec
Bounce speed multiplier: 0.85
Min projectile speed: 5
Owner safe time: 0.15 sec
After safe time: projectile may hit shooter
Hit wall/obstacle: reflect by collision normal
Glancing tank hit: reflect by tank contact normal at auto ricochet angle 70 degrees
Projectile collision source: manual SphereCast, not Unity collision callbacks
After ricochet count exceeded: destroy projectile on next contact
Hit tank: resolve basic armor, then apply fixed damage on penetration
```

Future extension:

```text
Armor zones: front 100 / side 70 / rear 40
Hit results: penetrated / ricochet / no penetration / wall ricochet
Kinetic damage based on projectile speed
Enemy AI with line of sight and lead aiming
```

## Coding conventions

- Private fields use `_camelCase`.
- No giant MonoBehaviour.
- No Singleton.
- No `FindObjectOfType` spam.
- Use named event handlers, not disposable anonymous lambdas, when unsubscribe is needed.
- Views expose events and display methods.
- Presenters/controllers own wiring.
- Bootstrap owns scene-level composition.

## Files AI should read first

1. `AGENTS.md` — strict AI rules.
2. `README.md` — public project description and how to test.
3. `GDD.md` — design direction.
4. This file — compact graph.
5. GitHub issue `#9` if accessible.

## Milestone priority

1. Compile in Unity.
2. Sandbox opens and builds itself through Play.
3. Player movement and turret aim.
4. Visible shooting.
5. Ricochet.
6. Enemy damage/death.
7. HUD and restart.
8. Mobile controls.
9. Enemy shooting/AI.

## Anti-goals

Do not spend time on:

- polished art;
- complex AI before Milestone 1;
- full kinetic armor/penetration simulation before first playable;
- multiplayer;
- advanced save system;
- monetization;
- over-engineered DI container;
- rewriting unrelated homework/course code.
