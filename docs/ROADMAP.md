# Ricochet Tanks - Roadmap

**Synced:** 2026-04-28  
**Design source:** `docs/GDD_RU.md`  
**Status source:** `docs/TECH_STATUS.md`

This roadmap restores the useful milestone structure from the old root GDD and updates it for the current playable prototype state. Items that were not manually verified in Unity remain unchecked.

## Prioritized Roadmap

### MVP Current

1. Stabilize the PC demo in `RicochetTanks_Demo` - Needs Manual Unity Check.
2. Verify HP bars and floating damage feedback - Needs Manual Unity Check.
3. Validate armor, penetration, damage, ricochet, and speed-loss formulas against actual gameplay.
4. Check whether projectile speed loss after ricochet is visually strong enough.
5. Confirm restart does not duplicate HP bars, floating text presenters, or event subscriptions.

### Next

1. Finalize mobile controls design in `docs/MOBILE_CONTROLS.md`.
2. Answer guiding questions in `docs/GD_QUESTIONS.md`.
3. Prototype mobile controls: left joystick for hull/movement, right joystick for turret aim, tap or fire button for shot.
4. Test Android landscape build.
5. Add minimal VFX feedback: small hit/explosion effect, visible impact, smoke/wreck marker.
6. Add shot recoil/knockback feeling after deciding whether it is visual-only or physics-affecting.

### Later

1. Improve tank/arena visuals.
2. Test iOS build.
3. Research network architecture.
4. Build multiplayer prototype only after PC demo and mobile controls are stable.

## Immediate Next Tasks

1. Manually verify PC demo stability in Unity.
2. Manually verify HP bars, floating hit text, and restart behavior.
3. Manually verify wall/obstacle ricochet and tank ricochet in `RicochetTanks_Demo`.
4. Review/tune projectile speed-loss readability after ricochet.
5. Design mobile landscape control layout before changing controls code.
6. Keep network/multiplayer as research only.

## Milestone 0 - Documentation And Repository

- [x] Update README.
- [x] Restore structured GDD docs.
- [x] Preserve Russian design source.
- [x] Record current prototype status.
- [x] Record current manual-check risks.

## Milestone 1 - First Playable Sandbox

- [ ] Bootstrap -> MainMenu -> Sandbox flow - Partial / Needs Manual Unity Check.
- [ ] `RicochetTanks_Demo` launches cleanly - Needs Manual Unity Check.
- [ ] Arena and central obstacle are correct - Needs Manual Unity Check.
- [ ] Player movement, turret aim, and shooting work - Needs Manual Unity Check.
- [ ] Enemy dummy has HP and can die - Needs Manual Unity Check.
- [ ] Fast visible projectile works - Needs Manual Unity Check.
- [ ] Projectile ricochets from walls/obstacles up to 3 times - Needs Manual Unity Check.
- [ ] Damage reduction after ricochet works - Needs Manual Unity Check.
- [ ] Armor penetration/no-penetration/ricochet works - Needs Manual Unity Check.
- [ ] HUD updates HP and round result - Needs Manual Unity Check.
- [ ] World HP bars and floating hit text work - Needs Manual Unity Check.
- [ ] Restart flow resets without duplicates - Needs Manual Unity Check.

## Milestone 2 - Combat Feel

- [ ] Add or tune hit feedback only after current combat feedback is manually verified.
- [ ] Verify how much damage feedback is enough for the player.
- [ ] Review projectile speed loss after ricochet and tune only after manual check.
- [ ] Add shot recoil/knockback feeling after design decision.
- [ ] Add critical ammo rack zone.
- [ ] Add projectile prediction/debug readout if useful.
- [ ] Add VFX/SFX for shots, ricochets, impacts, and damage.
- [ ] Add smoke/wreck marker after destroyed tank.
- [ ] Prepare pooling for projectiles/VFX if performance or allocations become a problem.

## Milestone 3 - Enemy AI

- [ ] Simple enemy movement.
- [ ] Enemy aiming and shooting.
- [ ] Reposition / Evade states.
- [ ] Debug AI state display.

## Milestone 4 - Mobile / Android

- [ ] Approve mobile landscape layout.
- [ ] Left virtual joystick for movement/hull control.
- [ ] Right virtual joystick for turret/cannon aiming.
- [ ] Decide tap-to-fire or fire button.
- [ ] Restart button.
- [ ] Android-safe UI.
- [ ] Android build.

## Milestone 5 - Portfolio Polish

- [ ] Improve arena visuals.
- [ ] SFX/VFX pass.
- [ ] Screenshots/GIF for devlog.
- [ ] Public README polish.

## Future - Network / Multiplayer

- [ ] Network architecture research.
- [ ] Decide authoritative simulation model.
- [ ] Decide rollback/prediction needs for projectiles and ricochets.
- [ ] Multiplayer prototype after PC and mobile prototypes are stable.

## Manual Test Checklist

### Scenes

- [ ] Project opens without compile errors.
- [ ] `RicochetTanks_Demo` opens.
- [ ] Press Play starts the match.
- [ ] Restart resets the match.
- [ ] Console has no blocking errors.

### Player

- [ ] Player spawns in the expected position.
- [ ] Player accelerates forward.
- [ ] Player brakes/coasts.
- [ ] Player reverses.
- [ ] Player rotates hull predictably.
- [ ] Hull does not rotate without steering input.
- [ ] Turret follows mouse ground point.
- [ ] Turret does not spin randomly.
- [ ] Player fires.

### Projectile

- [ ] Projectile appears from the muzzle.
- [ ] Projectile does not hit owner immediately.
- [ ] Projectile is fast but visible.
- [ ] Projectile ricochets from walls.
- [ ] Projectile ricochets from center/arena blocks.
- [ ] Projectile disappears after the ricochet limit/contact rules.
- [ ] Projectile loses speed after ricochet.
- [ ] Projectile loses damage cap after ricochet.

### Combat

- [ ] Projectile can hit enemy.
- [ ] Front armor resists default projectile penetration.
- [ ] Direct side hit can penetrate.
- [ ] Glancing side hit can no-pen/ricochet.
- [ ] Rear hit penetrates.
- [ ] Damage after ricochet is lower.
- [ ] Enemy dies at 0 HP.
- [ ] Player can be damaged by returning projectile after safe owner time.
- [ ] Last hit/HUD/floating text communicate the result.

### Architecture

- [ ] No new God Object.
- [ ] No gameplay `FindObjectOfType` in hot paths.
- [ ] Event subscriptions unsubscribe.
- [ ] Balance remains in configs.
- [ ] UI does not contain combat rules.
- [ ] Gameplay systems do not instantiate UI.
