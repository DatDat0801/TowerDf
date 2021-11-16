using System;
using System.Collections.Generic;
using Invoke;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class WaveInfoController : MonoBehaviour
    {
        [SerializeField] private Transform goContent;

        [SerializeField] private Transform arrow;
        
        [SerializeField] private Text lbTapToCall;

        [SerializeField] private Text lbTitle;

        private void Start()
        {
            lbTitle.text = L.gameplay.incoming;

            lbTapToCall.text = L.gameplay.tap_to_call;
        }

        public void ShowWaveInfo(Vector3 position, Dictionary<int, int> dictEnemyInLane)
        {
            transform.localScale = Vector3.one;

            transform.position = Vector3.zero;

            var posTarget = position;

            if ((posTarget.x < transform.position.x && arrow.transform.localScale.x < 0) ||
                (posTarget.x >= transform.position.x && arrow.transform.localScale.x > 0))
            {
                var posOld = arrow.localPosition;

                posOld.x *= -1f;

                arrow.localPosition = posOld;

                var scaleOld = arrow.localScale;

                scaleOld.x *= -1f;

                arrow.localScale = scaleOld;
            }

            if (posTarget.x < transform.position.x)
            {
                posTarget.x += 1.8f;

                transform.position = posTarget;
            }
            else
            {
                posTarget.x -= 1.8f;

                transform.position = posTarget;
            }

            InstanceItemInfoWave(dictEnemyInLane);
        }

        public void HideInfoWave()
        {
            foreach (Transform child in goContent)
            {
                Destroy(child.gameObject);
            }

            gameObject.SetActive(false);
        }

        private void InstanceItemInfoWave(Dictionary<int, int> dictEnemyInLane)
        {
            foreach (var enemy in dictEnemyInLane)
            {
                var go = ResourceUtils.GetUnitOther("item_info_wave", goContent);

                if (go)
                {
                    var control = go.GetComponent<ItemInfoWaveController>();

                    if (control != null) control.ShowInfo(enemy.Key, enemy.Value);
                }
            }
        }
    }
}