# World of Balance / Ricochet Tanks

**by Sergo Burnheart · BurnHeartGames**  
3D top-down tactical prototype about tanks, ricochets, armor angles and readable combat systems.

---

## 🎮 Коротко об игре

**World of Balance** — мой первый портфолио-прототип на Unity 6: дуэль танков 1v1 на компактной арене, где решают не цифры, а **позиция, угол выстрела, рикошет и контроль дистанции**.

Игрок управляет танком с независимым корпусом и башней. Снаряд летит быстро, может отскакивать от стен и препятствий до **3 раз**, теряет энергию после каждого рикошета и может вернуться в стрелявшего. Броня работает по зонам: лоб, борта, корма. Чем острее угол попадания, тем выше шанс рикошета или непробития.

> Идея простая: **меньше лишнего — больше драйва, честной физики и понятной тактики**.

---

## 🧪 Текущая цель разработки

Сейчас проект ведётся как **чистый Unity 6 прототип для портфолио**. Главная задача — собрать стабильный First Playable и дальше развивать его по документам:

- [GDD.md](GDD.md) — основной дизайн-документ и правила разработки.
- [AI_IMPLEMENTATION_PROMPT.md](AI_IMPLEMENTATION_PROMPT.md) — промпт для Codex/AI-ассистента, чтобы новые фичи интегрировались архитектурно правильно.

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
