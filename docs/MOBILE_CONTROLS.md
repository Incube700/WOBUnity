# Ricochet Tanks - Mobile Controls Design

**Status:** design only, no code implemented
**Orientation:** landscape
**Related docs:** `docs/GDD_RU.md`, `docs/GD_QUESTIONS.md`, `docs/ROADMAP.md`

This document describes the intended mobile control direction. It must be reviewed before changing `PlayerTankController`, `TankMovement`, `TurretAiming`, or input code.

## Goals

- Make the tank playable on a phone in landscape orientation.
- Keep hull movement and turret aiming independent.
- Preserve the current top-down 3D/XZ gameplay feel.
- Avoid rewriting PC controls until the mobile layout is approved.

## Baseline Layout

```text
Left side of screen                         Right side of screen
┌─────────────────────┐                    ┌─────────────────────┐
│ Left joystick        │                    │ Right joystick       │
│ movement / hull      │                    │ turret / cannon aim  │
└─────────────────────┘                    └─────────────────────┘

Optional: fire button near right thumb, or tap-to-fire.
Optional: restart/pause button in a top corner.
```

## Left Joystick - Movement / Hull

Purpose:

- forward/back throttle;
- braking/reverse through downward input;
- hull steering left/right;
- preserve inertia, acceleration, braking/coasting from the PC demo.

Possible mapping:

- vertical axis controls throttle/brake/reverse;
- horizontal axis controls hull turn;
- dead zone prevents drift;
- releasing joystick should coast/brake according to `TankMovement`.

Pros:

- familiar mobile tank control pattern;
- maps well to current W/S/A/D responsibilities;
- keeps body control under the left thumb.

Cons / risks:

- diagonal input may feel muddy if acceleration and steering fight each other;
- thumb drift can cause unwanted hull rotation;
- small screens need generous dead zone and clear joystick radius.

## Right Joystick - Turret / Cannon Aim

Purpose:

- aim turret independently from hull;
- keep aim stable when the hull turns;
- give the player fine control for ricochet shots.

Possible modes:

- hold-drag joystick direction controls turret aim direction;
- floating joystick appears where the right thumb touches;
- fixed joystick stays in a known screen area.

Pros:

- direct control over cannon direction;
- works without relying on a mouse ground-plane ray;
- makes mobile ricochet aiming possible.

Cons / risks:

- fixed joystick can be uncomfortable on different phone sizes;
- floating joystick can confuse players if it appears over action;
- hold-drag needs clear visual feedback for current aim direction.

## Fire Input

Open decision:

- tap-to-fire anywhere on the right side;
- separate fire button;
- fire on right joystick release;
- hybrid: button for precision, tap for quick shots.

Recommended first prototype:

- separate fire button near the right thumb;
- keep tap-to-fire as an optional experiment.

Reason:

- accidental shots are expensive in a ricochet game;
- a button is easier to explain and test;
- it separates aim adjustment from shooting.

## Landscape UI Notes

- Keep HP bars and key combat feedback away from thumb zones.
- Left/right joysticks should not cover the tank or hit text.
- Fire button should be reachable without blocking the turret aim view.
- Restart/pause should be away from active thumbs, likely top corner.
- Test on small and large phone aspect ratios.

## UX Risks

- Player accidentally rotates hull while trying to brake.
- Turret aim drifts when right thumb is idle.
- Fire button overlaps right joystick or hides important combat feedback.
- HP bars/floating damage become too small on mobile.
- Ricochet precision suffers if the right joystick is too coarse.
- Screen edges make diagonal aiming uncomfortable.

## Test Checklist

- [ ] Landscape orientation is forced or clearly expected.
- [ ] Left joystick moves forward/reverse and turns hull predictably.
- [ ] Releasing left joystick causes coasting/stop behavior, not drift.
- [ ] Right joystick aims turret independently.
- [ ] Turret does not spin when right joystick is idle.
- [ ] Fire input does not interfere with aiming.
- [ ] HP bars remain readable.
- [ ] Floating hit text remains readable.
- [ ] Joysticks do not cover important combat space.
- [ ] Restart/pause is reachable but not easy to hit accidentally.
- [ ] Android build runs and accepts touch input.

## Decisions Needed

See `docs/GD_QUESTIONS.md` for the active game designer questions before implementation.
