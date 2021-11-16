using System;
using System.Collections;
using System.Collections.Generic;
using Hellmade.Sound;
using Invoke;
using Lean.Pool;
using TigerForge;
using UnityEngine;
using Zitga.ContextSystem;
using Zitga.Update;

namespace EW2
{
    public class GroupCavalryController : Unit
    {
        [SerializeField] private Cavalry6050Controller[] listCavalrys;

        [SerializeField] private Transform posTarget;

        [SerializeField] private Collider2D rangeCollider;

        private Vector3 posStart, posEnd;

        private Cavalry6050Data.DataCavalry dataCavalry;

        public Cavalry6050Data.DataCavalry DataCavalry => dataCavalry;

        private bool isActive, isReady;

        private GameObject fxReady;

        private List<GameObject> listArrow = new List<GameObject>();

        //sound
        private int readySound;
        private int attackSound;

        private float lastReadySoundPlayed;

        public override RPGStatCollection Stats
        {
            get
            {
                if (stats == null)
                {
                    stats = new CavalryStat(this, dataCavalry);
                }

                return stats;
            }
        }

        private void Start()
        {
            posStart = posEnd = transform.position;

            GetData();

            InitAction();

            InitCavalry();
        }

        public override void OnEnable()
        {
            EventManager.StartListening(GamePlayEvent.OnStartGame, Cooldown);

            if (Context.Current.GetService<GlobalUpdateSystem>() != null)
            {
                Context.Current.GetService<GlobalUpdateSystem>().Add(this);
            }
        }

        public override void OnDisable()
        {
            EventManager.StopListening(GamePlayEvent.OnStartGame, Cooldown);

            if (Context.Current != null)
                Context.Current.GetService<GlobalUpdateSystem>().Remove(this);

            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this);
        }

        private void InitCavalry()
        {
            for (var i = 0; i < listCavalrys.Length; i++)
            {
                listCavalrys[i].InitCavalry(i);
            }
        }

        private void CavalryIdle()
        {
            rangeCollider.enabled = false;

            for (var i = 0; i < listCavalrys.Length; i++)
            {
                listCavalrys[i].SetIdle();
            }

            //Sfx move
            var myAudio = EazySoundManager.GetAudio(attackSound);
            if (myAudio != null)
            {
                myAudio.Stop();
            }
        }

        // void DoPlayDelaySound()
        // {
        //     
        //     if (lastReadySoundPlayed >= 7f || lastReadySoundPlayed.Equals(0f))
        //     {
        //         var audioClip = ResourceUtils.LoadSound(SoundConstant.CAVALRY_READY);
        //         readySound = EazySoundManager.PlaySound(audioClip, false);
        //         lastReadySoundPlayed = Time.time;
        //     }
        // }

        private void CavalryReady()
        {
            isReady = true;

            //Sfx
            var attackAudio = EazySoundManager.GetAudio(attackSound);
            if (attackAudio != null)
            {
                attackAudio.Stop();
            }

            var audioClip = ResourceUtils.LoadSound(SoundConstant.CAVALRY_READY);
            readySound = EazySoundManager.PlaySound(audioClip, false);
            //DoPlayDelaySound();
            if (Vector3.Distance(posStart, posEnd) <= 0)
            {
                posEnd = posTarget.position;
            }
            else
            {
                posEnd = posStart;
            }

            for (var i = 0; i < listCavalrys.Length; i++)
            {
                listCavalrys[i].SetReady();
            }

            ShowFxReady();
        }

        private void CavalryMove()
        {
            rangeCollider.enabled = true;

            for (var i = 0; i < listCavalrys.Length; i++)
            {
                listCavalrys[i].SetMove();
            }
        }

        public void OnCavalryClick()
        {
            if (isActive) return;

            if (!isActive && isReady)
            {
                GamePlayController.Instance.TotalUseEnv++;
                
                PlayMoveSfx();
                CavalryMove();

                RemoveArrowLine();

                isActive = true;
            }
        }

        private void GetData()
        {
            var cavalry = GameContainer.Instance.Get<UnitDataBase>().Get<Cavalry6050Data>();

            dataCavalry = cavalry.GetDataCavalry();
        }

        private void Cooldown()
        {
            InvokeProxy.Iinvoke.CancelInvoke(this, CavalryReady);

            InvokeProxy.Iinvoke.Invoke(this, CavalryReady, dataCavalry.timeCooldown);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (isActive)
            {
                float step = Stats.GetStat(RPGStatType.MoveSpeed).StatValue * Time.deltaTime;

                float distance = Vector3.Distance(transform.position, posEnd);

                if (distance > step)
                {
                    var position = transform.position;

                    transform.position = Vector3.MoveTowards(position, posEnd, step);
                }
                else
                {
                    transform.position = posEnd;

                    isActive = false;

                    isReady = false;

                    var scale = transform.localScale;

                    scale.x *= -1;

                    transform.localScale = scale;

                    CavalryIdle();

                    Cooldown();
                }
            }
        }

        void PlayMoveSfx()
        {
            //Sfx move
            var readyAudio = EazySoundManager.GetAudio(readySound);
            if (readyAudio != null)
            {
                readyAudio.Stop();
            }

            var audioClip = ResourceUtils.LoadSound(SoundConstant.CAVALRY_ATTACK);
            attackSound = EazySoundManager.PlaySound(audioClip, true);
        }

        public override void Remove()
        {
        }

        protected override void InitAction()
        {
            isActive = false;

            isReady = false;
        }

        private void ShowFxReady()
        {
            if (fxReady)
            {
                fxReady.SetActive(true);
            }
            else
            {
                fxReady = ResourceUtils.GetVfx("Assets", "6050_ready", Vector3.zero, Quaternion.identity,
                    this.transform);
            }

            InstanceArrow();
        }

        private void RemoveArrowLine()
        {
            foreach (var arrow in listArrow)
            {
                arrow.SetActive(false);
            }

            if (fxReady)
            {
                fxReady.SetActive(false);
            }
        }

        private void InstanceArrow()
        {
            var angle = CaculateAngle();

            var xScale = Vector3.Distance(transform.position, posEnd);

            if (listArrow.Count <= 0)
            {
                var listPosInstance = new List<Vector3>();

                var fracJourney = 0.2f;

                for (int i = 0; i < 30; i++)
                {
                    var checkPoint = Vector2.Lerp(posStart, posEnd, fracJourney);

                    if (Vector3.Distance(posEnd, checkPoint) <= 0.3f) break;

                    listPosInstance.Add(checkPoint);

                    fracJourney += 0.15f;
                }

                CoroutineUtils.Instance.StartCoroutine(InstanceArrowDelay(listPosInstance, angle));
            }
            else
            {
                foreach (var arrow in listArrow)
                {
                    arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

                    arrow.SetActive(true);
                }
            }
        }

        private float CaculateAngle()
        {
            var angle = 30f;

            var dir = transform.position - posEnd;

            angle = (float) (Math.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

            if (angle < 0)
            {
                angle = 180 + angle;
            }
            else if (angle > 0)
            {
                angle = -180 + angle;
            }

            return angle;
        }

        private IEnumerator InstanceArrowDelay(List<Vector3> listPosInstance, float angle)
        {
            for (int i = 0; i < listPosInstance.Count; i++)
            {
                var go = ResourceUtils.GetVfx("Assets", "6050_line_arrow", listPosInstance[i], Quaternion.identity);

                go.transform.rotation = Quaternion.Euler(0, 0, angle);

                listArrow.Add(go);

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}