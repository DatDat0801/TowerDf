using System;
using Cysharp.Threading.Tasks;
using Hellmade.Sound;

namespace EW2
{
    public class Tower2003Skill2 : TowerSkill2
    {
        private Tower2003 control;
        private TowerData2003 dataTower;
        private TowerData2003.Skill2 dataSkill;
        private Soldier2003 currentSoldierSelected;
        private Action callbackActiveDone;

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
            dataSkill = dataTower.GetDataSkill2ByLevel(Level);
            if (dataSkill != null)
                timeCooldown = dataSkill.cooldown;
        }

        public override void ActiveSkill(Action callback)
        {
            if (!CheckUnlocked()) return;
            PlaySoundSkill2();
            
            currentSoldierSelected = control.soldier;
            callbackActiveDone = callback;
            AttackTarget();
        }

        async void PlaySoundSkill2()
        {
            var audioClip = ResourceSoundManager.GetSoundTower(this.owner.Id, SoundConstant.Tower2003CastSkill);
            await UniTask.Delay(200);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            await UniTask.Delay(800);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            await UniTask.Delay(1000);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
        }
        private void AttackTarget()
        {
            StartCooldown();

            currentSoldierSelected.AttackSkill2(dataSkill, () => { callbackActiveDone?.Invoke(); });
        }
    }
}