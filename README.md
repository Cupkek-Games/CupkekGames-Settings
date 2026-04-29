# CupkekGames Settings

Settings registry + Luna UI panel: graphics quality, audio, locale, input bindings.

## What's inside

- **`SettingsSystem/`** (CupkekGames.Systems.Settings.asmdef) — settings registry, persistence, Unity URP/Localization/Input adapters.
- **`UI.Settings/`** (CupkekGames.Systems.UI.Settings.asmdef) — Luna UI settings panel + per-control widgets.

`UI.WithGameSave` (autosave indicator UI bridge) lives in Luna's `Samples~/GameFull/Scripts/UI.WithGameSave/` rather than here — it's sample-quality scaffolding driven by GameSave events from the data package.

## Dependencies

- `com.cupkekgames.singleton` (`SettingsSystem` singleton)
- `com.cupkekgames.keyvaluedatabase` (`SettingsDataSO` extends `KeyValueDatabaseSO`)
- `com.cupkekgames.editorinspector` (`[MultiLineHeader]` on data classes)
- `com.cupkekgames.input`
- `com.cupkekgames.luna` (UI components)
- `com.cupkekgames.data` (settings persistence via GameSave; UI.WithGameSave subscribes to GameSave events)
- `com.unity.localization` (locale switching)
- `com.unity.inputsystem` (input rebinding)
- `com.unity.render-pipelines.universal` (graphics quality)

Note: `EnumHelper` (small static helper, formerly in `com.cupkekgames.core`) was folded into this package's `SettingsSystem/Runtime/EnumHelper.cs` since `SettingsMenuViewGraphics` was its only consumer.
