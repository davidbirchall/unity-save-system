using UnityEngine;

namespace Birchall.SaveSystem.Examples {
  public class Inventory : MonoBehaviour, ISaveable {
    public string[] items = new string[0];
    public string GetUniqueId() => "Inventory";

    public object CaptureState() {
      return new InventoryData {
        Items = items
      };
    }

    public void RestoreState(object state) {
      var data = state as InventoryData;
      if (data == null) return;
      items = data.Items;
    }
  }
}