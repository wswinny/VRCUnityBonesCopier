using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VRCAvatarTools
{
    public class GenericEntityMerge<T>
    {
        private GameObject baseAvatar;
        private GameObject newAvatar;
        
        public List<T> baseAvatarComponents;
        public List<Component> newAvatarComponents;
    
        public List<String> newAvatarComponentNames = new List<string>();
        public int[] selectedComponentOptions;

        public void CalculateCopyableAssets(GameObject baseAvatar, GameObject newAvatar)
        {
            this.baseAvatar = baseAvatar;
            this.newAvatar = newAvatar;
            
            //get bones and store them for avatars
            baseAvatarComponents = GetTypedComponentsFromBaseAvatar(baseAvatar);
            newAvatarComponents = GetComponentsForNewAvatar(newAvatar);
            
            //make parallel array to new avatar bones of bone options 
            newAvatarComponentNames = new List<string>();
            newAvatarComponentNames.Add("Do Nothing");
            foreach (Component comp in newAvatarComponents)
            {
                newAvatarComponentNames.Add(comp.name);
            }
            
            //create a place to store the selections from the dropdowns
            selectedComponentOptions = new int[baseAvatarComponents.Count];
        }

        public void PerformCopy()
        {
            for (var i = 0; i < selectedComponentOptions.Length; i++)
            {
                if(selectedComponentOptions[i] == 0)
                    continue;

                var copyFrom = baseAvatarComponents[i];
                var copyTo = newAvatarComponents[selectedComponentOptions[i] - 1];

                var newCollider = (T)Convert.ChangeType(copyTo.gameObject.AddComponent(typeof(T)), typeof(T));
                CopyGenericType(copyFrom, newCollider);
            }
        }

        public void PaintGUI()
        {
            if (baseAvatarComponents != null)
            {
                //create the dropdowns for bone selection with selectable bones from base avatar
                for (int i = 0; i < baseAvatarComponents.Count; i++)
                {
                    selectedComponentOptions[i] = EditorGUILayout.Popup(
                        baseAvatarComponents[i].GetType().GetProperty("name")?.GetValue(baseAvatarComponents[i]).ToString(),
                        selectedComponentOptions[i],
                        newAvatarComponentNames.ToArray());
                }
            }
        }

        private static List<Component> GetComponentsForNewAvatar(GameObject avatar)
        {
            return avatar.GetComponentsInChildren(typeof(Transform)).ToList();
        }
    
        private static List<T> GetTypedComponentsFromBaseAvatar(GameObject avatar)
        {
            List<T> typedComponents = new List<T>();
            Component[] componentsOfGenericType = avatar.GetComponentsInChildren(typeof(T));

            foreach (var component in componentsOfGenericType)
            {
                if (component is T)
                {
                    typedComponents.Add((T)Convert.ChangeType(component, typeof(T)));
                }
            }

            return typedComponents;
        }
        
        private static void CopyGenericType(T copyFrom, T copyTo)
        {
            EditorUtility.CopySerialized(copyFrom as Object, copyTo as Object);
        }
    }
}