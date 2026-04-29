# Android Build Checklist

Local prototype APK only. Do not use this as a Google Play release checklist.

## Scenes

Build Settings should include:

1. `Assets/_Project/RicochetTanks/Scenes/MainMenu.unity`
2. `Assets/_Project/RicochetTanks/Scenes/RicochetTanks_Demo.unity`

`Sandbox.unity` is a legacy/procedural fallback and should stay disabled unless a specific test needs it.

## Unity Setup

1. Open Unity.
2. Open `File > Build Settings`.
3. Switch platform to `Android`.
4. Confirm the scenes above are present and enabled in order.
5. Open `Player Settings`.
6. Set orientation to landscape for the test build.
7. Open `RicochetTanks_Demo` and confirm `GameplayEntryPoint > Input Mode` is `Auto` or `Mobile`.

## Build APK

1. In `Build Settings`, choose `Build` or `Build And Run`.
2. Save the APK outside `Assets/`.
3. Install on an Android device.
4. Launch the app.

## Device Test

- Main menu opens.
- Play loads `RicochetTanks_Demo`.
- Mobile joysticks and `FIRE` button are visible.
- Left joystick moves/turns the tank.
- Right joystick aims the turret.
- Fire button shoots.
- Projectile direction follows the barrel.
- HP bars and floating hit text appear.
- Restart reloads cleanly without duplicate UI.

## Known Prototype Notes

- Mobile controls are a first prototype, not final UX.
- VFX are placeholder/fallback unless custom prefabs are assigned in `CombatVfxConfig`.
- Enemy is currently a dummy target, not full AI.
