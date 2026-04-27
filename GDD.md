<<<<<<< Updated upstream
# GDD — World of Balance / Ricochet Tanks

**Версия:** 0.5 — русская редакция, правила рикошетов, брони и пробития  
**Автор:** Sergo Burnheart / BurnHeartGames  
**Движок:** Unity 6  
**Статус:** Milestone 1 — первый играбельный прототип в разработке  
**Дата обновления:** 27 апреля 2026

---

## 1. Видение

**World of Balance / Ricochet Tanks** — компактный прототип дуэли танков с видом сверху.

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

Камера для Milestone 1 — **строго сверху**. Не вид сбоку, не кинематографическая камера. Игрок должен воспринимать арену как игровую доску.

Главная идея боя:

> Побеждает не тот, у кого больше цифры, а тот, кто лучше использует угол, позицию, рикошеты и броню.

---

## 2. Принципы дизайна

### 2.1. Честная физика рикошетов

Поведение снаряда должно быть читаемым и стабильным.

Игрок должен понимать:

- куда летит снаряд;
- от какой поверхности он отскочит;
- сколько рикошетов уже было;
- почему снаряд пробил или не пробил;
- почему урон стал меньше после рикошета.

### 2.2. Читаемый минимализм

Прототип использует простую 3D-геометрию, чистые цвета, видимые снаряды, понятный HUD и минимум визуального шума.

Графика служит геймплею. Если эффект мешает видеть траекторию или попадание, он должен быть упрощён.

### 2.3. Угол важнее скрытых статов

Игрок должен выигрывать за счёт:

- правильного позиционирования;
- наведения башни;
- использования рикошетов;
- ромбования корпуса;
- атаки в слабые зоны;
- провокации врага на плохой выстрел.

### 2.4. Малый объём, но настоящая архитектура

Даже маленький прототип должен быть сделан аккуратно:

- без God Object;
- без огромного GameManager;
- с разделением UI, геймплея и конфигов;
- с событиями;
- с отдельными модулями для снарядов, брони, здоровья и матча;
- с возможностью расширить механику без переписывания всего проекта.

---

## 3. Первый играбельный демо

Каноничная рабочая сцена:
=======
# GDD Redirect

The current source of truth is:
>>>>>>> Stashed changes

```text
docs/GDD.md
```

<<<<<<< Updated upstream
Для первого демо нужна только `Sandbox.unity`.

Старые сцены типа `Sand Box` и `RicochetTanks_*` считаются legacy и не должны быть частью активного игрового потока.

- 10x10 dark greybox arena.
- Visible grid on the arena floor.
- Four dark ricochet walls.
- Center square cover / ricochet block.
- Green player tank at bottom-left.
- Red enemy dummy tank at top-right.
- Orthographic strict top-down camera.
- HUD with HP, last hit, round result, controls, restart.

- арену 10x10;
- видимую сетку пола;
- четыре стены для рикошетов;
- центральный квадратный блок;
- танк игрока снизу-слева;
- танк-манекен врага сверху-справа;
- ортографическую камеру сверху;
- HUD с HP, последним попаданием, результатом раунда, подсказками и кнопкой перезапуска.

Сцена может собираться кодом через `SandboxBootstrapper` и `SandboxSceneBuilder`, чтобы Unity-сцена оставалась почти пустой.

---

## 4. Стартовые параметры Milestone 1

| Параметр | Значение |
|---|---:|
| Арена | 10x10 |
| HP игрока | 300 |
| HP врага | 300 |
| Скорость танка | 4 |
| Поворот корпуса | 130°/с |
| Поворот башни | 220°/с |
| Скорость снаряда | 22 |
| Max Damage | 110 |
| Penetration | 45 |
| Max Ricochets | 3 |
| Speed after ricochet | x0.78 |
| Damage after ricochet | x0.75 |
| Min projectile speed | 5 |
| Projectile lifetime | 4 сек |
| Safe owner time | 0.15 сек |
| Reload time | 0.8 сек |
| Front Armor | 50 |
| Side Armor | 40 |
| Rear Armor | 10 |
| Auto Ricochet Angle | 70° |
| Restart delay | 1 сек |

Обязательные конфиги:

- `ArenaConfig`;
- `TankConfig`;
- `ProjectileConfig`;
- позже: `ArmorConfig`, `EnemyAIConfig`, `VFXConfig`.

- gameplay plane: XZ;
- Y is height only;
- arena center: `(0, 0, 0)`;
- Game View must be strict top-down.

Игровая плоскость — XZ.  
Y — высота.  
Центр арены — `(0, 0, 0)`.

| Объект | Позиция | Размер |
|---|---|---|
| Arena center | `(0, 0, 0)` | 10x10 |
| Player spawn | `(-3.5, 0, -3.5)` | faces center, appears bottom-left in Game View |
| Enemy spawn | `(3.5, 0, 3.5)` | faces center, appears top-right in Game View |
| Center obstacle | `(0, 0, 0)` | 2x2 square |
| North wall | `(0, 0, 5)` | boundary |
| South wall | `(0, 0, -5)` | boundary |
| East wall | `(5, 0, 0)` | boundary |
| West wall | `(-5, 0, 0)` | boundary |

Камера:

- type: Orthographic;
- first playable position: `(0, 10, 0)`;
- first playable rotation: `(90, 0, 0)`;
- orthographic size: around `6-7`;
- goal: full arena visibility from strict top-down Game View.

Isometric or tilted cameras are not allowed for the MVP demo, because they confuse scene assembly, projectile debugging, ricochet readability, and Codex/AI follow-up tasks.

---

## 6. Управление

ПК:

```text
W / S или ↑ / ↓       движение вперёд / назад по корпусу
A / D или ← / →       поворот корпуса
Мышь                  поворот башни
ЛКМ / Space           выстрел
R                     перезапуск
```

Правило: W/S — это не движение по мировым координатам вверх/вниз. Это движение вдоль направления корпуса танка.

Будущий мобильный слой:

```text
Левый джойстик        движение корпуса
Правая зона           наведение башни
Fire                  выстрел
Restart               перезапуск
```

Мобильный ввод должен быть отдельным слоем, не зашитым в логику танка.

---

## 8. Combat Rules

Milestone 1:

- Tanks start with 100 HP.
- Projectile speed is 22 units/sec.
- Projectile damage cap is 110.
- Projectile penetration is 45.
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
- Each ricochet multiplies speed by `0.78`.
- Projectile speed is clamped to minimum `5`.
- Reload time is `0.8` seconds.
- Safe owner time is `0.15` seconds.
- Basic armor values are front `50`, side `40`, rear `10`.
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
попадание
→ зона брони
→ угол
→ effectiveArmor
→ проверка penetration
→ качество пробития
→ итоговый урон
→ рикошет снижает потенциал
```

Ключевой принцип:

> Броня не просто режет HP. Броня решает, пробил снаряд или нет. Если пробил, качество пробития влияет на итоговый урон.

---

## 8. Снаряд

У снаряда есть два разных параметра:

### 8.1. Penetration

`Penetration = 45`

Пробитие отвечает на вопрос:

> Может ли снаряд пройти через эффективную броню?

### 8.2. Max Damage

`Max Damage = 110`

Это не гарантированный урон. Это верхний потолок.

Реальный урон зависит от:

- эффективной брони;
- качества пробития;
- количества рикошетов до попадания;
- возможного попадания в критическую зону.

---

## 9. Броня танка

Базовые значения:

| Зона | Armor |
|---|---:|
| Лоб | 50 |
| Борт | 40 |
| Корма | 10 |

Лоб должен держать прямые попадания лучше.  
Борт пробивается, если подставлен плохо.  
Корма уязвима и должна наказываться.

---

## 10. Угол и effectiveArmor

Эффективная броня растёт от угла попадания.

Прямое попадание почти не усиливает броню.  
Скользящее попадание сильно усиливает броню или вызывает рикошет.

Для Milestone 1 используется простая зависимость:

| Угол | Множитель |
|---:|---:|
| 0° | x1.0 |
| 45° | x1.5 |
| 70° | x2.5 |
| >70° | авто-рикошет |

Пример для борта:

| Угол | Борт |
|---:|---:|
| 0° | 40 |
| 45° | 60 |
| 70° | 100 |

Точные цифры можно крутить в конфигах. Важно сохранить саму зависимость:

> Чем более скользящее попадание, тем выше effectiveArmor.

---

## 11. Проверка пробития

После расчёта effectiveArmor игра сравнивает:

```text
projectilePenetration >= effectiveArmor
```

Если пробитие меньше эффективной брони:

```text
damage = 0
result = Ricochet или NoPenetration
```

Если пробитие равно или больше эффективной брони:

```text
result = Penetrated
далее считается качество пробития
```

---

## 12. Качество пробития и урон

Качество пробития:

```text
quality = penetration / effectiveArmor
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

| Quality | Damage |
|---:|---:|
| < 1.0 | 0% |
| 1.0-1.25 | 55% |
| 1.25-1.75 | 80% |
| >= 1.75 | 100% |

Урон считается от текущего потолка снаряда.

Пример без рикошета:

```text
MaxDamage = 110
quality = 1.1
damage ≈ 60
```

Пример хорошего пробития:

```text
MaxDamage = 110
quality = 2.0
damage = 110
```

---

## 13. Рикошеты

Снаряд отражается от стен, центрального блока и брони.

Отражение:

```text
Vector3.Reflect(direction, hitNormal)
```

Максимум:

```text
MaxRicochets = 3
```

После 3 рикошетов следующий контакт уничтожает снаряд.

Каждый рикошет снижает скорость:

```text
speed *= 0.78
```

Скорость не должна падать ниже:

```text
minSpeed = 5
```

Каждый рикошет снижает потенциал урона:

```text
currentDamageCap *= 0.75
```

То есть каждый рикошет снижает максимальный возможный урон на 25%.

| Рикошеты | Damage Cap |
|---:|---:|
| 0 | 110 |
| 1 | 82.5 |
| 2 | 61.875 |
| 3 | 46.4 |

Снаряд после рикошета остаётся опасным, но уже не должен быть таким же сильным, как прямой выстрел.

---

## 14. Примеры расчёта

### 14.1. Прямо в лоб

```text
FrontArmor = 50
AngleMultiplier = 1.0
EffectiveArmor = 50
Penetration = 45
```

Результат:

```text
45 < 50
NoPenetration / Ricochet
Damage = 0
```

### 14.2. Прямо в борт

```text
SideArmor = 40
AngleMultiplier = 1.0
EffectiveArmor = 40
Penetration = 45
Quality = 1.125
```

Результат:

```text
Penetrated
Damage ≈ 55% от DamageCap
```

### 14.3. Борт под углом

```text
SideArmor = 40
AngleMultiplier = 1.5
EffectiveArmor = 60
Penetration = 45
```

Результат:

```text
45 < 60
NoPenetration / Ricochet
Damage = 0
```

### 14.4. Корма

```text
RearArmor = 10
AngleMultiplier = 1.0
EffectiveArmor = 10
Penetration = 45
Quality = 4.5
```

Результат:

```text
Penetrated
Damage = 100% от DamageCap
```

### 14.5. Попадание после одного рикошета

```text
BaseMaxDamage = 110
After 1 ricochet = 82.5
Quality >= 1.75
```

Результат:

```text
Damage ≈ 82.5
```

---

## 15. Критическая зона / БК

В будущем у танка должна быть маленькая критическая зона:

```text
AmmoRack / БК
```

Правило:

- если снаряд не пробил броню, крит не срабатывает;
- если снаряд пробил и попал в БК, танк уничтожается сразу;
- БК должен быть отдельной зоной/компонентом, а не случайным `if` внутри Projectile.

Для MVP можно сначала отложить БК, но архитектура должна позволять его добавить.

---

## 16. Форма корпуса и тактика

Пока танки прямоугольные, игрок может танковать углом:

```text
враг стреляет → танк стоит ромбом → effectiveArmor выше
```

Если в будущем добавить танк с клиновидным носом, тактика меняется.

Клиновидному танку выгоднее стоять ровно носом к врагу, потому что наклон брони уже задан формой корпуса.

Вывод:

> В будущем логика должна поддерживать не только зоны Front/Side/Rear, но и бронепластины.

Будущая структура:

```text
Tank
├── ArmorPlate_Front
├── ArmorPlate_LeftSide
├── ArmorPlate_RightSide
├── ArmorPlate_Rear
├── ArmorPlate_NoseLeft
├── ArmorPlate_NoseRight
└── CriticalZone_AmmoRack
```

---

## 17. События геймплея

Основные события:

- `HealthChanged`;
- `Died`;
- `ProjectileSpawned`;
- `ProjectileHit`;
- `ProjectileBounced`;
- `HitResolved`;
- `MatchStarted`;
- `MatchFinished`;
- `RestartRequested`.

Правила:

- геймплейные системы поднимают события;
- UI слушает через презентеры;
- VFX/SFX слушают через отдельные обработчики;
- геймплей не должен напрямую вызывать UI;
- подписки должны отписываться;
- для подписок использовать именованные методы.

---

## 18. Отладка

Нужны короткие, фильтруемые логи.

Префиксы:

```text
[SHOT]
[BOUNCE]
[HIT]
[ARMOR]
[DAMAGE]
[CRITICAL]
[ROUND]
[AI]
[FLOW]
```

Примеры:

```text
[ARMOR] zone=Side baseArmor=40 angle=12 effectiveArmor=40 penetration=45 result=Penetrated
[DAMAGE] target=Enemy damage=61 hp=300->239
```

```text
[ARMOR] zone=Side baseArmor=40 angle=45 effectiveArmor=60 penetration=45 result=NoPenetration
[BOUNCE] count=1 speed=17.1 damageCap=82.5
```

```text
[ARMOR] zone=Rear baseArmor=10 angle=5 effectiveArmor=10 penetration=45 result=Penetrated
[DAMAGE] target=Enemy damage=110 hp=300->190
```

---

## 19. Техническое направление

Активный корень прототипа:

```text
Assets/_Project/RicochetTanks/
```

Важные элементы:

- `SandboxBootstrapper` — создаёт и связывает сцену;
- `SandboxSceneBuilder` — строит арену, танки, камеру, HUD и фабрики;
- `SandboxMatchController` — отвечает за раунд и перезапуск;
- `SandboxGameplayEvents` — события геймплея;
- `SandboxDebugVisualizer` — отладочная визуализация;
- `DesktopInputReader` — ввод клавиатуры/мыши;
- `TankFacade` — фасад танка;
- `TankMovement` — движение корпуса;
- `TurretAiming` — вращение башни;
- `TankShooter` — стрельба;
- `ProjectileFactory` — создание снарядов;
- `Projectile` — движение, рикошеты, lifetime;
- `HitResolver` — расчёт результата попадания;
- `TankArmor` — зоны брони и effectiveArmor;
- `TankHealth` — HP и смерть.

---

## 20. Правила архитектуры

- Никаких Singleton.
- Никакого огромного `GameManager`.
- UI View только отображает данные и поднимает события.
- Presenter/Controller связывает UI с геймплеем.
- Projectile не лезет напрямую во внутренности здоровья танка.
- Health не знает про углы и броню.
- Round logic отделена от здоровья и снарядов.
- EntryPoint/Bootstrapper только собирает зависимости.
- Приватные поля пишутся как `_camelCase`.
- Не использовать LINQ в `Update`, `FixedUpdate`, projectile systems, AI systems и горячих циклах.
- Не использовать `FindObjectOfType` / `GameObject.Find` в геймплейном цикле.

---

## 21. UI / HUD

HUD должен показывать:

- HP игрока;
- HP врага;
- последний результат попадания;
- результат раунда;
- подсказки управления;
- кнопку перезапуска.

HUD не должен:

- наносить урон;
- создавать снаряды;
- решать исход раунда;
- рассчитывать броню.

---

## 22. Дорожная карта

### Milestone 0 — документация и репозиторий

- [x] Обновить `README.md`.
- [x] Обновить `GDD.md`.
- [x] Зафиксировать первый playable scope.
- [x] Добавить стартовые значения баланса.
- [x] Добавить правила рикошетов, брони и пробития.
- [x] Добавить чек-лист тестирования.

### Milestone 1 — первый playable sandbox

- [ ] Bootstrap → MainMenu → Sandbox.
- [ ] Арена 10x10 + центральный блок.
- [ ] Игрок: движение, башня, стрельба.
- [ ] Враг-манекен: HP, смерть.
- [ ] Быстрый видимый снаряд.
- [ ] 3 рикошета.
- [ ] Потеря урона после рикошета.
- [ ] Броня и пробитие.
- [ ] HUD.
- [ ] Restart flow.
- [ ] Ручной чек-лист проходит.

### Milestone 2 — боевой характер

- [ ] Улучшить hit feedback.
- [ ] Добавить floating combat text.
- [ ] Добавить критическую зону БК.
- [ ] Добавить projectile prediction line.
- [ ] Добавить VFX/SFX попаданий.
- [ ] Подготовить пул снарядов/VFX.

### Milestone 3 — AI врага

- [ ] Простое движение бота.
- [ ] Наведение и стрельба.
- [ ] Reposition / Evade states.
- [ ] Debug AI state display.

### Milestone 4 — Mobile / Android

- [ ] Мобильный input layer.
- [ ] Fire button.
- [ ] Restart button.
- [ ] Android-safe UI.
- [ ] Android build.

### Milestone 5 — портфолио

- [ ] Улучшить визуал арены.
- [ ] SFX/VFX pass.
- [ ] Скриншоты/GIF для devlog.
- [ ] Public README polish.

---

## 23. Чек-лист ручного теста

### 23.1. Сцены

- [ ] Проект стартует из Bootstrap.
- [ ] MainMenu открывается после Bootstrap.
- [ ] Play открывает Sandbox.
- [ ] Restart сбрасывает матч.
- [ ] Нет ошибок в Console.

### 23.2. Игрок

- [ ] Игрок спавнится снизу-слева.
- [ ] Игрок едет вперёд/назад.
- [ ] Игрок вращает корпус.
- [ ] Игрок не выезжает с арены.
- [ ] Башня следует за мышью.
- [ ] Игрок стреляет.

### 23.3. Снаряд

- [ ] Снаряд появляется из ствола.
- [ ] Снаряд не бьёт владельца сразу.
- [ ] Снаряд летит быстро, но видимо.
- [ ] Снаряд отскакивает от стен.
- [ ] Снаряд отскакивает от центрального блока.
- [ ] Снаряд исчезает после лимита рикошетов.
- [ ] Снаряд теряет скорость после рикошета.
- [ ] Снаряд теряет damage cap после рикошета.

### 23.4. Бой

- [ ] Снаряд может попасть во врага.
- [ ] Лоб держит слабое пробитие.
- [ ] Борт пробивается при плохом угле.
- [ ] Борт под углом может дать рикошет.
- [ ] Корма получает высокий урон.
- [ ] После рикошета урон меньше.
- [ ] Враг умирает при 0 HP.
- [ ] Игрок может погибнуть от своего вернувшегося снаряда.
- [ ] Последний результат виден в HUD.

### 23.5. Архитектура

- [ ] Нет нового God Object.
- [ ] Нет gameplay `FindObjectOfType`.
- [ ] Подписки отписываются.
- [ ] Баланс в конфигах.
- [ ] UI не содержит боевую логику.
- [ ] Снаряд не лезет напрямую во внутренности `TankHealth`.

---

## 24. Definition of Done

Фича считается готовой, если:

- она вписана в существующий модуль или создаёт понятный новый модуль;
- нет God Object;
- значения вынесены в конфиги;
- зависимости передаются явно;
- события подписываются и отписываются корректно;
- UI отделён от геймплея;
- горячие циклы без лишних аллокаций;
- есть достаточные debug logs;
- сцена запускается после чистого клона;
- README/GDD обновлены;
- ручной чек-лист проходит.
=======
This root file is kept only as a compatibility pointer for old links.
>>>>>>> Stashed changes
