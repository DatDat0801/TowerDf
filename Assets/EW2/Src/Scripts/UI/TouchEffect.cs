using System;
using UnityEngine;
using Zitga.ContextSystem;
using Zitga.UIFramework;
using Zitga.Update;

namespace EW2
{
    public class TouchEffect : MonoBehaviour, IUpdateSystem
    {
        private Camera cameraUI;

        private void OnEnable()
        {
            if (Context.Current.GetService<GlobalUpdateSystem>() != null)
            {
                Context.Current.GetService<GlobalUpdateSystem>().Add(this);
            }
        }

        public virtual void OnDisable()
        {
            if (Context.Current != null)
                Context.Current.GetService<GlobalUpdateSystem>().Remove(this);
        }

        public void OnUpdate(float deltaTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ShowEffect();
            }
        }

        private void ShowEffect()
        {
            if (cameraUI == null)
            {
                cameraUI = UIFrame.Instance.UICamera;
            }

            if (!cameraUI.gameObject.activeSelf) return;

            var pos = cameraUI.ScreenToWorldPoint(Input.mousePosition);

            pos.z = 0;
            
            var go = ResourceUtils.GetVfx("Effects", "fx_common_touch", pos, Quaternion.identity,
                UIFrame.Instance.MainCanvas.transform);

            if (go)
                go.transform.position = pos;
        }
    }
}