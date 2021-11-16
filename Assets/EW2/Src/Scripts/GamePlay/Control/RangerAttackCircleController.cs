using TigerForge;
using UnityEngine;
using Zitga.ContextSystem;
using Zitga.Update;

namespace EW2
{
    public class RangerAttackCircleController : MonoBehaviour, IUpdateSystem
    {
        //private const float RatioConvertRange = 3.3f;
        private HeroBase target;
        
        public virtual void OnEnable()
        {
            
            if (Context.Current.GetService<GlobalUpdateSystem>() != null)
            {
                Context.Current.GetService<GlobalUpdateSystem>().Add(this);
            }

            EventManager.StartListening(GamePlayEvent.OnUpdateRangeAttack, RangeUpdate);
        }


        public virtual void OnDisable()
        {
            if (Context.Current != null)
                Context.Current.GetService<GlobalUpdateSystem>().Remove(this);

            EventManager.StopListening(GamePlayEvent.OnUpdateRangeAttack, RangeUpdate);
        }

        private void RangeUpdate()
        {
            var radius = EventManager.GetData<float>(GamePlayEvent.OnUpdateRangeAttack);
            transform.localScale = Vector3.one * radius;
        }

        public void SetTarget(HeroBase hero)
        {
            this.target = hero;

            var rangeSearchTarget = hero.SearchTarget as RangerSearchTarget;

            if (rangeSearchTarget)
            {
                var radius = rangeSearchTarget.GetRangeAttack();

                var scaleConvert = radius / GameConfig.RatioConvertSizeRangeDetect;
                transform.localScale = new Vector3(scaleConvert, scaleConvert);
            }
            transform.position = target.transform.position;
        }

        public void OnUpdate(float deltaTime)
        {
            if (target)
            {
                transform.position = target.transform.position;
            }
        }
    }
}