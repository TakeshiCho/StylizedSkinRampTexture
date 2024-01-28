using UnityEditor.AssetImporters;
using UnityEngine;

namespace StylizedSkinRampTexture.Editor
{
    [ScriptedImporter(0,StylizedSkinRampTextureImporter.Extension)]
    public class StylizedSkinRampTextureImporter : ScriptedImporter
    {
        public const string Extension = "skinramp";

        public bool mip = true;
        public TextureFormat format = TextureFormat.RGB24;
        public FilterMode filterMode = FilterMode.Bilinear;
        public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        
        public bool isDataDirty;
        
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var data = StylizedSkinRampTextureData.Load(assetPath);
            var texture = data.GenerateTexture(format, mip, SettingsAfterGenerateTexture);
            ctx.AddObjectToAsset("MainObject",texture);
            data.Unload();
        }

        void SettingsAfterGenerateTexture(Texture2D texture)
        {
            texture.filterMode = filterMode;
            texture.wrapMode = wrapMode;
        }
    }
}