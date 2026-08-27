# ClientCreator

A NosTale multiclient creator supporting **IP + port** patching and **client language selection**.

It patches an original `NostaleClientX.exe` to connect to a custom server endpoint
(IP/port) and generates a ready-to-use shortcut that launches the client with the old
ID/password login interface, in the language of your choice.

## Features

- Patch the client's server **IP address** and **port**.
- **Language dropdown** — pick the client display language (UK, DE, FR, IT, PL, ES, RU, CZ, TR).
- **One port per language** — selecting a language auto-fills the matching login port
  (base `4000` + language index, e.g. FR → `4002`); the port stays editable.
- Generates a launch shortcut with the correct arguments
  (`"EntwellNostaleClient" <languageIndex>`).

## Usage

1. Click **Browse...** and select an **original** (unmodified) `NostaleClientX.exe`.
2. Enter the server **IP**.
3. Pick a **Language** — the **Port** field fills in automatically (editable).
4. Enter a **File Name** for the generated shortcut.
5. Click **Generate Multiclient**.

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
  (UK, DE, FR, IT, PL, ES, RU, CZ, TR), passed to the client as the launch argument.
- **One login port per language** — selecting a language auto-fills the matching port
  (base `4000` + language index, e.g. FR → `4002`); still editable by hand.
- **Fixed the launch argument** — the shortcut now uses the correct Entwell format
  (`"EntwellNostaleClient" <index>`) instead of an invalid one.
- **Full SEOVA visual redesign** — dark theme, branded header with logo and teal→magenta
  gradient, rounded gradient action button, dark title bar. The previous background image
  and styling were removed in favor of the SEOVA design identity.

## License

See [`LICENSE.md`](LICENSE.md). Original © 2021 Fizo55.
