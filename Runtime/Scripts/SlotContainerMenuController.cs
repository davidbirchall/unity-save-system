using UnityEngine;
using Birchall.SaveSystem.UI;

namespace Birchall.SaveSystem.UI {

  /// <summary>
  /// Controls all SaveSlotUI children in a container, allowing switching
  /// between Save and Load menu modes.
  /// </summary>
  public class SlotContainerMenuController : MonoBehaviour {
    [Header("Menu Mode")]
    [SerializeField] private SaveSlotUI.SaveMenuMode currentMode = SaveSlotUI.SaveMenuMode.Save;

    private void Start() {
      // Ensure all child slots are updated when the scene starts
      ApplyMenuMode();
    }

    /// <summary>
    /// Apply the current menu mode to all SaveSlotUI children.
    /// </summary>
    public void ApplyMenuMode() {
      var slots = GetComponentsInChildren<SaveSlotUI>();
      foreach (var slot in slots) {
        slot.SetMenuMode(currentMode);
      }
    }

    /// <summary>
    /// Switch menu mode at runtime (Save or Load).
    /// </summary>
    public void SetMode(SaveSlotUI.SaveMenuMode mode) {
      currentMode = mode;
      ApplyMenuMode();
    }
  }
}
