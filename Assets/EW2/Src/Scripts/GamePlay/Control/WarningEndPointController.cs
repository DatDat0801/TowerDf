using System;
using TigerForge;
using UnityEngine;
using Zitga.Update;
using Zitga.ContextSystem;
using Zitga.UIFramework;

namespace EW2
{
    public class WarningEndPointController : MonoBehaviour, IUpdateSystem
    {
        public Transform pointer;

        private Rect screenRect;

        private EndPointController target;

        private Vector3 pointCheck;

        public virtual void OnEnable()
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
            if (target)
            {
                Vector3 direction = pointCheck - transform.position;

                float angle = (float) Math.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                pointer.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        public void ShowWarning()
        {
            gameObject.SetActive(true);
        }

        public void InitWarning(EndPointController gate)
        {
            this.target = gate;

            pointCheck = target.GetPointSpawnWarning();

            UpdatePosition();

            ShowWarning();
        }

        private void UpdatePosition()
        {
            screenRect = new Rect(0f, 0f, Screen.width, Screen.height);

            var myCamera = GamePlayController.Instance.GetCameraController().MyCamera;
            
            var xMaxScreen = myCamera.ScreenToWorldPoint(new Vector2(screenRect.max.x, 0));

            var xMinScreen = myCamera.ScreenToWorldPoint(new Vector2(screenRect.min.x, 0));

            var yMaxScreen = myCamera.ScreenToWorldPoint(new Vector2(0, screenRect.max.y - 35f));

            var yMinScreen = myCamera.ScreenToWorldPoint(new Vector2(0, screenRect.min.y + 35f));

            var posTarget = pointCheck;

            if (posTarget.x <= xMinScreen.x)
            {
                posTarget.x = xMinScreen.x + 1f;
            }

            if (posTarget.x >= xMaxScreen.x)
            {
                posTarget.x = xMaxScreen.x - 1f;
            }

            if (posTarget.y >= yMaxScreen.y)
            {
                posTarget.y = yMaxScreen.y - 1f;
            }

            if (posTarget.y <= yMinScreen.y)
            {
                posTarget.y = yMinScreen.y + 1f;
            }
            
            transform.position = posTarget;
        }

        void OnGameClose()
        {
            Destroy(gameObject, 0.5f);
        }
    }
}