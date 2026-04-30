# Enemy AI FSM Plan

No enemy AI is implemented yet. The current enemy remains a dummy tank for the local prototype.

## Planned States

- `Idle`
- `Patrol / Reposition`
- `AimAtPlayer`
- `Shoot`
- `AvoidObstacle`
- `KeepDistance`
- `ChaseLineOfSight`
- `Retreat`
- `Dead`

## Future Behavior Goals

- Enemy shoots at the player.
- Enemy aims before shooting.
- Enemy tries to keep distance.
- Enemy avoids obstacles.
- Enemy tries to get line of sight.
- Enemy does not drive into walls forever.

## Integration Notes

- Keep AI decisions separate from combat formulas.
- AI should send movement/aim/fire commands through the same tank-facing APIs as player input.
- Do not change armor, ricochet, damage, projectile, or movement math while adding AI.
