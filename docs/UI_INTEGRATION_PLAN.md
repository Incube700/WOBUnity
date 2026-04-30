# UI Integration Plan

- Test new UI assets first in `Assets/_Project/Features/UI/Scenes/UISandbox.unity`.
- Convert UI pieces into reusable prefabs only after they work in the sandbox.
- Connect screens through View/Presenter pairs; Views display and raise UI events, Presenters call interfaces.
- Gameplay must not know concrete UI prefabs, lobby implementations, network classes, or server classes.
- MainMenu UI integration comes before PvP UI.
- PvP lobby UI stays mock-only until the gameplay prototype is stronger.
- Keep `UISandbox` separate from `MainMenu` and `RicochetTanks_Demo`.
