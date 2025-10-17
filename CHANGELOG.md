# Changelog

## 1.0.1 (2025-10-07)
* Enabled intro skipping functionality
* Skips boot screen, splash screen, and loading bars by patching StartMenu.Awake
* Directly initializes to start screen bypassing intro sequence
* Proper state management for first-time initialization flag
* Maintains music playback and cursor unlocking during skip
* Auto-hosts for first-time setup when profile is invalid

## 1.0.0 (2025-10-07)

### Tech
* Add MinVer
* Add thunderstore.toml for [tcli](https://github.com/thunderstore-io/thunderstore-cli)
* Add LICENSE and CHANGELOG.md
