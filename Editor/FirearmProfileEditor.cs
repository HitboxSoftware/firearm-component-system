using System;
using System.Collections;
using System.Collections.Generic;
using Hitbox.Firearms;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FirearmProfile))]
public class FirearmProfileEditor : Editor
{
    #region --- VARIABLES ---
    
    private FirearmProfile profile;
    
    // Toolbar
    private int tabIndex;
    private int previousTabIndex;

    private bool foldout;

    #region - STYLES -

    private GUIStyle titleStyle;
    private GUIStyle headerStyle; 
    private GUIStyle subHeaderStyle;
    
    #endregion

    #region - SERIALIZED -

    private SerializedObject soTarget;

    // Details
    private SerializedProperty fireRate;
    private SerializedProperty fireMode;

    // Recoil
    private SerializedProperty recoilX;
    private SerializedProperty recoilY;
    private SerializedProperty recoilZ;
    private SerializedProperty recoilPosMinus;
    private SerializedProperty aimRecoilX;
    private SerializedProperty aimRecoilY;
    private SerializedProperty aimRecoilZ;
    private SerializedProperty aimRecoilPosMinus;
    private SerializedProperty snappiness;
    private SerializedProperty returnSpeed;
    private SerializedProperty cameraRecoilMultiplier;

    // Positions
    private SerializedProperty defaultPos;
    private SerializedProperty aimPos;
    private SerializedProperty highReadyPos;
    private SerializedProperty lowReadyPos;
    
    #endregion

    #endregion
    
    #region --- EDITOR ---

    private void OnEnable()
    {
        Init();
        GetProperties();
    }

    public override void OnInspectorGUI()
    {
        soTarget.Update();
        GUILayout.Label("Firearm Profile", titleStyle);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUI.BeginChangeCheck();
        tabIndex = GUILayout.Toolbar(tabIndex, new[] {"General", "Recoil", "Positions"});
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        switch (tabIndex)
        {
            case 0:
                GUILayout.Label("General", headerStyle);
                EditorGUILayout.PropertyField(fireRate);
                EditorGUILayout.PropertyField(fireMode);
                break;
            case 1:
                GUILayout.Label("Recoil", headerStyle);
                GUILayout.Label("Default Recoil", subHeaderStyle);
                EditorGUILayout.PropertyField(recoilX);
                EditorGUILayout.PropertyField(recoilY);
                EditorGUILayout.PropertyField(recoilZ);
                EditorGUILayout.PropertyField(recoilPosMinus);
                EditorGUILayout.Separator();
                GUILayout.Label("Aim Recoil", subHeaderStyle);
                EditorGUILayout.PropertyField(aimRecoilX);
                EditorGUILayout.PropertyField(aimRecoilY);
                EditorGUILayout.PropertyField(aimRecoilZ);
                EditorGUILayout.PropertyField(aimRecoilPosMinus);
                EditorGUILayout.Separator();
                GUILayout.Label("Settings", subHeaderStyle);
                EditorGUILayout.PropertyField(snappiness);
                EditorGUILayout.PropertyField(returnSpeed);
                EditorGUILayout.PropertyField(cameraRecoilMultiplier);
                break;
            case 2:
                GUILayout.Label("Positions", headerStyle);
                EditorGUILayout.PropertyField(defaultPos, new GUIContent("Default Position: "));
                EditorGUILayout.PropertyField(aimPos, new GUIContent("Aim Position: "));
                EditorGUILayout.PropertyField(highReadyPos, new GUIContent("High Ready Position: "));
                EditorGUILayout.PropertyField(lowReadyPos, new GUIContent("Low Ready Position: "));
                break;
        }

        if (EditorGUI.EndChangeCheck())
        {
            soTarget.ApplyModifiedProperties();
            if(tabIndex != previousTabIndex)
                GUI.FocusControl(null);
        }

        previousTabIndex = tabIndex;
    }

    #endregion

    #region --- METHODS ---

    private void Init()
    {
        // --- Get Target ---
        profile = (FirearmProfile) target;
        soTarget = new SerializedObject(target);
        
        // --- Initialize Styles ---
        // - Title -
        titleStyle = new GUIStyle
        {
            padding = new RectOffset(5, 5, 5, 5),
            alignment = TextAnchor.MiddleCenter,
            fontSize = 25,
            normal =
            {
                textColor = Color.white
            }
        };
        
        // - Header -
        headerStyle = new GUIStyle
        {
            padding = new RectOffset(5, 5, 5, 5),
            alignment = TextAnchor.MiddleCenter,
            fontSize = 20,
            normal =
            {
                textColor = Color.white
            }
        };
        
        // - Sub-Header -
        subHeaderStyle = new GUIStyle
        {
            padding = new RectOffset(0, 0, 5, 0),
            alignment = TextAnchor.MiddleLeft,
            fontSize = 14,
            normal =
            {
                textColor = Color.white
            }
        };
    }

    private void GetProperties()
    {
        // General
        fireRate = soTarget.FindProperty("fireRate");
        fireMode = soTarget.FindProperty("fireMode");

        // Recoil
        recoilX = soTarget.FindProperty("recoilX");
        recoilY = soTarget.FindProperty("recoilY");
        recoilZ = soTarget.FindProperty("recoilZ");
        recoilPosMinus = soTarget.FindProperty("recoilPosMinus");

        aimRecoilX = soTarget.FindProperty("aimRecoilX");
        aimRecoilY = soTarget.FindProperty("aimRecoilY");
        aimRecoilZ = soTarget.FindProperty("aimRecoilZ");
        aimRecoilPosMinus = soTarget.FindProperty("aimRecoilPosMinus");

        snappiness = soTarget.FindProperty("snappiness");
        returnSpeed = soTarget.FindProperty("returnSpeed");
        cameraRecoilMultiplier = soTarget.FindProperty("cameraRecoilMultiplier");

        // Positions
        defaultPos = soTarget.FindProperty("defaultPos");
        aimPos = soTarget.FindProperty("aimPos");
        highReadyPos = soTarget.FindProperty("highReadyPos");
        lowReadyPos = soTarget.FindProperty("lowReadyPos");
    }

    #endregion
}
