using UnityEngine;
using CupkekGames.KeyValueDatabase;

namespace CupkekGames.Settings
{
  [CreateAssetMenu(fileName = "Settings", menuName = "CupkekGames/Settings/Settings")]
  public class SettingsDataSO : KeyValueDatabaseSO<string, SettingsDataSection>
  {
    public void CopyValuesFrom(SettingsDataSO copy)
    {
      foreach (var key in copy.Keys)
      {
        SettingsDataSection section = copy.GetValue(key);

        SettingsDataSection clone;
        if (ContainsKey(key))
        {
          clone = GetValue(key);
        }
        else
        {
          clone = (SettingsDataSection)ScriptableObject.CreateInstance(section.GetType());
          TryAdd(key, clone);
        }

        clone.CopyValuesFrom(section);
      }
    }

    public override bool Equals(object obj)
    {
      if (obj == null || GetType() != obj.GetType())
        return false;

      SettingsDataSO other = (SettingsDataSO)obj;

      if (Count != other.Count)
        return false;

      // Compare each key-value pair
      foreach (var key in Keys)
      {
        if (other.TryGetValue(key, out var otherValue))
        {
          if (!GetValue(key).Equals(otherValue))
          {
            return false;
          }
        }
      }

      return true;
    }

    public override int GetHashCode()
    {
      int hash = 17;
      foreach (var key in Keys)
      {
        hash = hash * 31 + key.GetHashCode();
        var value = GetValue(key);
        hash = hash * 31 + (value != null ? value.GetHashCode() : 0);
      }
      return hash;
    }

    public void SaveToPlayerPrefs()
    {
      foreach (var key in Keys)
      {
        GetValue(key).SaveToPlayerPrefs($"settings_{key}");
      }
    }

    public void LoadFromPlayerPrefs()
    {
      foreach (var key in Keys)
      {
        GetValue(key).LoadFromPlayerPrefs($"settings_{key}");
      }
    }
  }
}
