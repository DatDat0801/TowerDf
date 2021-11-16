using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace Zitga.UIFramework
{
    /// <summary>
    /// This is a "helper" layer so Windows with higher priority can be displayed.
    /// By default, it contains any window tagged as a Popup. It is controlled by the WindowUILayer.
    /// </summary>
    public class WindowParaLayer : MonoBehaviour
    {
        [SerializeField] private GameObject darkenBgObject = null;

        private List<GameObject> containedScreens = new List<GameObject>();
        

        public void AddScreen(Transform screenRectTransform)
        {
            screenRectTransform.SetParent(transform, false);
            containedScreens.Add(screenRectTransform.gameObject);
        }

        public void RefreshDarken()
        {
            // for (int i = 0; i < containedScreens.Count; i++)
            // {
            //     if (containedScreens[i] != null)
            //     {
            //         if (containedScreens[i].activeSelf)
            //         {
            //             darkenBgObject.SetActive(true);
            //             var screenIndex = containedScreens[i].transform.GetSiblingIndex();
            //             darkenBgObject.transform.SetSiblingIndex(screenIndex == 0 ? 0 : screenIndex - 1);
            //             return;
            //         }
            //     }
            // }
            for (int i = transform.childCount - 1; i >=0 ; i--)
            {
                if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).gameObject != darkenBgObject)
                {
                    darkenBgObject.SetActive(true);
                    var screenIndex = transform.GetChild(i).GetSiblingIndex();
                    darkenBgObject.transform.SetSiblingIndex(screenIndex == 0 ? 0 : screenIndex - 1);
                    return;
                }
            }
            //var topScreen = containedScreens.FindLast(o => o.activeSelf);
            
            //if (topScreen != null)
            //{
            //    darkenBgObject.SetActive(true);
            //    var screenIndex = topScreen.transform.GetSiblingIndex();
            //    darkenBgObject.transform.SetSiblingIndex(screenIndex == 0 ? 0 : screenIndex - 1);
            //}
            //else
            //{
                darkenBgObject.SetActive(false);
            //}
        }

        public void DarkenBG()
        {
            darkenBgObject.SetActive(true);
            darkenBgObject.transform.SetAsLastSibling();
        }

        public void CloseAllPriorityLayer()
        {
            for (var i = 0; i < containedScreens.Count; i++)
            {
                containedScreens[i].gameObject.SetActive(false);
            }

            darkenBgObject.SetActive(false);
        }
    }
}