# Game Design Document — World of Balance / Мир Баланса

**Версия:** 0.3  
**Автор:** Sergo Burnheart / BurnHeartGames  
**Движок:** Unity 6  
**Статус:** ранний прототип / First Playable in development

---

## 1. High Concept

**World of Balance / Мир Баланса** — это 3D top-down тактическая дуэль танков 1v1, где победа строится на позиционировании, угле выстрела, рикошетах, зональной броне и чтении траектории снаряда.

Игрок управляет танком на небольшой арене. Корпус и башня вращаются независимо. Снаряды быстрые, заметные, могут рикошетить от стен и препятствий до 3 раз, теряют энергию и могут вернуться в стрелявшего. Броня работает по зонам: лоб, борта, корма. Угол попадания решает, будет ли пробитие, непробитие или рикошет.

---

## 2. Design Pillars

### 2.1. Честная физика

Игрок должен чувствовать, что выстрел подчиняется понятным правилам: угол падения, нормаль поверхности, отражение, потеря скорости и энергии.

### 2.2. Минимализм ради читаемости

Первый прототип не гонится за графикой. Важны читаемые формы, контрастные цвета, понятная траектория снаряда, видимые попадания и ясный UI.

### 2.3. Угол важнее статов

Побеждает не тот, у кого “цифры больше”, а тот, кто лучше встал корпусом, поймал борт/корму противника, рассчитал рикошет и вовремя выстрелил.

### 2.4. Маленький scope, но взрослая архитектура

Проект делается как портфолио: даже маленькая фича должна быть встроена чисто — через feature-модуль, конфиги, события, state machine, ECS/Data-Oriented слой там, где это оправдано, и MVP для UI.

---

## 3. Target Experience

Матч должен ощущаться как короткая напряжённая дуэль:

1. Игрок появляется снизу-слева.
2. Противник появляется сверху-справа.
3. В центре арены стоит квадратный блок/укрытие.
4. Игрок пробует прямой выстрел, рикошет от стены или обход позиции.
5. Каждый снаряд летит быстро, но остаётся читаемым.
6. Попадание объясняется через UI: пробил, рикошет, не пробил, уничтожил.
7. После смерти одного танка матч заканчивается, игрок может быстро перезапустить Sandbox.

---

## 4. First Playable Scope

First Playable — это не “полная игра”, а маленькая стабильная версия, которую можно запускать, тестировать и показывать.

| Область | Требование |
|---|---|
| Сцены | `Bootstrap`, `MainMenu`, `Sandbox` |
| Арена | 10x10, границы, центральный квадрат/блок |
| Игрок | танк снизу-слева, движение, башня, выстрел |
| Противник | болванка сверху-справа, здоровье, смерть |
| Снаряды | быстрые, до 3 рикошетов, исчезновение на 4-м столкновении или при низкой скорости |
| Броня | зоны front/side/rear, базовый расчёт угла попадания |
| UI | здоровье игрока/врага, результат попадания, кнопка/клавиша рестарта |
| Desktop input | WASD/стрелки, мышь, ЛКМ/Space, R |
| Mobile input | виртуальный джойстик, кнопка выстрела, кнопка рестарта |
| Build target | Desktop для разработки, Android как важная целевая платформа |

---

## 5. Camera & Visual Direction

Для First Playable используется **top-down orthographic camera**. Допускается лёгкий изометрический угол позже, если он не ухудшает читаемость траекторий.

Визуальный стиль:

- простая 3D геометрия;
- танки из понятных примитивов: корпус + башня + дуло;
- контрастные цвета для игрока, врага, стен и снарядов;
- минимум визуального шума;
- важные VFX: выстрел, удар, рикошет, смерть.

---

## 6. Core Gameplay Loop

1. Игрок появляется на арене.
2. Игрок маневрирует корпусом и наводит башню.
3. Игрок стреляет прямым выстрелом или через рикошет.
4. Снаряд сталкивается с поверхностью, бронёй или целью.
5. Система боя рассчитывает результат попадания.
6. UI показывает результат.
7. При смерти танка CombatFlow переводит матч в состояние завершения.
8. Игрок перезапускает Sandbox и быстро тестирует следующий бой.

---

## 7. Controls

### 7.1. Desktop

| Input | Action |
|---|---|
| `W / S` или стрелки | движение вперёд/назад относительно корпуса |
| `A / D` | поворот корпуса |
| Mouse position | наведение башни |
| LMB / Space | выстрел |
| R | рестарт матча |
| Esc | выход в меню, позже |

### 7.2. Mobile

| Input | Action |
|---|---|
| Left virtual joystick | движение корпуса |
| Right aim area или auto-aim mode | наведение башни, в зависимости от тестов |
| Fire button | выстрел |
| Restart button | перезапуск Sandbox |

Mobile input должен быть отдельным слоем ввода, а не захардкоженным внутри танка.

---

## 8. Tank System

Танк состоит из нескольких логических частей.

### 8.1. Hull

Корпус отвечает за:

- движение вперёд/назад;
- поворот влево/вправо;
- инерцию и торможение;
- столкновения с ареной и препятствиями;
- ориентацию броневых зон.

Параметры выносятся в `TankConfig`:

- `maxHealth`;
- `moveSpeed`;
- `acceleration`;
- `rotationSpeed`;
- `friction`;
- `collisionRadius` или размеры корпуса;
- `frontArmor`, `sideArmor`, `rearArmor`;
- `reloadTime`.

### 8.2. Turret

Башня отвечает за:

- независимый поворот к aim-point;
- ограничение скорости поворота;
- spawn-point снаряда;
- проверку готовности к выстрелу через weapon/reload service.

### 8.3. Health & Death

- Здоровье не должно быть публичным изменяемым полем.
- Наружу отдавать read-only состояние и события.
- Входной невалидный урон `<= 0` — ошибка программиста, можно бросать исключение.
- Валидная игровая ситуация “танк уже мёртв” — безопасно игнорируется или возвращает `false` через `Try`-метод.

---

## 9. Projectile System

Снаряд — динамическая сущность, которую желательно вести через ECS/Data-Oriented слой или лёгкий runtime model + view.

### 9.1. ProjectileConfig

- `initialSpeed`;
- `minSpeed`;
- `radius`;
- `maxBounces = 3`;
- `bounceSpeedMultiplier`;
- `baseDamage`;
- `basePenetration`;
- `safeOwnerTime`;
- `maxLifetime`;
- `mass`;
- `caliber`.

### 9.2. Ricochet Rules

- столкновение с поверхностью даёт normal;
- направление отражается через `Vector3.Reflect` или аналогичный расчёт в XZ-плоскости;
- скорость уменьшается;
- `bounceCount` увеличивается;
- после 3 рикошетов следующий отскок/столкновение удаляет снаряд.

### 9.3. Safety Window

После выстрела снаряд короткое время не должен наносить урон владельцу. Это защищает от мгновенного самопопадания в дуло/корпус. После safe-time снаряд может убить стрелявшего, если вернулся рикошетом.

---

## 10. Armor & Damage System

### 10.1. Armor Zones

Танк имеет зоны:

- `Front` — самая сильная броня;
- `Side` — средняя броня;
- `Rear` — слабая броня.

Зона определяется через направление корпуса и точку попадания.

### 10.2. Hit Result

Система боя возвращает результат:

```csharp
public enum HitResultType
{
    Penetrated,
    Ricochet,
    NoPenetration,
    Destroyed
}
```

### 10.3. Формулы прототипа

- `kineticFactor = (currentSpeed / initialSpeed)^2`
- `currentPenetration = basePenetration * kineticFactor`
- `damage = baseDamage * kineticFactor^0.72`
- `effectiveArmor = armor / cos(angleOfImpact)`

Критический угол авторикошета настраивается через конфиг, стартовое значение около 70°.

---

## 11. Arena System

### 11.1. First Arena

- Размер: 10x10.
- Границы: четыре стены.
- Центр: один квадратный блок/укрытие.
- Player spawn: нижний левый сектор.
- Enemy spawn: верхний правый сектор.

### 11.2. Requirements

- Танки не проходят сквозь стены/блоки.
- Снаряды отражаются от стен/блоков.
- Конфигурация арены должна быть вынесена в `ArenaConfig`.
- Debug gizmos показывают границы, spawn points и normal при столкновениях.

---

## 12. Enemy AI

### 12.1. Этапы развития

**Stage 1 — Dummy**

- стоит на месте;
- получает урон;
- умирает;
- нужен для проверки стрельбы и рикошетов.

**Stage 2 — Simple FSM Bot**

Состояния:

- `Idle`;
- `Patrol`;
- `Chase`;
- `Aim`;
- `Shoot`;
- `Evade`;
- `Dead`.

**Stage 3 — Tactical Bot**

- держит предпочтительную дистанцию;
- использует укрытие;
- стреляет с упреждением;
- меняет позицию, если линия огня плохая.

### 12.2. State Machine Rules

- Логика состояния живёт внутри state-класса, а не в одном огромном `Update`.
- Переходы должны быть явными.
- Все подписки внутри state/controller должны отписываться.
- Никаких россыпей boolean-флагов вместо нормальной FSM.

---

## 13. UI / HUD

UI делается через MVP.

### 13.1. View

`MonoBehaviour` слой:

- отображает здоровье;
- показывает reload;
- показывает result text;
- отдаёт события кнопок (`PlayClicked`, `RestartClicked`, `FireClicked`).

View не принимает геймплейных решений.

### 13.2. Presenter

Обычный C# класс:

- получает сервисы/модели через constructor injection;
- подписывается в `Initialize`;
- отписывается в `Dispose`;
- преобразует данные модели в формат для View.

### 13.3. Screens

Минимальный набор:

- `MainMenuScreen`;
- `GameplayHudScreen`;
- `GameResultPopup`.

---

## 14. VFX / SFX

First Playable:

- muzzle flash;
- trail/trace для снаряда;
- hit spark;
- ricochet spark;
- explosion/death effect.

VFX не должен управлять боем. Он слушает события боя и отображает результат.

---

## 15. Scene Flow

```text
Bootstrap Scene
    ↓
ProjectContext / Global services
    ↓
MainMenu Scene
    ↓ Play
Sandbox Scene
    ↓ Match end / Restart
Sandbox Scene reload or soft reset
```

### 15.1. Bootstrap

Bootstrap отвечает только за:

- создание ProjectContext;
- регистрацию глобальных сервисов;
- запуск перехода в стартовую сцену;
- не содержит gameplay logic.

### 15.2. Sandbox

Sandbox отвечает за:

- создание SceneContext;
- загрузку конфигов сцены;
- создание арены, танков, UI;
- запуск CombatFlow;
- корректный Dispose при выходе.

---

## 16. Architecture Rules

### 16.1. Главный принцип

Никакого монолитного `GameManager`. Любая фича должна быть отдельным модулем с ясной ответственностью.

### 16.2. Contexts & DI

- `ProjectContext` живёт всю игру.
- `SceneContext` живёт только в рамках сцены.
- Дочерний scene-контейнер может резолвить зависимости из project-контейнера.
- Project-level код не должен знать scene-level объекты.
- DI-контейнер передаётся только в инфраструктуру: bootstrap, factories, installers.
- Gameplay-классы получают конкретные зависимости, а не контейнер.

### 16.3. Lifecycle

Для обычных C# сервисов/презентеров:

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

- Подписки делать в `Initialize`.
- Отписки делать в `Dispose`.
- `MonoBehaviour` использовать только там, где нужен Unity lifecycle, сцена, prefab, collider, view или gizmos.

### 16.4. ECS / Data-Oriented Layer

ECS использовать для динамических сущностей и массовой логики:

- projectiles;
- hit requests;
- damage requests;
- temporary VFX requests;
- bot/tank runtime components, если это упрощает систему.

Если используется сгенерированный Entity API, после добавления/переименования компонентов нужно регенерировать API и обновить код через `entity.Xxx`, а не рассыпать `GetComponent<T>()` по проекту.

### 16.5. State Machines

FSM использовать для:

- enemy AI;
- match/game flow;
- возможно UI flow.

State-класс должен иметь явные `Enter`, `Tick`, `Exit` или аналогичный lifecycle.

### 16.6. Events

- События именовать в прошедшем времени: `Died`, `HealthChanged`, `ProjectileHit`, `MatchFinished`.
- Подписываться именованными методами.
- Не использовать анонимные лямбды там, где нужна отписка.
- Не отдавать наружу изменяемые списки/реактивные переменные.

### 16.7. Performance

- Не использовать LINQ в `Update`, `FixedUpdate`, системах ECS и частых циклах.
- Пулить снаряды, VFX и floating text.
- Не делать `FindObjectOfType`/`GameObject.Find` в gameplay loop.
- Не аллоцировать каждый кадр без необходимости.
- Частые расчёты вести в XZ-плоскости без лишней 3D-сложности.

---

## 17. Feature Modules

### Infrastructure

DI container, ProjectContext, SceneContext, SceneSwitcherService, ConfigService, Resources/Addressables loading service.

### Arena Feature

Arena config, boundary creation, obstacles, collision normals, gizmos.

### Tanks Feature

Tank facade, hull movement, turret aiming, health, armor zones, death.

### Projectiles Feature

Projectile spawning, projectile pooling, movement, ricochet, lifetime, owner safety.

### Armor/Combat Feature

Hit detection, penetration calculation, damage application, hit result events.

### Input Feature

Desktop input reader, mobile input reader, common `IPlayerInput` interface, input events/commands.

### EnemyAI Feature

FSM, decision config, aiming service, movement decisions, debug state display.

### UI/HUD Feature

Views, presenters, screens, popups.

### VFX/SFX Feature

Visual event listeners, audio event listeners, pools.

---

## 18. Development Roadmap

### Milestone 0 — Documentation & Repository

- [x] Обновить `README.md`.
- [x] Обновить `GDD.md`.
- [x] Добавить `AI_IMPLEMENTATION_PROMPT.md`.
- [x] Сделать эти документы главным источником правды для разработки.

### Milestone 1 — First Playable Sandbox

- [ ] Bootstrap → MainMenu → Sandbox.
- [ ] Arena 10x10 + central block.
- [ ] Player tank movement + turret + shooting.
- [ ] Dummy enemy + health + death.
- [ ] Fast projectiles + 3 ricochets.
- [ ] Simple HUD.

### Milestone 2 — Combat Systems

- [ ] Armor zones.
- [ ] Penetration/ricochet/no-penetration.
- [ ] Floating hit result text.
- [ ] Projectile/VFX pools.

### Milestone 3 — Enemy FSM

- [ ] Simple bot movement.
- [ ] Aim/shoot logic.
- [ ] Evade/chase states.
- [ ] Debug display.

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

## 19. Definition of Done for Any Feature

Фича считается готовой, только если:

- она вписана в существующий feature-модуль или создан новый модуль;
- нет монолитного `GameManager`;
- конфиги вынесены в ScriptableObject;
- зависимости прокинуты через DI/factory/init method, а не найдены через `Find`;
- подписки корректно отписываются;
- UI идёт через MVP;
- частая логика не аллоцирует мусор каждый кадр;
- есть понятный debug/gizmos/log для проверки;
- сцена запускается без ручной магии после клона репозитория;
- README/GDD обновлены, если фича меняет дизайн или архитектуру.
