# Graph Report - .  (2026-04-26)

## Corpus Check
- 43 files · ~320,362 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 189 nodes · 237 edges · 14 communities detected
- Extraction: 84% EXTRACTED · 16% INFERRED · 0% AMBIGUOUS · INFERRED: 38 edges (avg confidence: 0.79)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- [[_COMMUNITY_Runtime UI Builders|Runtime UI Builders]]
- [[_COMMUNITY_Input Action Wrapper|Input Action Wrapper]]
- [[_COMMUNITY_Player Health Sprites|Player Health Sprites]]
- [[_COMMUNITY_Tank Weapons Aiming|Tank Weapons Aiming]]
- [[_COMMUNITY_Ricochet Design Assets|Ricochet Design Assets]]
- [[_COMMUNITY_Player Input Damage|Player Input Damage]]
- [[_COMMUNITY_Enemy Cover Behavior|Enemy Cover Behavior]]
- [[_COMMUNITY_Movement Projectiles|Movement Projectiles]]
- [[_COMMUNITY_Bootstrap Shooting|Bootstrap Shooting]]
- [[_COMMUNITY_Scene Loading|Scene Loading]]
- [[_COMMUNITY_Strategy Interfaces|Strategy Interfaces]]
- [[_COMMUNITY_Explosion VFX|Explosion VFX]]
- [[_COMMUNITY_Legacy Player Shooter|Legacy Player Shooter]]
- [[_COMMUNITY_Movement Strategy|Movement Strategy]]

## God Nodes (most connected - your core abstractions)
1. `SandboxBootstrapper` - 13 edges
2. `EnemyController` - 11 edges
3. `@GameInputActions` - 10 edges
4. `PlayerController` - 9 edges
5. `MainMenuBootstrapper` - 7 edges
6. `Projectile` - 7 edges
7. `ExplosionVFX` - 7 edges
8. `Bullet` - 7 edges
9. `TankMovement` - 6 edges
10. `TurretController` - 5 edges

## Surprising Connections (you probably didn't know these)
- `Unity 3D MVP Implementation` --rationale_for--> `TankMovement`  [INFERRED]
  GDD.md → Assets/_Project/RicochetTanks/Scripts/Gameplay/TankMovement.cs
- `Projectile Ricochet Rule` --rationale_for--> `Projectile`  [INFERRED]
  GDD.md → Assets/_Project/RicochetTanks/Scripts/Gameplay/Projectile.cs
- `Unity 3D MVP Implementation` --rationale_for--> `Projectile`  [INFERRED]
  GDD.md → Assets/_Project/RicochetTanks/Scripts/Gameplay/Projectile.cs
- `Enemy Cover AI` --rationale_for--> `EnemyController`  [INFERRED]
  GDD.md → Assets/Scripts/Enemy/EnemyController.cs
- `Isolated RicochetTanks Prototype` --rationale_for--> `SandboxBootstrapper`  [INFERRED]
  Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Bootstrap.unity → Assets/_Project/RicochetTanks/Scripts/UI/SandboxBootstrapper.cs

## Hyperedges (group relationships)
- **Ricochet Gameplay Loop** — gdd_player_controls, playertankcontroller_playertankcontroller, tankmovement_tankmovement, turretaiming_turretaiming, tankshooter_tankshooter, projectile_projectile, tankhealth_tankhealth [INFERRED 0.84]
- **Isolated Prototype Scene Flow** — prototype_isolated_ricochet_tanks, projectbootstrapper_projectbootstrapper, mainmenubootstrapper_mainmenubootstrapper, sandboxbootstrapper_sandboxbootstrapper, sceneloaderservice_sceneloaderservice [INFERRED 0.86]
- **Visual Asset Pack** — asset_explosion_vfx_sheet, asset_tank_sprite_set, asset_tank_body_turret_sprites, asset_environment_textures, asset_chatgpt_concept_art [INFERRED 0.72]

## Communities

### Community 0 - "Runtime UI Builders"
Cohesion: 0.12
Nodes (6): MainMenuBootstrapper, RicochetTanks.UI, RicochetTanks.UI, SandboxBootstrapper, RicochetTanks.UI, UiFactory

### Community 1 - "Input Action Wrapper"
Cohesion: 0.12
Nodes (11): AddCallbacks(), Disable(), Enable(), @GameInputActions, Get(), IGameplayActions, RemoveCallbacks(), SetCallbacks() (+3 more)

### Community 2 - "Player Health Sprites"
Cohesion: 0.12
Nodes (6): Tank Body Turret Sprites, Tank Sprite Set, DamageReceiver, Health, PlayerController, TurretController

### Community 3 - "Tank Weapons Aiming"
Cohesion: 0.11
Nodes (6): SimpleCannonStrategy, TripleShotStrategy, RicochetTanks.Gameplay, TurretAiming, WeaponController, WeaponStrategy

### Community 4 - "Ricochet Design Assets"
Cohesion: 0.12
Nodes (11): Generated Tank Concept Art, Grass Stone Environment Textures, Bullet, Clean Code PDF Reference, Projectile Ricochet Rule, Ricochet Tank Arena Concept, MathAngles, Unity 6000.0.54f1 Project Version (+3 more)

### Community 5 - "Player Input Damage"
Cohesion: 0.12
Nodes (9): CoverPoint2D, DamageReceiver2D, GameInput, WASD Aim Fire Controls, MonoBehaviour, PlayerTankController, RicochetTanks.Gameplay, RicochetTanks.Gameplay (+1 more)

### Community 6 - "Enemy Cover Behavior"
Cohesion: 0.2
Nodes (3): EnemyController, Enemy Cover AI, TankMover

### Community 7 - "Movement Projectiles"
Cohesion: 0.14
Nodes (5): Unity 3D MVP Implementation, Projectile, RicochetTanks.Gameplay, RicochetTanks.Gameplay, TankMovement

### Community 8 - "Bootstrap Shooting"
Cohesion: 0.22
Nodes (5): ProjectBootstrapper, RicochetTanks.Infrastructure, Isolated RicochetTanks Prototype, RicochetTanks.Gameplay, TankShooter

### Community 9 - "Scene Loading"
Cohesion: 0.29
Nodes (2): RicochetTanks.Infrastructure, SceneLoaderService

### Community 10 - "Strategy Interfaces"
Cohesion: 0.29
Nodes (3): MovementStrategy, ScriptableObject, WeaponStrategy

### Community 11 - "Explosion VFX"
Cohesion: 0.38
Nodes (2): Explosion VFX Sprite Sheet, ExplosionVFX

### Community 12 - "Legacy Player Shooter"
Cohesion: 0.5
Nodes (1): PlayerShooter

### Community 13 - "Movement Strategy"
Cohesion: 0.5
Nodes (2): MovementStrategy, StandardMovementStrategy

## Knowledge Gaps
- **18 isolated node(s):** `RicochetTanks.UI`, `RicochetTanks.UI`, `RicochetTanks.UI`, `RicochetTanks.Gameplay`, `RicochetTanks.Gameplay` (+13 more)
  These have ≤1 connection - possible missing edges or undocumented components.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `GameInput` connect `Player Input Damage` to `Input Action Wrapper`?**
  _High betweenness centrality (0.214) - this node is a cross-community bridge._
- **Why does `SandboxBootstrapper` connect `Runtime UI Builders` to `Bootstrap Shooting`, `Scene Loading`, `Player Input Damage`?**
  _High betweenness centrality (0.201) - this node is a cross-community bridge._
- **What connects `RicochetTanks.UI`, `RicochetTanks.UI`, `RicochetTanks.UI` to the rest of the system?**
  _18 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Runtime UI Builders` be split into smaller, more focused modules?**
  _Cohesion score 0.12 - nodes in this community are weakly interconnected._
- **Should `Input Action Wrapper` be split into smaller, more focused modules?**
  _Cohesion score 0.12 - nodes in this community are weakly interconnected._
- **Should `Player Health Sprites` be split into smaller, more focused modules?**
  _Cohesion score 0.12 - nodes in this community are weakly interconnected._
- **Should `Tank Weapons Aiming` be split into smaller, more focused modules?**
  _Cohesion score 0.11 - nodes in this community are weakly interconnected._