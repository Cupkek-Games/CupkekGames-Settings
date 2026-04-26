using System;
using UnityEngine;
using UnityEngine.Audio;

namespace CupkekGames.Systems
{
  [CreateAssetMenu(fileName = "SectionAudio", menuName = "CupkekGames/Settings/Section/Audio")]
  public class SettingsDataSectionAudio : SettingsDataSection
  {
    [SerializeField] public AudioMixer AudioMixer;
    [SerializeField] private float _masterVolume;
    public float MasterVolume
    {
      get
      {
        return _masterVolume;
      }
      set
      {
        _masterVolume = value;
        SetGroupVolume("MasterVolume", _masterVolume);
      }
    }
    [SerializeField] private float _musicVolume;
    public float MusicVolume
    {
      get
      {
        return _musicVolume;
      }
      set
      {
        _musicVolume = value;
        SetGroupVolume("MusicVolume", _musicVolume);
      }
    }
    [SerializeField] private float _ambientVolume;
    public float AmbientVolume
    {
      get
      {
        return _ambientVolume;
      }
      set
      {
        _ambientVolume = value;
        SetGroupVolume("AmbientVolume", _ambientVolume);
      }
    }
    [SerializeField] private float _sfxVolume;
    public float SFXVolume
    {
      get
      {
        return _sfxVolume;
      }
      set
      {
        _sfxVolume = value;
        SetGroupVolume("SFXVolume", _sfxVolume);
      }
    }

    public override bool Equals(object obj)
    {
      // Check for null and compare types
      if (obj == null || GetType() != obj.GetType())
        return false;

      // Cast and compare properties
      SettingsDataSectionAudio b = (SettingsDataSectionAudio)obj;

      return _masterVolume == b._masterVolume
        && _musicVolume == b._musicVolume
        && _ambientVolume == b._ambientVolume
        && _sfxVolume == b._sfxVolume;
    }

    public override int GetHashCode()
    {
      return HashCode.Combine(_masterVolume, _musicVolume, _ambientVolume, _sfxVolume);
    }

    public override void SaveToPlayerPrefs(string key)
    {
      PlayerPrefs.SetFloat($"{key}_MasterVolume", _masterVolume);
      PlayerPrefs.SetFloat($"{key}_MusicVolume", _musicVolume);
      PlayerPrefs.SetFloat($"{key}_AmbientVolume", _ambientVolume);
      PlayerPrefs.SetFloat($"{key}_SFXVolume", _sfxVolume);
      PlayerPrefs.Save();
    }

    public override void LoadFromPlayerPrefs(string key)
    {
      if (PlayerPrefs.HasKey($"{key}_MasterVolume"))
      {
        _masterVolume = PlayerPrefs.GetFloat($"{key}_MasterVolume");
      }
      if (PlayerPrefs.HasKey($"{key}_MusicVolume"))
      {
        _musicVolume = PlayerPrefs.GetFloat($"{key}_MusicVolume");
      }
      if (PlayerPrefs.HasKey($"{key}_AmbientVolume"))
      {
        _ambientVolume = PlayerPrefs.GetFloat($"{key}_AmbientVolume");
      }
      if (PlayerPrefs.HasKey($"{key}_SFXVolume"))
      {
        _sfxVolume = PlayerPrefs.GetFloat($"{key}_SFXVolume");
      }
    }

    public override void CopyValuesFrom(SettingsDataSection section)
    {
      if (section is SettingsDataSectionAudio copy)
      {
        AudioMixer = copy.AudioMixer;
        _masterVolume = copy._masterVolume;
        _musicVolume = copy._musicVolume;
        _ambientVolume = copy._ambientVolume;
        _sfxVolume = copy._sfxVolume;
      }
    }

    public override void ApplySettings(SettingsDataSection settingsData)
    {
      if (settingsData is SettingsDataSectionAudio copy)
      {
        MasterVolume = copy._masterVolume;
        MusicVolume = copy._musicVolume;
        AmbientVolume = copy._ambientVolume;
        SFXVolume = copy._sfxVolume;
      }
    }

    private void SetGroupVolume(string parameterName, float normalizedVolume)
    {
      bool volumeSet = AudioMixer.SetFloat(parameterName, NormalizedToMixerValue(normalizedVolume));
      if (!volumeSet)
        Debug.LogError("The AudioMixer parameter was not found: " + parameterName);
    }

    private float GetGroupVolume(string parameterName)
    {
      if (AudioMixer.GetFloat(parameterName, out float rawVolume))
      {
        return MixerValueToNormalized(rawVolume);
      }
      else
      {
        Debug.LogError("The AudioMixer parameter was not found");
        return 0f;
      }
    }

    // Both MixerValueNormalized and NormalizedToMixerValue functions are used for easier transformations
    /// when using UI sliders normalized format
    private float MixerValueToNormalized(float mixerValue)
    {
      // We're assuming the range [-80dB to 0dB] becomes [0 to 1]
      return 1f + (mixerValue / 80f);
    }
    private float NormalizedToMixerValue(float normalizedValue)
    {
      // We're assuming the range [0 to 1] becomes [-80dB to 20dB]
      // This doesn't allow values over 0dB
      normalizedValue = normalizedValue < 0.0001f ? 0.0001f : normalizedValue;
      normalizedValue = normalizedValue > 1f ? 1f : normalizedValue;

      return Mathf.Log10(normalizedValue) * 20;
    }
  }
}