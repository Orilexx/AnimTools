using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine.SceneManagement;

public class ToolWindow : EditorWindow
{
    static GameObject[] objectsInScene;
    public static List<Animator> animators = new List<Animator>();
    static Animator animSelected;
    static GameObject objSelected;
    static AnimationClip clipToPlay;
    static double timer = 0f;
    static double baseTimer;
    static double originalTimer;
    static float speed = 1f;
    static bool sliderMode = false;
    static float sliderValue = 0f;

    [MenuItem("AnimTool/AnimationsTool")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ToolWindow));
        EditorApplication.playModeStateChanged += stateChanged;
        SceneManager.activeSceneChanged += Reset;
    }


    private void Update()
    {
#if UNITY_EDITOR
        if (!sliderMode)
        {
            timer = EditorApplication.timeSinceStartup;
            baseTimer = originalTimer;
        }


        if (clipToPlay != null && !EditorApplication.isPlaying && objSelected != null)
        {
            if (sliderMode)
            {
                timer = sliderValue * clipToPlay.length;
                baseTimer = 0;
            }
            AnimationMode.StartAnimationMode();
            AnimationMode.BeginSampling();

            AnimationMode.SampleAnimationClip(objSelected, clipToPlay, (float)(timer - baseTimer) * speed);
            AnimationMode.EndSampling();

            SceneView.RepaintAll();

            

            if ((timer - baseTimer) *speed > clipToPlay.length)
            {
                if (clipToPlay.isLooping)
                {
                    baseTimer = EditorApplication.timeSinceStartup;
                    originalTimer = EditorApplication.timeSinceStartup;
                    timer = baseTimer;

                }
                else
                {
                    clipToPlay = null;
                }
            }
        }
#endif
    }

    void OnGUI()
    {
        
#if UNITY_EDITOR

        animators.Clear();
        GUILayout.Label("Animators Found", EditorStyles.boldLabel);
        objectsInScene = EditorSceneManager.GetActiveScene().GetRootGameObjects();
        Animator anim;

        SearchField search;

        

        foreach (GameObject obj in objectsInScene)
        {
            if (obj.TryGetComponent<Animator>(out anim))
            {
                
                animators.Add(anim);
                if(GUILayout.Button(obj.name))
                {
                    SceneView.lastActiveSceneView.LookAt(obj.transform.position);
                    sliderMode = false;
                    speed = 1;
                    Selection.activeGameObject = obj;
                    animSelected = anim;
                    objSelected = obj;
                }
            }

        }

        if(animSelected != null)
        {
            EditorGUILayout.Space();
            GUILayout.Label("Clips", EditorStyles.boldLabel);

            sliderMode = EditorGUILayout.Toggle(sliderMode);

            speed = EditorGUILayout.FloatField("Animation speed ", speed);
            if (speed < 0) speed = 0;


            foreach (AnimationClip clip in animSelected.runtimeAnimatorController.animationClips)
            {
                if (GUILayout.Button(clip.ToString())) {
                    clipToPlay = clip;
                    baseTimer = EditorApplication.timeSinceStartup;
                    originalTimer = EditorApplication.timeSinceStartup;
                }
            }
        }

        
        if(sliderMode)
        {
            sliderValue = EditorGUILayout.Slider("Animation timeline ", sliderValue, 0, 1);
            //timer = sliderValue;
        }

        if (clipToPlay != null && !EditorApplication.isPlaying && objSelected != null)
        {
            

            EditorGUILayout.Space();
            GUILayout.Label("Clip Informations", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Animation name :", clipToPlay.name);
            EditorGUILayout.LabelField("Loop :", clipToPlay.isLooping.ToString());
            EditorGUILayout.LabelField("Length :", clipToPlay.length.ToString());
            EditorGUILayout.LabelField("Timer :", ((timer - baseTimer) * speed).ToString());

        }


#endif
    }

    
    private static void stateChanged(PlayModeStateChange state)
    {
        animators.Clear();
        sliderMode = false;
        objectsInScene = null;
        animSelected = null;
        objSelected = null;
        clipToPlay = null;
        timer = 0f;
        baseTimer = 0f;
        originalTimer = 0f;
        speed = 1f;
        sliderValue = 0f;
    }


    private static void Reset(Scene current, Scene next)
    {
        animators.Clear();
        sliderMode = false;
        objectsInScene = null;
        animSelected = null;
        objSelected = null;
        clipToPlay = null;
        timer = 0f;
        baseTimer = 0f;
        originalTimer = 0f;
        speed = 1f;
        sliderValue = 0f;
    }
}