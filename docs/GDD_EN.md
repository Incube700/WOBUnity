# GDD - World of Balance / Ricochet Tanks

**Language:** English adaptation  
**Primary design source:** `docs/GDD_RU.md`  
**Engine:** Unity 6  
**Active prototype:** RicochetTanks_Demo  
**Synced:** 2026-04-28  
**Status:** playable prototype, manual Unity verification still required

This file is an English adaptation of the Russian GDD. The Russian design document is the source of truth. Current implementation details live in `docs/TECH_STATUS.md`; next tasks live in `docs/ROADMAP.md`.

## Vision

**World of Balance / Ricochet Tanks** is a compact top-down tank duel prototype.

Core combat idea:

> The winner is not simply the tank with bigger numbers, but the player who better uses angle, position, ricochets, and armor.

The player should read the arena like a board: hull direction, turret direction, projectile path, impact angle, weak armor zones, and ricochet surfaces.

## Camera And Coordinates

Milestone 1 uses a strict top-down camera. It is not side view, isometric, tilted, or cinematic orbit.

The game world uses the XZ plane:

- `X` is the horizontal map axis.
- `Z` is the vertical map axis.
- `Y` is height only.

Gameplay rules:

- tanks move on XZ;
- hulls rotate only around Y;
- turrets rotate only around Y;
- projectiles fly on XZ;
- ricochet reflection must not introduce accidental vertical movement.

## Design Principles

### Honest Ricochet Physics

Projectile behavior should be readable and stable. The player should understand where a shell is going, what it will bounce from, how many bounces happened, and why armor was penetrated or resisted.

### Readable Minimalism

The prototype uses simple 3D shapes, clean colors, visible projectiles, clear HUD, and low visual noise. Effects should support gameplay readability.

### Angle Beats Hidden Stats

The player should win through positioning, turret aim, ricochet use, angling the hull, attacking weak zones, and provoking bad enemy shots.

### Small Scope, Real Architecture

The prototype should stay small but clean: no God Object, no huge GameManager, separated gameplay/UI/configs, explicit events, and feature modules for projectiles, armor, health, match flow, and feedback.

## Current Playable Demo

Main scene:

```text
Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity
```

Demo goals:

- greybox arena;
- walls and obstacles for ricochet;
- player tank;
- enemy dummy tank;
- strict top-down orthographic camera;
- fast visible projectiles;
- wall/obstacle ricochet;
- tank armor ricochet/no-penetration;
- HP, death, win/lose;
- HUD and combat feedback;
- restart.

Anything that cannot be verified from code/assets is listed in `docs/TECH_STATUS.md` as **Needs Manual Unity Check**.

## Launch Instructions

1. Open the project in Unity.
2. Open `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity`.
3. Press Play.
4. Check movement, aiming, shooting, ricochet, armor hits, HP, floating hit text, win/lose, and restart.

## Controls

| Input | Action |
|---|---|
| `W` / `Up Arrow` | Accelerate forward along hull forward |
| `S` / `Down Arrow` | Brake, then reverse |
| `A` / `Left Arrow` | Rotate hull left |
| `D` / `Right Arrow` | Rotate hull right |
| Mouse | Aim turret |
| Left Mouse Button / `Space` | Fire |
| `R` | Restart |

Mobile controls are future work and should be implemented as a separate input layer.

## Projectile And Armor Model

Projectile has two separate combat concepts:

- `Penetration`: can the shell pass through effective armor?
- `Max Damage`: upper damage cap, not guaranteed damage.

Actual damage depends on effective armor, penetration result, ricochet damage reduction, and possible future critical zones.

Armor zones:

- local `+Z` is Front;
- local `-Z` is Rear;
- local `+/-X` is Side.

Effective armor grows with impact angle:

```text
dot = Vector3.Dot(-incomingDirection.normalized, hitNormal.normalized)
angle = acos(clamp(dot, -1, 1))
effectiveArmor = armor / max(cos(angle), safeMinCos)
```

Penetration rule:

```text
projectilePenetration >= effectiveArmor
```

If penetration is lower than effective armor, the result is `NoPenetration` or `Ricochet` with no damage. If penetration is equal or higher, the result is `Penetrated` and damage is applied.

## Ricochet Model

Projectiles can bounce from walls, obstacles, and tank armor when armor is not penetrated or auto-ricochet is triggered.

Reflection uses:

```text
Vector3.Reflect(direction, hitNormal)
```

After a ricochet:

- movement remains on XZ;
- speed is reduced by config;
- damage cap is reduced by config;
- remaining bounces decrease;
- after the bounce limit, the next contact destroys the projectile.

## Future Design Ideas

Critical ammo rack / `AmmoRack` is future work:

- no penetration means no critical hit;
- penetration plus ammo-rack hit can destroy the tank;
- ammo rack should be a separate zone/component, not a random conditional inside projectile logic.

Future hull shapes may use armor plates instead of only Front/Side/Rear zones.

## Architecture Rules

- No Singleton.
- No giant GameManager.
- UI views display data and raise UI events only.
- Presenters/controllers wire UI to services/gameplay.
- Gameplay systems raise gameplay events and do not instantiate UI.
- Projectile logic stays separate from health internals.
- Health does not know about armor angles.
- Round logic stays separate from health/projectiles.
- EntryPoint/Bootstrapper composes dependencies only.
- Private fields use `_camelCase`.
- Event subscriptions that need unsubscribe use named handlers.

## Links

- Current code/assets status: `docs/TECH_STATUS.md`
- Next tasks: `docs/ROADMAP.md`
- Compact AI context: `AI_CONTEXT_GRAPH.md`
