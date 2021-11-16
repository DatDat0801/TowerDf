namespace EW2
{
    public class SoldierData
    {
        public int id;
        public int level;
        public float health;
        public float moveSpeed;
        public float attackSpeed;
        public float armor;
        public float resistance;
        public float damage;
        public float critChance;
        public float critDamage;
        public float hpRegeneration;
        public float timeTriggerRegeneration;
        public float timeRevive;
        public DamageType damageType;
        public int blockEnemy;
        public MoveType searchTarget;
        public PriorityTargetType[] priorityTarget;
    }
}