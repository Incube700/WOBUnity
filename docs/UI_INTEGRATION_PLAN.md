# UI Integration Plan

- Current production flow is `MainMenu -> RicochetTanks_Demo`; integrate and verify MainMenu UI before any PvP lobby UI.
- MainMenu statistics is the only active UI integration added in this pass.
- Test new UI assets first in `Assets/_Project/Features/UI/Scenes/UISandbox.unity`.
- Convert UI pieces into reusable prefabs only after they work in the sandbox.
- Connect screens through View/Presenter pairs; Views display and raise UI events, Presenters call interfaces.
- Gameplay must not know concrete UI prefabs, lobby implementations, network classes, or server classes.
- PvP lobby UI stays mock-only until the local gameplay/session prototype is stronger.
- Keep `UISandbox` separate from `MainMenu` and `RicochetTanks_Demo`.
- Do not connect UI buttons directly to combat, networking, server, or scene-specific prefab classes.
