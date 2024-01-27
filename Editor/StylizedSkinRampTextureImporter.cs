using System;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Callbacks;
using UnityEngine;

namespace StylizedSkinRampTexture.Editor
{
    [ScriptedImporter(0,StylizedSkinRampTextureImporter.Extension)]
    public class StylizedSkinRampTextureImporter : ScriptedImporter
    {
        public const string Extension = "skinramp";

        public bool mip = true;
        public TextureFormat format = TextureFormat.RGBA32;
        public FilterMode filterMode = FilterMode.Bilinear;
        public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        
        public bool isDataDirty;
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var data = StylizedSkinRampTextureData.Load(assetPath);
            var texture = data.GenerateTexture(format, mip, SettingsAfterGenerateTexture);
            ctx.AddObjectToAsset("MainObject",texture);
        }

        void SettingsAfterGenerateTexture(Texture2D texture)
        {
            texture.filterMode = filterMode;
            texture.wrapMode = wrapMode;
        }
        
        
        [OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var path = AssetDatabase.GetAssetPath(instanceID);
            if (!(AssetImporter.GetAtPath(path) as StylizedSkinRampTextureImporter)) return false;
            EditorUtility.OpenPropertyEditor(AssetDatabase.LoadAssetAtPath<Texture2D>(path));
            return true;
        }
    }
}