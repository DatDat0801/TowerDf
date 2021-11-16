using System;
using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class TowerSkillController
    {
        private readonly Dictionary<BranchType, BaseTowerSkill> dictSkill = new Dictionary<BranchType, BaseTowerSkill>();

        public void AddSkill(BaseTowerSkill towerSkill)
        {
            if (!dictSkill.ContainsKey(towerSkill.BranchType))
                dictSkill.Add(towerSkill.BranchType, towerSkill);
        }

        public BaseTowerSkill GetSkill(BranchType branchType)
        {
            try
            {
                return dictSkill[branchType];
            }
            catch
            {
                return null;
            }
        }

        public int GetSkillLevel(BranchType branchType)
        {
            try
            {
                return dictSkill[branchType].Level;
            }
            catch
            {
                return 0;
            }
        }

        public bool CheckSkillActiveTarget()
        {
            return GetSkillActive() != null;
        }

        public BaseTowerSkill GetSkillActive()
        {
            foreach (var skill in dictSkill.Values)
            {
                if (skill.SkillType == SkillType.ActiveTarget && skill.IsReady)
                    return skill;
            }

            return null;
        }
        
        public void ActiveSkillTarget(BranchType branchType, Action callback)
        {
            var skillActive = GetSkill(branchType);
            if (skillActive != null)
            {
                skillActive.ActiveSkill(callback);
            }
            else
            {
                Debug.LogError("Skill index is invalid: " + branchType);
            }
        }

        public bool OnRaiseSkill(BranchType branchType)
        {
            var skill = GetSkill(branchType);
            if (skill != null)
            {
                return skill.OnRaise();
            }
            else
            {
                Debug.LogError("Skill index is invalid: " + branchType);
                return false;
            }
        }

        public void Reset()
        {
            dictSkill.Clear();
        }
    }
}