# CupkekGames Settings

Settings registry + Luna UI panel: graphics quality, audio, locale, input bindings.

## What's inside

- **`SettingsSystem/`** (CupkekGames.Systems.Settings.asmdef) — settings registry, persistence, Unity URP/Localization/Input adapters.
- **`UI.Settings/`** (CupkekGames.Systems.UI.Settings.asmdef) — Luna UI settings panel + per-control widgets.

`UI.WithGameSave` (autosave indicator UI bridge) lives in Luna's `Samples~/GameFull/Scripts/UI.WithGameSave/` rather than here — it's sample-quality scaffolding driven by GameSave events from the data package.

## Graphics: runtime writes to shared assets

The graphics section writes the player's choices into shared assets (URP assets, renderer features, volume profiles, QualitySettings). `AuthoredGraphicsState` remembers each value before its first write: Effects High returns to that authored state instead of switching everything on, and in the editor every value is put back on leaving play mode, so play sessions never save the player's settings into the project. An optional **Effects Low drops profile** turns a whole profile off at Low (since 0.3.5).

## Dependencies

- `com.cupkekgames.singletons` (`SettingsSystem` singleton)
- `com.cupkekgames.keyvaluedatabases` (`SettingsDataSO` extends `KeyValueDatabaseSO`)
- `com.cupkekgames.editorinspector` (`[MultiLineHeader]` on data classes)
- `com.cupkekgames.input`
- `com.cupkekgames.luna` (UI components)
- `com.cupkekgames.data` (settings persistence via GameSave; UI.WithGameSave subscribes to GameSave events)
- `com.unity.localization` (locale switching)
- `com.unity.inputsystem` (input rebinding)
- `com.unity.render-pipelines.universal` (graphics quality)

Note: `EnumHelper` (small static helper, formerly in `com.cupkekgames.core`) was folded into this package's `SettingsSystem/Runtime/EnumHelper.cs` since `SettingsMenuViewGraphics` was its only consumer.
