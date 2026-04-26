# CupkekGames Settings — AI Agent Instructions

## Package Overview

**CupkekGames Settings** (`com.cupkekgames.settings`) is the settings registry + Luna UI panel + autosave indicator. Bridges Unity URP/Localization/Input/GameSave systems for end-user-facing configuration.

## Critical: Do not hand-edit Unity serialized assets or `.meta` files

Apply scene/prefab/SO changes in Unity Editor; preserve `.meta` GUIDs.

## Package Structure

```
com.cupkekgames.settings/
  package.json
  README.md
  AGENTS.md
  SettingsSystem/                  ← CupkekGames.Systems.Settings.asmdef
    Runtime/                         (settings registry, URP/Localization/Input adapters)
    Editor/
  UI.Settings/                     ← CupkekGames.Systems.UI.Settings.asmdef
    Runtime/                         (Luna settings panel + control widgets)
    Editor/

(UI.WithGameSave — autosave indicator UI — lives in Luna's `Samples~/GameFull/Scripts/UI.WithGameSave/`, not here. It's sample-grade scaffolding.)
```

## Dependencies

- `com.cupkekgames.core`
- `com.cupkekgames.luna`
- `com.cupkekgames.data` (settings persist via GameSave; autosave UI listens to GameSave events)
- `com.unity.localization`, `com.unity.inputsystem`, `com.unity.render-pipelines.universal` (versionDefines)

## Coding Conventions

- **Namespaces**: `CupkekGames.Systems.Settings`, `CupkekGames.Systems.UI.Settings`, `CupkekGames.Systems.UI.WithGameSave`
- **versionDefines** for Localization/Input/URP — code paths gate on package presence
- **Asmdefs**: GUID references
- **Strict typing**
