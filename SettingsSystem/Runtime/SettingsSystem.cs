using UnityEngine;
using CupkekGames.Singletons;

#if UNITY_LOCALIZATION
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#endif

namespace CupkekGames.Settings
{
  public class SettingsSystem : Singleton<SettingsSystem>
  {
    [SerializeField] private SettingsDataSO _defaultSettings;
    public SettingsDataSO DefaultSettings => _defaultSettings;

    // The player's settings live in a runtime copy of the defaults, never in a project
    // asset: an asset the save is loaded into shows up modified after every editor
    // play session (and a stale copy of the player's save ends up in version control).
    private SettingsDataSO _currentSettings;

    /// <summary>
    /// The settings in effect: a runtime copy of <see cref="DefaultSettings"/> with the
    /// player's saved values loaded over it. Built on first use, so it is ready for
    /// readers whose Awake or Start runs before this one's.
    /// </summary>
    public SettingsDataSO CurrentSettings
    {
      get
      {
        if (_currentSettings == null)
        {
          _currentSettings = CreateCurrentSettings(_defaultSettings);
        }

        return _currentSettings;
      }
    }

    /// <summary>A runtime copy of <paramref name="defaults"/> with the saved values loaded.</summary>
    public static SettingsDataSO CreateCurrentSettings(SettingsDataSO defaults)
    {
      if (defaults == null)
      {
        throw new System.InvalidOperationException("[SettingsSystem] No default settings assigned.");
      }

      SettingsDataSO current = ScriptableObject.CreateInstance<SettingsDataSO>();
      current.name = defaults.name + " (current)";
      current.hideFlags = HideFlags.DontSave;
      current.CopyValuesFrom(defaults);
      current.LoadFromPlayerPrefs();
      return current;
    }

#if UNITY_LOCALIZATION
    private const string LocalizationSectionKey = "localization";

    private Coroutine _applySelectedLocaleRoutine;
#endif

    private void Start()
    {
      ApplySettings(CurrentSettings);
    }

    protected virtual void OnDestroy()
    {
      if (_currentSettings == null)
      {
        return;
      }

      foreach (SettingsDataSection section in _currentSettings.Values)
      {
        if (section != null)
        {
          Destroy(section);
        }
      }

      Destroy(_currentSettings);
      _currentSettings = null;
    }

    public void SaveAndApplySettings()
    {
      CurrentSettings.SaveToPlayerPrefs();
      ApplySettings(CurrentSettings);
    }

    public void ApplySettings(SettingsDataSO settingsData)
    {
      Debug.Log("Applying settings");

      SettingsDataSO current = CurrentSettings;
      foreach (var key in current.Keys)
      {
        current.GetValue(key).ApplySettings(settingsData.GetValue(key));
      }

#if UNITY_LOCALIZATION
      if (current.TryGetValue(LocalizationSectionKey, out var loc) &&
          loc is SettingsDataSectionLocalization locSection)
        ScheduleApplySelectedLocale(locSection);
#endif
    }

#if UNITY_LOCALIZATION
    /// <summary>
    /// Applies <see cref="LocalizationSettings.SelectedLocale"/> after Addressables/localization init,
    /// avoiding WaitForCompletion while inside ResourceManager callbacks.
    /// </summary>
    public void ScheduleApplySelectedLocale(SettingsDataSectionLocalization section)
    {
      if (section == null)
        return;

      // No LocalizationSettings configured in the project → nothing to apply.
      // Without this guard, LocalizationSettings.InitializationOperation faults
      // (no locales/tables exist) and spams the console every settings-apply.
      if (!LocalizationSettings.HasSettings)
        return;

      if (_applySelectedLocaleRoutine != null)
        StopCoroutine(_applySelectedLocaleRoutine);

      _applySelectedLocaleRoutine = StartCoroutine(ApplySelectedLocaleWhenSafe(section));
    }

    private IEnumerator ApplySelectedLocaleWhenSafe(SettingsDataSectionLocalization section)
    {
      yield return LocalizationSettings.InitializationOperation;
      // Leave any synchronous completion stack before Locales may call WaitForCompletion.
      yield return null;

      if (section != null)
      {
        Locale locale = LocalizationSettings.AvailableLocales.GetLocale(section.LocaleIdentifier);
        if (locale != null)
          LocalizationSettings.SelectedLocale = locale;
      }

      _applySelectedLocaleRoutine = null;
    }
#endif
  }
}