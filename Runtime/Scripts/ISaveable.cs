namespace Birchall.SaveSystem {
  public interface ISaveable {
    string GetUniqueId();
    object CaptureState();
    void RestoreState(object state);
  }
}