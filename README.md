# Brutalsky
A physics-based fighting game with the objective of ramming / baiting / whatever else your opponent until they run out of health and go kaboom!

## How to Play

### - At the moment, this game is meant to be played with two people using the same keyboard, but I plan to add multiplayer in the future.

### Ability Key
This key is used for charging and shielding, depending on your speed and ability cooldown. For charging, this key can be held down to increase the power of the charge (no more power increase after 3 seconds of holding).
The chart below shows the properties of each ability:
| Ability | Required speed | Charge time for full power | Cooldown after usage | Ring color* |
|:-:|:-:|:-:|:-:|:-:|
| Charge | 5-30 meters/second | 3 seconds | 2 seconds | cyan |
| Shield | ? | ? | ? | yellow |

*a translucent ring will appear around the player showing which ability is currently possible to perform, if any

### Damage System
Damage dealt is based on how much impact you can deal to your opponent, so higher speed is always better when colliding with the other player.

For maximum damage, charge at your opponent when they are moving slowly or can't charge against you at the moment (for example, if they are cornered or moving too slowly, they can't counter-charge when you charge them).

When a player's health hits 0, they will explode (I'm so proud of my explosion effect lol) and the other player will win the round if they stay alive for another 3 seconds.

### Obstacles
#### Health change
- Changes health of players on contact (damage or heal)
- Effect types:
	- **Instant:** instantly effects players on contact and then doesn't do anything else until the player leaves and comes back into contact with this obstacle (e.g. -50 health on collision)
	- **Gradual:** changes player health over time as long as players are in contact (e.g. -50 health per second while in contact)

## Controls

### Player 1
- **Move:** `🠙` `🠘` `🠛` `🠚`
- **Ability:** `'`

### Player 2
- **Move:** `W` `A` `S` `D`
- **Ability:** `~`

## Object Color Meaning

- **Orange:** player 1
- **Aqua:** player 2
- **Gray:** wall
- **Red:** damage obstacle
- **Green:** heal obstacle
- **Yellow:** instant death obstacle (only causes instant death if not full health)
