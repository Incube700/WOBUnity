# World of Balance / Ricochet Tanks

Unity 6 prototype: top-down tank duel with visible ricochet projectiles.

## Main Scene

Open:

```text
Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity
```

If the scene/prefabs/configs need to be regenerated, run this editor menu item:

```text
Tools/Ricochet Tanks/Generate Editor-Friendly Demo
```

The generator creates the demo scene, prefabs, configs, and adds the demo scene to Build Settings. It does not run automatically on editor load.

Legacy fallback scene:

```text
Assets/_Project/RicochetTanks/Scenes/Sandbox.unity
```

## Editor Workflow

Main editable objects in `RicochetTanks_Demo`:

- `SceneContext / GameplayEntryPoint`
- `ArenaRoot`
- `Floor`
- `Walls`
- `Obstacles`
- `SpawnPoints/PlayerSpawnPoint`
- `SpawnPoints/EnemySpawnPoint`
- `PlayerTank`
- `EnemyDummyTank`
- `CameraRig`
- `GameplayCanvas`

Move spawn points, tanks, walls, and obstacles in the scene. Do not change code for layout tests.
At runtime `GameplayEntryPoint` places tanks from `PlayerSpawnPoint` and `EnemySpawnPoint`, so spawn positions are scene data, not hardcoded code values.

## Configs

Generated configs live in:

```text
Assets/_Project/RicochetTanks/Configs/
```

Important assets:

- `PlayerTankConfig`
- `EnemyTankConfig`
- `ProjectileConfig`
- `MatchConfig`
- `CameraConfig`

Tune projectile speed, damage, lifetime, max ricochets, damage multiplier per bounce, ricochet offset, layer masks, tank HP, movement, turret speed, and camera values there.

## Prefabs

Generated prefabs live in:

```text
Assets/_Project/RicochetTanks/Prefabs/
```

Use:

- `PlayerTankPrefab`
- `EnemyDummyTankPrefab`
- `ProjectilePrefab`
- `WallSegmentPrefab`
- `ArenaBlockPrefab`
- `GameplayCanvasPrefab`

## Required Layers

- `RicochetReflectable` for walls/ricochet surfaces.
- `Tank` for player/enemy hitboxes.
- `Projectile` for projectile views.
- `Obstacle` for optional obstacle masks.

`ProjectileConfig.ReflectableMask` should include ricochet surfaces. `ProjectileConfig.HittableMask` should include tanks.

## Quick Ricochet Test

1. Open `RicochetTanks_Demo`.
2. Select `ProjectileConfig` and confirm max ricochets is `3` and damage multiplier is `0.75`.
3. Press Play.
4. Move with `W/S`, rotate hull with `A/D`, aim with mouse, fire with LMB or `Space`.
5. Shoot a wall at an angle.
6. Expected: projectile checks previous position to current position, bounces, rotates to the new direction, loses 25% damage, and disappears after ricochets are exhausted.

## Documentation

Main GDD:

```text
docs/GDD.md
```

Compact AI map:

```text
AI_CONTEXT_GRAPH.md
```
