# Ricochet Tanks - Roadmap

**Synced:** 2026-04-28  
**Design source:** `docs/GDD_RU.md`  
**Status source:** `docs/TECH_STATUS.md`

This roadmap restores the useful milestone structure from the old root GDD and updates it for the current playable prototype state. Items that were not manually verified in Unity remain unchecked.

## Immediate Next Tasks

1. Verify HP bars, floating hit text, and restart behavior in Unity.
2. Verify wall/obstacle ricochet and tank ricochet in `RicochetTanks_Demo`.
3. Add `DebugLogConfig` or a simple logging toggle.
4. Clean materials and visual readability if Unity shows broken/magenta materials.
5. Stabilize arena size/layout if needed.
6. Improve simple 3D tank visual prefab.
7. Add simple enemy behavior.
8. Polish main menu and restart flow.
9. Add mobile controls later.

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
- [ ] Add critical ammo rack zone.
- [ ] Add projectile prediction/debug readout if useful.
- [ ] Add VFX/SFX for shots, ricochets, impacts, and damage.
- [ ] Prepare pooling for projectiles/VFX if performance or allocations become a problem.

## Milestone 3 - Enemy AI

- [ ] Simple enemy movement.
- [ ] Enemy aiming and shooting.
- [ ] Reposition / Evade states.
- [ ] Debug AI state display.

## Milestone 4 - Mobile / Android

- [ ] Mobile input layer.
- [ ] Fire button.
- [ ] Restart button.
- [ ] Android-safe UI.
- [ ] Android build.

## Milestone 5 - Portfolio Polish

- [ ] Improve arena visuals.
- [ ] SFX/VFX pass.
- [ ] Screenshots/GIF for devlog.
- [ ] Public README polish.

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
