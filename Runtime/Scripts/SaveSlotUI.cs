using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Birchall.SaveSystem;

namespace Birchall.SaveSystem.UI {

  public class SaveSlotUI : MonoBehaviour {
    [Header("Slot Info")]
    [Tooltip("-1 = Autosave, 0+ = Manual slots")]
    [SerializeField] private int slotIndex = 0;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button deleteButton;

    public enum SaveMenuMode { Save, Load }
    [SerializeField] private SaveMenuMode menuMode = SaveMenuMode.Save;

    private SaveSystem saveSystem;

    private void Start() {
      saveSystem = FindFirstObjectByType<SaveSystem>();

      saveButton.onClick.AddListener(OnSaveClicked);
      loadButton.onClick.AddListener(OnLoadClicked);
      deleteButton.onClick.AddListener(OnDeleteClicked);

      if (saveSystem != null) {
        saveSystem.OnBeforeSave += Refresh;
        saveSystem.OnAfterLoad += Refresh;
      }

      Refresh();
    }

    private void OnDestroy() {
      if (saveSystem != null) {
        saveSystem.OnBeforeSave -= Refresh;
        saveSystem.OnAfterLoad -= Refresh;
      }
    }

    /// <summary>
    /// Runtime menu mode switching
    /// </summary>
    public void SetMenuMode(SaveMenuMode mode) {
      menuMode = mode;
      Refresh();
    }

    private void OnSaveClicked() {
      if (saveSystem == null) return;

      if (slotIndex == -1)
        saveSystem.AutoSave();
      else
        saveSystem.SaveToSlot(slotIndex);

      Refresh();
    }

    private void OnLoadClicked() {
      if (saveSystem == null) return;

      if (slotIndex == -1)
        saveSystem.LoadAutoSave();
      else
        saveSystem.LoadFromSlot(slotIndex);

      Refresh();
    }

    private void OnDeleteClicked() {
      if (slotIndex == -1)
        SaveManager.DeleteAutoSave();
      else
        SaveManager.Delete(slotIndex);

      Refresh();
    }

    private void Refresh() {
      var meta = slotIndex == -1
          ? SaveManager.GetAutoSaveMetadata()
          : SaveManager.GetMetadata(slotIndex);

      string slotLabel = slotIndex == -1 ? "Autosave" : $"Slot {slotIndex}";
      label.text = meta != null
          ? $"{slotLabel} - {meta.SceneName} - {meta.Timestamp}"
          : $"{slotLabel} <Empty>";

      // Button visibility logic
      switch (menuMode) {
        case SaveMenuMode.Save:
          saveButton.gameObject.SetActive(true);
          loadButton.gameObject.SetActive(false);
          deleteButton.gameObject.SetActive(true); // always show Delete in Save menu
          break;

        case SaveMenuMode.Load:
          saveButton.gameObject.SetActive(false);
          loadButton.gameObject.SetActive(meta != null);
          deleteButton.gameObject.SetActive(meta != null);
          break;
      }
    }
  }
}
