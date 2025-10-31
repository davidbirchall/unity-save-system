# Unity Save System (10 Slots + Autosave)

**Namespaces**: `Birchall.SaveSystem` (+ `.Example` for demo scripts)

## Install
- Copy the `SaveSystem` folder into your Unity `Assets` folder.
- Ensure all scripts compile in the same assembly (avoid asmdef splits unless you add references).

## Use
1. Add `SaveSystem` (component) to a scene object (e.g., an empty "Systems" GameObject).
2. Add your saveable components (e.g., `Player`, `Inventory`, `World`, `Settings`) to scene objects.
3. (Optional) Add `Example/SaveUI` to test with keyboard:
   - F5: Save to slot 0
   - F9: Load from slot 0
   - F10: Autosave
   - F11: Load autosave

## Notes
- Save files are written to `Application.persistentDataPath`:
  - Manual slots: `save_0.json` ... `save_9.json`
  - Autosave: `autosave.json`
- Metadata includes save name, scene, timestamp, and a version field.
- If you use Assembly Definitions, keep everything in one asmdef or add a reference from your game asmdef to the save-system asmdef.

## Extending
- Add or remove modules in `SaveGame` (e.g., remove `World` if unused).
- Create new `...Data` classes in `SaveSystem/Data` and corresponding components that implement `ISaveable`.
- For a more generic container, you could swap typed fields in `SaveGame` for a list of (key,json) pairs.