using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.Events;

namespace EW2.Spell
{
    public class Spell4006ExplosionImpact : SpellDamageImpactBase
    {
        public UnityAction OnRemoveBomb { get; set; }
        public TriggerBombCollider triggerBombCollider;
        
        //private SpellStatBase stat;
        
        public void SpellData(SpellStatBase spellStatBase, Unit unit)
        {
            owner = unit;
            //stat = spellStatBase;
            damageBoxAOE.enabled = false;
        }

        public async void Explosion(Vector3 spawnPosition, float radius)
        {
            var go = ResourceUtils.GetSpellUnit("4006_magical_landmine_impact", spawnPosition, Quaternion.identity);
            var scale = radius / 0.5f;
            go.transform.localScale = new Vector3(scale, scale);
            
            //sfx
            var audioClip = ResourceUtils.LoadSound(SoundConstant.SPELL_4006_EXPLOSION);
            EazySoundManager.PlaySound(audioClip);
            await UniTask.Delay(200);
            Trigger(0.25f, 0f);
            
            await UniTask.Delay(200);
            var crack = ResourceUtils.GetSpellUnit("4006_magical_landmine_crack", spawnPosition, Quaternion.identity);
            crack.transform.localScale = new Vector3(scale, scale);
            OnRemoveBomb?.Invoke();
        }

        public override DamageInfo GetDamage(Unit target)
        {
            var damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = owner.DamageType,

                value = this.owner.Stats.GetStat<Damage>(RPGStatType.Damage).StatValue//stat.damage
            };

            return damageInfo;
        }
    }
}