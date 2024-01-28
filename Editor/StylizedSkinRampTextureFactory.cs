using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace StylizedSkinRampTexture.Editor
{
    public static class StylizedSkinRampTextureFactory 
    {
        [MenuItem("Assets/Create/Stylized Skin Ramp Texture", priority = 0)]
        static void CreateStylizedSkinRampTexture()
        {
            CreateAction createAction = ScriptableObject.CreateInstance<CreateAction>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,createAction,
                $"new skinRamp.{StylizedSkinRampTextureImporter.Extension}",
                null,
                null);
        }
        
        
        public class CreateAction : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var json = JsonUtility.ToJson(StylizedSkinRampTextureData.Default(),true);
                File.WriteAllLines(pathName,new []{json});
                AssetDatabase.Refresh();
            }
        }
    }
    
}