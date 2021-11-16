using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace EffectOnAnimFrameTool
{
   

    [Serializable]
    public class EffectData
    {
        [SpineAnimation] public string animName;
        [FormerlySerializedAs("frames")] public FrameData[] effects;
    }

    [Serializable]
    public class FrameData
    {
        [FormerlySerializedAs("frame")] public int startFrame;
        public Apply applyFor;
        public string effectType;
        public string effectName;
        public GameObject parent;
        public bool configPosition;
        [ShowIf("configPosition")]   public Vector3 localPosition;
        public bool configScale;
        [ShowIf("configScale")]      public Vector3 localScale = Vector3.one;
        public bool configSkin;
        [ShowIf("configSkin")] [SpineSkin] public string skinName;
        public bool isChildParent=true;

    }

    [Serializable]
    public class QueueAction
    {
        public Apply applyFor;
        public string animName;
        public float time;
        public string effectType;
        public string effectName;
        public GameObject parent;
        public Vector3 localPosition;
        public Vector3 localScale = Vector3.one;
        public bool isSelectedSkin;
        public bool isChildParent=true;
        [SpineSkin] public string skinName;
        
    }

    [Serializable]
    public class FxParentData
    {
        public GameObject parent;


        public void ComputeFollowParentPositionAndRotation()
        {
        }


        public FxParentData(QueueAction action, GameObject gameObject)
        {
            parent = action.parent == null ? gameObject : action.parent;
        }
    }

    public enum Apply
    {
        Both,
    }
}