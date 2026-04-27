# Game Design Document — World of Balance / Ricochet Tanks

## 1. Vision

World of Balance / Ricochet Tanks is a compact tank duel prototype. The core fantasy is simple: win by reading the arena, aiming well, and using ricochets better than the opponent.

Milestone 1 is a first playable demo, not the full final game.

## 2. Milestone 1 Playable Demo

Scene flow:

```text
Bootstrap -> MainMenu -> Sandbox
```

Sandbox requirements:

- 10x10 greybox arena.
- Four boundary walls.
- Center square obstacle.
- Player tank in the bottom-left.
- Enemy dummy tank in the top-right.
- Top-down / light-isometric camera.
- HUD with player HP, enemy HP, last hit result, round result, controls hint, and restart.

## 3. Controls

Desktop is first priority:

```text
W / S or Up / Down     Move forward / backward relative to the hull
A / D or Left / Right  Rotate hull
Mouse                  Aim turret independently
Left Mouse / Space     Fire
R                      Restart Sandbox
```

Mobile controls are deferred until after the desktop first playable is stable.

## 4. Combat Rules

Current Milestone 1 rules:

- Tanks start with 100 HP.
- Projectile damage is fixed.
- Projectile speed is fast but readable.
- Projectile has a visible material and trail.
- Projectile ignores its owner briefly after firing.
- After safe time, a returning projectile can hit its owner.
- Projectile ricochets from walls and the center obstacle using `Vector3.Reflect`.
- Each ricochet multiplies speed by `0.78`.
- Projectile can ricochet 3 times; the next contact destroys it.
- Enemy death ends the round with `Player Wins`.
- Player death ends the round with `Enemy Wins`.

Debug feedback during development:

```text
[SHOT]
[BOUNCE]
[HIT]
[ROUND]
```

## 5. Technical Direction

Current implementation keeps the prototype isolated in:

```text
Assets/_Project/RicochetTanks/
```

Important runtime pieces:

- `ProjectBootstrapper` starts the canonical scene flow.
- `MainMenuView` and `MainMenuPresenter` keep UI display and button logic separate.
- `SandboxBootstrapper` wires scene-level dependencies.
- `SandboxSceneBuilder` procedurally creates the greybox demo scene.
- `SandboxMatchController` owns match state, restart, and win/loss.
- `DesktopInputReader` reads desktop controls.
- `TankFacade` exposes tank subsystems.
- `ProjectileFactory` creates visible projectiles.
- `HitResolver` applies damage and reports hit results.
- `ArenaConfig`, `TankConfig`, and `ProjectileConfig` hold gameplay numbers.

Architecture rules:

- No Singleton.
- No huge all-in-one MonoBehaviour.
- UI views display data and raise events.
- Presenters/controllers wire UI to services/gameplay.
- Gameplay logic does not live in UI button handlers.
- Scene references use serialized fields or explicit bootstrap wiring.
- Event subscriptions use named handlers and unsubscribe cleanly.

## 6. Future GDD Combat Core

After Milestone 1:

- Add armor zones: front, side, rear.
- Add hit outcomes: penetrated, ricochet, no penetration, wall ricochet.
- Add kinetic projectile data: initial speed, current speed, mass, base damage, base penetration.
- Add damage falloff based on current projectile speed.
- Add angle-based armor math and auto-ricochet thresholds.
- Add impact VFX, muzzle flash, sparks, impact marks, and floating result text.

## 7. Future Enemy AI

Enemy AI is intentionally out of scope for Milestone 1. Later versions should add:

- Search.
- Chase.
- Reposition.
- Engage.
- Evade.
- Line-of-sight checks.
- Simple lead aiming.
- HUD/debug display for AI state.
