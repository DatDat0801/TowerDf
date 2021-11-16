using System;
using Hellmade.Sound;
using Invoke;

namespace EW2
{
    public class Tower2003Skill1 : TowerSkill1
    {
        private Tower2003 control;
        private TowerData2003 dataTower;
        private TowerData2003.Skill1 dataSkill;
        private Soldier2003 currentSoldierSelected;
        private Action callbackActiveDone;
        private Unit targetAttack;

        public override void Init(Unit owner, BranchType branchType, SkillType skillType, TowerData data)
        {
            this.owner = owner;
            this.control = owner as Tower2003;
            this.branchType = branchType;
            this.skillType = skillType;
            this.dataTower = data as TowerData2003;
        }

        public override void UpdateData()
        {
            dataSkill = dataTower.GetDataSkill1ByLevel(Level);
            if (dataSkill != null)
                timeCooldown = dataSkill.cooldown;
        }

        public override void ActiveSkill(Action callback)
        {
            if (!CheckUnlocked()) return;
            
            currentSoldierSelected = control.soldier;
            callbackActiveDone = callback;
            AttackTarget();
        }

        private void AttackTarget()
        {
            StartCooldown();

            currentSoldierSelected.AttackSkill1(dataSkill, () => { callbackActiveDone?.Invoke(); });
        }
    }
}