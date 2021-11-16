using System;
using Hellmade.Sound;

namespace EW2
{
    public class Tower2001Skill2 : TowerSkill2
    {
        private TowerData2001 dataTower;
        private TowerData2001.Skill2 dataSkill;

        public override void Init(Unit owner, BranchType branchType, SkillType skillType, TowerData data)
        {
            this.owner = owner;
            this.branchType = branchType;
            this.skillType = skillType;
            this.dataTower = data as TowerData2001;
        }

        public override void UpdateData()
        {
            dataSkill = dataTower.GetDataSkill2ByLevel(Level);
        }

        public override void ActiveSkill(Action callback = null)
        {
            if (Level <= 0) return;

            EazySoundManager.PlaySound(
                ResourceSoundManager.GetSoundTower(((Tower2001) owner).Id, SoundConstant.Tower2001CastSkill2),
                EazySoundManager.GlobalSoundsVolume);

            var control = owner as Tower2001;
            var goBuff = ResourceUtils.GetUnit("tower_2001_buff_atk_speed");
            if (goBuff != null)
            {
                goBuff.transform.position = this.owner.Transform.position;
                if (control != null)
                {
                    control.searchTargetBuffAtkSpeed = goBuff.GetComponent<Tower2001BuffAtkSpeed>();
                    if (control.searchTargetBuffAtkSpeed)
                    {
                        control.searchTargetBuffAtkSpeed.ActiveSkill(this.owner as Tower2001, dataSkill);
                    }
                }
            }
        }
    }
}