using System;
using Invoke;

namespace EW2
{
    public enum SkillType
    {
        Passive,
        ActiveTarget,
    }

    public abstract class BaseTowerSkill
    {
        protected Unit owner;
        protected SkillType skillType;

        public SkillType SkillType => skillType;

        protected BranchType branchType;
        public BranchType BranchType => branchType;

        protected float timeCooldown;

        public int Level { get; private set; }
        
        public int MaxLevel { get; protected set; }

        public bool IsReady { get; protected set; }

        public abstract void Init(Unit owner, BranchType branchType, SkillType skillType, TowerData data);

        /// <summary>
        /// Event upgrade skill
        /// </summary>
        public bool OnRaise()
        {
            if (!CheckMaxLevelSkill())
            {
                Level++;
                UpdateData();
                OnRaiseSuccess();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract void OnRaiseSuccess();

        public bool CheckMaxLevelSkill()
        {
            return Level >= MaxLevel;
        }

        /// <summary>
        /// Update data after upgrade skill
        /// </summary>
        public abstract void UpdateData();

        public bool CheckUnlocked()
        {
            return Level >= 0;
        }

        public virtual void ActiveSkill(Action callback = null)
        {
            callback?.Invoke();
        }

        public void StartCooldown()
        {
            IsReady = false;
            InvokeProxy.Iinvoke.CancelInvoke(this, CooldownDone);
            InvokeProxy.Iinvoke.Invoke(this, CooldownDone, timeCooldown);
        }

        protected void ResetCooldown()
        {
            IsReady = true;
            InvokeProxy.Iinvoke.CancelInvoke(this, CooldownDone);
        }

        protected void CooldownDone()
        {
            IsReady = true;
        }
    }
}