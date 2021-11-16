using System.Collections.Generic;
using Coffee.UIEffects;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using EW2.Spell;
using Lean.Pool;
using Sirenix.OdinInspector;
using Spine.Unity;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class SpellBarController : MonoBehaviour
    {
        [SerializeField] private GameObject effect;
        [SerializeField] private Image openBook;
        [SerializeField] private Image closeBook;
        [SerializeField] private Image lockIcon;
        [SerializeField] private GameObject spellList;
        [SerializeField] private SpellButton spellButtonPrefab;

        [SerializeField] private Button bookBtn;

        //Fx 
        [SerializeField] private SkeletonAnimation lockOpenFx;
        [SerializeField] private SkeletonAnimation bookOpenFx;

        private int SpellCount { get; set; }

        //user can not interact with
        public bool IsLocked { get; set; }

        public bool IsClose { get; private set; } = true;

        //available at stage 12
        private bool IsAvailable { get; set; }

        #region MonobehaviorMethod

        private void Awake()
        {
            //init for book button
            bookBtn.onClick.AddListener(OnBookClick);
        }

        private void OnEnable()
        {
            EventManager.StartListening(GamePlayEvent.OnConfirmCallWave, OnConfirmCallWave);
        }

        private void OnDisable()
        {
            EventManager.StopListening(GamePlayEvent.OnConfirmCallWave, OnConfirmCallWave);
            CloseSpellBook(false);
        }

        #endregion

        private void OnConfirmCallWave()
        {
            if (IsLocked && IsAvailable)
            {
                Unlock();
            }
        }


        public void Initialize()
        {
            var selectedHeroes = new List<HeroItem>();

            if (GamePlayControllerBase.gameMode == GameMode.CampaignMode)
            {
                var userHeroes = UserData.Instance.UserHeroData;
                selectedHeroes = userHeroes.GetSelectedHeroItems();
            }
            else if (GamePlayControllerBase.gameMode == GameMode.DefenseMode)
            {
                var userHeroes = UserData.Instance.UserHeroDefenseData;
                selectedHeroes = userHeroes.GetSelectedHeroItems();
            }else if (GamePlayControllerBase.gameMode == GameMode.TournamentMode)
            {
                var userHeroes = UserData.Instance.TournamentData;
                selectedHeroes = userHeroes.GetSelectedHeroItems();
                var banSpellId = UserData.Instance.TournamentData.spellBanId;
                for (var i = 0; i < selectedHeroes.Count; i++)
                {
                    if (selectedHeroes[i].spellId == banSpellId)
                    {
                        selectedHeroes[i].spellId = 0;
                        break;
                    }
                }
            }

            SpellCount = 0;

            foreach (Transform child in spellList.transform)
            {
                Destroy(child.gameObject);
            }

            for (var i = 0; i < selectedHeroes.Count; i++)
            {
                if (selectedHeroes[i].spellId > 0)
                {
                    SpellCount++;
                    var spellBtn = LeanPool.Spawn(spellButtonPrefab, spellList.transform);
                    var go = ResourceUtils.GetSpellUnit(
                        $"spell_{Ultilities.GetSpellIdFamily(selectedHeroes[i].spellId).ToString()}", Vector3.zero,
                        Quaternion.identity);
                    var spellUnitBase = go.GetComponent<SpellUnitBase>();
                    spellUnitBase.SetId(selectedHeroes[i].spellId);
                    spellUnitBase.InitSpellData();
                    var heroBase =
                        GamePlayController.Instance.SpawnController.GetHeroUnitById(selectedHeroes[i].heroId);
                    if (heroBase != null)
                    {
                        heroBase.Spell = spellUnitBase;
                        spellUnitBase.SetHero(heroBase);

                        spellBtn.Repaint(selectedHeroes[i], spellUnitBase);
                    }
                }
            }

            //lock on start game
            IsLocked = true;

            CloseSpellBook(IsLocked || !IsAvailable);

            if (IsAvailable)
            {
                ReadyFxToUnlock();
            }

            EventManager.EmitEvent(GamePlayEvent.ON_SPELL_INIT_READY);
        }

        private void OnBookClick()
        {
            if (!IsAvailable)
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti,
                    string.Format(L.popup.spell_unlock_condition,
                        UnlockFeatureUtilities.SPELL_AVAILABLE_AT_STAGE_ID + 1));
                return;
            }

            if (IsLocked)
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.start_battle_to_use);
                return;
            }

            if (SpellCount <= 0)
            {
                EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.popup.not_equip_spell);
                return;
            }

            if (IsClose)
            {
                OpenSpellBook();
            }
            else
            {
                CloseSpellBook(false);
            }
        }

        public void OpenSpellBook()
        {
            if (effect != null)
            {
                effect.SetActive(true);
            }

            if (openBook != null)
            {
                openBook.gameObject.SetActive(true);
            }

            if (closeBook != null)
            {
                closeBook.gameObject.SetActive(false);
            }

            if (spellList != null)
            {
                spellList.SetActive(true);
                //SetChildrenActive(true);
            }

            IsClose = false;
        }


        public void CloseSpellBook(bool locked)
        {
            IsLocked = locked;
            //lock     true
            //available false

            if (locked == true || !IsAvailable)
            {
                lockIcon.enabled = true;
            }
            else
            {
                lockIcon.enabled = false;
            }

            if (effect != null)
            {
                effect.SetActive(false);
            }

            if (openBook != null)
            {
                openBook.gameObject.SetActive(false);
            }

            if (closeBook != null)
            {
                closeBook.gameObject.SetActive(true);
            }

            if (spellList != null)
            {
                spellList.SetActive(false);
                //SetChildrenActive(false);
            }


            if (UnlockFeatureUtilities.IsSpellAvailable())
            {
                IsAvailable = true;
                closeBook.GetComponent<UIEffect>().enabled = false;
            }
            else
            {
                closeBook.GetComponent<UIEffect>().enabled = true;
            }

            IsClose = true;
        }

        protected virtual void SetChildrenActive(bool active)
        {
            foreach (Transform t in spellList.transform)
            {
                //t.gameObject.SetActive(active);
                var graphics = t.GetComponents<UnityEngine.UI.Graphic>();
                foreach (var graphic in graphics)
                {
                    graphic.enabled = active;
                }
            }

            // var img = spellList.GetComponents<UnityEngine.UI.Graphic>();
            // foreach (var graphic in img)
            // {
            //     graphic.enabled = active;
            // }
        }

        public async void Unlock()
        {
            foreach (Transform t in lockIcon.transform)
            {
                LeanPool.Despawn(t.gameObject);
            }

            lockIcon.enabled = false;
            closeBook.gameObject.SetActive(false);
            bookBtn.interactable = false;
            var lockAnimation = LeanPool.Spawn(lockOpenFx, lockIcon.transform);
            var bookAnimation = LeanPool.Spawn(bookOpenFx, lockIcon.transform);
            lockAnimation.AnimationState.SetAnimation(0, "unlock", false);
            bookAnimation.AnimationState.SetAnimation(0, "unlock", true);
            await UniTask.DelayFrame(15);
            var fx = ResourceUtils.GetVfx("UI", "fx_ui_unlock_book", Vector3.zero, Quaternion.identity,
                lockIcon.transform);

            await UniTask.Delay(1000);

            LeanPool.Despawn(lockAnimation);
            LeanPool.Despawn(bookAnimation);
            LeanPool.Despawn(fx);
            closeBook.gameObject.SetActive(true);

            bookBtn.interactable = true;
            IsLocked = false;
            if (SpellCount > 0)
                OnBookClick();
            //bookAnimation.AnimationState.Complete += entry => { closeBook.gameObject.SetActive(true);};fx_ui_unlock_book 15
        }


        public void ReadyFxToUnlock()
        {
            foreach (Transform t in lockIcon.transform)
            {
                Destroy(t.gameObject);
            }

            ResourceUtils.GetVfx("UI", "fx_ui_pre_unlock_book", Vector3.zero, Quaternion.identity,
                lockIcon.transform);
        }

        [Button]
        private void AddFakeSpellForTest()
        {
            SpellCount = 1;
        }
    }
}