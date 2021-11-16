using System.Collections.Generic;
using EW2.Constants;
using EW2.Tutorial.General;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace EW2.Tutorial.Map
{
    public class Map0Tutorial : MonoBehaviour
    {
        public Transform Hero1002MoveTransform
        {
            get => hero1002MoveTransform;
            set => hero1002MoveTransform = value;
        }

        public Button Hero1002MovePositionFocusBtn
        {
            get => hero1002MovePositionFocusBtn;
            set => hero1002MovePositionFocusBtn = value;
        }

        public Button HeroSkillPositionFocusBtn
        {
            get => heroSkillPositionFocusBtn;
            set => heroSkillPositionFocusBtn = value;
        }

        public Transform HeroSkillTransform
        {
            get => heroSkillTransform;
            set => heroSkillTransform = value;
        }



        [SerializeField] private Transform hero1002MoveTransform;
        [FormerlySerializedAs("hero1001SkillTransform")] [SerializeField] private Transform heroSkillTransform;

        [SerializeField] private Canvas focusCanvas;
        [SerializeField] private Button hero1002MovePositionFocusBtn;
        [FormerlySerializedAs("hero1001SkillPositionFocusBtn")] [SerializeField] private Button heroSkillPositionFocusBtn;
        [SerializeField] private Transform towerBuildingFocusTransform;
        [SerializeField] private float towerBuildingFocusSize;
        [SerializeField] private float towerBuildingFocusSpeed;
        [SerializeField] private Transform startWaveTransform;
        [SerializeField] private float startWaveFocusSize;
        [SerializeField] private float startWaveFocusSpeed;
        [SerializeField] private Transform hero1002FocusTransform;
        [SerializeField] private float hero1002FocusSize;
        [SerializeField] private float hero1002FocusSpeed;


        [FormerlySerializedAs("_anyTowerPoints")] [SerializeField] private List<GameObject> anyTowerPoints;
        private CameraController cameraController;



        private void Awake()
        {
            focusCanvas.worldCamera = Camera.main;
            cameraController = FindObjectOfType<CameraController>();
            TrackCompleteFocusFirstTowerTutorial();
            TrackCompleteFocusSecondTowerTutorial();
            TrackCompleteFocusThirdTowerTutorial();
        }

        private void Start()
        {
            Hero1002MovePositionFocusBtn.onClick.AddListener(OnHero1002MovePositionFocusClick);
            heroSkillPositionFocusBtn.onClick.AddListener(OnHero1001SkillPositionFocusClick);
            heroSkillPositionFocusBtn.onClick.AddListener(OnHero1002SkillPositionFocusClick);
        }

        public void ExecuteFocusTowerBuilding()
        {
            cameraController.ExecuteFocusPoint(towerBuildingFocusSize, towerBuildingFocusTransform.position, towerBuildingFocusSpeed);
        }

        public void ExecuteFocusStartWave()
        {
            cameraController.ExecuteFocusPoint(startWaveFocusSize, startWaveTransform.position, startWaveFocusSpeed);
        }

        public void ExecuteFocusHero1002()
        {
            GamePlayUIManager.Instance.CloseCurrentUI(false);

            cameraController.ExecuteFocusPoint(hero1002FocusSize, hero1002FocusTransform.position, hero1002FocusSpeed);
        }

        public void ResetCameraDefault()
        {
            cameraController.ResetDefault();
        }
        void IncreaseLayerFocusPoint(GameObject focusPoint)
        {
            // var  layerFocusPoint = focusPoint.AddComponent<LayerFocusPoint>();
            // layerFocusPoint.IncreaseLayer();
            SetLayerTowerPoint(focusPoint, 1, SortingLayerConstants.TUTORIAL);
        }

        public void ResetLayerFocusPoint()
        {
            foreach (var towerPoint in anyTowerPoints)
            {
                SetLayerTowerPoint(towerPoint, 30, SortingLayerConstants.MAP);
            }
        }
        private void SetLayerTowerPoint(GameObject focusPoint, int startLayer, string layer)
        {
            TowerPointController towerPoint = focusPoint.transform.GetComponent<TowerPointController>();
            var buildPoint = towerPoint.myRenderer;
            buildPoint.sortingLayerName = layer;
            buildPoint.sortingOrder = startLayer;
            if (layer == SortingLayerConstants.MAP)
            {
                layer = SortingLayerConstants.UNIT;
            }
            towerPoint.raiseIcon.sortingOrder = startLayer + 1;
            towerPoint.raiseIcon.sortingLayerName = layer;
        }

        public void IncreaseMageTowerPoint()
        {
            IncreaseLayerFocusPoint(anyTowerPoints[1]);
        }
        public void ShowFirstTowerPoint()
        {
            anyTowerPoints[0].SetActive(true);
            IncreaseLayerFocusPoint(anyTowerPoints[0]);
            SpawnTowerPointOverlayTutorial(anyTowerPoints[0]);

        }

        public void ShowSecondTowerPoint()
        {
            anyTowerPoints[1].SetActive(true);
            SetLayerTowerTutorial(1);
            SpawnTowerPointOverlayTutorial(anyTowerPoints[1]);
        }

        public void SetLayerTowerTutorial(int indexOfTowerPoint)
        {
            TowerPointController towerPoint = anyTowerPoints[indexOfTowerPoint].GetComponent<TowerPointController>();
            if (towerPoint.myTower != null)
            {
                towerPoint.myTower.SetSortLayerForTutorial();
            }
        }
        public void SetLayerTowerGameplay(int indexOfTowerPoint)
        {
            TowerPointController towerPoint = anyTowerPoints[indexOfTowerPoint].GetComponent<TowerPointController>();
            if (towerPoint.myTower != null)
            {
                towerPoint.myTower.SetSortLayerForGamePlay();
            }
        }
        public void ShowThirdTowerPoint()
        {
            anyTowerPoints[2].SetActive(true);
            IncreaseLayerFocusPoint(anyTowerPoints[2]);
            SpawnTowerPointOverlayTutorial(anyTowerPoints[2]);
        }



        public void HideAllTowerPoints()
        {
            foreach (var towerPoint in anyTowerPoints)
            {
                towerPoint.SetActive(false);
            }
        }

        public void ShowAllTowerPoints()
        {
            foreach (var towerPoint in anyTowerPoints)
            {
                towerPoint.SetActive(true);
            }
        }

        public Vector3 CalculateFirstTowerPointPosition() => anyTowerPoints[0].transform.position;
        public Vector3 CalculateSecondTowerPointPosition() => anyTowerPoints[1].transform.position;
        public Vector3 CalculateThirdTowerPointPosition() => anyTowerPoints[2].transform.position;

        public float CalculateSecondDelayCameraFocus() => 1 / CameraController.SPEED_RESET_DEFAULT + 0.2f;

        public void TrackCompleteFocusMageBuildedTowerTutorial()
        {
            anyTowerPoints[1].GetComponent<TowerPointController>().PressingTowerPoint = () => {
                TutorialManager.Instance.CompleteCurrentTutorialFollowId(AnyTutorialConstants.FOCUS_MAGE_BUILED_TOWER);
            };
        }

        private void SpawnTowerPointOverlayTutorial(GameObject towerPoint)
        {
            var towerPointOverlay = ResourceUtils.GetTutorial(AnyTutorialConstants.TOWER_POINT_OVERLAY_NAME);
            var towerPointOverlayClone =
                LeanPool.Spawn(towerPointOverlay, Vector3.zero, Quaternion.identity, towerPoint.transform)
                    .GetComponent<TowerPointOverlayTutorial>();
            towerPointOverlayClone.ComputeTowerPointPosition(towerPoint.transform.position);
        }

        private void TrackCompleteFocusFirstTowerTutorial()
        {
            anyTowerPoints[0].GetComponent<TowerPointController>().PressingTowerPoint = () => {
                TutorialManager.Instance.CompleteCurrentTutorialFollowId(AnyTutorialConstants.FOCUS_BUILD_FIRST_TOWER);
            };
        }

        private void TrackCompleteFocusSecondTowerTutorial()
        {
            anyTowerPoints[1].GetComponent<TowerPointController>().PressingTowerPoint = () => {
                TutorialManager.Instance.CompleteCurrentTutorialFollowId(AnyTutorialConstants.FOCUS_BUILD_SECOND_TOWER);
            };
        }

        private void TrackCompleteFocusThirdTowerTutorial()
        {
            anyTowerPoints[2].GetComponent<TowerPointController>().PressingTowerPoint = () => {
                TutorialManager.Instance.CompleteCurrentTutorialFollowId(AnyTutorialConstants.FOCUS_BUILD_THIRD_TOWER);
            };
        }

        private void OnHero1002MovePositionFocusClick()
        {
            TutorialManager.Instance.CompleteCurrentTutorialFollowId(AnyTutorialConstants.FOCUS_1002_HERO_MOVE_POSITION);
        }

        private void OnHero1001SkillPositionFocusClick()
        {
            TutorialManager.Instance.CompleteCurrentTutorialFollowId(AnyTutorialConstants.FOCUS_1001_HERO_SkILL_POSITION);
        }

        private void OnHero1002SkillPositionFocusClick()
        {
            TutorialManager.Instance.CompleteCurrentTutorialFollowId(AnyTutorialConstants.FOCUS_1002_HERO_SkILL_POSITION);
        }


    }
}
