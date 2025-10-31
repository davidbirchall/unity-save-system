using UnityEngine;

namespace Birchall.SaveSystem.Examples {
  public class Settings : MonoBehaviour, ISaveable {
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    public string GetUniqueId() => "Settings";

    public object CaptureState() {
      return new SettingsData {
        MasterVolume = masterVolume
      };
    }

    public void RestoreState(object state) {
      var data = state as SettingsData;
      if (data == null) return;
      masterVolume = data.MasterVolume;
    }
  }
}