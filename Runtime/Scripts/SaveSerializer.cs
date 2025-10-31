using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Birchall.SaveSystem {
  public static class SaveSerializer {
    public static SaveGame Capture(IEnumerable<ISaveable> saveables) {
      var save = new SaveGame();
      foreach (var s in saveables) {
        var state = s.CaptureState();
        string json = JsonUtility.ToJson(state);
        Debug.Log($"[SaveSerializer] Capturing {s.GetUniqueId()} -> {json}");

        save.States.Add(new SaveState {
          Key = s.GetUniqueId(),
          JsonData = json
        });
      }
      return save;
    }

    public static void Restore(IEnumerable<ISaveable> saveables, SaveGame save) {
      if (save == null) return;

      foreach (var s in saveables) {
        var found = save.States.FirstOrDefault(st => st.Key == s.GetUniqueId());
        if (found != null) {
          var expectedType = s.CaptureState().GetType();
          var state = JsonUtility.FromJson(found.JsonData, expectedType);
          Debug.Log($"[SaveSerializer] Restoring {s.GetUniqueId()} <- {found.JsonData}");
          s.RestoreState(state);
        }
      }
    }
  }
}
