# GDD - World of Balance / Ricochet Tanks

**Язык:** русский, основной источник правды для дизайна  
**Движок:** Unity 6  
**Активный прототип:** RicochetTanks_Demo  
**Дата синхронизации:** 2026-04-28  
**Статус:** играбельный прототип требует ручной проверки в Unity

Этот файл восстанавливает полезное русское содержание из старого root `GDD.md` и отделяет дизайн от технического статуса. Текущая реализация вынесена в `docs/TECH_STATUS.md`, план работ - в `docs/ROADMAP.md`.

## 1. Видение

**World of Balance / Ricochet Tanks** - компактный прототип дуэли танков с видом строго сверху.

Главная идея боя:

> Побеждает не тот, у кого больше цифры, а тот, кто лучше использует угол, позицию, рикошеты и броню.

Игрок должен читать арену как игровую доску: направление корпуса, направление башни, траекторию снаряда, угол встречи с броней и поверхности для рикошета.

## 2. Камера и координаты

Камера для Milestone 1 - строго сверху. Не вид сбоку, не изометрия, не наклоненная кинематографическая камера.

Игровой мир построен на плоскости XZ:

- `X` - горизонтальная ось карты.
- `Z` - вертикальная ось карты.
- `Y` - только высота.

Геймплей должен оставаться на XZ:

- танки движутся по XZ;
- корпус танка вращается только вокруг Y;
- башня вращается только вокруг Y;
- снаряды летят по XZ;
- рикошет не должен вносить случайное вертикальное движение.

Рекомендуемый стиль камеры:

```text
Projection: Orthographic
Rotation:   (90, 0, 0)
```

Текущие значения камеры смотри в `docs/TECH_STATUS.md`, потому что они берутся из `CameraConfig`.

## 3. Принципы дизайна

### 3.1. Честная физика рикошетов

Поведение снаряда должно быть читаемым и стабильным.

Игрок должен понимать:

- куда летит снаряд;
- от какой поверхности он отскочит;
- сколько рикошетов уже было;
- почему снаряд пробил или не пробил;
- почему урон стал меньше после рикошета.

### 3.2. Читаемый минимализм

Прототип использует простую 3D-геометрию, чистые цвета, видимые снаряды, понятный HUD и минимум визуального шума.

Графика служит геймплею. Если эффект мешает видеть траекторию или попадание, он должен быть упрощен.

### 3.3. Угол важнее скрытых статов

Игрок должен выигрывать за счет:

- правильного позиционирования;
- наведения башни;
- использования рикошетов;
- ромбования корпуса;
- атаки в слабые зоны;
- провокации врага на плохой выстрел.

### 3.4. Малый объем, но настоящая архитектура

Даже маленький прототип должен быть сделан аккуратно:

- без God Object;
- без огромного GameManager;
- с разделением UI, геймплея и конфигов;
- с событиями;
- с отдельными модулями для снарядов, брони, здоровья и матча;
- с возможностью расширить механику без переписывания всего проекта.

## 4. Текущий playable demo

Основная сцена:

```text
Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity
```

Цель демо:

- арена greybox;
- стены и препятствия для рикошета;
- танк игрока;
- танк-манекен врага;
- ортографическая камера сверху;
- видимые быстрые снаряды;
- рикошеты от стен/препятствий;
- рикошет/непробитие от брони танков;
- HP, смерть, win/lose;
- HUD и combat feedback;
- перезапуск.

Все пункты, которые нельзя проверить автоматически из файлов, отмечены в `docs/TECH_STATUS.md` как **Needs Manual Unity Check**.

### 4.1. Текущее реализованное состояние

На уровне кода и ассетов сейчас есть:

- основной playable scene asset `RicochetTanks_Demo.unity`;
- танк игрока и enemy dummy tank;
- desktop-управление корпусом, башней и стрельбой;
- видимые снаряды с trail;
- рикошеты от стен/препятствий;
- armor zones `Front`, `Side`, `Rear`;
- проверка пробития через effective armor;
- no-penetration / ricochet без урона;
- HP, смерть, win/lose и restart flow;
- экранный HUD;
- Combat Feedback через world-space HP bars и floating hit text.

Needs Manual Unity Check:

- Play Mode без ошибок;
- визуальное уменьшение HP bars;
- floating damage / `NO PEN` / `RICOCHET`;
- restart без дублей подписок и UI;
- текущий размер арены, материалы и читаемость сцены.

### 4.2. Последний фидбек геймдизайнера

- HP bars - хорошая идея. Игрок должен ясно видеть текущий HP и сколько урона наносит каждый снаряд.
- Формулы damage, penetration, armor, ricochet и speed loss должны быть конкретизированы и записаны.
- Mobile landscape - следующий важный UX-направление.
- Для mobile нужны left virtual joystick для корпуса, right virtual joystick для башни/пушки, tap или fire button для выстрела.
- Для прототипа достаточно минимального VFX: projectile trail, небольшой hit/explosion effect, видимый impact feedback, smoke/wreck marker после уничтожения танка.
- Нужен recoil/knockback feeling на выстреле.
- Текущая потеря скорости после рикошета может визуально быть слишком маленькой. Нужно проверить и настроить.
- Геймдизайнеру удобнее отвечать на направляющие вопросы. Список вопросов ведется в `docs/GD_QUESTIONS.md`.
- Network/multiplayer - будущая перспектива, не ближайшая реализация.

## 5. Как запустить демо

1. Открыть проект в Unity.
2. Открыть сцену `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity`.
3. Нажать Play.
4. Проверить управление, стрельбу, рикошеты, броню, HP, floating text, win/lose и restart.

## 6. Управление

ПК:

```text
W / Up Arrow          ускорение вперед по корпусу
S / Down Arrow        торможение, затем задний ход
A / Left Arrow        поворот корпуса влево
D / Right Arrow       поворот корпуса вправо
Мышь                  наведение башни
ЛКМ / Space           выстрел
R                     перезапуск
```

Правило: W/S - это не движение по мировым координатам вверх/вниз. Это движение вдоль направления корпуса танка.

Будущий мобильный слой:

```text
Левый джойстик        движение корпуса
Правая зона           наведение башни
Fire                  выстрел
Restart               перезапуск
```

Мобильный ввод должен быть отдельным слоем, не зашитым в логику танка.

### 6.1. Mobile controls direction

Mobile controls пока не реализованы. Это следующий дизайн и prototype direction, описанный отдельно в `docs/MOBILE_CONTROLS.md`.

Базовое направление:

- landscape orientation;
- левый virtual joystick - движение танка и управление корпусом;
- правый virtual joystick - наведение башни/пушки;
- выстрел - tap или отдельная fire button, решение открыто;
- mobile controls не должны переписывать PC controls до утверждения схемы.

## 7. Снаряд

У снаряда есть два разных параметра:

### 7.1. Penetration

`Penetration` отвечает на вопрос:

> Может ли снаряд пройти через эффективную броню?

### 7.2. Max Damage

`Max Damage` - это не гарантированный урон. Это верхний потолок.

Реальный урон зависит от:

- эффективной брони;
- качества пробития;
- количества рикошетов до попадания;
- возможной будущей критической зоны.

Текущие числа берутся из `ProjectileConfig` и перечислены в `docs/TECH_STATUS.md`.

## 8. Броня танка

Базовая идея:

| Зона | Смысл |
|---|---|
| Лоб | самая крепкая зона |
| Борт | пробивается при плохом угле, может держать скользящие попадания |
| Корма | слабая зона, должна наказывать за плохую позицию |

Ориентация зон:

- `+Z` локального пространства танка - Front.
- `-Z` - Rear.
- `+/-X` - Side.

Текущие значения брони смотри в `docs/TECH_STATUS.md`.

## 9. Угол и effectiveArmor

Эффективная броня растет от угла попадания.

Прямое попадание почти не усиливает броню. Скользящее попадание сильно усиливает броню или вызывает рикошет.

Текущая формула реализации:

```text
dot = Vector3.Dot(-incomingDirection.normalized, hitNormal.normalized)
angle = acos(clamp(dot, -1, 1))
effectiveArmor = armor / max(cos(angle), safeMinCos)
```

Дизайн-принцип:

> Чем более скользящее попадание, тем выше effectiveArmor.

## 9.1. Конкретные формулы боя

Текущие формулы реализации должны оставаться явно задокументированными. Если формула меняется в коде, этот раздел и `docs/TECH_STATUS.md` нужно обновить.

Текущие входные значения из конфигов:

```text
ProjectileDamage = 110
ProjectilePenetration = 45
FrontArmor = 50
SideArmor = 40
RearArmor = 10
MaxRicochets = 3
BounceSpeedMultiplier = 0.78
DamageMultiplierPerBounce = 0.75
MinProjectileSpeed = 5
```

Текущая effective armor formula:

```text
impactDot = Dot(-incomingDirection.normalized, hitNormal.normalized)
angle = Acos(Clamp(impactDot, -1, 1))
effectiveArmor = armor / Max(Clamp01(impactDot), safeMinCos)
```

Пробитие:

```text
if penetration < effectiveArmor:
    result = NoPenetration
    damage = 0
else:
    result = Penetrated
    damage = currentDamage
```

Auto ricochet:

```text
if hitAngle >= AutoRicochetAngle:
    result = Ricochet
    damage = 0
```

Damage после рикошета:

```text
currentDamage = currentDamage * DamageMultiplierPerBounce
```

Speed после рикошета:

```text
currentSpeed = Max(MinProjectileSpeed, currentSpeed * BounceSpeedMultiplier)
```

При текущем `BounceSpeedMultiplier = 0.78` визуальная потеря скорости может быть недостаточно заметной. Это не меняется автоматически: нужна ручная проверка в Unity и отдельная tuning-задача.

## 10. Проверка пробития

После расчета effectiveArmor игра сравнивает:

```text
projectilePenetration >= effectiveArmor
```

Если пробитие меньше эффективной брони:

```text
damage = 0
result = NoPenetration или Ricochet
```

Если пробитие равно или больше эффективной брони:

```text
result = Penetrated
damage = расчетный урон
```

Ключевой принцип:

> Броня не просто режет HP. Броня решает, пробил снаряд или нет.

## 11. Рикошеты

Снаряд отражается от стен, препятствий и брони при непробитии/рикошете.

Отражение:

```text
Vector3.Reflect(direction, hitNormal)
```

После рикошета:

- направление остается на XZ-плоскости;
- скорость уменьшается по конфигу;
- текущий damage cap уменьшается по конфигу;
- количество оставшихся рикошетов уменьшается;
- после исчерпания лимита следующий контакт уничтожает снаряд.

Текущие числа смотри в `docs/TECH_STATUS.md`.

## 12. Примеры расчета

### 12.1. Прямо в лоб

```text
FrontArmor = 50
EffectiveArmor = 50
Penetration = 45
```

Результат:

```text
45 < 50
NoPenetration / Ricochet
Damage = 0
```

### 12.2. Прямо в борт

```text
SideArmor = 40
EffectiveArmor = 40
Penetration = 45
```

Результат:

```text
Penetrated
Damage applies
```

### 12.3. Борт под углом

```text
SideArmor = 40
EffectiveArmor > 45 из-за угла
Penetration = 45
```

Результат:

```text
NoPenetration / Ricochet
Damage = 0
```

### 12.4. Корма

```text
RearArmor = 10
Penetration = 45
```

Результат:

```text
Penetrated
Damage applies
```

## 13. Критическая зона / БК

Будущая идея, не текущая реализованная фича:

```text
AmmoRack / БК
```

Правила для будущего:

- если снаряд не пробил броню, крит не срабатывает;
- если снаряд пробил и попал в БК, танк уничтожается сразу;
- БК должен быть отдельной зоной/компонентом, а не случайным `if` внутри Projectile.

## 14. Форма корпуса и тактика

Пока танки прямоугольные, игрок может танковать углом:

```text
враг стреляет -> танк стоит ромбом -> effectiveArmor выше
```

Если в будущем добавить танк с клиновидным носом, тактика меняется.

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

## 15. События геймплея

Основные события:

- `HealthChanged`;
- `Died`;
- `ProjectileSpawned`;
- `ProjectileHit`;
- `ProjectileBounced`;
- `HitResolved`;
- `CombatFeedbackRequested`;
- `MatchFinished`.

Правила:

- геймплейные системы поднимают события;
- UI слушает через презентеры;
- VFX/SFX должны слушать через отдельные обработчики;
- геймплей не должен напрямую вызывать UI;
- подписки должны отписываться;
- для подписок использовать именованные методы, если нужна отписка.

## 16. UI / HUD

HUD должен показывать:

- HP игрока;
- HP врага;
- последний результат попадания;
- результат раунда;
- подсказки управления;
- кнопку перезапуска.

Combat Feedback должен показывать:

- world-space HP bars над танками;
- floating hit text над точкой попадания;
- damage / `NO PEN` / `RICOCHET`.

Геймдизайн-требование:

- игрок должен сразу видеть текущий HP;
- игрок должен понимать, сколько урона нанес конкретный снаряд;
- если урона нет, feedback должен объяснять почему: `NO PEN` или `RICOCHET`;
- HP bars и floating text остаются визуальным слоем, не gameplay logic.

HUD и Combat Feedback не должны:

- наносить урон;
- создавать снаряды;
- решать исход раунда;
- рассчитывать броню.

## 16.1. VFX feedback direction

Минимальный VFX для прототипа:

- projectile trail - текущий код/ассеты уже поддерживают trail, Needs Manual Unity Check;
- small hit/explosion effect - TODO;
- visible impact feedback при попадании/рикошете - TODO;
- smoke/wreck marker после destroyed tank - TODO;
- recoil/knockback feeling на выстреле - TODO.

Recoil пока не считается реализованной механикой. Нужно отдельно решить, будет ли он:

- только визуальным откатом корпуса/башни/камеры;
- физическим импульсом, влияющим на позицию;
- смешанным вариантом.

Для MVP безопаснее сначала проверить visual-only recoil, чтобы не ломать движение, броню и рикошеты.

## 17. Отладка

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

Примеры ожидаемого смысла логов:

```text
[ARMOR] zone=Side baseArmor=40 angle=12 effectiveArmor=40 penetration=45 result=Penetrated
[DAMAGE] target=Enemy damage=61 hp=300->239
```

```text
[ARMOR] zone=Side baseArmor=40 angle=45 effectiveArmor=60 penetration=45 result=NoPenetration
[BOUNCE] count=1 speed=17.1 damageCap=82.5
```

## 17.1. Открытые вопросы геймдизайна

Короткий список направляющих вопросов ведется отдельно: `docs/GD_QUESTIONS.md`.

Главные темы:

- мобильная схема управления;
- tap или fire button;
- насколько сильной должна быть потеря скорости после рикошета;
- должен ли recoil быть только визуальным;
- сколько должен жить wreck/smoke marker;
- какой объем damage feedback достаточен;
- желаемая длина матча;
- правила draw/self-kill.

## 17.2. Network / Multiplayer

Network и multiplayer - будущая перспектива, но не ближайшая реализация.

Правило для следующих итераций:

- не начинать network implementation до стабилизации PC demo и mobile controls prototype;
- сначала провести отдельное network architecture research;
- multiplayer prototype делать отдельной milestone-задачей.

## 18. Техническое направление

Активный корень прототипа:

```text
Assets/_Project/RicochetTanks/
```

Важные элементы:

- `GameplayEntryPoint` - composition root для `RicochetTanks_Demo`.
- `SandboxBootstrapper` / `SandboxSceneBuilder` - legacy/procedural sandbox flow.
- `SandboxMatchController` - раунд и перезапуск.
- `SandboxGameplayEvents` - события геймплея.
- `SandboxDebugVisualizer` - только debug-визуализация.
- `DesktopInputReader` - ввод клавиатуры/мыши.
- `TankFacade` - фасад танка.
- `TankMovement` - движение корпуса.
- `TurretAiming` - вращение башни.
- `TankShooter` - стрельба.
- `ProjectileFactory` - создание снарядов.
- `Projectile` и projectile systems - движение, рикошеты, lifetime.
- `HitResolver` - расчет результата попадания.
- `TankArmor` - зоны брони и effectiveArmor.
- `TankHealth` - HP и смерть.
- `UI/CombatFeedback` - визуальная обратная связь.

## 19. Правила архитектуры

- Никаких Singleton.
- Никакого огромного `GameManager`.
- UI View только отображает данные и поднимает события.
- Presenter/Controller связывает UI с сервисами и геймплеем.
- Projectile не лезет напрямую во внутренности здоровья танка.
- Health не знает про углы и броню.
- Round logic отделена от здоровья и снарядов.
- EntryPoint/Bootstrapper только собирает зависимости.
- Приватные поля пишутся как `_camelCase`.
- Не использовать LINQ в `Update`, `FixedUpdate`, projectile systems, AI systems и горячих циклах.
- Не использовать `FindObjectOfType` / `GameObject.Find` в геймплейном цикле.
- Не класть UI-логику в gameplay systems.

## 20. Definition of Done

Фича считается готовой, если:

- она вписана в существующий модуль или создает понятный новый модуль;
- нет God Object;
- значения вынесены в конфиги;
- зависимости передаются явно;
- события подписываются и отписываются корректно;
- UI отделен от геймплея;
- горячие циклы без лишних аллокаций;
- есть достаточные debug logs;
- сцена запускается после чистого клона;
- README/GDD/TECH_STATUS обновлены;
- ручной чек-лист проходит или явно помечен как **Needs Manual Unity Check**.
