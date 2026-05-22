using CupkekGames.Luna;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Settings.UI
{
  public abstract class SettingsMenuViewSection : MonoBehaviour
  {
    protected UIViewComponent _uiViewComponent;
    protected PanelRenderer PanelRenderer => _uiViewComponent.PanelRenderer;
    protected UIView UIView => _uiViewComponent.UIView;

    /// <summary>
    /// Shortcut to the parent UIViewComponent's mount root, used as the
    /// query root for section element lookups in <see cref="Initialize"/>.
    /// Non-null by the time Initialize fires because SettingsMenuView
    /// calls section.Initialize from its own OnUILoaded — after UIView
    /// is constructed.
    /// </summary>
    protected VisualElement Root => _uiViewComponent.ParentElement;
    protected SettingsDataSO _changedSettings;

    public void Initialize(SettingsDataSO changedSettings)
    {
      _changedSettings = changedSettings;

      _uiViewComponent = GetComponentInParent<UIViewComponent>();

      Initialize();

      ApplySettingsToUI();
    }

    protected abstract void Initialize();
    public abstract void ApplySettingsToUI();
    public virtual void OnTabSelected()
    {

      Debug.Log("OnTabSelected: " + gameObject.name);
    }
  }
}