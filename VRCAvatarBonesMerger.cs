using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class VRCAvatarBonesMerger : EditorWindow
{
    private GameObject baseAvatar;
    private List<DynamicBoneCollider> baseAvatarDynamicBoneColliders;
    
    private GameObject newAvatar;
    private List<Component> newAvatarBones;
    
    private List<String> bonesOptions = new List<string>(); // selected from new avatar
    private int[] selectedBonesOptions;

    [MenuItem("Avatar Tools/Avatar Bones Merger")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<VRCAvatarBonesMerger>();
    }

    private void OnGUI()
    {
        //gui avatar options
        baseAvatar = EditorGUILayout.ObjectField(
            "Avatar to copy bones from", 
            baseAvatar, 
            typeof(GameObject), 
            false) as GameObject;
        newAvatar = EditorGUILayout.ObjectField(
            "Avatar to copy bones to", 
            newAvatar, 
            typeof(GameObject), 
            false) as GameObject;

        //if the button is clicked wipe all the fields and remake everything
        if (GUILayout.Button("Calculate Bones"))
        {
            //get bones and store them for avatars
            baseAvatarDynamicBoneColliders = getColliderBonesFromAvatar(baseAvatar);
            newAvatarBones = getBonesForNewAvatar(newAvatar);
            
            //make parallel array to new avatar bones of bone options 
            bonesOptions = new List<string>();
            bonesOptions.Add("Don't Copy");
            foreach (Component bone in newAvatarBones)
            {
                bonesOptions.Add(bone.name);
            }
            
            //create a place to store the selections from the dropdowns
            selectedBonesOptions = new int[baseAvatarDynamicBoneColliders.Count];
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        //create the dropdowns for bone selection with selectable bones from base avatar
        for (int i = 0; i < baseAvatarDynamicBoneColliders.Count; i++)
        {
            selectedBonesOptions[i] = EditorGUILayout.Popup(
                baseAvatarDynamicBoneColliders[i].name, 
                selectedBonesOptions[i], 
                bonesOptions.ToArray());
        }
        
        //create a button to save your new bones
        if (GUILayout.Button("Copy Bones"))
        {
            for (var i = 0; i < selectedBonesOptions.Length; i++)
            {
                if(selectedBonesOptions[i] == 0)
                    continue;

                DynamicBoneCollider copyFrom = baseAvatarDynamicBoneColliders[i];
                Component copyTo = newAvatarBones[selectedBonesOptions[i] - 1];

                DynamicBoneCollider newCollider = copyTo.gameObject.AddComponent(typeof(DynamicBoneCollider)) as DynamicBoneCollider;
                copyDynamicBonesCollider(copyFrom, newCollider);
            }
        }
    }

    private List<Component> getBonesForNewAvatar(GameObject avatar)
    {
        return avatar.GetComponentsInChildren(typeof(Transform)).ToList();
    }
    
    private List<DynamicBoneCollider> getColliderBonesFromAvatar(GameObject avatar)
    {
        List<DynamicBoneCollider> colliderBones = new List<DynamicBoneCollider>();
        Component[] dynamicBoneColliders = avatar.GetComponentsInChildren(typeof(DynamicBoneCollider));

        foreach (var component in dynamicBoneColliders)
        {
            colliderBones.Add((DynamicBoneCollider) component);
        }

        return colliderBones;
    }
    
    private void copyDynamicBonesCollider(DynamicBoneCollider copyFrom, DynamicBoneCollider copyTo)
    {
        copyTo.m_Direction = copyFrom.m_Direction;
        copyTo.m_Center = copyFrom.m_Center;
        copyTo.m_Bound = copyFrom.m_Bound;
        copyTo.m_Radius = copyFrom.m_Radius;
        copyTo.m_Height = copyFrom.m_Height;
        copyTo.m_Radius2 = copyFrom.m_Radius2;
    }
}
