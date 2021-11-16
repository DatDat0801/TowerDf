using System.Collections.Generic;
using EW2.Tutorial.General;
using TigerForge;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class StartWaveButtonController : Singleton<StartWaveButtonController>
    {
        private Canvas canvas;

        private GraphicRaycaster graphicRaycaster;

        private List<StartWaveButton> listWaveButton = new List<StartWaveButton>();

        private WaveInfoController waveInfo;

        public bool GameStarted { get; private set; }
        private void Awake()
        {
            EventManager.StartListening(GamePlayEvent.OnConfirmCallWave, OnConfirmCallWave);

            StartWaveButton.onStartButtonConfirmed += HandleStartButtonConfirmed;

            StartWaveButton.onStartButtonUnconfirmed += HandleStartButtonUnconfirmed;

            GameStarted = false;
        }

        private void HandleStartButtonUnconfirmed(StartWaveButton btn)
        {
            if (waveInfo != null)
            {
                waveInfo.HideInfoWave();
            }
        }

        private void HandleStartButtonConfirmed(StartWaveButton btn)
        {
            if (waveInfo == null)
            {
                if (canvas)
                {
                    var go = ResourceUtils.GetUnitOther("tool_tip_info_wave",canvas.transform,false);

                    if (go != null)
                    {
                        waveInfo = go.GetComponent<WaveInfoController>();

                        waveInfo.ShowWaveInfo(btn.transform.position, btn.EnemyInLane);
                    }
                }
            }
            else
            {
                waveInfo.ShowWaveInfo(btn.transform.position, btn.EnemyInLane);

                waveInfo.gameObject.SetActive(true);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventManager.StopListening(GamePlayEvent.OnConfirmCallWave, OnConfirmCallWave);
        }

        private void OnConfirmCallWave()
        {
            CallWave.Instance.Execute();
            HideButtonCallWave();
            GameStarted = true;
        }

        public void InitButtonCallWave(List<PointCallSpawn> listPointSpawn)
        {
            if (canvas == null)
            {
                canvas = GetComponentInChildren<Canvas>();

                canvas.worldCamera = GamePlayController.Instance.GetCameraController().MyCamera;

                graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
            }


            listWaveButton.Clear();

            for (int i = 0; i < listPointSpawn.Count; i++)
            {
                var go = ResourceUtils.GetUnitOther("button_start_wave",canvas.transform,false );

                if (go != null)
                {
                    var control = go.GetComponent<StartWaveButton>();

                    control.InitButton(listPointSpawn[i]);

                    listWaveButton.Add(control);
                }
            }
        }

        public void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (graphicRaycaster == null) return;

            graphicRaycaster.Raycast(eventData, resultAppendList);
        }

        public void ShowButtonCallWave()
        {
            for (int i = 0; i < listWaveButton.Count; i++)
            {
                listWaveButton[i].ShowNextWave();
            }
        }

        public void HideButtonCallWave()
        {
            for (int i = 0; i < listWaveButton.Count; i++)
            {
                listWaveButton[i].HideButton();
            }
        }

        public StartWaveButton GetFirstBtnCallWave() => listWaveButton[0];
    }
}