using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CupkekGames.Settings.Tests
{
  /// <summary>
  /// The graphics section's runtime writes to shared assets: Effects High returns to
  /// the authored state, and every value is put back afterwards (the editor does it on
  /// leaving play mode, which <see cref="AuthoredGraphicsState.RestoreAll"/> stands for here).
  /// </summary>
  public class AuthoredGraphicsStateTests
  {
    private SettingsDataSectionGraphics _section;
    private VolumeProfile _profile;
    private VolumeProfile _dropProfile;
    private UniversalRenderPipelineAsset _urpAsset;

    [SetUp]
    public void SetUp()
    {
      AuthoredGraphicsState.RestoreAll();
      _section = ScriptableObject.CreateInstance<SettingsDataSectionGraphics>();
      _profile = ScriptableObject.CreateInstance<VolumeProfile>();
      _dropProfile = ScriptableObject.CreateInstance<VolumeProfile>();
      _urpAsset = ScriptableObject.CreateInstance<UniversalRenderPipelineAsset>();
    }

    [TearDown]
    public void TearDown()
    {
      AuthoredGraphicsState.RestoreAll();
      Object.DestroyImmediate(_section);
      Object.DestroyImmediate(_profile);
      Object.DestroyImmediate(_dropProfile);
      Object.DestroyImmediate(_urpAsset);
    }

    private void Wire(bool lowDisablesDepthOfField = true, bool lowDisablesBloomHighQuality = true)
    {
      var so = new SerializedObject(_section);
      so.FindProperty("_effectsVolumeProfile").objectReferenceValue = _profile;
      so.FindProperty("_effectsLowDropsProfile").objectReferenceValue = _dropProfile;
      so.FindProperty("_effectsLowDisablesDepthOfField").boolValue = lowDisablesDepthOfField;
      so.FindProperty("_effectsLowDisablesBloomHighQuality").boolValue = lowDisablesBloomHighQuality;
      so.ApplyModifiedPropertiesWithoutUndo();
      _section.RenderPipelineAssets = new[] { _urpAsset };
    }

    [Test]
    public void Remember_KeepsTheFirstValue_AndRestoreAllWritesItBack()
    {
      int value = 1;
      Assert.AreEqual(1, AuthoredGraphicsState.Remember(_profile, "x", value, v => value = v));
      value = 5;
      Assert.AreEqual(1, AuthoredGraphicsState.Remember(_profile, "x", value, v => value = v), "The first value is the authored one.");
      Assert.AreEqual(1, AuthoredGraphicsState.Count);

      AuthoredGraphicsState.RestoreAll();

      Assert.AreEqual(1, value);
      Assert.AreEqual(0, AuthoredGraphicsState.Count);
    }

    [Test]
    public void EffectsHigh_ReturnsToTheAuthoredState_NotEverythingOn()
    {
      var depthOfField = _profile.Add<DepthOfField>();
      depthOfField.active = false; // authored off: a dormant override
      var bloom = _profile.Add<Bloom>();
      bloom.highQualityFiltering.overrideState = true;
      bloom.highQualityFiltering.value = true;
      Wire();

      _section.Effects = SettingsDataSectionGraphics.SettingsEffects.Low;
      Assert.IsFalse(depthOfField.active);
      Assert.IsTrue(bloom.highQualityFiltering.overrideState, "Low overrides Bloom HQ, or the off value never applies.");
      Assert.IsFalse(bloom.highQualityFiltering.value);

      _section.Effects = SettingsDataSectionGraphics.SettingsEffects.High;
      Assert.IsFalse(depthOfField.active, "High must not switch on an override authored off.");
      Assert.IsTrue(bloom.highQualityFiltering.overrideState);
      Assert.IsTrue(bloom.highQualityFiltering.value);
    }

    [Test]
    public void EffectsLow_DropsTheWholeProfile_AndHighBringsItBack()
    {
      var vignette = _dropProfile.Add<Vignette>();
      var dof = _dropProfile.Add<DepthOfField>();
      Wire();

      _section.Effects = SettingsDataSectionGraphics.SettingsEffects.Low;
      Assert.IsFalse(vignette.active);
      Assert.IsFalse(dof.active);

      _section.Effects = SettingsDataSectionGraphics.SettingsEffects.High;
      Assert.IsTrue(vignette.active);
      Assert.IsTrue(dof.active);
    }

    [Test]
    public void EveryRuntimeWrite_IsPutBack()
    {
      var bloom = _profile.Add<Bloom>();
      bloom.highQualityFiltering.overrideState = false;
      bloom.highQualityFiltering.value = false;
      var vignette = _dropProfile.Add<Vignette>();
      _urpAsset.msaaSampleCount = 4;
      int qualityAntiAliasing = QualitySettings.antiAliasing;
      int mipmapLimit = QualitySettings.globalTextureMipmapLimit;
      Wire();

      _section.AntiAliasing = SettingsDataSectionGraphics.SettingsAntiAliasing.Off;
      _section.RenderScale = SettingsDataSectionGraphics.SettingsRenderScale.OneFifty;
      Assert.AreEqual(1.5f, _urpAsset.renderScale, 1e-4f);
      _section.Effects = SettingsDataSectionGraphics.SettingsEffects.Low;
      _section.TextureQuality = SettingsDataSectionGraphics.SettingsTextureQuality.Half;
      Assert.AreEqual(1, _urpAsset.msaaSampleCount);

      AuthoredGraphicsState.RestoreAll();

      Assert.AreEqual(4, _urpAsset.msaaSampleCount);
      Assert.AreEqual(1f, _urpAsset.renderScale, 1e-4f, "Render scale back to authored.");
      Assert.IsFalse(bloom.highQualityFiltering.overrideState);
      Assert.IsFalse(bloom.highQualityFiltering.value);
      Assert.IsTrue(vignette.active);
      Assert.AreEqual(qualityAntiAliasing, QualitySettings.antiAliasing);
      Assert.AreEqual(mipmapLimit, QualitySettings.globalTextureMipmapLimit);
      Assert.AreEqual(0, AuthoredGraphicsState.Count);
    }
  }
}
