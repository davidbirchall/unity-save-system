using UnityEngine;
namespace Birchall.SaveSystem.Examples {
  public class World : MonoBehaviour, ISaveable {
    public int seed;
    public string timeOfDay = "Noon";

    public string GetUniqueId() => "World";

    public object CaptureState() {
      return new WorldData {
        Seed = seed,
        TimeOfDay = timeOfDay
      };
    }

    public void RestoreState(object state) {
      var data = state as WorldData;
      if (data == null) return;
      seed = data.Seed;
      timeOfDay = data.TimeOfDay;
    }
  }
}