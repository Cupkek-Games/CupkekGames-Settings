using UnityEngine;
using CupkekGames.Core;

#if UNITY_LOCALIZATION
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#endif

namespace CupkekGames.Systems
{
  public class SettingsSystem : Singleton<SettingsSystem>
  {
    [SerializeField] private SettingsDataSO _currentSettings;
    public SettingsDataSO CurrentSettings => _currentSettings;
    [SerializeField] private SettingsDataSO _defaultSettings;
    public SettingsDataSO DefaultSettings => _defaultSettings;

#if UNITY_LOCALIZATION
    private const string LocalizationSectionKey = "localization";

    private Coroutine _applySelectedLocaleRoutine;
#endif

    private void Start()
    {
      CurrentSettings.CopyValuesFrom(DefaultSettings);
      CurrentSettings.LoadFromPlayerPrefs();
      ApplySettings(CurrentSettings);
    }

    public void SaveAndApplySettings()
    {
      CurrentSettings.SaveToPlayerPrefs();
      ApplySettings(_currentSettings);
    }

    public void ApplySettings(SettingsDataSO settingsData)
    {
      Debug.Log("Applying settings");

      foreach (var key in _currentSettings.Keys)
      {
        _currentSettings.GetValue(key).ApplySettings(settingsData.GetValue(key));
      }

#if UNITY_LOCALIZATION
      if (_currentSettings.TryGetValue(LocalizationSectionKey, out var loc) &&
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