using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Spine.Unity;
using UnityEngine;
using UnityEditor;

namespace EW2
{
    public class PlaySoundEffectByFrame : MonoBehaviour
    {
        public string fileName;
        private SkeletonAnimation _anim;
        private IEnumerator effectCoroutineHandle;
        private List<SoundQueueAction> queue;
        public List<SoundEffectData> soundEffects;

        [Space(10)] [SpineAnimation] [SerializeField]
        public string previewAnimation;

        public SkeletonAnimation anim
        {
            get
            {
                if (_anim == null)
                {
                    _anim = GetComponent<SkeletonAnimation>();
                    if (_anim == null)
                    {
                        _anim = GetComponentInChildren<SkeletonAnimation>();
                        Debug.Assert(_anim != null, "Cannot get SkeletonAnimation!");
                    }
                }

                return _anim;
            }

            set { _anim = value; }
        }

        private string GetPlayingAnimation()
        {
            return previewAnimation;
        }

        private void CreateQueueAction(string animName = "animation")
        {
            queue = new List<SoundQueueAction>();
            for (int i = 0; i < soundEffects.Count; i++)
            {
                if (soundEffects[i].animName != animName)
                {
                    continue;
                }

                var listFrameData = soundEffects[i].listSoundFrameData;
                for (int j = 0; j < listFrameData.Count; j++)
                {
                    SoundQueueAction action = new SoundQueueAction();
                    action.animName = soundEffects[i].animName;
                    action.time = listFrameData[j].frame * 1.0f / 30;

                    action.folderPath = listFrameData[j].fixedFolderPath;
                    action.fileName = listFrameData[j].fixedFileName;

                    queue.Add(action);
                }
            }
        }

        public void PlayAnimationEffectByName(string animName)
        {
            previewAnimation = animName;
            CreateQueueAction(animName);
            if (queue.Count > 0)
            {
                effectCoroutineHandle = DoPlayEffectAnimation();
                StartCoroutine(effectCoroutineHandle);
            }
        }

        private IEnumerator DoPlayEffectAnimation()
        {
            float elapse = 0;
            while (true)
            {
                if (queue.Count == 0)
                {
                    yield break;
                }

                for (int i = 0; i < queue.Count; ++i)
                {
                    if (queue[i].animName == GetPlayingAnimation())
                    {
                        if (queue[i].time < elapse)
                        {
                            string soundPath = string.Format("AssetBundles/Audio/{0}/{1}", queue[i].folderPath,
                                queue[i].fileName);
                            PlaySoundByPath(soundPath);
                            // TODO Update Play Sound
                            queue.RemoveAt(i);
                            --i;
                        }
                    }
                }

                yield return null;
                elapse += Time.deltaTime;
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

        public void PreviewAnimation()
        {
            anim.AnimationState.SetAnimation(0, previewAnimation, false);
            CreateQueueAction(previewAnimation);
            if (queue.Count > 0)
            {
                StopAnimationEffect();
                effectCoroutineHandle = DoPlayEffectAnimation();
                StartCoroutine(effectCoroutineHandle);
            }
        }

        private void PlaySoundByPath(string path)
        {
            var audioClip = Resources.Load<AudioClip>(path);
            var audioSource = FindObjectOfType<AudioSource>();
            if (audioSource == null)
            {
                GameObject go = new GameObject();
                audioSource = go.AddComponent<AudioSource>();
                audioSource.loop = false;
                audioSource.playOnAwake = false;
            }

            audioSource.PlayOneShot(audioClip);
        }

        public void OnValidate()
        {
            if (string.IsNullOrEmpty(fileName))
            {
                var gameObjectName = gameObject.name;
                var arr = gameObjectName.Split('_');
                var heroId = Int32.Parse(arr[1]);
                fileName = arr[0] + "_" + heroId;
            }
        }
    }

    [Serializable]
    public class SoundEffectData
    {
        [SpineAnimation]
        public string animName;
        public List<SoundFrameData> listSoundFrameData = new List<SoundFrameData>();

        public void ParseData(string[] config)
        {
            SoundFrameData soundFrameData = new SoundFrameData();
            soundFrameData.ParseData(config);
            listSoundFrameData.Add(soundFrameData);
        }
    }

    [Serializable]
    public class SoundFrameData
    {
        public int frame;
        public string fixedFolderPath;
        public string fixedFileName;

        public void ParseData(string[] config)
        {
            var frameKey = config[1];
            if (!string.IsNullOrEmpty(frameKey))
            {
                frame = Int32.Parse(frameKey);
            }
            fixedFolderPath = config[2];
            fixedFileName = config[3];
        }
    }

    [Serializable]
    public class SoundQueueAction
    {
        public string animName;
        public float time;
        public string folderPath;
        public string fileName;
    }
}
