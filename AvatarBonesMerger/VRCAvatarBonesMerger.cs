using UnityEditor;
using UnityEngine;
using VRCAvatarTools;

public class VRCAvatarBonesMerger : EditorWindow
{
    private GameObject baseAvatar;
    private GameObject newAvatar;

    private GenericEntityMerge<DynamicBoneCollider> dynamicBoneColliderMerger = new GenericEntityMerge<DynamicBoneCollider>();
    private GenericEntityMerge<DynamicBone> dynamicBoneMerger = new GenericEntityMerge<DynamicBone>();

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
            dynamicBoneColliderMerger.CalculateCopyableAssets(baseAvatar, newAvatar);
            dynamicBoneMerger.CalculateCopyableAssets(baseAvatar, newAvatar);
        }
        
        EditorGUILayout.Space();

        GUILayout.Label("Dynamic Bone Colliders");
        dynamicBoneColliderMerger.PaintGUI();
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        GUILayout.Label("Dynamic Bones");
        dynamicBoneMerger.PaintGUI();
        
        //create a button to save your new bones
        if (GUILayout.Button("Copy Bones"))
        {
            dynamicBoneColliderMerger.PerformCopy();
            dynamicBoneMerger.PerformCopy();
        }
    }
}
