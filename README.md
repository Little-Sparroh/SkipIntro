# SkipIntro

A BepInEx mod for MycoPunk that skips the intro sequence and jumps directly to the main menu.

## Description

This client-side mod eliminates the lengthy boot screens, splash screens, loading bars, and initialization sequences that traditionally play during game startup. By patching the StartMenu.Awake method, the mod skips directly to the main menu interface, saving valuable time for players who have already seen the intro content.

The mod properly handles first-time initialization by maintaining state flags and preserving essential setup processes like profile validation, settings initialization, and auto-hosting logic. Music playback and cursor unlocking are also preserved during the skip process.

## Getting Started

### Dependencies

* MycoPunk (base game)
* [BepInEx](https://github.com/BepInEx/BepInEx) - Version 5.4.2403 or compatible
* .NET Framework 4.8

### Building/Compiling

1. Clone this repository
2. Open the solution file in Visual Studio, Rider, or your preferred C# IDE
3. Build the project in Release mode

Alternatively, use dotnet CLI:
```bash
dotnet build --configuration Release
```

### Installing

**Option 1: Via Thunderstore (Recommended)**
1. Download and install using the Thunderstore Mod Manager
2. Search for "SkipIntro" under MycoPunk community
3. Install and enable the mod

**Option 2: Manual Installation**
1. Ensure BepInEx is installed for MycoPunk
2. Copy `SkipIntro.dll` from the build folder
3. Place it in `<MycoPunk Game Directory>/BepInEx/plugins/`
4. Launch the game

### Executing program

Once installed, the mod works automatically on every game launch:

**Skipped Content:**
- Boot screen with Mycopunk logo
- Splash screen animations
- Loading bars and progress indicators
- Initialization verification messages
- Wipe transitions and UI animations

**Preserved Features:**
- Log text for debugging
- Main menu music playback
- Cursor unlocking for navigation
- First-time profile setup and auto-hosting
- Settings window initialization
- Profile validation checks

**Launch Behavior:**
- Game starts directly at the main menu
- No downtime waiting for intro sequence
- Full functionality preserved post-skip

## Help

* **Still seeing intro?** Check BepInEx console logs - mod may not be loading correctly
* **Music not playing?** Mod preserves music playback - check your audio settings
* **Profile issues?** First-time setup logic is preserved - contact support if profile validation fails
* **Cursor locked?** Mod unlocks cursor - check other mods if cursor remains locked
* **Performance impact?** Zero - only runs during startup initialization
* **Conflicts?** Unlikely but avoid other mods modifying StartMenu behavior
* **Need intro back?** Disable the mod to restore original intro sequence
* **Debugging?** Check BepInEx logs for detailed skip process information

## Authors

* Sparroh
* funlennysub (original mod template)
* [@DomPizzie](https://twitter.com/dompizzie) (README template)

## License

* This project is licensed under the MIT License - see the LICENSE.md file for details
