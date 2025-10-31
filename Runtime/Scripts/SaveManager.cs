using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Birchall.SaveSystem {
  public static class SaveManager {
    public const int ManualSlotCount = 10;

    private static string SaveDir => Path.Combine(Application.persistentDataPath, "Saves");
    private static string AutoSaveFile => Path.Combine(SaveDir, "autosave.json");

    // NOTE: replace with a securely generated key in a real product.
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes for AES-128
    private static readonly byte[] IV = Encoding.UTF8.GetBytes("6543210987654321"); // 16 bytes for AES-128

    // --- Public API ---
    public static void Save(SaveGame save, int slot) {
      Directory.CreateDirectory(SaveDir);
      save.Metadata.SaveName = $"Slot {slot}";
      save.Metadata.SceneName = SceneManager.GetActiveScene().name;
      save.Metadata.Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
      var path = GetSlotPath(slot);
      WriteEncrypted(save, path);
    }

    public static SaveGame Load(int slot) {
      var path = GetSlotPath(slot);
      return ReadEncrypted(path);
    }

    public static void AutoSave(SaveGame save) {
      Directory.CreateDirectory(SaveDir);
      save.Metadata.SaveName = "Autosave";
      save.Metadata.SceneName = SceneManager.GetActiveScene().name;
      save.Metadata.Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
      WriteEncrypted(save, AutoSaveFile);
    }

    public static SaveGame LoadAutoSave() {
      return ReadEncrypted(AutoSaveFile);
    }

    // --- Delete helpers (restored) ---
    public static void Delete(int slot) {
      var path = GetSlotPath(slot);
      if (File.Exists(path)) File.Delete(path);
    }

    public static void DeleteAutoSave() {
      if (File.Exists(AutoSaveFile)) File.Delete(AutoSaveFile);
    }

    // --- Metadata only helpers (restored) ---
    public static SaveMetadata GetMetadata(int slot) {
      var path = GetSlotPath(slot);
      if (!File.Exists(path)) return null;
      var save = ReadEncrypted(path);
      return save?.Metadata;
    }

    public static SaveMetadata GetAutoSaveMetadata() {
      if (!File.Exists(AutoSaveFile)) return null;
      var save = ReadEncrypted(AutoSaveFile);
      return save?.Metadata;
    }

    // --- Core file ops ---
    private static string GetSlotPath(int slot) {
      Directory.CreateDirectory(SaveDir);
      return Path.Combine(SaveDir, $"save_{slot}.json");
    }

    private static void WriteEncrypted(SaveGame save, string path) {
      try {
        string json = JsonUtility.ToJson(save, true);
        byte[] compressed = Compress(Encoding.UTF8.GetBytes(json));
        byte[] encrypted = Encrypt(compressed);

        File.WriteAllBytes(path, encrypted);
        Debug.Log($"[SaveManager] Saved -> {path}");
      }
      catch (Exception e) {
        Debug.LogError($"[SaveManager] Failed to save: {e}");
      }
    }

    /// <summary>
    /// Attempts to read the file as an encrypted+compressed save. If decryption fails,
    /// falls back to treating the file as plain JSON (for backward compatibility).
    /// </summary>
    private static SaveGame ReadEncrypted(string path) {
      try {
        if (!File.Exists(path)) {
          Debug.LogWarning($"[SaveManager] No save file at {path}");
          return null;
        }

        byte[] fileBytes = File.ReadAllBytes(path);

        // First: try decrypt+decompress
        try {
          byte[] decrypted = Decrypt(fileBytes);
          byte[] decompressed = Decompress(decrypted);
          string json = Encoding.UTF8.GetString(decompressed);
          var save = JsonUtility.FromJson<SaveGame>(json);
          Debug.Log($"[SaveManager] Loaded (encrypted) <- {path}");
          return save;
        }
        catch (Exception decryptEx) {
          // Fallback: try plain-text JSON file (older saves)
          try {
            // If file is small plain-text JSON, parsing directly should work.
            string text = File.ReadAllText(path);
            if (!string.IsNullOrWhiteSpace(text) && (text.TrimStart().StartsWith("{") || text.TrimStart().StartsWith("["))) {
              var save = JsonUtility.FromJson<SaveGame>(text);
              Debug.Log($"[SaveManager] Loaded (plaintext fallback) <- {path}");
              return save;
            }

            Debug.LogWarning($"[SaveManager] Decrypt failed and file doesn't look like JSON: {decryptEx.Message}");
            return null;
          }
          catch (Exception plainEx) {
            Debug.LogError($"[SaveManager] Failed to load (decrypt error: {decryptEx.Message}; plain fallback error: {plainEx.Message})");
            return null;
          }
        }
      }
      catch (Exception e) {
        Debug.LogError($"[SaveManager] Failed to load: {e}");
        return null;
      }
    }

    // --- Compression ---
    private static byte[] Compress(byte[] data) {
      using (var ms = new MemoryStream())
      using (var gz = new GZipStream(ms, CompressionMode.Compress)) {
        gz.Write(data, 0, data.Length);
        gz.Close();
        return ms.ToArray();
      }
    }

    private static byte[] Decompress(byte[] data) {
      using (var input = new MemoryStream(data))
      using (var gz = new GZipStream(input, CompressionMode.Decompress))
      using (var output = new MemoryStream()) {
        gz.CopyTo(output);
        return output.ToArray();
      }
    }

    // --- Encryption (AES) ---
    private static byte[] Encrypt(byte[] data) {
      using (var aes = Aes.Create()) {
        aes.Key = Key;
        aes.IV = IV;

        using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
        using (var ms = new MemoryStream())
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write)) {
          cs.Write(data, 0, data.Length);
          cs.Close();
          return ms.ToArray();
        }
      }
    }

    private static byte[] Decrypt(byte[] data) {
      using (var aes = Aes.Create()) {
        aes.Key = Key;
        aes.IV = IV;

        using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
        using (var ms = new MemoryStream(data))
        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
        using (var output = new MemoryStream()) {
          cs.CopyTo(output);
          return output.ToArray();
        }
      }
    }
  }
}
