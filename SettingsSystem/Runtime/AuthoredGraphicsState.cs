using System;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CupkekGames.Settings
{
  /// <summary>
  /// The authored values of the shared state the graphics section writes at runtime:
  /// URP assets, renderer features, volume profiles and QualitySettings. Each value is
  /// remembered the first time the section touches it. That memory is what Effects
  /// High returns to (the authored state, not "everything on"), and in the editor it
  /// is what leaving play mode puts back: those are project files, and a runtime
  /// write left in one is saved into the project by the next asset save.
  /// </summary>
  public static class AuthoredGraphicsState
  {
    private sealed class Entry
    {
      public object Value;
      public Action<object> Restore;
    }

    // Lives for the whole play session on purpose and empties itself on restore: an
    // automatic statics reset on exiting play could run before the restore.
    [NoAutoStaticsCleanup]
    private static readonly Dictionary<(object Owner, string Property), Entry> Entries = new();

    /// <summary>Values remembered and not yet restored.</summary>
    public static int Count => Entries.Count;

    /// <summary>
    /// Remembers <paramref name="owner"/>'s <paramref name="property"/> as
    /// <paramref name="current"/> the first time it is called for that pair, and
    /// returns the remembered (authored) value. <paramref name="restore"/> writes a
    /// value back; it runs on <see cref="RestoreAll"/>.
    /// </summary>
    public static T Remember<T>(object owner, string property, T current, Action<T> restore)
    {
      if (owner == null) throw new ArgumentNullException(nameof(owner));
      if (restore == null) throw new ArgumentNullException(nameof(restore));

      var key = (owner, property);
      if (!Entries.TryGetValue(key, out Entry entry))
      {
        entry = new Entry { Value = current, Restore = value => restore((T)value) };
        Entries.Add(key, entry);
      }

      return (T)entry.Value;
    }

    /// <summary>Writes every remembered value back and forgets them all.</summary>
    public static void RestoreAll()
    {
      foreach (KeyValuePair<(object Owner, string Property), Entry> pair in Entries)
      {
        // A destroyed Unity object compares equal to null through its own operator.
        if (pair.Key.Owner is UnityEngine.Object unityObject && unityObject == null)
        {
          continue;
        }

        pair.Value.Restore(pair.Value.Value);
      }

      Entries.Clear();
    }

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    private static void HookPlayMode()
    {
      EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
      EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    // Twice: leaving play reloads the edit scene while isPlaying is still true, and
    // anything that applies settings on that reload writes again after the first pass.
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
      if (state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.EnteredEditMode)
      {
        RestoreAll();
      }
    }
#endif
  }
}
