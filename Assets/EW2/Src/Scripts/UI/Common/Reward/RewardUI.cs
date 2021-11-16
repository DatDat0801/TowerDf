using System;
using EW2;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class RewardUI : MonoBehaviour
{
    public Button buttonClick;
    public abstract void SetData<T>(T data) where T : Reward;

    protected abstract void UpdateUi();

    private void Awake()
    {
        buttonClick.onClick.AddListener(ItemClick);
    }

    protected abstract void ItemClick();

    public virtual void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        
        transform.localScale = Vector3.one;
        
        transform.localPosition = Vector3.zero;
    }

    public virtual void ReturnPool()
    {
        LeanPool.Despawn(gameObject);
    }
}
