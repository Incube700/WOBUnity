# AI_CONTEXT_GRAPH.md — compact project map

Purpose: reduce token usage for Codex/AI tools. Read this file before scanning the whole repository.

## Project identity

- Project: **World of Balance / Мир баланса / Ricochet Tanks**
- Owner/brand: **BurnHeartGames**
- Repository: `Incube700/WOBLearnUnity`
- Engine target: **Unity 6**
- Prototype genre: top-down / light-isometric tank duel with ricochets
- Current main implementation task: GitHub issue `#9`

## Current prototype direction

The game is a minimal tank duel where position, angle, and ricochet matter more than stats.

Core loop:

```text
Start Sandbox -> move tank -> aim turret -> shoot -> ricochet projectile -> damage/destroy enemy -> restart or return to menu
```

## Desired scene graph

```text
Bootstrap
  ProjectBootstrapper
  SceneLoaderService
  loads -> MainMenu

MainMenu
  MainMenuView
  MainMenuPresenter
  Play Sandbox -> loads Sandbox
  Quit -> quits/editor-safe

Sandbox
  SandboxBootstrapper
  ArenaBuilder / ArenaFactory
  Player TankFacade
  Enemy Dummy TankFacade
  ProjectileFactory
  SandboxHudView
  SandboxHudPresenter
```

## Desired folder graph

```text
Assets/_Project/RicochetTanks/
  Scenes/
    Bootstrap.unity
    MainMenu.unity
    Sandbox.unity
  Scripts/
    Infrastructure/
      Bootstrap/
      SceneLoading/
    Gameplay/
      Arena/
      Tanks/
      Projectiles/
      Combat/
    Input/
      Desktop/
      Mobile/
    UI/
      MainMenu/
      Sandbox/
    Configs/
  Prefabs/
    Tanks/
    Projectiles/
    UI/
  Configs/
  Art/
    Greybox/
```

## Gameplay object graph

```text
SandboxBootstrapper
  -> creates/wires SceneLoaderService or receives it from project context
  -> references SandboxHudView
  -> references player TankFacade
  -> references enemy TankFacade
  -> references ProjectileFactory
  -> initializes presenters/controllers

TankFacade
  -> TankHealth
  -> TankMovement
  -> TurretAiming
  -> TankShooter

TankShooter
  -> ProjectileFactory
  -> spawns Projectile

Projectile
  -> moves every frame/fixed step
  -> asks RicochetCalculator/uses collision normal
  -> tracks ricochet count
  -> asks HitResolver/DamageService for tank hits

SandboxHudPresenter
  -> subscribes to player/enemy health changes
  -> updates SandboxHudView
  -> listens to RestartClicked
```

## Input graph

Desktop first:

```text
DesktopInputReader
  movement: WASD / Arrow keys
  aim: mouse world position
  fire: Left Mouse Button or Space
```

Mobile second:

```text
MobileInputView
  virtual joystick / movement pad
  fire button
  optional aim control later
```

Do not block Milestone 1 on advanced mobile aim.

## Ricochet rules

Milestone 1:

```text
Projectile speed: fast but visible
Max ricochets: 3
Hit wall/obstacle: reflect direction by collision normal
After ricochet: projectile may hit shooter
After ricochet count exceeded: destroy projectile
Hit tank: apply fixed damage first
```

Future extension:

```text
Angle-based damage:
  direct side hit -> damage
  shallow angle -> ricochet/no damage
  tank corner -> ricochet
```

Keep a separate class/function for hit resolving so this can be added later.

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
2. `README.md` — public project description.
3. `GDD.md` — design direction if present.
4. GitHub issue `#9` — implementation task.
5. This file — compact graph.

## Files AI should create/change first

Implementation branch:

```text
feature/ricochet-tanks-prototype
```

Expected first created files, if missing:

```text
Assets/_Project/RicochetTanks/Scripts/Infrastructure/SceneLoading/SceneLoaderService.cs
Assets/_Project/RicochetTanks/Scripts/Infrastructure/Bootstrap/ProjectBootstrapper.cs
Assets/_Project/RicochetTanks/Scripts/UI/MainMenu/MainMenuView.cs
Assets/_Project/RicochetTanks/Scripts/UI/MainMenu/MainMenuPresenter.cs
Assets/_Project/RicochetTanks/Scripts/Gameplay/Tanks/TankFacade.cs
Assets/_Project/RicochetTanks/Scripts/Gameplay/Tanks/TankHealth.cs
Assets/_Project/RicochetTanks/Scripts/Gameplay/Tanks/TankMovement.cs
Assets/_Project/RicochetTanks/Scripts/Gameplay/Tanks/TurretAiming.cs
Assets/_Project/RicochetTanks/Scripts/Gameplay/Tanks/TankShooter.cs
Assets/_Project/RicochetTanks/Scripts/Gameplay/Projectiles/Projectile.cs
Assets/_Project/RicochetTanks/Scripts/Gameplay/Projectiles/ProjectileFactory.cs
Assets/_Project/RicochetTanks/Scripts/Gameplay/Combat/RicochetCalculator.cs
Assets/_Project/RicochetTanks/Scripts/Gameplay/Combat/HitResolver.cs
Assets/_Project/RicochetTanks/Scripts/Input/Desktop/DesktopInputReader.cs
Assets/_Project/RicochetTanks/Scripts/UI/Sandbox/SandboxHudView.cs
Assets/_Project/RicochetTanks/Scripts/UI/Sandbox/SandboxHudPresenter.cs
Assets/_Project/RicochetTanks/Scripts/Infrastructure/Bootstrap/SandboxBootstrapper.cs
```

## Milestone priority

1. Compile.
2. Scene flow works.
3. Player can move.
4. Player can shoot.
5. Ricochet works.
6. Enemy can receive damage and die.
7. HUD and restart.
8. Mobile controls.
9. Enemy shooting/AI.

## Anti-goals

Do not spend time on:

- polished art;
- complex AI before Milestone 1;
- advanced save system;
- monetization;
- over-engineered DI container;
- rewriting unrelated homework/course code.
