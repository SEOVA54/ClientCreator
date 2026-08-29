# ClientCreator

A NosTale multiclient creator supporting **IP + port** patching and **client language selection**.

It patches an original `NostaleClientX.exe` to connect to a custom server endpoint
(IP/port) and generates a ready-to-use shortcut that launches the client with the old
ID/password login interface, in the language of your choice.

## Features

- Patch the client's server **IP address** and **login port**.
- **Language dropdown** — pick the client display language (UK, DE, FR, IT, PL, ES, CZ, RU, TR).
- **One port per language** — the login port is derived from the selected language
  (base `4000` + language index, e.g. FR → `4002`).
- Generates a launch shortcut with the correct arguments
  (`"EntwellNostaleClient" <languageIndex>`).
- **Fails loudly on a partial patch** — if either the IP or the port table cannot be
  located, the client is left untouched instead of being reported as generated.

## Usage

1. Click **Browse...** and select an **original** (unmodified) `NostaleClientX.exe`.
2. Enter the server **IP**.
3. Pick a **Language** — this sets both the login port and the client's region.
4. Enter a **File Name** for the generated shortcut.
5. Click **Generate Multiclient**. The confirmation shows the resulting `IP:port`.

The client must contain the language data files for the chosen language
(e.g. `NostaleData/NScliData_FR.NOS`, `NSlangData_FR.NOS`) for the text to display in it.

## Credits

This project stands on the work of the original authors — full credit to them:

- **Fizo55** — original author and creator of the MulticlientCreator (see `LICENSE.md`, © 2021 Fizo55).
- **lpplayzo** — preserved and forked the project after Fizo55's original repository became unavailable; the fork this repository descends from.

### This fork (SEOVA54)

Huge thanks to **Fizo55** for creating the original tool and to **lpplayzo** for keeping it
alive — this fork wouldn't exist without their work. 🙏

Changes made in this version:

- **Client language selection** — a dropdown to choose the client display language
  (UK, DE, FR, IT, PL, ES, CZ, RU, TR), passed to the client as the launch argument.
- **One login port per language** — the port is derived from the selected language
  (base `4000` + language index, e.g. FR → `4002`).
- **Port table matched by structure, not by a fixed value** — retail builds ship
  different stock tables (`4000,4001,4002,4000,4000,4000,4003` on some, `4000` x7 on
  others). The old fixed pattern silently matched nothing on an unknown build, so the
  client kept dialing its stock port while the tool reported success.
- **Honest success reporting** — the IP and the port are patched in two unrelated places;
  patching only one is now an error and the file is left untouched.
- **Language order matches the client region index** — `CZ` is 6 and `RU` is 7, not the
  reverse. The index is both the login port offset and the region byte the client sends
  in its login packet, so it has to line up with the server's region table.
- **Fixed the launch argument** — the shortcut now uses the correct Entwell format
  (`"EntwellNostaleClient" <index>`) instead of an invalid one.
- **Full SEOVA visual redesign** — dark theme, branded header with logo and teal→magenta
  gradient, rounded gradient action button, dark title bar. The previous background image
  and styling were removed in favor of the SEOVA design identity.

## License

See [`LICENSE.md`](LICENSE.md). Original © 2021 Fizo55.
