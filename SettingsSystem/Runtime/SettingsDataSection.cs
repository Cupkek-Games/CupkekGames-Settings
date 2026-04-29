
using UnityEngine;

namespace CupkekGames.Settings
{
  public abstract class SettingsDataSection : ScriptableObject
  {
    public override abstract bool Equals(object obj);
    public override abstract int GetHashCode();
    public abstract void CopyValuesFrom(SettingsDataSection copy);
    public abstract void SaveToPlayerPrefs(string key);
    public abstract void LoadFromPlayerPrefs(string key);
    public abstract void ApplySettings(SettingsDataSection settingsData);

  }
}