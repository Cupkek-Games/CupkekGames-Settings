using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Core;
using CupkekGames.Luna;

#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Systems.UI
{
  public class SettingsMenuView : UIViewComponent
  {
    private SettingsSystem _settingsSystem;
    private SettingsDataSO _changedSettings;

    // Left Panel
    private Luna.TabView _tabView;
    private Button _buttonReturn;

    // Bottom Panel
    private Button _buttonRevert;
    private Button _buttonDefault;

    // Pages
    [MultiLineHeader("Ensure that the order of elements matches the order in TabView!")] [SerializeField]
    private SettingsMenuViewSection[] _sections;

    // Input
    [SerializeField] private string _inputNameRevert = "UI/InteractHold";
    [SerializeField] private string _inputNameDefault = "UI/ActionHold";
#if UNITY_INPUT
    private InputAction _inputRevert;
    private InputAction _inputDefault;
#endif
    protected override void Awake()
    {
      base.Awake();

      _settingsSystem = SettingsSystem.Instance;

      _changedSettings = ScriptableObject.CreateInstance<SettingsDataSO>();
      _changedSettings.CopyValuesFrom(_settingsSystem.CurrentSettings);

      // Left Panel
      _tabView = UIDocument.rootVisualElement.Q<Luna.TabView>();
      // _tabView.Initialize(0, ParentElement);
      _buttonReturn = UIDocument.rootVisualElement.Q<Button>("ReturnButton");

      // Bottom Panel
      _buttonRevert = UIDocument.rootVisualElement.Q<Button>("Revert");
      _buttonDefault = UIDocument.rootVisualElement.Q<Button>("Default");

      foreach (var section in _sections)
      {
        section.Initialize(_changedSettings);
      }

      // Return on escape input
      UIView.AddAction(new UIViewActionEscape(Return));
    }

    private void OnEnable()
    {
      _buttonReturn.clicked += Return;
      _buttonRevert.clicked += OnButtonRevertClicked;
      _buttonDefault.clicked += OnButtonDefaultClicked;

      _tabView.OnTabSelected += OnTabSelected;

#if UNITY_INPUT
      PlayerInput playerInput = null;

      if (InputDeviceManager.PlayerInputs.Count > 0)
      {
        playerInput = InputDeviceManager.PlayerInputs[0];
      }

      if (playerInput != null)
      {
        _inputRevert = playerInput.actions[_inputNameRevert];
        if (_inputRevert != null)
        {
          _inputRevert.performed += OnInputRevert;
        }

        _inputDefault = playerInput.actions[_inputNameDefault];
        if (_inputDefault != null)
        {
          _inputDefault.performed += OnInputDefault;
        }
      }
#endif
    }

    private void OnDisable()
    {
      _buttonReturn.clicked -= Return;
      _buttonRevert.clicked -= OnButtonRevertClicked;
      _buttonDefault.clicked -= OnButtonDefaultClicked;

      _tabView.OnTabSelected -= OnTabSelected;

#if UNITY_INPUT
      if (_inputRevert != null)
      {
        _inputRevert.performed -= OnInputRevert;
      }

      if (_inputDefault != null)
      {
        _inputDefault.performed -= OnInputDefault;
      }
#endif
    }

    private void Start()
    {
      _tabView.GetTabHeader(_tabView.ActiveTab).Focus();
    }

    private void OnTabSelected(int index)
    {
      _sections[index].OnTabSelected();
    }

    public void ApplySettingsToUI()
    {
      foreach (var section in _sections)
      {
        section.ApplySettingsToUI();
      }
    }

    private void Return()
    {
      OnAcceptModal();

      // if (_changedSettings.Equals(_settingsSystem.CurrentSettings))
      // {
      //   ExitSettings();
      // }
      // else
      // {
      //   _choicePopupController.Fade.FadeIn();
      // }
    }

#if UNITY_INPUT
    private void OnInputDefault(InputAction.CallbackContext context)
    {
      // Defer to next frame so RemoveAllBindingOverrides doesn't cancel
      // other active hold interactions mid-callback.
      ParentElement.schedule.Execute(OnButtonDefaultClicked);
    }

    private void OnInputRevert(InputAction.CallbackContext context)
    {
      // Defer to next frame so RemoveAllBindingOverrides doesn't cancel
      // other active hold interactions mid-callback.
      ParentElement.schedule.Execute(OnButtonRevertClicked);
    }
#endif

    private void OnButtonRevertClicked()
    {
      RevertSettings();
      ApplySettingsToUI();
    }

    private void OnButtonDefaultClicked()
    {
      // Write default settings to changed settings
      _changedSettings.CopyValuesFrom(_settingsSystem.DefaultSettings);
      // Do not write default settings to current settings
      ApplySettingsToUI();
    }

    // Save & Revert

    public void SaveSettings()
    {
      // Write changed settings to current settings
      _settingsSystem.CurrentSettings.CopyValuesFrom(_changedSettings);
      // Any change is applied immediately, so we don't need to apply here
      // Save current settings
      _settingsSystem.SaveAndApplySettings();
    }

    public void RevertSettings()
    {
      // Write current settings to changed settings
      _changedSettings.CopyValuesFrom(_settingsSystem.CurrentSettings);
    }

    public void ExitSettings()
    {
      FadeOutThenDestroy();
    }

    private void OnAcceptModal()
    {
      SaveSettings();
      ExitSettings();
    }

    private void OnDeclineModal()
    {
      RevertSettings();
      ExitSettings();
      _settingsSystem.ApplySettings(_settingsSystem.CurrentSettings);
    }
  }
}