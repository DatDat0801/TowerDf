using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace EffectOnAnimFrameTool
{
    public class PlayEffectByFrame : MonoBehaviour
    {
        public bool ignoreTimeScale = false;
        
        private SkeletonAnimation anim;

        private List<QueueAction> queue;

        private Dictionary<GameObject, FxParentData> playingEffects;

        private IEnumerator effectCoroutineHandle;

        private string playingAnimation;

        [Title("VFX TOOL", TitleAlignment = TitleAlignments.Centered)] [FormerlySerializedAs("effects")]
        public List<EffectData> totalAnim;

        [Title("PREVIEW ANIMATION", TitleAlignment = TitleAlignments.Centered)]
        [Space(10)]
        [SpineAnimation]
        [SerializeField]
        public string previewAnimation;

#if UNITY_EDITOR
        private string currentAnimationName;
        private bool isPreview;
        private List<GameObject> anySpawnedVfxs = new List<GameObject>();
#endif

        protected virtual void Start()
        {
            if (Application.isPlaying)
            {
                if (Anim != null)
                {
                    Anim.state.Start += StartAnimation;
                    Anim.state.Complete += EndAnimation;
                }
            }
        }

        protected virtual void StartAnimation(TrackEntry trackEntry)
        {
            PlayAnimationEffectByName(trackEntry.Animation.Name);
        }

        protected virtual void EndAnimation(TrackEntry trackEntry)
        {
            if (!string.IsNullOrEmpty(playingAnimation) && playingAnimation.Equals(trackEntry.Animation.Name))
            {
                print("End animation: " + trackEntry.Animation.Name);
                playingAnimation = string.Empty;
            }
        }

        protected SkeletonAnimation Anim
        {
            get
            {
                if (anim == null)
                {
                    anim = GetComponent<SkeletonAnimation>() ?? GetComponentInChildren<SkeletonAnimation>();

                    //Debug.Assert(anim != null, "Cannot get SkeletonAnimation!");
                }

                return anim;
            }
        }

        protected virtual float GetTimeScale()
        {
            return Anim.timeScale;
        }

        private void CreateQueueAction(string animName = "animation")
        {
            queue = new List<QueueAction>();
            playingEffects = new Dictionary<GameObject, FxParentData>();
            for (var i = 0; i < totalAnim.Count; i++)
            {
                var effectData = totalAnim[i];
                if (effectData.animName != animName)
                {
                    continue;
                }

                for (var j = 0; j < effectData.effects.Length; j++)
                {
                    var effectDataFrame = effectData.effects[j];
                    if (string.IsNullOrEmpty(effectDataFrame.effectName))
                        continue;
                    var action = new QueueAction
                    {
                        animName = effectData.animName,
                        time = effectDataFrame.startFrame * (1.0f / (30 * GetTimeScale())),
                        effectType = effectDataFrame.effectType,
                        effectName = effectDataFrame.effectName,
                        parent = effectDataFrame.parent,
                        localPosition = effectDataFrame.configPosition ? effectDataFrame.localPosition : Vector3.zero,
                        localScale = effectDataFrame.configScale ? effectDataFrame.localScale : Vector3.one,
                        applyFor = effectDataFrame.applyFor,
                        isSelectedSkin = effectDataFrame.configSkin,
                        skinName = effectDataFrame.skinName,
                        isChildParent = effectDataFrame.isChildParent,
                    };
                    queue.Add(action);
                }
            }
        }

        private bool IsApplyFor(Apply applyFor)
        {
            return applyFor == Apply.Both;
        }

        private void PlayAnimationEffectByName(string animName)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            playingAnimation = animName;

            CreateQueueAction(animName);
            if (queue.Count > 0)
            {
                StopAnimationEffect();
                effectCoroutineHandle = DoPlayEffectAnimation();
                StartCoroutine(effectCoroutineHandle);
            }
        }

        public void StopAnimationEffect()
        {
            if (effectCoroutineHandle != null)
            {
                StopCoroutine(effectCoroutineHandle);
                effectCoroutineHandle = null;
            }
        }

        private IEnumerator DoPlayEffectAnimation()
        {
            float elapse = 0;
            while (true)
            {
                if (queue.Count == 0 && playingEffects.Count == 0)
                {
                    yield break;
                }

                for (var i = 0; i < queue.Count; ++i)
                {
                    var queueItem = queue[i];
                    if (queueItem.isSelectedSkin)
                    {
                        if (queueItem.skinName != CalculateCurrentSkin())
                        {
                            continue;
                        }
                    }

                    if (string.IsNullOrEmpty(playingAnimation))
                    {
                        //throw new Exception($"Playing Animation is null: {queueItem.time} => {elapse}");
                        Debug.LogWarning($"Playing Animation is null: {queueItem.time} => {elapse}");
                        yield break;
                    }

                    if (queueItem.animName == playingAnimation && IsApplyFor(queueItem.applyFor))
                    {
                        if (queueItem.time < /*GetAnimStateTime()*/elapse)
                        {
                            var fx = SpawnObject(queueItem.effectType, queueItem.effectName);
                            if (fx != null)
                            {
                                var fxTransform = fx.transform;

                                if (queueItem.parent)
                                {
                                    if (queueItem.isChildParent)
                                    {
                                        fxTransform.SetParent(queueItem.parent.transform);
                                        fxTransform.localPosition = queueItem.localPosition;
                                        fxTransform.localScale = queueItem.localScale;
                                        fxTransform.localRotation = Quaternion.identity;
                                    }
                                    else
                                    {
                                        fxTransform.position = queueItem.parent.transform.position;
                                        fxTransform.rotation = queueItem.parent.transform.rotation;
                                        fxTransform.localScale = queueItem.parent.transform.localScale;
                                    }
                                }
                                else
                                {
                                    fxTransform.position = queueItem.localPosition + transform.position;
                                    fxTransform.localScale = transform.localScale;
                                }

                                playingEffects.Add(fx, new FxParentData(queueItem, queueItem.parent));
                                fx.SetActive(true);
                            }

                            //Debug.Log("Remove Queue");

                            queue.RemoveAt(i);
                            --i;
                        }
                    }
                }

                var temp = new List<GameObject>();
                foreach (var entry in playingEffects)
                {
                    var fx = entry.Key;

                    temp.Add(fx);
                }

                for (var i = 0; i < temp.Count; ++i)
                    playingEffects.Remove(temp[i]);

                yield return new WaitForEndOfFrame();
                
                elapse += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            }
        }

        protected virtual GameObject SpawnObject(string type, string effectName)
        {
            return ResourceUtils.GetVfx(type, effectName, Vector3.zero, Quaternion.identity);
        }

        protected virtual string CalculateCurrentSkin() => Anim.initialSkinName;

#if UNITY_EDITOR
        [Button(ButtonSizes.Large), GUIColor(0.2f, 0.7f, 0.1f)]
        public virtual void StartPreviewAnimation()
        {
            if (string.IsNullOrEmpty(previewAnimation))
            {
                return;
            }

            isPreview = true;
            InitPreviewAnimation();
            ClearAnySpawnedVfxs();
            PreviewAnimation();
        }

        protected virtual void PreviewAnimation()
        {
            Anim.AnimationState.ClearTracks();
            Anim.AnimationState.SetAnimation(0, previewAnimation, false);
        }
        
        private void InitPreviewAnimation()
        {
            PlayAnimationEffectByName(previewAnimation);
        }

        private void ClearAnySpawnedVfxs()
        {
            foreach (var spawnedVfx in anySpawnedVfxs)
            {
                DestroyImmediate(spawnedVfx);
            }

            anySpawnedVfxs.Clear();
        }

        [Button(ButtonSizes.Large), GUIColor(0.7f, 0.1f, 0.1f)]
        public void StopPreviewAnimation()
        {
            StopAnimationEffect();
            ClearAnySpawnedVfxs();
            isPreview = false;
        }
#endif
    }
}