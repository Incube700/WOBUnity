# Ricochet Tanks - Roadmap

**Synced:** 2026-04-29
**Design source:** `docs/GDD_RU.md`  
**Status source:** `docs/TECH_STATUS.md`

This roadmap restores the useful milestone structure from the old root GDD and updates it for the current playable prototype state. Items verified by the owner in Unity are checked; APK/device checks remain separate.

## Prioritized Roadmap

### MVP Current

1. Stabilize the PC demo in `RicochetTanks_Demo` - owner verified in Unity.
2. Verify HP bars and floating damage feedback - owner verified in Unity; prefab polish can be tuned later.
3. Validate armor, penetration, damage, ricochet, and speed-loss formulas against actual gameplay.
4. Check whether projectile speed loss after ricochet is visually strong enough.
5. Confirm restart does not duplicate HP bars, floating text presenters, or event subscriptions - owner verified in Unity.

### Next

1. Tune HP bar prefab readability if needed.
2. Test Android landscape build.
3. Review/tune projectile speed-loss readability after ricochet.
4. Improve mobile layout on a real device if Editor layout differs.
5. Add simple enemy behavior after the prototype flow is stable.

### Later

1. Improve tank/arena visuals.
2. Test iOS build.
3. Research network architecture.
4. Build multiplayer prototype only after PC demo and mobile controls are stable.

## Immediate Next Tasks

1. Build/test local Android APK with MainMenu -> RicochetTanks_Demo.
2. Tune HP bar prefab readability if the current bar is too flat or hard to read.
3. Review/tune projectile speed-loss readability after ricochet.
4. Keep network/multiplayer as research only.

## Milestone 0 - Documentation And Repository

- [x] Update README.
- [x] Restore structured GDD docs.
- [x] Preserve Russian design source.
- [x] Record current prototype status.
- [x] Record current manual-check risks.

## Milestone 1 - First Playable Sandbox

- [x] Bootstrap -> MainMenu -> RicochetTanks_Demo flow - owner verified in Unity.
- [x] `RicochetTanks_Demo` launches cleanly - owner verified in Unity.
- [x] Arena and central obstacle are correct - owner verified in Unity.
- [x] Player movement, turret aim, and shooting work - owner verified in Unity.
- [x] Enemy dummy has HP and can die - owner verified in Unity.
- [x] Fast visible projectile works - owner verified in Unity.
- [x] Projectile ricochets from walls/obstacles up to 3 times - owner verified in Unity.
- [x] Damage reduction after ricochet works - owner verified in Unity.
- [x] Armor penetration/no-penetration/ricochet works - owner verified in Unity.
- [x] HUD updates HP and round result - owner verified in Unity.
- [x] World HP bars and floating hit text work - owner verified in Unity; HP bar prefab may need later visual tuning.
- [x] Restart flow resets without duplicates - owner verified in Unity.

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
