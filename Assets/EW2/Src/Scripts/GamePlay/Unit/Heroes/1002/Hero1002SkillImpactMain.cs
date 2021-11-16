using System;
using System.Collections;
using Spine.Unity;
using UnityEngine;

namespace EW2
{
    public class Hero1002SkillImpactMain : MonoBehaviour
    {
        public SkeletonAnimation[] arrSkeletonArrows;
        public Hero1002SkillImpact[] arrImpacts;

        private Hero1002 _owner;
        private HeroData1002.ActiveSkill _dataSkill;

        public void Init(Hero1002 shooter, HeroData1002.ActiveSkill dataActiveSkill)
        {
            this._owner = shooter;
            this._dataSkill = dataActiveSkill;
            StartCoroutine(ShowEffect());
        }

        public IEnumerator ShowEffect()
        {
            for (int i = 0; i < this.arrSkeletonArrows.Length; i++)
            {
                arrSkeletonArrows[i].gameObject.SetActive(true);
                arrSkeletonArrows[i].AnimationState.SetAnimation(0, "skill_active_1", false);
                yield return new WaitForSeconds(0.18f);
                this.arrImpacts[i].gameObject.SetActive(true);
                this.arrImpacts[i].Init(this._owner, this._dataSkill, i >= 1);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < this.arrSkeletonArrows.Length; i++)
            {
                arrSkeletonArrows[i].gameObject.SetActive(false);
                this.arrImpacts[i].gameObject.SetActive(false);
            }
        }
    }
}