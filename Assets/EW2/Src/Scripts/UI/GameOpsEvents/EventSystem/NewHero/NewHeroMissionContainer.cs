using System.Collections;
using System.Collections.Generic;
using EW2;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

public class NewHeroMissionContainer : TabContainer
{
    [SerializeField] private ItemMissionNewHeroContainer questContainer;
    [SerializeField] private Text txtTitle;
    [SerializeField] private Text txtSlogan;
    [SerializeField] private Button btnInfo;
    
    private void Awake()
    {
        btnInfo.onClick.AddListener(OnInfoClick);
    }
    
    void OnInfoClick()
    {
        var properties = new PopupInfoWindowProperties(L.popup.notice_txt,L.game_event.hero_mission_rule);
        UIFrame.Instance.OpenWindow(ScreenIds.popup_info, properties);
    }

    public override void ShowContainer()
    {
        txtTitle.text = L.game_event.nhero_challenge_title_txt.ToUpper();
        txtSlogan.text = L.game_event.nhero_mission_slogan_txt;

        gameObject.SetActive(true);

        var listQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<NewHeroEventQuest>().GetAllQuests();
        if (listQuest != null)
        {
            questContainer.ShowQuest(listQuest);
            questContainer.RefreshTab(RefreshTab);
        }
    }

    private void RefreshTab()
    {
        var listQuest = GameContainer.Instance.Get<QuestManager>().GetQuest<NewHeroEventQuest>().GetAllQuests();
        questContainer.ShowQuest(listQuest);
    }

    public override void HideContainer()
    {
        gameObject.SetActive(false);
    }
}