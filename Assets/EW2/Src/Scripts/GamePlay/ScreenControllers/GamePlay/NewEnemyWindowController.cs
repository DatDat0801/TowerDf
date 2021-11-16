using System;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class NewEnemyWindowProperties : WindowProperties
    {
        public int enemyId;

        public NewEnemyWindowProperties(int data)
        {
            enemyId = data;
        }
    }

    public class NewEnemyWindowController : AWindowController<NewEnemyWindowProperties>
    {
        [SerializeField] private Button btnClose;

        [SerializeField] private Image iconEnemy;

        [SerializeField] private Text lbContent;

        [SerializeField] private Text lbTip;

        [SerializeField] private Text lbNameEnemy;

        [SerializeField] private Text lbTitle;
        
        [SerializeField] private Text lbTapToClose;

        protected override void Awake()
        {
            base.Awake();

            btnClose.onClick.AddListener(OnClose);
        }

        private void OnClose()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            print($"Enemy Id {Properties.enemyId}");
            
            ShowInfoEnemy();
        }

        private void ShowInfoEnemy()
        {
            iconEnemy.sprite = ResourceUtils.GetSpriteEnemyGuide(Properties.enemyId);

            lbContent.text = R.enemy.GetDesEnemy(Properties.enemyId);

            var stringWarnEnemy = R.enemy.GetWarnEnemy(Properties.enemyId);

            if (!stringWarnEnemy.StartsWith("#enemy"))
                lbTip.text = R.enemy.GetWarnEnemy(Properties.enemyId);
            else
                lbTip.text = String.Empty;

            lbNameEnemy.text = R.enemy.GetNameEnemy(Properties.enemyId);

            lbTitle.text = R.gameplay.caution_new_enemy;

            lbTapToClose.text = L.popup.tap_to_close;
        }
    }
}