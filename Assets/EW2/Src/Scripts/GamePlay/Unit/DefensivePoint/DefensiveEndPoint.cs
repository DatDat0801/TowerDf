using Constants;
using UnityEngine;

namespace EW2
{
    public class DefensiveEndPoint : MonoBehaviour
    {
        [SerializeField] private DefensivePointBase _defensivePoint;
        private float _stricterBaseDamage;
        private float _growthDamage;

        private void OnEnable()
        {
            var heroDefendModeConfig = GameContainer.Instance.Get<DefendModeDataBase>().Get<HeroDefendModeConfig>()
                .GetDataConfig();
            this._stricterBaseDamage = heroDefendModeConfig.tricksterBaseDamage;
            var mapId = UserData.Instance.UserHeroDefenseData.currMapId;
            var defensiveModeData = GameContainer.Instance.GetMapDefensiveData(mapId);
            this._growthDamage = defensiveModeData.GetDefensiveEnemyConfig().tricksterDamage;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(TagConstants.Enemy) &&
                other.gameObject.layer == LayerMask.NameToLayer(LayerConstants.EnemyBodyBox))
            {
                BodyCollider bodySpineCollider = other.GetComponent<BodyCollider>();

                if (bodySpineCollider != null)
                {
                    var enemy = (EnemyBase)bodySpineCollider.Owner;
                    if (enemy.TraitType == TraitEnemyType.Trickster)
                    {
                        //enemy.IsCompleteEndPoint = true;
                        //GamePlayData.Instance.SubMoneyInGame(MoneyInGameType.LifePoint, enemy.EnemyData.life);
                        this._defensivePoint.GetHurt(new DamageInfo() {
                            creator = enemy,
                            target = this._defensivePoint,
                            value = this._stricterBaseDamage + (CallWave.Instance.CountLoop * this._growthDamage)
                        });

                        //bodySpineCollider.Owner.Remove();

                        enemy.CheckEnemyDie();
                        enemy.Remove();
                        if (UserData.Instance.SettingData.shake)
                            Handheld.Vibrate();
                    }
                }
            }
        }
    }
}