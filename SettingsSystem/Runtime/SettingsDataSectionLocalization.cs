using UnityEngine;

#if UNITY_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#endif

namespace CupkekGames.Settings
{
  [CreateAssetMenu(fileName = "SectionLocalization", menuName = "CupkekGames/Settings/Section/Localization")]
  public class SettingsDataSectionLocalization : SettingsDataSection
  {
#if UNITY_LOCALIZATION
    [SerializeField] private LocaleIdentifier _localeIdentifier = new LocaleIdentifier("en");
    public LocaleIdentifier LocaleIdentifier
    {
      get
      {
        return _localeIdentifier;
      }
      set
      {
        // Do not touch LocalizationSettings here: AvailableLocales.Locales uses WaitForCompletion,
        // which throws if called from an Addressables/async completion callback (e.g. during init).
        // SettingsSystem applies the locale after InitializationOperation via coroutine.
        _localeIdentifier = value;
      }
    }

    public Locale Locale => LocalizationSettings.AvailableLocales.GetLocale(LocaleIdentifier);

    public int LocaleIndex => LocalizationSettings.AvailableLocales.Locales.IndexOf(Locale);
#endif

    public override bool Equals(object obj)
    {
      // Check for null and compare types
      if (obj == null || GetType() != obj.GetType())
        return false;

#if UNITY_LOCALIZATION
      // Cast and compare properties
      SettingsDataSectionLocalization b = (SettingsDataSectionLocalization)obj;

      return LocaleIdentifier == b.LocaleIdentifier;
#else
      return true;
#endif
    }

    public override int GetHashCode()
    {
#if UNITY_LOCALIZATION
      return LocaleIdentifier.GetHashCode();
#else
      return "REQUIRES UNITY_LOCALIZATION".GetHashCode();
#endif
    }
    public override void SaveToPlayerPrefs(string key)
    {
#if UNITY_LOCALIZATION
      PlayerPrefs.SetString($"{key}_LocaleIdentifier", _localeIdentifier.ToString());
      PlayerPrefs.Save();
#endif
    }
    public override void LoadFromPlayerPrefs(string key)
    {
#if UNITY_LOCALIZATION
      if (PlayerPrefs.HasKey($"{key}_LocaleIdentifier"))
      {
        string localeCode = PlayerPrefs.GetString($"{key}_LocaleIdentifier");
        _localeIdentifier = new LocaleIdentifier(localeCode);
      }
#endif
    }

    public override void CopyValuesFrom(SettingsDataSection section)
    {
#if UNITY_LOCALIZATION
      if (section is SettingsDataSectionLocalization copy)
      {
        _localeIdentifier = copy.LocaleIdentifier;
      }
#endif
    }

    public override void ApplySettings(SettingsDataSection settingsData)
    {
#if UNITY_LOCALIZATION
      if (settingsData is SettingsDataSectionLocalization copy)
      {
        LocaleIdentifier = copy.LocaleIdentifier;
      }
#endif
    }
  }
}