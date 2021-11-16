using System;
using System.Collections;
using System.Collections.Generic;
using EW2;
using Hellmade.Sound;
using UnityEngine;

public class PoisonDumpController : MonoBehaviour
{
    [SerializeField] private PoisonDump dump;


    private void Start()
    {
        var poisonDump = GameContainer.Instance.Get<UnitDataBase>().Get<PoisonDump6051Data>();
        var poisonData = poisonDump.GetDataPoisonDump();
        dump.InitDump(new DataDump()
        {
            damage = poisonData.damage,
            damageType = poisonData.damageType,
            interval = poisonData.interval,
            duration = poisonData.duration,
            effectOnType = poisonData.effectOnType
        });
        PlaySFX();
    }

    private void PlaySFX()
    {
        var audioClip = ResourceUtils.LoadSound(SoundConstant.POISON);
        EazySoundManager.PlaySound(audioClip, true);
    }
}
