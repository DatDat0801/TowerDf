using System;
using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class HeroSkillController
    {
        protected Unit owner;

        protected UnitSkill skillActive;

        protected readonly List<UnitSkill> skillPassives;

        public UnitSkill SkillActive => skillActive;

        public HeroSkillController(Unit owner)
        {
            this.owner = owner;

            skillPassives = new List<UnitSkill>();
        }

        public void InitSkill()
        {
            var levelSkills = new[] {1, 0, 0, 0};

            var data = UserData.Instance.UserHeroData.GetHeroById(owner.Id);

            if (data != null)
            {
                levelSkills = UserData.Instance.UserHeroData.GetHeroById(owner.Id).levelSkills;
            }
            else
            {
                if (GamePlayController.IsTrialCampaign)
                {
                    var trialHeroData = GameContainer.Instance.Get<MapDataBase>().GetTrialHeroData();
                    if (trialHeroData != null)
                    {
                        var trialData = trialHeroData.GetDataTrial(GamePlayController.CampaignId);
                        levelSkills = new[] {trialData.levelSkillActive, trialData.levelSkillPassive1, trialData.levelSkillPassive2, trialData.levelSkillPassive3};
                    }
                }
            }


            skillActive.Level = levelSkills[0];

            skillActive.Init();

            for (int i = 0; i < skillPassives.Count; i++)
            {
                var skillPassive = skillPassives[i];

                skillPassive.Level = levelSkills[i + 1];

                skillPassive.Init();
            }
        }

        public void RunActiveSkill()
        {
            Debug.Log("Run skill active");

            skillActive.Execute();
        }

        public void RunPassiveSkill()
        {
            foreach (var skillPassive in skillPassives)
            {
                skillPassive.Execute();
            }
        }

        public void StopAllSkill()
        {
            skillActive.Remove();

            foreach (var skillPassive in skillPassives)
            {
                skillPassive.Remove();
            }
        }

        public void SetSkillActive(UnitSkill skillActive)
        {
            this.skillActive = skillActive;
        }

        public void AddSkillPassive(UnitSkill skillPassive)
        {
            skillPassives.Add(skillPassive);
        }

        public UnitSkill GetSkillPassive(int index)
        {
            try
            {
                return skillPassives[index];
            }
            catch (Exception e)
            {
                Debug.Log("Skill index is invalid: " + index);
                throw;
            }
        }
    }
}