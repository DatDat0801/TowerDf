using DG.Tweening;
using UnityEngine;

namespace EW2
{
    public class RangeCircleVisual : MonoBehaviour
    {
        private static RangeCircleVisual s_instance;

        public static RangeCircleVisual Instance
        {
            get
            {
                if (s_instance == null)
                {
                    var obj = ResourceUtils.GetUnitOther("range_circle");
                    if (obj != null)
                    {
                        s_instance = obj.GetComponent<RangeCircleVisual>();
                    }
                }

                return s_instance;
            }
        }

        private SpriteRenderer Sprite => GetComponent<SpriteRenderer>();
        public Building Tower { get; private set; }


        public void SetColor(Color color)
        {
            Sprite.color = color;
        }
        /// <summary>
        /// preview build range
        /// </summary>
        /// <param name="tower"></param>
        /// <param name="position"></param>
        public void ShowRangePreviewTower(TowerData tower, Vector3 position)
        {
            if (tower == null) return;
            var statBase = tower.GetDataStatFinalByLevel(1);
            if (statBase == null)
            {
                //On tower no range
                Hide();
                return;
            }

            // Debug.LogAssertion("Tower id: " + tower + " Detect range attack: " + statBase.detectRangeAttack);
            var radius = statBase.detectRangeAttack;
            transform.position = position;
            var scaleRatio = radius / GameConfig.RatioConvertSizeRangeDetect;
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            transform.DOScale(scaleRatio, 0.2f).From(Vector3.zero);
        }
        /// <summary>
        /// Show preview range on if upgrade
        /// </summary>
        /// <param name="tower"></param>
        /// <param name="level"></param>
        public void ShowRangeBuildingByLevel(Building tower, int level)
        {
            if (tower == null) return;

            var statBase = tower.Stats.GetStat<RangeDetect>(RPGStatType.RangeDetect);
            if (statBase == null)
            {
                //On tower no range
                Hide();
                return;
            }
            
            var data = tower.TowerData.GetDataStatFinalByLevel(level);
            var radius = data.detectRangeAttack;
            
            var scaleRatio = radius / GameConfig.RatioConvertSizeRangeDetect;
            transform.position = tower.transform.position;
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            GetComponent<SpriteRenderer>().size = scaleRatio * Vector2.one;
            transform.DOScale(scaleRatio, 0.2f).From(Vector3.zero);
        }
        /// <summary>
        /// Show range of current level
        /// </summary>
        /// <param name="tower"></param>
        public void ShowRangeBuildingByCurrentStat(Building tower)
        {
            if (tower == null) return;

            var statBase = tower.Stats.GetStat<RangeDetect>(RPGStatType.RangeDetect);
            if (statBase == null)
            {
                //On tower no range
                Hide();
                return;
            }
            
            //var data = tower.TowerData.GetDataStatBaseByLevel(level);
            //var radius = data.detectRangeAttack;
            
            var scaleRatio = statBase.StatValue / GameConfig.RatioConvertSizeRangeDetect;
            transform.position = tower.transform.position;
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);
            GetComponent<SpriteRenderer>().size = scaleRatio * Vector2.one;
            transform.DOScale(scaleRatio, 0.2f).From(Vector3.zero);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}