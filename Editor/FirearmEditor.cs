using System;
using System.Collections;
using System.Collections.Generic;
using Hitbox.Firearms;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Firearm))]
public class FirearmEditor : Editor
{
    #region --- VARIABLES ---
    
    private Firearm firearm;
    
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
    
    // Profile
    private SerializedObject soProfile;
    private Editor profileEditor;
    private FirearmProfile profileReference;
    private SerializedProperty profile;

    // Search Tags
    private SerializedProperty chamberTag;
    private SerializedProperty magazineTag;
    private SerializedProperty barrelTags;

    // Components
    private SerializedProperty nodes;
    private SerializedProperty requiredUnfilledSlots;
    
    // Core Nodes
    private bool hasChamberNode;
    private bool hasMagazineNode;
    private bool hasBarrelNode;

    // Events
    private SerializedProperty isShooting;
    private SerializedProperty isLocked;
    private SerializedProperty triggerHeld;
    private SerializedProperty boltLatched;
    

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
        GUILayout.Label("Firearm", titleStyle);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUI.BeginChangeCheck();
        tabIndex = GUILayout.Toolbar(tabIndex, new[] {"General", "Components"});
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        
        if (soProfile == null)
        {
            try
            {
                soProfile = new SerializedObject(firearm.profile);
                profileEditor = CreateEditor(firearm.profile);
            }
            catch (Exception)
            {
                // ignored
            }
        }
        
        switch (tabIndex)
        {
            case 0:
                GUILayout.Label("General", headerStyle);
                // Horizontal Layout, Displays both Foldout and weaponProperty Field in same line.
                EditorGUILayout.BeginHorizontal();
                foldout = EditorGUILayout.Foldout(foldout, "Firearm Profile", true);
                EditorGUILayout.PropertyField(profile, GUIContent.none);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();

                // Allows Editing of Scriptable Object without going to File.
                if (foldout && soProfile != null)
                {
                    GUIStyle style = GUI.skin.GetStyle("HelpBox");
                    style.padding = new RectOffset(20, 20, 5, 10);
                    EditorGUILayout.BeginVertical(style);
                    profileEditor.OnInspectorGUI();
                    EditorGUILayout.EndVertical();
                }
                GUILayout.Label("Events", subHeaderStyle);
                EditorGUILayout.PropertyField(isShooting);
                EditorGUILayout.PropertyField(isLocked);
                EditorGUILayout.PropertyField(triggerHeld);
                EditorGUILayout.PropertyField(boltLatched);
                break;
            
            case 1:
                GUILayout.Label("Components", headerStyle);
                EditorGUILayout.PropertyField(nodes);
                EditorGUILayout.PropertyField(requiredUnfilledSlots);
                GUILayout.Label("Core Search Tags", subHeaderStyle);
                EditorGUILayout.PropertyField(chamberTag);
                EditorGUILayout.PropertyField(magazineTag);
                EditorGUILayout.PropertyField(barrelTags);
                GUILayout.Label("Found Core Components", subHeaderStyle);
                EditorGUILayout.ToggleLeft("Chamber", hasChamberNode);
                EditorGUILayout.ToggleLeft("Magazine", hasMagazineNode);
                EditorGUILayout.ToggleLeft("Barrel", hasBarrelNode);
                break;
        }

        if (EditorGUI.EndChangeCheck())
        {
            soTarget.ApplyModifiedProperties();
            if(tabIndex != previousTabIndex)
                GUI.FocusControl(null);
            
            if (firearm.profile == null)
            {
                soProfile = null;
                profileEditor = null;
                profileReference = null;
            }
            else if (firearm.profile != profileReference)
            {
                soProfile = new SerializedObject(firearm.profile);
                profileEditor = CreateEditor(firearm.profile);
                profileReference = firearm.profile;
            }
            
            try
            {
                hasChamberNode = firearm.ChamberNode != null;
                hasMagazineNode = firearm.MagazineNode != null;
                hasBarrelNode = firearm.BarrelNode != null;
            }
            catch { /* ignored */ }
        }

        previousTabIndex = tabIndex;
    }

    #endregion

    #region --- METHODS ---

    private void Init()
    {
        // --- Get Target ---
        firearm = (Firearm) target;
        soTarget = new SerializedObject(target);
        
        // --- Get Data ---
        try
        {
            hasChamberNode = firearm.ChamberNode != null;
            hasMagazineNode = firearm.MagazineNode != null;
            hasBarrelNode = firearm.BarrelNode != null;
        }
        catch { /* ignored */ }

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
        profile = soTarget.FindProperty("profile");
        
        // Search Tags
        chamberTag = soTarget.FindProperty("chamberTag");
        magazineTag = soTarget.FindProperty("magazineTag");
        barrelTags = soTarget.FindProperty("barrelTags");
        
        // Components
        nodes = soTarget.FindProperty("nodes");
        requiredUnfilledSlots = soTarget.FindProperty("requiredUnfilledSlots");
        
        // Events
        isShooting = soTarget.FindProperty("isShooting");
        isLocked = soTarget.FindProperty("isLocked");
        triggerHeld = soTarget.FindProperty("triggerHeld");
        boltLatched = soTarget.FindProperty("boltLatched");

    }

    #endregion
}
