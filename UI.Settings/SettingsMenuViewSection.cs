using CupkekGames.Luna;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Settings.UI
{
  public abstract class SettingsMenuViewSection : MonoBehaviour
  {
    protected UIViewComponent _uiViewComponent;
    protected UIDocument UIDocument => _uiViewComponent.UIDocument;
    protected UIView UIView => _uiViewComponent.UIView;
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