# World of Balance / Ricochet Tanks

**by Sergo Burnheart**

Unity prototype for a top-down / light-isometric tank duel where positioning, aiming angles, and ricochets matter.

## Current Status

Milestone 1 is focused on a clean first playable demo:

- `Bootstrap -> MainMenu -> Sandbox` scene flow.
- 10x10 greybox arena with four walls and a center square obstacle.
- Player tank starts bottom-left; enemy dummy starts top-right.
- Desktop tank controls: hull movement/turning, independent turret aim, shooting, restart.
- Fast visible projectiles with a bright material and trail.
- Projectiles ricochet from walls/obstacles up to 3 times; the next contact destroys them.
- Tanks have HP; enemy can die; HUD shows HP, last hit, round result, controls, and restart.
- Runtime debug logs: `[SHOT]`, `[BOUNCE]`, `[HIT]`, `[ROUND]`.

Older course/homework code is kept outside the active prototype path. The playable prototype lives under:

```text
Assets/_Project/RicochetTanks/
```

## How To Run

Use Unity `6000.4.3f1`.

1. Open the project in Unity.
2. Open `Assets/_Project/RicochetTanks/Scenes/Bootstrap.unity`.
3. Press Play.
4. Click `Play Sandbox` in the main menu.

You can also open `Assets/_Project/RicochetTanks/Scenes/Sandbox.unity` directly and press Play.

## Controls

```text
W / S or Up / Down     Move forward / backward relative to the hull
A / D or Left / Right  Rotate hull
Mouse                  Aim turret
Left Mouse / Space     Fire
R                      Restart Sandbox
Restart button         Restart Sandbox
```

## Prototype Architecture

The current prototype uses small MonoBehaviours for Unity-facing objects and keeps wiring in scene bootstraps:

- `ProjectBootstrapper` starts the scene flow.
- `MainMenuView` + `MainMenuPresenter` handle menu UI.
- `SandboxBootstrapper` builds and wires the playable scene.
- `SandboxSceneBuilder` procedurally creates the greybox arena, tanks, camera, HUD, input reader, and projectile factory.
- `SandboxMatchController` owns match state, restart, win/loss result, and hit feedback.
- `TankFacade`, `TankMovement`, `TurretAiming`, `TankShooter`, and `TankHealth` split tank responsibilities.
- `ProjectileFactory`, `Projectile`, `RicochetCalculator`, and `HitResolver` handle shooting, ricochets, and damage.
- `ArenaConfig`, `TankConfig`, and `ProjectileConfig` keep gameplay numbers out of core logic.

## Deferred Features

Not part of Milestone 1 yet:

- Full armor model with front/side/rear zones.
- Kinetic penetration, damage falloff, and angle-based ricochet.
- Enemy AI and enemy shooting.
- Mobile controls beyond the planned input layer.
- VFX polish such as impact sparks, muzzle flash, and floating combat text.

## Design Docs

- Gameplay direction: [GDD.md](GDD.md)
- Compact AI/code map: [AI_CONTEXT_GRAPH.md](AI_CONTEXT_GRAPH.md)
