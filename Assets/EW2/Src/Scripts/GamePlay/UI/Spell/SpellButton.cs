using EW2.Spell;
using TigerForge;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zitga.ContextSystem;
using Zitga.Update;

namespace EW2
{
    public class SpellButton : MonoBehaviour, IPointerClickHandler, IUpdateSystem, UIGameplay
    {
        [SerializeField] private GameObject selectedSpell;
        [SerializeField] private Image spellIcon;
        [SerializeField] private Image imgCooldown;
        [SerializeField] private Image heroIcon;

        private float countTime;
        private bool isReadyToUse;

        public static int CurrentSpellId { get; private set; }
        public HeroItem Hero { get; private set; }
        public SpellUnitBase SpellUnit { get; private set; }

        #region MonoBehaviorMethod

        public void Start()
        {
            isReadyToUse = true;
            UpdateCooldown();
            Context.Current.GetService<GlobalUpdateSystem>().Add(this);
        }

        public void OnDestroy()
        {
            if (Context.Current != null)
                Context.Current.GetService<GlobalUpdateSystem>().Remove(this);
        }

        #endregion

        // private void Awake()
        // {
        //     isReadyToUse = true;
        // }

        public void Repaint(HeroItem heroItem, SpellUnitBase spell)
        {
            Hero = heroItem;
            SpellUnit = spell;

            spellIcon.sprite = ResourceUtils.GetSpriteSpell($"{heroItem.spellId.ToString()}_0");
            selectedSpell.SetActive(false);
            countTime = SpellUnit.SpellStatBase.cooldown;
            if (heroIcon != null)
            {
                heroIcon.sprite = ResourceUtils.GetSpriteAtlas("hero_icons", $"hero_icon_info_{heroItem.heroId.ToString()}");
            }
        }


        private void UpdateCooldown()
        {
            if (SpellUnit != null)
            {
                imgCooldown.fillAmount = Mathf.Clamp01(countTime / SpellUnit.SpellStatBase.cooldown);
            }
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isReadyToUse)
            {
                return;
            }

            if (SpellUnit.SpellStatBase.useType == SpellUseType.Auto)
            {
                //Debug.LogAssertion($"AUTO USE SPELL {SpellUnit.SpellStatBase.spellId.ToString()}");
                imgCooldown.gameObject.SetActive(true);
                GamePlayController.Instance.TotalUseSpellInWave++;
                GamePlayController.Instance.CountUseSpell(SpellUnit.SpellStatBase.spellId);
            }
            else
            {
                if (CurrentSpellId.Equals(Hero.spellId))
                {
                    //Debug.LogAssertion($"CANCEL SPELL {SpellUnit.SpellStatBase.spellId}");
                    GamePlayUIManager.Instance.CloseCurrentUI(true);
                    CurrentSpellId = -1;
                }
                else
                {
                    CurrentSpellId = Hero.spellId;
                    //Debug.LogAssertion($"MANUALLY USE SPELL {SpellUnit.SpellStatBase.spellId}");
                    GamePlayUIManager.Instance.TryOpenUI(this);
                }
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (isReadyToUse)
            {
                return;
            }

            countTime -= deltaTime;

            if (countTime <= 0)
            {
                isReadyToUse = true;

                imgCooldown.fillAmount = 1;

                imgCooldown.gameObject.SetActive(false);
            }

            UpdateCooldown();
        }

        public void Open()
        {
            selectedSpell.SetActive(true);

            EventManager.StartListening(GamePlayEvent.OnSpellSelectTarget, ExecuteSpell);
        }

        public void Close()
        {
            selectedSpell.SetActive(false);
            EventManager.StopListening(GamePlayEvent.OnSpellSelectTarget, ExecuteSpell);
        }

        public UI_STATE GetUIType()
        {
            return UI_STATE.ActiveSpell;
        }

        private void ExecuteSpell()
        {
            //Debug.LogAssertion($"Spell {SpellUnit.SpellStatBase.spellId} executed");
            Vector3 position = EventManager.GetData<Vector3>(GamePlayEvent.OnSpellSelectTarget);
            ActiveSelectedHeroSkillToTarget(position);
        }

        public void ActiveSelectedHeroSkillToTarget(Vector3 position)
        {
            //check if spell button is selected
            if (CurrentSpellId == Hero.spellId)
            {
                SpellUnit.ActiveSkillToTarget(position, () =>
                {
                    GamePlayUIManager.Instance.CloseCurrentUI(true);

                    countTime = SpellUnit.SpellStatBase.cooldown;

                    imgCooldown.gameObject.SetActive(true);
                    GamePlayController.Instance.TotalUseSpellInWave++;
                    GamePlayController.Instance.CountUseSpell(SpellUnit.SpellStatBase.spellId);
                    CurrentSpellId = -1;
                    //Debug.LogAssertion($"Spell {Hero.spellId} Fired");
                    isReadyToUse = false;
                });
            }
        }
    }
}