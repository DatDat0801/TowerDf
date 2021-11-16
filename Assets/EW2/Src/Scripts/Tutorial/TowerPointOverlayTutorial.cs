using System;
using System.Collections;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace EW2.Tutorial.Map
{
    public class TowerPointOverlayTutorial : MonoBehaviour
    {
        [SerializeField] private Button towerPointBtn;
        
        
        private Canvas canvas;
        private TowerPointController towerPointController;


        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            towerPointController = GetComponentInParent<TowerPointController>();
            canvas.worldCamera=Camera.main;
            towerPointBtn.onClick.AddListener(OnTowerPointClick );
        }

        public void ComputeTowerPointPosition(Vector3 position)
        {
            StartCoroutine(CoComputeTowerPointPosition(position));
        }
      

        private IEnumerator CoComputeTowerPointPosition(Vector3 position)
        {
            yield return new WaitForEndOfFrame();
            towerPointBtn.transform.position = position;
        }

      

        private void OnTowerPointClick()
        {
            towerPointController.OnPressed();
            gameObject.SetActive(false);
          //  LeanPool.Despawn(gameObject);
        }
    }
}