# World of Balance / Ricochet Tanks

**by Sergo Burnheart / BurnHeartGames**

Unity 6 prototype: top-down tank duel with ricochet projectiles.

## Как запускать сейчас

Открывать и запускать нужно только одну сцену:

```text
Assets/_Project/RicochetTanks/Scenes/Sandbox.unity
```

Шаги:

1. Открой проект в Unity `6000.4.3f1`.
2. Открой `Assets/_Project/RicochetTanks/Scenes/Sandbox.unity`.
3. Нажми Play.

Ничего вручную в сцену добавлять не нужно. `SandboxBootstrapper` создаётся автоматически и сам собирает арену, камеру, HUD, танки, input и projectile factory.

## Что должно быть видно в Play Mode

- Тёмная 10x10 арена с сеткой.
- Чёрные стены по периметру для рикошетов.
- Серый центральный квадратный блок / укрытие.
- Зелёный `Player Tank` внизу-слева.
- Красный `Enemy Dummy Tank` вверху-справа.
- Камера строго сверху, без вращения вокруг танка.
- HUD: HP игрока, HP врага, Last Hit, Round Result, Restart, подсказка управления.

## Управление

```text
W / S или Up / Down      вперёд / назад относительно корпуса
A / D или Left / Right   поворот корпуса
Mouse                    поворот башни
Left Mouse / Space       выстрел
R                        рестарт матча
Restart button           рестарт матча
```

## Если нужно собрать сцену вручную

Это запасной вариант. Обычно он не нужен.

1. Создай пустую сцену и назови её `Sandbox`.
2. Сохрани её в `Assets/_Project/RicochetTanks/Scenes/Sandbox.unity`.
3. Добавь пустой GameObject `SandboxBootstrapper`.
4. Повесь на него компонент:

```text
RicochetTanks.UI.Sandbox.SandboxBootstrapper
```

5. Нажми Play. Bootstrapper сам создаст всё остальное.

Не добавляй вручную player, enemy, стены, камеру или HUD, если используешь текущий procedural setup. Иначе можно получить дубли.

## Что уже работает

- Одна основная рабочая сцена `Sandbox`.
- Top-down камера.
- Procedural arena builder.
- Player movement и turret aiming.
- Shooting через LMB / Space.
- Visible projectile с TrailRenderer.
- Projectile летит через deterministic custom movement + SphereCast per tick, а не через Unity collision callbacks.
- Ricochet от стен, центрального блока и скользящих попаданий по броне танка.
- Debug visualization включён через `DebugVisualizationConfig`: projectile direction, predicted next segment, collision normal, bounce count, armor zone, hit angle, penetration, effective armor, enemy FSM state, spawn points, arena bounds.
- Core gameplay events: `HealthChanged`, `Died`, `ProjectileSpawned`, `ProjectileHit`, `ProjectileBounced`, `HitResolved`, `MatchStarted`, `MatchFinished`, `RestartRequested`.
- HUD обновляется через `SandboxHudPresenter`; gameplay controllers не вызывают UI view напрямую.
- Initial balance: arena 10x10, HP 100/100, projectile speed 22, damage 35, penetration 100, reload 0.8 sec, bounce multiplier 0.85, min projectile speed 5, safe owner time 0.15 sec.
- Armor balance: front 100, side 70, rear 40, auto ricochet angle 70 degrees.
- Damage по enemy dummy.
- HP HUD.
- Player Wins / Enemy Wins.
- Restart по `R` и кнопке.
- Debug logs: `[SHOT]`, `[BOUNCE]`, `[HIT]`, `[ROUND]`.

## Что ещё не реализовано

- Kinetic penetration и damage falloff.
- Подробная damage model поверх текущей базовой брони.
- Enemy AI.
- Mobile controls.
- Muzzle flash, sparks, impact marks, floating text.

## Основные файлы

- `Assets/_Project/RicochetTanks/Scenes/Sandbox.unity`
- `Assets/_Project/RicochetTanks/Scripts/Infrastructure/SandboxSceneBuilder.cs`
- `Assets/_Project/RicochetTanks/Scripts/UI/Sandbox/SandboxBootstrapper.cs`
- `Assets/_Project/RicochetTanks/Scripts/UI/Sandbox/SandboxMatchController.cs`
- `Assets/_Project/RicochetTanks/Scripts/Gameplay/Projectiles/Projectile.cs`
- `Assets/_Project/RicochetTanks/Scripts/Gameplay/Projectiles/ProjectileFactory.cs`
- `Assets/_Project/RicochetTanks/Scripts/Gameplay/Debug/SandboxDebugVisualizer.cs`
- `Assets/_Project/RicochetTanks/Scripts/Configs/DebugVisualizationConfig.cs`

## Design Docs

- [GDD.md](GDD.md)
- [AI_CONTEXT_GRAPH.md](AI_CONTEXT_GRAPH.md)
