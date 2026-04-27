# AI_CONTEXT_GRAPH.md — compact project map

Purpose: reduce token usage for Codex/AI tools. Read this file before scanning the whole repository.

## Project identity

- Project: **World of Balance / Мир баланса / Ricochet Tanks**
- Owner/brand: **BurnHeartGames**
- Repository: `Incube700/WOBLearnUnity`
- Engine target: **Unity 6**
- Prototype genre: top-down / light-isometric tank duel with ricochets
- Current branch for gameplay work: `feature/ricochet-tanks-prototype`

## Current prototype direction

Milestone 1 is a first playable tank duel:

```text
Bootstrap -> MainMenu -> Sandbox -> move/aim/shoot -> ricochet -> damage/destroy enemy -> restart
```

The active prototype is isolated under:

```text
Assets/_Project/RicochetTanks/
```

## Scene graph

```text
Bootstrap.unity
  ProjectBootstrapper (runtime static)
  SceneLoaderService
  loads -> MainMenu

MainMenu.unity
  MainMenuBootstrapper (runtime-created)
  MainMenuView
  MainMenuPresenter
  Play Sandbox -> loads Sandbox
  Quit -> quits/editor-safe

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

Legacy scene names such as `Sand Box` and `RicochetTanks_Sandbox` are not part of the canonical flow.

## Folder graph

```text
Assets/_Project/RicochetTanks/
  Scenes/
    Bootstrap.unity
    MainMenu.unity
    Sandbox.unity
  Scripts/
    Configs/
      ArenaConfig.cs
      TankConfig.cs
      ProjectileConfig.cs
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
  -> wires SandboxHudPresenter
  -> wires SandboxMatchController

SandboxSceneBuilder
  -> creates 10x10 arena, walls, center obstacle
  -> creates camera/light/HUD/EventSystem
  -> creates DesktopInputReader
  -> creates ProjectileFactory
  -> creates Player and Enemy TankFacade

TankFacade
  -> TankHealth
  -> TankMovement
  -> TurretAiming
  -> TankShooter
  -> PlayerTankController (enabled only for player)

TankShooter
  -> ProjectileFactory
  -> spawns Projectile from muzzle

Projectile
  -> ignores owner during safe time
  -> moves via Rigidbody
  -> reflects via RicochetCalculator
  -> asks HitResolver for tank hits

SandboxMatchController
  -> listens to health deaths, restart, and hit events
  -> sets last hit / round result HUD text
  -> disables gameplay after match end
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
Bounce speed multiplier: 0.78
Owner safe time: short initial collision ignore
After safe time: projectile may hit shooter
Hit wall/obstacle: reflect by collision normal
After ricochet count exceeded: destroy projectile on next contact
Hit tank: apply fixed damage first
```

Future extension:

```text
Armor zones: front / side / rear
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
2. Bootstrap -> MainMenu -> Sandbox flow.
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
- full armor/penetration before first playable;
- multiplayer;
- advanced save system;
- monetization;
- over-engineered DI container;
- rewriting unrelated homework/course code.
