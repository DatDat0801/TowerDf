using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Invoke;
using Lean.Pool;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class HandleGamePlayUI : MonoBehaviour
    {
        void Start()
        {
            GamePlayController.Instance.GetCameraController().OnTap += HandleCameraOnTap;

            GamePlayController.Instance.GetCameraController().OnCameraMove += HandleCameraMove;
        }

        private void HandleCameraMove()
        {
            if (GamePlayUIManager.Instance.MyUIState == UI_STATE.Soft)
            {
                if (GamePlayUIManager.Instance.CurrentUi != null)
                    GamePlayUIManager.Instance.CurrentUi.Close();

                GamePlayUIManager.Instance.MyUIState = UI_STATE.Free;
            }
        }

        private void HandleCameraOnTap(bool success, Vector3 pos)
        {
            switch (GamePlayUIManager.Instance.MyUIState)
            {
                case UI_STATE.Soft:
                case UI_STATE.Normal:
                    if (GamePlayUIManager.Instance.CurrentUi != null)
                        GamePlayUIManager.Instance.CurrentUi.Close();

                    GamePlayUIManager.Instance.MyUIState = UI_STATE.Free;
                    break;
                case UI_STATE.SelectHero:
                    if (success)
                    {
                        EventManager.EmitEventData(GamePlayEvent.OnTargetDone, pos);

                        var go = ResourceUtils.GetUnit("move_effect", pos, Quaternion.identity);
                        if (go != null)
                            SetEffectCancel(go.transform);
                    }
                    else
                    {
                        Vector3 newPos = GamePlayController.Instance.GetCameraController()
                            .HandleCheckNearestPosition(pos, Vector3.zero);

                        var go = ResourceUtils.GetUnit("cancel_x", pos, Quaternion.identity);
                        if (go != null)
                            SetEffectCancel(go.transform);

                        EventManager.EmitEventData(GamePlayEvent.OnTargetDone, newPos);
                    }

                    GamePlayUIManager.Instance.IsLock = true;

                    StartCoroutine(Unlock());

                    GamePlayUIManager.Instance.MyUIState = UI_STATE.Free;

                    break;
                case UI_STATE.ActiveSkill:
                    if (success)
                    {
                        EventManager.EmitEventData(GamePlayEvent.OnTargetDone, pos);
                    }
                    else
                    {
                        Vector3 newPos = GamePlayController.Instance.GetCameraController()
                            .HandleCheckNearestPosition(pos, Vector3.zero);

                        EventManager.EmitEventData(GamePlayEvent.OnTargetDone, newPos);
                    }

                    GamePlayUIManager.Instance.IsLock = true;

                    StartCoroutine(Unlock());

                    break;
                case UI_STATE.ActiveSpell:
                    if (success)
                    {
                        EventManager.EmitEventData(GamePlayEvent.OnSpellSelectTarget, pos);
                    }
                    
                    //datnd: dont know what the hell is this
                    GamePlayUIManager.Instance.IsLock = true;
                    StartCoroutine(Unlock());
                    break;
                case UI_STATE.Rally:
                    var checkInRange = GamePlayController.Instance.GetCameraController()
                        .CheckPointTargetRallySuccess(pos);

                    if (checkInRange)
                    {
                        EventManager.EmitEventData(GamePlayEvent.OnRallyDone, pos);

                        var go = ResourceUtils.GetUnit("rally", pos, Quaternion.identity);

                        if (go != null)
                            SetEffectCancel(go.transform);
                    }
                    else
                    {
                        Vector3 newPos = GamePlayController.Instance.GetCameraController()
                            .HandleCheckNearestPositionInRange(pos,
                                RangeRallyController.Instance.BuildingTarget.Transform.position);

                        var go = ResourceUtils.GetUnit("cancel_x", pos, Quaternion.identity);

                        if (go != null)
                            SetEffectCancel(go.transform);

                        EventManager.EmitEventData(GamePlayEvent.OnRallyDone, newPos);
                    }

                    GamePlayUIManager.Instance.IsLock = true;

                    StartCoroutine(Unlock());

                    GamePlayUIManager.Instance.MyUIState = UI_STATE.Free;

                    break;
            }
        }

        private void SetEffectCancel(Transform obj)
        {
            obj.DOKill();
            obj.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f);
            obj.DOPlay();

            InvokeProxy.Iinvoke.Invoke(this, () => { LeanPool.Despawn(obj.gameObject); }, 0.5f);
        }

        private IEnumerator Unlock()
        {
            yield return new WaitForSecondsRealtime(0.05f);
            GamePlayUIManager.Instance.IsLock = false;
        }

        private void OnDisable()
        {
            if (InvokeProxy.Iinvoke != null)
            {
                InvokeProxy.Iinvoke.CancelInvoke(this);
            }
        }
    }
}