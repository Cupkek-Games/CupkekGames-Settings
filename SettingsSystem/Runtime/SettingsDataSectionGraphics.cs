using System;
using System.Linq;
using UnityEngine;

#if UNITY_URP
using UnityEngine.Rendering;
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
        RememberQuality(nameof(QualitySettings.vSyncCount), QualitySettings.vSyncCount, v => QualitySettings.vSyncCount = v);
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

        // URP mirrors the active asset's MSAA into QualitySettings.antiAliasing.
        RememberQuality(nameof(QualitySettings.antiAliasing), QualitySettings.antiAliasing, v => QualitySettings.antiAliasing = v);
        foreach (UniversalRenderPipelineAsset renderPipelineAsset in RenderPipelineAssets)
        {
          if (renderPipelineAsset == null) continue;
          UniversalRenderPipelineAsset asset = renderPipelineAsset;
          AuthoredGraphicsState.Remember(asset, nameof(asset.msaaSampleCount), asset.msaaSampleCount, v => asset.msaaSampleCount = v);
          asset.msaaSampleCount = (int)AntiAliasing;
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

        RememberQuality(nameof(QualitySettings.renderPipeline), QualitySettings.renderPipeline, v => QualitySettings.renderPipeline = v);
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

    // Effects: a coarse Low/High that turns the expensive screen-space work off. What
    // "Low" disables is authored per game on the section asset: renderer features
    // (SSAO, screen-space shadows, ...), the camera opaque-texture copy on the URP
    // assets, the DoF / Bloom-HQ overrides on a volume profile, and a whole profile of
    // optional effects. High is the authored state, not "everything on": every value
    // is remembered before its first write (AuthoredGraphicsState), which is also what
    // the editor puts back on leaving play mode.
    [Header("Effects (what Low turns off)")]
    [Tooltip("Renderer features disabled at Low and re-enabled at High (e.g. SSAO, Screen Space Shadows).")]
    [SerializeField] private ScriptableRendererFeature[] _effectsLowDisablesFeatures;
    [Tooltip("Turn the URP assets' Opaque Texture off at Low (refraction/distortion effects lose scene color).")]
    [SerializeField] private bool _effectsLowDisablesOpaqueTexture;
    [Tooltip("Volume profile whose Depth Of Field / Bloom overrides Low simplifies. Optional.")]
    [SerializeField] private VolumeProfile _effectsVolumeProfile;
    [SerializeField] private bool _effectsLowDisablesDepthOfField = true;
    [SerializeField] private bool _effectsLowDisablesBloomHighQuality = true;
    [Tooltip("Volume profile Low turns off entirely (optional effects only, e.g. a presentation profile with depth of field on close-up cameras). Optional.")]
    [SerializeField] private VolumeProfile _effectsLowDropsProfile;

    [SerializeField] private SettingsEffects _effects = SettingsEffects.High;
    public SettingsEffects Effects
    {
      get
      {
        return _effects;
      }
      set
      {
        _effects = value;
        bool high = _effects == SettingsEffects.High;

        if (_effectsLowDisablesFeatures != null)
        {
          foreach (ScriptableRendererFeature feature in _effectsLowDisablesFeatures)
          {
            if (feature == null) continue;
            bool authored = AuthoredGraphicsState.Remember(feature, nameof(feature.isActive), feature.isActive, feature.SetActive);
            feature.SetActive(high && authored);
          }
        }

        if (_effectsLowDisablesOpaqueTexture && RenderPipelineAssets != null)
        {
          foreach (UniversalRenderPipelineAsset renderPipelineAsset in RenderPipelineAssets)
          {
            if (renderPipelineAsset == null) continue;
            UniversalRenderPipelineAsset asset = renderPipelineAsset;
            bool authored = AuthoredGraphicsState.Remember(asset, nameof(asset.supportsCameraOpaqueTexture),
              asset.supportsCameraOpaqueTexture, v => asset.supportsCameraOpaqueTexture = v);
            asset.supportsCameraOpaqueTexture = high && authored;
          }
        }

        if (_effectsVolumeProfile != null)
        {
          if (_effectsLowDisablesDepthOfField && _effectsVolumeProfile.TryGet(out DepthOfField depthOfField))
          {
            SetComponentActive(depthOfField, high);
          }

          if (_effectsLowDisablesBloomHighQuality && _effectsVolumeProfile.TryGet(out Bloom bloom))
          {
            // The value only counts with its override on, so the two move together.
            BoolParameter hq = bloom.highQualityFiltering;
            (bool Overridden, bool On) authored = AuthoredGraphicsState.Remember(bloom, nameof(bloom.highQualityFiltering),
              (hq.overrideState, hq.value), v => { hq.overrideState = v.Item1; hq.value = v.Item2; });
            hq.overrideState = high ? authored.Overridden : true;
            hq.value = high && authored.On;
          }
        }

        if (_effectsLowDropsProfile != null)
        {
          foreach (VolumeComponent component in _effectsLowDropsProfile.components)
          {
            if (component != null)
            {
              SetComponentActive(component, high);
            }
          }
        }
      }
    }

    public enum SettingsEffects
    {
      Low = 0,
      High = 1
    }

    // High: the authored state. Low: off.
    private static void SetComponentActive(VolumeComponent component, bool high)
    {
      bool authored = AuthoredGraphicsState.Remember(component, nameof(component.active), component.active, v => component.active = v);
      component.active = high && authored;
    }
#endif

    private static void RememberQuality<T>(string property, T current, Action<T> restore)
    {
      AuthoredGraphicsState.Remember(typeof(QualitySettings), property, current, restore);
    }

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

        RememberQuality(nameof(QualitySettings.globalTextureMipmapLimit), QualitySettings.globalTextureMipmapLimit,
          v => QualitySettings.globalTextureMipmapLimit = v);
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

    public override void SaveToPlayerPrefs(string key)
    {
      PlayerPrefs.SetInt($"{key}_FullScreenMode", (int)_fullScreenMode);
      PlayerPrefs.SetInt($"{key}_VSync", VSync ? 1 : 0);
      PlayerPrefs.SetInt($"{key}_TargetFrameRate", (int)TargetFrameRate);
#if UNITY_URP
      PlayerPrefs.SetInt($"{key}_AntiAliasing", (int)AntiAliasing);
      PlayerPrefs.SetInt($"{key}_Shadows", (int)Shadows);
      PlayerPrefs.SetInt($"{key}_Effects", (int)Effects);
#endif
      PlayerPrefs.SetInt($"{key}_TextureQuality", (int)TextureQuality);

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
      if (PlayerPrefs.HasKey($"{key}_Effects"))
      {
        _effects = (SettingsEffects)PlayerPrefs.GetInt($"{key}_Effects");
      }
#endif
      if (PlayerPrefs.HasKey($"{key}_TextureQuality"))
      {
        _textureQuality = (SettingsTextureQuality)PlayerPrefs.GetInt($"{key}_TextureQuality");
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
        _effectsLowDisablesFeatures = copy._effectsLowDisablesFeatures;
        _effectsLowDisablesOpaqueTexture = copy._effectsLowDisablesOpaqueTexture;
        _effectsVolumeProfile = copy._effectsVolumeProfile;
        _effectsLowDisablesDepthOfField = copy._effectsLowDisablesDepthOfField;
        _effectsLowDisablesBloomHighQuality = copy._effectsLowDisablesBloomHighQuality;
        _effectsLowDropsProfile = copy._effectsLowDropsProfile;
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
        _effects = copy.Effects;
#endif
        _textureQuality = copy.TextureQuality;
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

#if UNITY_URP
        AntiAliasing = copy.AntiAliasing;
        Shadows = copy.Shadows;
        Effects = copy.Effects;
#endif

        TextureQuality = copy.TextureQuality;
      }
    }

    public override bool Equals(object obj)
    {
      if (obj == null || GetType() != obj.GetType())
        return false;

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
             Effects == b.Effects &&
#endif
             TextureQuality == b.TextureQuality;
    }

    public override int GetHashCode()
    {
      Resolution resolution = Resolution;
      int hash1 = HashCode.Combine(resolution.width, resolution.height, resolution.refreshRateRatio.denominator,
        resolution.refreshRateRatio.numerator, _fullScreenMode);
#if UNITY_URP
      int hash2 = HashCode.Combine(VSync, TargetFrameRate, AntiAliasing, Shadows, Effects, TextureQuality);
#else
      int hash2 = HashCode.Combine(VSync, TargetFrameRate, TextureQuality);
#endif

      return HashCode.Combine(hash1, hash2);
    }
  }
}
