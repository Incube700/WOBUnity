# Codex / AI Implementation Prompt — World of Balance / Ricochet Tanks

Use this prompt when developing **World of Balance / Ricochet Tanks** in Unity 6.

Goal: implement features quickly, but do not break architecture, performance, scene flow, or the Milestone 1 scope lock.

---

## 1. Required Reading

Before changing code, read:

- `README.md`
- `GDD.md`
- `AI_IMPLEMENTATION_PROMPT.md`

`GDD.md` is the main source of truth. If code and docs disagree, report the mismatch before making a risky change.

---

## 2. Project Context

You are working on a Unity 6 project by BurnHeartGames.

Current prototype identity:

- compact 10x10 tank duel;
- Bootstrap -> MainMenu -> Sandbox scene flow;
- player tank bottom-left;
- enemy dummy tank top-right;
- center square obstacle;
- independent hull and turret;
- fast visible projectiles;
- up to 3 ricochets;
- projectile is destroyed on the next contact after max bounces;
- projectile ignores owner briefly after firing;
- after safe time, returning projectile can hit owner;
- desktop first playable comes before mobile controls;
- future identity includes armor zones and angle-based ricochet/penetration.

Current implementation is isolated in:

```text
Assets/_Project/RicochetTanks/
```

Important existing runtime pieces:

- `ProjectBootstrapper`
- `MainMenuView`
- `MainMenuPresenter`
- `SandboxBootstrapper`
- `SandboxSceneBuilder`
- `SandboxMatchController`
- `DesktopInputReader`
- `TankFacade`
- `ProjectileFactory`
- `HitResolver`
- `ArenaConfig`
- `TankConfig`
- `ProjectileConfig`

Prefer extending these cleanly over creating parallel duplicate systems.

---

## 3. Milestone 1 Scope Lock

First playable must stay focused:

1. Bootstrap -> MainMenu -> Sandbox.
2. 10x10 arena + center obstacle.
3. Player tank movement + turret aiming.
4. Shooting + fast visible projectiles.
5. Ricochet up to 3 bounces.
6. Dummy enemy health/death.
7. HUD with HP, last hit result, round result, restart.
8. Restart flow.
9. Manual test checklist passes.

Do **not** add before Milestone 1 is stable:

- multiplayer;
- progression/currency/shop;
- multiple tanks;
- multiple maps;
- complex enemy AI;
- destructible environment;
- different projectile types;
- dash/shield abilities;
- campaign/story;
- leaderboards;
- advanced VFX/SFX pass;
- monetization.

---

## 4. Initial Balance Values

Use config values, not magic numbers in gameplay code.

| Parameter | Start Value |
|---|---:|
| Arena size | 10x10 |
| Player HP | 100 |
| Enemy HP | 100 |
| Player move speed | 4 units/sec |
| Player rotation speed | 130 deg/sec |
| Turret rotation speed | 220 deg/sec |
| Projectile speed | 22 units/sec |
| Projectile damage | 35 |
| Projectile max bounces | 3 |
| Bounce speed multiplier | 0.78 |
| Min projectile speed | 5 units/sec |
| Projectile max lifetime | 4 sec |
| Safe owner time | 0.15 sec |
| Reload time | 0.8 sec |
| Restart delay after death | 1.0 sec |

Future armor values:

| Parameter | Start Value |
|---|---:|
| Front armor | 100 |
| Side armor | 70 |
| Rear armor | 40 |
| Auto ricochet angle | 70 degrees |

---

## 5. First Sandbox Layout

Gameplay plane is XZ.

| Object | Position | Notes |
|---|---|---|
| Arena center | `(0, 0, 0)` | 10x10 |
| Player spawn | `(-3.5, 0, -3.5)` | faces center |
| Enemy spawn | `(3.5, 0, 3.5)` | faces center |
| Center obstacle | `(0, 0, 0)` | 2x2 square |

Camera:

- Orthographic;
- position around `(0, 10, 0)`;
- rotation around `(90, 0, 0)`;
- orthographic size around `6`;
- full arena visible.

---

## 6. Non-Negotiable Architecture Rules

### 6.1. No God Objects

Forbidden:

- giant `GameManager`;
- `GameManager.Instance`;
- huge all-in-one MonoBehaviour;
- input + UI + combat + spawn + match flow in one class;
- random boolean flags instead of state machine when state machine is required;
- runtime `FindObjectOfType` / `GameObject.Find` in gameplay loop.

### 6.2. Bootstrap / Context Direction

Current scene bootstrappers are allowed and expected.

Long-term direction:

- `ProjectContext` for global services;
- `SceneContext` for scene services;
- bootstrapper wires scene dependencies;
- DI container is infrastructure only;
- gameplay classes receive concrete dependencies, not the container.

### 6.3. Feature-Based Integration

Do not create unrelated duplicate architecture. Fit new code into the existing RicochetTanks structure.

Preferred logical modules:

- Infrastructure / Bootstrap;
- MainMenu;
- Sandbox;
- Input;
- Tanks;
- Projectiles;
- Combat / Hit resolving;
- HUD / UI;
- VFX;
- future Armor;
- future EnemyAI.

---

## 7. Coding Style

- C#.
- Private fields: `_camelCase`.
- Classes, methods, properties: `PascalCase`.
- Bool methods: `Is...`, `Can...`, `Has...`.
- Try methods return bool: `TryShoot`, `TryGetTarget`.
- No magic numbers in gameplay code.
- Avoid unnecessary comments.
- Comments are allowed for non-obvious ricochet, physics, or armor math.
- Keep methods small and responsibilities clear.

---

## 8. Lifecycle Rules

For plain C# services/controllers/presenters use:

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
- MonoBehaviour may use `OnEnable` / `OnDisable` if it is a real scene/prefab object;
- prefer named event handlers;
- avoid anonymous lambdas for subscriptions that require unsubscribe;
- if a lambda is unavoidable, store the delegate in a field.

---

## 9. MVP UI Rules

UI views:

- are MonoBehaviours;
- display data;
- raise button/input events;
- do not apply damage;
- do not spawn projectiles;
- do not decide match results.

Presenters/controllers:

- wire UI to services/gameplay;
- subscribe/unsubscribe cleanly;
- convert gameplay state into view state;
- should not become overloaded God Objects.

Existing examples:

- `MainMenuView`
- `MainMenuPresenter`

Follow that separation for HUD/result/restart UI.

---

## 10. Projectile Physics Rules

Fast projectile handling must be stable.

Preferred approach:

- custom deterministic movement per tick;
- Raycast/SphereCast along the movement segment;
- ricochet from `hit.normal`;
- direction locked to XZ plane;
- speed reduced by bounce multiplier;
- destroy after max bounces next contact;
- destroy after max lifetime or low speed;
- owner safe time prevents immediate self-hit.

Do not rely only on Unity collision callbacks for high-speed projectiles.

---

## 11. ECS / Data-Oriented Direction

Use ECS/Data-Oriented style when it makes the project simpler and faster, especially for:

- projectiles;
- hit requests;
- damage requests;
- temporary VFX requests;
- future bot runtime data.

If the project has a custom `EntitiesCore` and generated `EntityAPI`, do not scatter `GetComponent<T>()` through gameplay code. Use generated `entity.Xxx` access and regenerate API after component changes.

Do not introduce Unity DOTS/ECS only for show. Use the project’s existing architecture first.

---

## 12. State Machine Rules

FSM is planned for:

- future enemy AI;
- match/game flow if match logic grows;
- possibly UI flow.

State classes should have explicit lifecycle such as:

```csharp
public interface IState
{
    void Enter();
    void Tick(float deltaTime);
    void Exit();
}
```

Rules:

- state logic lives inside states;
- transitions are explicit;
- no random bool-flag state soup;
- unsubscribe in `Exit` or `Dispose`;
- add debug display for current enemy state when AI arrives.

Enemy AI is out of scope for Milestone 1 except dummy enemy health/death.

---

## 13. Event Contracts

Core gameplay events to prefer:

- `MatchStarted`
- `MatchFinished`
- `RestartRequested`
- `HealthChanged`
- `Died`
- `ProjectileSpawned`
- `ProjectileBounced`
- `ProjectileHit`
- `ProjectileDestroyed`
- `HitResolved`
- `ReloadStarted`
- `ReloadFinished`

Rules:

- gameplay raises events;
- UI listens through presenters/controllers;
- VFX/SFX listen through dedicated handlers;
- gameplay must not directly call UI views;
- fact events use past tense when appropriate.

---

## 14. Debug Requirements

Required debug feedback:

```text
[SHOT]
[BOUNCE]
[HIT]
[ROUND]
[ARMOR]
[AI]
[FLOW]
```

Useful debug visualization:

- projectile direction;
- bounce count;
- collision normal;
- hit point;
- owner safe-time state;
- spawn points;
- arena bounds;
- future armor zone and hit angle;
- future enemy FSM state.

Debug must be toggleable or editor-only where possible.

---

## 15. Performance Rules

- No LINQ in `Update`, `FixedUpdate`, projectile systems, AI systems, or hot loops.
- Pool projectiles, VFX, and floating text when repeated spawning appears.
- No per-frame allocations without reason.
- No runtime object search in gameplay loops.
- Keep projectile/armor math on XZ plane.
- Avoid physics spam.

---

## 16. Feature Integration Algorithm

When adding a feature:

1. Read `README.md`, `GDD.md`, and this file.
2. Check whether the feature is in Milestone 1 or backlog.
3. If it is out of scope, do not implement it unless explicitly requested.
4. Find the existing module/class to extend.
5. Avoid duplicate systems.
6. Add/update configs for all gameplay numbers.
7. Add service/system/controller code with clear responsibilities.
8. Add View + Presenter/controller only if UI is needed.
9. Add debug logs/gizmos for verification.
10. Run/describe manual test checklist.
11. Update docs if mechanics, architecture, or roadmap changed.

---

## 17. Manual Test Checklist

Before finalizing a gameplay change, verify relevant items:

### Scene Flow

- Project starts from Bootstrap.
- MainMenu opens.
- Play opens Sandbox.
- Restart resets match.
- No Console errors on scene load.

### Player

- Player spawns bottom-left.
- Player moves forward/back.
- Player rotates hull.
- Player cannot leave arena.
- Turret follows aim point.
- Player can shoot.

### Projectile

- Projectile spawns from barrel.
- Projectile does not instantly hit owner.
- Projectile is fast but visible.
- Projectile bounces from wall.
- Projectile bounces from center obstacle.
- Projectile disappears after max bounces.
- Projectile disappears after max lifetime or low speed.

### Combat

- Projectile can hit enemy.
- Enemy loses HP.
- Enemy dies at 0 HP.
- Round ends after death.
- HUD shows last hit result.

### Architecture

- No new God Object.
- No gameplay `FindObjectOfType` / `GameObject.Find`.
- Subscriptions unsubscribe.
- Config values are not hardcoded.
- UI does not contain gameplay logic.

---

## 18. Acceptance Criteria

A task is acceptable only if:

- project compiles;
- relevant scene runs;
- no new God Object appears;
- dependencies are wired explicitly;
- UI and gameplay remain separated;
- hot paths avoid avoidable allocations;
- events unsubscribe correctly;
- config values are used for balance;
- debug feedback is enough to verify behavior;
- relevant manual tests are described or passed;
- docs are updated when design/architecture changes.

---

## 19. Response Format for Codex

When proposing changes, answer in this format:

1. **What changes** — short summary.
2. **Files to create/change** — paths.
3. **Architecture reason** — 3-5 points.
4. **Code** — complete files or precise patches.
5. **How to test in Unity** — step-by-step.
6. **What was not touched** — to avoid hidden breakage.

Do not propose rewriting the whole project unless absolutely necessary. Prefer small safe iterations.
