using EW2;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

public class MoneyUI : RewardUI
{
    private Reward data;

    [SerializeField] private Image icon;

    [SerializeField] private Text txtValue;

    [SerializeField] private Image border;

    [SerializeField] private Image bgr;

    public override void SetData<T>(T data)
    {
        this.data = data;

        UpdateUi();
    }

    protected override void UpdateUi()
    {
        icon.sprite = ResourceUtils.GetIconMoneyReward(data.id);

        icon.SetNativeSize();

        border.sprite = ResourceUtils.GetBorderMoney(data.id);
        bgr.sprite = ResourceUtils.GetBgrMoney(data.id);

        txtValue.text = data.number.ToString();

        txtValue.gameObject.SetActive(data.number > 0);
    }

    protected override void ItemClick()
    {
        var data = new ItemInfoWindowProperties(this.data.type, this.data.id, this.data.itemType, this.data.number);
        UIFrame.Instance.OpenWindow(ScreenIds.item_info, data);
    }
}