using System;
using System.Collections.Generic;
using Zitga.Utils;
using UnityEngine;

namespace Zitga.UIFramework.Examples
{
    public class UIDemoController : MonoBehaviour
    {
        private void Awake() {
            Instantiate(Resources.Load<UIFrame>("UIFrame"));
        }

        private void Start() {
            UIFrame.Instance.OpenWindow(ScreenIds.StartGameWindow);
        }
    }
}