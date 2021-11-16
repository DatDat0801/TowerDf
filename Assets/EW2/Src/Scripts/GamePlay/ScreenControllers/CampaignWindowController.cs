using System;
using EW2;
using UnityEngine;
using UnityEngine.UI;

public class CampaignWindowController : MonoBehaviour
{
    [SerializeField] private Transform groupStage;

    [SerializeField] private GameObject fxCurrentStage, fxGlowStage;
    [SerializeField] private ScrollRect worldMapScrollRect;

    private void Awake()
    {
        worldMapScrollRect.normalizedPosition = Vector2.zero;
    }

    private void OnEnable()
    {
        if (fxCurrentStage)
            fxCurrentStage.SetActive(false);

        if (fxGlowStage)
            fxGlowStage.SetActive(false);

        SetDataMap();
    }

    private void SetDataMap()
    {
        var campaignData = UserData.Instance.CampaignData;
        
        for (int i = 0; i < groupStage.childCount; i++)
        {
            var stage = groupStage.GetChild(i).GetComponent<ButtonStageController>();

            var unlocked = i == 0 || campaignData.IsUnlockedStage(0, i);

            if (stage)
            {
                if (unlocked)
                {
                    stage.InitStage(i);

                    stage.gameObject.SetActive(true);

                    if (campaignData.GetStar(0,i) <= 0)
                    {
                        SetFxCurrentStage(stage.transform.position);
                    }
                }
                else
                {
                    stage.gameObject.SetActive(false);
                }
            }
        }
    }

    private void SetFxCurrentStage(Vector3 pos)
    {
        fxCurrentStage.transform.position = new Vector3(pos.x, pos.y + 0.2f, pos.z);

        fxGlowStage.transform.position = new Vector3(pos.x, pos.y + 0.2f, pos.z);

        fxGlowStage.SetActive(true);

        fxCurrentStage.SetActive(true);
    }
}