using UnityEngine;
namespace Birchall.SaveSystem.Examples {
  public class Player : MonoBehaviour, ISaveable {
    public int health = 100;
    public string GetUniqueId() => "Player";

    public object CaptureState() {
      return new PlayerData {
        Position = transform.position,
        Health = health
      };
    }

    public void RestoreState(object state) {
      var data = state as PlayerData;
      if (data == null) return;
      transform.position = data.Position;
      health = data.Health;
    }
  }
  [System.Serializable]
  public class PlayerData {
    public int Health;
    public Vector3 Position;
  }
}