# Game Design Document — World of Balance / Ricochet Tanks

**Version:** 0.4 — Production Rules & First Playable Lock  
**Author:** Sergo Burnheart / BurnHeartGames  
**Engine:** Unity 6  
**Status:** Milestone 1 First Playable in development

---

## 1. Vision

Ricochet Tanks is a compact top-down tank duel prototype. The first playable must be readable immediately: one arena, two tanks, one central obstacle, visible projectiles, ricochets, HP, win/loss, restart.

The camera style for Milestone 1 is **strict top-down**, not side view, not isometric, not tilted, and not cinematic orbit. The player should understand the arena like a board.

## 1.1. Camera and Coordinate System

The MVP demo uses a **strict top-down camera**, not an isometric or tilted camera.

### World Coordinate System

The game world is built on the XZ plane:

- `X` — horizontal map axis.
- `Z` — vertical map axis on the map.
- `Y` — height only.

Gameplay movement and combat must stay on the XZ plane:

- tanks move on XZ;
- tank hulls rotate around Y;
- turrets rotate around Y;
- projectiles fly on XZ;
- ricochet reflection must preserve XZ-plane movement;
- projectile ricochet must not introduce unwanted vertical movement.

### Main Camera

The main gameplay camera must be orthographic and look straight down.

Recommended MVP camera setup:

```text
Projection: Orthographic
Position:   (0, 10, 0)
Rotation:   (90, 0, 0)
Size:       6-7
```

The main demo must not use an isometric/tilted camera such as:

```text
Position: (0, 8, -7)
Rotation: (55, 0, 0)
```

Scene View in Unity may be angled for editing convenience, but Game View must show the arena strictly from above.

## 2. Milestone 1 Scene
The project must stay small in gameplay scope but clean in architecture. Every new feature should make the prototype more playable, more readable, or more portfolio-ready.

---

## 2.0.1 Design Pillars

### 2.1. Honest Ricochet Physics

Projectile behavior should be readable and consistent: direction, surface normal, reflection, bounce count, speed loss, and destruction rules must be easy to debug.

### 2.2. Readable Minimalism

The prototype uses simple 3D geometry, clear colors, visible projectiles, readable VFX, and a clean HUD. Visual style must support gameplay clarity first.

### 2.3. Angle Over Stats

The player should win by positioning, aiming, ricochet usage, and armor angling, not by hidden stats.

### 2.4. Small Scope, Real Architecture

Even small features must be integrated cleanly: feature modules, configs, events, state machines when needed, MVP for UI, and ECS/Data-Oriented logic where it gives practical value.

---

## 3. Milestone 1 Playable Demo

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
- Orthographic strict top-down camera.
- HUD with HP, last hit, round result, controls, restart.

The scene is built by `SandboxBootstrapper` and `SandboxSceneBuilder`, so the Unity scene file can stay almost empty.

| Parameter | Start Value |
|---|---:|
| Arena size | 10x10 |
| Player HP | 100 |
| Enemy HP | 100 |
| Player move speed | 4 units/sec |
| Player rotation speed | 130 deg/sec |
| Turret rotation speed | 220 deg/sec |
| Projectile speed | 22 units/sec |
| Projectile fixed Milestone 1 damage | 35 |
| Projectile max bounces | 3 |
| Bounce speed multiplier | 0.78 |
| Min projectile speed | 5 units/sec |
| Projectile max lifetime | 4 sec |
| Safe owner time | 0.15 sec |
| Reload time | 0.8 sec |
| Future front armor | 100 |
| Future side armor | 70 |
| Future rear armor | 40 |
| Future auto ricochet angle | 70 degrees |
| Floating hit text lifetime | 0.8 sec |
| Restart delay after death | 1.0 sec |

Required configs:

- `ArenaConfig`
- `TankConfig`
- `ProjectileConfig`
- later: `ArmorConfig`, `EnemyAIConfig`, `VFXConfig`

---

## 6. First Sandbox Layout

Coordinate system:

- gameplay plane: XZ;
- Y is height only;
- arena center: `(0, 0, 0)`;
- Game View must be strict top-down.

| Object | Position | Size / Notes |
|---|---|---|
| Arena center | `(0, 0, 0)` | 10x10 |
| Player spawn | `(-3.5, 0, -3.5)` | faces center, appears bottom-left in Game View |
| Enemy spawn | `(3.5, 0, 3.5)` | faces center, appears top-right in Game View |
| Center obstacle | `(0, 0, 0)` | 2x2 square |
| North wall | `(0, 0, 5)` | boundary |
| South wall | `(0, 0, -5)` | boundary |
| East wall | `(5, 0, 0)` | boundary |
| West wall | `(-5, 0, 0)` | boundary |

Camera:

- type: Orthographic;
- first playable position: `(0, 10, 0)`;
- first playable rotation: `(90, 0, 0)`;
- orthographic size: around `6-7`;
- goal: full arena visibility from strict top-down Game View.

Isometric or tilted cameras are not allowed for the MVP demo, because they confuse scene assembly, projectile debugging, ricochet readability, and Codex/AI follow-up tasks.

---

## 7. Controls

Desktop controls:

```text
W / S or Up / Down      move forward / backward relative to hull
A / D or Left / Right   rotate hull
Mouse                   rotate turret
Left Mouse / Space      fire
R                       restart match
```

Movement rule: W/S never means world up/down. It means forward/backward along the tank body direction.

Future mobile layer:

```text
Left virtual joystick  Hull movement
Right aim area         Turret aim, or auto-aim mode after testing
Fire button            Fire
Restart button         Restart Sandbox
```

Mobile input must be a separate input layer, not hardcoded inside tank logic.

---

## 8. Combat Rules

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
- Projectile direction must stay on the XZ plane after ricochet.
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

---

## 9. Physics Decision

Fast projectiles are risky if handled only through Unity collision callbacks, because they can tunnel through colliders. For Milestone 1 and beyond:

### 9.1. Tanks

Tank movement may use:

- Rigidbody-based movement with controlled velocity / `MovePosition` / `MoveRotation`; or
- controlled transform movement with explicit collision handling.

Tank `MonoBehaviour` should not contain all combat and match logic in `Update`.

### 9.2. Projectiles

Projectile movement should prefer deterministic custom movement:

- movement is calculated manually per tick;
- collision check uses `Raycast` or `SphereCast` along the movement segment;
- ricochet is calculated from `hit.normal`;
- direction is kept on the XZ plane;
- Unity collision callbacks must not be the only source of truth for high-speed projectile hits.

### 9.3. Collision Layers

Minimum useful layers:

- `Tank`
- `Projectile`
- `ArenaWall`
- `Obstacle`
- `UI`

Additional owner-ignore logic can be implemented through owner id, safe time, or temporary collision filtering.

---

## 10. Future Armor & Damage Core

Armor is not required for the earliest Milestone 1 if fixed damage is already playable, but it is a core identity feature and should be added after the ricochet loop is stable.

Future rules:

- armor zones: front, side, rear;
- hit outcomes: penetrated, ricochet, no penetration, wall ricochet;
- kinetic projectile data: initial speed, current speed, mass, base damage, base penetration;
- damage falloff based on current projectile speed;
- angle-based armor math and auto-ricochet thresholds;
- impact VFX, muzzle flash, sparks, impact marks, and floating result text.

Suggested hit result enum:

```csharp
public enum HitResultType
{
    Penetrated,
    Ricochet,
    NoPenetration,
    Destroyed
}
```

Prototype formulas:

```text
kineticFactor = (currentSpeed / initialSpeed)^2
currentPenetration = basePenetration * kineticFactor
damage = baseDamage * kineticFactor^0.72
effectiveArmor = armor / cos(angleOfImpact)
```

---

## 11. Event Contracts

Gameplay, UI, VFX, and match flow should communicate through explicit events and read-only state, not direct cross-calls.

Core events:

- `MatchStarted`
- `MatchFinished`
- `RestartRequested`
- `HealthChanged`
- `Died`
- `ProjectileSpawned`
- `ProjectileMoved`
- `ProjectileBounced`
- `ProjectileHit`
- `ProjectileDestroyed`
- `HitResolved`
- `ReloadStarted`
- `ReloadFinished`

Rules:

- Gameplay services/systems raise gameplay events.
- UI listens through presenters/controllers.
- VFX/SFX listen through dedicated handlers/listeners.
- Gameplay systems must not directly call UI views.
- Events describing facts should be named in past tense: `Died`, `HealthChanged`, `ProjectileBounced`.
- Subscriptions must unsubscribe cleanly in `Dispose` or `OnDisable`.
- Prefer named handlers over lambdas when unsubscribe is required.

---

## 12. Technical Direction

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
- World/gameplay logic uses XZ as the gameplay plane and Y as height only.
- The MVP camera is strict orthographic top-down, not isometric/tilted.

## 6. Future GDD Features

Not part of Milestone 1:

- Kinetic penetration and speed-based damage falloff.
- More detailed armor/damage model beyond the current basic armor checks.
- Enemy AI and enemy shooting.
- Mobile controls.
- Muzzle flash, sparks, impact marks, floating combat text.
- No huge all-in-one MonoBehaviour.
- UI views display data and raise events.
- Presenters/controllers wire UI to services/gameplay.
- Gameplay logic does not live in UI button handlers.
- Scene references use serialized fields or explicit bootstrap wiring.
- Event subscriptions use named handlers and unsubscribe cleanly.
- Config values live in ScriptableObjects or config classes, not magic numbers in gameplay code.

---

## 13. Architecture Rules

### 13.1. No God Object

Do not create a single `GameManager` that owns input, UI, spawning, combat, VFX, and match flow.

### 13.2. Bootstrap & Contexts

Long-term direction:

- `ProjectContext` for global services;
- `SceneContext` for scene services;
- scene bootstrapper wires scene dependencies;
- DI container is used only in infrastructure, installers, and factories;
- gameplay classes receive concrete dependencies, not the container.

### 13.3. Lifecycle

For plain C# services, presenters, and controllers:

```csharp
public interface IInitializable
{
    void Initialize();
}

public interface IDisposable
{
    void Dispose();
}
```

Rules:

- subscribe in `Initialize`;
- unsubscribe in `Dispose`;
- `MonoBehaviour` is for scene objects, views, colliders, prefabs, and gizmos;
- presenters should be plain C# objects, not `MonoBehaviour`.

### 13.4. ECS / Data-Oriented Direction

ECS/Data-Oriented logic is useful for:

- projectiles;
- hit requests;
- damage requests;
- temporary VFX requests;
- bot/tank runtime data if it simplifies the implementation.

If a generated Entity API exists, regenerate it after component changes and use `entity.Xxx` access instead of scattering `GetComponent<T>()` across gameplay code.

### 13.5. State Machines

FSM is planned for:

- enemy AI;
- match/game flow;
- possibly UI flow.

State logic must live inside state classes with explicit `Enter`, `Tick`, `Exit`, or equivalent lifecycle. Do not replace state machines with random boolean flags.

### 13.6. Performance

- No LINQ in `Update`, `FixedUpdate`, projectile systems, AI systems, or hot loops.
- Pool projectiles, VFX, and floating text.
- No runtime `FindObjectOfType` / `GameObject.Find` in gameplay loop.
- Avoid per-frame allocations.
- Keep projectile and armor math on the XZ plane.

---

## 14. UI / HUD

UI should follow MVP-style separation.

View responsibilities:

- display HP;
- display reload;
- display last hit result;
- display round result;
- raise button events;
- play UI-only animations.

Presenter/controller responsibilities:

- subscribe to model/service events;
- update view state;
- handle button events by calling services/controllers;
- unsubscribe cleanly.

UI must not apply damage, spawn projectiles, or decide match outcome.

---

## 15. Debug Tools

Debug tools are required because ricochet and armor systems are hard to tune blind.

Required debug visualization:

- projectile direction;
- current bounce count;
- collision normal;
- hit point;
- owner safe-time state;
- armor zone: front / side / rear, once armor is added;
- hit angle, once armor is added;
- current penetration, once armor is added;
- effective armor, once armor is added;
- hit result;
- enemy FSM state, once AI is added;
- spawn points;
- arena bounds.

Debug rules:

- Debug visualization must be toggleable through config or editor-only flag.
- Runtime player build must not be cluttered by debug UI.
- Use Gizmos/Handles editor-side where possible.
- Logs should be short and filterable.

Suggested log prefixes:

```text
[SHOT]
[BOUNCE]
[HIT]
[ROUND]
[ARMOR]
[AI]
[FLOW]
```

---

## 16. Future Enemy AI

Enemy AI is intentionally out of scope for Milestone 1. Later versions should add a simple FSM first, not a complex tactical brain.

Planned states:

- `Idle`
- `Search`
- `Chase`
- `Reposition`
- `Engage`
- `Evade`
- `Dead`

Future behavior:

- line-of-sight checks;
- simple lead aiming;
- preferred distance;
- movement around central obstacle;
- debug display for current AI state.

---

## 17. Player Skill Curve

The player should gradually learn:

1. Direct shooting.
2. Shooting while moving.
3. Aiming turret separately from hull.
4. Using walls for simple ricochet shots.
5. Protecting front armor, once armor is added.
6. Punishing side/rear armor, once armor is added.
7. Predicting energy loss after bounces.
8. Using the center obstacle as cover.
9. Baiting bad enemy shots.
10. Attacking from safety through ricochets.

---

## 18. Feature Backlog

Backlog stores ideas but does not authorize adding them before the first playable is stable.

### Priority A — After First Playable

- armor zones;
- better hit feedback;
- projectile prediction line;
- enemy FSM;
- mobile controls;
- restart/result polish;
- Android build check.

### Priority B — Prototype Expansion

- destructible walls;
- explosive barrels;
- different projectile types;
- dash/shield abilities;
- multiple arenas;
- local PvP/hotseat;
- improved VFX/SFX.

### Priority C — Future Game

- online multiplayer;
- progression;
- ranked duels;
- cosmetics;
- campaign/challenges;
- multiple tanks/classes;
- monetization experiments.

---

## 19. Portfolio Goals

This prototype should demonstrate:

- clean Unity architecture;
- feature-based project structure;
- Bootstrap/MainMenu/Sandbox flow;
- MVP-style UI separation;
- future-ready FSM enemy behavior;
- future-ready ECS/Data-Oriented projectile/combat logic;
- readable debug tools;
- mobile-ready input abstraction after desktop first playable;
- small but complete playable loop;
- ability to document and iterate like a real indie project.

---

## 20. Development Roadmap

### Milestone 0 — Documentation & Repository

- [x] Update `README.md`.
- [x] Update `GDD.md`.
- [x] Add `AI_IMPLEMENTATION_PROMPT.md`.
- [x] Lock first playable scope.
- [x] Add starting balance values.
- [x] Add first sandbox layout.
- [x] Add testing and debug expectations.

### Milestone 1 — First Playable Sandbox

- [ ] Bootstrap -> MainMenu -> Sandbox.
- [ ] 10x10 arena + center obstacle.
- [ ] Player tank movement + turret + shooting.
- [ ] Dummy enemy + health + death.
- [ ] Fast projectiles + 3 ricochets.
- [ ] HUD with HP, hit result, round result, restart.
- [ ] Restart flow.
- [ ] Manual test checklist passes.

### Milestone 2 — Combat Identity

- [ ] Armor zones.
- [ ] Penetration / ricochet / no penetration.
- [ ] Floating hit result text.
- [ ] Projectile/VFX pools.
- [ ] Debug visualization for hit math.

### Milestone 3 — Enemy FSM

- [ ] Simple bot movement.
- [ ] Aim/shoot logic.
- [ ] Reposition/evade states.
- [ ] Debug AI state display.

### Milestone 4 — Mobile Controls & Android

- [ ] Mobile input layer.
- [ ] Fire/restart buttons.
- [ ] Android-safe UI.
- [ ] Android build.

### Milestone 5 — Portfolio Polish

- [ ] Better arena visuals.
- [ ] SFX/VFX pass.
- [ ] Devlog-ready screenshots/GIFs.
- [ ] Public README polish.

---

## 21. Manual Test Checklist

Before committing or merging a gameplay feature, manually check the relevant parts.

### 21.1. Scene Flow

- [ ] Project starts from Bootstrap.
- [ ] MainMenu opens after Bootstrap.
- [ ] Play opens Sandbox.
- [ ] Restart resets match.
- [ ] No errors in Console on scene load.

### 21.2. Player

- [ ] Player spawns in bottom-left.
- [ ] Player can move forward/back.
- [ ] Player can rotate hull.
- [ ] Player cannot leave arena.
- [ ] Turret follows aim point.
- [ ] Player can shoot.

### 21.3. Projectile

- [ ] Projectile spawns from barrel.
- [ ] Projectile does not instantly hit owner.
- [ ] Projectile moves fast but visibly.
- [ ] Projectile bounces from wall.
- [ ] Projectile bounces from center obstacle.
- [ ] Projectile disappears after max bounces.
- [ ] Projectile disappears after max lifetime or low speed.

### 21.4. Combat

- [ ] Projectile can hit enemy.
- [ ] Enemy loses HP.
- [ ] Enemy dies at 0 HP.
- [ ] Round ends after enemy death.
- [ ] Player can also be killed by valid projectile logic.
- [ ] Last hit result appears on HUD.

### 21.5. UI / VFX

- [ ] HUD updates after damage.
- [ ] Restart button/key works.
- [ ] Ricochet feedback appears.
- [ ] Hit feedback appears.
- [ ] Round result appears.
- [ ] UI does not contain gameplay logic.

### 21.6. Architecture

- [ ] No new God Object.
- [ ] No gameplay `FindObjectOfType` / `GameObject.Find`.
- [ ] Subscriptions are unsubscribed.
- [ ] Config values are not hardcoded.
- [ ] Presenter/controller is not an overloaded MonoBehaviour.
- [ ] State logic is not hidden in random boolean flags.

---

## 22. Definition of Done for Any Feature

A feature is done only if:

- it fits into an existing feature/module or creates a clear new module;
- it does not create a God Object;
- gameplay values are in configs, not magic numbers;
- dependencies are passed through explicit bootstrap/factory/init wiring;
- event subscriptions are unsubscribed;
- UI stays separated from gameplay logic;
- hot loops avoid unnecessary allocations;
- there is enough debug/logging to verify behavior;
- the scene still runs after a clean clone/setup;
- README/GDD are updated if the feature changes design, roadmap, or architecture;
- the relevant manual test checklist passes.
