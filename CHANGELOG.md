# Changelog

## *V 0.0.1 // Initial Commit* &nbsp; `2021-09-10 10:45`
### Added
- Created repository

## V 0.1 // The Beginning &nbsp; `2021-09-10 12:55`
### Added
- Basic arena
- Player with health bar and charge ability
- Camera with some basic shake effects

## V 0.2 // Enemy Update &nbsp; `2021-09-10 16:04`
### Added
- Second player
### Changed
- Controls are now easier to configure and more flexible

## V 0.3 // Lighting Update &nbsp; `2021-09-11 16:54`
### Added
- Players have lights on them that match their color
- Post processing (bright objects have a faint glow)
### Changed
- Camera shakes when both players collide head-on instead of just staying still

## V 0.4 // Damage Update &nbsp; `2021-09-11 21:44`
### Added
- Players take damage on strong impacts and die when health is used up
- Health changing obstacles that damage/heal players either on impact or slowly as long as contact is maintained

## *V 0.4.1 &nbsp; `2021-09-11 21:50`*
### Added
- Repository description

## V 0.5 // Deadly Update &nbsp; `2021-09-12 11:19`
### Added
- Camera shakes when players die
- Instant death obstacle that kills players on contact who don't have completely full health
### Changed
- Adjusted health change obstacles
### Fixed
- Camera shake and camera jagged shake values now snap to 0 when they get exponentially small

## V 0.6 // Reload Update &nbsp; `2021-09-12 16:08`
### Added
- Indicator ring on players that shows when health changes and when charging is possible
- Game automatically resets when one or both players die
### Fixed
- Players took a small amount of damage when they collided with something while moving slowly

## V 0.7 // Explosion Update &nbsp; `2021-09-12 21:54`
### Added
- Particle explosion on player death
- Trail effect to visualize charging
- Fade transition for when the charging indicator turns on or off
### Bugs
- Players do not despawn on death due to the fact that despawning the player would stop the explosion particles from appearing with the current setup (D:)
- Camera gets set to a (NaN, NaN) position and spams a lot of errors in the console

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

## V 0.10 // Arena Update &nbsp; `2021-09-13 19:56`
### Added
- Platforms sticking out from the left and right walls in the arena
- World border that causes instant death if players get 10 meters outside of the arena walls
### Changed
- Health change obstacles interact with players like a fluid instead of a hard object (players will have a buoyant force applied to them instead of just colliding)
- Instant death obstacle has been moved from the middle of the arena to the ends of the new platforms (and has also been duplicated because there are two platforms)
### Fixed
- Optimized camera shake system

## *V 0.10.1 &nbsp; `2021-09-13 21:07`*
### Added
- A proper `README.md`

## *V 0.10.2 &nbsp; `2021-09-13 21:10`*
### Fixed
- `README.md` was broken

## V 0.11 // Changelog Update &nbsp; `2021-09-14 21:49`
### Added
- `CHANGELOG.md` (this file you are reading right now)
### Changed
- Damage obstacles (red health change obstacles) are now capable of killing you directly and don't stop dealing damage at 10% health
- Damage obstacles deal damage at half the rate they used to (-50% health/sec instead of -100% health/sec)
- If a player gets too far inside one of the damage obstacles, they are instantly killed
- Heal obstalces (green health change obstacles) have twice as much buoyancy force so it's more difficult to remain in the same spot while healing (you may notice I don't like campers)
### Fixed
- It's no longer possible to escape the arena by going into the health change obstacles and through the sides of them (added walls to block the open gaps on the sides)

## V 0.12 // Charge Up(date) &nbsp; `2021-09-28 13:39`
### Added
- The ability key can be held while using the charge ability to add extra power to the charge (you will see some particles as an indicator of charge power, more particles = more power)
- Arena is always in view of the camera, even if the horizontal size of the camera gets smaller than the arena width (before this update, the sides of the arena would just go out of view of the camera if the horizontal camera size was too small)
- Health change obstacles have bubble effects to go with the liquid physics
- Instant death obstacles have a "saw blade" effect
- *[TECHNICAL] Created an official ability system*
### Changed
- The space outside of the arena shows as black instead of being able to see stuff out of bounds

## V 0.13 // UI Update &nbsp; `2021-09-30 15:07`
### Added
- Main menu (open by pressing escape)
- When the game ends, a text appears telling which player won
- Time slows down when the game ends
### Changed
- It is impossible to charge unless you are moving (previously, you could start pressing the ability key while moving fast enough to build up a charge and then use it even when you are sitting still, but now charging can only be done while moving and will be delayed until the player is moving fast enough if they stop pressing the ability key while moving slowly)
- Charging now leaves behind a "ghost image" of the player for 1 second
### Fixed
- Updated `README.md` to include info about ability charge up

## V 0.14 // Cheats Update &nbsp; `2021-10-01 15:24`
### Added
- A cheat menu accessible by pressing `/` that can heal or kill players (more options planned to be added)
- *[TECHNICAL] Created an official UI system*
### Changed
- Heal obstacles now have the same buoyancy force as the damage obstacles
- Removed saw blade effect from instant death obstacles
- The outside of the arena appears gray like the walls instead of black
### Fixed
- Fixed the camera breaking with (NaN, NaN) position if the level starts with the players touching the arena (finally) (this is the same bug mentioned in v0.7)
- Fixed ability cooldown not working
- Added title to v0.13 update entry

## V 0.15 // UI v2 &nbsp; `2021-10-03 10:52`
### Added
- Help menu
- Completely recreated the UI because it didn't match the game very well and the code behind it was messy (except for the brand new UI manager system :D)
### Removed
- Old UI
  - Cheat menu (Will be added back later but it's been deleted with the old UI)
### Fixed
- Edited controls shown in `README.md` to reflect default keybinds
- Edited damage system info in `README.md`
- Edited v0.12 and v0.13 changelog entries

## V 0.16 // Motion Update &nbsp; `2021-10-03 21:34`
### Added
- Crusher obstacle (I finally fulfilled my desire to add a dynamic obstacle :D)
  - New crushing physics which causes players to continually take damage while squeezed between objects (not really noticeable most of the time because it only affects the player with extreme crushing force)
- [[^]](https://github.com/XVNexus/brutalsky/issues/5) Settings are saved in system storage so that they stay the same even after restarting the game
### Changed
- Widened lava pools from 9 meters to 10 meters
### Fixed
- [[^]](https://github.com/XVNexus/brutalsky/issues/3) The outer arena walls weren't getting illuminated by player lights due to the wrong material being assigned to them
- Updated `README.md` and game help menu with new color meaning entry for dynamic (moving) walls
- Added titles to v0.11 and v0.12 update entries

## V 0.17 // Changelog Update &nbsp; `2021-10-05 13:58`
### Added
- Ingame changelog (UI elements are generated by scanning a markdown file from a github url)
### Changed
- Crusher obstacle has a slightly lighter background to show the danger area
- Slightly modified menu button text colors (made them more consistent in button groups)
- Added highlight effects to dropdown menus to make the UI clearer
### Fixed
- Added titles to v0.1 - 0.3, v0.5 - 0.7, and v0.10 update entries
- Edited v0.0, v0.3, v0.7, v0.14, and v0.15 changelog entries
- Removed notes showing which text was changed in file edits because I don't think anybody cares that much and its cluttering the changelog
