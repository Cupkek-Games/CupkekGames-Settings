using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace CupkekGames.Settings.Tests
{
  /// <summary>
  /// The player's settings load into a runtime copy, never into a project asset
  /// (an asset loaded with the save shows up modified after every play session).
  /// </summary>
  public class CurrentSettingsTests
  {
    private const string Key = "graphics";
    private const string PrefsKey = "settings_graphics_AntiAliasing";

    private SettingsDataSO _defaults;
    private SettingsDataSectionGraphics _defaultGraphics;
    private SettingsDataSO _current;
    private bool _hadPrefs;
    private int _oldPrefs;

    [SetUp]
    public void SetUp()
    {
      _hadPrefs = PlayerPrefs.HasKey(PrefsKey);
      _oldPrefs = PlayerPrefs.GetInt(PrefsKey);

      _defaults = ScriptableObject.CreateInstance<SettingsDataSO>();
      _defaultGraphics = ScriptableObject.CreateInstance<SettingsDataSectionGraphics>();
      var so = new SerializedObject(_defaultGraphics);
      so.FindProperty("_antiAliasing").intValue = (int)SettingsDataSectionGraphics.SettingsAntiAliasing.Four;
      so.ApplyModifiedPropertiesWithoutUndo();
      _defaults.TryAdd(Key, _defaultGraphics);
    }

    [TearDown]
    public void TearDown()
    {
      if (_hadPrefs) PlayerPrefs.SetInt(PrefsKey, _oldPrefs);
      else PlayerPrefs.DeleteKey(PrefsKey);

      if (_current != null)
      {
        foreach (SettingsDataSection section in _current.Values) Object.DestroyImmediate(section);
        Object.DestroyImmediate(_current);
      }

      Object.DestroyImmediate(_defaultGraphics);
      Object.DestroyImmediate(_defaults);
    }

    [Test]
    public void CreateCurrentSettings_LoadsTheSaveIntoACopy_AndLeavesTheDefaultsAlone()
    {
      PlayerPrefs.SetInt(PrefsKey, (int)SettingsDataSectionGraphics.SettingsAntiAliasing.Off);

      _current = SettingsSystem.CreateCurrentSettings(_defaults);

      Assert.AreNotSame(_defaults, _current);
      Assert.IsFalse(EditorUtility.IsPersistent(_current), "The current settings must not be an asset.");
      var currentGraphics = (SettingsDataSectionGraphics)_current.GetValue(Key);
      Assert.AreNotSame(_defaultGraphics, currentGraphics);
      Assert.IsFalse(EditorUtility.IsPersistent(currentGraphics));
      Assert.AreEqual(SettingsDataSectionGraphics.SettingsAntiAliasing.Off, currentGraphics.AntiAliasing, "The save loads into the copy.");
      Assert.AreEqual(SettingsDataSectionGraphics.SettingsAntiAliasing.Four, _defaultGraphics.AntiAliasing, "The defaults stay authored.");
    }

    [Test]
    public void CreateCurrentSettings_FailsLoud_WithoutDefaults()
    {
      Assert.Throws<System.InvalidOperationException>(() => SettingsSystem.CreateCurrentSettings(null));
    }
  }
}
