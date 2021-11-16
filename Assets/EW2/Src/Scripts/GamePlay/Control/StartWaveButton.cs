using System;
using System.Collections.Generic;
using DG.Tweening;
using EW2.Tutorial.General;
using Invoke;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Update;
using Zitga.ContextSystem;

namespace EW2
{
    public class StartWaveButton : MonoBehaviour, IUpdateSystem, UIGameplay
    {
        public delegate void PressButton(StartWaveButton btn);

        public static event PressButton onStartButtonConfirmed;

        public static event PressButton onStartButtonUnconfirmed;

        [SerializeField] private Sprite[] listIconWave;

        [SerializeField] private Button btnCallWave;

        [SerializeField] private Transform pointer;

        [SerializeField] private Image nextWaveTimer;

        [SerializeField] private Image iconTick;

        [SerializeField] private Image myIcon;

        private PointCallSpawn target;

        private Rect screenRect;

        private bool isConfirm;

        private float timeCooldown;

        private Dictionary<int, int> dictEnemyInLane = new Dictionary<int, int>();

        public Dictionary<int, int> EnemyInLane => dictEnemyInLane;

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

        private void Awake()
        {
            isConfirm = false;

            btnCallWave.onClick.AddListener(OnClick);
        }

        public void InitButton(PointCallSpawn pointSpawnEnemy)
        {
            this.target = pointSpawnEnemy;

            UpdatePosition();

            InitInfoWave();
        }

        public void ShowNextWave()
        {
            InitInfoWave();

            var duration = GameConfig.TimeDelayStartWave - GameConfig.TimeDelayShowCallWave;

            timeCooldown = duration;

            nextWaveTimer.fillAmount = 0;

            nextWaveTimer.DOKill();

            myIcon.color = Color.white;

            if (duration > 0)
            {
                nextWaveTimer.gameObject.SetActive(true);

                nextWaveTimer.DOFillAmount(1, duration).OnComplete(HideButton);

                InvokeProxy.Iinvoke.InvokeRepeating(this, Cooldown, 1f, 1f);
            }
        }

        private void CallEnemyEarly()
        {
            GamePlayControllerBase.Instance.CallEarlyWave();

            var goldReceive = (int)timeCooldown * 3;

            GamePlayData.Instance.AddMoneyInGame(MoneyInGameType.Gold, goldReceive);

            InvokeProxy.Iinvoke.CancelInvoke(this, Cooldown);

            HideButton();
        }

        public void HideButton()
        {
            nextWaveTimer.gameObject.SetActive(false);

            Close();

            gameObject.SetActive(false);
        }

        private void Cooldown()
        {
            timeCooldown--;

            if (timeCooldown <= 0)
                InvokeProxy.Iinvoke.CancelInvoke(this, Cooldown);
        }

        private void UpdatePosition()
        {
            var isInside = true;

            screenRect = new Rect(0f, 0f, Screen.width, Screen.height);

            var myCamera = GamePlayControllerBase.Instance.GetCameraController().MyCamera;

            var xMaxScreen = myCamera.ScreenToWorldPoint(new Vector2(screenRect.max.x, 0));

            var xMinScreen = myCamera.ScreenToWorldPoint(new Vector2(screenRect.min.x, 0));

            var yMaxScreen = myCamera.ScreenToWorldPoint(new Vector2(0, screenRect.max.y - 35f));

            var yMinScreen = myCamera.ScreenToWorldPoint(new Vector2(0, screenRect.min.y + 35f));

            var posTarget = this.target.GetPointSpawnButton();


            if (posTarget.x <= xMinScreen.x)
            {
                isInside = false;

                posTarget.x = xMinScreen.x + 1f;
            }

            if (posTarget.x >= xMaxScreen.x)
            {
                isInside = false;

                posTarget.x = xMaxScreen.x - 1f;
            }

            if (posTarget.y >= yMaxScreen.y)
            {
                isInside = false;

                posTarget.y = yMaxScreen.y - 1f;
            }

            if (posTarget.y <= yMinScreen.y)
            {
                isInside = false;

                posTarget.y = yMinScreen.y + 1f;
            }

            transform.position = posTarget;

            if (isInside)
            {
                if (this.target.location == Location.Top)
                {
                    var pos = transform.localPosition;
                    pos.x -= 30;
                    pos.y -= 30;

                    transform.localPosition = pos;
                }
                else if (this.target.location == Location.Bottom)
                {
                    var pos = transform.localPosition;
                    pos.x += 30;
                    pos.y += 30;

                    transform.localPosition = pos;
                }
            }
        }

        private void InitInfoWave()
        {
            dictEnemyInLane = CallWave.Instance.GetListEnemyInLane(target.laneId);

            if (dictEnemyInLane.Count <= 0)
            {
                HideButton();
            }
            else
            {
                gameObject.SetActive(true);
            }

            ShowIcon();
        }

        private void ShowIcon()
        {
            var hasBoss = false;

            var hasEnemyFly = false;

            foreach (var enemy in dictEnemyInLane)
            {
                if (GameConfig.ListBossIds.Contains(enemy.Key))
                {
                    hasBoss = true;
                    break;
                }

                // hardcore to check stat type of data
                var unitDatabase = GameContainer.Instance.Get<UnitDataBase>();
                var enemyData = unitDatabase.GetEnemyById(enemy.Key);
                var enemyStat = enemyData.GetStats(1);

                if (enemyStat != null && enemyStat.moveType == MoveType.Fly)
                {
                    hasEnemyFly = true;
                    break;
                }
            }

            if (hasBoss)
            {
                this.myIcon.sprite = listIconWave[2];

                this.myIcon.transform.localScale = new Vector3(1.1f, 1.1f);

                this.nextWaveTimer.color = new Color(0.8f, 0.24f, 0.15f);
            }
            else if (hasEnemyFly)
            {
                this.myIcon.sprite = listIconWave[1];

                this.myIcon.transform.localScale = Vector3.one;

                this.nextWaveTimer.color = new Color(0.97f, 0.47f, 0.19f);
            }
            else
            {
                this.myIcon.sprite = listIconWave[0];

                this.myIcon.transform.localScale = Vector3.one;

                this.nextWaveTimer.color = new Color(0.97f, 0.47f, 0.19f);
            }

            this.myIcon.SetNativeSize();
        }

        public void OnUpdate(float deltaTime)
        {
            if (target)
            {
                Vector3 direction = target.transform.position - transform.position;

                float angle = (float)Math.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                pointer.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        public void OnClick()
        {
            if (!isConfirm)
            {
                if ((this.timeCooldown <= 0 && (GamePlayControllerBase.gameMode == GameMode.DefenseMode )) ||
                    GamePlayControllerBase.gameMode == GameMode.CampaignMode || GamePlayControllerBase.gameMode == GameMode.TournamentMode)
                {
                    GamePlayUIManager.Instance.TryOpenUI(this);

                    isConfirm = true;
                }
            }
            else
            {
                if (GamePlayControllerBase.gameMode == GameMode.CampaignMode || GamePlayControllerBase.gameMode == GameMode.TournamentMode)
                {
                    CallEnemyEarly();

                    EventManager.EmitEvent(GamePlayEvent.OnConfirmCallWave);
                    TutorialManager.Instance.CompleteCurrentTutorialFollowId(AnyTutorialConstants.FOCUS_SPAWN_WAVE);
                }
                else if (GamePlayControllerBase.gameMode == GameMode.DefenseMode)
                {
                    if (this.timeCooldown <= 0)
                    {
                        HideButton();
                        EventManager.EmitEvent(GamePlayEvent.OnConfirmCallWave);
                    }
                }
            }
        }

        public void Open()
        {
            iconTick.gameObject.SetActive(true);

            myIcon.gameObject.SetActive(false);

            onStartButtonConfirmed?.Invoke(this);
        }

        public void Close()
        {
            if (isConfirm)
                onStartButtonUnconfirmed?.Invoke(this);


            isConfirm = false;

            iconTick.gameObject.SetActive(false);

            myIcon.gameObject.SetActive(true);
        }

        public UI_STATE GetUIType()
        {
            return UI_STATE.Soft;
        }
    }
}