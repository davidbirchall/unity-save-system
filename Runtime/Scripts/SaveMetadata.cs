using System;

namespace Birchall.SaveSystem {
  [Serializable]
  public class SaveMetadata {
    public string SaveName;       // e.g., "Slot 0" or "Autosave"
    public string SceneName;      // scene at save time
    public string Timestamp;      // yyyy-MM-dd HH:mm
    public float PlaytimeHours;   // tracked externally (optional)
    public int SaveVersion = 1;   // bump if your data model changes
  }
}
