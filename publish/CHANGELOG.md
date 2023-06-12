- v1.4
  - Updated for the new game version.

- v1.3
  - Adds support for changing icons in the inventory to match the style.
  - Adds support for Steam and Playfab ids.
  - Adds support for groups.
  - Fixes `wear_gear` not resetting the style when used without the second parameter.
  - Fixes config reload triggering on file metadata change.
  - Fixes color parsing not working with extra space bars.
  - Fixes item style not working (format was `id,variant` instead of `id variant`, now both work).
  - Fixes self-hosting acting as a server (locks other clients configuration). Maybe works, maybe doesn't.

- v1.2
  - Adds new commands `wear_color_duration` and `wear_color_interval`.
  - Fixes not working on character selection screen.
  - Fixes not working on servers when only installed on the client.

- v1.1
  - Fixes a possible conflict / issue with the trader.

- v1.0
  - Initial release.
