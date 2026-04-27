# Codex / AI Implementation Prompt — World of Balance

Используй этот промпт для дальнейшей разработки проекта **World of Balance / Мир Баланса** в Unity 6. Цель — добавлять фичи быстро, но не ломать архитектуру, производительность и читаемость проекта.

---

## 1. Project Context

Ты работаешь в Unity 6 проекте **World of Balance** от BurnHeartGames.

Это 3D top-down тактическая дуэль танков 1v1:

- компактная арена 10x10;
- игрок снизу-слева;
- противник/болванка сверху-справа;
- центральный квадратный блок/укрытие;
- независимые корпус и башня;
- быстрые снаряды;
- до 3 рикошетов, на 4-м столкновении снаряд исчезает;
- зональная броня: front/side/rear;
- результат попадания зависит от угла, пробития и оставшейся энергии;
- Desktop + Mobile input;
- Bootstrap → MainMenu → Sandbox scene flow.

Главные документы:

- `README.md` — GitHub landing page и краткое описание проекта.
- `GDD.md` — главный источник правды по дизайну и архитектуре.
- `AI_IMPLEMENTATION_PROMPT.md` — этот документ.

---

## 2. Non-Negotiable Architecture Rules

### 2.1. No God Objects

Не создавай и не расширяй монолитный `GameManager`, который знает обо всём.

Запрещено:

- огромный `Update` на 300+ строк;
- `GameManager.Instance`;
- `FindObjectOfType`/`GameObject.Find` в gameplay logic;
- UI, input, combat, spawn и game flow в одном классе;
- логика бота через россыпь bool-флагов вместо FSM.

### 2.2. Bootstrap & Contexts

Используй контексты:

- `ProjectContext` — глобальные сервисы, живут всю игру;
- `SceneContext` — сервисы и объекты текущей сцены;
- Bootstrap/EntryPoint только создаёт, регистрирует, связывает и запускает.

DI-контейнер можно использовать в инфраструктуре, installers, factories и bootstrap. Gameplay-классам контейнер напрямую не передавать. Они получают конкретные зависимости через constructor/init method.

### 2.3. Feature-Based Structure

Новые фичи добавляй по модулям:

```text
Assets/_Project/Scripts/
 ┣ Infrastructure/
 ┣ Common/
 ┣ Gameplay/
 ┃ ┣ ECS/
 ┃ ┣ Features/
 ┃ ┃ ┣ Arena/
 ┃ ┃ ┣ Tanks/
 ┃ ┃ ┣ Projectiles/
 ┃ ┃ ┣ Armor/
 ┃ ┃ ┣ CombatFlow/
 ┃ ┃ ┣ EnemyAI/
 ┃ ┃ ┣ Input/
 ┃ ┃ ┣ HUD/
 ┃ ┃ ┗ VFX/
 ┃ ┗ States/
 ┗ UI/
   ┣ Views/
   ┗ Presenters/
```

Если текущая структура проекта отличается, сначала найди ближайшее существующее место и аккуратно впиши фичу туда. Не создавай параллельную архитектуру без необходимости.

---

## 3. Coding Style

Следуй стилю проекта:

- C#;
- private поля: `_camelCase`;
- классы, методы, свойства: `PascalCase`;
- bool методы: `Is...`, `Can...`, `Has...`;
- try-методы возвращают `bool`: `TryShoot`, `TrySpend`, `TryGetTarget`;
- не использовать магические числа — выносить в config/const;
- избегать отрицательных условий там, где можно сделать код читаемее;
- не писать лишние комментарии, объясняющие очевидный код;
- комментарии допустимы для сложной математики рикошета/брони.

---

## 4. Lifecycle Rules

Для обычных C# классов используй lifecycle:

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

Правила:

- подписка на события — в `Initialize`;
- отписка — в `Dispose`;
- для `MonoBehaviour` допустимы `OnEnable/OnDisable`, если объект реально живёт в сцене;
- не смешивать `Construct` + `OnEnable` подписки без ясной причины;
- если подписка через лямбду неизбежна, сохранить delegate в поле, чтобы можно было отписаться. Лучше использовать именованный метод.

---

## 5. ECS / Data-Oriented Rules

Используй ECS/Data-Oriented слой там, где это даёт пользу:

- projectiles;
- damage requests;
- hit events;
- temporary VFX requests;
- bot runtime data;
- массовые объекты.

Если в проекте есть custom `EntitiesCore` и сгенерированный `EntityAPI`, не рассыпай `GetComponent<T>()` по gameplay-коду. Используй сгенерированные свойства/методы `entity.Xxx`.

После добавления или переименования компонента:

1. добавь/измени component;
2. сгенерируй Entity API через editor tool;
3. обнови код, использующий component;
4. проверь компиляцию;
5. не делай массовую миграцию без промежуточной проверки.

Не тащи Unity DOTS/ECS ради галочки, если текущий проект использует свой lightweight ECS.

---

## 6. State Machine Rules

FSM использовать для:

- enemy AI;
- match/game flow;
- возможно UI flow.

Enemy states на старте:

- `Idle`;
- `Patrol`;
- `Chase`;
- `Aim`;
- `Shoot`;
- `Evade`;
- `Dead`.

Каждый state должен иметь явный lifecycle:

```csharp
public interface IState
{
    void Enter();
    void Tick(float deltaTime);
    void Exit();
}
```

Правила:

- логика конкретного этапа живёт внутри state;
- переходы явные;
- нет россыпи bool-флагов, имитирующих state machine;
- подписки внутри state отписывать в `Exit` или `Dispose`;
- debug overlay должен показывать текущее состояние бота.

---

## 7. MVP UI Rules

UI делать через MVP.

### View

`MonoBehaviour`, который:

- показывает данные;
- проигрывает UI-анимации;
- отдаёт события кнопок;
- не принимает gameplay decisions.

### Presenter

Обычный C# класс, который:

- получает зависимости через constructor;
- подписывается в `Initialize`;
- отписывается в `Dispose`;
- преобразует данные модели/сервиса во view state;
- не должен быть `MonoBehaviour`.

Gameplay не должен напрямую менять UI. Он сообщает события/изменяет model, presenter обновляет view.

---

## 8. Config Rules

Все настройки баланса выносить в ScriptableObject:

- `TankConfig`;
- `ProjectileConfig`;
- `ArenaConfig`;
- `ArmorConfig`;
- `EnemyAIConfig`;
- `InputConfig`, если нужно;
- `VFXConfig`, если нужно.

Не хардкодить:

- скорость снаряда;
- количество рикошетов;
- размер арены;
- здоровье;
- броню;
- дистанции AI;
- cooldown;
- UI timings.

---

## 9. Performance Rules

Критично для мобильной цели и слабых устройств:

- не использовать LINQ в `Update`, `FixedUpdate`, ECS systems и частых циклах;
- не создавать мусор каждый кадр;
- использовать pooling для снарядов, VFX и floating text;
- не делать `new` в hot path без необходимости;
- не искать объекты через Unity search API в gameplay loop;
- physics/raycast делать осознанно, не спамить без причины;
- расчёты движения и рикошетов вести в XZ-плоскости.

---

## 10. Feature Integration Algorithm

Когда добавляешь новую фичу, действуй так:

1. Прочитай `README.md` и `GDD.md`.
2. Найди существующий feature-module, куда фича относится.
3. Если модуля нет — создай новый модуль с ясной ответственностью.
4. Определи public interfaces/events, не лезь во внутренности соседних модулей.
5. Добавь/обнови ScriptableObject config.
6. Добавь runtime/service/system классы.
7. Если есть UI — добавь View + Presenter.
8. Если есть поведение по этапам — добавь state/state machine.
9. Если есть динамические боевые сущности — рассмотри ECS component/system.
10. Добавь debug gizmos/logs для проверки.
11. Проверь компиляцию.
12. Обнови `GDD.md`/`README.md`, если изменилась механика или roadmap.

---

## 11. Current Implementation Priority

Сначала собрать First Playable, не расползаясь:

1. Bootstrap → MainMenu → Sandbox.
2. Arena 10x10 + central block.
3. Player tank movement + turret aiming.
4. Shooting + fast projectiles.
5. Ricochet up to 3 bounces.
6. Dummy enemy health/death.
7. HUD health/restart/result.
8. Mobile input layer.
9. Armor zones and penetration.
10. Enemy FSM bot.

Не добавлять новые “крутые” фичи, пока этот список не работает стабильно.

---

## 12. Acceptance Criteria

Перед завершением задачи проверь:

- проект компилируется;
- сцена запускается;
- нет новых God Object классов;
- зависимости не ищутся через `Find` в runtime logic;
- нет подписок без отписки;
- UI не содержит gameplay logic;
- Presenter не является MonoBehaviour;
- state machine не заменена bool-флагами;
- ECS API регенерирован, если менялись компоненты;
- нет LINQ/аллокаций в hot path;
- конфиги вынесены в ScriptableObject;
- добавлены понятные debug hooks;
- документация обновлена, если нужно.

---

## 13. Response Format for Codex

Когда предлагаешь изменения, выдавай результат так:

1. **Что меняется** — коротко.
2. **Какие файлы создать/изменить** — список путей.
3. **Почему так архитектурно правильно** — 3–5 пунктов.
4. **Код** — по файлам, полностью.
5. **Как проверить в Unity** — пошагово.
6. **Что не трогал** — чтобы не было скрытых поломок.

Не предлагай “переписать весь проект” без крайней необходимости. Делай маленькие безопасные итерации.
