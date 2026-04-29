using System;
using System.Linq;
using UnityEngine;

#if UNITY_URP
using UnityEngine.Rendering.Universal;
#endif

namespace CupkekGames.Settings
{
  [CreateAssetMenu(fileName = "SectionGraphics", menuName = "CupkekGames/Settings/Section/Graphics")]
  public class SettingsDataSectionGraphics : SettingsDataSection
  {
#if UNITY_URP
    [Header("URP Settings Assets [0 = low, 1 = medium, 2 = high, 3 = ultra]")]
    [SerializeField] public UniversalRenderPipelineAsset[] RenderPipelineAssets; // 0 = low, 1 = medium, 2 = high, 3 = ultra
#endif
    [SerializeField] private uint _refreshRateDenominator = 0;
    [SerializeField] private uint _refreshRateNumerator = 0;
    [Header("Set resolution width to 0 for default/native resolution")]
    [SerializeField] private int _resolutionWidth = 0;
    [SerializeField] private int _resolutionHeight = 0;
    public Resolution Resolution
    {
      get
      {
        if (_resolutionWidth == 0)
        {
          return Screen.resolutions.Last();
        }

        return new Resolution
        {
          width = _resolutionWidth,
          height = _resolutionHeight,
          refreshRateRatio = new RefreshRate
          {
            denominator = _refreshRateDenominator,
            numerator = _refreshRateNumerator
          }
        };
      }
      set
      {
        _resolutionWidth = value.width;
        _resolutionHeight = value.height;
        _refreshRateDenominator = value.refreshRateRatio.denominator;
        _refreshRateNumerator = value.refreshRateRatio.numerator;

        Screen.SetResolution(_resolutionWidth, _resolutionHeight, _fullScreenMode,
          new RefreshRate
          {
            denominator = _refreshRateDenominator,
            numerator = _refreshRateNumerator
          }
        );
      }
    }

    [SerializeField] private FullScreenMode _fullScreenMode;
    public FullScreenMode FullScreenMode
    {
      get
      {
        return _fullScreenMode;
      }
      set
      {
        _fullScreenMode = value;
        Screen.fullScreenMode = FullScreenMode;
      }
    }
    [SerializeField] private bool _vSync;
    public bool VSync
    {
      get
      {
        return _vSync;
      }
      set
      {
        _vSync = value;
        QualitySettings.vSyncCount = _vSync ? 1 : 0;
      }
    }
    [SerializeField] private SettingsTargetFrameRate _targetFrameRate = SettingsTargetFrameRate.Sixty;
    public SettingsTargetFrameRate TargetFrameRate
    {
      get
      {
        return _targetFrameRate;
      }
      set
      {
        _targetFrameRate = value;
        Application.targetFrameRate = (int)TargetFrameRate;
      }
    }

    public enum SettingsTargetFrameRate
    {
      Unlimited = -1,
      Thirty = 30,
      Sixty = 60,
      OneTwenty = 120,
      OneFortyFour = 144
    }

#if UNITY_URP
    [SerializeField] private SettingsAntiAliasing _antiAliasing;
    public SettingsAntiAliasing AntiAliasing
    {
      get
      {
        return _antiAliasing;
      }
      set
      {
        _antiAliasing = value;

        foreach (UniversalRenderPipelineAsset renderPipelineAsset in RenderPipelineAssets)
        {
          renderPipelineAsset.msaaSampleCount = (int)AntiAliasing;
        }
      }
    }

    public enum SettingsAntiAliasing
    {
      Off = 1,
      Two = 2,
      Four = 4,
      Eight = 8
    }
#endif

    [SerializeField] private SettingsPostProcessing _postProcessing;
    public SettingsPostProcessing PostProcessing
    {
      get
      {
        return _postProcessing;
      }
      set
      {
        _postProcessing = value;
        // TODO: Implement Post Processing Quality
      }
    }

    public enum SettingsPostProcessing
    {
      Low = 0,
      Medium = 1,
      High = 2,
      Ultra = 3
    }

#if UNITY_URP
    [SerializeField] private SettingsShadows _shadows;
    public SettingsShadows Shadows
    {
      get
      {
        return _shadows;
      }
      set
      {
        _shadows = value;


        QualitySettings.renderPipeline = RenderPipelineAssets[(int)_shadows];
      }
    }

    public enum SettingsShadows
    {
      Low = 0,
      Medium = 1,
      High = 2,
      Ultra = 3
    }
#endif

    [SerializeField] private SettingsTextureQuality _textureQuality; // 0 = original size, 1 = half size, 2 = quarter size, 3 = eighth size
    public SettingsTextureQuality TextureQuality
    {
      get
      {
        return _textureQuality;
      }
      set
      {
        _textureQuality = value;

        QualitySettings.globalTextureMipmapLimit = (int)_textureQuality;
      }
    }

    public enum SettingsTextureQuality
    {
      Original = 0,
      Half = 1,
      Quarter = 2,
      Eighth = 3
    }

    [SerializeField] private SettingsEffectsQuality _effectsQuality;
    public SettingsEffectsQuality EffectsQuality
    {
      get
      {
        return _effectsQuality;
      }
      set
      {
        _effectsQuality = value;

        // TODO: Implement Effects Quality
      }
    }

    public enum SettingsEffectsQuality
    {
      Low = 0,
      Medium = 1,
      High = 2,
      Ultra = 3
    }

    public override void SaveToPlayerPrefs(string key)
    {
      PlayerPrefs.SetInt($"{key}_FullScreenMode", (int)_fullScreenMode);
      PlayerPrefs.SetInt($"{key}_VSync", VSync ? 1 : 0);
      PlayerPrefs.SetInt($"{key}_TargetFrameRate", (int)TargetFrameRate);
#if UNITY_URP
      PlayerPrefs.SetInt($"{key}_AntiAliasing", (int)AntiAliasing);
      PlayerPrefs.SetInt($"{key}_Shadows", (int)Shadows);
#endif
      PlayerPrefs.SetInt($"{key}_PostProcessing", (int)PostProcessing);
      PlayerPrefs.SetInt($"{key}_TextureQuality", (int)TextureQuality);
      PlayerPrefs.SetInt($"{key}_EffectsQuality", (int)EffectsQuality);

      PlayerPrefs.SetInt($"{key}_ResolutionWidth", _resolutionWidth);
      PlayerPrefs.SetInt($"{key}_ResolutionHeight", _resolutionHeight);
      PlayerPrefs.SetInt($"{key}_RefreshRateDenominator", (int)_refreshRateDenominator);
      PlayerPrefs.SetInt($"{key}_RefreshRateNumerator", (int)_refreshRateNumerator);
      PlayerPrefs.Save();
    }

    public override void LoadFromPlayerPrefs(string key)
    {
      if (PlayerPrefs.HasKey($"{key}_FullScreenMode"))
      {
        _fullScreenMode = (FullScreenMode)PlayerPrefs.GetInt($"{key}_FullScreenMode");
      }
      if (PlayerPrefs.HasKey($"{key}_VSync"))
      {
        _vSync = PlayerPrefs.GetInt($"{key}_VSync") == 1;
      }
      if (PlayerPrefs.HasKey($"{key}_TargetFrameRate"))
      {
        _targetFrameRate = (SettingsTargetFrameRate)PlayerPrefs.GetInt($"{key}_TargetFrameRate");
      }
#if UNITY_URP
      if (PlayerPrefs.HasKey($"{key}_AntiAliasing"))
      {
        _antiAliasing = (SettingsAntiAliasing)PlayerPrefs.GetInt($"{key}_AntiAliasing");
      }
      if (PlayerPrefs.HasKey($"{key}_Shadows"))
      {
        _shadows = (SettingsShadows)PlayerPrefs.GetInt($"{key}_Shadows");
      }
#endif
      if (PlayerPrefs.HasKey($"{key}_PostProcessing"))
      {
        _postProcessing = (SettingsPostProcessing)PlayerPrefs.GetInt($"{key}_PostProcessing");
      }
      if (PlayerPrefs.HasKey($"{key}_TextureQuality"))
      {
        _textureQuality = (SettingsTextureQuality)PlayerPrefs.GetInt($"{key}_TextureQuality");
      }
      if (PlayerPrefs.HasKey($"{key}_EffectsQuality"))
      {
        _effectsQuality = (SettingsEffectsQuality)PlayerPrefs.GetInt($"{key}_EffectsQuality");
      }

      if (PlayerPrefs.HasKey($"{key}_ResolutionWidth"))
      {
        _resolutionWidth = PlayerPrefs.GetInt($"{key}_ResolutionWidth");
      }
      if (PlayerPrefs.HasKey($"{key}_ResolutionHeight"))
      {
        _resolutionHeight = PlayerPrefs.GetInt($"{key}_ResolutionHeight");
      }
      if (PlayerPrefs.HasKey($"{key}_RefreshRateDenominator"))
      {
        _refreshRateDenominator = (uint)PlayerPrefs.GetInt($"{key}_RefreshRateDenominator");
      }
      if (PlayerPrefs.HasKey($"{key}_RefreshRateNumerator"))
      {
        _refreshRateNumerator = (uint)PlayerPrefs.GetInt($"{key}_RefreshRateNumerator");
      }
    }

    public override void CopyValuesFrom(SettingsDataSection section)
    {
      if (section is SettingsDataSectionGraphics copy)
      {
#if UNITY_URP
        RenderPipelineAssets = copy.RenderPipelineAssets;
#endif

        _resolutionWidth = copy._resolutionWidth;
        _resolutionHeight = copy._resolutionHeight;
        _refreshRateDenominator = copy._refreshRateDenominator;
        _refreshRateNumerator = copy._refreshRateNumerator;
        _fullScreenMode = copy._fullScreenMode;
        _vSync = copy.VSync;
        _targetFrameRate = copy.TargetFrameRate;
#if UNITY_URP
        _antiAliasing = copy.AntiAliasing;
        _shadows = copy.Shadows;
#endif
        _postProcessing = copy.PostProcessing;
        _textureQuality = copy.TextureQuality;
        _effectsQuality = copy.EffectsQuality;
      }
    }

    public override void ApplySettings(SettingsDataSection settingsData)
    {
      if (settingsData is SettingsDataSectionGraphics copy)
      {
        Resolution = copy.Resolution;
        FullScreenMode = copy.FullScreenMode;
        TargetFrameRate = copy.TargetFrameRate;
        VSync = copy.VSync;
        PostProcessing = copy.PostProcessing;

#if UNITY_URP
        AntiAliasing = copy.AntiAliasing;
        Shadows = copy.Shadows;
#endif

        TextureQuality = copy.TextureQuality;
        EffectsQuality = copy.EffectsQuality;
      }
    }

    public override bool Equals(object obj)
    {
      // Check for null and compare types
      if (obj == null || GetType() != obj.GetType())
        return false;

      // Cast and compare properties
      SettingsDataSectionGraphics b = (SettingsDataSectionGraphics)obj;

      Resolution resolution = Resolution;
      Resolution resolutionB = b.Resolution;

      return resolution.width == resolutionB.width &&
              resolution.height == resolutionB.height &&
              resolution.refreshRateRatio.denominator == resolutionB.refreshRateRatio.denominator &&
              resolution.refreshRateRatio.numerator == resolutionB.refreshRateRatio.numerator &&
             _fullScreenMode == b._fullScreenMode &&
             VSync == b.VSync &&
             TargetFrameRate == b.TargetFrameRate &&
#if UNITY_URP
             AntiAliasing == b.AntiAliasing &&
             Shadows == b.Shadows &&
#endif
             PostProcessing == b.PostProcessing &&
             TextureQuality == b.TextureQuality &&
             EffectsQuality == b.EffectsQuality;
    }

    public override int GetHashCode()
    {
      // HashCode#Combine takes max 8 arguments
      Resolution resolution = Resolution;
      int hash1 = HashCode.Combine(resolution.width, resolution.height, resolution.refreshRateRatio.denominator,
        resolution.refreshRateRatio.numerator, _fullScreenMode);
#if UNITY_URP
      int hash2 = HashCode.Combine(VSync, TargetFrameRate, AntiAliasing, PostProcessing, Shadows, TextureQuality, EffectsQuality);
#else
      int hash2 = HashCode.Combine(VSync, TargetFrameRate, PostProcessing, TextureQuality, EffectsQuality);
#endif

      return HashCode.Combine(hash1, hash2);
    }
  }
}