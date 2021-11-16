using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class EditorTools : OdinMenuEditorWindow
{
    [MenuItem("Tools/Custom Tools")]
    private static void OpenWindow()
    {
        var window = GetWindow<EditorTools>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
    }
    protected override OdinMenuTree BuildMenuTree()
    {
        ClearPlayerPrefCustom prefCustom = new ClearPlayerPrefCustom();
        OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true) {{"PlayerPrefs", prefCustom, EditorIcons.House},};

        tree.SortMenuItemsByName();

        return tree;
    }
}

public class ClearPlayerPrefCustom
{
    [InfoBox("Clear a PlayerPrefs key")]
    public string key;

    [Button]
    public void Clear()
    {
        PlayerPrefs.DeleteKey(key);
        EditorUtility.DisplayDialog("Success", $"Deleted Key {key}","Ok");
    }
}