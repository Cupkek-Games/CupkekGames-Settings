using UnityEngine.UIElements;
using UnityEngine;

namespace CupkekGames.Settings.UI
{
  public class SettingsMenuViewAudio : SettingsMenuViewSection
  {
    // Audio Settings
    private SliderInt _sliderMasterVolume;
    private SliderInt _sliderMusicVolume;
    private SliderInt _sliderAmbientVolume;
    private SliderInt _sliderSFXVolume;

    protected override void Initialize()
    {
      // Audio Settings
      VisualElement masterContainer = UIDocument.rootVisualElement.Q<VisualElement>("AudioMaster");
      _sliderMasterVolume = masterContainer.Q<SliderInt>();

      VisualElement musicContainer = UIDocument.rootVisualElement.Q<VisualElement>("AudioMusic");
      _sliderMusicVolume = musicContainer.Q<SliderInt>();

      VisualElement ambientContainer = UIDocument.rootVisualElement.Q<VisualElement>("AudioAmbient");
      _sliderAmbientVolume = ambientContainer.Q<SliderInt>();

      VisualElement sfxContainer = UIDocument.rootVisualElement.Q<VisualElement>("AudioSFX");
      _sliderSFXVolume = sfxContainer.Q<SliderInt>();

      _sliderMasterVolume.RegisterValueChangedCallback(OnMasterChanged);
      _sliderMusicVolume.RegisterValueChangedCallback(OnMusicChanged);
      _sliderAmbientVolume.RegisterValueChangedCallback(OnAmbientChanged);
      _sliderSFXVolume.RegisterValueChangedCallback(OnSFXChanged);
    }

    public void OnDisable()
    {
      _sliderMasterVolume.UnregisterValueChangedCallback(OnMasterChanged);
      _sliderMusicVolume.UnregisterValueChangedCallback(OnMusicChanged);
      _sliderAmbientVolume.UnregisterValueChangedCallback(OnAmbientChanged);
      _sliderSFXVolume.UnregisterValueChangedCallback(OnSFXChanged);
    }

    public override void OnTabSelected()
    {
      _sliderMasterVolume.Focus();
    }

    // Apply Settings to UI

    public override void ApplySettingsToUI()
    {
      SettingsDataSectionAudio audio = (SettingsDataSectionAudio)_changedSettings.GetValue("audio");

      // Video Settings
      int masterVolume = (int)(audio.MasterVolume * 100f);
      _sliderMasterVolume.value = masterVolume;

      int musicVolume = (int)(audio.MusicVolume * 100f);
      _sliderMusicVolume.value = musicVolume;

      int ambientVolume = (int)(audio.AmbientVolume * 100f);
      _sliderAmbientVolume.value = ambientVolume;

      int sfxVolume = (int)(audio.SFXVolume * 100f);
      _sliderSFXVolume.value = sfxVolume;
    }

    private void OnMasterChanged(ChangeEvent<int> evt)
    {
      SettingsDataSectionAudio audio = (SettingsDataSectionAudio)_changedSettings.GetValue("audio");

      audio.MasterVolume = evt.newValue / 100f;
    }

    private void OnMusicChanged(ChangeEvent<int> evt)
    {
      SettingsDataSectionAudio audio = (SettingsDataSectionAudio)_changedSettings.GetValue("audio");

      audio.MusicVolume = evt.newValue / 100f;
    }

    private void OnAmbientChanged(ChangeEvent<int> evt)
    {
      SettingsDataSectionAudio audio = (SettingsDataSectionAudio)_changedSettings.GetValue("audio");

      audio.AmbientVolume = evt.newValue / 100f;
    }

    private void OnSFXChanged(ChangeEvent<int> evt)
    {
      SettingsDataSectionAudio audio = (SettingsDataSectionAudio)_changedSettings.GetValue("audio");

      audio.SFXVolume = evt.newValue / 100f;
    }
  }
}