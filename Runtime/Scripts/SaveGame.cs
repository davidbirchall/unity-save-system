using System;
using System.Collections.Generic;

namespace Birchall.SaveSystem {
  [Serializable]
  public class SaveState {
    public string Key;
    public string JsonData;
  }

  [Serializable]
  public class SaveGame {
    public SaveMetadata Metadata = new SaveMetadata();
    public List<SaveState> States = new List<SaveState>();
  }
}
