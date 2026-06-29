using System.Collections.Generic;
using UnityEngine.UIElements;
using CupkekGames.Services;
using CupkekGames.Settings;

#if UNITY_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace CupkekGames.Settings.UI
{
  public class SettingsMenuViewGameplay : SettingsMenuViewSection
  {
#if UNITY_LOCALIZATION
    private AsyncOperationHandle<Locale> _initializeOperation;
#endif

    // Gameplay Settings
    private DropdownField _dropdownLanguage;

    protected override void Initialize()
    {
      // Language Settings
      _dropdownLanguage = Root.Q<DropdownField>("DropdownLanguage");

#if UNITY_LOCALIZATION
      if (!LocalizationSettings.HasSettings)
      {
        // Localization package is installed, but no LocalizationSettings asset
        // is configured in the project. Don't start the (failing) init — show a
        // clear, disabled fallback instead of triggering an error storm.
        _dropdownLanguage.choices = new List<string>() { "Localization Not Configured" };
        _dropdownLanguage.value = _dropdownLanguage.choices[0];
        _dropdownLanguage.SetEnabled(false);
        return;
      }

      _initializeOperation = LocalizationSettings.SelectedLocaleAsync;
      if (_initializeOperation.IsDone)
      {
        LocalizationInitializeCompleted(_initializeOperation);
      }
      else
      {
        _initializeOperation.Completed += LocalizationInitializeCompleted;
      }
#else
      _dropdownLanguage.choices = new List<string>() { "Localization Package Not Installed", "Option 2", "Option 3" };
      _dropdownLanguage.value = _dropdownLanguage.choices[0];
#endif

#if UNITY_LOCALIZATION
      _dropdownLanguage.RegisterValueChangedCallback(OnDropdownLanguageChanged);
#endif
    }

    public void OnDisable()
    {
#if UNITY_LOCALIZATION
      _dropdownLanguage.UnregisterValueChangedCallback(OnDropdownLanguageChanged);
#endif
    }

    public override void OnTabSelected()
    {
      _dropdownLanguage.Focus();
    }

#if UNITY_LOCALIZATION
    public string LocaleToString(Locale locale)
    {
      return locale.Identifier.CultureInfo != null ? locale.Identifier.CultureInfo.NativeName : locale.ToString();
    }
#endif
    // Apply Settings to UI
    public override void ApplySettingsToUI()
    {
#if UNITY_LOCALIZATION
      if (!LocalizationSettings.HasSettings) return;

      SettingsDataSectionLocalization localization = (SettingsDataSectionLocalization)_changedSettings.GetValue("localization");

      if (_dropdownLanguage.choices.Count > localization.LocaleIndex && localization.LocaleIndex > -1)
      {
        _dropdownLanguage.value = _dropdownLanguage.choices[localization.LocaleIndex];
      }
#endif
    }

#if UNITY_LOCALIZATION
    private void LocalizationInitializeCompleted(AsyncOperationHandle<Locale> obj)
    {
      _initializeOperation.Completed -= LocalizationInitializeCompleted;

      // Create an option in the dropdown for each Locale
      var options = new List<string>();
      int selectedOption = 0;
      var locales = LocalizationSettings.AvailableLocales.Locales;
      for (int i = 0; i < locales.Count; ++i)
      {
        var locale = locales[i];
        if (LocalizationSettings.SelectedLocale == locale)
        {
          selectedOption = i;
        }

        options.Add(LocaleToString(locale));
      }

      // If we have no Locales then something may have gone wrong.
      if (options.Count == 0)
      {
        options.Add("No Locales Available");
        _dropdownLanguage.SetEnabled(false);
      }
      else
      {
        _dropdownLanguage.SetEnabled(true);
      }

      _dropdownLanguage.choices = options;
      _dropdownLanguage.value = _dropdownLanguage.choices[selectedOption];
    }
#endif

#if UNITY_LOCALIZATION
    private void OnDropdownLanguageChanged(ChangeEvent<string> evt)
    {
      SettingsDataSectionLocalization localization = (SettingsDataSectionLocalization)_changedSettings.GetValue("localization");

      if (LocalizationSettings.AvailableLocales.Locales.Count > _dropdownLanguage.index && _dropdownLanguage.index > -1)
      {
        localization.LocaleIdentifier = LocalizationSettings.AvailableLocales.Locales[_dropdownLanguage.index].Identifier;
        SettingsSystem.Instance.ScheduleApplySelectedLocale(localization);
      }
    }
#endif
  }
}