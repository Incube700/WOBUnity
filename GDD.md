# Game Design Document — World of Balance / Ricochet Tanks

## 1. Vision

Ricochet Tanks is a compact top-down tank duel prototype. The first playable must be readable immediately: one arena, two tanks, one central obstacle, visible projectiles, ricochets, HP, win/loss, restart.

The camera style for Milestone 1 is **strict top-down**, not side view and not cinematic orbit. The player should understand the arena like a board.

## 2. Milestone 1 Scene

Canonical working scene:

```text
Assets/_Project/RicochetTanks/Scenes/Sandbox.unity
```

Only `Sandbox.unity` is required for the playable demo. Old scenes such as `Sand Box` and `RicochetTanks_*` are legacy and should not exist in the active flow.

Scene content at runtime:

- 10x10 dark greybox arena.
- Visible grid on the arena floor.
- Four dark ricochet walls.
- Center square cover / ricochet block.
- Green player tank at bottom-left.
- Red enemy dummy tank at top-right.
- Orthographic top-down camera.
- HUD with HP, last hit, round result, controls, restart.

The scene is built by `SandboxBootstrapper` and `SandboxSceneBuilder`, so the Unity scene file can stay almost empty.

## 3. Controls

Desktop controls:

```text
W / S or Up / Down      move forward / backward relative to hull
A / D or Left / Right   rotate hull
Mouse                   rotate turret
Left Mouse / Space      fire
R                       restart match
```

Movement rule: W/S never means world up/down. It means forward/backward along the tank body direction.

## 4. Combat Rules

Milestone 1:

- Tanks start with 100 HP.
- Projectile speed is 22 units/sec.
- Projectile damage is 35.
- Projectile penetration is 100.
- Projectile is a bright visible sphere with TrailRenderer.
- Projectile spawns in front of the muzzle.
- Projectile ignores its owner briefly after firing.
- After safe time, a returning projectile can hit its owner.
- Projectile movement is deterministic custom movement with `SphereCast` checks per physics tick.
- Projectile ricochets manually from the hit normal using `Vector3.Reflect`.
- Ricochets work against arena walls and the center block.
- Glancing tank hits can ricochet from armor instead of dealing damage.
- Max ricochets: 3.
- After 3 ricochets, the next contact destroys the projectile.
- Each ricochet multiplies speed by `0.85`.
- Projectile speed is clamped to minimum `5`.
- Reload time is `0.8` seconds.
- Safe owner time is `0.15` seconds.
- Basic armor values are front `100`, side `70`, rear `40`.
- Auto ricochet angle is `70` degrees.
- Enemy death shows `Player Wins`.
- Player death shows `Enemy Wins`.
- Restart resets the match.

Gameplay event contracts:

- `HealthChanged`
- `Died`
- `ProjectileSpawned`
- `ProjectileHit`
- `ProjectileBounced`
- `HitResolved`
- `MatchStarted`
- `MatchFinished`
- `RestartRequested`

Gameplay systems raise these events. UI listens through presenters, VFX should listen through visual event handlers, and gameplay systems must not directly call UI views.

Debug visualization for First Playable:

- Toggle through `DebugVisualizationConfig`.
- Projectile direction.
- Predicted next projectile segment.
- Collision normal.
- Bounce count.
- Armor zone hit: Front / Side / Rear / Unknown.
- Hit angle.
- Current penetration.
- Effective armor.
- Enemy FSM state, currently `DummyIdle` / `Disabled`.
- Player/enemy spawn points.
- Arena bounds.

Debug logs:

```text
[SHOT]
[BOUNCE]
[HIT]
[ROUND]
```

## 5. Technical Direction

Active prototype root:

```text
Assets/_Project/RicochetTanks/
```

Important pieces:

- `SandboxBootstrapper` creates and wires the playable scene.
- `SandboxSceneBuilder` creates arena, tanks, camera, HUD, input, projectile factory.
- `SandboxMatchController` owns round state and restart.
- `SandboxGameplayEvents` exposes the core gameplay event contracts.
- `SandboxDebugVisualizer` listens to gameplay events and draws debug data.
- `DesktopInputReader` reads keyboard/mouse input.
- `TankFacade` connects movement, turret, shooter, health, controller.
- `TankMovement` owns hull movement.
- `TurretAiming` owns turret rotation.
- `TankShooter` delegates projectile creation.
- `ProjectileFactory` creates visible projectiles.
- `Projectile` owns movement, safe time, lifetime, ricochet count.
- `HitResolver` applies damage and publishes hit events.
- `TankArmor` resolves front / side / rear armor and auto ricochet angle.
- `TankHealth` owns HP and death.

Architecture rules:

- No Singleton.
- No giant GameManager.
- UI view only displays data and raises events.
- Presenter/controller wires systems.
- Projectile logic stays separate.
- Health logic stays separate.
- Round logic stays separate.
- Entry point only composes dependencies.
- No lambdas for event subscriptions that need unsubscribe.
- Private fields use `_camelCase`.

## 6. Future GDD Features

Not part of Milestone 1:

- Kinetic penetration and speed-based damage falloff.
- More detailed armor/damage model beyond the current basic armor checks.
- Enemy AI and enemy shooting.
- Mobile controls.
- Muzzle flash, sparks, impact marks, floating combat text.
