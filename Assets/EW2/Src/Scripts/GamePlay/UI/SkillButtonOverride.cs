using UnityEngine;
using UnityEngine.UI;
using Zitga.ContextSystem;
using Zitga.Update;

namespace EW2
{
    public class SkillButtonOverride : MonoBehaviour, IUpdateSystem
    {
        [SerializeField] private Image imgCooldown;
        [SerializeField] private Image iconSkill;

        public float CooldownTime { get; set; }
        public bool Running { get; private set; }

        private float countTime;

        public void ResetCooldown()
        {
            countTime = CooldownTime;
            Running = true;
        }

        void Cooldown(float deltaTime)
        {
            if (Running == false)
            {
                return;
            }

            countTime -= deltaTime;
            imgCooldown.fillAmount = Mathf.Clamp01(countTime / CooldownTime);
        }


        public void OnUpdate(float deltaTime)
        {
            Cooldown(deltaTime);

        }
        public void OnEnable()
        {
            Context.Current.GetService<GlobalUpdateSystem>().Add(this);
        }

        public void OnDisable()
        {
            if (Context.Current != null)
                Context.Current.GetService<GlobalUpdateSystem>().Remove(this);
            Running = false;
            gameObject.SetActive(false);
        }
    }
}