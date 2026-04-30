# Enemy AI FSM Plan

MVP enemy AI is now implemented under `Assets/_Project/RicochetTanks/Scripts/Gameplay/AI`.

## MVP States

- `Idle`
- `AimAndShoot`
- `KeepDistance`
- `Reposition`
- `AvoidObstacle`
- `Dead`

## Current Behavior

- Enemy shoots at the player.
- Enemy aims before shooting and checks line of sight.
- Enemy tries to keep distance.
- Enemy repositions when blocked or periodically.
- Enemy reverses and turns when an obstacle is directly ahead.

## Future Behavior Goals

- Better patrol/reposition choices.
- Chasing line of sight around obstacles.
- Smarter retreating when low HP.
- Avoid getting stuck in corners for long periods.

## Integration Notes

- Keep AI decisions separate from combat formulas.
- AI should send movement/aim/fire commands through the same tank-facing APIs as player input.
- Do not change armor, ricochet, damage, projectile, or movement math while improving AI.
