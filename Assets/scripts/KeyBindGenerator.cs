using UnityEditor;
using UnityEngine;

public class KeyBindGenerator
{
    [MenuItem("Tools/Generate Default KeyBinds")]
    public static void GenerateKeyBinds()
    {
        // Path to save ScriptableObjects
        string folderPath = "Assets/KeyBinds";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "KeyBinds");
        }

        // Define your keybinds
        (string, KeyCode)[] keybinds = new (string, KeyCode)[]
        {
            ("Forward", KeyCode.W),
            ("Backward", KeyCode.S),
            ("Right", KeyCode.D),
            ("Left", KeyCode.A),
            ("Jump", KeyCode.Space),
            ("Sprint", KeyCode.LeftShift),
            ("Interact", KeyCode.E),
            ("Attack", KeyCode.Mouse0),
            ("ToggleActivateAttack", KeyCode.LeftControl),
            ("TakeOneItem", KeyCode.LeftAlt),
            ("Pause", KeyCode.Escape),
            ("Inventory", KeyCode.M),
            ("SkillTree", KeyCode.N),
            ("FullMap", KeyCode.Tab),
        };

        foreach (var (name, key) in keybinds)
        {
            string assetPath = $"{folderPath}/{name}Key.asset";

            // Avoid overwriting if already exists
            Keybind existing = AssetDatabase.LoadAssetAtPath<Keybind>(assetPath);
            if (existing != null)
            {
                existing.SetKey(name);
                existing.SetKeycode(key);
                EditorUtility.SetDirty(existing);
                Debug.Log($"Updated existing KeyBind: {name}");
                continue;
            }

            // Create new ScriptableObject
            Keybind keyBind = ScriptableObject.CreateInstance<Keybind>();
            keyBind.SetKey(name);
            keyBind.SetKeycode(key);

            AssetDatabase.CreateAsset(keyBind, assetPath);
            Debug.Log($"Created KeyBind: {name}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("KeyBind generation complete!");
    }
}
