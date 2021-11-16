using EffectOnAnimFrameTool;
using Spine;
using Spine.Unity;
using UnityEngine;

public class PlayGraphicEffectByFrame : PlayEffectByFrame
{
    private SkeletonGraphic graphic;

    protected SkeletonGraphic Graphic
    {
        get
        {
            if (graphic == null)
            {
                graphic = GetComponent<SkeletonGraphic>() ?? GetComponentInChildren<SkeletonGraphic>();
            }

            return graphic;
        }
    }

    protected override void Start()
    {
        if (Application.isPlaying)
        {
            if (Graphic != null)
            {
                Graphic.AnimationState.Start += StartAnimation;
                Graphic.AnimationState.Complete += EndAnimation;
            }
        }
    }

    protected override void StartAnimation(TrackEntry trackEntry)
    {
        print("play animation: " + trackEntry.Animation.Name);
        base.StartAnimation(trackEntry);
    }

    protected override void EndAnimation(TrackEntry trackEntry)
    {
        print("end animation: " + trackEntry.Animation.Name);
        base.EndAnimation(trackEntry);
    }

    protected override float GetTimeScale()
    {
        return Graphic.timeScale;
    }

    protected override string CalculateCurrentSkin() => Graphic.initialSkinName;

#if UNITY_EDITOR
    protected override void PreviewAnimation()
    {
        Graphic.AnimationState.ClearTracks();
        Graphic.AnimationState.SetAnimation(0, previewAnimation, false);
    }

    // public override void StartPreviewAnimation()
    // {
    //     var a = gameObject.GetComponent<PlayEffectByFrame>();
    //
    //     if (a != this)
    //         totalAnim = a.totalAnim;
    // }
    #endif
}