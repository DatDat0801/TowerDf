using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EW2;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

public class HeroEndBattle : MonoBehaviour
{
    [SerializeField] private Image heroAvatar;
    [SerializeField] private Image expBar;
    [SerializeField] private Image runBar;
    
    [SerializeField] private Text lbLevel;
    [SerializeField] private GameObject upArrow;
    
    [SerializeField] private GameObject badgeLevel;

    [SerializeField] private GameObject upEffect;
    
    private readonly Vector3 scaleUp = new Vector3(1.2f, 1.2f, 1.2f);
    private readonly Vector3 scaleDown = new Vector3(1f, 1f, 1f);
    
    public void Init(int heroId, bool hasChangeExp , float originExpPercentage = 0, float currentExpPercentage = 0 , int numberLevelUp = 0)
    {
        
        
        var hero = UserData.Instance.UserHeroData.GetHeroById(heroId);
        gameObject.SetActive(true);
        heroAvatar.sprite = ResourceUtils.GetSpriteHeroIcon($"hero_icon_{hero.heroId}");
        lbLevel.text = (hero.level - numberLevelUp).ToString();
        
        GetComponent<DOTweenAnimation>().DORestart();

        if (upEffect != null)
        {
            upEffect.SetActive(false);
        }
        
        if (!hasChangeExp)
        {
            var heroDataBase = GameContainer.Instance.GetHeroData(heroId);
            
            
            if (!hero.IsLevelMax())
            {
                expBar.fillAmount = hero.exp / heroDataBase.stats[hero.level].maxExp;
                expBar.fillAmount = hero.exp / heroDataBase.stats[hero.level].maxExp;
            }
            else
            {
                runBar.fillAmount = 1;
                runBar.fillAmount = 1;
            }
        }
        else
        {
            runBar.fillAmount = originExpPercentage;
            expBar.fillAmount = originExpPercentage;
            StartCoroutine(AddExpCoroutine(originExpPercentage, currentExpPercentage, numberLevelUp));
        }
    }
    
    
    IEnumerator AddExpCoroutine(float originExpPercentage, float currentExpPercentage, int numberLevelUp)
    {
        upArrow.SetActive(numberLevelUp > 0);
        if (numberLevelUp <= 0)
        {
            runBar.fillAmount = currentExpPercentage;
        }
        else
        {
            runBar.fillAmount = 1;
        }

        yield return new WaitForSecondsRealtime(2f);
        
        if (numberLevelUp <= 0)
        {
            for (float i = originExpPercentage; i <= currentExpPercentage; i+= 0.01f)
            {
                expBar.fillAmount = i < 1 ? i : 1;
                yield return null;
                yield return null;
            }
        }
        else
        {
            int count = numberLevelUp;
            float origin = originExpPercentage;
            float current = 1;
            while (count >= 0)
            {
                if (upEffect != null)
                {
                    upEffect.SetActive(false);
                }
                runBar.fillAmount = current;

                for (float i = origin; i <= current; i += 0.01f)
                {
                    
                    expBar.fillAmount = i < 1 ? i : 1;
                    yield return null;
                    yield return null;
                }

                if (count > 0)
                {
                    if (upEffect != null)
                    {
                        upEffect.SetActive(true);
                    }
                   
                    lbLevel.text = (int.Parse(lbLevel.text) + 1).ToString();
                    badgeLevel.transform.transform.DOPunchScale(scaleUp, 0.4f).OnComplete(() => { transform.localScale = scaleDown; }).SetUpdate(true);;
                    yield return new WaitForSecondsRealtime(0.5f);
                }
                
                count--;
                origin = 0;
                current = count <= 0 ? currentExpPercentage : 1;
                if (count <= 0)
                {
                    upArrow.SetActive(false);
                }
            }
            
        }
    }
}