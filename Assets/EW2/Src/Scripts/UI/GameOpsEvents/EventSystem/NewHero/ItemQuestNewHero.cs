using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using EW2;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;

public class ItemQuestNewHero : EnhancedScrollerCellView
{
    [SerializeField] private Text txtTitle;
    [SerializeField] private Text txtProgress;
    [SerializeField] private Text txtClaimed;
    [SerializeField] private Button btnClaim;
    [SerializeField] private Button btnGoto;
    [SerializeField] private Image imgProgress;
    [SerializeField] private Transform panelReward;
    [SerializeField] private Sprite[] arrImgProgress;

    private QuestItem _questData;
    private Action _claimCallback;
    private GridReward _gridReward;

    private void Awake()
    {
        btnClaim.onClick.AddListener(ClaimCLick);
        btnGoto.onClick.AddListener(GoClick);
        _gridReward = new GridReward(panelReward);
    }

    private void GoClick()
    {
        this._questData.GoToTarget();
    }

    private void ClaimCLick()
    {
        if (!this._questData.IsCanReceive()) return;

        Reward.AddToUserData(this._questData.rewards, AnalyticsConstants.SourceNewHeroMissionEvent,
            this._questData.questId.ToString());
        PopupUtils.ShowReward(this._questData.rewards);
        this._questData.SetClaimed();
        this._claimCallback?.Invoke();
        EventManager.EmitEvent(GamePlayEvent.OnUpdateBadge);
    }

    public void InitData(QuestItem data, Action callback)
    {
        this._questData = data;
        this._claimCallback = callback;
        ShowUi();
    }

    private void ShowUi()
    {
        txtTitle.text = GetNameQuest();
        btnClaim.GetComponentInChildren<Text>().text = L.button.btn_claim;
        btnGoto.GetComponentInChildren<Text>().text = L.button.go_to_btn;
        txtClaimed.text = L.button.reward_video_received;

        var ratio = this._questData.questUserData.count * 1f /
            this._questData.questLocalData.infoQuests[this._questData.currLevel].numberRequire * 1f;
        imgProgress.fillAmount = ratio;

        if (ratio < 1f)
        {
            txtProgress.text =
                $"<color='{GameConfig.TextColorRed}'>{this._questData.questUserData.count}</color>/{this._questData.questLocalData.infoQuests[this._questData.currLevel].numberRequire}";
            imgProgress.sprite = arrImgProgress[0];
        }
        else
        {
            txtProgress.text =
                $"{this._questData.questUserData.count}/{this._questData.questLocalData.infoQuests[this._questData.currLevel].numberRequire}";
            imgProgress.sprite = arrImgProgress[1];
        }

        btnClaim.gameObject.SetActive(this._questData.IsCanReceive());

        if (this._questData.questLocalData.haveGoto <= 0)
            btnGoto.gameObject.SetActive(false);
        else
            btnGoto.gameObject.SetActive(!this._questData.IsComplete());

        txtClaimed.gameObject.SetActive(this._questData.IsReceived());

        this._gridReward.ReturnPool();
        this._gridReward.SetData(this._questData.rewards);
    }

    private string GetNameQuest()
    {
        if (this._questData.targetType == TargetType.Hero)
        {
            var nameHero = Ultilities.GetNameHero(this._questData.victim[0]);
            return string.Format(L.game_event.hero_required_formation, nameHero);
        }
        else if (this._questData.targetType == TargetType.Complete3StarNormal)
        {
            return string.Format(L.game_event.event_quest_pf5_txt, this._questData.victim[0] + 1);
        }
        else if (this._questData.targetType == TargetType.Complete3StarNormalWithHero)
        {
            var nameHero = Ultilities.GetNameHero(this._questData.victim[1]);
            return string.Format(L.game_event.event_quest_nhero_pf5_txt, this._questData.victim[0] + 1, nameHero);
        }
        else if (this._questData.targetType == TargetType.Complete3StarNightmare)
        {
            return string.Format(L.game_event.event_quest_pf5m_txt, this._questData.victim[0] + 1);
        }
        else if (this._questData.targetType == TargetType.Complete3StarNightmareWithHero)
        {
            var nameHero = Ultilities.GetNameHero(this._questData.victim[1]);
            return string.Format(L.game_event.event_quest_nhero_pf10m_txt, this._questData.victim[0] + 1, nameHero);
        }
        else if (this._questData.targetType == TargetType.UseSkillHero)
        {
            var nameHero = Ultilities.GetNameHero(this._questData.victim[0]);
            var nameSkillHero = Ultilities.GetHeroSkillName(this._questData.victim[0], 0);
            return string.Format(L.game_event.event_quest_nhero_pf20m_txt, nameSkillHero, nameHero,
                this._questData.questLocalData.infoQuests[0].numberRequire);
        }
        else if (this._questData.targetType == TargetType.UpgradeHero)
        {
            var nameHero = Ultilities.GetNameHero(this._questData.victim[1]);
            return string.Format(L.game_event.event_quest_nhero_lv5_txt, nameHero, this._questData.victim[0]);
        }
        else if (this._questData.targetType == TargetType.EquipRuneForHero)
        {
            var nameHero = Ultilities.GetNameHero(this._questData.victim[0]);
            return string.Format(L.game_event.event_quest_nhero_rune_txt, nameHero);
        }
        else if (this._questData.targetType == TargetType.EquipSpellForHero)
        {
            var nameHero = Ultilities.GetNameHero(this._questData.victim[0]);
            return string.Format(L.game_event.event_quest_nhero_spell_txt, nameHero);
        }

        return "Unavailable";
    }
}