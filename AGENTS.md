# AGENTS.md — AI working rules for World of Balance / Ricochet Tanks

This repository is used for the Unity prototype **World of Balance / Мир баланса / Ricochet Tanks**.

AI agents must treat this file as the highest-priority project guide after direct user instructions.

## Current main goal
Build a Unity 6 playable prototype:

- Bootstrap scene -> Main Menu -> Sandbox scene.
- Top-down 1v1 tank duel.
- 10x10 greybox arena.
- Center square obstacle.
- Player bottom-left.
- Enemy dummy top-right.
- Fast visible projectiles.
- Projectiles ricochet from walls/obstacles up to 3 times.
- Tanks have HP.
- Enemy can die.
- HUD shows HP.
- Restart works.
- Desktop controls first, mobile controls second.

## Work branch
Use a feature branch for implementation:

```text
feature/ricochet-tanks-prototype
```

Do not push gameplay implementation directly to `main` unless explicitly requested.

## Project isolation
Keep this prototype isolated from older course/homework code.

Preferred root:

```text
Assets/_Project/RicochetTanks/
  Scripts/
  Scenes/
  Prefabs/
  Configs/
  Art/Greybox/
```

If the repository already has a different but clear `_Project` layout, follow it without mixing unrelated homework code.

## Architecture rules
Keep the prototype small, playable, and clean.

- Avoid Singleton.
- Avoid huge all-in-one MonoBehaviours.
- Use MonoBehaviour for scene objects, views, Unity lifecycle wrappers, and serialized scene references.
- Put pure calculations and rules into plain C# classes where reasonable.
- UI View only displays data and raises events.
- Presenter/Controller wires UI to services/gameplay.
- Do not put gameplay logic into UI button handlers.
- Avoid `FindObjectOfType` spam.
- Use `[SerializeField]` references in bootstraps/views when scene references are required.
- Subscribe/unsubscribe cleanly.
- Do not use lambdas for event subscriptions when unsubscribe is needed; use named handlers.

## C# style
- Private fields: `_camelCase`.
- Classes, methods, properties: `PascalCase`.
- Boolean methods/properties: prefer `Is`, `Can`, `Has` prefixes.
- Keep methods small.
- Avoid magic numbers; use configs/constants.
- Prefer explicit access modifiers.

## Suggested structure

```text
Assets/_Project/RicochetTanks/Scripts/
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
```

## Suggested scene flow

```text
Bootstrap.unity
  -> ProjectBootstrapper
  -> SceneLoaderService
  -> loads MainMenu

MainMenu.unity
  -> MainMenuView
  -> MainMenuPresenter
  -> Play Sandbox button loads Sandbox

Sandbox.unity
  -> SandboxBootstrapper
  -> ArenaBuilder/ArenaFactory
  -> Player TankFacade
  -> Enemy TankFacade
  -> SandboxHudView + SandboxHudPresenter
```

## Suggested gameplay classes

```text
TankFacade
TankHealth
TankMovement
TurretAiming
TankShooter
Projectile
ProjectileFactory
RicochetCalculator
HitResolver
DesktopInputReader
MobileInputView
SandboxHudView
SandboxHudPresenter
```

Names can change if the existing project style requires it, but keep responsibilities separated.

## AI context economy
Before scanning the entire repository, read:

1. `README.md`
2. `GDD.md` if present
3. `AI_CONTEXT_GRAPH.md`
4. this `AGENTS.md`
5. GitHub issue `#9`

Do not repeatedly dump or re-read large files if the graph file is enough.

## Definition of done for Milestone 1
- Unity opens without compile errors.
- Bootstrap scene starts the flow.
- MainMenu opens Sandbox.
- Sandbox contains arena, player, enemy dummy.
- Player moves and aims turret on desktop.
- Player shoots.
- Projectile ricochets up to 3 times.
- Enemy receives damage and can die.
- HUD shows HP.
- Restart works.
- PR explains how to test.
