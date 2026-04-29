using CupkekGames.Input;
using UnityEngine.UIElements;
using System.Collections.Generic;
using CupkekGames.Luna;

#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Settings.UI
{
    public class SettingsMenuViewRebindElement
    {
        private SettingsMenuViewRebind _settings;
        private InputPrompt _inputPrompt;
        public InputPrompt InputPrompt => _inputPrompt;
        private Button _reset;
        public SettingsMenuViewRebindElement(SettingsMenuViewRebind settings, VisualElement parent, string inputName)
        {
            _settings = settings;

            VisualElement container = new VisualElement();
            container.AddToClassList("settings_line");
            parent.Add(container);

            VisualElement left = new VisualElement();
            left.AddToClassList("flex-row");
            left.AddToClassList("grow-1");
            container.Add(left);

            VisualElement right = new VisualElement();
            right.AddToClassList("flex-row");
            right.AddToClassList("items-center");
            right.AddToClassList("justify-end");
            right.AddToClassList("grow-1");
            container.Add(right);

            _inputPrompt = new InputPrompt();
            _inputPrompt.InputActionName = inputName;
            _inputPrompt.ButtonIf = new List<InputIconControlScheme>()
            {
                InputIconControlScheme.KeyboardMouse,
                InputIconControlScheme.PlayStation4,
                InputIconControlScheme.PlayStation5,
                InputIconControlScheme.Xbox
            };
            _inputPrompt.HideIf.Clear();
            right.Add(_inputPrompt);

            _reset = new Button();
            _reset.text = "Reset";
            _reset.AddToClassList("btn");
            _reset.AddToClassList("btn-sm");
            _reset.AddToClassList("ml-32");
            right.Add(_reset);

            Label label = new Label("Input Action Name");

#if UNITY_INPUT
            label.text = _inputPrompt.Action.name;
#endif

            left.Add(label);

#if UNITY_INPUT
            UpdateBindingName();
            _reset.clicked += Reset;
            _inputPrompt.clicked += OnInputPromptClick;
            _inputPrompt.OnUpdate += UpdateBindingName;
#endif
        }

#if UNITY_INPUT
        private void UpdateBindingName()
        {
            _inputPrompt.LabelText = _inputPrompt.Action.bindings[_inputPrompt.BindingIndex].ToDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions);
        }

        private void OnInputPromptClick()
        {
            _settings.StartInteractiveRebind(this);
        }

        private void Reset()
        {
            _settings.ResetToDefault(this);
        }
#endif
    }
}