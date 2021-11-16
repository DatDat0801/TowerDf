#if UNITY_EDITOR
using System;
using System.Linq;
using Spine.Unity;
using UnityEditor;
using UnityEngine;

public class EffectToolEditor : EditorWindow
{
    [MenuItem("MyTool/Effect Tool Window")]
    static void ShowWindow()
    {
        var editor = GetWindow(typeof(EffectToolEditor));
        editor.Show();
    }

    private int PreviousIndexLevel = - 1;
    private int CurrentIndexLevel;
    private float playTime;

    void OnGUI()
    {
        GUILayout.Label("EFFECT");
        Time.timeScale = EditorGUILayout.Slider("Time Scale", Time.timeScale, 0, 10);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Play Particle"))
            PlayParticle();
        if (GUILayout.Button("Play Effect"))
            PlayEffect();
        EditorGUILayout.EndHorizontal();
        if (!Application.isPlaying)
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<SkeletonAnimation>() != null)
            {
                SelectSkeletonAnimation();
            }
            else if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<SkeletonGraphic>() != null)
            {
                SelectSkeletonGraphic();
            }
        }
        else
        {
            EditorGUI.BeginDisabledGroup(true);
            
            GUILayout.Label(string.Empty);
            GUILayout.Label(string.Empty);
            GUILayout.Label("ANIMATION TIMELINE");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Frame: 0");
            EditorGUILayout.Slider(0, 0, 1f);
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();
        }
    }

    private void UpdateAnimation(Spine.AnimationState state, Spine.Skeleton skeleton, Action<bool> initialize)
    {
        CurrentIndexLevel = EditorGUILayout.Popup(CurrentIndexLevel, skeleton.Data.Animations.Select(s => s.Name).ToArray());
        
        if (PreviousIndexLevel != CurrentIndexLevel)
        {
            state.TimeScale = 1;
            state.SetAnimation(0, skeleton.Data.Animations.Items[CurrentIndexLevel].Name, false);
            state.TimeScale = 0;
            initialize?.Invoke(true);
            
            PreviousIndexLevel = CurrentIndexLevel;
            playTime = 0;
        }
        if (state.GetCurrent(0) == null)
        {
            state.TimeScale = 1;
            state.SetAnimation(0, skeleton.Data.Animations.Items[CurrentIndexLevel].Name, false);
            state.TimeScale = 0;
        }
    }

    private void DrawAnim(Spine.AnimationState state)
    {
        if (state.Tracks.Count == 0)
        {
            return;
        }
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Frame: " + (int)(playTime * 30));
        playTime = EditorGUILayout.Slider(playTime, 0, state.Tracks.Items[0].Animation.Duration);
        EditorGUILayout.EndHorizontal();    

        state.GetCurrent(0).TrackTime = playTime;
        state.Update(playTime);
    }

    private void SelectSkeletonAnimation()
    {
        GUILayout.Label(string.Empty);
        GUILayout.Label(string.Empty);
        GUILayout.Label("ANIMATION TIMELINE");

        var ani = Selection.activeGameObject.GetComponent<SkeletonAnimation>();
        ani.AnimationName = null;
        
        UpdateAnimation(ani.state, ani.Skeleton, ani.Initialize);
        
        DrawAnim(ani.state);
    }

    private void SelectSkeletonGraphic()
    {
        GUILayout.Label(string.Empty);
        GUILayout.Label(string.Empty);
        GUILayout.Label("ANIMATION TIMELINE");

        var ani = Selection.activeGameObject.GetComponent<SkeletonGraphic>();

        UpdateAnimation(ani.AnimationState, ani.Skeleton, ani.Initialize);

        DrawAnim(ani.AnimationState);
    }

    [MenuItem("MyTool/EffectTool/PlayParticle &a")]
    public static void PlayParticle()
    {
        var obj = Selection.activeObject as GameObject;
        if (obj != null)
        {
            var particle = obj.GetComponent<ParticleSystem>();
            if (particle != null)
            {
                particle.Clear();
                particle.Play();
            }
        }
    }
    [MenuItem("MyTool/EffectTool/PlayEffect &s")]
    public static void PlayEffect()
    {
        var obj = Selection.activeObject as GameObject;
        if (obj != null)
        {
            var particle = obj.GetComponent<ParticleSystem>();
            if (particle != null)
            {
                var rootObj = GetRoot(particle.gameObject);
                var rootParticle = rootObj.GetComponent<ParticleSystem>();
                rootParticle.Clear();
                rootParticle.Play();
            }
        }
    }

    private static GameObject GetRoot(GameObject obj)
    {
        var parent = obj.transform.parent;
        if (parent != null)
        {
            var particle = parent.GetComponent<ParticleSystem>();
            if (particle != null)
                return GetRoot(particle.gameObject);
        }
        return obj;
    }

    [MenuItem("MyTool/Apply All Prefab #&L")]
    public static void Apply()
    {
        var list = Selection.gameObjects;
        if (list.Length > 0)
#pragma warning disable 0618
            foreach (var obj in list)
                PrefabUtility.ReplacePrefab(PrefabUtility.FindPrefabRoot(obj), PrefabUtility.GetCorrespondingObjectFromSource(obj), ReplacePrefabOptions.ConnectToPrefab);
#pragma warning restore 0618
    }
}
#endif