using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class Tower2003SkillSlowTrigger : MonoBehaviour
    {
        //private TowerData2003.Skill2 dataSkill;
        [SerializeField] private Collider2D col;
        private List<EnemyBase> listEnemyInRange = new List<EnemyBase>();

        private Unit owner;
        private float lifeTimeSlow;
        private float slowPercent;
        private ModifierType modifierType;

        public void InitTrigger(Unit shooter, float lifeTime, float slowPercent, ModifierType type)
        {
            listEnemyInRange.Clear();

            //this.dataSkill = dataSkill2;
            this.slowPercent = slowPercent;
            this.lifeTimeSlow = lifeTime;
            this.modifierType = type;

            this.owner = shooter;
            
            col.enabled = true;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var bodyCollider = other.GetComponent<BodyCollider>();
            if (bodyCollider != null)
            {
                var enemy = bodyCollider.Owner as EnemyBase;
                if (enemy != null && !listEnemyInRange.Contains(enemy) && enemy.IsAlive &&
                    enemy.MoveType != MoveType.Fly)
                {
                    var moveSpeedAttribute = enemy.Stats.GetStat<RPGAttribute>(RPGStatType.MoveSpeed);

                    var statusOverTimeConfig = new StatusOverTimeConfig()
                    {
                        creator = owner,
                        owner = enemy,
                        lifeTime = lifeTimeSlow,
                        intervalTime = lifeTimeSlow,
                        statusType = StatusType.Slow
                    };

                    var modifierMoveSpeed = new RPGStatModifier(moveSpeedAttribute, modifierType,
                        - this.slowPercent, false, this.owner, enemy);

                    var modifierMoveSpeedOverTime = new ModifierStatOverTime(statusOverTimeConfig, modifierMoveSpeed);

                    enemy.StatusController.AddStatus(modifierMoveSpeedOverTime);

                    listEnemyInRange.Add(enemy);
                }
            }
        }
    }
}