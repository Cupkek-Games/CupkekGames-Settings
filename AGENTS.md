# CupkekGames Settings — AI Agent Instructions

## Package Overview

**CupkekGames Settings** (`com.cupkekgames.settings`) is the settings registry + Luna UI panel. Bridges Unity URP/Localization/Input/GameSave systems for end-user-facing configuration.

## Critical: Do not hand-edit Unity serialized assets or `.meta` files

Apply scene/prefab/SO changes in Unity Editor; preserve `.meta` GUIDs.

## Package Structure

```
com.cupkekgames.settings/
  package.json
  README.md
  AGENTS.md
  SettingsSystem/                  ← CupkekGames.Settings.asmdef
    Runtime/                         (settings registry, URP/Localization/Input adapters)
  UI.Settings/                     ← CupkekGames.Settings.UI.asmdef
                                     (Luna settings panel + control widgets)
  Tests/                           ← CupkekGames.Settings.Tests.asmdef

(The GameSave UI (save slots, version label) lives in `com.cupkekgames.gamesave.luna`, not here.)
```

## Dependencies

- `com.cupkekgames.singletons` (`SettingsSystem` singleton)
- `com.cupkekgames.keyvaluedatabases` (`SettingsDataSO` extends `KeyValueDatabaseSO`)
- `com.cupkekgames.editorinspector` (`[MultiLineHeader]`)
- `com.cupkekgames.input`
- `com.cupkekgames.luna`
- `com.cupkekgames.data`, `com.cupkekgames.gamesave` (settings persist via GameSave)
- `com.cupkekgames.services`, `com.cupkekgames.sequencer`, `com.cupkekgames.scenemanagement`, `com.cupkekgames.addressableassets`
- `com.unity.localization`, `com.unity.inputsystem`, `com.unity.render-pipelines.universal` (versionDefines)

Note: `EnumHelper` (formerly in `com.cupkekgames.core`) was folded into this package's `SettingsSystem/Runtime/EnumHelper.cs` since `SettingsMenuViewGraphics` is its only consumer.

## Coding Conventions

- **Namespaces**: `CupkekGames.Settings`, `CupkekGames.Settings.UI`
- **versionDefines** for Localization/Input/URP — code paths gate on package presence
- **Asmdefs**: GUID references
- **Strict typing**
