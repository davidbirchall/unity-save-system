using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Birchall.SaveSystem {

  public class SaveSystem : MonoBehaviour {
    private List<ISaveable> saveables = new List<ISaveable>();

    // --- Events for UI or other systems ---
    public event Action OnBeforeSave;
    public event Action OnAfterLoad;

    private void Awake() {
      // Find all saveables in the scene (active + inactive)
      saveables = UnityEngine.Object
          .FindObjectsByType<MonoBehaviour>(
              FindObjectsInactive.Include,   // include inactive objects
              FindObjectsSortMode.None      // no sorting for performance
          )
          .OfType<ISaveable>()
          .ToList();
    }

    // --- Dynamic registration (for runtime-spawned objects) ---
    public void RegisterSaveable(ISaveable saveable) => saveables.Add(saveable);
    public void UnregisterSaveable(ISaveable saveable) => saveables.Remove(saveable);

    // --- Manual slots ---
    public void SaveToSlot(int slot) {
      OnBeforeSave?.Invoke();
      var save = SaveSerializer.Capture(saveables);
      SaveManager.Save(save, slot);
    }

    public void LoadFromSlot(int slot) {
      var save = SaveManager.Load(slot);
      SaveSerializer.Restore(saveables, save);
      OnAfterLoad?.Invoke();
    }

    // --- Autosave ---
    public void AutoSave() {
      OnBeforeSave?.Invoke();
      var save = SaveSerializer.Capture(saveables);
      SaveManager.AutoSave(save);
    }

    public void LoadAutoSave() {
      var save = SaveManager.LoadAutoSave();
      SaveSerializer.Restore(saveables, save);
      OnAfterLoad?.Invoke();
    }
  }
}
