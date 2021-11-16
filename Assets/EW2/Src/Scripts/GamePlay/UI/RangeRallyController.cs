using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class RangeRallyController : MonoBehaviour, UIGameplay
    {
        private static RangeRallyController instance;

        public static RangeRallyController Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = ResourceUtils.GetUnitOther("range_rally");
                    if (obj != null)
                    {
                        // GameObject obj = Instantiate(prefab);
                        // obj.SetActive(false);
                        instance = obj.GetComponent<RangeRallyController>();
                    }
                }

                return instance;
            }
        }

        private Building buildingTarget;

        public Building BuildingTarget => buildingTarget;

        public void SetDataRange(Building towerMelee)
        {
            buildingTarget = towerMelee;
            transform.position = towerMelee.transform.position;
            gameObject.SetActive(true);
            EventManager.StartListening(GamePlayEvent.OnRallyDone, OnRallyDone);
            SelectSoldiers();
        }

        private void OnRallyDone()
        {
            var pos = EventManager.GetData<Vector3>(GamePlayEvent.OnRallyDone);

            if (buildingTarget is Tower2004)
            {
                Tower2004 tower2004 = buildingTarget as Tower2004;
                if (tower2004 != null)
                    tower2004.RallySoldier(pos);
            }
            else if (buildingTarget is Tower2002)
            {
                Tower2002 tower2002 = buildingTarget as Tower2002;
                if (tower2002 != null)
                    tower2002.RallyHole(pos);
            }

            GamePlayUIManager.Instance.CloseCurrentUI(true);
            DeselectSoldiers();
        }

        public void Open()
        {
            ShowAnim();
        }

        public void Close()
        {
            EventManager.StopListening(GamePlayEvent.OnRallyDone, OnRallyDone);

            gameObject.SetActive(false);
            
            DeselectSoldiers();
        }

        public UI_STATE GetUIType()
        {
            return UI_STATE.Rally;
        }

        private void ShowAnim()
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            transform.DOKill();
            transform.DOScale(Vector3.one, 0.2f);
        }

        void SelectSoldiers()
        {
            foreach (var soldier in buildingTarget.Soldiers)
            {
                if (soldier.Aura != null)
                    soldier.Aura.SetActive(true);
            }
        }

        private void DeselectSoldiers()
        {
            foreach (var soldier in buildingTarget.Soldiers)
            {
                if (soldier.Aura != null)
                    soldier.Aura.SetActive(false);
            }
        }
    }
}