using UnityEngine;

namespace Birchall.SaveSystem.Examples {
  public class SaveUI : MonoBehaviour {
    public SaveSystem saveSystem;

    private void Update() {
      if (Input.GetKeyDown(KeyCode.F5)) saveSystem.SaveToSlot(0);
      if (Input.GetKeyDown(KeyCode.F9)) saveSystem.LoadFromSlot(0);
      if (Input.GetKeyDown(KeyCode.F10)) saveSystem.AutoSave();
      if (Input.GetKeyDown(KeyCode.F11)) saveSystem.LoadAutoSave();
    }
  }
}