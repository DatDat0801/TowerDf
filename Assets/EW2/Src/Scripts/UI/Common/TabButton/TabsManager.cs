using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace EW2
{
    public class TabsManager : MonoBehaviour
    {
        [SerializeField] private bool changeColor;

        [ShowIf("changeColor")] public ColorConfig colorConfig;

        [SerializeField] private bool changeImageAndLabel;

        [ShowIf("changeImageAndLabel")] public ColorLabelConfig colorLabelConfig;

        [SerializeField] private List<int> listTabDisable;
        
        [SerializeField] private SubWindow[] subWindows;

        private IUpdateTabBarChanged onTabClickedListeners;

        private List<TabButton> tabs = new List<TabButton>();

        private int activeIndex;

        private bool isInited;

        public bool IsInited => isInited;

        public int ActiveIndex
        {
            get => activeIndex;
            set => activeIndex = value;
        }

        private void Awake()
        {
            tabs.Clear();

            foreach (Transform tab in transform)
            {
                var script = tab.GetComponent<TabButton>();

                if (script)
                {
                    script.OnClickButton = OnTabClicked;

                    tabs.Add(script);
                }
            }
        }

        public void InitTabManager(IUpdateTabBarChanged tabClickedListeners, int indexStart)
        {
            onTabClickedListeners = tabClickedListeners;

            SetSelected(indexStart);

            isInited = true;
        }

        private void SetTabActiveColor()
        {
            foreach (var tab in tabs)
            {

                tab.SetTabActiveChangeColor(tab.GetIndex() == activeIndex, colorConfig.colorActive,
                    colorConfig.colorInactive,
                    colorConfig.colorTextActive, colorConfig.colorTextInactive);
            }
        }

        private void SetTabActiveImageAndLabel()
        {
            foreach (var tab in tabs)
            {

                tab.SetTabActiveChangeImgAndLabel(tab.GetIndex() == activeIndex, colorLabelConfig.colorTextActive,
                    colorLabelConfig.colorTextInactive);
            }
        }

        private void SetTabActiveImage()
        {
            foreach (var tab in tabs)
            {
                // if (listTabDisable.Contains(tab.GetIndex())) continue;

                tab.SetTabActiveChangeImg(tab.GetIndex() == activeIndex);
            }
        }

        public void OnTabClicked(int indexActive)
        {
            if (activeIndex == indexActive) return;
            
            CloseSubWindows(indexActive);
            
            if (!listTabDisable.Contains(indexActive))
                activeIndex = indexActive;

            if (changeColor)
                SetTabActiveColor();
            else if (changeImageAndLabel)
                SetTabActiveImageAndLabel();
            else
                SetTabActiveImage();

            if (listTabDisable.Contains(indexActive))
            {
                Ultilities.ShowToastNoti(L.common.coming_soon);
                return;
            }

            onTabClickedListeners?.OnTabBarChanged(activeIndex);
        }

        private void CloseSubWindows(int indexClicked)
        {
            foreach (var subWindow in subWindows)
            {
                subWindow.Close(indexClicked);
            }
        }

        public void SetSelected(int selindex)
        {
            activeIndex = selindex;

            if (changeColor)
                SetTabActiveColor();
            else if (changeImageAndLabel)
                SetTabActiveImageAndLabel();
            else
                SetTabActiveImage();

            onTabClickedListeners?.OnTabBarChanged(activeIndex);
        }

        public void Reload()
        {
            SetSelected(activeIndex);
        }
    }

    [Serializable]
    public class ColorConfig
    {
        public Color colorActive = Color.white;

        public Color colorInactive = Color.white;

        public Color colorTextActive = Color.white;

        public Color colorTextInactive = Color.white;
    }

    [Serializable]
    public class ColorLabelConfig
    {
        public Color colorTextActive = Color.white;

        public Color colorTextInactive = Color.white;
    }
}