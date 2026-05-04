using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Settings.UI
{
  public class SettingsMenuViewGraphics : SettingsMenuViewSection
  {
    [Header("Set to 0,0 to not force aspect ratio")]
    [SerializeField] private Vector2 _resolutionAspectRatio = new Vector2(16, 9);

    private DropdownField _dropdownResolution;
    private List<Resolution> _resolutions = new List<Resolution>();

    private RadioButtonGroup _radioGroupWindowMode;

    private RadioButtonGroup _radioGroupFpsLimit;

    private Toggle _toggleVSync;

    private RadioButtonGroup _antiAliasing;

    private RadioButtonGroup _shadows;

    private RadioButtonGroup _textureQuality;

    protected override void Initialize()
    {
      Resolution[] resolutions = Screen.resolutions;

      _dropdownResolution = UIDocument.rootVisualElement.Q<DropdownField>("DropdownResolution");

      List<string> choices = new List<string>();
      _resolutions = new List<Resolution>();
      foreach (var res in resolutions)
      {
        if (_resolutionAspectRatio.x > 0 && res.width % _resolutionAspectRatio.x != 0)
        {
          continue;
        }

        if (_resolutionAspectRatio.y > 0 && res.height % _resolutionAspectRatio.y != 0)
        {
          continue;
        }

        choices.Add(ResolutionToString(res));
        _resolutions.Add(res);
      }
      _dropdownResolution.choices = choices;
      _dropdownResolution.value = _dropdownResolution.choices[0];

      _radioGroupWindowMode = UIDocument.rootVisualElement.Q<RadioButtonGroup>("RadioGroupWindowMode");
      _radioGroupFpsLimit = UIDocument.rootVisualElement.Q<RadioButtonGroup>("RadioGroupFpsLimit");
      _toggleVSync = UIDocument.rootVisualElement.Q<Toggle>("ToggleVSync");
      _antiAliasing = UIDocument.rootVisualElement.Q<RadioButtonGroup>("AntiAliasing");
      _shadows = UIDocument.rootVisualElement.Q<RadioButtonGroup>("Shadows");
      _textureQuality = UIDocument.rootVisualElement.Q<RadioButtonGroup>("TextureQuality");

      _dropdownResolution.RegisterValueChangedCallback(OnDropdownResolutionChanged);
      _radioGroupWindowMode.RegisterValueChangedCallback(OnRadioGroupWindowModeChanged);
      _radioGroupFpsLimit.RegisterValueChangedCallback(OnRadioGroupFpsLimitChanged);
      _toggleVSync.RegisterValueChangedCallback(OnToggleVSyncChanged);
#if UNITY_URP
      _antiAliasing.RegisterValueChangedCallback(OnAntiAliasingChanged);
      _shadows.RegisterValueChangedCallback(OnShadowsChanged);
#endif
      _textureQuality.RegisterValueChangedCallback(OnTextureQualityChanged);
    }

    public void OnDisable()
    {
      _dropdownResolution.UnregisterValueChangedCallback(OnDropdownResolutionChanged);
      _radioGroupWindowMode.UnregisterValueChangedCallback(OnRadioGroupWindowModeChanged);
      _radioGroupFpsLimit.UnregisterValueChangedCallback(OnRadioGroupFpsLimitChanged);
      _toggleVSync.UnregisterValueChangedCallback(OnToggleVSyncChanged);
#if UNITY_URP
      _antiAliasing.UnregisterValueChangedCallback(OnAntiAliasingChanged);
      _shadows.UnregisterValueChangedCallback(OnShadowsChanged);
#endif
      _textureQuality.UnregisterValueChangedCallback(OnTextureQualityChanged);
    }

    public override void OnTabSelected()
    {
      _dropdownResolution.Focus();
    }

    public string ResolutionToString(Resolution resolution)
    {
      return string.Format("{0}x{1} @{2:F2}Hz", resolution.width, resolution.height, resolution.refreshRateRatio.value);
    }

    public override void ApplySettingsToUI()
    {
      SettingsDataSectionGraphics graphics = (SettingsDataSectionGraphics)_changedSettings.GetValue("graphics");

      _dropdownResolution.value = ResolutionToString(graphics.Resolution);
      _radioGroupWindowMode.value = 3 - EnumHelper.GetIndexOfEnum(graphics.FullScreenMode);
      _radioGroupFpsLimit.value = EnumHelper.GetIndexOfEnum(graphics.TargetFrameRate);
      _toggleVSync.value = graphics.VSync;
#if UNITY_URP
      _antiAliasing.value = EnumHelper.GetIndexOfEnum(graphics.AntiAliasing);
      _shadows.value = EnumHelper.GetIndexOfEnum(graphics.Shadows);
#endif
      _textureQuality.value = 3 - EnumHelper.GetIndexOfEnum(graphics.TextureQuality);
    }

    private void OnDropdownResolutionChanged(ChangeEvent<string> evt)
    {
      SettingsDataSectionGraphics graphics = (SettingsDataSectionGraphics)_changedSettings.GetValue("graphics");

      int i = _dropdownResolution.index;
      if (i > -1 && i < _resolutions.Count)
      {
        graphics.Resolution = _resolutions[i];
      }
    }

    private void OnRadioGroupWindowModeChanged(ChangeEvent<int> evt)
    {
      SettingsDataSectionGraphics graphics = (SettingsDataSectionGraphics)_changedSettings.GetValue("graphics");

      if (evt.newValue == 0)
      {
        graphics.FullScreenMode = FullScreenMode.Windowed;
      }
      else
      {
        graphics.FullScreenMode = FullScreenMode.ExclusiveFullScreen;
      }
    }

    private void OnRadioGroupFpsLimitChanged(ChangeEvent<int> evt)
    {
      SettingsDataSectionGraphics graphics = (SettingsDataSectionGraphics)_changedSettings.GetValue("graphics");

      graphics.TargetFrameRate = EnumHelper.GetEnumFromIndex<SettingsDataSectionGraphics.SettingsTargetFrameRate>(evt.newValue);
    }

    private void OnToggleVSyncChanged(ChangeEvent<bool> evt)
    {
      SettingsDataSectionGraphics graphics = (SettingsDataSectionGraphics)_changedSettings.GetValue("graphics");

      graphics.VSync = evt.newValue;
    }

#if UNITY_URP
    private void OnAntiAliasingChanged(ChangeEvent<int> evt)
    {
      SettingsDataSectionGraphics graphics = (SettingsDataSectionGraphics)_changedSettings.GetValue("graphics");

      graphics.AntiAliasing = EnumHelper.GetEnumFromIndex<SettingsDataSectionGraphics.SettingsAntiAliasing>(evt.newValue);
    }

    private void OnShadowsChanged(ChangeEvent<int> evt)
    {
      SettingsDataSectionGraphics graphics = (SettingsDataSectionGraphics)_changedSettings.GetValue("graphics");

      graphics.Shadows = EnumHelper.GetEnumFromIndex<SettingsDataSectionGraphics.SettingsShadows>(evt.newValue);
    }
#endif

    private void OnTextureQualityChanged(ChangeEvent<int> evt)
    {
      SettingsDataSectionGraphics graphics = (SettingsDataSectionGraphics)_changedSettings.GetValue("graphics");

      graphics.TextureQuality = EnumHelper.GetEnumFromIndex<SettingsDataSectionGraphics.SettingsTextureQuality>(3 - evt.newValue);
    }
  }
}
