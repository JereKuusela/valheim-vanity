# Vanity

Allows admins to change visual style of any player.

Install on the client to change your style (modding [guide](https://youtu.be/L9ljm2eKLrk)).

Install on the server to enforce styles.

The changes done by the mod are visible to un-modded clients.

# Features

- Replace visual style of any equipment with commands (while still keeping the stats).
- Change hair or skin color, with automatic color cycling.
- Bind visual style to specific equipment to automatically apply it.

Visual style is applied per character. Commands also work in the character selection screen but without autocomplete ([wiki](https://valheim.fandom.com/wiki/Item_IDs) has a list of ids if needed).

Visual style is set for all characters when using commands on the new character screen but this is not recommended.

Note: Using some items can print warnings to the console but this should be harmless.

# Commands

## wear_bind [item name] [visual name] [variant = 0]

Binds visual style to an item. Equipping that item automatically applies the visual style.

Use the command without visual name to remove the binding.

Examples:

- wear_bind MaceIron skeleton_mace: Replaces iron mace with the mace of Rancid remains.
- wear_bind ShieldBlackmetal ShieldSerpentscale: Replaces Black metal shield with a Serpent scale shield.
- wear_bind CapeLox CapeLinen 2: Replaces Lox Cape with a green Linen Cape.

## wear_undo

Reverts used wear_* commands.

## wear_reset

Resets visual styles (except bindings and trophies).

# wear_info

Displays item names of current visual styles.

# wear_hair_color / wear_skin_color [r1,g1,b1] [r2,g2,b2] ...

Sets color of hair or skin. If multiple colors are given then the color is automatically cycled with a smooth transition.

Configuration can be used to change how long the colors last and how often they are updated.

Examples:

- wear_skin_color 1,0,0: Changes skin color to red.
- wear_skin_color 10,0,0: Changes skin color to bright red.
- wear_skin_color 10,0,0 0,10,0 0,0,10: Turns you into a light show.

# wear_hair / wear_beard [name]

Sets hair or beard style. Autocomplete provides list of available names.

# wear_* [name] [variant]

Remaining commands change visual style of a gear slot.

# Credits

Sources: [GitHub](https://github.com/JereKuusela/valheim-vanity)

Donations: [Buy me a computer](https://www.buymeacoffee.com/jerekuusela)
