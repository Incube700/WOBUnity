# World of Balance / Ricochet Tanks

Unity 6 prototype for a top-down 1v1 tank duel built around ricochet shots, armor angles, and readable arena positioning.

## Current Prototype Status

The current playable flow is MainMenu -> RicochetTanks_Demo:

```text
Assets/_Project/RicochetTanks/Scenes/MainMenu.unity
Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity
```

Implemented in code/assets:

- Player tank and enemy dummy tank.
- Editable greybox arena with walls/obstacles.
- Strict top-down orthographic camera.
- Desktop tank controls with acceleration, braking/coasting, reverse, hull turning, and independent turret aim.
- Projectile shooting, wall ricochet, speed reduction, damage reduction, and limited bounce count.
- Tank armor zones: front, side, rear.
- Armor penetration/no-penetration/ricochet resolution.
- HP, death, win/lose, HUD, restart request flow.
- Combat feedback: world-space HP bars and floating hit text.
- Mobile controls prototype, VFX feedback, and recoil prototype.

Core Play Mode smoke checks were owner-verified in Unity on 2026-04-29. Android APK/device testing and HP bar prefab polish remain follow-up work. See `docs/TECH_STATUS.md`.

## How To Run

1. Open the project in Unity.
2. Open `Assets/_Project/RicochetTanks/Scenes/MainMenu.unity` for the full flow, or `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity` for quick combat testing.
3. Press Play.
4. If starting from MainMenu, press Play.
5. Test movement, aiming, shooting, ricochet, armor hits, HP bars, floating text, win/lose, and restart.

## Controls

| Input | Action |
|---|---|
| `W` / `Up Arrow` | Accelerate forward |
| `S` / `Down Arrow` | Brake, then reverse |
| `A` / `Left Arrow` | Rotate hull left |
| `D` / `Right Arrow` | Rotate hull right |
| Mouse | Aim turret |
| Left Mouse Button / `Space` | Fire |
| `R` | Restart |

## Important Configs

Configs live in:

```text
Assets/_Project/RicochetTanks/Configs/
```

Current key assets:

- `ProjectileConfig`
- `PlayerTankConfig`
- `EnemyTankConfig`
- `CameraConfig`
- `MatchConfig`

Current balance highlights:

- Projectile damage: `110`
- Projectile penetration: `45`
- Max ricochets: `3`
- Bounce speed multiplier: `0.78`
- Damage multiplier per bounce: `0.75`
- Player HP: `100`
- Enemy HP: `300`
- Armor: front `50`, side `40`, rear `10`

## Documentation

Documentation is split by purpose:

- `docs/GDD_RU.md` - main Russian GDD and design source of truth.
- `docs/GDD_EN.md` - English translation/adaptation.
- `docs/TECH_STATUS.md` - current implemented code/assets status.
- `docs/ROADMAP.md` - next tasks and milestone plan.
- `docs/MOBILE_CONTROLS.md` - mobile landscape controls design and prototype reference.
- `docs/GD_QUESTIONS.md` - guiding questions for game design decisions.
- `docs/GDD.md` - compatibility pointer for older links.

Compact AI/project map:

```text
AI_CONTEXT_GRAPH.md
```

## Remaining Manual Checks

- Build and test a local Android APK.
- Tune HP bar prefab readability if needed.
- Inspect materials in Unity for any broken/magenta visuals.

## Notes

`Tools/Ricochet Tanks/Generate Editor-Friendly Demo` exists for regeneration, but do not run it casually. The current workflow is to edit and test `RicochetTanks_Demo` directly.
