using Lean.Pool;
using Spine;
using UnityEngine;

public class DestroyMe : MonoBehaviour
{
    public bool isNeccessory;
    public bool ignoreTimeScale = false;
    public float deathtimer = 10;

    private float timer;

    private bool isEnable;

    private void OnEnable()
    {
        isEnable = true;
        timer = 0;
    }

    private void OnDisable()
    {
        isEnable = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isEnable)
        {
            return;
        }

        timer += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

        if (timer < deathtimer)
        {
            return;
        }

        Despawn();
    }

    private void Despawn()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        if (isNeccessory)
        {
            LeanPool.Despawn(gameObject);
            return;
        }
        Destroy(gameObject);
    }
}