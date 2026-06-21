using CupkekGames.EditorInspector;
using CupkekGames.Input;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna;
using CupkekGames.Luna.Navigation;

#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Settings.UI
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
    }

    protected override void OnUILoaded(VisualElement root)
    {
      base.OnUILoaded(root);

      // Left Panel
      _tabView = root.Q<Luna.TabView>();
      // Content-panel display toggle is built into TabView now: with no
      // channel destinations it runs local mode — each <ui:VisualElement
      // class="tab-panel" name="X"> shows when tab button "X" activates.
      // No wiring needed (the former TabPanelSwitcher.Bind is absorbed).
      _buttonReturn = root.Q<Button>("ReturnButton");

      // Bottom Panel
      _buttonRevert = root.Q<Button>("Revert");
      _buttonDefault = root.Q<Button>("Default");

      foreach (var section in _sections)
      {
        section.Initialize(_changedSettings);
      }

      // Return on escape input. UIView is now constructed.
      UIView.AddAction(new UIViewActionEscape(Return));

      // Run the enable-side wire-up if we were already enabled by the
      // time the panel delivered its tree. Start() may also need to
      // fire its initial focus call — handled in Start with a null guard.
      if (enabled) OnEnable();
    }

    private void OnEnable()
    {
      if (_buttonReturn == null) return; // panel hasn't reloaded yet

      _buttonReturn.clicked += Return;
      _buttonRevert.clicked += OnButtonRevertClicked;
      _buttonDefault.clicked += OnButtonDefaultClicked;

      _tabView.OnTabChanged += OnTabChanged;

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
      if (_buttonReturn == null) return;

      _buttonReturn.clicked -= Return;
      _buttonRevert.clicked -= OnButtonRevertClicked;
      _buttonDefault.clicked -= OnButtonDefaultClicked;

      _tabView.OnTabChanged -= OnTabChanged;

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
      if (_tabView == null) return; // panel hasn't reloaded yet — focus deferred to OnUILoaded path
      _tabView.GetTabButton(_tabView.ActiveTab)?.Focus();
    }

    private void OnTabChanged(string tabId)
    {
      int index = _tabView.GetTabIndex(tabId);
      if (index < 0 || index >= _sections.Length) return;
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
      LunaNavigation.PopBackStack();
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