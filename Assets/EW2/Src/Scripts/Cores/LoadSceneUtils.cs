using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zitga.UIFramework;

namespace EW2
{
    public static class LoadSceneUtils
    {
        private static Action onLoadSceneDone;

        public static void LoadScene(string name, Action onComplete = null)
        {
            onLoadSceneDone = onComplete;

            UIFrame.Instance.ShowPanel(ScreenIds.loading);

            UIFrame.Instance.CloseAllWindows(false);

            CoroutineUtils.Instance.StartCoroutine(LoadAsyncScene(name));
        }

        static IEnumerator LoadAsyncScene(string name)
        {
            // The Application loads the Scene in the background as the current Scene runs.
            // This is particularly good for creating loading screens.
            // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
            // a sceneBuildIndex of 1 as shown in Build Settings.

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            if (onLoadSceneDone != null)
            {
                onLoadSceneDone.Invoke();
                onLoadSceneDone = null;
            }
        }
    }
}