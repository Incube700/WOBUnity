# Ricochet Tanks - Game Designer Questions

**Status:** active guiding questions
**Purpose:** help the game designer answer concrete product/gameplay decisions before implementation.

## Mobile Controls

1. How many on-screen buttons are acceptable on mobile?
2. Should shooting use tap-to-fire, a dedicated fire button, or both?
3. Should turret aim be hold-drag, fixed right joystick, or floating right joystick?
4. Should the left joystick allow full analog steering, or should it feel closer to W/S/A/D?
5. Should restart/pause be visible during combat or hidden behind a small menu button?
6. Should mobile controls show aim direction/trajectory assistance?

## Combat Readability

1. How much damage feedback is enough: HP bar only, floating number only, or both?
2. Should `NO PEN` and `RICOCHET` use different colors/sounds/effects?
3. Should every ricochet show feedback, or only tank hits?
4. Should the player see enemy HP as exact number, bar only, or both?
5. Should player HP be larger/more prominent than enemy HP?

## Damage / Armor / Ricochet

1. Should projectile speed loss after ricochet be stronger visually?
2. Should damage loss after ricochet stay at the current multiplier or become more aggressive?
3. Should front armor always resist the default projectile, or only at direct/low-angle hits?
4. Should glancing side hits ricochet often, or mostly no-penetrate and disappear?
5. Should self-hit after ricochet be common, rare, or mostly a funny edge case?

## Recoil / Knockback

1. Should recoil affect only visuals, or should it apply physical movement/knockback?
2. Should recoil move the hull, turret, camera, or only play a short animation?
3. Should recoil make aiming harder, or only make shooting feel stronger?
4. Should recoil depend on projectile damage/speed?

## VFX / Destroyed Tanks

1. How big should hit/explosion effects be for the prototype?
2. How long should smoke/wreck markers remain after a tank is destroyed?
3. Should a destroyed tank stay as a wreck, disappear, or become smoke only?
4. Should ricochet impacts create sparks/trails/marks?
5. What is the minimum acceptable visual feedback for a public demo GIF?

## Match Flow

1. What is an acceptable match length for the prototype?
2. Should player/enemy HP be tuned for short rounds or longer tactical duels?
3. What should happen on draw/self-kill?
4. Should restart be instant or delayed after win/lose?
5. Should the demo have a simple menu before the arena, or open directly into the scene?

## Future Network / Multiplayer

1. Is multiplayer a portfolio direction, product direction, or only a future experiment?
2. Should network research wait until mobile controls are stable?
3. Is local PvP more valuable before online multiplayer?
4. What is the minimum multiplayer prototype: two tanks shooting, full armor rules, or simplified combat?
