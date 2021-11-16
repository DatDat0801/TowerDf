using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    public class ButtonNewEnemyController : MonoBehaviour
    {
        [SerializeField] private Button btnNewEnemy;

        [SerializeField] private Text title;

        private Queue<int> newEnemyQueue = new Queue<int>();

        private void Awake()
        {
            if (btnNewEnemy)
            {
                btnNewEnemy.onClick.AddListener(OnClick);
            }
        }

        public void ShowButton(List<int> listNewEnemies)
        {
            title.text = L.gameplay.caution_new_enemy;
            
            newEnemyQueue.Clear();

            foreach (var enemy in listNewEnemies)
            {
                newEnemyQueue.Enqueue(enemy);
            }

            if (newEnemyQueue.Count > 0)
                gameObject.SetActive(true);
        }

        private void HideButton()
        {
            newEnemyQueue.Clear();

            gameObject.SetActive(false);
        }

        private void OnClick()
        {
            if (newEnemyQueue.Count > 0)
            {
                var enemyId = newEnemyQueue.Dequeue();
                
                GamePlayUIManager.Instance.CloseCurrentUI(true);
                GamePlayUIManager.Instance.CloseAllUI();
                
                UIFrame.Instance.OpenWindow(ScreenIds.new_enemies_popup, new NewEnemyWindowProperties(enemyId));
                if (newEnemyQueue.Count <= 0)
                    HideButton();
            }
        }

    }
}