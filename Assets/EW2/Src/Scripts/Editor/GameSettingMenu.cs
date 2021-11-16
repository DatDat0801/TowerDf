using TigerForge;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class GameSettingMenu
    {
        private const string CLEAR_DATA_SAVE = "Tools/Game Setting/Clear Data Save";
        
        private const string PLAY_GAME = "Tools/Game Setting/PLAY";

        [MenuItem(CLEAR_DATA_SAVE, false, 0)]
        static void ClearDataSave()
        {
            var myFile = new EasyFileSave();
            
            myFile.Delete();
            
            PlayerPrefs.SetInt("number_tab_shop", 0);
        }
        
        [MenuItem(PLAY_GAME, false, 0)]
        static void PlayGame()
        {
            if (Application.isPlaying)
                return;
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/EW2/Scenes/StartScene.unity");
                EditorApplication.isPlaying = true;
            }

        }
    }
