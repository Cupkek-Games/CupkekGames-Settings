# CupkekGames Settings

Settings registry + Luna UI panel: graphics quality, audio, locale, input bindings.

## What's inside

- **`SettingsSystem/`** (CupkekGames.Systems.Settings.asmdef) — settings registry, persistence, Unity URP/Localization/Input adapters.
- **`UI.Settings/`** (CupkekGames.Systems.UI.Settings.asmdef) — Luna UI settings panel + per-control widgets.

`UI.WithGameSave` (autosave indicator UI bridge) lives in Luna's `Samples~/GameFull/Scripts/UI.WithGameSave/` rather than here — it's sample-quality scaffolding driven by GameSave events from the data package.

## Dependencies

- `com.cupkekgames.core`
- `com.cupkekgames.luna` (UI components)
- `com.cupkekgames.data` (settings persistence via GameSave from data; UI.WithGameSave subscribes to GameSave events)
- `com.unity.localization` (locale switching)
- `com.unity.inputsystem` (input rebinding)
- `com.unity.render-pipelines.universal` (graphics quality)
