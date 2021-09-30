# Changelog

## *V 0.0.0 // Initial Commit* &nbsp; `2021-09-10 10:45`
- Created repository

## V 0.1 &nbsp; `2021-09-10 12:55`
### Added
- Basic arena
- Player with health bar and charge ability
- Camera with some basic shake effects

## V 0.2 &nbsp; `2021-09-10 16:04`
### Added
- Second player
### Changed
- Controls are now easier to configure and more flexible

## V 0.3 &nbsp; `2021-09-11 16:54`
### Added
- Camera shakes when both players collide head-on instead of just staying still
- Post processing

## V 0.4 // Damage Update &nbsp; `2021-09-11 21:44`
### Added
- Players take damage on strong impacts and die when health is used up
- Health changing obstacles that damage/heal players either on impact or slowly as long as contact is maintained

## *V 0.4.1* &nbsp; `2021-09-11 21:50`
### Added
- Repository description

## V 0.5 &nbsp; `2021-09-12 11:19`
### Added
- Camera shakes when players die
- Instant death obstacle that kills players on contact who don't have completely full health
### Changed
- Adjusted health change obstacles
### Fixed
- Camera shake and camera jagged shake values now snap to 0 when they get exponentially small

## V 0.6 &nbsp; `2021-09-12 16:08`
### Added
- Indicator ring on players that shows when health changes and when charging is possible
- Game automatically resets when one or both players die
### Fixed
- Players took a small amount of damage when they collided with something while moving slowly

## V 0.7 &nbsp; `2021-09-12 21:54`
### Added
- Particle explosion on player death
- Trail effect to visualize charging
- Fade transition for when the charging indicator turns on or off
### Fixedn't
- Camera gets set to a (NaN, NaN) position and spams a lot of errors in the console (still not fixed apparently, can be caused by players starting the level in contact with the ground)
### Bugs
- Players do not despawn on death due to the fact that despawning the player would stop the explosion particles from appearing with the current setup D:

## V 0.8 &nbsp; `2021-09-13 10:29`
### Added
- Player death particles randomly rotate
### Fixed
- Players not despawning on death

## V 0.9 &nbsp; `2021-09-13 15:32`
### Added
- Shockwave effect on player death
### Changed
- Camera shake caused by player death is now smoother and less jittery

## V 0.10 &nbsp; `2021-09-13 19:56`
### Added
- Platforms sticking out from the left and right walls in the arena
- World border that causes instant death if players get 10 meters outside of the arena walls
### Changed
- Health change obstacles interact with players like a fluid instead of a hard object (players will have a buoyant force applied to them instead of just colliding)
- Instant death obstacle has been moved from the middle of the arena to the ends of the new platforms (and has also been duplicated because there are two platforms)
### Fixed
- Optimized camera shake system

## *V 0.10.1* &nbsp; `2021-09-13 21:07`
### Added
- A proper `README.md`

## *V 0.10.2* &nbsp; `2021-09-13 21:10`
### Fixed
- `README.md` was broken

## V 0.11 &nbsp; `2021-09-14 21:49`
### Added
- `CHANGELOG.md` (this file you are reading right now)
### Changed
- Damage obstacles (red health change obstacles) are now capable of killing you directly and don't stop dealing damage at 10% health
- Damage obstacles deal damage at half the rate they used to (-50% health/sec instead of -100% health/sec)
- If a player gets too far inside one of the damage obstacles, they are instantly killed
- Heal obstalces (green health change obstacles) have twice as much buoyancy force so it's more difficult to remain in the same spot while healing (you may notice I don't like campers)
### Fixed
- It's no longer possible to escape the arena by going into the health change obstacles and through the sides of them (added walls to block the open gaps on the sides)

## V 0.12 &nbsp; `2021-09-28 13:39`
### Added
- The ability key can be held while using the charge ability to add extra power to the charge (you will see some particles as an indicator of charge power, more particles = more power)
- Arena is always in view of the camera, even if the horizontal size of the camera gets smaller than the arena width (before this update, the sides of the arena would just go out of view of the camera if the horizontal camera size was too small)
- Health change obstacles have bubble effects to go with the liquid physics
- Instant death obstacles have a "saw blade" effect
- *[TECHNICAL] Recreated the ability system so it's a lot more configurable*
### Changed
- The space outside of the arena shows as black instead of being able to see stuff out of bounds

## V 0.13 &nbsp; `2021-09-30 15:07`
### Added
- Main menu (open by pressing escape)
- When the game ends, a text appears telling which player won
- Time slows down when the game ends
### Changed
- It is impossible to charge unless you are moving (previously, you could start pressing the ability key while moving fast enough to build up a charge and then use it even when you are sitting still, but now charging can only be done while moving and will be delayed until the player is moving fast enough if they stop pressing the ability key while not moving)
- Charging now leaves behind a "ghost image" of the player for 1 second
### Fixed
- Updated `README.md` to include info about ability charge up
