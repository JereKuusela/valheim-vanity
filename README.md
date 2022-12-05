# Vanity

Allows admins to change visual style of players.

Install on the client to change your style (modding [guide](https://youtu.be/L9ljm2eKLrk)).

Install on the server to enforce styles.

The changes done by the mod are visible to un-modded clients.

# Features

- On single player, change style of any equipment (while keeping original stats).
- On servers, admins can change style of any player who has this mod installed.
- On servers, everyone sees tne changes (even without this mod).
- Hair, beard, hair color and skin color also supported (with automatic color cycling).

Note: Using some items can print warnings to the console but this should be harmless.

# Usage

Styles are saved to the config folder in file `vanity.yaml`:

```
# Id of the character. 0 is used to apply styling for every character.
0:
# Name of the character. This is automatically set to help editing the file.
  name: Everyone
# These replace style of a slot, regardless of which item is equipped there.
# Item ids can be found at https://valheim.fandom.com/wiki/Item_IDs
  helmet: item_id
  chest: item_id
  legs: item_id
# Variant is a number if the item has multiple skins. 0 is the default value if the value is not given.
  shoulder: item_id variant
  utility: item_id variant
  leftHand: item_id variant
  rightHand: item_id variant
  leftBack: item_id variant
  rightBack: item_id variant
  beard: beard_id
  hair: hair_id
# Color has three values red, green and blue. Values over 1.0 cause significant glowing.
  skinColor: r1,g1,b1 r2,g2,b2 r3,g3,b3 ...
# Multiple colors automatically switches the color over time.
  hairColor: r1,g1,b1 r2,g2,b2 r3,g3,b3 ...
# How often the color is updated. Default is 10 times per second.
  updateInterval: 0.1
# How long each color lasts. Default is 1 second. Increase to slow down the change speed.
  colorDuration: 1
# Overrides a specific gear. For example this would replace style of bronze helmets.
  gear:
    HelmetBronze: TrophyAbomination
# Overrides a specific gear crafted by this player. For example this would replace style of bronze helmets crafted by this player.
  crafted:
    HelmetBronze: TrophyAbomination
```

# Commands

The yaml file can also be modified with commands. This is useful in servers because not all admins have access to the yaml file. This can be also be easier because of the autocomplete.

- `playerid`: Returns your player id.
- `playerid name`: Returns player id of a player. This includes all players which have an entry in the yaml file, even if they are not online.
- `wear_gear [item id] [visual id] [variant = 0] player=[id]`: Replaces style of a specific gear.
  - `wear_gear CapeLox CapeLinen 2`: Replaces Lox Cape with a green Linen Cape.
  - `wear_gear MaceIron skeleton_mace player=123123`: Replaces Iron Mace style for a specific player.
- `wear_crafted [item id] [variant = 0] player=[id]`: Replaces style of crafted gear.
  - `wear_crafted MaceIron skeleton_mace player=123123`: Replaces styles of Iron Maces crafted by a specific player.
- `wear_* [id] [variant = 0] player=[id]`: Replaces style of a specific slot. Autocomplete will show available commands.
- `wear_hair_color [r1,g1,b1] [r2,g2,b2] ... player=[id]`: Replaces hair color.
- `wear_skin_color [r1,g1,b1] [r2,g2,b2] ... player=[id]`: Replaces skin color.
  - `wear_skin_color 1,0,0`: Changes skin color to red.
  - `wear_skin_color 10,0,0`: Changes skin color to bright red.
  - `wear_skin_color 10,0,0 0,10,0 0,0,10`: Turns you into a light show.
- `wear_hair [name] player=[id]`: Replaces hair.
- `wear_beard [name] player=[id]`: Replaces beard.
- `wear_undo`: Reverts used wear_* commands.
- `wear_reset`: Resets visual styles (except specific gears).
- `wear_info`: Displays item names of current visual styles.
