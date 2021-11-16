using System.Collections;
using System.Collections.Generic;
using EW2;
using EW2.Events;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

public class NewHeroBackpackContainer : TabContainer
{
    [SerializeField] private List<PanelItemBackpackUi> panelItemBackpackUis;
    [SerializeField] private Text txtTitle;
    [SerializeField] private Button btnInfo;

    private List<ShopLitmitedItemData> _datasBundle;

    private void Awake()
    {
        btnInfo.onClick.AddListener(OnInfoClick);
    }

    void OnInfoClick()
    {
        var properties = new PopupInfoWindowProperties(L.popup.notice_txt, L.game_event.hero_mission_rule);
        UIFrame.Instance.OpenWindow(ScreenIds.popup_info, properties);
    }

    private void GetDataBundle()
    {
        if (this._datasBundle == null)
        {
            this._datasBundle = new List<ShopLitmitedItemData>();
            var database = GameContainer.Instance.Get<EventDatabase>().Get<NewHeroEventBundle>();
            if (database)
            {
                for (int i = 2; i < database.shopLitmitItemDatas.Length; i++)
                {
                    this._datasBundle.Add(database.shopLitmitItemDatas[i]);
                }
            }
        }
    }


    public override void ShowContainer()
    {
        gameObject.SetActive(true);
        GetDataBundle();
        ShowUi();
    }

    public override void HideContainer()
    {
        gameObject.SetActive(false);
    }

    private void ShowUi()
    {
        this.txtTitle.text = L.game_event.hero_backpack_txt.ToUpper();

        for (int i = 0; i < this.panelItemBackpackUis.Count; i++)
        {
            if (i < this._datasBundle.Count)
            {
                this.panelItemBackpackUis[i].InitData(i, this._datasBundle[i]);
            }
        }
    }
}